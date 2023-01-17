using LibUsbDotNet;
using LibUsbDotNet.Main;

namespace Delfinovin
{
    /// <summary>
    /// A class providing USB read/write functionality using LibUSB/WinUSB
    /// </summary>
    public class Device
    {
        private int _vendorID;
        private int _productID;

        private UsbDevice UsbDeviceHandle;
        private UsbEndpointReader DeviceReader;
        private UsbEndpointWriter DeviceWriter;

        public Device(int vendorID, int productID)
        {
            _vendorID = vendorID;
            _productID = productID;  
        }

        /// <summary>
        /// Check if the USB device is connected.
        /// </summary>
        /// <param name="vendorID">The vendor ID of the USB device</param>
        /// <param name="productID">The product ID of the USB device</param>
        /// <returns>Boolean - if the device is connected, return true.</returns>
        public static bool IsConnected(int vendorID, int productID)
        {
            // Create a new USBDeviceFinder using the provided vendor/productIDs
            // and find if the usbdevice is within the list of usb devices.
            UsbRegistry usbRegistry = UsbDevice.AllDevices.Find(new UsbDeviceFinder(vendorID, productID));

            // If the usbRegistry is null,
            // it is not connected.
            return usbRegistry != null;
        }

        /// <summary>
        /// Try to capture the device handle and open the device.
        /// </summary>
        /// <returns>Boolean - return true if the device was opened sucessfully.</returns>
        public bool TryOpenDevice()
        {
            // Create a new USBDeviceFinder using the provided vendor/productIDs
            UsbDeviceFinder usbDeviceFinder = new UsbDeviceFinder(_vendorID, _productID);

            // Try to open the USB device
            UsbDeviceHandle = UsbDevice.OpenUsbDevice(usbDeviceFinder);

            // If this is null, return false.
            // The USBDevice is either not plugged in
            // or the WinUSB driver is not installed.
            if (UsbDeviceHandle == null)
                return false;

            // I forgot what this does honestly
            // Something about providing support for MonoLibUSB?
            IUsbDevice wholeUsbDevice = usbDeviceFinder as IUsbDevice;
            if (wholeUsbDevice != null)
            {
                // Set the configuration to one and 
                // and claim interface 0.
                wholeUsbDevice.SetConfiguration(1);
                wholeUsbDevice.ClaimInterface(0);
            }

            // Open the EndpointReader/Writer for use
            DeviceReader = UsbDeviceHandle.OpenEndpointReader(ReadEndpointID.Ep01);
            DeviceWriter = UsbDeviceHandle.OpenEndpointWriter(WriteEndpointID.Ep02);

            // Return true, the device has been opened properly
            return true;
        }

        /// <summary>
        /// Check if the USBDevice has been opened.
        /// </summary>
        /// <returns>Boolean - return if the USBDevice is open or not.</returns>
        public bool IsOpen()
        {
            // If the USBDevice is null, return false.
            if (UsbDeviceHandle == null)
                return false;

            return UsbDeviceHandle.IsOpen;
        }

        /// <summary>
        /// Write a byte array to the USBDevice endpoint.
        /// </summary>
        /// <param name="data">The byte array to write to the device.</param>
        /// <returns>ErrorCode - If the write fails or succeeds.</returns>
        public ErrorCode Write(byte[] data) => DeviceWriter.Write(data, 5000, out int _);


        /// <summary>
        /// Read a set amount of bytes from the device.
        /// </summary>
        /// <param name="data">The byte array to read data into.</param>
        /// <returns><ErrorCode - If the read fails or succeeds./returns>
        public ErrorCode Read(ref byte[] data) => DeviceReader.Read(data, 5000, out int _);

        /// <summary>
        /// Send a packet to the USB device
        /// </summary>
        /// <param name="packet">The packet to send over the USB device</param>
        /// <returns>Boolean - if the control transfer succeeds or fails.</returns>
        public bool ControlTransfer(UsbSetupPacket packet)
        {
            byte[] buffer = new byte[256];
            return UsbDeviceHandle.ControlTransfer(ref packet, buffer, buffer.Length, out int _);
        }

        /// <summary>
        /// Close the device handles and dispose 
        /// of any used resources.
        /// </summary>
        public void Close() 
        {
            // Dispose of the device reader/writers
            DeviceWriter?.Dispose();
            DeviceReader?.Dispose();

            if (UsbDeviceHandle != null)
            {
                if (UsbDeviceHandle.IsOpen)
                {
                    // If the USBDevice isn't null, release the interface
                    IUsbDevice wholeUsbDevice = UsbDeviceHandle as IUsbDevice;
                    if (!ReferenceEquals(wholeUsbDevice, null))
                    {
                        // Release interface #0.
                        wholeUsbDevice.ReleaseInterface(0);
                    }

                    // Close the USBDevice
                    UsbDeviceHandle.Close();
                }

                // Set our USBDevice to null
                UsbDeviceHandle = null;

                // Free usb resources
                UsbDevice.Exit();
            }
        }
    }
}
