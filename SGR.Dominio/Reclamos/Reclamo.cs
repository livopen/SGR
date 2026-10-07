using System;
<<<<<<< HEAD
using SGR.Dominio.Comun;
using SGR.Dominio.Actuaciones;
=======
using SGR.Dominio.Comun ;
>>>>>>> e32357f5bfc8ce27a5e14382654c52b8843df4f6
namespace SGR.Dominio.Reclamos;

public class Reclamo
{
    public Guid Id {get; private set; }
    public Asunto Asunto { get; private set; }
    public DateTime FechaCreacion { get; private set; }
    public DateTime FechaUltimaModificacion { get; private set; }
    public Guid UsuarioUltimoCambio { get; private set; }
    public EstadoReclamo Estado { get; private set; }

    public Reclamo(Asunto asunto, Guid usuarioUltimoCambio)
    {
        Id = Guid.NewGuid();
        Asunto = asunto ?? throw new DominioException("El asunto no puede ser nulo");
        FechaCreacion = DateTime.UtcNow;
        FechaUltimaModificacion = FechaCreacion;
        UsuarioUltimoCambio = usuarioUltimoCambio;
        Estado = EstadoReclamo.Recibido;
    }
    
    private Reclamo(Guid id, Asunto asunto, DateTime fechaCreacion, DateTime fechaUltimaModificacion, Guid usuarioUltimoCambio, EstadoReclamo estado)
    {
        if (fechaUltimaModificacion < fechaCreacion)
<<<<<<< HEAD
            throw new DominioException("La fecha de última modificación no puede ser anterior a la fecha de creación");
=======
            throw new DominioException("La fecha de última modificación no puede ser anterior a la fecha de creación", nameof(fechaUltimaModificacion));
>>>>>>> e32357f5bfc8ce27a5e14382654c52b8843df4f6
        Id = id;
        Asunto = asunto ?? throw new DominioException("El asunto no puede ser nulo");
        FechaCreacion = fechaCreacion;
        FechaUltimaModificacion = fechaUltimaModificacion;
        UsuarioUltimoCambio = usuarioUltimoCambio;
        Estado = estado;
    } // Constructor privado para EF Core
    public static Reclamo Reconstruir(Guid id, Asunto asunto, DateTime fechaCreacion, DateTime fechaUltimaModificacion, Guid usuarioUltimoCambio, EstadoReclamo estado)
    {
        return new Reclamo(id, asunto, fechaCreacion, fechaUltimaModificacion, usuarioUltimoCambio, estado);
    }
    
    public void ModificarAsunto(Asunto nuevoAsunto, Guid usuarioUltimoCambio)
    {
        if (nuevoAsunto == null)
            throw new DominioException("El nuevo asunto no puede ser nulo");
        Asunto = nuevoAsunto;
        FechaUltimaModificacion = DateTime.UtcNow;
        UsuarioUltimoCambio = usuarioUltimoCambio;
    }

    public void CambiarEstado(EstadoReclamo nuevoEstado, Guid usuarioUltimoCambio)
    {
        Estado = nuevoEstado;
        FechaUltimaModificacion = DateTime.UtcNow;
        UsuarioUltimoCambio = usuarioUltimoCambio;
    }
  public bool ActualizarEstado(TipoActuacion? ultimoTipo, Guid idUsuario)
{
    EstadoReclamo nuevoEstadoEsperado;
    if (ultimoTipo == null)
    {
        nuevoEstadoEsperado = EstadoReclamo.Recibido;
    }
    else
    {
        switch (ultimoTipo)
        {
            case TipoActuacion.Inspeccion:
                nuevoEstadoEsperado = EstadoReclamo.EnInspeccion;
                break;
            case TipoActuacion.OrdenDeTrabajo:
                nuevoEstadoEsperado = EstadoReclamo.EnEjecucion;
                break;
            case TipoActuacion.TrabajoRealizado:
                nuevoEstadoEsperado = EstadoReclamo.Resuelto;
                break;
            case TipoActuacion.Archivo:
                nuevoEstadoEsperado = EstadoReclamo.Cerrado;
                break;
            default:
                // Para Observacion o RespuestaAlVecino, no hay cambios
                return false; 
        }
    }
    if (Estado != nuevoEstadoEsperado)
    {
        CambiarEstado(nuevoEstadoEsperado, idUsuario);
        return true; 
    }

    return false; 
}
  
}
