using System;
using SGR.Dominio.Comun;

namespace SGR.Dominio.Usuarios;

public record class CorreoElectronico
{
    public string Valor {get;}
    public CorreoElectronico(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new DominioException("El correo electrónico no puede estar vacío.");

        var normalizado = valor.Trim().ToLowerInvariant();

        var partes = normalizado.Split('@');

        if (partes.Length != 2 || string.IsNullOrWhiteSpace(partes[0]) || string.IsNullOrWhiteSpace(partes[1]))
            throw new DominioException("El correo electrónico tiene un formato inválido.");
        
        var dominio = partes[1];
        var indicePunto = dominio.LastIndexOf('.');
        if (indicePunto <= 0 || indicePunto == dominio.Length - 1)
            throw new DominioException("El correo electrónico tiene un formato inválido.");

        Valor = normalizado;
    }
    public override string ToString() => Valor;
}
