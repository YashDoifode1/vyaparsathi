using vyaparsathi.Models;
using System.Collections.ObjectModel;

namespace vyaparsathi.Views;

public partial class PurchasePage : ContentPage
{
    private List<Category> _categories;
    private List<Item> _items;
    private List<Vendor> _vendors;
    private Vendor _selectedVendor;

    private ObservableCollection<PurchaseItem> _purchaseItems = new();

    public PurchasePage()
    {
        InitializeComponent();
        PurchaseItemsCollection.ItemsSource = _purchaseItems;

        LoadVendors();
        LoadCategories();
    }

    private async void LoadVendors()
    {
        _vendors = await App.Database.GetVendorsAsync();
        VendorPicker.ItemsSource = _vendors.Select(v => v.Name).ToList();
    }

    private void OnVendorSelected(object sender, EventArgs e)
    {
        if (VendorPicker.SelectedIndex < 0) return;

        _selectedVendor = _vendors[VendorPicker.SelectedIndex];
    }

    private async void LoadCategories()
    {
        _categories = await App.Database.GetCategoriesAsync();
        CategoryPicker.ItemsSource = _categories.Select(c => c.Name).ToList();
    }

    private async void OnCategoryChanged(object sender, EventArgs e)
    {
        if (CategoryPicker.SelectedIndex < 0) return;

        var selectedCategory = _categories[CategoryPicker.SelectedIndex];

        _items = (await App.Database.GetItemsAsync())
            .Where(i => i.CategoryId == selectedCategory.Id)
            .ToList();

        ItemPicker.ItemsSource = _items.Select(i => i.Name).ToList();
    }

    private void OnItemChanged(object sender, EventArgs e)
    {
        if (ItemPicker.SelectedIndex < 0) return;

        PriceEntry.Text = _items[ItemPicker.SelectedIndex].LandingPrice.ToString("F2");
    }

    private void OnAddPurchaseItemClicked(object sender, EventArgs e)
    {
        if (ItemPicker.SelectedIndex < 0 || _selectedVendor == null) return;

        var selectedItem = _items[ItemPicker.SelectedIndex];

        if (!decimal.TryParse(QuantityEntry.Text, out decimal qtyDecimal) ||
            !decimal.TryParse(PriceEntry.Text, out decimal priceDecimal))
        {
            DisplayAlert("Error", "Invalid quantity or price", "OK");
            return;
        }

        var purchaseItem = new PurchaseItem
        {
            ItemId = selectedItem.Id,
            ItemName = selectedItem.Name,
            Quantity = (int)qtyDecimal,   // ✅ Explicit cast
            Price = priceDecimal
        };

        _purchaseItems.Add(purchaseItem);
        UpdateTotal();

        QuantityEntry.Text = "";
        PriceEntry.Text = "";
        ItemPicker.SelectedIndex = -1;
    }

    private void UpdateTotal()
    {
        TotalLabel.Text = $"₹{_purchaseItems.Sum(i => i.Total):F2}";
    }

    private async void OnSavePurchaseClicked(object sender, EventArgs e)
    {
        if (_selectedVendor == null || _purchaseItems.Count == 0)
        {
            await DisplayAlert("Error", "Vendor and purchase items are required.", "OK");
            return;
        }

        var purchase = new Purchase
        {
            VendorId = _selectedVendor.Id,
            Date = PurchaseDatePicker.Date
        };

        await App.Database.SavePurchaseAsync(purchase);

        foreach (var item in _purchaseItems)
        {
            item.PurchaseId = purchase.Id;

            // Update stock
            var dbItem = await App.Database.GetItemByIdAsync(item.ItemId);
            if (dbItem != null)
            {
                dbItem.StockQuantity += item.Quantity; // Add purchased quantity
                await App.Database.SaveItemAsync(dbItem);
            }

            await App.Database.SavePurchaseItemAsync(item);
        }

        await DisplayAlert("Success", "Purchase saved successfully.", "OK");

        _purchaseItems.Clear();
        UpdateTotal();
        VendorPicker.SelectedIndex = -1;
        CategoryPicker.SelectedIndex = -1;
        ItemPicker.SelectedIndex = -1;
        QuantityEntry.Text = "";
        PriceEntry.Text = "";
    }
}
