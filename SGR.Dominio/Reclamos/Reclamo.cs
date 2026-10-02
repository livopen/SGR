using System;

namespace SGR.Dominio.Reclamos;

public class Reclamo
{
    private Guid Id;
    private Asunto Asunto;
    private DateTime FechaCreacion;
    private DateTime FechaUltimaModificacion;
    private Guid UsuarioUltimoCambio;
    private EstadoReclamo Estado;

}
