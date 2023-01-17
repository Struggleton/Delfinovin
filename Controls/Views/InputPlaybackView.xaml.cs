using Delfinovin.Controllers;
using Delfinovin.Controls.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Forms = System.Windows.Forms;

namespace Delfinovin.Controls
{
    /// <summary>
    /// Interaction logic for InputPlaybackView.xaml
    /// </summary>
    public partial class InputPlaybackView : UserControl
    {
        private const string STORED_RECORDING_NAME = "*Stored Recording";
        private const string PLAY_ICON_PATH = "/Delfinovin;component/Resources/Icons/play.png";
        private const string NO_RECORDING_ICON_PATH = "/Delfinovin;component/Resources/Icons/window-close.png";
        private const string BEGIN_RECORDING_ICON_PATH = "/Delfinovin;component/Resources/Icons/begin-recording.png";
        private const string STOP_RECORDING_ICON_PATH = "/Delfinovin;component/Resources/Icons/stop-recording.png";
        private const string RECORDING_IN_PROGRESS_PATH = "/Delfinovin;component/Resources/Icons/recording-in-progress.png";

        public int ControllerPort { get; set; }
        public bool IsPlaying { get; set; }

        private readonly GamecubeAdapter _adapter;
        private RecordingHandler _recordingHandler;
        private Recording _storedRecording;
        private Recording _selectedRecording;

        private System.Timers.Timer _playbackTimer;

        private CancellationTokenSource _playbackCancellationToken;

        public InputPlaybackView(ref GamecubeAdapter adapter)
        {
            InitializeComponent();
            this.DataContext = this;
            _adapter = adapter;

            _recordingHandler = new RecordingHandler(ref adapter);
            _recordingHandler.SetDialog(ref controllerDialog);
            _storedRecording = new Recording();
            _selectedRecording = new Recording();

            UpdateRecordingList();
        }

        private void UpdateRecordingList()
        {
            // Get a list of recording names from the recording directory
            List<string> recordingNames = RecordingManager.GetRecordingNameList();

            // Create a new list to store the recordings with
            List<Recording> recordings = new List<Recording>();

            // Clear the saved recording list view
            ClearRecordingList();

            foreach (string recordingName in recordingNames) 
            {
                recordings.Add(RecordingManager.GetRecordingFromRecordingName(recordingName));
            }

            foreach (Recording recording in recordings) 
            {
                // Create a imageListItem from the recording and it to the saved
                // recording list.
                ImageListItem recordingListItem = CreateItemFromRecording(recording);
                savedRecordingsList.Items.Add(recordingListItem);
            }
        }

        private void ClearRecordingList()
        {
            if (savedRecordingsList.Items.Count > 1)
            {
                for (int i = savedRecordingsList.Items.Count - 1; i > 0; i--)
                {
                    savedRecordingsList.Items.RemoveAt(i);
                }
            }
        }

        private ImageListItem CreateItemFromRecording(Recording recording)
        {
            // Create a new ImageListItem
            ImageListItem imageItem = new ImageListItem();

            // Get the timespan for the recording
            string recordingTimespan = GetTimespanString(recording.Records.Sum(x => x.TimePressed));

            // Fill out the image item with its name, text and an image
            imageItem.ItemText = $"{recording.RecordingName} ({recordingTimespan})";
            imageItem.ImageSource = PLAY_ICON_PATH;
            imageItem.Tag = recording.RecordingName;

            // Create a new context menu for deleting 
            // the recording from the list.
            imageItem.ContextMenu = new ContextMenu();

            // Create the menu item, subscribe to the event
            // and add it to the context menu
            MenuItem deleteItem = new MenuItem();
            deleteItem.Header = Strings.Delete;

            // Pass the click event the imageItem
            deleteItem.Click += (sender, e) => DeleteItem_Click(imageItem, e);

            imageItem.ContextMenu.Items.Add(deleteItem);

            // Return our image item
            return imageItem;
        }

        private string GetTimespanString(int milliseconds)
        {
            TimeSpan t = TimeSpan.FromMilliseconds(milliseconds);
            return string.Format("{0:D1}:{1:D2}", t.Minutes, t.Seconds);
        }

        private void DeleteItem_Click(ImageListItem imageItem, RoutedEventArgs e)
        {
            // Get the recording name from the imageItems' tag
            string recordingName = imageItem.Tag.ToString();
            
            // Format a string with the delete recording message
            string deleteRecordingMessage = string.Format(Strings.PromptDeleteRecording, recordingName);

            MessageDialog deleteDialog = new MessageDialog(deleteRecordingMessage);
            deleteDialog.ShowDialog();

            bool delete = deleteDialog.Result == Forms.DialogResult.Yes;
            if (!delete)
                return;

            RecordingManager.DeleteRecordingByRecordName(recordingName);

            // Tell the user we've deleted the recording.
            MessageDialog messageDialog = new MessageDialog(Strings.NotificationRecordingDeleted, Forms.MessageBoxButtons.OK);
            messageDialog.ShowDialog();

            // Refresh the recording list.
            UpdateRecordingList();
        }

        private void RecordingButton_Click(object sender, RoutedEventArgs e)
        {
            if (_recordingHandler.IsRecording)
            {
                StopRecording();
                return;
            }

            if (!string.IsNullOrEmpty(storedRecordingItem.Tag.ToString()))
            {
                MessageDialog recordDialog = new MessageDialog(Strings.PromptOverwriteRecording);
                recordDialog.ShowDialog();

                bool record = recordDialog.Result == Forms.DialogResult.Yes;
                if (!record) 
                    return;
            }

            BeginRecording();
        }

        public async Task StartPlayback()
        {
            if (_recordingHandler.IsRecording)
            {
                ToastNotificationHandler.ShowNotification(Strings.ToastHeaderRecordingInProgress, Strings.ToastRecordingInProgress);
                return;
            }

            if (_selectedRecording.Records.Sum(x => x.TimePressed) <= 0)
            {
                ToastNotificationHandler.ShowNotification(Strings.ToastHeaderNoRecordingSelected, Strings.ToastRecordingNotSelected);
                return;
            }

            if (IsPlaying)
            {
                ToastNotificationHandler.ShowNotification(Strings.ToastHeaderPlaybackInProgress, Strings.ToastStopCurrentPlayback);
                return;
            }



            InitPlayback();
            _playbackTimer.Start();
            IsPlaying = true;

            ToastNotificationHandler.ShowNotification(Strings.ToastHeaderPlaybackBegining, Strings.ToastPlaybackBeginning);

            await _adapter.PlayRecording(_selectedRecording.Records, ControllerPort, _playbackCancellationToken.Token, controllerDialog);
            IsPlaying = false;

            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                playButton.IsEnabled = true;
                recordingButton.IsEnabled = true;
                stopButton.IsEnabled = false;
            });
        }

        private void InitPlayback()
        {
            _playbackCancellationToken = new CancellationTokenSource();
            _playbackTimer = new System.Timers.Timer(16);
            _playbackTimer.Elapsed += PlaybackTimerTickElapsed;

            Application.Current.Dispatcher.BeginInvoke(() => 
            {
                controllerDialog.UpdateDialog(new ControllerStatus());
                timeSlider.Maximum = _selectedRecording.Records.Sum(x => x.TimePressed);
                timeSlider.Value = 0;

                playButton.IsEnabled = false;
                recordingButton.IsEnabled = false;
                stopButton.IsEnabled = true;
            });
        }

        public void StopPlayback() 
        { 
            if (!IsPlaying)
            {
                ToastNotificationHandler.ShowNotification(Strings.HeaderInputDisplay, Strings.ToastHeaderNoRecordingSession);
                return;
            }

            _playbackCancellationToken.Cancel();
            _playbackTimer.Stop();

            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                timeSlider.Value = 0;

                controllerDialog.UpdateDialog(new ControllerStatus());

                playButton.IsEnabled = true;
                recordingButton.IsEnabled = true;
            });

            IsPlaying = false;
        }

        private void PlaybackTimerTickElapsed(object? sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                timeSlider.Value += 16;

                // If we've exceeded the slider's maximum, stop the timer
                // and reset it back to 0.
                if (timeSlider.Value >= timeSlider.Maximum)
                {
                    controllerDialog.UpdateDialog(new ControllerStatus());
                    _playbackTimer.Stop();
                    timeSlider.Value = 0;
                }
            });
        }

        public void BeginRecording()
        {
            if (IsPlaying)
            {
                ToastNotificationHandler.ShowNotification(Strings.ToastHeaderPlaybackInProgress, Strings.ToastPlaybackInProgress);
                return;
            }

            if (_recordingHandler.IsRecording)
            {
                ToastNotificationHandler.ShowNotification(Strings.ToastHeaderRecordingInProgress, Strings.ToastStopCurrentRecording);
                return;
            }

            // Start recording inputs and send a notification that we've started.
            ToastNotificationHandler.ShowNotification(Strings.ToastHeaderRecordingStarting, Strings.ToastRecordingStarting);
            _recordingHandler.BeginRecording(ControllerPort);

            Application.Current.Dispatcher.Invoke(() =>
            {
                recordingBtnImage.Source = new BitmapImage(new Uri(STOP_RECORDING_ICON_PATH, UriKind.Relative));
                storedRecordingItem.ItemText = Strings.ListItemRecordingInProgress;
                storedRecordingItem.ImageSource = RECORDING_IN_PROGRESS_PATH;
                stopButton.IsEnabled = false;
                playButton.IsEnabled = false;
            });
        }

        public void StopRecording()
        {
            if (!_recordingHandler.IsRecording)
            {
                ToastNotificationHandler.ShowNotification(Strings.ToastHeaderNoRecordingSession, Strings.ToastNoRecordingStop);
                return;
            }

            // Tell the recordingHandler to stop recording inputs
            _recordingHandler.StopRecording();

            // Get the inputs from the recording handler
            _recordingHandler.TryGetCurrentRecording(out List<InputRecord> outputRecording);

            // Create a new recording to store the inputs
            _storedRecording = new Recording();
            _storedRecording.Records = outputRecording;

            // If we're already recording, stop recording and store it
            ToastNotificationHandler.ShowNotification(Strings.ToastHeaderRecordingStopping, Strings.ToastRecordingStopping);

            Application.Current.Dispatcher.Invoke(() =>
            {
                // Update our stored recording item
                string recordingLength = GetTimespanString(_storedRecording.Records.Sum(x => x.TimePressed));
                storedRecordingItem.ItemText = string.Format(Strings.MenuItemStoredRecording, recordingLength);
                storedRecordingItem.ImageSource = PLAY_ICON_PATH;
                storedRecordingItem.Tag = STORED_RECORDING_NAME;

                // Update the recording button image
                recordingBtnImage.Source = new BitmapImage(new Uri(BEGIN_RECORDING_ICON_PATH, UriKind.Relative));

                controllerDialog.UpdateDialog(new ControllerStatus());
                stopButton.IsEnabled = false;
                playButton.IsEnabled = true;
            });

            _selectedRecording = _storedRecording;
        }

        private void StoredRecording_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ImageListItem imageItem = (ImageListItem)sender;
            string recordingName = imageItem.Tag.ToString();

            if (string.IsNullOrEmpty(recordingName))
                return;

            // Create a context menu here so subsequent right clicks
            // don't apply previous menus
            storedRecordingItem.ContextMenu = new ContextMenu();

            // Add menu items and subscribe to the event
            MenuItem saveRecording = new MenuItem();
            saveRecording.Header = Strings.MenuItemSaveRecording;
            saveRecording.Click += SaveRecording_Click;

            MenuItem clearRecording = new MenuItem();
            clearRecording.Header = Strings.MenuItemClearRecording;
            clearRecording.Click += (sender, e) => ClearRecording_Click(imageItem, e);

            storedRecordingItem.ContextMenu.Items.Add(saveRecording);
            storedRecordingItem.ContextMenu.Items.Add(clearRecording);
        }

        private void ClearRecording_Click(ImageListItem imageItem, RoutedEventArgs e)
        {
            // Prompt the user if they want to clear the current recording.
            MessageDialog clearDialog = new MessageDialog(Strings.PromptClearRecording);
            clearDialog.ShowDialog();

            bool clearRecording = clearDialog.Result == Forms.DialogResult.Yes;

            if (!clearRecording)
                return;

            // Overwrite the stored recording.
            _storedRecording = new Recording();
            _selectedRecording = new Recording();

            // Reset the item to defaults.
            imageItem.ItemText = Strings.ListItemNoRecordingStored;
            imageItem.ImageSource = NO_RECORDING_ICON_PATH;
            imageItem.Tag = "";

            // Tell the user we've cleared the recording.
            MessageDialog messageDialog = new MessageDialog(Strings.NotificationRecordingCleared, Forms.MessageBoxButtons.OK);
            messageDialog.ShowDialog();
        }

        private void SaveRecording_Click(object sender, RoutedEventArgs e)
        {
            TextEntryWindow textEntry = new TextEntryWindow(Strings.PromptEnterRecordingName, Strings.HeaderInputRecording, Strings.Save);
            bool textEntered = (bool)textEntry.ShowDialog();

            if (!textEntered)
                return;

            RecordingManager.SaveRecording(_storedRecording, textEntry.EnteredText);

            string saveMessage = string.Format(Strings.NotificationRecordingSaved, textEntry.EnteredText);
            MessageDialog message = new MessageDialog(saveMessage, Forms.MessageBoxButtons.OK);
            message.ShowDialog();

            UpdateRecordingList();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            StopPlayback();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            ControllerPort = int.Parse(item.Tag.ToString());

            StartPlayback();
        }

        private void RecordingItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListViewItem listItem = (ListViewItem)sender;
            ImageListItem imageItem = (ImageListItem)listItem.Content;
            string recordingName = imageItem.Tag.ToString();

            if (string.IsNullOrEmpty(recordingName))
                return;

            if (IsPlaying)
                StopPlayback();

            if (recordingName == STORED_RECORDING_NAME)
                _selectedRecording = _storedRecording;

            else
                _selectedRecording = RecordingManager.GetRecordingFromRecordingName(recordingName);
        }
    }
}
