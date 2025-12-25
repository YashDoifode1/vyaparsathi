using vyaparsathi.Models;
using vyaparsathi.Services;

namespace vyaparsathi.Views;

public partial class StockPage : ContentPage
{
    private readonly DatabaseService _database;
    private List<Item> _items = new();
    private List<Category> _categories = new();

    public StockPage()
    {
        InitializeComponent();
        _database = App.Database;

        LoadCategories();
        LoadDashboard();
    }

    private async void LoadCategories()
    {
        _categories = await _database.GetCategoriesAsync();
        CategoryPicker.ItemsSource = _categories;
        CategoryPicker.ItemDisplayBinding = new Binding("Name");
    }

    private async void OnCategoryChanged(object sender, EventArgs e)
    {
        if (CategoryPicker.SelectedItem is not Category category)
            return;

        _items = (await _database.GetItemsAsync())
                    .Where(i => i.CategoryId == category.Id)
                    .ToList();

        ItemPicker.ItemsSource = _items;
        ItemPicker.ItemDisplayBinding = new Binding("Name");

        ItemPicker.SelectedItem = null;
        PriceEntry.Text = string.Empty;
        StockEntry.Text = string.Empty;
    }

    private void OnItemChanged(object sender, EventArgs e)
    {
        if (ItemPicker.SelectedItem is not Item item)
            return;

        PriceEntry.Text = item.SellingPrice.ToString("0.##");
    }

    private async void OnSaveStockClicked(object sender, EventArgs e)
    {
        if (ItemPicker.SelectedItem is not Item item)
        {
            await DisplayAlert("Error", "Please select an item", "OK");
            return;
        }

        if (!int.TryParse(StockEntry.Text, out int quantity))
        {
            await DisplayAlert("Error", "Enter valid quantity", "OK");
            return;
        }

        item.StockQuantity += quantity;
        await _database.SaveItemAsync(item);

        await DisplayAlert("Success", "Stock updated successfully", "OK");

        StockEntry.Text = string.Empty;
        PriceEntry.Text = string.Empty;
        ItemPicker.SelectedItem = null;

        LoadDashboard();
    }

    private async void LoadDashboard()
    {
        var allItems = await _database.GetItemsAsync();

        TotalItemsLabel.Text = allItems.Count.ToString();
        TotalQuantityLabel.Text = allItems.Sum(i => i.StockQuantity).ToString();
        LowStockLabel.Text = allItems.Count(i => i.StockQuantity < 10).ToString();

        var totalValue = allItems.Sum(i => i.SellingPrice * i.StockQuantity);
        TotalInventoryValueLabel.Text = $"₹{totalValue:0.##}";

        // Category-wise distribution
        var categoryStock = _categories.Select(c => new
        {
            Name = c.Name,
            Quantity = allItems.Where(i => i.CategoryId == c.Id).Sum(i => i.StockQuantity)
        }).ToList();

        CategoryStockCollection.ItemsSource = categoryStock;
    }
}
