using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
            optionsBuilder.UseSqlite("Data Source=SGR.sqlite");
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Mapeo de Reclamo
        modelBuilder.Entity<Reclamo>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Estado);

            entity.ComplexProperty(e => e.Asunto); 
        });

        // Mapeo de Actuaciones
        modelBuilder.Entity<Actuacion>(entity =>{
            
            entity.HasKey(t => t.Id);
        
            entity.Property(t => t.Tipo);
            entity.ComplexProperty(t => t.Detalle);
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
        entity.Property<HashSet<Permiso>>("_permisos")
                .HasColumnName("Permisos")
                .HasConversion(
                    lista => JsonSerializer.Serialize(lista, (JsonSerializerOptions?)null),
                    json => JsonSerializer.Deserialize<HashSet<Permiso>>(json, (JsonSerializerOptions?)null) ?? new HashSet<Permiso>()
                )
                .Metadata.SetValueComparer(new ValueComparer<HashSet<Permiso>>(
                    (actual, snapshot) => actual != null && snapshot != null && actual.SetEquals(snapshot),
                    permisos => permisos == null ? 0 : permisos.Aggregate(0, (hash, permiso) => hash ^ permiso.GetHashCode()),
                    permisos => permisos == null ? new HashSet<Permiso>() : new HashSet<Permiso>(permisos)
                ));
        });

    }

}

