using vyaparsathi.Models;
using vyaparsathi.ViewModels;

namespace vyaparsathi.Views;

public partial class StockPage : ContentPage
{
    private List<Category> _categories;
    private List<Item> _items;
    private List<ItemViewModel> _itemViewModels;
    private Item _editingItem;

    public StockPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _categories = await App.Database.GetCategoriesAsync();
        _items = await App.Database.GetItemsAsync();

        // Bind Pickers
        ItemPicker.ItemsSource = _items;
        ItemPicker.ItemDisplayBinding = new Binding("Name");

        CategoryPicker.ItemsSource = _categories;
        CategoryPicker.ItemDisplayBinding = new Binding("Name");

        BuildItemViewModels();
        UpdateSummary();
    }

    private void BuildItemViewModels()
    {
        _itemViewModels = (from item in _items
                           join cat in _categories
                           on item.CategoryId equals cat.Id
                           select new ItemViewModel(item, cat.Name))
                          .ToList();

        StockCollectionView.ItemsSource = _itemViewModels;
    }

    private void UpdateSummary()
    {
        TotalItemsLabel.Text = _items.Count.ToString();
        TotalStockLabel.Text = _items.Sum(i => i.Stock).ToString();
        TotalValueLabel.Text = _items.Sum(i => i.Stock * i.SellingPrice).ToString("F2");
    }

    // Save / Update Stock
    private async void OnSaveStockClicked(object sender, EventArgs e)
    {
        if (ItemPicker.SelectedItem == null ||
            !int.TryParse(StockQuantityEntry.Text, out int stock) ||
            !decimal.TryParse(StockPriceEntry.Text, out decimal price))
        {
            await DisplayAlert("Error", "Select an item and enter valid stock/price.", "OK");
            return;
        }

        var selectedItem = ItemPicker.SelectedItem as Item;

        selectedItem.Stock = stock;
        selectedItem.SellingPrice = price;

        await App.Database.SaveItemAsync(selectedItem);

        ClearStockForm();
        _items = await App.Database.GetItemsAsync();
        BuildItemViewModels();
        UpdateSummary();
    }

    private void ClearStockForm()
    {
        ItemPicker.SelectedIndex = -1;
        StockQuantityEntry.Text = "";
        StockPriceEntry.Text = "";
    }

    // Edit Stock
    private void OnEditStockClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var vm = button?.CommandParameter as ItemViewModel;
        if (vm == null) return;

        _editingItem = vm.Item;
        ItemPicker.SelectedItem = _editingItem;
        StockQuantityEntry.Text = _editingItem.Stock.ToString();
        StockPriceEntry.Text = _editingItem.SellingPrice.ToString();
    }

    // Delete Stock
    private async void OnDeleteStockClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var vm = button?.CommandParameter as ItemViewModel;
        if (vm == null) return;

        bool confirm = await DisplayAlert("Delete Item", $"Delete '{vm.Name}'?", "Delete", "Cancel");
        if (!confirm) return;

        await App.Database.DeleteItemAsync(vm.Item);
        _items = await App.Database.GetItemsAsync();
        BuildItemViewModels();
        UpdateSummary();
    }
}


