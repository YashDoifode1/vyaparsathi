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

        // Load existing business profile
        _profile = await App.Database.GetBusinessProfileAsync();
        if (_profile != null)
        {
            BusinessNameEntry.Text = _profile.BusinessName;
            OwnerNameEntry.Text = _profile.OwnerName;
            PhoneEntry.Text = _profile.Phone;
            WhatsAppEntry.Text = _profile.WhatsApp;
        }

        // Load saved SMS settings
        var smsSettings = await App.Database.GetSmsSettingsAsync();
        if (smsSettings != null)
        {
            ApiKeyEntry.Text = smsSettings.ApiKey;
            SenderIdEntry.Text = smsSettings.SenderId;
            SmsUrlEntry.Text = smsSettings.Url;
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

    // Navigate to Add Category page
    private async void OnAddCategoryTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddCategoryPage());
    }

    // Navigate to Add Item page
    private async void OnAddItemTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddItemPage());
    }

    // Placeholder for WhatsApp integration
    private void OnSendWhatsAppTapped(object sender, EventArgs e)
    {
        DisplayAlert("Info", "This feature will allow sending messages via WhatsApp.", "OK");
    }

    // Save SMS settings
    private async void OnSaveSmsSettingsClicked(object sender, EventArgs e)
    {
        string apiKey = ApiKeyEntry.Text?.Trim();
        string senderId = SenderIdEntry.Text?.Trim();
        string url = SmsUrlEntry.Text?.Trim();

        if (string.IsNullOrWhiteSpace(apiKey) ||
            string.IsNullOrWhiteSpace(senderId) ||
            string.IsNullOrWhiteSpace(url))
        {
            await DisplayAlert("Error", "All SMS settings are required.", "OK");
            return;
        }

        var smsSettings = new SmsSettings
        {
            ApiKey = apiKey,
            SenderId = senderId,
            Url = url
        };

        await App.Database.SaveSmsSettingsAsync(smsSettings);
        await DisplayAlert("Success", "SMS settings saved.", "OK");
    }

    // Reset database
    private async void OnResetDatabaseClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert(
            "Confirm Reset",
            "This will erase all app data. Are you sure?",
            "Yes",
            "Cancel");

        if (!confirm) return;

        await App.Database.ResetDatabaseAsync();
        await DisplayAlert("Success", "Database has been reset.", "OK");
    }
}
