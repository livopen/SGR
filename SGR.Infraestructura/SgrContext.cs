using Microsoft.EntityFrameworkCore;
using SGR.Dominio.Actuaciones;
using SGR.Dominio.Reclamos;
using SGR.Dominio.Usuarios;
using System.Text.Json;

namespace SGR.Infraestructura;

public class SgrContext : DbContext
{
    public DbSet<Reclamo> Reclamos {get; set;}
    public DbSet<Actuacion> Actuaciones {get; set;}
    public DbSet<Usuario> Usuarios {get; set;}

    public SgrContext(DbContextOptions<SgrContext> options) : base(options)
    {}

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Server=localhost\\SQLEXPRESS;Database=SGE_DB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true");
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Mapeo de Reclamo
        modelBuilder.Entity<Reclamo>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.ComplexProperty(e => e.Estado);

            entity.ComplexProperty(e => e.Asunto); 
        });

        // Mapeo de Actuaciones
        modelBuilder.Entity<Actuacion>(entity =>{
            
            entity.HasKey(t => t.Id);
        
            entity.ComplexProperty(t => t.Tipo);
        });

        // Mapeo de Usuario
        modelBuilder.Entity<Usuario>(entity =>{entity.HasKey(u => u._id);
        
            entity.Property(u => u._nombre).IsRequired();

            entity.Property(u => u._correoElectronico).HasConversion(correoObjeto => correoObjeto.Valor, 
                correoString => new CorreoElectronico(correoString)
            ).HasColumnName("_correoElectronico");

            entity.HasIndex(u => u._correoElectronico).IsUnique();

            entity.Property(u => u._contrasenaHash).IsRequired();
        
        //mapeo de lista de permisos
        entity.Property<List<Permiso>>("_permisos")
                .HasColumnName("Permisos")
                .HasConversion(
                    lista => JsonSerializer.Serialize(lista, (JsonSerializerOptions?)null),
                    json => JsonSerializer.Deserialize<List<Permiso>>(json, (JsonSerializerOptions?)null) ?? new List<Permiso>()
                );
        // Le indicamos a EF Core que la propiedad de solo lectura 'Permisos' usa el campo '_permisos'
        entity.Metadata.FindProperty(nameof(Usuario.Permisos))
                ?.SetPropertyAccessMode(PropertyAccessMode.Field);
        });

    }

}

