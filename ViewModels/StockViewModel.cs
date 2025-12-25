using vyaparsathi.Models;

namespace vyaparsathi.ViewModels;

public class StockViewModel
{
    public Item Item { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    public decimal TotalValue => Quantity * UnitPrice;

    public string ItemName => Item.Name;
    public string CategoryName => Item.CategoryId > 0 ? Item.CategoryId.ToString() : "Uncategorized";
}
