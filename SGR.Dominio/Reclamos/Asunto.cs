using System;
using SGR.Dominio.Comun ;
namespace SGR.Dominio.Reclamos;

public record class Asunto
{
    public string Descripcion {get; }
    public Asunto(string descripcion)
    {
        if (string.IsNullOrWhiteSpace(descripcion)|| descripcion.Length > 200)
            throw new DominioException("El asunto no puede estar vacío o exceder 200 caracteres");
        Descripcion = descripcion;
    }
}
