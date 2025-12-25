using vyaparsathi.Services;

namespace vyaparsathi;

public partial class App : Application
{
    // Public setter allows re-initializing Database from BackupRestorePage
    public static DatabaseService Database { get; set; }
    public static string DatabasePath { get; private set; }

    public App()
    {
        InitializeComponent();

        DatabasePath = Path.Combine(FileSystem.AppDataDirectory, "vyaparsathi.db3");
        Database = new DatabaseService(DatabasePath);

        MainPage = new AppShell();
    }
}
