using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using AvaloniaTestMVVM.Docking.View;

namespace AvaloniaTestMVVM.Views
{
    public class FloatingWindow : Window
    {
        public FloatingWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        public FloatingWindow(LayoutPanel content) : this()
        {
            var rootPanel = new RootPanel(content);
            this.Content = rootPanel;
            rootPanel.Cleared += this.Close;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}