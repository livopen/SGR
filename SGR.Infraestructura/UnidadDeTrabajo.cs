using SGR.Aplicacion;

namespace SGR.Infraestructura;

public class UnidadDeTrabajo : IUnidadDeTrabajo
{
    private readonly SgrContext _context;

    public UnidadDeTrabajo(SgrContext context)
    {
        _context = context;
    }

    public void Guardar()
    {
        _context.SaveChanges();
    }
}