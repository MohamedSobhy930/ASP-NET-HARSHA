using System;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents a sell order for a stock.
/// </summary>
public class SellOrder
{
    /// <summary>
    /// The unique identifier for the sell order.
    /// </summary>
    [Key]
    public Guid SellOrderID { get; set; }

    /// <summary>
    /// The stock symbol (e.g., AAPL for Apple Inc.).
    /// </summary>
    [Required(ErrorMessage = "Stock Symbol is required.")]
    public string? StockSymbol { get; set; }

    /// <summary>
    /// The name of the stock (e.g., Apple Inc.).
    /// </summary>
    [Required(ErrorMessage = "Stock Name is required.")]
    public string? StockName { get; set; }

    /// <summary>
    /// The date and time when the order was placed.
    /// </summary>
    public DateTime DateAndTimeOfOrder { get; set; }

    /// <summary>
    /// The number of shares to sell.
    /// </summary>
    [Range(1, 100000, ErrorMessage = "Quantity must be between 1 and 100,000.")]
    public uint Quantity { get; set; }

    /// <summary>
    /// The price per share.
    /// </summary>
    [Range(1.0, 10000.0, ErrorMessage = "Price must be between 1 and 10,000.")]
    public double Price { get; set; }
}
