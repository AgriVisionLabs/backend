using Agrivision.Backend.Infrastructure.Consts;
using Agrivision.Backend.Infrastructure.Persistence.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agrivision.Backend.Infrastructure.Persistence.Identity.EntitiesConfigurations;

public class ApplicationUserConfigurations : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(user => user.FirstName).HasMaxLength(100);
        builder.Property(user => user.LastName).HasMaxLength(100);

        builder.OwnsMany(user => user.RefreshTokens)
            .ToTable("RefreshTokens")
            .WithOwner()
            .HasForeignKey("UserId");


       /* var passwordHasher = new PasswordHasher<ApplicationUser>();

        builder.HasData(
            [
            new ApplicationUser
            {
                Id=DefaultUsers.AdminMId,
                FirstName="MOHAMED",
                LastName="OMAR",
                UserName= DefaultUsers.AdminMEmail,
                NormalizedUserName=DefaultUsers.AdminMEmail.ToUpper(),
                Email=DefaultUsers.AdminMEmail,
                NormalizedEmail=DefaultUsers.AdminMEmail.ToUpper(),
                SecurityStamp=DefaultUsers.AdminMSecurityStamp,
                ConcurrencyStamp=DefaultUsers.AdminMConcurrencyStamp,
                EmailConfirmed=true,
                PasswordHash=passwordHasher.HashPassword(null!,DefaultUsers.AdminMPassword),
                CreatedAt=DateTime.UtcNow
            },
             new ApplicationUser
            {
                Id=DefaultUsers.AdminYId,
                FirstName="YOUSSEF",
                LastName="MAHM",
                UserName= DefaultUsers.AdminYEmail,
                NormalizedUserName=DefaultUsers.AdminYEmail.ToUpper(),
                Email=DefaultUsers.AdminYEmail,
                NormalizedEmail=DefaultUsers.AdminYEmail.ToUpper(),
                SecurityStamp=DefaultUsers.AdminYSecurityStamp,
                ConcurrencyStamp=DefaultUsers.AdminYConcurrencyStamp,
                EmailConfirmed=true,
                PasswordHash=passwordHasher.HashPassword(null!,DefaultUsers.AdminYPassword),
                CreatedAt=
                DateTime.UtcNow
            }
            ]);
       */
    }
}