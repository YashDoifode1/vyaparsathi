using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using vyaparsathi.Models;
using vyaparsathi.Views; // ← ADD THIS LINE

namespace vyaparsathi.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    public ObservableCollection<KpiItem> KpiItems { get; } = new();

    [ObservableProperty]
    private bool isRefreshing;

    public DashboardViewModel()
    {
        // Placeholder data until loaded
        RefreshKpiItems(0m, 0m, 0, 0);
    }

    [RelayCommand]
    private async Task CreateBill()
    {
        await Shell.Current.GoToAsync(nameof(BillingPage));
    }

    [RelayCommand]
    private async Task ManageStock()
    {
        await Shell.Current.GoToAsync(nameof(StockPage));
    }

    public async Task LoadDashboardDataAsync()
    {
        IsRefreshing = true;

        try
        {
            var billsTask = App.Database.GetBillsAsync();
            var udharsTask = App.Database.GetUdharsAsync();
            var itemsTask = App.Database.GetItemsAsync();

            await Task.WhenAll(billsTask, udharsTask, itemsTask);

            var bills = await billsTask;
            var udhars = await udharsTask;
            var items = await itemsTask;

            // Calculations using decimal for accuracy
            decimal todaySales = bills
                .Where(b => b.Date.Date == DateTime.Today)
                .Sum(b => b.TotalAmount); // assuming TotalAmount is decimal

            decimal totalUdhar = udhars.Sum(u => u.Amount); // assuming Amount is decimal

            int lowStockCount = items.Count(i => i.StockQuantity <= 5);
            int totalProducts = items.Count;

            RefreshKpiItems(todaySales, totalUdhar, lowStockCount, totalProducts);
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Error",
                "Failed to load dashboard data. Please try again.",
                "OK");
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    private void RefreshKpiItems(decimal todaySales, decimal totalUdhar, int lowStockCount, int totalProducts)
    {
        KpiItems.Clear();

        KpiItems.Add(new KpiItem
        {
            Title = "Today's Sales",
            Value = $"₹{todaySales:N0}",
            ValueColor = Color.FromArgb("#16A34A"),
            Icon = "sales_icon.png"
        });

        KpiItems.Add(new KpiItem
        {
            Title = "Total Udhar",
            Value = $"₹{totalUdhar:N0}",
            ValueColor = Color.FromArgb("#DC2626"),
            Icon = "udhar_icon.png"
        });

        KpiItems.Add(new KpiItem
        {
            Title = "Low Stock Items",
            Value = lowStockCount.ToString(),
            ValueColor = Color.FromArgb("#F59E0B"),
            Icon = "lowstock_icon.png"
        });

        KpiItems.Add(new KpiItem
        {
            Title = "Total Products",
            Value = totalProducts.ToString(),
            ValueColor = Color.FromArgb("#2563EB"),
            Icon = "products_icon.png"
        });
    }
}