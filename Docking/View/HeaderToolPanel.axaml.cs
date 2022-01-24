using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AvaloniaTestMVVM.Docking.View
{
    public class HeaderToolPanel : UserControl
    {
        private string _header;

        public string Header
        {
            get { return _header; }
            set { _header = value;
                //OnPropertyChanged();
            }
        }

        public HeaderToolPanel()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}