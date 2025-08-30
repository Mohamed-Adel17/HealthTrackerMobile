using Plugin.LocalNotification;

namespace MinoxidilTrackerMobile;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		
		// Handle notification taps - commented out for now to get basic app working
		// LocalNotificationCenter.Current.NotificationActionTapped += OnNotificationTapped;
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}

	// Commented out notification handling for now
	/*
	private async void OnNotificationTapped(NotificationEventArgs e)
	{
		// Handle notification tap
		if (e.Request.ReturningData == "reminder" || e.Request.ReturningData == "reminder_backup")
		{
			// Navigate to main page and show reminder dialog
			await MainThread.InvokeOnMainThreadAsync(async () =>
			{
				if (Current?.MainPage is AppShell shell && shell.CurrentPage is MainPage mainPage)
				{
					// The reminder will be handled by the timer in MainPage
					await mainPage.DisplayAlert("⏰ Minoxidil Reminder", 
						"It's been 24 hours since your last application. Time for your next minoxidil application!", 
						"Record Now", "Dismiss");
				}
			});
		}
	}
	*/
}