using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace AvaloniaTestMVVM.Docking.View
{
    public class LocationControl : UserControl
    {
        private Dictionary<Image, Panel> _panels;
        
        public LocationControl()
        {
            InitializeComponent();
            
            Grid panelsGrid = (Grid)(this.Content as Grid).Children.First(c => c.Name == "PanelsGrid");
            var topPanel = (Panel)panelsGrid.Children.First(c => c.Name == "PanelTop");
            var leftPanel = (Panel)panelsGrid.Children.First(c => c.Name == "PanelLeft");
            var centerPanel = (Panel)panelsGrid.Children.First(c => c.Name == "PanelCenter");
            var rightPanel = (Panel)panelsGrid.Children.First(c => c.Name == "PanelRight");
            var bottomPanel = (Panel)panelsGrid.Children.First(c => c.Name == "PanelBottom");
            
            Grid imagesGrid = (Grid)(this.Content as Grid).Children.First(g => g.Name == "ImagesGrid");
            var topImage = (Image) imagesGrid.Children.First(c => c.Name == "ImageTop");
            var leftImage = (Image) imagesGrid.Children.First(c => c.Name == "ImageLeft");
            var centerImage = (Image) imagesGrid.Children.First(c => c.Name == "ImageCenter");
            var rightImage = (Image) imagesGrid.Children.First(c => c.Name == "ImageRight");
            var bottomImage = (Image) imagesGrid.Children.First(c => c.Name == "ImageBottom");

            _panels = new Dictionary<Image, Panel>
            {
                {topImage, topPanel},
                {leftImage, leftPanel},
                {centerImage, centerPanel},
                {rightImage, rightPanel},
                {bottomImage, bottomPanel}
            };
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void LocationImage_OnPointerEnter(object? sender, PointerEventArgs e)
        {
            // switch (sender)
            // {
            //     case var _ when object.ReferenceEquals(sender, _topImage):
            //         _topPanel.IsVisible = true;
            //         break;
            // }

            _panels[sender as Image].IsVisible = true;

        }

        private void LocationImage_OnPointerLeave(object? sender, PointerEventArgs e)
        {
            _panels[sender as Image].IsVisible = false;
        }
    }
}