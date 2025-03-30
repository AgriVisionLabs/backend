using Agrivision.Backend.Application.Features.Farm.Commands;
using Agrivision.Backend.Application.Features.Farm.Contracts;
using Agrivision.Backend.Domain.Entities.Core;
using Mapster;

namespace Agrivision.Backend.Application.Features.Farm.Mappings;

public class FarmMappingConfigurations : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateFarmCommand, Domain.Entities.Core.Farm>()
             .Map(dest => dest.CreatedOn, src => DateTime.UtcNow);

        config.NewConfig<Domain.Entities.Core.Farm, FarmResponse>()
            .Map(dest => dest.OwnerId, src => src.CreatedById);

        config.NewConfig<Domain.Entities.Core.Farm, FarmResponse>()
            .Ignore(dest => dest.Id);

        config.NewConfig<CreateFarmCommand, Domain.Entities.Core.Farm>()
            .Ignore(dest => dest.FarmMembers);

        config.NewConfig<FarmMember, FarmMembers_Contract>()
            .Map(dest => dest.Email, src => src.Email) 
            .Map(dest => dest.Role, src => src.Role);

      }
}