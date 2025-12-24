using vyaparsathi.Models;

namespace vyaparsathi;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register routes for navigation
        Routing.RegisterRoute(nameof(Views.SettingsPage), typeof(Views.SettingsPage));
        Routing.RegisterRoute(nameof(Views.AddCategoryPage), typeof(Views.AddCategoryPage));
        Routing.RegisterRoute(nameof(Views.AddItemPage), typeof(Views.AddItemPage));

        // Load profile for Flyout Header
        LoadProfile();
    }

    private async void LoadProfile()
    {
        try
        {
            var profile = await App.Database.GetBusinessProfileAsync();
            if (profile != null)
            {
                FlyoutBusinessName.Text = profile.BusinessName;
                FlyoutOwnerName.Text = profile.OwnerName;
            }
        }
        catch (Exception ex)
        {
            // Optional: handle error, e.g., no profile yet
            Console.WriteLine("Error loading profile: " + ex.Message);
        }
    }

    // Optional: Call this method after profile is updated to refresh header
    public async void RefreshProfileHeader()
    {
        var profile = await App.Database.GetBusinessProfileAsync();
        if (profile != null)
        {
            FlyoutBusinessName.Text = profile.BusinessName;
            FlyoutOwnerName.Text = profile.OwnerName;
        }
    }
}
