using vyaparsathi.Models;

namespace vyaparsathi.Views;

public partial class UdharDashboardPage : ContentPage
{
    public UdharDashboardPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadDashboardAsync();
    }

    private async Task LoadDashboardAsync()
    {
        var udhars = (await App.Database.GetUdharsAsync())
            .Where(u => !u.IsPaid)
            .ToList();

        var customers = await App.Database.GetCustomersAsync();

        // Map customer names
        foreach (var u in udhars)
        {
            u.CustomerName = customers
                .FirstOrDefault(c => c.Id == u.CustomerId)?.Name ?? "Unknown";
        }

        decimal totalPending = udhars.Sum(u => u.Amount);
        decimal overdueAmount = udhars
            .Where(u => u.DueDate < DateTime.Today)
            .Sum(u => u.Amount);

        decimal dueThisWeek = udhars
            .Where(u => u.DueDate >= DateTime.Today &&
                        u.DueDate <= DateTime.Today.AddDays(7))
            .Sum(u => u.Amount);

        int customerDueCount = udhars
            .Select(u => u.CustomerId)
            .Distinct()
            .Count();

        // UI
        TotalPendingLabel.Text = $"₹{totalPending:N0}";
        OverdueAmountLabel.Text = $"₹{overdueAmount:N0}";
        DueThisWeekLabel.Text = $"₹{dueThisWeek:N0}";
        CustomerDueCountLabel.Text = customerDueCount.ToString();

        // Recent Udhar (top 5)
        RecentUdharCollection.ItemsSource = udhars
            .OrderBy(u => u.DueDate)
            .Take(5)
            .Select(u => new
            {
                u.CustomerName,
                u.Amount,
                DueText = u.DueDate < DateTime.Today
                    ? "Overdue"
                    : $"Due {u.DueDate:dd MMM}",
                StatusColor = u.DueDate < DateTime.Today
                    ? Colors.Red
                    : Colors.Orange
            })
            .ToList();
    }

    private async void OnViewAllClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new UdharHistoryPage());
    }
}
