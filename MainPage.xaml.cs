using System.Collections.ObjectModel;
using System.Text.Json;
using Plugin.LocalNotification;

namespace MinoxidilTrackerMobile;

public partial class MainPage : ContentPage
{
    private ObservableCollection<ApplicationRecord> _applications;
    private readonly string _dataFilePath;
    private IDispatcherTimer _reminderTimer;
    private const int REMINDER_HOURS = 24; // 24-hour reminder as requested

    public ObservableCollection<ApplicationRecord> Applications
    {
        get => _applications;
        set
        {
            _applications = value;
            OnPropertyChanged();
        }
    }

    public MainPage()
    {
        InitializeComponent();
        _dataFilePath = Path.Combine(FileSystem.AppDataDirectory, "applications.json");
        _applications = new ObservableCollection<ApplicationRecord>();
        
        BindingContext = this;
        
        LoadApplications();
        SetupReminderTimer();
        UpdateDisplay();
        
        // Request notification permissions on startup
        RequestNotificationPermissions();
    }

    private async void RequestNotificationPermissions()
    {
        try
        {
            var request = new NotificationRequest
            {
                NotificationId = 999, // Temporary ID for permission request
                Title = "Minoxidil Tracker",
                Description = "Please allow notifications to receive reminders",
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now.AddSeconds(5) // Show immediately for permission
                }
            };

            await LocalNotificationCenter.Current.Show(request);
        }
        catch (Exception ex)
        {
            // Permission request failed, but app can still function
            System.Diagnostics.Debug.WriteLine($"Notification permission request failed: {ex.Message}");
        }
    }

    private void SetupReminderTimer()
    {
        _reminderTimer = Application.Current.Dispatcher.CreateTimer();
        _reminderTimer.Interval = TimeSpan.FromMinutes(5); // Check every 5 minutes
        _reminderTimer.Tick += ReminderTimer_Tick;
        _reminderTimer.Start();
    }

    private async void ReminderTimer_Tick(object sender, EventArgs e)
    {
        if (_applications.Count == 0) return;

        var lastApp = _applications.OrderByDescending(a => a.DateTime).First();
        var nextReminder = lastApp.DateTime.AddHours(REMINDER_HOURS);
        
        // Check if it's time for a reminder (within 5 minutes of the scheduled time)
        if (DateTime.Now >= nextReminder && DateTime.Now <= nextReminder.AddMinutes(5))
        {
            await ShowReminderNotification();
        }
    }

    private async Task ShowReminderNotification()
    {
        var lastApp = _applications.OrderByDescending(a => a.DateTime).First();
        var timeSinceLastApp = DateTime.Now - lastApp.DateTime;
        
        string message = $"It's been {timeSinceLastApp.Hours} hours since your last application. " +
                        "Time for your next minoxidil application!";

        var result = await DisplayAlert("⏰ Minoxidil Reminder", message, "Record Now", "Dismiss");
        
        if (result)
        {
            // User clicked "Record Now" - record the application
            await RecordApplicationNow();
        }
    }

    private async Task RecordApplicationNow()
    {
        var now = DateTime.Now;
        var newRecord = new ApplicationRecord { DateTime = now };
        
        _applications.Add(newRecord);
        SaveApplications();
        UpdateDisplay();
        
        // Schedule next 24-hour reminder
        ScheduleNextReminder(now);
        
        string time12Hour = now.ToString("hh:mm:ss tt");
        string date = now.ToString("MMM dd, yyyy");
        
        await DisplayAlert("✅ Recorded!", 
            $"Minoxidil application recorded!\n\nTime: {time12Hour}\nDate: {date}\n\nNext reminder in 24 hours!", 
            "OK");
    }

    private async void OnRecordClicked(object sender, EventArgs e)
    {
        await RecordApplicationNow();
    }

    private void ScheduleNextReminder(DateTime applicationTime)
    {
        var nextReminder = applicationTime.AddHours(REMINDER_HOURS);
        
        // Cancel any existing reminders first
        LocalNotificationCenter.Current.CancelAll();
        
        // Schedule local notification for the next reminder
        var notification = new NotificationRequest
        {
            NotificationId = 100,
            Title = "⏰ Minoxidil Reminder",
            Description = "It's been 24 hours since your last application. Time for your next minoxidil application!",
            ReturningData = "reminder",
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = nextReminder,
                RepeatType = NotificationRepeat.No
            }
        };

        LocalNotificationCenter.Current.Show(notification);
        
        // Also schedule a backup reminder 5 minutes later in case the first one is missed
        var backupNotification = new NotificationRequest
        {
            NotificationId = 101,
            Title = "⏰ Minoxidil Reminder",
            Description = "Reminder: Time for your minoxidil application!",
            ReturningData = "reminder_backup",
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = nextReminder.AddMinutes(5),
                RepeatType = NotificationRepeat.No
            }
        };

        LocalNotificationCenter.Current.Show(backupNotification);
    }

    private async void OnEditClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is ApplicationRecord record)
        {
            var result = await DisplayPromptAsync(
                "Edit Application Time",
                "Enter new date and time (MM/dd/yyyy HH:mm):",
                initialValue: record.DateTime.ToString("MM/dd/yyyy HH:mm"),
                keyboard: Keyboard.Text);

            if (!string.IsNullOrEmpty(result) && DateTime.TryParse(result, out DateTime newDateTime))
            {
                record.DateTime = newDateTime;
                SaveApplications();
                UpdateDisplay();
                
                // Reschedule reminder based on the most recent application
                var lastApp = _applications.OrderByDescending(a => a.DateTime).First();
                ScheduleNextReminder(lastApp.DateTime);
                
                string newTime12Hour = newDateTime.ToString("hh:mm:ss tt");
                string newDate = newDateTime.ToString("MMM dd, yyyy");
                
                await DisplayAlert("✅ Updated!", 
                    $"Record updated successfully!\n\nNew Time: {newTime12Hour}\nNew Date: {newDate}", 
                    "OK");
            }
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is ApplicationRecord record)
        {
            var result = await DisplayAlert(
                "Confirm Deletion",
                $"Are you sure you want to delete this record?\n\n{record.DisplayText}",
                "Delete",
                "Cancel");

            if (result)
            {
                _applications.Remove(record);
                SaveApplications();
                UpdateDisplay();
                
                // Reschedule reminder based on the most recent application (if any)
                if (_applications.Count > 0)
                {
                    var lastApp = _applications.OrderByDescending(a => a.DateTime).First();
                    ScheduleNextReminder(lastApp.DateTime);
                }
                else
                {
                    // No applications left, cancel all reminders
                    LocalNotificationCenter.Current.CancelAll();
                }
                
                await DisplayAlert("✅ Deleted!", "Record deleted successfully!", "OK");
            }
        }
    }

    private async void OnClearClicked(object sender, EventArgs e)
    {
        var result = await DisplayAlert(
            "Confirm Clear All",
            "Are you sure you want to clear all application data?\n\nThis action cannot be undone.",
            "Clear All",
            "Cancel");

        if (result)
        {
            _applications.Clear();
            SaveApplications();
            UpdateDisplay();
            
            // Cancel all reminders since there are no applications
            LocalNotificationCenter.Current.CancelAll();
            
            await DisplayAlert("✅ Cleared!", "All data has been cleared successfully!", "OK");
        }
    }

    private async void OnSettingsClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Settings", "Settings page coming soon!", "OK");
    }

    private void UpdateDisplay()
    {
        var todayApplications = _applications.Where(a => a.DateTime.Date == DateTime.Today).ToList();
        
        TotalLabel.Text = $"Total Applications: {_applications.Count}";
        TodayLabel.Text = $"Today: {todayApplications.Count}";
        
        if (_applications.Count > 0)
        {
            var lastApp = _applications.OrderByDescending(a => a.DateTime).First();
            string lastTime12Hour = lastApp.DateTime.ToString("hh:mm:ss tt");
            string lastDate = lastApp.DateTime.ToString("MMM dd, yyyy");
            LastAppLabel.Text = $"Last Application: {lastTime12Hour} ({lastDate})";

            var nextReminder = lastApp.DateTime.AddHours(REMINDER_HOURS);
            if (nextReminder > DateTime.Now)
            {
                string nextTime12Hour = nextReminder.ToString("hh:mm:ss tt");
                string nextDate = nextReminder.ToString("MMM dd, yyyy");
                NextReminderLabel.Text = $"Next Reminder: {nextTime12Hour} ({nextDate})";
            }
            else
            {
                NextReminderLabel.Text = "Next Reminder: Overdue";
            }
        }
        else
        {
            LastAppLabel.Text = "Last Application: Never";
            NextReminderLabel.Text = "Next Reminder: N/A";
        }
    }

    private void LoadApplications()
    {
        try
        {
            if (File.Exists(_dataFilePath))
            {
                string jsonData = File.ReadAllText(_dataFilePath);
                if (!string.IsNullOrEmpty(jsonData))
                {
                    var loadedApplications = JsonSerializer.Deserialize<List<ApplicationRecord>>(jsonData);
                    if (loadedApplications != null)
                    {
                        _applications.Clear();
                        foreach (var app in loadedApplications)
                        {
                            _applications.Add(app);
                        }
                        
                        // Reschedule reminder based on the most recent application
                        if (_applications.Count > 0)
                        {
                            var lastApp = _applications.OrderByDescending(a => a.DateTime).First();
                            ScheduleNextReminder(lastApp.DateTime);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Warning", $"Could not load previous data: {ex.Message}", "OK");
        }
    }

    private void SaveApplications()
    {
        try
        {
            string jsonData = JsonSerializer.Serialize(_applications.ToList(), new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_dataFilePath, jsonData);
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"Could not save data: {ex.Message}", "OK");
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        UpdateDisplay();
    }
}

public class ApplicationRecord
{
    public DateTime DateTime { get; set; }
    public string DisplayText => $"{DateTime.ToString("hh:mm:ss tt")} - {DateTime.ToString("MMM dd, yyyy")}";
}
