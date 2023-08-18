using Api.Domain;

namespace Api.Features.Common;

public record OrderDto(ProductDto Product, InvoiceDetail Detail, DateTime CreatedAt);