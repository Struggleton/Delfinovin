using Delfinovin.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;

namespace Delfinovin.Controllers
{
    /// <summary>
    /// A class providing functionality to listen
    /// and record inputs to a list for playback.
    /// </summary>
    public class RecordingHandler
    {
        public bool IsRecording { get; private set; }

        public event EventHandler RecordingStarted;
        public event EventHandler RecordingStopped;

        private List<InputRecord> _currentRecording;
        private int _controllerPort;

        private ControllerStatus _previousInput;
        private InputRecord _currentRecord;

        private Stopwatch _heldStopwatch;
        private GamecubeAdapter _adapter;
        private GamecubeDialog _dialog;

        public RecordingHandler(ref GamecubeAdapter adapter)
        {
            _adapter = adapter;
        }

        public void SetDialog(ref GamecubeDialog dialog)
        {
            _dialog = dialog;
        }

        /// <summary>
        /// Begin recording inputs on the specified port.
        /// </summary>
        /// <param name="port">The port to begin reading and recording inputs on.</param>
        public void BeginRecording(int port)
        {
            // We're already recording, don't start a second one
            if (IsRecording)
                return;

            // Assign the port to receive inputs on
            _controllerPort = port;

            // Initialize necessary values
            InitRecording();

            // Subscribe to the data event
            _adapter.InputFrameProcessed += InputFrameProcessed;

            // We've started recording, raise this event
            RecordingStarted?.Invoke(this, EventArgs.Empty);

            // We're recording, set this to true.
            IsRecording = true;
        }

        private void InitRecording()
        {
            // Create these for later
            _currentRecording = new List<InputRecord>();
            _previousInput = new ControllerStatus();
            _heldStopwatch = Stopwatch.StartNew();
            _heldStopwatch.Start();
        }

        private void InputFrameProcessed(object? sender, ControllerStatus[] inputs)
        {
            // Grab the input from the input frame
            ControllerStatus currentInput = inputs[_controllerPort];

            // If this input is different from the previous one, continue.
            if (!currentInput.IsEqual(_previousInput))
            {
                // Get for how long the previous input was held for.
                _currentRecord.TimePressed = (int)_heldStopwatch.ElapsedMilliseconds;

                // Add the current input record to the list after setting the 
                // amount of time it was held for
                _currentRecording.Add(_currentRecord);

                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    _dialog?.UpdateDialog(currentInput);
                });

                // Create a new record with our current inputs' values
                _currentRecord = new InputRecord()
                {
                    ButtonsPressed = currentInput.Buttons,
                    Triggers = currentInput.Triggers,
                    LStick = currentInput.LStick,
                    RStick = currentInput.RStick,
                };

                // Restart the stopwatch and begin counting
                // how long between this and the next different
                // input takes.
                _heldStopwatch.Restart();

                // Update the previous input with the current one.
                _previousInput = currentInput;
            }
        }

        /// <summary>
        /// Stop a currently active recording.
        /// </summary>
        public void StopRecording() 
        { 
            // If we're recording, stop it and raise the event
            // that we've stopped.
            if (IsRecording)
            {
                IsRecording = false;
                _adapter.InputFrameProcessed -= InputFrameProcessed;
                RecordingStopped?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Get the current recording list. 
        /// </summary>
        /// <param name="recording">The list to save the list of inputs to.</param>
        /// <returns>Boolean - whether or not the RecordingHandler was recording</returns>
        public bool TryGetCurrentRecording(out List<InputRecord> recording)
        {
            // Pass the list of input records out.
            // Return if we were recording or not.
            recording = _currentRecording;
            return !IsRecording;
        }
    }
}
