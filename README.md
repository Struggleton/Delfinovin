# Delfinovin
Delfinovin - An XInput solution for Gamecube Controllers
------------
Delfinovin is an XInput conversion for use with Gamecube Controller adapters that implement Nintendo's WUP-028 firmware.
This includes Mayflash controllers, Nintendo's own gamecube adapter and probably more that exist out there.

![](https://github.com/Struggleton/Delfinovin/blob/master/Delfinovin/resources/Delfinovin%20-%20Screenshot%20Debug.png)

## Usage
- Delfinovin uses ViGEmBus in order to create virtual XInput joysticks. Download the latest release of [ViGEmBus](https://github.com/ViGEm/ViGEmBus/releases "ViGEmBus") and install it.

- Delfinovin also uses the WinUSB driver to interface with the Gamecube Controller adapter. Download and run [Zadig](https://zadig.akeo.ie/ "Zadig"). Under *"Options, "* enable *"List all Devices"* and look for WUP-028 in the main dropdown menu.

	- (If this does not appear, ensure your adapter is in Wii U mode if you're using a Mayflash adapter and try plugging the black cable into another port.)

![](https://github.com/Struggleton/Delfinovin/blob/master/Delfinovin/resources/Zadig%20Icon.png)
- Select *"WinUSB"* in the right columm and click *"Replace Driver." Accept any system driver prompts. Restart the program and choose "*Begin controller input loop"*!

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

## Notes
Delfinovin can not take exclusive control over the Gamecube device yet. This means you will have to close anything that does (Yuzu, Dolphin, etc.) first, run the program and then open them. 

## Planned features
- In-program deadzone/range support for sticks

## Credits
Credits and appreciation go to:
- Rubendal for writing the BitStream library!
- The LibUSBDot.NET team for writing the LibUSBDotNET library! / WinUSB team at Microsoft for providing the driver
- Nefarius for writing the ViGEmBus driver and the ViGEmBus.NET client library
- ms4v for a great reference with GCNUSBFeeder and the vJoy library
- Gemarcano for providing the essential documentation on how the adapter sends data
- Pete Batard for Zadig/libwdi which makes setting up WinUSB convenient

## Libraries used
- LibUsbDotNet
- Nefarius.ViGEm.Client
- BitStream

## Contributing
Thank you! I am still learning a lot about the USB architecture and Gamecube Controllers, so any help / commits are appreciated! I can be contacted on Discord at Struggleton#4071 / Twitter at @Struggleton as well.
