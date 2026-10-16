namespace SGR.Aplicacion.Comun;
public class EntidadNoEncontradaException : System.Exception
{
    public EntidadNoEncontradaException() { }
    public EntidadNoEncontradaException(string message) : base(message) { }
    public EntidadNoEncontradaException(string message, System.Exception inner) : base(message, inner) { }

}