using System;
using SGR.Dominio.Comun;
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
            throw new ArgumentException("La fecha de última modificación no puede ser anterior a la fecha de creación", nameof(fechaUltimaModificacion));
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
}
