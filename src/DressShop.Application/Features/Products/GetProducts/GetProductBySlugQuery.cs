using DressShop.Application.Features.Products.DTOs;
using MediatR;

namespace DressShop.Application.Features.Products.GetProducts;

public record GetProductBySlugQuery(string Slug) : IRequest<ProductDetailDto>;
