using vyaparsathi.Models;

namespace vyaparsathi.Views;

public partial class BillingHistoryPage : ContentPage
{
    private List<Bill> _allBills = new();
    private List<string> _customers = new();

    public BillingHistoryPage()
    {
        InitializeComponent();
        LoadBills();
    }

    private async void LoadBills()
    {
        _allBills = await App.Database.GetBillsAsync();

        foreach (var bill in _allBills)
        {
            var items = await App.Database.GetBillItemsAsync(bill.Id);
            bill.TotalAmount = items.Sum(i => i.Total);
        }

        // Populate Customer Picker
        _customers = _allBills.Select(b => b.CustomerName).Distinct().OrderBy(c => c).ToList();
        CustomerPicker.ItemsSource = _customers;
        CustomerPicker.SelectedIndex = -1; // No selection initially

        ApplyFilters();
    }

    private void OnFilterClicked(object sender, EventArgs e)
    {
        ApplyFilters();
    }

    private void OnFilterChanged(object sender, EventArgs e)
    {
        ApplyFilters();
    }

    private void ApplyFilters()
    {
        var selectedCustomer = CustomerPicker.SelectedItem as string;

        var startDate = StartDatePicker.Date.Date;
        var endDate = EndDatePicker.Date.Date.AddDays(1).AddSeconds(-1);

        var filtered = _allBills
            .Where(b =>
                b.Date >= startDate &&
                b.Date <= endDate &&
                (string.IsNullOrEmpty(selectedCustomer) || b.CustomerName == selectedCustomer))
            .OrderByDescending(b => b.Date)
            .ToList();

        BillsCollection.ItemsSource = filtered;
    }

    private async void OnBillSelected(object sender, SelectionChangedEventArgs e)
    {
        var selectedBill = e.CurrentSelection.FirstOrDefault() as Bill;
        if (selectedBill == null)
            return;

        var items = await App.Database.GetBillItemsAsync(selectedBill.Id);

        string details = string.Join("\n", items.Select(i =>
            $"{i.ItemName} - Qty: {i.Quantity}, Price: ₹{i.Price:F2}, Total: ₹{i.Total:F2}"
        ));

        await DisplayAlert(
            $"Bill for {selectedBill.CustomerName}",
            $"Date: {selectedBill.Date:dd/MM/yyyy}\n\n{details}\n\nTotal: ₹{selectedBill.TotalAmount:F2}",
            "OK");

        ((CollectionView)sender).SelectedItem = null;
    }
}
