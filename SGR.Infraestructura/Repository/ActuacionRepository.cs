using SGR.Aplicacion.Actuaciones;
using SGR.Dominio.Actuaciones;

namespace SGR.Infraestructura;
public class ActuacionRepository : IActuacionRepository
{
    private readonly SgrContext _context;

    public ActuacionRepository(SgrContext context)
    {
        _context = context;
    }

    public void Agregar(Actuacion actuacion)
    {
        _context.Actuaciones.Add(actuacion);
    }

    public void Modificar(Actuacion actuacion)
    {
        _context.Actuaciones.Update(actuacion);
    }

    public void Eliminar(Guid id)
    {
        var actuacion = _context.Actuaciones.Find(id);
        if (actuacion is not null)
        {
            _context.Actuaciones.Remove(actuacion);
        }
    }
    public IEnumerable<Actuacion> ObtenerActuacionesIdReclamo(Guid ReclamoIdABuscar)
    {
        return _context.Actuaciones.Where(a => a.ReclamoId == ReclamoIdABuscar).ToList();
    }
    public IEnumerable<Actuacion> ObtenerTodos(){

        return _context.Actuaciones.ToList();
    }
}