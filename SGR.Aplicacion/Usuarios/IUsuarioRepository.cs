using SGR.Dominio.Usuarios;
namespace SGR.Aplicacion.Usuarios;
public interface IUsuarioRepository
{
    void Agregar(Usuario usuario);
    Usuario? ObtenerPorCorreo(CorreoElectronico correo);
    Usuario? ObtenerPorId(Guid id);
    List<Usuario> ObtenerTodos();
    void Modificar(Usuario usuario);
    void Eliminar(Guid id);
}