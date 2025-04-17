using System.Reflection;
using Agrivision.Backend.Domain.Entities.Identity;
using Agrivision.Backend.Infrastructure.Persistence.Identity.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Agrivision.Backend.Infrastructure.Persistence.Identity;

public class ApplicationUserDbContext(DbContextOptions<ApplicationUserDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{ 
    public DbSet<OtpVerification> OtpVerifications { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(InfrastructureAssemblyMarker).Assembly,
            x => x.Namespace != null && x.Namespace.Contains("Identity"));
        
        base.OnModelCreating(modelBuilder);
    }
}