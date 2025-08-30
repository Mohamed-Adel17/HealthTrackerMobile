# Minoxidil Tracker Mobile App

A .NET MAUI mobile application for tracking Minoxidil applications with intelligent 24-hour reminder notifications.

## Features

### Core Functionality
- **📝 Record Applications**: Log each Minoxidil application with precise timestamp
- **📊 Statistics**: View total applications, today's count, last application time, and next reminder
- **📅 Today's Applications**: See all applications recorded today with edit/delete options
- **💾 Data Persistence**: All data is automatically saved locally and persists between app sessions

### 24-Hour Notification System
- **🔔 Smart Reminders**: Receive notifications exactly 24 hours after each application
- **📱 Background Notifications**: Notifications work even when the app is closed
- **⚡ Quick Action**: Tap "Record Now" directly from the notification to log your application
- **🔄 Automatic Rescheduling**: Reminders automatically adjust when you edit or delete records
- **🛡️ Backup Notifications**: Secondary reminder 5 minutes later in case the first is missed

### User Interface
- **🎨 Modern Design**: Clean, intuitive interface with clear visual hierarchy
- **📱 Responsive Layout**: Optimized for mobile devices with touch-friendly controls
- **🔧 Easy Management**: Edit or delete individual records with simple tap actions
- **🗑️ Bulk Operations**: Clear all data with confirmation dialog

## How It Works

### Recording Applications
1. Tap the "📝 Record Application Now" button
2. The app logs the current time and date
3. A 24-hour reminder is automatically scheduled
4. You'll see confirmation with the recorded time

### Notification System
- **24-Hour Reminder**: Exactly 24 hours after your last application, you'll receive a notification
- **Notification Actions**: 
  - Tap "Record Now" to immediately log your application
  - Tap "Dismiss" to ignore the reminder
- **Background Operation**: Notifications work even when the app is not running

### Managing Records
- **Edit**: Tap the "✏️" button next to any record to modify the time
- **Delete**: Tap the "❌" button to remove individual records
- **Clear All**: Use the "🗑️ Clear All" button to remove all data

## Technical Details

### Platform Support
- **Android**: API level 21+ (Android 5.0+)
- **iOS**: iOS 11.0+
- **Windows**: Windows 10.0.17763.0+
- **macOS**: macOS 13.1+

### Dependencies
- **.NET MAUI**: Cross-platform UI framework
- **Plugin.LocalNotification**: Local notification system
- **System.Text.Json**: Data serialization

### Data Storage
- **Location**: App data directory (platform-specific)
- **Format**: JSON file (`applications.json`)
- **Structure**: Array of application timestamps

## Installation

### Prerequisites
- Visual Studio 2022 with .NET MAUI workload
- .NET 9.0 SDK
- Platform-specific development tools (Android Studio, Xcode, etc.)

### Building and Running
1. Open the solution in Visual Studio 2022
2. Select your target platform (Android, iOS, Windows, macOS)
3. Build and run the application
4. Grant notification permissions when prompted

### Deployment
- **Android**: Generate APK or AAB for Google Play Store
- **iOS**: Archive and distribute via App Store
- **Windows**: Create MSIX package for Microsoft Store
- **macOS**: Create DMG or distribute via Mac App Store

## Usage Tips

### Best Practices
1. **Consistent Timing**: Try to apply Minoxidil at the same times each day
2. **Respond to Notifications**: Tap "Record Now" when notified for accurate tracking
3. **Regular Check-ins**: Review your statistics to monitor your progress
4. **Backup Data**: Consider backing up your data periodically

### Notification Settings
- Ensure the app has notification permissions
- Keep the app installed for background notifications
- Don't force-stop the app if you want reminders to work

## Troubleshooting

### Common Issues
- **Notifications not working**: Check notification permissions in device settings
- **Data not saving**: Ensure the app has storage permissions
- **App crashes**: Try clearing app data and reinstalling

### Support
For technical support or feature requests, please refer to the project documentation or contact the development team.

## Privacy and Data

- **Local Storage**: All data is stored locally on your device
- **No Cloud Sync**: No data is transmitted to external servers
- **User Control**: You can clear all data at any time
- **No Tracking**: The app does not collect personal information

---

**Note**: This app is designed to help track Minoxidil applications but should not replace medical advice. Always follow your doctor's instructions regarding Minoxidil use.
