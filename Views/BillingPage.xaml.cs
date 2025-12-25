using vyaparsathi.Models;
using System.Collections.ObjectModel;

namespace vyaparsathi.Views;

public partial class BillingPage : ContentPage
{
    private List<Category> _categories;
    private List<Item> _items;
    private List<Customer> _customers;
    private Customer _selectedCustomer;

    private ObservableCollection<BillItem> _billItems = new();

    public BillingPage()
    {
        InitializeComponent();
        BillItemsCollection.ItemsSource = _billItems;

        LoadCategories();
        LoadCustomers();
        UpdateTotal();
    }

    private async void LoadCustomers()
    {
        _customers = await App.Database.GetCustomersAsync();
        CustomerPicker.ItemsSource = _customers.Select(c => c.Name).ToList();
    }

    private void OnCustomerSelected(object sender, EventArgs e)
    {
        if (CustomerPicker.SelectedIndex < 0)
            return;

        _selectedCustomer = _customers[CustomerPicker.SelectedIndex];
        CustomerNameEntry.Text = _selectedCustomer.Name;
    }

    private async void LoadCategories()
    {
        _categories = await App.Database.GetCategoriesAsync();
        CategoryPicker.ItemsSource = _categories.Select(c => c.Name).ToList();
    }

    private async void OnCategoryChanged(object sender, EventArgs e)
    {
        if (CategoryPicker.SelectedIndex < 0)
            return;

        var selectedCategory = _categories[CategoryPicker.SelectedIndex];

        _items = (await App.Database.GetItemsAsync())
            .Where(i => i.CategoryId == selectedCategory.Id)
            .ToList();

        ItemPicker.ItemsSource = _items.Select(i => i.Name).ToList();
    }

    private void OnItemChanged(object sender, EventArgs e)
    {
        if (ItemPicker.SelectedIndex < 0)
            return;

        PriceEntry.Text = _items[ItemPicker.SelectedIndex]
            .SellingPrice.ToString("F2");
    }

    private void OnAddItemClicked(object sender, EventArgs e)
    {
        if (ItemPicker.SelectedIndex < 0)
            return;

        var item = new BillItem
        {
            ItemName = ItemPicker.SelectedItem.ToString(),
            Quantity = decimal.Parse(QuantityEntry.Text),
            Price = decimal.Parse(PriceEntry.Text)
        };

        _billItems.Add(item);
        UpdateTotal();

        QuantityEntry.Text = "";
        PriceEntry.Text = "";
        ItemPicker.SelectedIndex = -1;
    }

    private void UpdateTotal()
    {
        TotalLabel.Text = $"Total: ₹{_billItems.Sum(i => i.Total):F2}";
    }

    private async void OnSaveBillClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(CustomerNameEntry.Text) || _billItems.Count == 0)
        {
            await DisplayAlert("Error", "Customer name and items required.", "OK");
            return;
        }

        var bill = new Bill
        {
            CustomerName = CustomerNameEntry.Text,
            Date = BillDatePicker.Date
        };

        await App.Database.SaveBillAsync(bill);

        foreach (var item in _billItems)
        {
            item.BillId = bill.Id;
            await App.Database.SaveBillItemAsync(item);
        }

        if (UdharCheckBox.IsChecked && _selectedCustomer != null)
        {
            var udhar = new Udhar
            {
                CustomerId = _selectedCustomer.Id,
                Amount = _billItems.Sum(i => i.Total),
                IsPaid = false
            };

            await App.Database.SaveUdharAsync(udhar);
        }

        await DisplayAlert("Success", "Bill saved successfully.", "OK");

        CustomerNameEntry.Text = "";
        CustomerPicker.SelectedIndex = -1;
        UdharCheckBox.IsChecked = false;
        _billItems.Clear();
        UpdateTotal();
    }

    private void OnPrintClicked(object sender, EventArgs e)
    {
        DisplayAlert("Print", "Printing bill...", "OK");
    }

    private void OnShareClicked(object sender, EventArgs e)
    {
        DisplayAlert("Share", "Sharing bill...", "OK");
    }


}
