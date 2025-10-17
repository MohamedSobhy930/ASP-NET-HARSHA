using System;

/// <summary>
/// Represents the response data for a successfully placed buy order.
/// </summary>
public class BuyOrderResponse
{
    public Guid BuyOrderID { get; set; }
    public string? StockSymbol { get; set; }
    public string? StockName { get; set; }
    public DateTime DateAndTimeOfOrder { get; set; }
    public uint Quantity { get; set; }
    public double Price { get; set; }
    public double TradeAmount { get; set; }
    public bool Equals(BuyOrderResponse? other)
    {
        if (other is null)
            return false;

        // For reference equality (same object)
        if (ReferenceEquals(this, other))
            return true;

        // Check all properties for value equality
        return BuyOrderID.Equals(other.BuyOrderID) &&
               StockSymbol == other.StockSymbol &&
               StockName == other.StockName &&
               DateAndTimeOfOrder.Equals(other.DateAndTimeOfOrder) &&
               Quantity == other.Quantity &&
               Price == other.Price &&
               TradeAmount == other.TradeAmount;
    }
    public override bool Equals(object? obj)
    {
        return Equals(obj as BuyOrderResponse);
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
public static class OrderExtensions
{
    /// <summary>
    /// Maps a BuyOrder entity to a BuyOrderResponse DTO.
    /// </summary>
    public static BuyOrderResponse ToBuyOrderResponse(this BuyOrder buyOrder)
    {
        return new BuyOrderResponse
        {
            BuyOrderID = buyOrder.BuyOrderID,
            StockSymbol = buyOrder.StockSymbol,
            StockName = buyOrder.StockName,
            DateAndTimeOfOrder = buyOrder.DateAndTimeOfOrder,
            Quantity = buyOrder.Quantity,
            Price = buyOrder.Price,
            TradeAmount = buyOrder.Quantity * buyOrder.Price
        };
    }
}