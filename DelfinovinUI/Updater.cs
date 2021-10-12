using Octokit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DelfinovinUI
{
    public static class Updater
    {
        public static async void CheckCurrentRelease(bool silent)
        {
            try
            {
                // Do this so we don't get rejected by the API
                var client = new GitHubClient(new ProductHeaderValue("DelfinovinUI"));

                // Access the main repository
                var releases = await client.Repository.Release.GetAll("Struggleton", "Delfinovin");

                // Get the most current release
                var latest = releases[0];

                // Create a version object. We need to strip the nonnumeric chars from the
                // Github versioning. 
                Version githubVersion = new Version(Regex.Replace(latest.TagName, "[^0-9.]", ""));

                // Get the application's current version and compare it to the github version.
                Version localVersion = typeof(Updater).Assembly.GetName().Version;
                int versionComparison = localVersion.CompareTo(githubVersion);

                // The version on GitHub is more up to date than this local release.
                if (versionComparison < 0)
                {
                    string updateText = string.Format("There is a newer version of Delfinovin (Current is v{0}, Newer is v{1}) available for download!\n"
                                                + "Would you like to?", localVersion.ToString(), githubVersion.ToString());

                    MessageWindow updateWindow = new MessageWindow(updateText, true, true, "Yes", "No");
                    updateWindow.ShowDialog();


                    if (updateWindow.Result == WindowResult.OK)
                    {
                        DownloadAndUnpackNewRelease();
                    }

                }

                // The local version is more up to date or equal to the newest version on Github
                else
                {
                    // Supress the message window 
                    if (!silent)
                    {
                        MessageWindow updateWindow = new MessageWindow("You are on the most up to date version!");
                        updateWindow.ShowDialog();
                    }
                }
            }

            catch (Exception e)
            {

            }
        }

        private static async void DownloadAndUnpackNewRelease()
        {
            // Create a WebClient object and download the newest release.
            using (WebClient webClient = new WebClient())
            {
                await webClient.DownloadFileTaskAsync(new Uri("https://github.com/Struggleton/Delfinovin/releases/latest/download/Delfinovin.zip"), "DelfinovinLatest.zip");
            }

            // If the directory for updates exists, delete it.
            if (Directory.Exists("update"))
                Directory.Delete("update", true);

            // Extract files from the downloaded zip to an update folder
            ZipFile.ExtractToDirectory("DelfinovinLatest.zip", "update");

            string[] updateScript =
            {
                // Disable text writing to the console
                "@echo off",

                // Kill the currently running process of Delfinovin
                "taskkill /f /im DelfinovinUI.exe > NUL", 

                 // Wait 1 second for the process to close 
                "TIMEOUT /t 1 /nobreak > NUL",

                // Make a copy of the settings file before overwriting it
                "copy \"settings.txt\" \"settings.bak\"",

                // Move the update files from the update folder to the working dir
                "move /y \"update\\*.*\"",

                // Bring settings file back
                "move /y \"settings.bak\" \"settings.txt\"",

                // Run Delfinovin
                "start DelfinovinUI.exe",

                // Delete the update archive
                "del /Q  \"DelfinovinLatest.zip\"",

                // Delete the update script
                "del /Q  \"updateScript.bat\""
            };

            // Write the script to a bat file and run it
            File.WriteAllLines("updateScript.bat", updateScript);
            Process.Start("updateScript.bat");
        }
    }
}
