using Octokit;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DelfinovinUI
{
    public static class Updater
    {
        private static async Task<Version> GetGithubVersion()
        {
            // Do this so we don't get rejected by the API
            GitHubClient client = new GitHubClient(new ProductHeaderValue("DelfinovinUI"));

            // Access the main repository
            var releases = await client.Repository.Release.GetAll("Struggleton", "Delfinovin");

            // Get the most current release
            var latest = releases[0];

            // Create a version object. We need to strip the nonnumeric chars from the
            // Github versioning. 
            Version githubVersion = new Version(Regex.Replace(latest.TagName, "[^0-9.]", ""));

            return githubVersion;
        }

        public static async void CheckCurrentRelease(bool silent)
        {
            // Get the version of Delfinovin on Github
            var githubVersion = await GetGithubVersion();

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
                    // TO-DO: pull this from a separate repository, so it can't be missing
                    try
                    {
                        Process.Start("DelfinovinUpdater.exe");
                    }

                    catch (Exception ex)
                    {
                        Process.Start("https://github.com/Struggleton/Delfinovin/releases/latest/");

                        MessageWindow errorWindow = new MessageWindow("Missing updater. Navigating to the github releases page.", true);
                        errorWindow.Show();  
                    }
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
    }
}
