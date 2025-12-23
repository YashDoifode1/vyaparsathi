using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;
using vyaparsathi.Models;

namespace vyaparsathi.Services;

public class DatabaseService
{
    private readonly SQLiteAsyncConnection _db;

    public DatabaseService(string dbPath)
    {
        _db = new SQLiteAsyncConnection(dbPath);
        _db.CreateTableAsync<Item>().Wait();
        _db.CreateTableAsync<Bill>().Wait();
        _db.CreateTableAsync<Udhar>().Wait();
    }

    // Items
    public Task<List<Item>> GetItemsAsync() => _db.Table<Item>().ToListAsync();
    public Task<int> SaveItemAsync(Item item) => item.Id == 0 ? _db.InsertAsync(item) : _db.UpdateAsync(item);
    public Task<int> DeleteItemAsync(Item item) => _db.DeleteAsync(item);

    // Bills
    public Task<List<Bill>> GetBillsAsync() => _db.Table<Bill>().ToListAsync();
    public Task<int> SaveBillAsync(Bill bill) => bill.Id == 0 ? _db.InsertAsync(bill) : _db.UpdateAsync(bill);

    // Udhar
    public Task<List<Udhar>> GetUdharAsync() => _db.Table<Udhar>().ToListAsync();
    public Task<int> SaveUdharAsync(Udhar udhar) => udhar.Id == 0 ? _db.InsertAsync(udhar) : _db.UpdateAsync(udhar);
}
