using vyaparsathi.Models;

namespace vyaparsathi.Views;

public partial class DashboardPage : ContentPage
{
    public DashboardPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadDashboardData();
    }

    private async Task LoadDashboardData()
    {
        // Today's Sales
        var bills = await App.Database.GetBillsAsync();
        var todaySales = bills
            .Where(b => b.Date.Date == DateTime.Today)
            .Sum(b => b.TotalAmount);

        TodaySalesLabel.Text = $"₹{todaySales:N2}";

        // Total Udhar
        var udhars = await App.Database.GetUdharsAsync();
        var totalUdhar = udhars.Sum(u => u.Amount);
        TotalUdharLabel.Text = $"₹{totalUdhar:N2}";

        // Low Stock Items
        var items = await App.Database.GetItemsAsync();
        var lowStockCount = items.Count(i => i.StockQuantity <= 5);
        LowStockLabel.Text = lowStockCount.ToString();

        // Total Products
        TotalProductsLabel.Text = items.Count.ToString();
    }

    private async void OnCreateBillClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(BillingPage));
    }

    private async void OnStockClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(StockPage));
    }
}
