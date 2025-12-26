using vyaparsathi.Models;
using vyaparsathi.Services;
using System.Net.Http;
using System.Web;

namespace vyaparsathi.Views;

public partial class UdharNotificationPage : ContentPage
{
    private List<Customer> _customers = new();
    private BusinessProfile _businessProfile;

    public UdharNotificationPage()
    {
        InitializeComponent();
        LoadBusinessProfile();
        LoadCustomers();
    }

    private async void LoadBusinessProfile()
    {
        // Assuming only one business profile exists
        _businessProfile = await App.Database.GetBusinessProfileAsync();
        if (_businessProfile == null)
        {
            await DisplayAlert("Error", "Business profile not found. Configure API credentials.", "OK");
        }
    }

    private async void LoadCustomers()
    {
        _customers = await App.Database.GetCustomersAsync();
        CustomerPicker.ItemsSource = _customers.Select(c => c.Name).ToList();
    }

    private async void OnSendSmsClicked(object sender, EventArgs e)
    {
        if (_businessProfile == null)
        {
            await DisplayAlert("Error", "Business profile not configured.", "OK");
            return;
        }

        if (CustomerPicker.SelectedIndex < 0)
        {
            await DisplayAlert("Error", "Please select a customer.", "OK");
            return;
        }

        var selectedCustomer = _customers[CustomerPicker.SelectedIndex];
        string message = MessageEditor.Text?.Trim();

        if (string.IsNullOrEmpty(message))
        {
            await DisplayAlert("Error", "Please enter a message.", "OK");
            return;
        }

        try
        {
            StatusLabel.Text = "Sending SMS...";
            bool success = await SendSmsAsync(selectedCustomer.PhoneNumber, message);

            if (success)
            {
                await DisplayAlert("Success", $"SMS sent to {selectedCustomer.Name}", "OK");
                StatusLabel.Text = "SMS sent successfully!";
                MessageEditor.Text = "";
            }
            else
            {
                await DisplayAlert("Error", "Failed to send SMS. Check API credentials.", "OK");
                StatusLabel.Text = "SMS failed.";
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error sending SMS: {ex.Message}", "OK");
            StatusLabel.Text = "SMS failed.";
        }
    }

    private async Task<bool> SendSmsAsync(string phoneNumber, string message)
    {
        try
        {
            if (_businessProfile == null) return false;

            using var client = new HttpClient();

            string apiKey = _businessProfile.SmsApiKey;
            string senderId = _businessProfile.SmsSenderId ?? "VYAPSATHI";
            string smsUrl = _businessProfile.SmsApiUrl;

            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(senderId) || string.IsNullOrEmpty(smsUrl))
                return false;

            string encodedMessage = HttpUtility.UrlEncode(message);

            // Build request URL using the business profile details
            string url = $"{smsUrl}?apiKey={apiKey}&to={phoneNumber}&sender={senderId}&message={encodedMessage}";

            var response = await client.GetAsync(url);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
