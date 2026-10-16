using SGR.Dominio.Actuaciones;

namespace SGR.Aplicacion.Actuaciones;

public interface IActuacionRepository
{
    void  Agregar(Actuacion actuacion);
    void Modificar(Actuacion actuacion);
    void Eliminar(Guid id);
    IEnumerable<Actuacion> ObtenerActuacionesIdReclamo(Guid id);
}
