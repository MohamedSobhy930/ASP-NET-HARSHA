using System;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents the data required to place a sell order.
/// </summary>
public class SellOrderRequest
{
    [Required(ErrorMessage = "Stock Symbol is required.")]
    public string? StockSymbol { get; set; }

    [Required(ErrorMessage = "Stock Name is required.")]
    public string? StockName { get; set; }

    [DateMustBeAfter(2000, 1, 1)]
    public DateTime DateAndTimeOfOrder { get; set; }

    [Range(1, 100000, ErrorMessage = "Quantity must be between 1 and 100,000.")]
    public uint Quantity { get; set; }

    [Range(1.0, 10000.0, ErrorMessage = "Price must be between 1 and 10,000.")]
    public double Price { get; set; }
    public SellOrder ToSellOrder()
    {
        return new SellOrder { StockSymbol = StockSymbol, StockName = StockName, Price = Price, DateAndTimeOfOrder = DateAndTimeOfOrder, Quantity = Quantity };
    }
}
