using vyaparsathi.Models;
using vyaparsathi.ViewModels;

namespace vyaparsathi.Views;

public partial class StockPage : ContentPage
{
    private List<Item> _allItems;
    private StockViewModel _editingStock;

    public StockPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadItems();
        UpdateSummary();
    }

    private async Task LoadItems()
    {
        _allItems = await App.Database.GetItemsAsync();

        var stockItems = _allItems.Select(i => new StockViewModel
        {
            Item = i,
            Quantity = i.Stock,
            UnitPrice = i.SellingPrice
        }).ToList();

        StockCollectionView.ItemsSource = stockItems;
        UpdateSummary();
    }

    private void UpdateSummary()
    {
        int totalItems = _allItems.Count;
        int totalStock = _allItems.Sum(i => i.Stock);
        decimal totalValue = _allItems.Sum(i => i.Stock * i.SellingPrice);

        TotalItemsLabel.Text = totalItems.ToString();
        TotalStockLabel.Text = totalStock.ToString();
        TotalValueLabel.Text = totalValue.ToString("F2");
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (ItemPicker.SelectedItem == null ||
            !int.TryParse(StockQuantityEntry.Text, out int quantity) ||
            !decimal.TryParse(StockPriceEntry.Text, out decimal price))
        {
            await DisplayAlert("Error", "Please fill all fields correctly", "OK");
            return;
        }

        var item = (Item)ItemPicker.SelectedItem;

        item.Stock = quantity;
        item.SellingPrice = price;
        item.UpdatedAt = DateTime.UtcNow;

        await App.Database.SaveItemAsync(item);

        // Clear
        StockQuantityEntry.Text = "";
        StockPriceEntry.Text = "";
        ItemPicker.SelectedIndex = -1;

        await LoadItems();
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var stock = button?.CommandParameter as StockViewModel;
        if (stock == null) return;

        bool confirm = await DisplayAlert("Delete Stock", $"Delete '{stock.Item.Name}'?", "Delete", "Cancel");
        if (!confirm) return;

        await App.Database.DeleteItemAsync(stock.Item);
        await LoadItems();
    }

    private void OnEditClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var stock = button?.CommandParameter as StockViewModel;
        if (stock == null) return;

        ItemPicker.SelectedItem = stock.Item;
        StockQuantityEntry.Text = stock.Quantity.ToString();
        StockPriceEntry.Text = stock.UnitPrice.ToString();
        FormTitle.Text = "Edit Stock Item";
        SaveButton.Text = "Update Stock";
    }
}
