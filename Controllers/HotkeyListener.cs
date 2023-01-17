using System;
using System.Timers;
using UserSettings = Delfinovin.Properties.Settings;

namespace Delfinovin.Controllers
{
    /// <summary>
    /// Provides functionality to raise events based on if
    /// specified hotkeys are pressed.
    /// </summary>
    public class HotkeyListener
    {
        private const int TIMER_DURATION = 4500;

        // Get all of the hot keys from the application settings. 
        // Cast the values from a long value to a GamecubeControllerButtons enum
        private GamecubeControllerButtons _calibrationHotkey => (GamecubeControllerButtons)UserSettings.Default.CalibrationHotkey;
        private GamecubeControllerButtons _startRecordingHotkey => (GamecubeControllerButtons)UserSettings.Default.StartRecordingHotkey;
        private GamecubeControllerButtons _stopRecordingHotkey => (GamecubeControllerButtons)UserSettings.Default.StopRecordingHotkey;
        private GamecubeControllerButtons _beginPlaybackHotkey => (GamecubeControllerButtons)UserSettings.Default.BeginPlaybackHotkey;
        private GamecubeControllerButtons _stopPlaybackHotkey => (GamecubeControllerButtons)UserSettings.Default.StopPlaybackHotkey;

        private bool _calibrationEventRaised = false;
        private bool _startRecordingEventRaised = false;
        private bool _stopRecordingEventRaised = false;
        private bool _beginPlaybackEventRaised = false;
        private bool _stopPlaybackEventRaised = false;

        // Timers for delaying the raising of events.
        private Timer _calibrationEventTimer;
        private Timer _startRecordingTimer;
        private Timer _stopRecordingTimer;
        private Timer _beginPlaybackTimer;
        private Timer _stopPlaybackTimer;

        public event EventHandler<int>? CalibrationEventRequested;
        public event EventHandler<int>? StartRecordingInputEventRequested;
        public event EventHandler<int>? StopRecordingInputEventRequested;
        public event EventHandler<int>? BeginPlaybackEventRequested;
        public event EventHandler<int>? StopPlaybackEventRequested;

        public HotkeyListener()
        {

        }

        /// <summary>
        /// Update the listener with new input.
        /// </summary>
        /// <param name="input">The controller input to update the listener with</param>
        /// <param name="port">The port the controller input was passed under.</param>
        public void UpdateListener(ControllerStatus input, int port)
        {
            // If any value is pressed, invoke the corresponding event.
            if (IsHotkeyPressed(input, _calibrationHotkey) && !_calibrationEventRaised)
            {
                // Raise the event
                CalibrationEventRequested?.Invoke(this, port);

                // Block subsquent event raising 
                _calibrationEventRaised = true;

                // Create a timer to reset our flag in 3 seconds
                _calibrationEventTimer = new Timer(TIMER_DURATION);
                _calibrationEventTimer.Elapsed += (sender, e) =>
                {
                    _calibrationEventRaised = false;
                };

                // Start the timer
                _calibrationEventTimer.Start();
            }

            if (IsHotkeyPressed(input, _startRecordingHotkey) && !_startRecordingEventRaised)
            {
                StartRecordingInputEventRequested?.Invoke(this, port);
                _startRecordingEventRaised = true;

                _startRecordingTimer = new Timer(TIMER_DURATION);
                _startRecordingTimer.Elapsed += (sender, e) =>
                {
                    _startRecordingEventRaised = false;
                };

                _startRecordingTimer.Start();
            }

            if (IsHotkeyPressed(input, _stopRecordingHotkey) && !_stopRecordingEventRaised)
            {
                StopRecordingInputEventRequested?.Invoke(this, port);
                _stopRecordingEventRaised = true;

                _stopRecordingTimer = new Timer(TIMER_DURATION);
                _stopRecordingTimer.Elapsed += (sender, e) =>
                {
                    _stopRecordingEventRaised = false;
                };

                _stopRecordingTimer.Start();
            }

            if (IsHotkeyPressed(input, _beginPlaybackHotkey) && !_beginPlaybackEventRaised)
            {
                BeginPlaybackEventRequested?.Invoke(this, port);
                _beginPlaybackEventRaised = true;

                _beginPlaybackTimer = new Timer(TIMER_DURATION);
                _beginPlaybackTimer.Elapsed += (sender, e) =>
                {
                    _beginPlaybackEventRaised = false;
                };


                _beginPlaybackTimer.Start();
            }

            if (IsHotkeyPressed(input, _stopPlaybackHotkey) && !_stopPlaybackEventRaised)
            {
                StopPlaybackEventRequested?.Invoke(this, port);
                _stopPlaybackEventRaised = true;

                _stopPlaybackTimer = new Timer(TIMER_DURATION);
                _stopPlaybackTimer.Elapsed += (sender, e) =>
                {
                    _stopPlaybackEventRaised = false;
                };


                _stopPlaybackTimer.Start();
            }
        }

        private bool IsHotkeyPressed(ControllerStatus input, GamecubeControllerButtons hotkeys) 
        {
            // Verify that the hotkey is not set to none and that the button's been pressed
            return hotkeys != GamecubeControllerButtons.None && input.IsButtonPressed(hotkeys);
        }
    }
}
