using vyaparsathi.Models;
using vyaparsathi.Services;

namespace vyaparsathi.Views;

public partial class UdharHistoryPage : ContentPage
{
    private List<Udhar> _allUdhars = new();
    private List<Customer> _customers = new();
    private int? _selectedCustomerId = null;

    public UdharHistoryPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadCustomers();
        await LoadUdhars();
    }

    private async Task LoadCustomers()
    {
        _customers = await App.Database.GetCustomersAsync();
        CustomerPicker.ItemsSource = _customers.Select(c => c.Name).ToList();
    }

    private async Task LoadUdhars()
    {
        _allUdhars = await App.Database.GetUdharsAsync();

        // Map customer names
        foreach (var u in _allUdhars)
        {
            u.CustomerName = _customers.FirstOrDefault(c => c.Id == u.CustomerId)?.Name ?? "Unknown";
        }

        ApplyFilters();
    }

    private void ApplyFilters()
    {
        var filtered = _allUdhars.AsEnumerable();

        // Customer dropdown filter
        if (_selectedCustomerId.HasValue)
        {
            filtered = filtered.Where(u => u.CustomerId == _selectedCustomerId.Value);
        }

        // Search filter
        var keyword = SearchEntry.Text?.ToLower() ?? "";
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            filtered = filtered.Where(u => u.CustomerName.ToLower().Contains(keyword));
        }

        UdharCollection.ItemsSource = filtered.ToList();
    }

    private void OnCustomerFilterChanged(object sender, EventArgs e)
    {
        var index = CustomerPicker.SelectedIndex;
        if (index >= 0 && index < _customers.Count)
            _selectedCustomerId = _customers[index].Id;
        else
            _selectedCustomerId = null;

        ApplyFilters();
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        ApplyFilters();
    }

    private async void OnMarkPaidClicked(object sender, EventArgs e)
    {
        if (sender is not Button button) return;
        if (button.BindingContext is not Udhar udhar) return;

        bool confirm = await DisplayAlert(
            "Confirm Payment",
            $"Mark ₹{udhar.Amount} as paid?",
            "Yes",
            "Cancel");

        if (!confirm) return;

        await App.Database.MarkUdharPaidAsync(udhar);
        await LoadUdhars();
    }
}
