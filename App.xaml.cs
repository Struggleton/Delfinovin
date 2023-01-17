using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using IO = System.IO;
using UserSettings = Delfinovin.Properties.Settings;

namespace Delfinovin
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Mutex _appMutex;
        protected override void OnStartup(StartupEventArgs e)
        {
            UpdateRunningOldExecutable();
            IsInstanceRunning();
            ApplyThemes();

            base.OnStartup(e);
        }

        private void UpdateRunningOldExecutable()
        {
            var currentExecutablePath = Process.GetCurrentProcess().MainModule.FileName;
            string oldName = IO.Path.GetFileNameWithoutExtension(currentExecutablePath);

            if (oldName == "DelfinovinUI")
            {
                IO.File.Move(currentExecutablePath, "Delfinovin.exe");
            }
        }

        private void IsInstanceRunning()
        {
            // Create a new mutex. Check to see if there is a mutex already in play.
            // If so, shutdown this instance so not more than one is running.
            _appMutex = new Mutex(true, "Delfinovin", out bool aIsNewInstance);
            if (!aIsNewInstance)
            {
                Application.Current.Shutdown();
            }
        }

        private void ApplyThemes()
        {
            // Get the application themes and controller colors
            // from our user settings
            string applicationTheme = UserSettings.Default.ApplicationTheme;
            string controllerColor = UserSettings.Default.ControllerColor;

            if (!String.IsNullOrEmpty(controllerColor))
            {
                // Parse our controller color to an enum value
                if (Enum.TryParse(controllerColor, out ControllerColor color))
                {
                    // Case the controllerColor into an uint
                    uint hexColorCode = (uint)color;

                    // Get the color value from the hex code
                    Color convertedColor = ControlExtensions.GetColorFromHex(hexColorCode);

                    // Set the resource "ControllerColor" to with the new color 
                    Application.Current.Resources["ControllerColor"] = new SolidColorBrush(convertedColor);
                }
            }

            if (!String.IsNullOrEmpty(applicationTheme))
            {
                // Update the application-wide theme with the selected one.
                Application.Current.Resources.MergedDictionaries[0] = new ResourceDictionary()
                {
                    Source = new Uri($"/Delfinovin;component/Resources/Themes/{applicationTheme}.xaml", UriKind.Relative)
                };
            }
        }
    }
}
