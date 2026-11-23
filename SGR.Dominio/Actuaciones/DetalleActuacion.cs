using SGR.Dominio.Comun;

namespace SGR.Dominio.Actuaciones;

public record class DetalleActuacion
{
    public string Descripcion { get; private set; }

    private DetalleActuacion()
    {
        Descripcion = string.Empty;
    }

    public DetalleActuacion(string descripcion)
    {
        if (string.IsNullOrWhiteSpace(descripcion))
            throw new DominioException("La descripción no puede estar vacía");
        Descripcion = descripcion;
    }
}
