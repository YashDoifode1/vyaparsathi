namespace vyaparsathi.Views;

public partial class CustomerDetailsPage : ContentPage
{
    private readonly int _customerId;

    public CustomerDetailsPage(int customerId)
    {
        InitializeComponent();
        _customerId = customerId;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var customer = await App.Database.GetCustomerByIdAsync(_customerId);
        CustomerNameLabel.Text = customer.Name;

        var udhars = await App.Database.GetUdharsByCustomerAsync(_customerId);

        UdharCollection.ItemsSource = udhars.Select(u => new
        {
            u.Amount,
            u.Date,
            Status = u.IsPaid ? "Paid" : "Due",
            StatusColor = u.IsPaid ? Colors.Green : Colors.Red
        });
    }
}
