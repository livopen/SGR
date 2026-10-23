using System.Text.Json;
using SGR.Aplicacion.Reclamos;
using SGR.Dominio.Reclamos;
namespace SGR.Infraestructura.Reclamos;

public sealed class ReclamoTxtRepository : IReclamoRepository
{
    private readonly string _ruta;
    private readonly JsonSerializerOptions _opciones = new() { WriteIndented = false };

    public ReclamoTxtRepository(string? ruta = null)
    {
        _ruta = ruta ?? Path.Combine(AppContext.BaseDirectory, "Datos", "reclamos.txt");
        CrearArchivoSiNoExiste();
    }

    public void Agregar(Reclamo reclamo)
    {
        ArgumentNullException.ThrowIfNull(reclamo);

        var reclamos = ObtenerTodos().ToList();
        if (reclamos.Any(r => r.Id == reclamo.Id))
            throw new InvalidOperationException($"Ya existe un reclamo con Id {reclamo.Id}.");

        reclamos.Add(reclamo);
        GuardarTodos(reclamos);
    }

    public Reclamo? ObtenerPorId(Guid id)
        => ObtenerTodos().FirstOrDefault(r => r.Id == id);

    public IEnumerable<Reclamo> ObtenerTodos()
    {
        if (!File.Exists(_ruta))
            return Enumerable.Empty<Reclamo>();

        var resultado = new List<Reclamo>();
        foreach (string linea in File.ReadLines(_ruta))
        {
            if (string.IsNullOrWhiteSpace(linea))
                continue;

            var registro = JsonSerializer.Deserialize<ReclamoRegistro>(linea, _opciones)
                ?? throw new InvalidDataException("No se pudo leer un registro de reclamo.");

                resultado.Add(Reclamo.Reconstruir(
                registro.Id,
                new Asunto(registro.Asunto),
                registro.FechaCreacion,
                registro.FechaUltimaModificacion,
                registro.UsuarioUltimoCambio,
                Enum.Parse<EstadoReclamo>(registro.Estado)));
        }

        return resultado;
    }

    public void Modificar(Reclamo reclamo)
    {
        ArgumentNullException.ThrowIfNull(reclamo);
        var reclamos = ObtenerTodos().ToList();
        int indice = reclamos.FindIndex(r => r.Id == reclamo.Id);

        if (indice < 0)
            throw new InvalidOperationException($"No existe el reclamo {reclamo.Id} para modificar.");

        reclamos[indice] = reclamo;
        GuardarTodos(reclamos);
    }

    public void Eliminar(Guid id)
    {
        var reclamos = ObtenerTodos().ToList();
        int eliminados = reclamos.RemoveAll(r => r.Id == id);

        if (eliminados == 0)
            throw new InvalidOperationException($"No existe el reclamo {id} para eliminar.");

        GuardarTodos(reclamos);
    }

    private void GuardarTodos(IEnumerable<Reclamo> reclamos)
    {
        CrearArchivoSiNoExiste();
        var lineas = reclamos.Select(r => JsonSerializer.Serialize(new ReclamoRegistro
        {
            Id = r.Id,
            Asunto = r.Asunto.Descripcion,
            FechaCreacion = r.FechaCreacion,
            FechaUltimaModificacion = r.FechaUltimaModificacion,
            UsuarioUltimoCambio = r.UsuarioUltimoCambio,
            Estado = r.Estado.ToString()
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

    private sealed class ReclamoRegistro
    {
        public ReclamoRegistro() { }
        public Guid Id { get; set; }
        public string Asunto { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaUltimaModificacion { get; set; }
        public Guid UsuarioUltimoCambio { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}
