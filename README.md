# Delfinovin 
[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/C0C43E4IB)

Delfinovin - An XInput solution for Gamecube Controllers (Dell-fie-no-vin)
------------
Delfinovin is an XInput conversion for use with Gamecube Controller adapters that implement Nintendo's WUP-028 firmware.
This includes Mayflash controllers, Nintendo's own gamecube adapter and probably more that exist out there.

![](https://github.com/Struggleton/Delfinovin/blob/event-cleanup/DelfinovinActX2/resources/DelfinovinDebugScreenshot.png)

## Usage
- Delfinovin uses ViGEmBus in order to create virtual XInput joysticks. Download the latest release of [ViGEmBus](https://github.com/ViGEm/ViGEmBus/releases/tag/setup-v1.16.116 "ViGEmBus") and install it.

- Delfinovin also uses the WinUSB driver to interface with the Gamecube Controller adapter. Download and run [Zadig](https://zadig.akeo.ie/ "Zadig"). Under *"Options,"* enable *"List all Devices"* and look for WUP-028 in the main dropdown menu.

	- (If this does not appear, ensure your adapter is in Wii U mode if you're using a Mayflash adapter and try plugging the black cable into another port.)

![](https://github.com/Struggleton/Delfinovin/blob/event-cleanup/DelfinovinActX2/resources/Zadig%20Icon.png)
- Select *"WinUSB"* in the right columm and click *"Replace Driver."* Accept any system driver prompts. 

- Download the latest release of Delfinovin from the releases page and extract it to somewhere safe. 

- Check settings.txt and modify the settings to your desired values. [GamepadTester](https://gamepad-tester.com/ "Gamepad Tester") can be useful for visualizing the sticks / button inputs if you're trying to figure out what settings to use in programs and other games. 

- Restart the program and choose "*Begin controller input loop"*! Delfinovin will only translate gamecube controller inputs while it is open and is in the controller loop, meaning you have to keep it open as long as you want to use your controller.

If you would like to calibrate the sticks in order to use their full ranges, do so by hitting Enter on your keyboard, rotating your sticks on the controllers you'd like to calibrate and press Backspace once you're done. The controller status should update to say "Calibrated."

- Plug in a controller and it should pick it up. Have fun!

## Settings
Delfinovin has a couple of settings that can be set currently:

- TriggerDeadzone [Default - 0.15]
	-  TriggerDeadzone refers to the amount that the trigger has to be pressed in order to register any input.
- TriggerThreshold [Default - 0.65]
	-  TriggerThreshold refers to the amount that the analog trigger has to be pressed before it registers as a full press.
-StickDeadzone [Default - 0.00]
	-  Sets the range of the stick that is registered as a neutral position.
- EnableRawPrint [Default - True]
	-  Enable/disable printing raw data that is being sent from the adapter.
- EnableDigitalPress [Default - False]
	- If any press is past the TriggerDeadzone, it registers as a digital button, emulating digital triggers. Make sure the TriggerDeadzone setting is set properly!
- PortsEnabled [Default - 1]
	- Enable/disable specific ports. Ports 1-4 supported. (*PortsEnabled: 1 2 3 4 will enable all ports.*)
- CalibrateCenter [Default - True]
	- If enabled, when first running the programs, the stick's center will be recalibrated. Make sure to leave the stick in a neutral position on startup. Calibrating the sticks from the menu will override this option.
- EnableRumble [Default - False]
	-If enabled, gamecube controllers will receive haptic feedback from the game (Rumble.)
- SwapAB [Default - True]
	- If enabled, the A/B buttons are swapped to fit the Nintendo layout.
- SwapXY [Default - True]
	- If enabled, the X/Y buttons are swapped to fit the Nintendo layout.

## Notes
- Delfinovin can not take exclusive control over the controller adapter. This means you will have to close anything that does (Yuzu, Dolphin, etc.) first, run the program and then open them. 

- By default, Delfinovin will calibrate the stick centers. However, if you calibrate the sticks, the new calibration for the ranges will take priority. 


## Planned features
- Create new/default profiles for stick calibrations - Name and create new ones for quick access.
- Controller by controller settings
- Manual stick ranges 
- WPF UI?

## Credits
Credits and appreciation go to:
- The [LibUSB.NET](https://github.com/LibUsbDotNet) team for writing the [LibUSB.NET](https://github.com/LibUsbDotNet/LibUsbDotNet) library! / WinUSB team at Microsoft for providing the driver
- [Nefarius](https://github.com/nefarius) for writing the ViGEmBus driver and the [ViGEmBus.NET](https://github.com/ViGEm/ViGEm.NET) client library
- [Matt "ms4v" Cunningham](https://bitbucket.org/elmassivo/) for a great reference with GCNUSBFeeder and the vJoy library
- [Gabriel "gemarcano" Marcano](https://github.com/gemarcano) for providing the [essential documentation on how the adapter sends data](https://github.com/gemarcano/GCN_Adapter-Driver/tree/master/docs)
- [Pete "pbatard" Batard](https://github.com/pbatard) for [Zadig/libwdi](https://github.com/pbatard/libwdi) which makes setting up WinUSB convenient
- [BarkingFrog](https://twitter.com/Barking_Frogssb) for providing the math behind calibrating/centering sticks
- All of the testers in the Smash Ultimate Yuzu discord for helping iron out bugs!

## Libraries used
- LibUSB.NET
- Nefarius.ViGEm.Client

## Contributing and Support
Thank you! I am still learning a lot about the USB architecture and Gamecube Controllers, so any help / commits are appreciated! I can be contacted on Discord at Struggleton#4071 / Twitter at @Struggleton as well. If you are interested in what I've got going on here, please consider throwing [a couple coins my way!](https://ko-fi.com/C0C43E4IB)