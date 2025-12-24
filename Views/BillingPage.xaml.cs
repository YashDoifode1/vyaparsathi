using vyaparsathi.Models;
using System.Collections.ObjectModel;

namespace vyaparsathi.Views;

public partial class BillingPage : ContentPage
{
    private List<Category> _categories;
    private List<Item> _items;
    private ObservableCollection<BillItem> _billItems = new ObservableCollection<BillItem>();

    public BillingPage()
    {
        InitializeComponent();
        BillItemsCollection.ItemsSource = _billItems;
        LoadCategories();
        UpdateTotal();
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
        ItemPicker.SelectedIndex = -1;
        PriceEntry.Text = "";
    }

    private void OnItemChanged(object sender, EventArgs e)
    {
        if (ItemPicker.SelectedIndex < 0)
            return;

        var selectedItem = _items[ItemPicker.SelectedIndex];
        PriceEntry.Text = selectedItem.SellingPrice.ToString("F2"); // auto-fill selling price
    }

    private void UpdateTotal()
    {
        decimal total = _billItems.Sum(i => i.Total);
        TotalLabel.Text = $"Total: ₹{total:F2}";
    }

    private void OnAddItemClicked(object sender, EventArgs e)
    {
        if (ItemPicker.SelectedIndex < 0 || string.IsNullOrWhiteSpace(QuantityEntry.Text) || string.IsNullOrWhiteSpace(PriceEntry.Text))
            return;

        var billItem = new BillItem
        {
            ItemName = ItemPicker.SelectedItem.ToString(),
            Quantity = decimal.Parse(QuantityEntry.Text),
            Price = decimal.Parse(PriceEntry.Text)
        };

        _billItems.Add(billItem);

        // Clear inputs
        QuantityEntry.Text = "";
        PriceEntry.Text = "";
        ItemPicker.SelectedIndex = -1;

        UpdateTotal();
    }

    private async void OnSaveBillClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(CustomerNameEntry.Text) || _billItems.Count == 0)
        {
            await DisplayAlert("Error", "Enter customer name and add at least one item.", "OK");
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

        await DisplayAlert("Success", "Bill saved successfully!", "OK");

        CustomerNameEntry.Text = "";
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
