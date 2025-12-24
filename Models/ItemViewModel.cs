using vyaparsathi.Models;

namespace vyaparsathi.ViewModels;

public class ItemViewModel
{
    public Item Item { get; }

    // Expose fields for display
    public string Name => Item.Name;
    public string CategoryName { get; }
    public decimal LandingPrice => Item.LandingPrice;
    public decimal SellingPrice => Item.SellingPrice;
    public int Stock => Item.Stock;

    public ItemViewModel(Item item, string categoryName)
    {
        Item = item;
        CategoryName = categoryName;
    }
}
