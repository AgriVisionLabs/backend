using Agrivision.Backend.Application.Features.Crop.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Crop.Queries;

public record GetAllCropsQuery() : IRequest<Result<IReadOnlyList<CropResponse>>>;