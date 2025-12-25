using vyaparsathi.Models;

namespace vyaparsathi.Views;

public partial class UdharHistoryPage : ContentPage
{
    private List<Udhar> _allUdhars = new();

    public UdharHistoryPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadUdhars();
    }

    private async Task LoadUdhars()
    {
        _allUdhars = await App.Database.GetUdharHistoryAsync();

        // TODO: Replace with real customer lookup later
        foreach (var u in _allUdhars)
            u.CustomerName = $"Customer #{u.CustomerId}";

        UdharCollection.ItemsSource = _allUdhars;
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        var keyword = e.NewTextValue?.ToLower() ?? "";

        UdharCollection.ItemsSource = _allUdhars
            .Where(u => u.CustomerName.ToLower().Contains(keyword))
            .ToList();
    }

    private async void OnMarkPaidClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var udhar = button?.BindingContext as Udhar;

        if (udhar == null) return;

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
