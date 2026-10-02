namespace SGR.Aplicacion.Autorizacion;

public class AutorizacionException : System.Exception
{
    public AutorizacionException() { }
    public AutorizacionException(string message) : base(message) { }
    public AutorizacionException(string message, System.Exception inner) : base(message, inner) { }

}
