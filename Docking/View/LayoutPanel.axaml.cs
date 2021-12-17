using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using AvaloniaTestMVVM.Docking.Model;

namespace AvaloniaTestMVVM.Docking.View
{
    public class LayoutPanel : UserControl, ILayoutPanel
    {
        #region Events

        public event Action<object> Closed;
        
        #endregion
        
        #region Fields
        
        private Grid _mainGrid;
        private TabControl _tabControl;
        private GridSplitter _gridSplitter;

        #endregion
        
        #region Properties
        
        private LayoutPanel Parent { get; set; }
        
        private LayoutPanel Child1 { get; set; }
        
        private LayoutPanel Child2 { get; set; }
        
        public bool IsSplitted{ get; private set; }
        
        public EOrientation Orientation { get; private set; }
        
        #endregion

        public LayoutPanel()
        {
            InitializeComponent();
            _mainGrid = this.FindControl<Grid>("MainGrid");
            _tabControl = this.FindControl<TabControl>("MainTabControl");
            
            Random r = new Random();
            _tabControl.Background = new SolidColorBrush(Color.FromRgb((byte)r.Next(1, 255), 
                (byte)r.Next(1, 255), (byte)r.Next(1, 233)));

            SubscribeEvents();
        }

        #region Methods

        public object GetContent()
        {
            return _tabControl;
        }
        
        void SplitVertical()
        {
            if (IsSplitted) return;
            
            _mainGrid.Children.Remove(_tabControl);
            _mainGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star)); // for child 1
            _mainGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto)); // for splitter
            _mainGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star)); // for child 2

            Child1 = new LayoutPanel();
            Child1.Parent = this;
            Child1.Closed += ChildOnClosed;
            Grid.SetRow(Child1,0);
            Grid.SetColumn(Child1,0);
            _mainGrid.Children.Add(Child1);

            _gridSplitter = new GridSplitter() {HorizontalAlignment = HorizontalAlignment.Stretch, Height = 2, Background = Brushes.Aqua};
            Grid.SetRow(_gridSplitter,1);
            Grid.SetColumn(_gridSplitter,0);
            _mainGrid.Children.Add(_gridSplitter);
            
            Child2 = new LayoutPanel();
            Child2.Parent = this;
            Child2.Closed += ChildOnClosed;
            Grid.SetRow(Child2,2);
            Grid.SetColumn(Child2,0);
            _mainGrid.Children.Add(Child2);

            Orientation = EOrientation.Vertical;
            IsSplitted = true;
        }

        void SplitHorizontal()
        {
            if (IsSplitted) return;
            
            _mainGrid.Children.Remove(_tabControl);
            _mainGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star)); // for child 1
            _mainGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto)); // for splitter
            _mainGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star)); // for child 2
            
            Child1 = new LayoutPanel();
            Child1.Parent = this;
            Child1.Closed += ChildOnClosed;
            Grid.SetRow(Child1,0);
            Grid.SetColumn(Child1,0);
            _mainGrid.Children.Add(Child1);

            _gridSplitter = new GridSplitter() {VerticalAlignment = VerticalAlignment.Stretch, Width = 2, Background = Brushes.Aqua};
            Grid.SetRow(_gridSplitter,0);
            Grid.SetColumn(_gridSplitter,1);
            _mainGrid.Children.Add(_gridSplitter);
            
            Child2 = new LayoutPanel();
            Child2.Parent = this;
            Child2.Closed += ChildOnClosed;
            Grid.SetRow(Child2,0);
            Grid.SetColumn(Child2,2);
            _mainGrid.Children.Add(Child2);

            Orientation = EOrientation.Horizontal;
            IsSplitted = true;
        }

        void ChildOnClosed(object sender)
        {
            _mainGrid.RowDefinitions.Clear();
            _mainGrid.ColumnDefinitions.Clear();
            Orientation = EOrientation.None;
            IsSplitted = false;
            
            _mainGrid.Children.Clear();
            _mainGrid.Children.Add(_tabControl);
        }

        void Close()
        {
            Closed?.Invoke(this);
        }

        void SubscribeEvents()
        {
            this.AddHandler(PointerReleasedEvent, MouseDownHandler, handledEventsToo: true);
        }

        private void MouseDownHandler(object? sender, PointerReleasedEventArgs e)
        {
            System.Diagnostics.Debug.Write("Mouse pressed: ");
            switch (e.InitialPressMouseButton)
            {
                case MouseButton.Left: 
                    System.Diagnostics.Debug.WriteLine("LEFT");
                    this.SplitHorizontal(); 
                    break;
                case MouseButton.Right: 
                    System.Diagnostics.Debug.WriteLine("RIGHT");
                    this.SplitVertical(); 
                    break;
                case MouseButton.Middle: 
                    System.Diagnostics.Debug.WriteLine("MIDDLE");
                    this.Close();
                    break;
            }
        }


        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        #endregion
    }
}