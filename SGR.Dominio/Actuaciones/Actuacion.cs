using System;

namespace SGR.Dominio.Actuaciones;

public class Actuacion
{
    private Guid Id;
    private Guid ReclamoId;
    private TipoActuacion Tipo;
    private DetalleActuacion Detalle;
    private DateTime FechaCreacion;
    private DateTime FechaUltimaModificacion;
    private Guid UsuarioUltimoCambio;

}
