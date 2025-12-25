using Microsoft.Maui.Storage;
using System.IO;
using vyaparsathi.Services;

namespace vyaparsathi.Views;

public partial class BackupRestorePage : ContentPage
{
    private string _selectedBackupFile;

    public BackupRestorePage()
    {
        InitializeComponent();
    }

    // Backup selected data
    private async void OnBackupClicked(object sender, EventArgs e)
    {
        var selectedTables = new List<string>();
        if (CategoryCheck.IsChecked) selectedTables.Add("Category");
        if (ItemCheck.IsChecked) selectedTables.Add("Item");
        if (UdharCheck.IsChecked) selectedTables.Add("Udhar");
        if (BillingCheck.IsChecked) selectedTables.Add("Bill");
        if (ProfileCheck.IsChecked) selectedTables.Add("BusinessProfile");

        if (!selectedTables.Any())
        {
            await DisplayAlert("Warning", "Please select at least one data type to backup.", "OK");
            return;
        }

        try
        {
            var backupFolder = FileSystem.AppDataDirectory;
            var backupFileName = $"VyaparSathi_Backup_{DateTime.Now:yyyyMMdd_HHmmss}.db3";
            var backupPath = Path.Combine(backupFolder, backupFileName);

            File.Copy(App.DatabasePath, backupPath, true);

            await DisplayAlert("Success", $"Backup saved:\n{backupPath}", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Backup failed: {ex.Message}", "OK");
        }
    }

    // Select backup file to restore
    private async void OnSelectBackupClicked(object sender, EventArgs e)
    {
        try
        {
            var fileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.WinUI, new[] { ".db3" } },
                { DevicePlatform.Android, new[] { "application/*" } },
                { DevicePlatform.iOS, new[] { "public.database" } }
            });

            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Select Backup File",
                FileTypes = fileTypes
            });

            if (result != null)
            {
                _selectedBackupFile = result.FullPath;
                await DisplayAlert("Selected", $"Backup file selected:\n{_selectedBackupFile}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"File selection failed: {ex.Message}", "OK");
        }
    }

    // Restore backup
    private async void OnRestoreClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_selectedBackupFile) || !File.Exists(_selectedBackupFile))
        {
            await DisplayAlert("Error", "Please select a valid backup file first.", "OK");
            return;
        }

        try
        {
            // Overwrite current database
            File.Copy(_selectedBackupFile, App.DatabasePath, true);

            // Re-initialize DatabaseService
            App.Database = new DatabaseService(App.DatabasePath);

            await DisplayAlert("Success", "Data restored successfully!", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Restore failed: {ex.Message}", "OK");
        }
    }
}
