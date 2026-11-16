namespace SGR.Aplicacion.Comun;

public class CredencialesInvalidasException : Exception
{
    public CredencialesInvalidasException() { }
    public CredencialesInvalidasException(string message) : base(message) { }
    public CredencialesInvalidasException(string message, System.Exception inner) : base(message, inner) { }

}
