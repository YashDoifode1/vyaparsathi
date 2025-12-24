using SQLite;
using vyaparsathi.Models;

namespace vyaparsathi.Services;

public class DatabaseService
{
    private readonly SQLiteAsyncConnection _database;

    public DatabaseService(string dbPath)
    {
        _database = new SQLiteAsyncConnection(dbPath);

        // Tables
        _database.CreateTableAsync<Category>().Wait();
        _database.CreateTableAsync<Item>().Wait();
        _database.CreateTableAsync<Bill>().Wait();
        _database.CreateTableAsync<Udhar>().Wait();
    }

    // CATEGORY CRUD
    public Task<int> SaveCategoryAsync(Category category)
    {
        category.UpdatedAt = DateTime.UtcNow;

        if (category.Id != 0)
            return _database.UpdateAsync(category);

        category.CreatedAt = DateTime.UtcNow;
        return _database.InsertAsync(category);
    }

    public Task<List<Category>> GetCategoriesAsync() =>
        _database.Table<Category>()
                 .OrderByDescending(c => c.CreatedAt)
                 .ToListAsync();

    public Task<int> DeleteCategoryAsync(Category category) =>
        _database.DeleteAsync(category);

    // ITEM CRUD
    public Task<int> SaveItemAsync(Item item)
    {
        item.UpdatedAt = DateTime.UtcNow;

        if (item.Id != 0)
            return _database.UpdateAsync(item);

        item.CreatedAt = DateTime.UtcNow;
        return _database.InsertAsync(item);
    }

    public Task<List<Item>> GetItemsAsync() =>
        _database.Table<Item>()
                 .OrderByDescending(i => i.CreatedAt)
                 .ToListAsync();

    public Task<int> DeleteItemAsync(Item item) =>
        _database.DeleteAsync(item);

    // BILL
    public Task<int> SaveBillAsync(Bill bill)
    {
        if (bill.Id != 0)
            return _database.UpdateAsync(bill);

        return _database.InsertAsync(bill);
    }

    public Task<List<Bill>> GetBillsAsync() =>
        _database.Table<Bill>().ToListAsync();

    // UDHAR
    public Task<int> SaveUdharAsync(Udhar udhar)
    {
        if (udhar.Id != 0)
            return _database.UpdateAsync(udhar);

        return _database.InsertAsync(udhar);
    }

    public Task<List<Udhar>> GetUdharsAsync() =>
        _database.Table<Udhar>().ToListAsync();
}
