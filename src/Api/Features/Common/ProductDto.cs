using Api.Domain;

namespace Api.Features.Common;

public record ProductDto(int Id, string Title, uint InventoryCount, Price Price);