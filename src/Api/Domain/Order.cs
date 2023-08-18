namespace Api.Domain;

public class Order
{
    public long Id { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; }

    public int BuyerId { get; set; }
    public User Buyer { get; set; }

    public InvoiceDetail Detail { get; set; }
    public DateTime CreatedAt { get; set; }

    public static Order Create(Product product, int userId, uint count) =>
        new()
        {
            ProductId = product.Id,
            BuyerId = userId,
            Detail = new()
            {
                DiscountPercent = product.Price.DiscountPercent,
                GrossValue = product.Price.GrossValue,
                Count = count
            }
        };
}

public record InvoiceDetail : Price
{
    public uint Count { get; init; }
    public uint TotalNetValue => (uint)(GrossValue - DiscountPercent * GrossValue / 100) * Count;
}