using System.Text.Json;
using SystemHealthDashboard.Core.Models;

namespace SystemHealthDashboard.Core.Services;

public class SettingsService
{
    private readonly string _settingsFilePath;
    private ApplicationSettings _currentSettings;

    public SettingsService()
    {
        var userDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var appDirectory = Path.Combine(userDirectory, "SystemHealthDashboard");
        
        if (!Directory.Exists(appDirectory))
        {
            Directory.CreateDirectory(appDirectory);
        }
        
        _settingsFilePath = Path.Combine(appDirectory, "settings.json");
        _currentSettings = new ApplicationSettings();
    }

    public ApplicationSettings LoadSettings()
    {
        try
        {
            if (File.Exists(_settingsFilePath))
            {
                var json = File.ReadAllText(_settingsFilePath);
                var settings = JsonSerializer.Deserialize<ApplicationSettings>(json);
                
                if (settings != null)
                {
                    _currentSettings = settings;
                    return settings;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading settings: {ex.Message}");
        }
        
        return _currentSettings;
    }

    public void SaveSettings(ApplicationSettings settings)
    {
        try
        {
            _currentSettings = settings;
            
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            
            var json = JsonSerializer.Serialize(settings, options);
            File.WriteAllText(_settingsFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving settings: {ex.Message}");
        }
    }

    public ApplicationSettings GetCurrentSettings()
    {
        return _currentSettings;
    }

    public void UpdateThresholds(ThresholdSettings thresholds)
    {
        _currentSettings.Thresholds = thresholds;
        SaveSettings(_currentSettings);
    }

    public void UpdateTheme(ThemeSettings theme)
    {
        _currentSettings.Theme = theme;
        SaveSettings(_currentSettings);
    }

    public void UpdateStartup(StartupSettings startup)
    {
        _currentSettings.Startup = startup;
        SaveSettings(_currentSettings);
    }

    public void UpdateRefreshInterval(int intervalMs)
    {
        _currentSettings.RefreshIntervalMs = intervalMs;
        SaveSettings(_currentSettings);
    }
}
