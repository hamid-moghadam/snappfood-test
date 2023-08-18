namespace Api.Options;

public class DefaultValuesOptions
{
    public const string Key = "DefaultValues";

    public uint InventoryCount { get; set; }
    public uint ProductSlidingExpirationInMinutes { get; set; }
    public uint ProductAbsoluteExpirationInMinutes { get; set; }
}