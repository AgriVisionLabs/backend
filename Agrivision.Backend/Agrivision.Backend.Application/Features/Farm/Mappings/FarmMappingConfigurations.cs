using Agrivision.Backend.Application.Features.Farm.Commands;
using Agrivision.Backend.Application.Features.Farm.Contracts;
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
    }
}