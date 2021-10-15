using System.Threading;
using System.Windows;

namespace DelfinovinUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Mutex _mutex = null;
        protected override void OnStartup(StartupEventArgs e)
        {
            bool aIsNewInstance = false;
            _mutex = new Mutex(true, "DelfinovinUI", out aIsNewInstance);
            if (!aIsNewInstance)
            {

                App.Current.Shutdown();
            }
        }
    }
}
