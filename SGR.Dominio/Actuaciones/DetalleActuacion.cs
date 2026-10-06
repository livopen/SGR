using SGR.Dominio.Comun;

namespace SGR.Dominio.Actuaciones;

public record class DetalleActuacion
{
    public string Descripcion { get; private set; }
    public DetalleActuacion(string descripcion)
    {
        if (string.IsNullOrWhiteSpace(descripcion) || descripcion != " ")
            throw new DominioException("La descripción no puede estar vacía");
        Descripcion = descripcion;
    }
}
