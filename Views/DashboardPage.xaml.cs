using vyaparsathi.Models;

namespace vyaparsathi.Views;

public partial class DashboardPage : ContentPage
{
    public DashboardPage()
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
        var db = App.Database;

        var bills = await db.GetBillsAsync();
        var billItems = new List<BillItem>();

        foreach (var bill in bills)
        {
            var items = await db.GetBillItemsAsync(bill.Id);
            billItems.AddRange(items);
        }

        var itemsList = await db.GetItemsAsync();
        var udhars = await db.GetUdharsAsync();
        var customers = await db.GetCustomersAsync();
        var vendors = await db.GetVendorsAsync();

        // TODAY SALES
        // ✅ Timezone-safe today's sales
        var today = DateTime.Now.Date;

        decimal todaySales = bills
            .Where(b => b.Date.ToLocalTime().Date == today)
            .Sum(b => b.TotalAmount);

        TodaySalesLabel.Text = $"₹{todaySales:N2}";

        // TOTAL PROFIT (Selling - Landing) * Sold Qty
        decimal totalProfit = 0;

        foreach (var bi in billItems)
        {
            var item = itemsList.FirstOrDefault(i => i.Name == bi.ItemName);
            if (item == null) continue;

            var profitPerUnit = item.SellingPrice - item.LandingPrice;
            totalProfit += profitPerUnit * bi.Quantity;
        }

        TotalProfitLabel.Text = $"₹{totalProfit:N2}";

        // STOCK VALUE (Landing price * quantity)
        decimal stockValue = itemsList.Sum(i => i.LandingPrice * i.StockQuantity);
        StockValueLabel.Text = $"₹{stockValue:N2}";

        // TOTAL UDHAR
        decimal totalUdhar = udhars.Where(u => !u.IsPaid).Sum(u => u.Amount);
        TotalUdharLabel.Text = $"₹{totalUdhar:N2}";

        // COUNTS
        TotalCustomersLabel.Text = customers.Count.ToString();
        TotalVendorsLabel.Text = vendors.Count.ToString();
    }

    private async void OnCreateBillClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(BillingPage));
    }

    private async void OnStockClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(StockPage));
    }
}
