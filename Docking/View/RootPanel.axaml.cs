using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AvaloniaTestMVVM.Docking.View
{
    public class RootPanel : UserControl
    {
        #region Ctors
        
        public RootPanel()
        {
            InitializeComponent();
        }
        
        #endregion

        #region Methods
        
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        #endregion
    }
}