using vyaparsathi.Models;

namespace vyaparsathi.Views;

public partial class AddCustomerPage : ContentPage
{
    public AddCustomerPage()
    {
        InitializeComponent();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameEntry.Text))
        {
            await DisplayAlert("Error", "Customer name is required", "OK");
            return;
        }

        var customer = new Customer
        {
            Name = NameEntry.Text.Trim(),
            PhoneNumber = PhoneEntry.Text?.Trim(),
            Gender = GenderPicker.SelectedItem?.ToString(),
            Address = AddressEditor.Text?.Trim()
        };

        await App.Database.SaveCustomerAsync(customer);

        await DisplayAlert("Success", "Customer added successfully", "OK");

        await Navigation.PopAsync(); // 🔙 Go back to Customers page
    }
}
