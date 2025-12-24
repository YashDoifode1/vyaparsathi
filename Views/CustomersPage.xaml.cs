using vyaparsathi.Models;

namespace vyaparsathi.Views;

public partial class CustomersPage : ContentPage
{
    private List<Customer> _customers = new List<Customer>();

    public CustomersPage()
    {
        InitializeComponent();
        LoadCustomers();
    }

    private async void LoadCustomers()
    {
        _customers = await App.Database.GetCustomersAsync();
        CustomersCollection.ItemsSource = _customers;
    }

    private async void OnSaveCustomerClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameEntry.Text))
        {
            await DisplayAlert("Error", "Customer name is required.", "OK");
            return;
        }

        var customer = new Customer
        {
            Name = NameEntry.Text,
            PhoneNumber = PhoneEntry.Text,
            Gender = GenderPicker.SelectedItem?.ToString() ?? "",
            Address = AddressEditor.Text
        };

        await App.Database.SaveCustomerAsync(customer);

        NameEntry.Text = "";
        PhoneEntry.Text = "";
        GenderPicker.SelectedIndex = -1;
        AddressEditor.Text = "";

        LoadCustomers();
    }

    private async void OnDeleteCustomerClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var customer = button?.CommandParameter as Customer;

        if (customer == null)
            return;

        var confirm = await DisplayAlert("Confirm", $"Delete customer {customer.Name}?", "Yes", "No");
        if (!confirm)
            return;

        await App.Database.DeleteCustomerAsync(customer);
        LoadCustomers();
    }
}
