using SGR.Dominio.Comun;

namespace SGR.Dominio.Actuaciones;

public class Actuacion
{
    public Guid Id { get; private set; }
    public Guid ReclamoId { get; private set; }
    public TipoActuacion Tipo { get; private set; }
    public DetalleActuacion Detalle { get; private set; }
    public DateTime FechaCreacion { get; private set; }
    public DateTime FechaUltimaModificacion { get; private set; }
    public Guid UsuarioUltimoCambio { get; private set; }

    public Actuacion(Guid reclamoId, TipoActuacion tipo, DetalleActuacion detalle, Guid usuarioUltimoCambio)
    {
        if (reclamoId == Guid.Empty)
        throw new DominioException("El Id del reclamo es obligatorio.");
        if (usuarioUltimoCambio == Guid.Empty)
        throw new DominioException("El Id del usuario es obligatorio.");
        Id = Guid.NewGuid();
        ReclamoId = reclamoId;
        Tipo = tipo;
        Detalle = detalle ?? throw new DominioException("El detalle de la actuación no puede ser nulo");
        FechaCreacion = DateTime.UtcNow;
        FechaUltimaModificacion = FechaCreacion;
        UsuarioUltimoCambio = usuarioUltimoCambio;
    }
    public void Modificar(TipoActuacion nuevoTipo, DetalleActuacion nuevoDetalle, Guid idUsuario)
    {
        if (nuevoDetalle == null)
            throw new DominioException("El detalle de la actuación no puede ser nulo");

        Tipo = nuevoTipo;
        Detalle = nuevoDetalle;
        
        // Actualizamos los datos de auditoría, tal como exige el dominio
        FechaUltimaModificacion = DateTime.UtcNow;
        UsuarioUltimoCambio = idUsuario;
    }
    private Actuacion(Guid id, Guid reclamoId, TipoActuacion tipo, DetalleActuacion detalle, DateTime fechaCreacion, DateTime fechaUltimaModificacion, Guid usuarioUltimoCambio)
    {
        if (fechaUltimaModificacion < fechaCreacion)
            throw new DominioException("La fecha de última modificación no puede ser anterior a la fecha de creación");
        Id = id;
        ReclamoId = reclamoId;
        Tipo = tipo;
        Detalle = detalle ?? throw new DominioException("El detalle de la actuación no puede ser nulo");
        FechaCreacion = fechaCreacion;
        FechaUltimaModificacion = fechaUltimaModificacion;
        UsuarioUltimoCambio = usuarioUltimoCambio;
    } // Constructor privado para EF Core

    public static Actuacion Reconstruir(Guid id, Guid reclamoId, TipoActuacion tipo, DetalleActuacion detalle, DateTime fechaCreacion, DateTime fechaUltimaModificacion, Guid usuarioUltimoCambio)
    {
        return new Actuacion(id, reclamoId, tipo, detalle, fechaCreacion, fechaUltimaModificacion, usuarioUltimoCambio);
    }
}
