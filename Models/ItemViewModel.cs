using vyaparsathi.Models;

namespace vyaparsathi.ViewModels;

public class ItemViewModel
{
    public Item Item { get; }
    public string Name => Item.Name;
    public string CategoryName { get; }
    public int Stock => Item.Stock;
    public decimal LandingPrice => Item.LandingPrice;
    public decimal SellingPrice => Item.SellingPrice;

    public decimal TotalValue => Stock * SellingPrice;

    public ItemViewModel(Item item, string categoryName)
    {
        Item = item;
        CategoryName = categoryName;
    }
}
