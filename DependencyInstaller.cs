using Delfinovin.Controls.Windows;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Exceptions;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using Forms = System.Windows.Forms;

namespace Delfinovin
{
    /// <summary>
    /// A class for that provide functionality to check if 
    /// the dependencies (ViGEmBus, eventually WinUSB)
    /// are installed and provide installations.
    /// </summary>
    public static class DependencyInstaller
    {
        private const string VIGEM_RELEASE_LINK = "https://github.com/ViGEm/ViGEmBus/releases/download/v1.21.442.0/ViGEmBus_1.21.442_x64_x86_arm64.exe";
        private const string VIGEM_FILENAME = "ViGEmBusSetup_1.21.442.exe";

        /// <summary>
        /// Check whether or not ViGEmBus is installed.
        /// </summary>
        /// <returns>Boolean - return whether it is installed</returns>
        public static bool CheckViGEmInstallation()
        {
            bool vigemInstalled = false;
            try
            {
                // Try to create a new ViGEmClient.
                // If this doesn't exception, set the
                // variable to true.
                ViGEmClient client = new ViGEmClient();
                vigemInstalled = true;

                // Dispose of the client we created
                client.Dispose();
            }

            catch (VigemBusNotFoundException)
            {
                // ViGEmBus is not installed, 
                // set to false.
                vigemInstalled = false;
            }

            // return the install status.
            return vigemInstalled;
        }

        /// <summary>
        /// Download and install ViGEmBus from the Github repository.
        /// </summary>
        /// <returns></returns>
        public static async Task DownloadAndInstallViGEm()
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
                    // Download the setup file
                    await webClient.DownloadFileTaskAsync(new Uri(VIGEM_RELEASE_LINK), VIGEM_FILENAME);
                }

                catch (WebException ex)
                {
                    // Our download failed somehow, assume it's because of an internet issue.
                    MessageDialog errorMessage = new MessageDialog(Strings.ErrorDownloadFailed, Forms.MessageBoxButtons.OK);
                    errorMessage.ShowDialog();

                    return;
                }
            }
        }

        private static void InstallViGEmClient()
        {
            // Start the ViGEm installer.
            var process = Process.Start(VIGEM_FILENAME);

            // Wait for the installer process to exit (up-to 5 minutes.)
            process.WaitForExit(1000 * 60 * 5);

            // Get the current file name of the running application
            var currentExecutablePath = Process.GetCurrentProcess().MainModule.FileName;

            // Restart the application through Process.Start + Application.Shutdown
            Process.Start(currentExecutablePath);
            Application.Current.Shutdown();
        }

        private static void DownloadFileCompleted(object sender, AsyncCompletedEventArgs e, ProgressDisplayWindow progressWindow)
        {
            // Close the progress window
            progressWindow.Close();

            // Run the ViGEm installer.
            InstallViGEmClient();
        }

        private static void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e, ProgressDisplayWindow window)
        {
            window.Progress = e.ProgressPercentage;
        }
    }
}
