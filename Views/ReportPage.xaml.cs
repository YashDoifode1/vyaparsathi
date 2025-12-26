using vyaparsathi.Models;

namespace vyaparsathi.Views;

public partial class ReportPage : ContentPage
{
    private List<Bill> _allBills = new();
    private List<string> _customers = new();

    public ReportPage()
    {
        InitializeComponent();
        FromDatePicker.Date = DateTime.Today.AddDays(-30);
        ToDatePicker.Date = DateTime.Today;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadDataAsync();
        await LoadReportAsync();
    }

    private async Task LoadDataAsync()
    {
        _allBills = await App.Database.GetBillsAsync();

        foreach (var bill in _allBills)
        {
            var items = await App.Database.GetBillItemsAsync(bill.Id);
            bill.TotalAmount = items.Sum(i => i.Total);
        }

        // Populate Customer Picker
        _customers = _allBills.Select(b => b.CustomerName)
                              .Distinct()
                              .OrderBy(c => c)
                              .ToList();

        CustomerPicker.ItemsSource = _customers;
        CustomerPicker.SelectedIndex = -1;
    }

    private async void OnApplyFilterClicked(object sender, EventArgs e)
    {
        await LoadReportAsync();
    }

    private async void OnFilterChanged(object sender, EventArgs e)
    {
        await LoadReportAsync();
    }

    private async Task LoadReportAsync()
    {
        var from = FromDatePicker.Date.Date;
        var to = ToDatePicker.Date.Date.AddDays(1).AddSeconds(-1);

        var selectedCustomer = CustomerPicker.SelectedItem as string;

        // Filter bills
        var filteredBills = _allBills
            .Where(b => b.Date >= from && b.Date <= to &&
                        (string.IsNullOrEmpty(selectedCustomer) || b.CustomerName == selectedCustomer))
            .ToList();

        decimal totalSales = filteredBills.Sum(b => b.TotalAmount);

        // Purchases (no customer filter)
        var purchases = await App.Database.GetPurchasesAsync();
        decimal totalPurchases = purchases
            .Where(p => p.Date >= from && p.Date <= to)
            .Sum(p => p.TotalAmount);

        // Udhar
        var udhars = await App.Database.GetUdharsAsync();
        decimal totalUdhar = udhars
            .Where(u => u.Date >= from && u.Date <= to &&
                        !u.IsPaid &&
                        (string.IsNullOrEmpty(selectedCustomer) || u.CustomerName == selectedCustomer))
            .Sum(u => u.Amount);

        decimal profit = totalSales - totalPurchases;

        // Update UI
        TotalSalesLabel.Text = $"₹{totalSales:N0}";
        PurchaseCostLabel.Text = $"₹{totalPurchases:N0}";
        ProfitLabel.Text = $"₹{profit:N0}";
        UdharLabel.Text = $"₹{totalUdhar:N0}";
    }
}
