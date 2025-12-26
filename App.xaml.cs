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

        // Load Shell
        MainPage = new AppShell();

        // Navigate after slight delay to ensure Shell is fully loaded
        Task.Run(async () =>
        {
            await Task.Delay(100); // small delay
            await MainPage.Dispatcher.DispatchAsync(async () =>
            {
                await Shell.Current.GoToAsync("//startup");
            });
        });
    }

}
