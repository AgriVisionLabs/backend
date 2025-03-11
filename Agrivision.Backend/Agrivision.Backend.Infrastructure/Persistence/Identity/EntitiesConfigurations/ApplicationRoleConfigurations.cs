using Agrivision.Backend.Infrastructure.Consts;
using Agrivision.Backend.Infrastructure.Persistence.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agrivision.Backend.Infrastructure.Persistence.Identity.EntitiesConfigurations;

public class ApplicationRoleConfigurations : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.HasData(
            [
            new ApplicationRole
            {
               Id=DefaultRoles.AdminRoleId,
               Name=DefaultRoles.Admin,
               NormalizedName=DefaultRoles.Admin.ToUpper(),
               ConcurrencyStamp=DefaultRoles.AdminRoleConcurrencyStamp,
            },
            new ApplicationRole
            {
               Id=DefaultRoles.ManagerRoleId,
               Name=DefaultRoles.Member,
               NormalizedName=DefaultRoles.Member.ToUpper(),
               ConcurrencyStamp=DefaultRoles.MemberRoleConcurrencyStamp,
               IsDefault=true,
            }
            ]);

    }
}