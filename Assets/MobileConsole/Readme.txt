Mobile Console

Mobile Console is a clean and robust console logger optimised for mobile devices.

Features
- Clean and simple interface
- See all output when testing on device
- Customisable skin
- Simple installation


1. Installation

Drag the "Mobile Console" prefab into your first scene. Open the console by
tapping in the top left corner three times while the application is running.


2. Configuration (Optional)

Select the "Mobile Console" game object and change the values in the inspector:

- Skin: Change how you want the console to look
- Show On Error: Check if you want the console to automatically open when an error occurs
- Tap Corner: Select which corner you tap three times to open the console
- Disable In Release Build: Mobile Console will only be enabled in development builds
- Max Log Entries: Drag the slider to specify the maximum number of log entries


3. Troubleshooting

Mobile Console uses Text Mesh Pro. If you have not yet used it in your project you
will get a popup asking you to import it. Simply click "Import TMP Essentials" while
not in play mode.

For any issues or suggestions please create an issue here:
https://github.com/ghroot/MobileConsole-Issues/issues

Or send an email to:
marcus.lagerstrom@gmail.com


4. Version History

1.1.1b
- Fixed issue with log entry backgrounds

1.1.0b
- Added configuration: Disable In Release Build
- Now uses Text Mesh Pro for all text
- Identical log entries can now be collapsed together into one
- Will now adapt to devices notches
- Optimizations

1.0.2b
- Bug fixes

1.0.0b
- First release (beta)
