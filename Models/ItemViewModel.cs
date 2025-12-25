using vyaparsathi.Models;

namespace vyaparsathi.ViewModels;

public class ItemViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string CategoryName { get; set; }
    public decimal LandingPrice { get; set; }
    public decimal SellingPrice { get; set; }
    public int StockQuantity { get; set; }

    public string StockText => $"Stock: {StockQuantity}";
    public string PriceText => $"₹{SellingPrice:N2}";

    // ✅ CONSTRUCTOR FIX
    public ItemViewModel(Item item, string categoryName)
    {
        Id = item.Id;
        Name = item.Name;
        CategoryName = categoryName;
        LandingPrice = item.LandingPrice;
        SellingPrice = item.SellingPrice;
        StockQuantity = item.StockQuantity;
    }

    // Optional empty constructor (for binding)
    public ItemViewModel() { }
}
