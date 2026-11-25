using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SGR.Dominio.Actuaciones;
using SGR.Dominio.Reclamos;
using SGR.Dominio.Usuarios;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SGR.Infraestructura;

public class SgrContext : DbContext
{
    private static readonly JsonSerializerOptions PermisoJsonOptions = new()
    {
        Converters = { new JsonStringEnumConverter() }
    };

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
            
            entity.Property(e => e.Estado).HasConversion<string>();

            entity.ComplexProperty(e => e.Asunto); 
        });

        // Mapeo de Actuaciones
        modelBuilder.Entity<Actuacion>(entity =>{
            
            entity.HasKey(t => t.Id);
        
            entity.Property(t => t.Tipo).HasConversion<string>();
            entity.ComplexProperty(t => t.Detalle);
            entity.HasOne<Reclamo>()
                .WithMany()
                .HasForeignKey(t => t.ReclamoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Mapeo de Usuario
        modelBuilder.Entity<Usuario>(entity =>{entity.HasKey(u => u._id);
        
            entity.Property(u => u._nombre).IsRequired();

            entity.ComplexProperty(u => u._correoElectronico, correo =>
            {
                correo.Property(c => c.Valor)
                    .HasColumnName("_correoElectronico")
                    .IsRequired();
            });

            entity.Property(u => u._contrasenaHash).IsRequired();
        
        //mapeo de lista de permisos
        entity.Property<HashSet<Permiso>>("_permisos")
                .HasColumnName("Permisos")
                .HasConversion(
                    lista => JsonSerializer.Serialize(lista, PermisoJsonOptions),
                    json => JsonSerializer.Deserialize<HashSet<Permiso>>(json, PermisoJsonOptions) ?? new HashSet<Permiso>()
                )
                .Metadata.SetValueComparer(new ValueComparer<HashSet<Permiso>>(
                    (actual, snapshot) => actual != null && snapshot != null && actual.SetEquals(snapshot),
                    permisos => permisos == null ? 0 : permisos.Aggregate(0, (hash, permiso) => hash ^ permiso.GetHashCode()),
                    permisos => permisos == null ? new HashSet<Permiso>() : new HashSet<Permiso>(permisos)
                ));
        });

    }

}

