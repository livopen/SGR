using SGR.Aplicacion.Actuaciones;
using SGR.Dominio.Actuaciones;
using SGR.Infraestructura.Comun;

namespace SGR.Infraestructura.Actuaciones;

/// <summary>Repositorio de actuaciones respaldado por un archivo JSON Lines (.txt).</summary>
public sealed class ActuacionTxtRepository : IActuacionRepository
{
    private readonly string _ruta;

    public ActuacionTxtRepository(string? ruta = null)
    {
        _ruta = ruta ?? Path.Combine(AppContext.BaseDirectory, "Datos", "actuaciones.txt");
    }

    public void Agregar(Actuacion actuacion)
    {
        ArgumentNullException.ThrowIfNull(actuacion);
        var actuaciones = ObtenerTodos().ToList();
        if (actuaciones.Any(a => a.Id == actuacion.Id))
            throw new RepositorioException($"Ya existe una actuación con Id {actuacion.Id}.");
        actuaciones.Add(actuacion);
        Guardar(actuaciones);
    }

    public Actuacion? ObtenerPorId(Guid id)
        => ObtenerTodos().FirstOrDefault(a => a.Id == id);

    public IEnumerable<Actuacion> ObtenerTodos()
        => ArchivoJsonLineas.Leer<ActuacionRegistro>(_ruta).Select(Reconstruir).ToList();

    public IEnumerable<Actuacion> ObtenerPorReclamoId(Guid reclamoId)
        => ObtenerTodos().Where(a => a.ReclamoId == reclamoId).ToList();

    public void Modificar(Actuacion actuacion)
    {
        ArgumentNullException.ThrowIfNull(actuacion);
        var actuaciones = ObtenerTodos().ToList();
        int indice = actuaciones.FindIndex(a => a.Id == actuacion.Id);
        if (indice < 0) throw new RepositorioException($"No existe la actuación {actuacion.Id} para modificar.");
        actuaciones[indice] = actuacion;
        Guardar(actuaciones);
    }

    public void Eliminar(Guid id)
    {
        var actuaciones = ObtenerTodos().ToList();
        int eliminados = actuaciones.RemoveAll(a => a.Id == id);
        if (eliminados == 0) throw new RepositorioException($"No existe la actuación {id} para eliminar.");
        Guardar(actuaciones);
    }

    /// <summary>Elimina todas las actuaciones de un reclamo (útil para la baja en cascada).</summary>
    public void EliminarPorReclamoId(Guid reclamoId)
    {
        var actuaciones = ObtenerTodos().ToList();
        actuaciones.RemoveAll(a => a.ReclamoId == reclamoId);
        Guardar(actuaciones);
    }

    private void Guardar(IEnumerable<Actuacion> actuaciones)
        => ArchivoJsonLineas.Sobrescribir(_ruta, actuaciones.Select(a => new ActuacionRegistro
        {
            Id = a.Id,
            ReclamoId = a.ReclamoId,
            Tipo = a.Tipo.ToString(),
            Detalle = ReconstruccionEntidad.TextoValueObject(a.Detalle),
            FechaCreacion = a.FechaCreacion,
            FechaUltimaModificacion = a.FechaUltimaModificacion,
            UsuarioUltimoCambio = a.UsuarioUltimoCambio
        }));

    private static Actuacion Reconstruir(ActuacionRegistro a)
    {
        var datos = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
        {
            ["id"] = a.Id, ["reclamoId"] = a.ReclamoId,
            ["tipo"] = a.Tipo, ["detalle"] = a.Detalle,
            ["fechaCreacion"] = a.FechaCreacion,
            ["fechaUltimaModificacion"] = a.FechaUltimaModificacion,
            ["usuarioUltimoCambio"] = a.UsuarioUltimoCambio
        };
        return ReconstruccionEntidad.Reconstruir<Actuacion>(datos);
    }

    private sealed class ActuacionRegistro
    {
        public Guid Id { get; set; }
        public Guid ReclamoId { get; set; }
        public string Tipo { get; set; } = "";
        public string Detalle { get; set; } = "";
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaUltimaModificacion { get; set; }
        public Guid UsuarioUltimoCambio { get; set; }
    }
}
