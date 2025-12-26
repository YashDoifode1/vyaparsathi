using System.Windows.Input;
using vyaparsathi.Models;

namespace vyaparsathi.Views;

public partial class CustomersPage : ContentPage
{
    private List<CustomerViewModel> _allCustomers = new();

    public CustomersPage()
    {
        InitializeComponent();
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadCustomersAsync();
    }

    private async Task LoadCustomersAsync()
    {
        var customers = await App.Database.GetCustomersAsync();
        var udhars = await App.Database.GetUdharsAsync();

        _allCustomers = customers.Select(c =>
        {
            var pending = udhars
                .Where(u => u.CustomerId == c.Id && !u.IsPaid)
                .ToList();

            decimal due = pending.Sum(u => u.Amount);
            bool overdue = pending.Any(u => u.DueDate < DateTime.Today);

            return new CustomerViewModel
            {
                Id = c.Id,
                Name = c.Name,
                PhoneNumber = c.PhoneNumber,

                DueAmount = due,
                DueLabel = due > 0 ? "Due" : "Paid",
                DueStatus = overdue ? "Overdue" : "Active",

                AmountColor = due > 0 ? Colors.Red : Colors.Green,
                StatusColor = overdue ? Colors.Red : Colors.Green
            };
        }).ToList();

        CustomersCollection.ItemsSource = _allCustomers;
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        var text = e.NewTextValue?.ToLower() ?? "";

        CustomersCollection.ItemsSource = _allCustomers
            .Where(c =>
                c.Name.ToLower().Contains(text) ||
                c.PhoneNumber.Contains(text))
            .ToList();
    }

    private async void OnAddCustomerClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddCustomerPage());
    }

    public ICommand OpenCustomerCommand => new Command<CustomerViewModel>(async customer =>
    {
        await Navigation.PushAsync(new CustomerDetailsPage(customer.Id));
    });
}
