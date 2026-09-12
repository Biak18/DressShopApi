using DressShop.Application.Features.Admin.DTOs;
using MediatR;

namespace DressShop.Application.Features.Admin.GetStats;

public sealed record GetAdminStatsQuery
    : IRequest<AdminStatsDto>;
