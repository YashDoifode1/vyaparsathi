using vyaparsathi.Models;
using vyaparsathi.Services;

namespace vyaparsathi.Views;

public partial class AddItemPage : ContentPage
{
    private readonly DatabaseService _database;
    private Item _editingItem;

    public AddItemPage()
    {
        InitializeComponent();
        _database = App.Database;
        LoadCategories();
    }

    // Constructor for Edit
    public AddItemPage(Item item) : this()
    {
        _editingItem = item;
        FillFormForEdit();
    }

    private async void LoadCategories()
    {
        var categories = await _database.GetCategoriesAsync();
        CategoryPicker.ItemsSource = categories;
        CategoryPicker.ItemDisplayBinding = new Binding("Name");
    }

    private void FillFormForEdit()
    {
        if (_editingItem == null) return;

        NameEntry.Text = _editingItem.Name;
        PriceEntry.Text = _editingItem.SellingPrice.ToString();
        StockEntry.Text = _editingItem.StockQuantity.ToString();

        CategoryPicker.SelectedItem =
            ((List<Category>)CategoryPicker.ItemsSource)
            .FirstOrDefault(c => c.Id == _editingItem.CategoryId);
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameEntry.Text) ||
            string.IsNullOrWhiteSpace(PriceEntry.Text) ||
            CategoryPicker.SelectedItem == null)
        {
            await DisplayAlert("Error", "Please fill all required fields", "OK");
            return;
        }

        var selectedCategory = (Category)CategoryPicker.SelectedItem;

        var item = _editingItem ?? new Item();

        item.Name = NameEntry.Text.Trim();
        item.CategoryId = selectedCategory.Id;
        item.SellingPrice = decimal.Parse(PriceEntry.Text);
        item.StockQuantity = int.TryParse(StockEntry.Text, out int stock) ? stock : 0;

        await _database.SaveItemAsync(item);

        await DisplayAlert("Success", "Item saved successfully", "OK");
        await Navigation.PopAsync();
    }
}
