
# Delfinovin

Delfinovin - An XInput solution for Gamecube Controllers (Dell-fee-no-vin)
------------
![enter image description here](https://github.com/Struggleton/Delfinovin/blob/wpf-uidev/MainWindow_Screenshot.png?raw=true)

Delfinovin is an XInput conversion for use with Gamecube Controller adapters that implement Nintendo's WUP-028 firmware. This includes Mayflash controllers, Nintendo's own gamecube adapter and probably more that exist out there. 

If you would like to donate to the project, you can do so using the links here:

 - [Paypal](https://paypal.me/Struggleton)
 - [Ko-Fi](https://ko-fi.com/struggleton)

# Downloads & Usage
[Download the latest releases of Delfinovin here from the releases page.](https://github.com/Struggleton/Delfinovin/releases)

- Delfinovin uses ViGEmBus in order to create virtual XInput joysticks. Download the latest release of [ViGEmBus](https://github.com/ViGEm/ViGEmBus/releases/tag/setup-v1.16.116 "ViGEmBus") and install it.

- Delfinovin also uses WinUSB to communicate with the Gamecube Adapter directly.

	-   Download and run the [latest version of](https://zadig.akeo.ie/) [Zadig](https://zadig.akeo.ie/).
    
	-   Under "Options", enable "List All Devices” and select WUP-028 from the list of devices.
    
	-   Choose WinUSB in the right dropdown list and then click on "Replace Driver."
    
-   Accept any prompts and wait for it to install and then restart Delfinovin.
![](https://github.com/Struggleton/Delfinovin/blob/event-cleanup/DelfinovinActX2/resources/Zadig%20Icon.png)

Delfinovin should automatically detect any controllers / adapters being plugged in and create XInput joysticks dynamically. 

- If you would like to calibrate the controller to use the full range of the sticks, do so by pressing the "Edit" button next to the controller you'd like to calibrate. 
- Rotate the control sticks slowly so that they reach each edge of the controller.
- Once done, press the Edit button again and click on "Finish Calibration" to finish calibrating the controller and have it use the new range.

# Settings
- A number of settings can be set on a per-controller basis through the same Edit button menu. Navigate to the settings menu by clicking on the "Edit" button next to the controller and clicking on "Edit Settings."
	- For the controller settings:
		-  Enable Rumble - When enabled, controllers will receive haptic feedback (rumble.)
		
		-   Enable Digital Triggers - When enabled, the digital button of the triggers will be enabled. Button/trigger layouts will be swapped.
		
		-   SwapAB / SwapXY - Swaps the button layout of the face buttons.
		
		-   TriggerDeadzone - The percentage of the button that registers no input.
		
		-   TriggerThreshold - The amount that you have to press the button to register a full press.
		
		-   LeftStick/RightStick Deadzone - The percentage of the stick that registers no input.
	
		-  LeftStick/RightStick Range - The size of the virtual stick. Larger values mean a further stick move is required.
	- Click on "Apply" to apply the currently selected settings to the controller.
- These settings can be saved to profiles and loaded from using the "Controller Profiles" button on the same page.

- Settings for the application can be set under the "Settings" tab at the bottom of the application. These settings consist of:
	- Minimize Program to System Tray - When enabled, Delfinovin will minimize itself to the system tray. Right-click the icon to open a menu with options to open/close the program.

	- Check for Updates on Startup – When enabled, Delfinovin will check for new releases from the main Github page.

 	- Minimize Application on Startup – When set, the application will minimize when the application is opened.

 	- Run Application on PC Startup – When the setting is enabled, Delfinovin will be added to the computer’s application startup list

 	- Default Profile - Controller #1-4 - Load the selected setting profiles when starting Delfinovin and apply them to the corresponding controller port. 

- Delfinovin has also has a number of themes that can be selected from the application settings window. Click on Settings > Select Themes at the bottom of the dialog and choose Delfinovin’s theme and controller color.

# Notes
- Delfinovin can not take exclusive control over the controller adapter. This means you will have to close anything that does (Yuzu, Dolphin, etc.) first, run the program and then open them. 

# Planned features
- not sure, mainly bug fixes at this moment

# Credits
Credits and appreciation go to:
- The [LibUSB.NET](https://github.com/LibUsbDotNet) team for writing the [LibUSB.NET](https://github.com/LibUsbDotNet/LibUsbDotNet) library! / WinUSB team at Microsoft for providing the driver
- [Nefarius](https://github.com/nefarius) for writing the ViGEmBus driver and the [ViGEmBus.NET](https://github.com/ViGEm/ViGEm.NET) client library
- [Matt "ms4v" Cunningham](https://bitbucket.org/elmassivo/) for a great reference with GCNUSBFeeder and the vJoy library
- [Gabriel "gemarcano" Marcano](https://github.com/gemarcano) for providing the [essential documentation on how the adapter sends data](https://github.com/gemarcano/GCN_Adapter-Driver/tree/master/docs)
- [Pete "pbatard" Batard](https://github.com/pbatard) for [Zadig/libwdi](https://github.com/pbatard/libwdi) which makes setting up WinUSB convenient
- [BarkingFrog](https://twitter.com/Barking_Frogssb) for providing the math behind calibrating/centering sticks
- [Narr the Reg](https://github.com/german77) / The [Dolphin team](https://github.com/dolphin-emu) for advice on fixing Nyko adapters + gathering stick centers
- The [MaterialDesignInXAML](https://github.com/MaterialDesignInXAML) team behind Material Design in XAML toolkit for providing a great framework for UI design
- [OpenClipArt](http://www.openclipart.org/) for providing the base vector for the design behind the GamecubeDialog
- The [Octokit](https://github.com/octokit) team for providing a great library for interacting with Github
- All of the testers in the Smash Ultimate Yuzu discord for helping iron out bugs!

## Libraries used
- MaterialDesignInXamlToolkit
- Octokit.NET
- LibUSB.NET
- ViGEm.NET

## Contributing and Support
Thank you! I am still learning a lot about the USB architecture and Gamecube Controllers, so any help / commits are appreciated! I can be contacted on Discord at Struggleton#4071 / Twitter at @Struggleton as well. 