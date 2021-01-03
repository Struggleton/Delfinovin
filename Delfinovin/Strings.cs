namespace Delfinovin
{
    public static class Strings
    {
        public static string PROGRAM_NAME = "Delfinovin - v0.01 [@Struggleton]";
        public static string MENU_DIVIDER = "=================================================";
        public static string MENU_OPTIONS = "1.) Begin controller input loop\n" +
            "2.) Credits / Thanks\n" +
            "3.) Setup Guide\n" +
            "4.) Exit";

        public static string MENU_CREDITS = "Credits and appreciation go to\n" +
            $"{MENU_DIVIDER}\n" +
            "Rubendal for writing the BitStream library!\n" +
            "The LibUSBDot.NET team for writing the LibUSBDotNET library! / WinUSB team at Microsoft for providing the driver\n" +
            "Nefarius for writing the ViGEmBus driver and the ViGEmBus.NET client library\n" +
            "ms4v for a great reference with GCNUSBFeeder and the vJoy library\n" +
            "Gemarcano for providing the essential documentation on how the adapter sends data\n" +
            "Pete Batard for Zadig/libwdi which makes setting up WinUSB convenient\n" +
            $"{MENU_DIVIDER}\n";

        public static string MENU_SETUP = "Delfinovin uses the ViGEmBus driver to setup up virtual controller inputs.\n" +
            "Download and install the latest release of ViGEmBus here: https://github.com/ViGEm/ViGEmBus/releases\n\n" +
            "Delfinovin also uses the WinUSB driver to interface with the Gamecube adapter as well.\n" +
            "Download and run Zadig (https://zadig.akeo.ie/)\n\n" +
            "Under Options, select \"List All Devices \" and look for WUP-028 in the main dropdown menu. " +
            "(If this does not appear, ensure your adapter is in Wii U mode and try plugging the black cable into another port.)\n" +
            "Select WinUSB in the right column and click on \"Replace Driver\" and accept any system driver prompts.\n\n" +
            "Restart this program and select the 1st option in the main menu!";

        public static string MENU_BEGINNING = "Beginning controller input loop.";
        public static string MENU_COMPLETE = "All done! Ending loop...";

        public static string INFO_FACEBUTTONS = "A Button: [{0}] | B Button [{1}] | X Button [{2}] | Y Button [{3}] | Start Button [{4}]";
        public static string INFO_DPAD = "Left DPAD: [{0}] | Right DPAD: [{1}] | Up DPAD: [{2}] | Down DPAD: [{3}]";
        public static string INFO_TRIGGERS = "Left Analog: [{0}] | Right Analog [{1}] | Z Button: [{2}]";
        public static string INFO_STICK = "Left Stick X: [{0}] | Left Stick Y: [{1}]";
        public static string INFO_CSTICK = "Right Stick X: [{0}] | Right Stick Y: [{1}]";

        public static string ERROR_SELECTIONINVALID = "Invalid input! Choose a number between 1-5.";
        public static string ERROR_ADAPTERNOTFOUND = "Gamecube Adapter not found. Have you installed WinUSB? " +
            "If so, make sure any other program that takes control over GC adapter (Dolphin, Yuzu.) is closed and reopen the program.";
        public static string ERROR_SETTINGSNOTFOUND = "Settings.txt not found! Generating one with default settings.";
        public static string ERROR_NOBYTES = "{0}: No more bytes!";
        public static string ERROR_PORTS = "Failed to read port number! Make sure that the setting is formatted properly (PortsEnabled: 1 2 3 4)";

        public static string EXCEPTION_IDENTIFIER = "Error! Gamecube magic header not found!";
        public static string EXCEPTION_EMBUSNOTFOUND = "The ViGEmBus driver was not found! Have you installed it?";
    }
}
