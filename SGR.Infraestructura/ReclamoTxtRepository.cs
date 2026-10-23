
using System.Text.Json;
using SGR.Aplicacion.Reclamos;
using SGR.Dominio.Reclamos;
using SGR.Infraestructura.Comun;

namespace SGR.Infraestructura.Reclamos;

/// <summary>Repositorio de reclamos respaldado por un archivo JSON Lines (.txt).</summary>
public sealed class ReclamoTxtRepository : IReclamoRepository
{
    private readonly string _ruta;

    public ReclamoTxtRepository(string? ruta = null)
    {
        _ruta = ruta ?? Path.Combine(AppContext.BaseDirectory, "Datos", "reclamos.txt");
    }

    public void Agregar(Reclamo reclamo)
    {
        ArgumentNullException.ThrowIfNull(reclamo);
        var reclamos = ObtenerTodos().ToList();
        if (reclamos.Any(r => r.Id == reclamo.Id))
            throw new RepositorioException($"Ya existe un reclamo con Id {reclamo.Id}.");
        reclamos.Add(reclamo);
        Guardar(reclamos);
    }

    public Reclamo? ObtenerPorId(Guid id)
        => ObtenerTodos().FirstOrDefault(r => r.Id == id);

    public IEnumerable<Reclamo> ObtenerTodos()
        => ArchivoJsonLineas.Leer<ReclamoRegistro>(_ruta).Select(Reconstruir).ToList();

    public void Modificar(Reclamo reclamo)
    {
        ArgumentNullException.ThrowIfNull(reclamo);
        var reclamos = ObtenerTodos().ToList();
        int indice = reclamos.FindIndex(r => r.Id == reclamo.Id);
        if (indice < 0) throw new RepositorioException($"No existe el reclamo {reclamo.Id} para modificar.");
        reclamos[indice] = reclamo;
        Guardar(reclamos);
    }

    public void Eliminar(Guid id)
    {
        var reclamos = ObtenerTodos().ToList();
        int eliminados = reclamos.RemoveAll(r => r.Id == id);
        if (eliminados == 0) throw new RepositorioException($"No existe el reclamo {id} para eliminar.");
        Guardar(reclamos);
    }

    private void Guardar(IEnumerable<Reclamo> reclamos)
        => ArchivoJsonLineas.Sobrescribir(_ruta, reclamos.Select(r => new ReclamoRegistro
        {
            Id = r.Id,
            Asunto = ReconstruccionEntidad.TextoValueObject(r.Asunto),
            FechaCreacion = r.FechaCreacion,
            FechaUltimaModificacion = r.FechaUltimaModificacion,
            UsuarioUltimoCambio = r.UsuarioUltimoCambio,
            Estado = r.Estado.ToString()
        }));

    private static Reclamo Reconstruir(ReclamoRegistro r)
    {
        var datos = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
        {
            ["id"] = r.Id, ["asunto"] = r.Asunto,
            ["fechaCreacion"] = r.FechaCreacion,
            ["fechaUltimaModificacion"] = r.FechaUltimaModificacion,
            ["usuarioUltimoCambio"] = r.UsuarioUltimoCambio,
            ["estado"] = r.Estado
        };
        return ReconstruccionEntidad.Reconstruir<Reclamo>(datos);
    }

    private sealed class ReclamoRegistro
    {
        public Guid Id { get; set; }
        public string Asunto { get; set; } = "";
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaUltimaModificacion { get; set; }
        public Guid UsuarioUltimoCambio { get; set; }
        public string Estado { get; set; } = "";
    }
}
