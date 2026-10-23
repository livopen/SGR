using System.Text.Json;
using SGR.Aplicacion.Actuaciones;
using SGR.Dominio.Actuaciones;

namespace SGR.Infraestructura.Actuaciones;

public sealed class ActuacionTxtRepository : IActuacionRepository
{
    private readonly string _ruta;
    private readonly JsonSerializerOptions _opciones = new() { WriteIndented = false };

    public ActuacionTxtRepository(string? ruta = null)
    {
        _ruta = ruta ?? Path.Combine(AppContext.BaseDirectory, "Datos", "actuaciones.txt");
        CrearArchivoSiNoExiste();
    }

    public void Agregar(Actuacion actuacion)
    {
        ArgumentNullException.ThrowIfNull(actuacion);

        var actuaciones = ObtenerTodos().ToList();
        if (actuaciones.Any(a => a.Id == actuacion.Id))
            throw new InvalidOperationException($"Ya existe una actuación con Id {actuacion.Id}.");

        actuaciones.Add(actuacion);
        GuardarTodos(actuaciones);
    }

    public void Modificar(Actuacion actuacion)
    {
        ArgumentNullException.ThrowIfNull(actuacion);
        var actuaciones = ObtenerTodos().ToList();
        int indice = actuaciones.FindIndex(a => a.Id == actuacion.Id);

        if (indice < 0)
            throw new InvalidOperationException($"No existe la actuación {actuacion.Id} para modificar.");

        actuaciones[indice] = actuacion;
        GuardarTodos(actuaciones);
    }

    public void Eliminar(Guid id)
    {
        var actuaciones = ObtenerTodos().ToList();
        int eliminados = actuaciones.RemoveAll(a => a.Id == id);

        if (eliminados == 0)
            throw new InvalidOperationException($"No existe la actuación {id} para eliminar.");

        GuardarTodos(actuaciones);
    }

    public IEnumerable<Actuacion> ObtenerActuacionesIdReclamo(Guid id)
        => ObtenerTodos().Where(a => a.ReclamoId == id).ToList();

    private IEnumerable<Actuacion> ObtenerTodos()
    {
        if (!File.Exists(_ruta))
            return Enumerable.Empty<Actuacion>();

        var resultado = new List<Actuacion>();
        foreach (string linea in File.ReadLines(_ruta))
        {
            if (string.IsNullOrWhiteSpace(linea))
                continue;

            var registro = JsonSerializer.Deserialize<ActuacionRegistro>(linea, _opciones)
                ?? throw new InvalidDataException("No se pudo leer un registro de actuación.");

            // Al leer se usa el Factory Method de Dominio.
            resultado.Add(Actuacion.Reconstruir(
                registro.Id,
                registro.ReclamoId,
                Enum.Parse<TipoActuacion>(registro.Tipo),
                new DetalleActuacion(registro.Detalle),
                registro.FechaCreacion,
                registro.FechaUltimaModificacion,
                registro.UsuarioUltimoCambio));
        }

        return resultado;
    }

    private void GuardarTodos(IEnumerable<Actuacion> actuaciones)
    {
        CrearArchivoSiNoExiste();
        var lineas = actuaciones.Select(a => JsonSerializer.Serialize(new ActuacionRegistro
        {
            Id = a.Id,
            ReclamoId = a.ReclamoId,
            Tipo = a.Tipo.ToString(),
            Detalle = a.Detalle.Descripcion,
            FechaCreacion = a.FechaCreacion,
            FechaUltimaModificacion = a.FechaUltimaModificacion,
            UsuarioUltimoCambio = a.UsuarioUltimoCambio
        }, _opciones));

        File.WriteAllLines(_ruta, lineas);
    }

    private void CrearArchivoSiNoExiste()
    {
        string? directorio = Path.GetDirectoryName(_ruta);
        if (!string.IsNullOrEmpty(directorio))
            Directory.CreateDirectory(directorio);

        if (!File.Exists(_ruta))
            File.WriteAllText(_ruta, string.Empty);
    }

    private sealed class ActuacionRegistro
    {
        public ActuacionRegistro() { }
        public Guid Id { get; set; }
        public Guid ReclamoId { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Detalle { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaUltimaModificacion { get; set; }
        public Guid UsuarioUltimoCambio { get; set; }
    }
}
