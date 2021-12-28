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
            this.Content = new Grid() { Background = Brushes.Red };
        }

        public FloatingWindow(LayoutPanel content)
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            this.Content = content;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}