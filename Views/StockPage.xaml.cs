using vyaparsathi.Models;
using vyaparsathi.Services;

namespace vyaparsathi.Views;

public partial class StockPage : ContentPage
{
    private readonly DatabaseService _database;
    private List<Item> _items = new();

    public StockPage()
    {
        InitializeComponent();
        _database = App.Database;
        LoadCategories();
    }

    private async void LoadCategories()
    {
        var categories = await _database.GetCategoriesAsync();
        CategoryPicker.ItemsSource = categories;
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

    // ✅ FIXED EVENT HANDLER
    private async void OnSaveStockClicked(object sender, EventArgs e)
    {
        if (ItemPicker.SelectedItem is not Item item)
        {
            await DisplayAlert("Error", "Please select an item", "OK");
            return;
        }

        if (!int.TryParse(StockEntry.Text, out int quantity))
        {
            await DisplayAlert("Error", "Enter valid stock quantity", "OK");
            return;
        }

        item.StockQuantity += quantity;
        await _database.SaveItemAsync(item);

        await DisplayAlert("Success", "Stock updated successfully", "OK");

        StockEntry.Text = string.Empty;
    }
}
