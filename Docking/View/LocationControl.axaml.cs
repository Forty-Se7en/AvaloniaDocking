using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Parsers;
using Avalonia.Markup.Xaml;
using AvaloniaTestMVVM.Docking.Model;

namespace AvaloniaTestMVVM.Docking.View
{
    public class LocationControl : UserControl
    {
        public event Action<ELocation> LocationSelected; 

        private Dictionary<Image, Locations> _panels;
        
        public LocationControl()
        {
            InitializeComponent();
            
            // Grid panelsGrid = (Grid)(this.Content as Grid).Children.First(c => c.Name == "PanelsGrid");
            // var topPanel = (Panel)panelsGrid.Children.First(c => c.Name == "PanelTop");
            // var leftPanel = (Panel)panelsGrid.Children.First(c => c.Name == "PanelLeft");
            // var centerPanel = (Panel)panelsGrid.Children.First(c => c.Name == "PanelCenter");
            // var rightPanel = (Panel)panelsGrid.Children.First(c => c.Name == "PanelRight");
            // var bottomPanel = (Panel)panelsGrid.Children.First(c => c.Name == "PanelBottom");
            //
            // Grid imagesGrid = (Grid)(this.Content as Grid).Children.First(g => g.Name == "ImagesGrid");
            // var topImage = (Image) imagesGrid.Children.First(c => c.Name == "ImageTop");
            // var leftImage = (Image) imagesGrid.Children.First(c => c.Name == "ImageLeft");
            // var centerImage = (Image) imagesGrid.Children.First(c => c.Name == "ImageCenter");
            // var rightImage = (Image) imagesGrid.Children.First(c => c.Name == "ImageRight");
            // var bottomImage = (Image) imagesGrid.Children.First(c => c.Name == "ImageBottom");
            
            var topImage = this.FindControl<Image>("ImageTop");
            var leftImage = this.FindControl<Image>("ImageLeft");
            var centerImage = this.FindControl<Image>("ImageCenter");
            var rightImage = this.FindControl<Image>("ImageRight");
            var bottomImage = this.FindControl<Image>("ImageBottom");
            
            var topPanel = this.FindControl<Panel>("PanelTop");
            var leftPanel = this.FindControl<Panel>("PanelLeft");
            var centerPanel = this.FindControl<Panel>("PanelCenter");
            var rightPanel = this.FindControl<Panel>("PanelRight");
            var bottomPanel = this.FindControl<Panel>("PanelBottom");

            _panels = new Dictionary<Image, Locations>
            {
                {topImage, new Locations(topPanel, ELocation.Top)},
                {leftImage, new Locations(leftPanel, ELocation.Left)},
                {centerImage, new Locations(centerPanel, ELocation.Inside)},
                {rightImage, new Locations(rightPanel, ELocation.Right)},
                {bottomImage, new Locations(bottomPanel, ELocation.Bottom)}
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

            _panels[sender as Image].Panel.IsVisible = true;

        }

        private void LocationImage_OnPointerLeave(object? sender, PointerEventArgs e)
        {
            _panels[sender as Image].Panel.IsVisible = false;
        }

        private void LocationImage_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            LocationSelected?.Invoke(_panels[sender as Image].Location);
        }
    }

    class Locations
    {
        public Panel Panel;

        public ELocation Location;

        public Locations(Panel panel, ELocation location)
        {
            Panel = panel;
            Location = location;
        }
    }
}