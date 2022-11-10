using Nefarius.Drivers.WinUSB;
using Nefarius.Utilities.DeviceManagement.Extensions;
using Nefarius.Utilities.DeviceManagement.PnP;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Delfinovin
{
    public class GamecubeAdapter
    {
        private const int VENDOR_ID = 0x057E;
        private const int PRODUCT_ID = 0x0337;

        private readonly DeviceNotificationListener _deviceNotificationListener;

        public ConnectionStatus AdapterStatus { get; private set; }

        // TODO - create events for adapter connections
        public event EventHandler<ConnectionStatus> AdapterConnectionStatusChanged;

        public GamecubeAdapter()
        {
            _deviceNotificationListener = new DeviceNotificationListener();
            _deviceNotificationListener.DeviceArrived += DeviceArrived;
            _deviceNotificationListener.DeviceRemoved += DeviceRemoved;

            _deviceNotificationListener.StartListen(DeviceInterfaceIds.UsbDevice);

            InitializeAdapter();
        }

        private void InitializeAdapter()
        {
            // The adapter is already initialized/plugged in,
            // Delfinovin only supports one adapter at a time plugged in
            // so return early before moving forward
            if (AdapterStatus != ConnectionStatus.Disconnected)
                return;

            // Check if the adapter is plugged in.
            bool adapterPlugged = TryFindDevice(out var usbDevice);
            AdapterStatus = adapterPlugged ? ConnectionStatus.Connected :
                                             ConnectionStatus.Disconnected;

            // Our adapter is not plugged in, return early
            if (!adapterPlugged)
                return;

            // Set the timeout for transfers.
            usbDevice.ControlPipeTimeout = 5000;

            // ----------------- [TO-DO] Setup Nyko-support here. -----------------

            // Prompt the adapter to start sending data.
            WriteAdapterCommand(usbDevice, new byte[] { 0x13 });

            // Adapter is initialized - set the adapterStatus as such
            // Invoke the connection event as we 
            AdapterStatus = ConnectionStatus.Initialized;
            AdapterConnectionStatusChanged?.Invoke(this, AdapterStatus);

            // Begin reading data from adapter.
            ReadAdapterData(usbDevice);
        }


        private bool TryFindDevice(out USBDevice usbDevice)
        {
            usbDevice = null;

            // Get all USB devices with the USBDevice interface id
            USBDeviceInfo[] details = USBDevice.GetDevices(DeviceInterfaceIds.UsbDevice);

            // Try to find and match USB device with the GCC Adapter
            // ProductID and VendorID. If we can't find it, return false 
            // and a null USB device object.
            USBDeviceInfo match = details?.FirstOrDefault(info => info.VID == VENDOR_ID
                                                               && info.PID == PRODUCT_ID);

            if (match == null)
                return false;

            usbDevice = new USBDevice(match);
            return true;
        }

        
        private void ReadAdapterData(USBDevice adapterDevice)
        {
            // Get the in pipe on the USB device to read data from.
            USBPipe inPipe = adapterDevice.Interfaces.FirstOrDefault()?.InPipe;

            // Initialize a byte array to read data into.
            byte[] data = new byte[0x25];

            // Set up a new task in the background to read
            // adapter data.
            Task.Run(async () =>
            {
                while (AdapterStatus == ConnectionStatus.Initialized)
                {
                    try
                    {
                        // Read controller data into the byte array
                        await inPipe.ReadAsync(data, 0, data.Length);
                        Debug.WriteLine(BitConverter.ToString(data));
                    }

                    catch (Exception ex) 
                    {
                        throw new Exception($"Failed to read data from adapter. Exception: {ex.Message}");
                    }
                }
            });
        }

        private void WriteAdapterCommand(USBDevice adapterDevice, byte[] data)
        {
            USBPipe outPipe = adapterDevice.Interfaces.FirstOrDefault()?.OutPipe;
            Task.Run(async () =>
            {
                try
                {
                    await outPipe.WriteAsync(data, 0, data.Length);
                }

                catch (Exception ex)
                {
                    throw new Exception($"Failed to write command to adapter. Exception: {ex.Message}");
                }
            });
        }

        private void DeviceArrived(DeviceEventArgs args)
        {
            InitializeAdapter();
        }

        private void DeviceRemoved(DeviceEventArgs args)
        {
            if (!TryFindDevice(out USBDevice _))
            {
                AdapterStatus = ConnectionStatus.Disconnected;
                AdapterConnectionStatusChanged?.Invoke(this, AdapterStatus);
            }
        }
    }
}
