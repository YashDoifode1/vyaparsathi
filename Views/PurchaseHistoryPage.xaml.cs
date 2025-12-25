using vyaparsathi.Models;
using vyaparsathi.Services;

namespace vyaparsathi.Views;

public partial class PurchaseHistoryPage : ContentPage
{
    private List<Vendor> _vendors = new();
    private List<Purchase> _purchases = new();

    public PurchaseHistoryPage()
    {
        InitializeComponent();
        LoadVendors();
        LoadPurchases();
    }

    private async void LoadVendors()
    {
        _vendors = await App.Database.GetVendorsAsync();
        VendorPicker.ItemsSource = _vendors.Select(v => v.Name).ToList();
    }

    private async void LoadPurchases(int? vendorId = null)
    {
        _purchases = await App.Database.GetPurchasesAsync(); // We'll add this method in DatabaseService
        if (vendorId.HasValue)
            _purchases = _purchases.Where(p => p.VendorId == vendorId.Value).ToList();

        var purchaseDisplayList = new List<PurchaseDisplayModel>();

        foreach (var purchase in _purchases)
        {
            var vendor = _vendors.FirstOrDefault(v => v.Id == purchase.VendorId);
            var items = await App.Database.GetPurchaseItemsAsync(purchase.Id);

            purchaseDisplayList.Add(new PurchaseDisplayModel
            {
                VendorName = vendor?.Name ?? "Unknown",
                Date = purchase.Date,
                Items = items,
                GrandTotal = items.Sum(i => i.Total)
            });
        }

        PurchaseCollectionView.ItemsSource = purchaseDisplayList;
    }

    private void OnVendorChanged(object sender, EventArgs e)
    {
        if (VendorPicker.SelectedIndex < 0)
        {
            LoadPurchases();
            return;
        }

        var vendorId = _vendors[VendorPicker.SelectedIndex].Id;
        LoadPurchases(vendorId);
    }
}

public class PurchaseDisplayModel
{
    public string VendorName { get; set; }
    public DateTime Date { get; set; }
    public List<PurchaseItem> Items { get; set; } = new();
    public decimal GrandTotal { get; set; }
}
