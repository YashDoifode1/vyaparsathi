using vyaparsathi.Models;

namespace vyaparsathi.Views;

public partial class BillingHistoryPage : ContentPage
{
    private List<Bill> _allBills = new List<Bill>();

    public BillingHistoryPage()
    {
        InitializeComponent();
        LoadBills();
    }

    private async void LoadBills()
    {
        _allBills = await App.Database.GetBillsAsync();

        // Calculate total for each bill
        foreach (var bill in _allBills)
        {
            var items = await App.Database.GetBillItemsAsync(bill.Id);
            bill.TotalAmount = items.Sum(i => i.Total);
        }

        BillsCollection.ItemsSource = _allBills.OrderByDescending(b => b.Date).ToList();
    }

    private void OnFilterClicked(object sender, EventArgs e)
    {
        var start = StartDatePicker.Date;
        var end = EndDatePicker.Date.AddDays(1).AddSeconds(-1); // include full day

        var filtered = _allBills
            .Where(b => b.Date >= start && b.Date <= end)
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

        string itemDetails = string.Join("\n", items.Select(i =>
            $"{i.ItemName} - Qty: {i.Quantity}, Price: ₹{i.Price:F2}, Total: ₹{i.Total:F2}"
        ));

        await DisplayAlert($"Bill for {selectedBill.CustomerName}",
            $"Date: {selectedBill.Date:dd/MM/yyyy}\n\n{itemDetails}\n\nTotal: ₹{selectedBill.TotalAmount:F2}", "OK");

        ((CollectionView)sender).SelectedItem = null;
    }
}
