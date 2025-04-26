using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Domain.Entities.Identity;
using Agrivision.Backend.Infrastructure.Persistence.Identity.Entities;
using Microsoft.EntityFrameworkCore;

namespace Agrivision.Backend.Infrastructure.Persistence.Core;

public class CoreDbContext(DbContextOptions<CoreDbContext> options) : DbContext(options)
{
    public DbSet<Farm> Farms { get; set; }
    public DbSet<Field> Fields { get; set; }
    public DbSet<FarmRole> FarmRoles { get; set; }
    public DbSet<FarmUserRole> FarmUserRoles { get; set; }
    public DbSet<FarmRoleClaim> FarmRoleClaims { get; set; }
    public DbSet<FarmInvitation> FarmInvitations { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(InfrastructureAssemblyMarker).Assembly,
            x => x.Namespace != null && x.Namespace.Contains("Core")); // if you use the normal format (without the namespace contain core stuff) it will add the configuration of ApplicationUser hence add a table for application user
        
        base.OnModelCreating(modelBuilder);
    }
}