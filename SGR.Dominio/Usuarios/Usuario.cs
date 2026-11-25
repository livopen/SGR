using System;
using SGR.Dominio;
using SGR.Dominio.Comun;

namespace SGR.Dominio.Usuarios;

public class Usuario
{
    public string? _nombre {get; private set;}
    public Guid _id {get; private set;}
    public CorreoElectronico _correoElectronico {get; private set;} = null!;
    public string? _contrasenaHash {get; private set;}
    public bool _esAdministrador {get; private set;}
    private HashSet<Permiso> _permisos=new();

    public IReadOnlyCollection<Permiso> Permisos => _permisos;

    private Usuario(){}

    public Usuario(string nombre, CorreoElectronico correoElectronico, string contrasenaHash, bool esAdministrador)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new DominioException("El nombre del usuario no puede estar vacío.");

        _correoElectronico = correoElectronico ?? throw new DominioException("El correo electrónico no puede ser nulo.");
        if (contrasenaHash is null)
            throw new DominioException("La contraseña no puede ser nula.");
        if(string.IsNullOrWhiteSpace(contrasenaHash))
            throw new DominioException("La contraseña no puede estar vacía.");

        _id = Guid.NewGuid();
        _nombre = nombre.Trim();
        _contrasenaHash = contrasenaHash;
        _esAdministrador = esAdministrador;
    }
    public static Usuario UsuarioFactoryMethod(Guid id, string nombre, CorreoElectronico correo, string contraseniaHash,bool esAdministrador)
    {
        if (id == Guid.Empty)
            throw new DominioException("El Id del usuario no puede estar vacío.");
        if (string.IsNullOrWhiteSpace(nombre))
            throw new DominioException("El nombre del usuario no puede estar vacío.");
        if (correo is null)
            throw new DominioException("El correo electrónico no puede ser nulo.");
        if (string.IsNullOrWhiteSpace(contraseniaHash))
            throw new DominioException("La contraseña no puede estar vacía.");

        var usuario = new Usuario();
        
        usuario._id = id;
        usuario._nombre = nombre.Trim();
        usuario._correoElectronico = correo;
        usuario._contrasenaHash = contraseniaHash;
        usuario._esAdministrador = esAdministrador;

        return usuario;
    }
    public void AsignarPermiso(Permiso permiso)
    {
        if (!Enum.IsDefined(typeof(Permiso), permiso))
            throw new DominioException("El permiso no es válido.");
        
        _permisos.Add(permiso);
    }
    public void QuitarPermiso(Permiso permiso)
    {
        if (!Enum.IsDefined(typeof(Permiso), permiso))
            throw new DominioException("El permiso no es válido.");
        
        _permisos.Remove(permiso);
    }

    public bool tienePermiso (Permiso permiso)
    {
        return _permisos.Contains(permiso);
    }

    
    public void ReemplazarPermisos(List<Permiso> nuevosPermisos)
    {
        _permisos.Clear(); // Limpiamos sobre la lista privada 
        if (nuevosPermisos != null) 
        {
            foreach (var permiso in nuevosPermisos) 
            {
                if (!_permisos.Contains(permiso)) 
                {
                    _permisos.Add(permiso); 
                }
            }
        }
    }

    public void ActualizarDatos(string nuevoNombre, string nuevoCorreo)
    {
        if (string.IsNullOrWhiteSpace(nuevoNombre))
            throw new DominioException("El nombre no puede estar vacío.");

        var correoElectronico = new CorreoElectronico(nuevoCorreo);
        _nombre = nuevoNombre;
        _correoElectronico = correoElectronico;
    }

    public void Cambiarcontrasenia(string nuevacontraseniaHash)
    {
        if (string.IsNullOrWhiteSpace(nuevacontraseniaHash))
            throw new DominioException("La contrasenia hash no puede estar vacía.");
        
        _contrasenaHash = nuevacontraseniaHash;
    }

    public bool EsAdministrador()
    {
        return _esAdministrador;
    }
}
