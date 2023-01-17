using Delfinovin.Controllers;
using Delfinovin.Controls;
using Delfinovin.Controls.Windows;
using LibUsbDotNet.Main;
using Nefarius.Utilities.DeviceManagement.PnP;
using Nefarius.ViGEm.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Forms = System.Windows.Forms;

namespace Delfinovin
{
    /// <summary>
    /// A class representing a Gamecube Adapter, providing various
    /// functionality for reading/writing to it, creating events for
    /// various states (Controller connection changes, etc.)
    /// </summary>
    public class GamecubeAdapter
    {
        private const int VENDOR_ID = 0x057E;
        private const int PRODUCT_ID = 0x0337;

        public event EventHandler<ControllerConnectionStatusEventArgs>? ControllerConnectionStatusChanged;
        public event EventHandler<CalibrationChangedEventArgs>? CalibrationStatusChanged;
        public event EventHandler<ConnectionStatus> AdapterConnectionStatusChanged;
        public event EventHandler<ControllerStatus[]> InputFrameProcessed;
        public event EventHandler<byte[]> RumbleStatusChanged;

        public event EventHandler<int> BeginPlaybackEventRequested;
        public event EventHandler<int> StopPlaybackEventRequested;
        public event EventHandler<int> StopRecordingInputEventRequested;
        public event EventHandler<int> StartRecordingInputEventRequested;

        private byte[] _rumbleCommand;

        private readonly DeviceNotificationListener _deviceNotificationListener;
        private HotkeyListener _hotkeyListener;
        private CancellationTokenSource _cancellationToken;
        private ViGEmClient _ViGEmClient;
        private Device _adapterDevice;

        private ControllerStatus[] _controllerStates = new ControllerStatus[4];

        private ConnectionStatus _adapterStatus;
        public ConnectionStatus AdapterStatus
        {
            get { return _adapterStatus; }
            private set
            {
                _adapterStatus = value;
                AdapterConnectionStatusChanged?.Invoke(this, AdapterStatus);
            }
        }

        public GamecubeController[] Controllers = new GamecubeController[4];

        public GamecubeAdapter()
        {
            // Create a new device listener
            _deviceNotificationListener = new DeviceNotificationListener();

            // Subscribe to any USB connection events
            _deviceNotificationListener.DeviceArrived += DeviceArrived;
            _deviceNotificationListener.DeviceRemoved += DeviceRemoved;

            // Begin listening for the events.
            _deviceNotificationListener.StartListen(DeviceInterfaceIds.UsbDevice);

            // Create a new rumble command
            // byte array. The command for rumble is 0x11
            _rumbleCommand = new byte[5];
            _rumbleCommand[0] = 0x11;
        }

        private async Task<bool> CheckViGEmStatus()
        {
            // Check to see if ViGEmBus is installed.
            bool vigemInstalled = DependencyInstaller.CheckViGEmInstallation();
            if (!vigemInstalled)
            {
                Application.Current.Dispatcher.Invoke(async () =>
                {
                    // Prompt the user to see if they want to install it.
                    MessageDialog installDialog = new MessageDialog(Strings.PromptInstallViGEm);
                    installDialog.ShowDialog();

                    bool install = installDialog.Result == Forms.DialogResult.Yes;
                    if (install)
                    {
                        // If yes, download and install ViGEm for the user
                        await DependencyInstaller.DownloadAndInstallViGEm();
                    }
                });
            }

            // Return if ViGEm was installed or not.
            return vigemInstalled;
        }

        private void InitializeDevice()
        {
            // Our adapter has not been plugged in yet, get it set up.
            if (Device.IsConnected(VENDOR_ID, PRODUCT_ID) && _adapterDevice == null)
            {
                _adapterDevice = new Device(VENDOR_ID, PRODUCT_ID);

                // Get whether or not our device is being used by another process.
                bool result = _adapterDevice.TryOpenDevice();

                // Update the adapter status.
                AdapterStatus = result ? ConnectionStatus.Connected :
                                         ConnectionStatus.Disconnected;
            }
        }

        private async Task InitializeAdapter()
        {
            if (AdapterStatus == ConnectionStatus.Connected)
            {
                // Send the setup packet for Nyko/3rd Party adapters
                _adapterDevice.ControlTransfer(new UsbSetupPacket(0x21, 11, 0x0001, 0, 0));

                // Prompt the adapter to begin writing data.
                _adapterDevice.Write(new byte[] { 0x13 });

                // Check if ViGEm is installed. If not, allow the user to
                // continue using the program without initializing any
                // buttons/functionality
                bool isInstalled = await CheckViGEmStatus();
                if (!isInstalled)
                    return;

                // Create our ViGEmClient
                _ViGEmClient = new ViGEmClient();

                // Create the controller array and initialize a
                // controller in each one.
                Controllers = new GamecubeController[4];
                for (int i = 0; i < 4; i++)
                {
                    Controllers[i] = new GamecubeController(_ViGEmClient, i);
                }

                // Begin a new hotkey listener and subscribe
                // to the calibration event
                _hotkeyListener = new HotkeyListener();
                _hotkeyListener.CalibrationEventRequested += hotkeyListener_CalibrationEventRequested;
                _hotkeyListener.StartRecordingInputEventRequested += _hotkeyListener_StartRecordingInputEventRequested;
                _hotkeyListener.StopRecordingInputEventRequested += _hotkeyListener_StopRecordingInputEventRequested;
                _hotkeyListener.BeginPlaybackEventRequested += _hotkeyListener_BeginPlaybackEventRequested;
                _hotkeyListener.StopPlaybackEventRequested += _hotkeyListener_StopPlaybackEventRequested;

                // Begin listening to the ControllerConnection and RumbleStatus Changed events
                ControllerConnectionStatusChanged += ControllerConnectionChanged;
                RumbleStatusChanged += RumbleStatusUpdated;

                // Our adapter is initialized, update the status.
                AdapterStatus = ConnectionStatus.Initialized;
            }
        }

        private async Task BeginPolling()
        {
            // If our adapter is initialized, begin reading data from the adapter.
            if (AdapterStatus == ConnectionStatus.Initialized)
            {
                // Begin reading adapter data on a different thread.
                // TO-DO - This would probably be better off on its own
                // Thread object instead of Task.Run.
                Task.Run(async () =>
                {
                    Poll(_cancellationToken.Token);
                }, _cancellationToken.Token);

                // Set our adapter as running.
                AdapterStatus = ConnectionStatus.Running;
            }
        }

        private async Task Poll(CancellationToken token)
        {
            // Create a byte array to read data into.
            byte[] data = new byte[0x25];
            while (_adapterDevice.Read(ref data) == ErrorCode.None)
            {
                // If the task is requested to end, break out of the loop
                if (token.IsCancellationRequested)
                    break;

                UpdateInputs(data);
            }
        }

        public void UpdateInputs(byte[] controllerData)
        {
            // The first byte is a magic byte. If
            // this is not found, the data is invalid.
            if (controllerData[0] != 0x21)
                throw new Exception(Strings.ErrorMagicNotFound);

            for (int port = 0; port < 4; port++)
            {
                // Get a byte and work on each bit. Fill out each inputState
                // using the data from each 36 bytes.
                byte workingByte = controllerData[port * 9 + 1];

                _controllerStates[port].IsPowered = Extensions.GetBit(workingByte, 2);

                // OR / AND NOT these values together so that it can be either 
                // of these controller types.
                _controllerStates[port].ControllerType = Extensions.GetBit(workingByte, 4) ? _controllerStates[port].ControllerType | ControllerType.Standard :
                                                                                             _controllerStates[port].ControllerType & ~ControllerType.Standard;

                _controllerStates[port].ControllerType = Extensions.GetBit(workingByte, 5) ? _controllerStates[port].ControllerType | ControllerType.Wavebird :
                                                                                             _controllerStates[port].ControllerType & ~ControllerType.Wavebird;
                workingByte = controllerData[port * 9 + 2];
                _controllerStates[port].SetButtonFlag(GamecubeControllerButtons.A, Extensions.GetBit(workingByte, 0));
                _controllerStates[port].SetButtonFlag(GamecubeControllerButtons.B, Extensions.GetBit(workingByte, 1));
                _controllerStates[port].SetButtonFlag(GamecubeControllerButtons.X, Extensions.GetBit(workingByte, 2));
                _controllerStates[port].SetButtonFlag(GamecubeControllerButtons.Y, Extensions.GetBit(workingByte, 3));
                _controllerStates[port].SetButtonFlag(GamecubeControllerButtons.DpadLeft, Extensions.GetBit(workingByte, 4));
                _controllerStates[port].SetButtonFlag(GamecubeControllerButtons.DpadRight, Extensions.GetBit(workingByte, 5));
                _controllerStates[port].SetButtonFlag(GamecubeControllerButtons.DpadDown, Extensions.GetBit(workingByte, 6));
                _controllerStates[port].SetButtonFlag(GamecubeControllerButtons.DpadUp, Extensions.GetBit(workingByte, 7));

                workingByte = controllerData[port * 9 + 3];
                _controllerStates[port].SetButtonFlag(GamecubeControllerButtons.Start, Extensions.GetBit(workingByte, 0));
                _controllerStates[port].SetButtonFlag(GamecubeControllerButtons.Z, Extensions.GetBit(workingByte, 1));
                _controllerStates[port].SetButtonFlag(GamecubeControllerButtons.R, Extensions.GetBit(workingByte, 2));
                _controllerStates[port].SetButtonFlag(GamecubeControllerButtons.L, Extensions.GetBit(workingByte, 3));

                // Update the trigger buttons based on our current trigger deadzone
                _controllerStates[port].UpdateTriggerButtons(ProfileManager.CurrentProfiles[port].TriggerDeadzone);

                _controllerStates[port].LStick.X = controllerData[port * 9 + 4];
                _controllerStates[port].LStick.Y = controllerData[port * 9 + 5];
                _controllerStates[port].RStick.X = controllerData[port * 9 + 6];
                _controllerStates[port].RStick.Y = controllerData[port * 9 + 7];
                _controllerStates[port].Triggers.X = controllerData[port * 9 + 8];
                _controllerStates[port].Triggers.Y = controllerData[port * 9 + 9];

                // Add in the virtual clause so we don't disconnect a virtual controller
                // That gets done manually 
                if (Controllers[port].ConnectionStatus != _controllerStates[port].IsPlugged &&
                    Controllers[port].ControllerType != ControllerType.Virtual)
                {
                    ControllerConnectionStatusChanged.Invoke(this, new ControllerConnectionStatusEventArgs()
                    {
                        ConnectionStatus = _controllerStates[port].IsPlugged,
                        TimeStamp = DateTime.UtcNow,
                        ControllerPort = port,
                        ControllerType = _controllerStates[port].ControllerType
                    });
                }

                // Check if the current rumbleCommand is the same
                // If it isn't, fire the event for writing the command and update
                // the array.
                byte controllerVibrating = Convert.ToByte(Controllers[port].IsVibrating);
                if (_rumbleCommand[port + 1] != controllerVibrating)
                {
                    _rumbleCommand[port + 1] = controllerVibrating;
                    RumbleStatusChanged?.Invoke(this, _rumbleCommand);
                }

                // Send the input update to the 
                // ViGEm controller
                UpdateController(port);

                // Update our listener with new button states
                _hotkeyListener.UpdateListener(_controllerStates[port], port);
            }

            // We've finished, send a new input frame to the event.
            InputFrameProcessed?.Invoke(this, _controllerStates);
        }

        public void UpdateController(int port)
        {
            // Only update this controller if it is connected.
            if (Controllers[port].ConnectionStatus == ConnectionStatus.Connected)
            {
                // Gather the maximum values for the control stick.
                if (Controllers[port].CalibrationStatus == CalibrationStatus.Calibrating)
                {
                    Controllers[port].Calibration.SetMinMax(_controllerStates[port]);
                }

                // Update each controller's inputs using the inputStates
                // Only update it if there isn't a recording playing
                if (!Controllers[port].IsPlayingRecording)
                    Controllers[port].UpdateInputs(_controllerStates[port]);
            }
        }

        public void UpdateCalibration(int port, CalibrationStatus status)
        {
            // Update the calibration status on our controller
            Controllers[port].CalibrationStatus = status;

            // Create new event args for our calibration changing
            CalibrationChangedEventArgs args = new CalibrationChangedEventArgs()
            {
                CalibrationStatus = status,
                TimeStamp = DateTime.UtcNow,
                ControllerPort = port
            };

            // Invoke the new event
            CalibrationStatusChanged?.Invoke(this, args);
        }

        public async Task PlayRecording(List<InputRecord> recording, int port, CancellationToken token, GamecubeDialog dialog = null)
        {
            // If the controller is not connected on this port, connect a virtual one.
            if (Controllers[port].ConnectionStatus != ConnectionStatus.Connected)
                Controllers[port].Connect(ControllerType.Virtual);

            // Begin playing the recording back.
            await Controllers[port].PlayRecording(recording, token, dialog);

            if (Controllers[port].ControllerType == ControllerType.Virtual)
            {
                Controllers[port].Disconnect();
            }
        }

        public void Start()
        {
            // Try to open the device and initialize it. If it's not connected do nothing
            InitializeDevice();
            if (AdapterStatus == ConnectionStatus.Connected)
            {
                // Setup the adapter commands and begin reading from it.
                _cancellationToken = new CancellationTokenSource();
                InitializeAdapter();
                BeginPolling();
            }
        }

        public void Stop()
        {
            // This should only get called when the application is being closed.
            _deviceNotificationListener.DeviceArrived -= DeviceArrived;
            _deviceNotificationListener.DeviceRemoved -= DeviceRemoved;
            _deviceNotificationListener.StopListen();

            if (AdapterStatus == ConnectionStatus.Disconnected)
                return;

            // Cancel out of the polling operation
            _cancellationToken.Cancel();
        }

        private void DeviceArrived(DeviceEventArgs obj)
        {
            // Check to see if our newly arrived device is 
            // a Gamecube Controller Adapter.

            // If so setup the commands for it.
            Start();
        }

        private void DeviceRemoved(DeviceEventArgs obj)
        {

            // If our adapterDevice is not null but the adapter has been disconnected, shut down
            // the device handles.
            if (!Device.IsConnected(VENDOR_ID, PRODUCT_ID) && _adapterDevice != null)
            {
                // Cancel out of the polling loop
                _cancellationToken?.Cancel();
                _adapterDevice.Close();
                _adapterDevice = null;

                // Update all of the controllers
                for (int i = 0; i < 4; i++)
                {
                    // Disconnect any connected controllers
                    Controllers[i]?.Disconnect();

                    // Create new args for our controllers being disconnected
                    ControllerConnectionStatusEventArgs args = new ControllerConnectionStatusEventArgs()
                    {
                        ConnectionStatus = ConnectionStatus.Disconnected,
                        TimeStamp = DateTime.UtcNow,
                        ControllerPort = i,
                        ControllerType = ControllerType.None,
                    };

                    // Invoke the event
                    ControllerConnectionStatusChanged.Invoke(this, args);
                }

                // Our adapter is now shut down, set this to disconnected.
                AdapterStatus = ConnectionStatus.Disconnected;
            }
        }

        private void hotkeyListener_CalibrationEventRequested(object? sender, int port)
        {
            // Block the event from happening if we're already calibrating.
            if (Controllers[port].CalibrationStatus == CalibrationStatus.Calibrating)
                return;

            // Send a notification that we're starting the calibration.
            ToastNotificationHandler.ShowNotification(Strings.ToastHeaderCalibrationBeginning, Strings.ToastCalibrationBeginning);

            // Set the adapter to begin pulling calibration values
            UpdateCalibration(port, CalibrationStatus.Calibrating);

            // Asynchronously wait for 5 seconds, then set it as calibrated.
            Task.Run(async () =>
            {
                await Task.Delay(5000);
                UpdateCalibration(port, CalibrationStatus.Calibrated);
            }).ContinueWith(task =>
            {
                // Tell the user that we've updated current calibration.
                ToastNotificationHandler.ShowNotification(Strings.ToastHeaderCalibrationComplete, Strings.NotificationCalibrationComplete);

                float[] ranges = Controllers[port].Calibration.GetRange();
                ProfileManager.CurrentProfiles[port].LeftStickRange = ranges[0];
                ProfileManager.CurrentProfiles[port].RightStickRange = ranges[1];
            });
        }

        private void ControllerCalibrationStatusChanged(object sender, CalibrationChangedEventArgs e)
        {
            // Only continue if this is for our controller being calibrated.
            if (e.CalibrationStatus == CalibrationStatus.Calibrated)
            {
                // Update the current profiles with the calibration values.
                float[] stickRanges = Controllers[e.ControllerPort].Calibration.GetRange();
                ProfileManager.CurrentProfiles[e.ControllerPort].LeftStickRange = stickRanges[0];
                ProfileManager.CurrentProfiles[e.ControllerPort].RightStickRange = stickRanges[1];
            }
        }

        private void ControllerConnectionChanged(object? sender, ControllerConnectionStatusEventArgs e)
        {
            // If the connection status changed to connected,
            // connect the controller. If not, disconnect it.
            if (e.ConnectionStatus == ConnectionStatus.Connected)
            {
                Controllers[e.ControllerPort].Connect(e.ControllerType);
            }

            else
            {
                Controllers[e.ControllerPort].Disconnect();
            }
        }

        // Pass the hotkey events to our own event handlers in this class
        private void _hotkeyListener_StopRecordingInputEventRequested(object? sender, int port) => StopRecordingInputEventRequested.Invoke(sender, port);
        private void _hotkeyListener_StartRecordingInputEventRequested(object? sender, int port) => StartRecordingInputEventRequested.Invoke(sender, port);
        private void _hotkeyListener_BeginPlaybackEventRequested(object? sender, int port) => BeginPlaybackEventRequested.Invoke(sender, port);
        private void _hotkeyListener_StopPlaybackEventRequested(object? sender, int port) => StopPlaybackEventRequested.Invoke(sender, port);

        // Write our updated rumble status to the adapter.
        private void RumbleStatusUpdated(object? sender, byte[] e)
        {
            _adapterDevice.Write(e);
            Debug.WriteLine("rumble changed");
        }
    }
}
