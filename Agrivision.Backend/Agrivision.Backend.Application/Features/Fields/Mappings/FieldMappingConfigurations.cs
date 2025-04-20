using Agrivision.Backend.Application.Features.Fields.Commands;
using Mapster;

namespace Agrivision.Backend.Application.Features.Fields.Mappings;

public class FieldMappingConfigurations : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateFieldCommand, Domain.Entities.Core.Field>()
            .Map(dest => dest.CreatedOn, src => DateTime.UtcNow);
    }
}