using Delfinovin.Controls.Windows;
using Nefarius.Utilities.GitHubUpdater;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using Forms = System.Windows.Forms;

namespace Delfinovin
{
    /// <summary>
    /// A class providing functionality to check, download and install new updates.
    /// </summary>
    public static class Updater
    {
        private const string DOWNLOAD_LINK = "https://github.com/Struggleton/Delfinovin/releases/latest/download/Delfinovin.zip";
        private const string GITHUB_USERNAME = "Struggleton";
        private const string REPOSITORY_NAME = "Delfinovin";
        private const string FILE_NAME = "DelfinovinLatest.zip";
        private const string UPDATE_DIR = ".\\updates";

        static Updater()
        {
            RemovePreviousFiles();
        }

        /// <summary>
        /// Check if there are new releases of the application.
        /// If so, prompt the user to update.
        /// </summary>
        /// <param name="silent">Supress no-update messages.</param>
        public static void CheckForUpdates(bool silent)
        {
            // Init an UpdateChecker using our github name/repository and
            // Check if there is a new update
            if (new UpdateChecker(GITHUB_USERNAME, REPOSITORY_NAME).IsUpdateAvailable)
            {
                // Tell the user there is an update and ask if they want to update
                MessageDialog updateDialog = new MessageDialog(Strings.PromptUpdateAvailable);
                updateDialog.ShowDialog();

                bool dialogResult = updateDialog.Result == Forms.DialogResult.Yes;

                // Download and unpack the update if they say yes
                if (dialogResult)
                    GetLatestRelease();
            }

            else
            {
                // If the silent flag is not set, create a new dialog telling the user 
                // there is no update
                if (!silent)
                {
                    MessageDialog messageDialog = new MessageDialog(Strings.NotificationNoUpdate, Forms.MessageBoxButtons.OK);
                    messageDialog.ShowDialog();
                }
            }
        }

        private static void RemovePreviousFiles()
        {
            // Clean up the old files and delete any previous update directories
            foreach (string oldFile in Directory.GetFiles(".\\", "*.old"))
            {
                File.Delete(oldFile);
            }

            if (Directory.Exists(UPDATE_DIR))
                Directory.Delete(UPDATE_DIR, true);
        }

        private static async Task GetLatestRelease()
        {
            // Create a WebClient object and download the newest release.
            using (WebClient webClient = new WebClient())
            {
                // Create a download progress window
                ProgressDisplayWindow progressWindow = new ProgressDisplayWindow();
                progressWindow.Show();

                // Subscribe to the download events and pass the progress window to it
                // so we can update it with info
                webClient.DownloadProgressChanged += (sender, e) => DownloadProgressChanged(sender, e, progressWindow);
                webClient.DownloadFileCompleted += (sender, e) => DownloadFileCompleted(sender, e, progressWindow);

                try
                {
                    // Download the file
                    await webClient.DownloadFileTaskAsync(new Uri(DOWNLOAD_LINK), FILE_NAME);
                }
                
                catch (WebException ex) 
                {
                    // Our download failed somehow, assume it's because of an internet issue.
                    MessageDialog errorDialog = new MessageDialog(Strings.ErrorDownloadFailed, Forms.MessageBoxButtons.OK);
                    errorDialog.ShowDialog();

                    return;
                }

                // If the directory for updates exists, delete it.
                if (Directory.Exists(UPDATE_DIR))
                    Directory.Delete(UPDATE_DIR, true);
                
                // Unzip the download to our update directory
                ZipFile.ExtractToDirectory(FILE_NAME, UPDATE_DIR);
                
                // Rename all the files in the current directory to .old.
                foreach (string fileName in Directory.GetFiles(".\\"))
                {
                    File.Move(fileName, fileName + ".old");
                }
                
                // Move the files from the update directory here
                foreach (string updateFile in Directory.GetFiles(UPDATE_DIR))
                {
                    File.Move(updateFile, Path.Combine(".\\", Path.GetFileName(updateFile)));
                }
            }

            // Get the current file name of the running application
            var currentExecutablePath = Process.GetCurrentProcess().MainModule.FileName;

            // Restart the application through Process.Start + Application.Shutdown
            Process.Start(currentExecutablePath);
            Application.Current.Shutdown();
        }

        private static void DownloadFileCompleted(object sender, AsyncCompletedEventArgs e, ProgressDisplayWindow progressWindow)
        {
            // When the download completes, close our progress window.
            progressWindow.Close();
        }

        private static void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e, ProgressDisplayWindow window)
        {
            // Update the progress bar window with the progress percent.
            window.Progress = e.ProgressPercentage;
        }
    }
}
