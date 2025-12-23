using vyaparsathi.Services;
using System.IO;

namespace vyaparsathi;

public partial class App : Application
{
    public static DatabaseService Database { get; private set; }

    public App()
    {
        InitializeComponent();

        // Initialize database
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "vyaparsathi.db3");
        Database = new DatabaseService(dbPath);

        MainPage = new AppShell();
    }
}
