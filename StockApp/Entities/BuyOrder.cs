using System;
using System.ComponentModel.DataAnnotations;

public class BuyOrder
{
    /// <summary>
    /// The unique identifier for the buy order.
    /// </summary>
    [Key]
    public Guid BuyOrderID { get; set; }

    /// <summary>
    /// The stock symbol (e.g., MSFT for Microsoft).
    /// </summary>
    [Required(ErrorMessage = "Stock Symbol is required.")]
    public string? StockSymbol { get; set; }

    /// <summary>
    /// The name of the stock (e.g., Microsoft Corporation).
    /// </summary>
    [Required(ErrorMessage = "Stock Name is required.")]
    public string? StockName { get; set; }

    /// <summary>
    /// The date and time when the order was placed.
    /// </summary>
    public DateTime DateAndTimeOfOrder { get; set; }

    /// <summary>
    /// The number of shares to buy.
    /// </summary>
    [Range(1, 100000, ErrorMessage = "Quantity must be between 1 and 100,000.")]
    public uint Quantity { get; set; }

    /// <summary>
    /// The price per share.
    /// </summary>
    [Range(1.0, 10000.0, ErrorMessage = "Price must be between 1 and 10,000.")]
    public double Price { get; set; }
}
