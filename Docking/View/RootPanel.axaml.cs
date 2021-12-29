using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaTestMVVM.Views;

namespace AvaloniaTestMVVM.Docking.View
{
    public class RootPanel : UserControl
    {
        public event Action Cleared;
        
        private Grid _mainGrid;
        
        #region Ctors
        
        public RootPanel()
        {
            InitializeComponent();
            _mainGrid = new Grid();
            this.Content = _mainGrid;
        }

        public RootPanel(LayoutPanel panel) : this()
        {
            this.AddLayout(panel);
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
            
            panel.CloseRequest -= PanelOnCloseRequest;
            panel.SwapRequest -= PanelOnSwapRequest;
            panel.FlowRequest -= PanelOnFlowRequest;
        }

        void AddLayout(LayoutPanel panel)
        {
            _mainGrid.Children.Add(panel);
            
            panel.CloseRequest += PanelOnCloseRequest;
            panel.SwapRequest += PanelOnSwapRequest;
            panel.FlowRequest += PanelOnFlowRequest;
        }
        
        #endregion
        
        #region Handlers
        
        private void PanelOnCloseRequest(LayoutPanel sender)
        {
            RemoveLayout(sender);
            Cleared?.Invoke();
        }

        private void PanelOnSwapRequest(LayoutPanel sender, LayoutPanel newPanel)
        {
            RemoveLayout(sender);
            AddLayout(newPanel);
        }

        private void PanelOnFlowRequest(LayoutPanel sender)
        {
            RemoveLayout(sender);
            new FloatingWindow(sender).Show();
            Cleared?.Invoke();
        }

        #endregion
    }
}