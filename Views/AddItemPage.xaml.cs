using vyaparsathi.Models;
using vyaparsathi.ViewModels;

namespace vyaparsathi.Views;

public partial class AddItemPage : ContentPage
{
    private List<Category> _categories;
    private List<Item> _allItems;
    private List<ItemViewModel> _allItemVMs;

    private Item _editingItem;

    public AddItemPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _categories = await App.Database.GetCategoriesAsync();
        CategoryPicker.ItemsSource = _categories;

        await LoadItems();
    }

    private async Task LoadItems()
    {
        _allItems = await App.Database.GetItemsAsync();

        _allItemVMs = (from item in _allItems
                       join cat in _categories
                       on item.CategoryId equals cat.Id
                       select new ItemViewModel(item, cat.Name))
                       .ToList();

        ItemCollectionView.ItemsSource = _allItemVMs;
    }

    // Save / Update Item
    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ItemNameEntry.Text) ||
            CategoryPicker.SelectedItem == null ||
            !decimal.TryParse(LandingPriceEntry.Text, out decimal landingPrice) ||
            !decimal.TryParse(SellingPriceEntry.Text, out decimal sellingPrice) ||
            !int.TryParse(StockEntry.Text, out int stock))
        {
            await DisplayAlert("Error", "Please fill all fields correctly", "OK");
            return;
        }

        var selectedCategory = CategoryPicker.SelectedItem as Category;

        if (_editingItem != null)
        {
            // Update existing item
            _editingItem.Name = ItemNameEntry.Text.Trim();
            _editingItem.CategoryId = selectedCategory.Id;
            _editingItem.LandingPrice = landingPrice;
            _editingItem.SellingPrice = sellingPrice;
            _editingItem.Stock = stock;

            await App.Database.SaveItemAsync(_editingItem);

            _editingItem = null;
            FormTitle.Text = "Add New Item";
            SaveButton.Text = "Save Item";
        }
        else
        {
            // New item
            var newItem = new Item
            {
                Name = ItemNameEntry.Text.Trim(),
                CategoryId = selectedCategory.Id,
                LandingPrice = landingPrice,
                SellingPrice = sellingPrice,
                Stock = stock
            };

            await App.Database.SaveItemAsync(newItem);
        }

        ClearForm();
        await LoadItems();
    }

    private void ClearForm()
    {
        ItemNameEntry.Text = "";
        CategoryPicker.SelectedIndex = -1;
        LandingPriceEntry.Text = "";
        SellingPriceEntry.Text = "";
        StockEntry.Text = "";
    }

    // Edit Item
    private void OnEditClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var vm = button?.CommandParameter as ItemViewModel;
        if (vm == null) return;

        _editingItem = vm.Item;

        ItemNameEntry.Text = _editingItem.Name;
        CategoryPicker.SelectedItem = _categories.FirstOrDefault(c => c.Id == _editingItem.CategoryId);
        LandingPriceEntry.Text = _editingItem.LandingPrice.ToString();
        SellingPriceEntry.Text = _editingItem.SellingPrice.ToString();
        StockEntry.Text = _editingItem.Stock.ToString();

        FormTitle.Text = "Edit Item";
        SaveButton.Text = "Update Item";
    }

    // Delete Item
    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var vm = button?.CommandParameter as ItemViewModel;
        if (vm == null) return;

        bool confirm = await DisplayAlert("Delete Item", $"Delete '{vm.Name}'?", "Delete", "Cancel");
        if (!confirm) return;

        await App.Database.DeleteItemAsync(vm.Item);
        await LoadItems();
    }
}
