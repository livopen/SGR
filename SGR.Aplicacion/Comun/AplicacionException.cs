namespace SGR.Aplicacion.Comun;
public class AplicacionException : System.Exception
{
    public AplicacionException() { }
    public AplicacionException(string message) : base(message) { }
    public AplicacionException(string message, System.Exception inner) : base(message, inner) { }

}