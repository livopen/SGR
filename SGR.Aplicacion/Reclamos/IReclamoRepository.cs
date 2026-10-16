namespace SGR.Aplicacion.Reclamos;
using SGR.Dominio.Reclamos;
public interface IReclamoRepository
{
    void  Agregar(Reclamo reclamo);
    void Modificar(Reclamo reclamo);
    void Eliminar(Guid id);
    Reclamo? ObtenerPorId(Guid id);
    IEnumerable<Reclamo> ObtenerTodos();
}
