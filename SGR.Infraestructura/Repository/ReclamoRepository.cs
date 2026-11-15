using SGR.Aplicacion.Reclamos;
using SGR.Dominio.Reclamos;
using SGR.Dominio.Comun;

namespace SGR.Infraestructura;
public class ReclamoRepository : IReclamoRepository
{
    private readonly SgrContext _context;

    public ReclamoRepository(SgrContext context)
    {
        _context = context;
    }

    public void Agregar(Reclamo reclamo)
    {
        _context.Reclamos.Add(reclamo);
    }

    public void Modificar(Reclamo reclamo)
    {
        _context.Reclamos.Update(reclamo);
    }

    public void Eliminar(Guid id)
    {
        var reclamo = _context.Reclamos.Find(id);
        if (reclamo is not null)
        {
            _context.Reclamos.Remove(reclamo);
        }
    }
    public Reclamo? ObtenerPorId(Guid id)
    {
        return _context.Reclamos.Find(id)
        ?? throw new DominioException($"No se encontró el Reclamo con ID {id}");
    }
    public IEnumerable<Reclamo> ObtenerTodos(){

        return _context.Reclamos.ToList();
    }
}