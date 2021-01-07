# Delfinovin
Delfinovin - An XInput solution for Gamecube Controllers (Dell-fie-no-vin)
------------
Delfinovin is an XInput conversion for use with Gamecube Controller adapters that implement Nintendo's WUP-028 firmware.
This includes Mayflash controllers, Nintendo's own gamecube adapter and probably more that exist out there.

![](https://github.com/Struggleton/Delfinovin/blob/master/Delfinovin/resources/Delfinovin%20-%20Screenshot%20Debug.png)

## Usage
- Delfinovin uses ViGEmBus in order to create virtual XInput joysticks. Download the latest release of [ViGEmBus](https://github.com/ViGEm/ViGEmBus/releases "ViGEmBus") and install it.

- Delfinovin also uses the WinUSB driver to interface with the Gamecube Controller adapter. Download and run [Zadig](https://zadig.akeo.ie/ "Zadig"). Under *"Options, "* enable *"List all Devices"* and look for WUP-028 in the main dropdown menu.

	- (If this does not appear, ensure your adapter is in Wii U mode if you're using a Mayflash adapter and try plugging the black cable into another port.)

![](https://github.com/Struggleton/Delfinovin/blob/master/Delfinovin/resources/Zadig%20Icon.png)
- Select *"WinUSB"* in the right columm and click *"Replace Driver."* Accept any system driver prompts. 

- Check settings.txt and modify the settings to your desired values. [GamepadTester](https://gamepad-tester.com/ "Gamepad Tester") can be useful for visualizing the sticks / button inputs if you're trying to figure out what settings to use in programs and other games. 

If you would like to calibrate the sticks in order to use their full ranges, do so now by using the *Calibrate Controllers* menu. 
- Restart the program and choose "*Begin controller input loop"*!

## Settings
Delfinovin has a couple of settings that can be set currently:

- TriggerDeadzone [Default - 0.15]
	-  TriggerDeadzone refers to the amount that the trigger has to be pressed in order to register any input.
- TriggerThreshold [Default - 0.65]
	-  TriggerThreshold refers to the amount that the analog trigger has to be pressed before it registers as a full press.
- EnableRawPrint [Default - True]
	-  Enable/disable printing raw data that is being sent from the adapter.
- EnableAnalogPress [Default - False]
	- If enabled, any analog button press will act as a full digital press. Make sure your analog deadzone is set properly!
- PortsEnabled [Default - 1]
	- Enable/disable specific ports. Ports 1-4 supported. (*PortsEnabled: 1 2 3 4 will enable all ports.*)
- CalibrateCenter [Default - True]
	- If enabled, when first running the programs, the stick's center will be recalibrated. Make sure to leave the stick in a neutral position on startup.

## Notes
- Delfinovin can not take exclusive control over the controller adapter. This means you will have to close anything that does (Yuzu, Dolphin, etc.) first, run the program and then open them. 

- By default, Delfinovin will calibrate the stick centers. However, if you calibrate the sticks in the stick menu, the new calibration for the ranges will take priority. 

- Currently, there is a bug in the program ([Documented here](https://github.com/Struggleton/Delfinovin/issues/2)) that causes the controller loop to not properly begin. There's a workaround documented in this video here: https://www.youtube.com/watch?v=bi2hf6VxmiI, but please post on the linked issue if there are any underlying causes you can find.

- The Z-button is currently mapped to Back (the select button.) I ran out of buttons because of the analog triggers... Please remember to map your buttons in your games accordingly. I will look into migrating it to the Right Bumper.


## Planned features
- In-program deadzone support for sticks

## Credits
Credits and appreciation go to:
- (Rubendal)[https://github.com/rubendal] for writing the (BitStream)[https://github.com/rubendal/BitStream] library!
- The (LibUSB.NET)[https://github.com/LibUsbDotNet] team for writing the (LibUSB.NET)[https://github.com/LibUsbDotNet/LibUsbDotNet] library! / WinUSB team at Microsoft for providing the driver
- (Nefarius)[https://github.com/nefarius] for writing the ViGEmBus driver and the (ViGEmBus.NET)[https://github.com/ViGEm/ViGEm.NET] client library
- (Matt "ms4v" Cunningham)[https://bitbucket.org/elmassivo/] for a great reference with GCNUSBFeeder and the vJoy library
- (Gabriel "gemarcano" Marcano)[https://github.com/gemarcano] for providing the (essential documentation on how the adapter sends data)[https://github.com/gemarcano/GCN_Adapter-Driver/tree/master/docs]
- (Pete "pbatard" Batard)[https://github.com/pbatard] for (Zadig/libwdi)[https://github.com/pbatard/libwdi] which makes setting up WinUSB convenient
- (BarkingFrog)[https://twitter.com/Barking_Frogssb] for providing the math behind calibrating/centering sticks

## Libraries used
- LibUSB.NET
- Nefarius.ViGEm.Client
- BitStream

## Contributing
Thank you! I am still learning a lot about the USB architecture and Gamecube Controllers, so any help / commits are appreciated! I can be contacted on Discord at Struggleton#4071 / Twitter at @Struggleton as well.
