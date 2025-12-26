using SQLite;
using vyaparsathi.Models;

namespace vyaparsathi.Services;

public class DatabaseService
{
    private readonly SQLiteAsyncConnection _database;

    public DatabaseService(string dbPath)
    {
        _database = new SQLiteAsyncConnection(dbPath);

        _database.CreateTableAsync<Vendor>().Wait();
        _database.CreateTableAsync<Purchase>().Wait();
        _database.CreateTableAsync<PurchaseItem>().Wait();
        _database.CreateTableAsync<Category>().Wait();
        _database.CreateTableAsync<Item>().Wait();
        _database.CreateTableAsync<Bill>().Wait();
        _database.CreateTableAsync<BillItem>().Wait();
        _database.CreateTableAsync<Udhar>().Wait();
        _database.CreateTableAsync<BusinessProfile>().Wait();
        _database.CreateTableAsync<Customer>().Wait();
        _database.CreateTableAsync<SmsSettings>().Wait();
    }

    // =======================
    // CATEGORY
    // =======================
    public Task<int> SaveCategoryAsync(Category category)
    {
        category.UpdatedAt = DateTime.UtcNow;
        if (category.Id != 0)
            return _database.UpdateAsync(category);

        category.CreatedAt = DateTime.UtcNow;
        return _database.InsertAsync(category);
    }

    public Task<List<Category>> GetCategoriesAsync() =>
        _database.Table<Category>().OrderByDescending(c => c.CreatedAt).ToListAsync();

    public Task<int> DeleteCategoryAsync(Category category) =>
        _database.DeleteAsync(category);

    // =======================
    // ITEM
    // =======================
    public Task<int> SaveItemAsync(Item item)
    {
        item.UpdatedAt = DateTime.UtcNow;

        if (item.Id != 0)
            return _database.UpdateAsync(item);

        item.CreatedAt = DateTime.UtcNow;
        item.StockQuantity = Math.Max(item.StockQuantity, 0);
        return _database.InsertAsync(item);
    }

    public Task<List<Item>> GetItemsAsync() =>
        _database.Table<Item>().OrderByDescending(i => i.CreatedAt).ToListAsync();

    public Task<Item> GetItemByIdAsync(int id) =>
        _database.Table<Item>().FirstOrDefaultAsync(i => i.Id == id);

    public Task<int> DeleteItemAsync(Item item) =>
        _database.DeleteAsync(item);

    // =======================
    // VENDOR
    // =======================
    public Task<int> SaveVendorAsync(Vendor vendor)
    {
        vendor.UpdatedAt = DateTime.UtcNow;
        if (vendor.Id != 0)
            return _database.UpdateAsync(vendor);

        vendor.CreatedAt = DateTime.UtcNow;
        return _database.InsertAsync(vendor);
    }

    public Task<List<Vendor>> GetVendorsAsync() =>
        _database.Table<Vendor>().OrderByDescending(v => v.CreatedAt).ToListAsync();

    public Task<int> DeleteVendorAsync(Vendor vendor) =>
        _database.DeleteAsync(vendor);

    // =======================
    // PURCHASE
    // =======================
    public Task<int> SavePurchaseAsync(Purchase purchase)
    {
        if (purchase.Id != 0)
            return _database.UpdateAsync(purchase);

        purchase.Date = DateTime.UtcNow;
        return _database.InsertAsync(purchase);
    }

    public Task<List<Purchase>> GetPurchasesAsync() =>
        _database.Table<Purchase>().OrderByDescending(p => p.Date).ToListAsync();

    // =======================
    // PURCHASE ITEM
    // =======================
    public Task<int> SavePurchaseItemAsync(PurchaseItem item)
    {
        if (item.Id != 0)
            return _database.UpdateAsync(item);

        return _database.InsertAsync(item);
    }

    public Task<List<PurchaseItem>> GetPurchaseItemsAsync(int purchaseId) =>
        _database.Table<PurchaseItem>()
            .Where(i => i.PurchaseId == purchaseId)
            .ToListAsync();

    // =======================
    // BILL
    // =======================
    public Task<int> SaveBillAsync(Bill bill)
    {
        if (bill.Id != 0)
            return _database.UpdateAsync(bill);

        bill.Date = DateTime.UtcNow;
        return _database.InsertAsync(bill);
    }

    public Task<List<Bill>> GetBillsAsync() =>
        _database.Table<Bill>().OrderByDescending(b => b.Date).ToListAsync();

    // =======================
    // BILL ITEM
    // =======================
    public Task<int> SaveBillItemAsync(BillItem item)
    {
        if (item.Id != 0)
            return _database.UpdateAsync(item);

        return _database.InsertAsync(item);
    }

    public Task<List<BillItem>> GetBillItemsAsync(int billId) =>
        _database.Table<BillItem>()
            .Where(i => i.BillId == billId)
            .ToListAsync();

    // =======================
    // UDHAR
    // =======================
    public Task<int> SaveUdharAsync(Udhar udhar)
    {
        if (udhar.Id != 0)
            return _database.UpdateAsync(udhar);

        udhar.Date = DateTime.UtcNow;
        return _database.InsertAsync(udhar);
    }

    public async Task<List<Udhar>> GetUdharsAsync()
    {
        var udhars = await _database.Table<Udhar>()
            .OrderByDescending(u => u.Date)
            .ToListAsync();

        var customers = await GetCustomersAsync();

        foreach (var u in udhars)
        {
            u.CustomerName = customers
                .FirstOrDefault(c => c.Id == u.CustomerId)?.Name ?? "Unknown";
        }

        return udhars;
    }

    // ✅ REQUIRED FOR CUSTOMER DETAILS PAGE
    public Task<List<Udhar>> GetUdharsByCustomerAsync(int customerId) =>
        _database.Table<Udhar>()
            .Where(u => u.CustomerId == customerId)
            .OrderByDescending(u => u.Date)
            .ToListAsync();

    public Task<int> DeleteUdharAsync(Udhar udhar) =>
        _database.DeleteAsync(udhar);

    public Task<int> MarkUdharPaidAsync(Udhar udhar)
    {
        udhar.IsPaid = true;
        return _database.UpdateAsync(udhar);
    }

    // =======================
    // CUSTOMER
    // =======================
    public Task<int> SaveCustomerAsync(Customer customer)
    {
        customer.UpdatedAt = DateTime.UtcNow;
        if (customer.Id != 0)
            return _database.UpdateAsync(customer);

        customer.CreatedAt = DateTime.UtcNow;
        return _database.InsertAsync(customer);
    }

    public Task<List<Customer>> GetCustomersAsync() =>
        _database.Table<Customer>()
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

    // ✅ REQUIRED FOR CUSTOMER DETAILS PAGE
    public Task<Customer> GetCustomerByIdAsync(int id) =>
        _database.Table<Customer>()
            .FirstOrDefaultAsync(c => c.Id == id);

    public Task<int> DeleteCustomerAsync(Customer customer) =>
        _database.DeleteAsync(customer);

    // =======================
    // BUSINESS PROFILE
    // =======================
    public Task<int> SaveBusinessProfileAsync(BusinessProfile profile)
    {
        profile.UpdatedAt = DateTime.UtcNow;
        if (profile.Id != 0)
            return _database.UpdateAsync(profile);

        profile.CreatedAt = DateTime.UtcNow;
        return _database.InsertAsync(profile);
    }

    public Task<BusinessProfile> GetBusinessProfileAsync() =>
        _database.Table<BusinessProfile>().FirstOrDefaultAsync();

    // =======================
    // SMS SETTINGS
    // =======================
    public Task<SmsSettings> GetSmsSettingsAsync() =>
        _database.Table<SmsSettings>().FirstOrDefaultAsync();

    public Task<int> SaveSmsSettingsAsync(SmsSettings settings)
    {
        if (settings.Id != 0)
            return _database.UpdateAsync(settings);

        return _database.InsertAsync(settings);
    }

    // =======================
    // RESET DATABASE
    // =======================
    public async Task ResetDatabaseAsync()
    {
        await _database.DropTableAsync<Bill>();
        await _database.DropTableAsync<BillItem>();
        await _database.DropTableAsync<Item>();
        await _database.DropTableAsync<Category>();
        await _database.DropTableAsync<Vendor>();
        await _database.DropTableAsync<Purchase>();
        await _database.DropTableAsync<PurchaseItem>();
        await _database.DropTableAsync<Udhar>();
        await _database.DropTableAsync<Customer>();
        await _database.DropTableAsync<BusinessProfile>();
        await _database.DropTableAsync<SmsSettings>();

        await _database.CreateTableAsync<Bill>();
        await _database.CreateTableAsync<BillItem>();
        await _database.CreateTableAsync<Item>();
        await _database.CreateTableAsync<Category>();
        await _database.CreateTableAsync<Vendor>();
        await _database.CreateTableAsync<Purchase>();
        await _database.CreateTableAsync<PurchaseItem>();
        await _database.CreateTableAsync<Udhar>();
        await _database.CreateTableAsync<Customer>();
        await _database.CreateTableAsync<BusinessProfile>();
        await _database.CreateTableAsync<SmsSettings>();
    }
}
