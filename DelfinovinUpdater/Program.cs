using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DelfinovinUpdater
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                DeleteWorkspace();

                // Create a WebClient object and download the newest release.
                using (WebClient webClient = new WebClient())
                {
                    Console.WriteLine("Downloading latest Delfinovin release.");
                    await webClient.DownloadFileTaskAsync(new Uri("https://github.com/Struggleton/Delfinovin/releases/latest/download/Delfinovin.zip"), "DelfinovinLatest.zip");
                }

                // Check to see if there is a current config
                // if so, back it up.
                if (File.Exists("settings.txt"))
                {
                    // Backup settings config
                    Console.WriteLine("Backing up current config.");
                    File.Move("settings.txt", "settings.bak");
                }

                // Get the current running process of Delfinovin and kill them.
                Console.WriteLine("Closing any current instances of Delfinovin...");
                Process[] processes = Process.GetProcessesByName("DelfinovinUI");
                if (processes.Length > 0)
                {
                    foreach (Process p in processes)
                        p.Kill();
                }

                UnpackRelease();

                // Check to see if previous settings exist.
                // if so, pull them in.
                if (File.Exists("settings.bak"))
                    File.Delete("settings.txt");

                File.Move("settings.bak", "settings.txt");

                // Start Delfinovin
                Console.WriteLine("Starting Delfinovin.");
                Process.Start("DelfinovinUI.exe");

                DeleteWorkspace();
            }

            catch (Exception ex)
            {
                Console.WriteLine("Could not download latest release. Check your internet connection.");
                Console.WriteLine(ex.Message);
            }
        }

        static void DeleteWorkspace()
        {
            // If the directory for updates exists, delete it.
            if (Directory.Exists("update"))
                Directory.Delete("update", true);

            if (File.Exists("DelfinovinLatest.zip"))
                File.Delete("DelfinovinLatest.zip");
        }

        static void UnpackRelease()
        {
            // Extract files from the downloaded zip to an update folder
            Console.WriteLine("Unpacking release...");
            ZipFile.ExtractToDirectory("DelfinovinLatest.zip", "update");

            // Get all the files from the update directory and try to move them
            string[] files = Directory.GetFiles("update");
            foreach (string file in files)
            {
                try
                {
                    File.Copy(file, Path.GetFileName(file), true);
                }

                // Skip the file copy if it can't be copied
                catch (Exception ex)
                {
                    continue;
                }

            }
        }
    }
}
