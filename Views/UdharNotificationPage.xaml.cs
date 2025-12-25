using vyaparsathi.Models;
using vyaparsathi.Services;
using System.Net.Http;
using System.Web;

namespace vyaparsathi.Views;

public partial class UdharNotificationPage : ContentPage
{
    private List<Customer> _customers = new();

    public UdharNotificationPage()
    {
        InitializeComponent();
        LoadCustomers();
    }

    private async void LoadCustomers()
    {
        _customers = await App.Database.GetCustomersAsync();
        CustomerPicker.ItemsSource = _customers.Select(c => c.Name).ToList();
    }

    private async void OnSendSmsClicked(object sender, EventArgs e)
    {
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

    // Example using HttpClient to send SMS via an API
    private async Task<bool> SendSmsAsync(string phoneNumber, string message)
    {
        try
        {
            using var client = new HttpClient();

            // Replace below with your actual SMS API details
            string apiKey = "YOUR_API_KEY";
            string senderId = "VYAPSATHI";
            string encodedMessage = HttpUtility.UrlEncode(message);

            string url = $"https://api.yoursmsgateway.com/send?apiKey={apiKey}&to={phoneNumber}&sender={senderId}&message={encodedMessage}";

            var response = await client.GetAsync(url);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
