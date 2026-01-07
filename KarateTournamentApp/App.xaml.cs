using System.Configuration;
using System.Data;
using System.Windows;
using KarateTournamentApp.Services;
using KarateTournamentApp.ViewModels;

namespace KarateTournamentApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var categoryManager = new CategoryManager();
            var mainViewModel = new MainViewModel(categoryManager);

            var mainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };

            mainWindow.Show();
        }
    }

}
