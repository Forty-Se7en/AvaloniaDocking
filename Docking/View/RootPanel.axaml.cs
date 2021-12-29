using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AvaloniaTestMVVM.Docking.View
{
    public class RootPanel : UserControl
    {
        private Grid _mainGrid;
        
        #region Ctors
        
        public RootPanel()
        {
            InitializeComponent();
        }

        public RootPanel(LayoutPanel panel) : this()
        {
            _mainGrid = new Grid();
            this.Content = _mainGrid;
            panel.CloseRequest += PanelOnCloseRequest;
            panel.SwapRequest += PanelOnSwapRequest;
            panel.FlowRequest += PanelOnFlowRequest;
        }

        #endregion

        #region Methods
        
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        void RemoveLayout(LayoutPanel panel)
        {
            _mainGrid.Children.Remove(panel);
            _mainGrid.Children.Clear();
        }
        
        
        #endregion
        
        #region Handlers
        
        private void PanelOnCloseRequest(LayoutPanel sender)
        {
            sender.CloseRequest += PanelOnCloseRequest;
            sender.SwapRequest += PanelOnSwapRequest;
            sender.FlowRequest += PanelOnFlowRequest;
            RemoveLayout(sender);
        }

        private void PanelOnSwapRequest(LayoutPanel sender, LayoutPanel newPanel)
        {
            RemoveLayout(sender);
        }

        private void PanelOnFlowRequest(LayoutPanel sender)
        {
            RemoveLayout(sender);
        }

        #endregion
    }
}