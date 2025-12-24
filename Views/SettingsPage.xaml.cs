using vyaparsathi.Models;

namespace vyaparsathi.Views;

public partial class SettingsPage : ContentPage
{
    private BusinessProfile _profile;

    public SettingsPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Load existing profile if any
        _profile = await App.Database.GetBusinessProfileAsync();
        if (_profile != null)
        {
            BusinessNameEntry.Text = _profile.BusinessName;
            OwnerNameEntry.Text = _profile.OwnerName;
            PhoneEntry.Text = _profile.Phone;
            WhatsAppEntry.Text = _profile.WhatsApp;
        }
    }

    // Save business profile
    private async void OnSaveProfileClicked(object sender, EventArgs e)
    {
        string businessName = BusinessNameEntry.Text?.Trim();
        string ownerName = OwnerNameEntry.Text?.Trim();
        string phone = PhoneEntry.Text?.Trim();
        string whatsapp = WhatsAppEntry.Text?.Trim();

        if (string.IsNullOrWhiteSpace(businessName) || string.IsNullOrWhiteSpace(phone))
        {
            await DisplayAlert("Error", "Business name and phone are required.", "OK");
            return;
        }

        if (_profile == null)
            _profile = new BusinessProfile();

        _profile.BusinessName = businessName;
        _profile.OwnerName = ownerName;
        _profile.Phone = phone;
        _profile.WhatsApp = whatsapp;

        await App.Database.SaveBusinessProfileAsync(_profile);
        await DisplayAlert("Success", "Business profile saved.", "OK");
    }

    private async void OnAddCategoryTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddCategoryPage());
    }

    private async void OnAddItemTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddItemPage());
    }

    private void OnSendWhatsAppTapped(object sender, EventArgs e)
    {
        DisplayAlert("Info", "This feature will allow sending messages via WhatsApp.", "OK");
    }
}
