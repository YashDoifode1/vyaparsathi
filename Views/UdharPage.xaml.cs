using vyaparsathi.Models;

namespace vyaparsathi.Views;

public partial class UdharPage : ContentPage
{
    private List<Customer> _customers;
    private List<Udhar> _udhars;

    public UdharPage()
    {
        InitializeComponent();
        LoadCustomers();
        LoadUdhars();
    }

    private async void LoadCustomers()
    {
        _customers = await App.Database.GetCustomersAsync();
        CustomerPicker.ItemsSource = _customers.Select(c => c.Name).ToList();
    }

    private async void LoadUdhars()
    {
        _udhars = await App.Database.GetUdharsAsync();
        UdharCollection.ItemsSource = _udhars;
    }

    private async void OnSaveUdharClicked(object sender, EventArgs e)
    {
        if (CustomerPicker.SelectedIndex < 0 || string.IsNullOrWhiteSpace(AmountEntry.Text))
        {
            await DisplayAlert("Error", "Please select a customer and enter an amount.", "OK");
            return;
        }

        var udhar = new Udhar
        {
            CustomerId = _customers[CustomerPicker.SelectedIndex].Id,
            Amount = decimal.Parse(AmountEntry.Text),
            Notes = NotesEditor.Text,
            Date = DateTime.UtcNow
        };

        await App.Database.SaveUdharAsync(udhar);

        AmountEntry.Text = "";
        NotesEditor.Text = "";
        CustomerPicker.SelectedIndex = -1;

        LoadUdhars();
        await DisplayAlert("Success", "Udhar entry saved successfully!", "OK");
    }

    private async void OnDeleteUdharClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var udhar = button?.CommandParameter as Udhar;
        if (udhar == null)
            return;

        bool confirm = await DisplayAlert("Confirm", $"Delete Udhar for {udhar.CustomerName}?", "Yes", "No");
        if (!confirm)
            return;

        await App.Database.DeleteUdharAsync(udhar);
        LoadUdhars();
    }
}
