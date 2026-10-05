using System;

namespace SGR.Dominio.Reclamos;

public record class Asunto
{
    public string Descripcion {get; }
    public Asunto(string descripcion)
    {
        if (string.IsNullOrWhiteSpace(descripcion)|| descripcion.Length > 200)
            throw new ArgumentException("El asunto no puede estar vacío o exceder 200 caracteres", nameof(descripcion));
        Descripcion = descripcion;
    }
}
