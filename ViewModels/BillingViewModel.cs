using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using vyaparsathi.Models;

namespace vyaparsathi.ViewModels;

public partial class BillingViewModel : ObservableObject
{
    public ObservableCollection<Customer> Customers { get; } = new();
    public ObservableCollection<Item> AllItems { get; } = new();
    public ObservableCollection<BillItemViewModel> BillItems { get; } = new();

    [ObservableProperty]
    private ObservableCollection<Customer> filteredCustomers;

    [ObservableProperty]
    private ObservableCollection<Item> filteredItems;

    [ObservableProperty]
    private string customerSearchText = "";

    [ObservableProperty]
    private string itemSearchText = "";

    [ObservableProperty]
    private Customer? selectedCustomer;

    [ObservableProperty]
    private Item? selectedItemToAdd;

    [ObservableProperty]
    private bool isUdhar;

    [ObservableProperty]
    private DateTime billDate = DateTime.Today;

    [ObservableProperty]
    private string quantityText = "1";

    [ObservableProperty]
    private string priceText = "";

    [ObservableProperty]
    private bool isRefreshing;

    public decimal TotalAmount => BillItems.Sum(i => i.Total);

    public BillingViewModel()
    {
        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
        AddItemCommand = new RelayCommand(AddItem);
        RemoveItemCommand = new RelayCommand<BillItemViewModel>(RemoveItem);
        SaveBillCommand = new AsyncRelayCommand(SaveBillAsync);
        PrintBillCommand = new RelayCommand(PrintBill);
        ShareBillCommand = new RelayCommand(ShareBill);

        FilteredCustomers = new ObservableCollection<Customer>(Customers);
        FilteredItems = new ObservableCollection<Item>(AllItems);
    }

    public IAsyncRelayCommand LoadDataCommand { get; }
    public IRelayCommand AddItemCommand { get; }
    public IRelayCommand<BillItemViewModel> RemoveItemCommand { get; }
    public IAsyncRelayCommand SaveBillCommand { get; }
    public IRelayCommand PrintBillCommand { get; }
    public IRelayCommand ShareBillCommand { get; }

    private async Task LoadDataAsync()
    {
        IsRefreshing = true;

        var customersTask = App.Database.GetCustomersAsync();
        var itemsTask = App.Database.GetItemsAsync();

        await Task.WhenAll(customersTask, itemsTask);

        var customers = await customersTask;
        var items = await itemsTask;

        Customers.Clear();
        foreach (var c in customers) Customers.Add(c);

        AllItems.Clear();
        foreach (var i in items) AllItems.Add(i);

        ApplyFilters();

        IsRefreshing = false;
    }

    partial void OnCustomerSearchTextChanged(string value) => ApplyCustomerFilter(value);

    partial void OnItemSearchTextChanged(string value) => ApplyItemFilter(value);

    partial void OnSelectedItemToAddChanged(Item? value)
    {
        if (value != null)
        {
            PriceText = value.SellingPrice.ToString("N0");
        }
    }

    private void ApplyCustomerFilter(string search)
    {
        var query = string.IsNullOrWhiteSpace(search)
            ? Customers
            : Customers.Where(c => c.Name.Contains(search, StringComparison.OrdinalIgnoreCase));

        FilteredCustomers.Clear();
        foreach (var c in query) FilteredCustomers.Add(c);
    }

    private void ApplyItemFilter(string search)
    {
        var query = string.IsNullOrWhiteSpace(search)
            ? AllItems
            : AllItems.Where(i => i.Name.Contains(search, StringComparison.OrdinalIgnoreCase));

        FilteredItems.Clear();
        foreach (var i in query) FilteredItems.Add(i);
    }

    private void ApplyFilters()
    {
        ApplyCustomerFilter(CustomerSearchText);
        ApplyItemFilter(ItemSearchText);
    }

    private void AddItem()
    {
        if (selectedItemToAdd == null) return;

        if (!int.TryParse(QuantityText, out int qty) || qty <= 0)
        {
            Application.Current?.MainPage?.DisplayAlert("Invalid Quantity", "Please enter a valid quantity.", "OK");
            return;
        }

        if (qty > selectedItemToAdd.StockQuantity)
        {
            Application.Current?.MainPage?.DisplayAlert("Insufficient Stock", $"Only {selectedItemToAdd.StockQuantity} available.", "OK");
            return;
        }

        decimal price = string.IsNullOrWhiteSpace(PriceText)
            ? selectedItemToAdd.SellingPrice
            : decimal.Parse(PriceText);

        var billItem = new BillItemViewModel
        {
            ItemName = selectedItemToAdd.Name,
            Quantity = qty,
            Price = price
        };

        BillItems.Add(billItem);
        OnPropertyChanged(nameof(TotalAmount));

        // Reset inputs
        SelectedItemToAdd = null;
        QuantityText = "1";
        PriceText = "";
    }

    private void RemoveItem(BillItemViewModel? item)
    {
        if (item != null)
        {
            BillItems.Remove(item);
            OnPropertyChanged(nameof(TotalAmount));
        }
    }

    private async Task SaveBillAsync()
    {
        if (string.IsNullOrWhiteSpace(customerSearchText) && selectedCustomer == null)
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Please enter or select a customer name.", "OK");
            return;
        }

        if (BillItems.Count == 0)
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Please add at least one item to the bill.", "OK");
            return;
        }

        string customerName = selectedCustomer?.Name ?? customerSearchText;

        var bill = new Bill
        {
            CustomerName = customerName,
            Date = BillDate,
            TotalAmount = TotalAmount
        };

        await App.Database.SaveBillAsync(bill);

        foreach (var bItem in BillItems)
        {
            var dbBillItem = new BillItem
            {
                BillId = bill.Id,
                ItemName = bItem.ItemName,
                Quantity = bItem.Quantity,
                Price = bItem.Price
                // Total is read-only — do NOT assign it here
            };

            await App.Database.SaveBillItemAsync(dbBillItem);

            // Update stock
            var stockItem = AllItems.FirstOrDefault(i => i.Name == bItem.ItemName);
            if (stockItem != null)
            {
                stockItem.StockQuantity -= bItem.Quantity;
                await App.Database.SaveItemAsync(stockItem);
            }
        }

        // Save Udhar if marked and customer exists in database
        if (IsUdhar && selectedCustomer != null)
        {
            var udhar = new Udhar
            {
                CustomerId = selectedCustomer.Id,
                Amount = TotalAmount,
                Date = BillDate,
                IsPaid = false
            };
            await App.Database.SaveUdharAsync(udhar);
        }

        await Application.Current.MainPage.DisplayAlert("Success", "Bill saved successfully!", "OK");

        // Clear the form
        BillItems.Clear();
        SelectedCustomer = null;
        CustomerSearchText = "";
        ItemSearchText = "";
        IsUdhar = false;
        QuantityText = "1";
        PriceText = "";
        SelectedItemToAdd = null;
        OnPropertyChanged(nameof(TotalAmount));
    }

    private void PrintBill()
    {
        Application.Current?.MainPage?.DisplayAlert("Print", "Printing bill... (implement printer integration)", "OK");
    }

    private void ShareBill()
    {
        Application.Current?.MainPage?.DisplayAlert("Share", "Sharing bill... (implement share functionality)", "OK");
    }
}

public class BillItemViewModel : ObservableObject
{
    public string ItemName { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public decimal Total => Quantity * Price;
}