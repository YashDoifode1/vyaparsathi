using SQLite;
using vyaparsathi.Models;

namespace vyaparsathi.Services;

public class DatabaseService
{
    private readonly SQLiteAsyncConnection _database;

    public DatabaseService(string dbPath)
    {
        _database = new SQLiteAsyncConnection(dbPath);

        // Create Tables
        _database.CreateTableAsync<Category>().Wait();
        _database.CreateTableAsync<Item>().Wait();
        _database.CreateTableAsync<Bill>().Wait();
        _database.CreateTableAsync<BillItem>().Wait();
        _database.CreateTableAsync<Udhar>().Wait();
        _database.CreateTableAsync<BusinessProfile>().Wait();
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
        return _database.InsertAsync(item);
    }

    public Task<List<Item>> GetItemsAsync() =>
        _database.Table<Item>().OrderByDescending(i => i.CreatedAt).ToListAsync();

    public Task<int> DeleteItemAsync(Item item) =>
        _database.DeleteAsync(item);

    // =======================
    // BILL
    // =======================
    public Task<int> SaveBillAsync(Bill bill)
    {
        if (bill.Id != 0)
            return _database.UpdateAsync(bill);

        return _database.InsertAsync(bill);
    }

    public Task<List<Bill>> GetBillsAsync() =>
        _database.Table<Bill>().OrderByDescending(b => b.Date).ToListAsync();

    // =======================
    // BILL ITEMS
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

        return _database.InsertAsync(udhar);
    }

    public Task<List<Udhar>> GetUdharsAsync() =>
        _database.Table<Udhar>().ToListAsync();

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
}
