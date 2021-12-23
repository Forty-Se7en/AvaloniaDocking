using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaTestMVVM.Docking.View;
using AvaloniaTestMVVM.ViewModels;
using AvaloniaTestMVVM.Views;

namespace AvaloniaTestMVVM
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel()
                };

                desktop.MainWindow.Content = new LayoutPanel(LayoutPanel.CreateRandomContent());
            }
            
            

            base.OnFrameworkInitializationCompleted();
        }
    }
}