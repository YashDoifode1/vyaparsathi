namespace vyaparsathi;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // -----------------------------
        // Register routes
        // -----------------------------
        Routing.RegisterRoute("startup", typeof(Views.StartupPage));
        Routing.RegisterRoute("dashboard", typeof(Views.DashboardPage));
        Routing.RegisterRoute(nameof(Views.SettingsPage), typeof(Views.SettingsPage));
        Routing.RegisterRoute(nameof(Views.AddCategoryPage), typeof(Views.AddCategoryPage));
        Routing.RegisterRoute(nameof(Views.AddItemPage), typeof(Views.AddItemPage));
        Routing.RegisterRoute(nameof(Views.CustomersPage), typeof(Views.CustomersPage));
        Routing.RegisterRoute(nameof(Views.BillingHistoryPage), typeof(Views.BillingHistoryPage));
        Routing.RegisterRoute(nameof(Views.BackupRestorePage), typeof(Views.BackupRestorePage));

        // -----------------------------
        // Navigate to hidden StartupPage
        // -----------------------------
        // Use relative routing, not absolute "//"
        this.Dispatcher.Dispatch(async () =>
        {
            await this.GoToAsync("startup");
        });
    }
}
