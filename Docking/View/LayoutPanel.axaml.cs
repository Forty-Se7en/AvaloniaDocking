using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using AvaloniaTestMVVM.Docking.Model;
using AvaloniaTestMVVM.ViewModels;
using AvaloniaTestMVVM.Views;
using ReactiveUI;

namespace AvaloniaTestMVVM.Docking.View
{
    public class LayoutPanel : UserControl, ILayoutPanel
    {
        #region Events
        public event Action<LayoutPanel> CloseRequest;

        public event Action<LayoutPanel, LayoutPanel> SwapRequest;

        public event Action<LayoutPanel> FlowRequest; 

        #endregion
        
        #region Fields
        
        private readonly Grid _mainGrid;
        private Grid _contentGrid;
        private TabControl _tabControl;
        //private TabItem _tabItem;
        private GridSplitter _gridSplitter;
        private Label _label;
        //private ContentViewModel _content;
        private string _key;
        private LocationControl _locationControl;

        private ContentViewModel _content;
        
        #endregion
        
        #region Properties
        
        ///// <summary> Родительский контрол </summary>
        //private LayoutPanel Parent { get; set; }
        
        /// <summary> Дочерний контрол </summary>
        private LayoutPanel Child1 { get; set; }
        
        /// <summary> Дочерний контрол </summary>
        private LayoutPanel Child2 { get; set; }
        
        /// <summary> Состояние контрола </summary>
        bool IsSplitted{ get; set; }
        
        /// <summary> Ориентация </summary>
        EOrientation Orientation { get; set; }
        
        #endregion

        #region Ctors
        
        public LayoutPanel(object content) : this()
        {
            _key = "Layout " + _index++;
            //_mainGrid = this.FindControl<Grid>("MainGrid");
            _mainGrid = new Grid();
            this.Content = _mainGrid;

            if (content is Grid contentGrid)
            {
                _contentGrid = contentGrid;
                _tabControl = (TabControl)_contentGrid.Children.First(c => c.Name == "tabControl");
            }
            else
            {
                _contentGrid = new Grid();
            }
            _mainGrid.Children.Add(_contentGrid);
            _label = new Label()
            {
                Content = _key, 
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Background = Brushes.White
            };
            _mainGrid.Children.Add(_label);

            /*if (content is TabControl tabControl)
            {
                AddTabControl(tabControl);
            }*/
            if (content is ContentViewModel contentViewModel)
            {
                AddTabControl();
                AddContent(contentViewModel, EPosition.Center);
            }

            AddContextMenu();
            AddEvents();
        }

        public LayoutPanel()
        {
            InitializeComponent();
        }

        #endregion
        
        #region Methods

        /// <summary> Добавляет контент </summary>
        void AddContent(ContentViewModel content, EPosition position)
        {
            switch (position)
            {
                default: throw new Exception("Не задано положение для закрепления элемента");
                case EPosition.Center:
                    InsertContent(content);
                    break;
                case EPosition.Left:
                case EPosition.Right:
                    SplitHorizontal(new LayoutPanel(content), position);
                    break;
                case EPosition.Top:
                case EPosition.Bottom:
                    SplitVertical(new LayoutPanel(content), position);
                    break;
            }
        }

        void AddTabControl(TabControl tabControl = null)
        {
            if (tabControl == null)
                tabControl = new TabControl() {Name = "tabControl"};
            _tabControl = tabControl;
            _contentGrid.Children.Add(_tabControl);
        }
        
        void InsertContent(ContentViewModel content)
        {
            var tabItem = new TabItem();
            tabItem.Header = content.Title;
            tabItem.Content = content.Content;

            var items = _tabControl.Items.Cast<object>().ToList();
            items.Add(tabItem);

            _tabControl.Items = items;

        }

        void AddContextMenu()
        {
            ContextMenu menu = new ContextMenu();
            var items = new[]
            {
                new MenuItem()
                    { Header = "Отсоединить", Command = ReactiveCommand.Create(Flow) },
                new MenuItem()
                {
                    Header = "Добавить внутрь",
                    Command = ReactiveCommand.Create(
                        () => { this.AddContent(CreateRandomContent(), EPosition.Center); })
                },
                new MenuItem()
                {
                    Header = "Добавить слева", 
                    Command = ReactiveCommand.Create(
                        () => { this.AddContent(CreateRandomContent(), EPosition.Left); })
                },
                new MenuItem()
                {
                    Header = "Добавить справа", 
                    Command = ReactiveCommand.Create(
                        () => { this.AddContent(CreateRandomContent(), EPosition.Right); })
                },
                new MenuItem()
                {
                    Header = "Добавить сверху", 
                    Command = ReactiveCommand.Create(
                        () => { this.AddContent(CreateRandomContent(), EPosition.Top); })
                },
                new MenuItem()
                {
                    Header = "Добавить снизу", 
                    Command = ReactiveCommand.Create(
                        () => { this.AddContent(CreateRandomContent(), EPosition.Bottom); })
                },
                new MenuItem()
                    { Header = "Удалить", Command = ReactiveCommand.Create(RemoveActiveContent) }
            };

            menu.Items = items;
            //_tabControl.ContextMenu = menu;
            _mainGrid.ContextMenu = menu;

        }
       
        void SplitVertical(LayoutPanel panel, EPosition position)
        {
            if (IsSplitted) return;
            if (position != EPosition.Bottom && position != EPosition.Top) return;
            
            _mainGrid.Children.Remove(_contentGrid);
            _mainGrid.Children.Clear();

            LayoutPanel topChild = null, bottomChild = null;

            if (position == EPosition.Top)
            {
                // topChild = new LayoutPanel(content);
                topChild = panel;
                bottomChild = new LayoutPanel(_contentGrid);
            }
            else if (position == EPosition.Bottom)
            {
                // bottomChild = new LayoutPanel(content);
                bottomChild = panel;
                topChild = new LayoutPanel(_contentGrid);
            }
            
            Child1 = topChild;
            //Child1.Parent = this;
            // Child1.Closed += ChildOnClosed;
            Child1.CloseRequest += this.OnChildCloseRequest;
            Child1.SwapRequest += this.OnChildSwapRequest;
            Child1.FlowRequest += this.OnChildFlowRequest;
            Grid.SetRow(Child1,0);
            Grid.SetColumn(Child1,0);

            _gridSplitter = new GridSplitter() {HorizontalAlignment = HorizontalAlignment.Stretch, Height = 2, Background = Brushes.Aqua};
            Grid.SetRow(_gridSplitter,1);
            Grid.SetColumn(_gridSplitter,0);

            Child2 = bottomChild;
            //Child2.Parent = this;
            //Child2.Closed += ChildOnClosed;
            Child2.CloseRequest += this.OnChildCloseRequest;
            Child2.SwapRequest += this.OnChildSwapRequest;
            Child2.FlowRequest += this.OnChildFlowRequest;
            Grid.SetRow(Child2,2);
            Grid.SetColumn(Child2,0);
            
            _contentGrid = new Grid();
            _contentGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star)); // for child 1
            _contentGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto)); // for splitter
            _contentGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star)); // for child 2
            _contentGrid.Children.Add(Child1);
            _contentGrid.Children.Add(_gridSplitter);
            _contentGrid.Children.Add(Child2);
            
            _mainGrid.Children.Add(_contentGrid);
            _mainGrid.Children.Add(_label);

            Orientation = EOrientation.Vertical;
            IsSplitted = true;
            _mainGrid.ContextMenu.IsEnabled = false;
            this.RemoveEvents();
        }

        void SplitHorizontal(LayoutPanel panel, EPosition position)
        {
            if (IsSplitted) return;
            if (position != EPosition.Left && position != EPosition.Right) return;

            _mainGrid.Children.Remove(_contentGrid);
            _mainGrid.Children.Clear();
            
            LayoutPanel leftChild = null, rightChild = null;

            if (position == EPosition.Left)
            {
                // leftChild = new LayoutPanel(content);
                leftChild = panel;
                rightChild = new LayoutPanel(_contentGrid);
            }
            else if (position == EPosition.Right)
            {
                // rightChild = new LayoutPanel(content);
                rightChild = panel;
                leftChild = new LayoutPanel(_contentGrid);
            }
            
            Child1 = leftChild;
            //Child1.Parent = this;
            //Child1.Closed += ChildOnClosed;
            Child1.CloseRequest += this.OnChildCloseRequest;
            Child1.SwapRequest += this.OnChildSwapRequest;
            Child1.FlowRequest += this.OnChildFlowRequest;
            Grid.SetRow(Child1,0);
            Grid.SetColumn(Child1,0);

            _gridSplitter = new GridSplitter() {VerticalAlignment = VerticalAlignment.Stretch, Width = 2, Background = Brushes.Aqua};
            Grid.SetRow(_gridSplitter,0);
            Grid.SetColumn(_gridSplitter,1);
            
            Child2 = rightChild;
            //Child2.Parent = this;
            //Child2.Closed += ChildOnClosed;
            Child2.CloseRequest += this.OnChildCloseRequest;
            Child2.SwapRequest += this.OnChildSwapRequest;
            Child2.FlowRequest += this.OnChildFlowRequest;
            Grid.SetRow(Child2,0);
            Grid.SetColumn(Child2,2);

            _contentGrid = new Grid();
            _contentGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star)); // for child 1
            _contentGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto)); // for splitter
            _contentGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star)); // for child 2
            _contentGrid.Children.Add(Child1);
            _contentGrid.Children.Add(_gridSplitter);
            _contentGrid.Children.Add(Child2);
            
            _mainGrid.Children.Add(_contentGrid);
            _mainGrid.Children.Add(_label);
            
            Orientation = EOrientation.Horizontal;
            IsSplitted = true;
            _mainGrid.ContextMenu.IsEnabled = false;
            this.RemoveEvents();
        }

        public void RemoveActiveContent()
        {
            if (_tabControl.SelectedItem != null)
            {
                var tabItem = (TabItem)_tabControl.SelectedItem;
                var items = _tabControl.Items.Cast<object>().ToList();
                items.Remove(tabItem);
                _tabControl.Items = items;

                if (items.Count == 0)
                {
                    Close();
                }
            }
        }

        void Close()
        {
            CloseRequest.Invoke(this);
        }
        
        void Flow()
        {
            this.FlowRequest?.Invoke(this);
        }

        void AddEvents()
        {
            this.AddHandler(PointerReleasedEvent, MouseUpHandler, handledEventsToo: true);
            this.AddHandler(PointerPressedEvent, MouseDownHandler, handledEventsToo: true);
            this.AddHandler(PointerLeaveEvent, MouseLeaveHandler, handledEventsToo: true);
            this.AddHandler(PointerEnterEvent, MouseEnterHandler, handledEventsToo: true);
            //this.AddHandler(PointerMovedEvent, MouseMovedHandler, handledEventsToo: true);
        }
        
        void RemoveEvents()
        {
            this.RemoveHandler(PointerReleasedEvent, MouseUpHandler);
            this.RemoveHandler(PointerPressedEvent, MouseDownHandler);
            this.RemoveHandler(PointerLeaveEvent, MouseLeaveHandler);
            this.RemoveHandler(PointerEnterEvent, MouseEnterHandler);
            //this.RemoveHandler(PointerMovedEvent, MouseMovedHandler);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        void DragDrop(LayoutPanel source, LayoutPanel target, ContentViewModel content)
        {
            source.RemoveActiveContent();
            target.AddContent(content, EPosition.Right);
        }
        
        #endregion
        
        #region Handlers
        
        void OnChildCloseRequest(LayoutPanel sender)
        {
            _contentGrid.Children.Remove(Child1);
            _contentGrid.Children.Remove(Child2);
            _contentGrid.Children.Clear();
            _contentGrid.RowDefinitions.Clear();
            _contentGrid.ColumnDefinitions.Clear();
            
            _mainGrid.Children.Remove(_contentGrid);
            _mainGrid.Children.Clear();
            
            Orientation = EOrientation.None;
            IsSplitted = false;

            Child1.CloseRequest -= this.OnChildCloseRequest; 
            Child2.CloseRequest -= this.OnChildCloseRequest;
            
            Child1.SwapRequest -= this.OnChildSwapRequest; 
            Child2.SwapRequest -= this.OnChildSwapRequest;
            
            Child1.FlowRequest -= this.OnChildFlowRequest;
            Child2.FlowRequest -= this.OnChildFlowRequest;
            
            if (sender == Child1)
            {
                SwapRequest?.Invoke(this, Child2);
            }
            else if (sender == Child2)
            {
                SwapRequest?.Invoke(this, Child1);
            }
        }

        void OnChildSwapRequest(LayoutPanel sender, LayoutPanel newPanel)
        {
            LayoutPanel panelToSwap = null;
            if (sender == Child1)
            {
                panelToSwap = Child1;
                Child1 = newPanel;
            }

            if (sender == Child2)
            {
                panelToSwap = Child2;
                Child2 = newPanel;
            }

            panelToSwap.CloseRequest -= this.OnChildCloseRequest;
            panelToSwap.SwapRequest -= this.OnChildSwapRequest;
            panelToSwap.FlowRequest -= this.OnChildFlowRequest;
            
            newPanel.CloseRequest += this.OnChildCloseRequest;
            newPanel.SwapRequest += this.OnChildSwapRequest;
            newPanel.FlowRequest += this.OnChildFlowRequest;
            
            Grid.SetRow(newPanel, Grid.GetRow(panelToSwap));
            Grid.SetColumn(newPanel, Grid.GetColumn(panelToSwap));
            _contentGrid.Children.Remove(panelToSwap);
            
            _contentGrid.Children.Add(newPanel);

        }

        void OnChildFlowRequest(LayoutPanel sender)
        {
            OnChildCloseRequest(sender);
            new FloatingWindow(sender).Show();
        }

        private void MouseUpHandler(object? sender, PointerReleasedEventArgs e)
        {
            //System.Diagnostics.Debug.Write("Mouse pressed: ");
            switch (e.InitialPressMouseButton)
            {
                case MouseButton.Left:
                    if (DragData.DragSource != this)
                    {
                        DragData.DragTarget = this;
                        DragDrop(DragData.DragSource, DragData.DragTarget, DragData.DragContent);
                    }
                    else
                    {
                        DragData.DragSource = null;
                    }
                        
                    System.Diagnostics.Debug.WriteLine($"Mouse up on {this._key}");
                    //this.SplitHorizontal(); 
                    break;
                case MouseButton.Right: 
                    //System.Diagnostics.Debug.WriteLine("RIGHT");
                    //this.SplitVertical(); 
                    break;
                case MouseButton.Middle: 
                    //System.Diagnostics.Debug.WriteLine("MIDDLE");
                    //this.Close();
                    break;
            }
        }
        
        private void MouseDownHandler(object? sender, PointerPressedEventArgs e)
        {
            var props = e.GetCurrentPoint(this).Properties;
            
            if (props.IsLeftButtonPressed)
            {
                if (_tabControl.SelectedItem != null)
                {
                    var tabItem = (TabItem)_tabControl.SelectedItem;
                    ContentViewModel content = new ContentViewModel();
                    content.Content = tabItem.Content;
                    content.Title = tabItem.Header as string;
                    DragData.DragSource = this;
                    DragData.DragContent = content;
                }
                e.Pointer.Capture(null);
                //System.Diagnostics.Debug.WriteLine($"Mouse down on {this._key}");
            }
        }
        
        private void MouseLeaveHandler(object? sender, PointerEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine($"Mouse leave on {this._key}");
            if (_locationControl != null)
            {
                _mainGrid.Children.Remove(_locationControl);
            }
        }
        
        private void MouseEnterHandler(object? sender, PointerEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine($"Mouse enter on {this._key}");
            if (_locationControl == null)
            {
                _locationControl = new LocationControl();
            }
            _mainGrid.Children.Add(_locationControl);
        }
        
        private void MouseMovedHandler(object? sender, PointerEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine($"Mouse move on {this._key}");
        }
        
        #endregion

        #region Static

        private static int _index;

        public static ContentViewModel CreateRandomContent()
        {
            Random r = new Random();
            var content = new ContentViewModel()
            {
                Content = new Panel()
                {
                    Background = new SolidColorBrush(Color.FromRgb((byte)r.Next(1, 255),
                        (byte)r.Next(1, 255), (byte)r.Next(1, 233)))
                }
            };
            return content;
        }

        #endregion
    }
}