using Agrivision.Backend.Application.Features.Field.Commands;
using Mapster;

namespace Agrivision.Backend.Application.Features.Field.Mappings;

public class FieldMappingConfigurations : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateFieldCommand, Domain.Entities.Core.Field>()
            .Map(dest => dest.CreatedOn, src => DateTime.UtcNow);
    }
}