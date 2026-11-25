using SGR.Aplicacion.Usuarios;
using SGR.Dominio.Usuarios;
namespace SGR.Infraestructura.Repository;
public class UsuarioRepository : IUsuarioRepository
{
    private readonly SgrContext _context;
    public UsuarioRepository(SgrContext context)
    {
        _context = context;
    
    }
    public void Agregar(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
    }

    public Usuario? ObtenerPorCorreo(CorreoElectronico correo)
    {
        return _context.Usuarios
            .FirstOrDefault(u => u._correoElectronico.Valor == correo.Valor);
    }
    public Usuario? ObtenerPorId(Guid id)
    {
        return _context.Usuarios.Find(id);
    }
    public List<Usuario> ObtenerTodos()
    {
        return _context.Usuarios.ToList();
    }
    public void Modificar(Usuario usuario)
    {
        _context.Usuarios.Update(usuario);
    }
    public void Eliminar(Guid id)
    {
        var usuario = _context.Usuarios.Find(id);
        
        if (usuario != null)
        {
            _context.Usuarios.Remove(usuario);
        }
        else
        {
            throw new Exception($"No se puede eliminar el usuario. El ID {id} no existe.");
        }
    }
}