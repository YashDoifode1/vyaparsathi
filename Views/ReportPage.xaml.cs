using vyaparsathi.Models;

namespace vyaparsathi.Views;

public partial class ReportPage : ContentPage
{
    public ReportPage()
    {
        InitializeComponent();

        FromDatePicker.Date = DateTime.Today.AddDays(-30);
        ToDatePicker.Date = DateTime.Today;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadReportAsync();
    }

    private async void OnApplyFilterClicked(object sender, EventArgs e)
    {
        await LoadReportAsync();
    }

    private async Task LoadReportAsync()
    {
        var from = FromDatePicker.Date.Date;
        var to = ToDatePicker.Date.Date.AddDays(1).AddSeconds(-1);

        // Bills
        var bills = await App.Database.GetBillsAsync();
        var filteredBills = bills
            .Where(b => b.Date >= from && b.Date <= to)
            .ToList();

        decimal totalSales = filteredBills.Sum(b => b.TotalAmount);

        // Udhar
        var udhars = await App.Database.GetUdharsAsync();
        decimal totalUdhar = udhars
            .Where(u => u.Date >= from && u.Date <= to && !u.IsPaid)
            .Sum(u => u.Amount);

        // Purchase Cost (from Item LandingPrice * stock sold approx)
        var items = await App.Database.GetItemsAsync();
        decimal purchaseCost = items.Sum(i => i.LandingPrice * i.StockQuantity);

        // Profit
        decimal profit = totalSales - purchaseCost;

        // UI Update
        TotalSalesLabel.Text = $"₹{totalSales:N2}";
        PurchaseCostLabel.Text = $"₹{purchaseCost:N2}";
        ProfitLabel.Text = $"₹{profit:N2}";
        UdharLabel.Text = $"₹{totalUdhar:N2}";
        BillsCountLabel.Text = filteredBills.Count.ToString();
    }
}
