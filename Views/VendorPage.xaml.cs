using vyaparsathi.Models;
using vyaparsathi.Services;

namespace vyaparsathi.Views;

public partial class VendorPage : ContentPage
{
    private readonly DatabaseService _database;
    private List<Vendor> _vendors;

    public VendorPage()
    {
        InitializeComponent();
        _database = App.Database;
        LoadVendors();
    }

    private async void LoadVendors()
    {
        _vendors = await _database.GetVendorsAsync();
        VendorCollectionView.ItemsSource = _vendors;
    }

    private async void OnSaveVendorClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameEntry.Text))
        {
            await DisplayAlert("Error", "Vendor name is required", "OK");
            return;
        }

        var vendor = new Vendor
        {
            Name = NameEntry.Text.Trim(),
            Phone = PhoneEntry.Text.Trim(),
            Email = EmailEntry.Text.Trim(),
            Address = AddressEditor.Text.Trim()
        };

        await _database.SaveVendorAsync(vendor);
        await DisplayAlert("Success", "Vendor saved successfully", "OK");
        NameEntry.Text = PhoneEntry.Text = EmailEntry.Text = AddressEditor.Text = "";
        LoadVendors();
    }

    private async void OnDeleteVendorClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var vendor = (Vendor)button.CommandParameter;

        var confirm = await DisplayAlert("Confirm", $"Delete vendor {vendor.Name}?", "Yes", "No");
        if (confirm)
        {
            await _database.DeleteVendorAsync(vendor);
            LoadVendors();
        }
    }
}
