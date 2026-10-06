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
        Id = Guid.NewGuid();
        ReclamoId = reclamoId;
        Tipo = tipo ?? throw new DominioException("El tipo de actuación no puede ser nulo");
        Detalle = detalle ?? throw new ArgumentNullException(nameof(detalle));
        FechaCreacion = DateTime.UtcNow;
        FechaUltimaModificacion = FechaCreacion;
        UsuarioUltimoCambio = usuarioUltimoCambio;
    }
}
