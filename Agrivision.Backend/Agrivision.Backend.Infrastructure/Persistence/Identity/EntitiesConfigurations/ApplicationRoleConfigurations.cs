using Agrivision.Backend.Infrastructure.Consts;
using Agrivision.Backend.Infrastructure.Persistence.Identity.Entities;
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
               ConcurrencyStamp=DefaultRoles.AdminRoleConcurrencyStamp
            },
            new ApplicationRole
            {
               Id=DefaultRoles.MemberRoleId,
               Name=DefaultRoles.Member,
               NormalizedName=DefaultRoles.Member.ToUpper(),
               ConcurrencyStamp=DefaultRoles.MemberRoleConcurrencyStamp,
               IsDefault=true
            }
            , new ApplicationRole
            {
               Id=DefaultRoles.OwnerRoleId,
               Name=DefaultRoles.Owner,
               NormalizedName=DefaultRoles.Owner.ToUpper(),
               ConcurrencyStamp=DefaultRoles.OwnerRoleConcurrencyStamp
               
            }
            , new ApplicationRole
            {
               Id=DefaultRoles.WorkerRoleId,
               Name=DefaultRoles.Worker,
               NormalizedName=DefaultRoles.Worker.ToUpper(),
               ConcurrencyStamp=DefaultRoles.WorkerRoleConcurrencyStamp

            }
            , new ApplicationRole
            {
               Id=DefaultRoles.ManagerRoleId,
               Name=DefaultRoles.Manager,
               NormalizedName=DefaultRoles.Manager.ToUpper(),
               ConcurrencyStamp=DefaultRoles.ManagerRoleConcurrencyStamp

            }
            , new ApplicationRole
            {
               Id=DefaultRoles.ExpertRoleId,
               Name=DefaultRoles.Expert,
               NormalizedName=DefaultRoles.Expert.ToUpper(),
               ConcurrencyStamp=DefaultRoles.ExpertRoleConcurrencyStamp

            }
            ]);

    }
}