using System;

namespace SGR.Dominio.Comun;

public class DominioException : Exception
{
    public DominioException(string message) : base(message)
    {
    }
}
