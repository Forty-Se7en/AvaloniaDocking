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
using ReactiveUI;

namespace AvaloniaTestMVVM.Docking.View
{
    public class LayoutPanel : UserControl, ILayoutPanel
    {
        private static int _index;

        #region Events

        public event Action<object> Closed;
        
        #endregion
        
        #region Fields
        
        private readonly Grid _mainGrid;
        private Grid _contentGrid;
        private TabControl _tabControl;
        //private TabItem _tabItem;
        private GridSplitter _gridSplitter;
        private Label _label; 
        
        private ContentViewModel _content;

        #endregion
        
        #region Properties
        
        /// <summary> Родительский контрол </summary>
        private LayoutPanel Parent { get; set; }
        
        /// <summary> Дочерний контрол </summary>
        private LayoutPanel Child1 { get; set; }
        
        /// <summary> Дочерний контрол </summary>
        private LayoutPanel Child2 { get; set; }
        
        /// <summary> Состояние контрола </summary>
        public bool IsSplitted{ get; private set; }
        
        /// <summary> Ориентация </summary>
        public EOrientation Orientation { get; private set; }
        
        #endregion

        public LayoutPanel(object content)
        {
            InitializeComponent();
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
                Content = "Layout " + _index++, 
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
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
        }

        public LayoutPanel()
        {
            InitializeComponent();
        }


        #region Methods

        /// <summary> Добавляет контент </summary>
        public void AddContent(ContentViewModel content, EPosition position)
        {
            switch (position)
            {
                default: throw new Exception("Не задано положение для закрепления элемента");
                case EPosition.Center:
                    InsertContent(content);
                    break;
                case EPosition.Left:
                case EPosition.Right:
                    SplitHorizontal(content, position);
                    break;
                case EPosition.Top:
                case EPosition.Bottom:
                    SplitVertical(content, position);
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



        public void AddContextMenu()
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
                    { Header = "Удалить", Command = ReactiveCommand.Create(Close) }
            };

            menu.Items = items;
            //_tabControl.ContextMenu = menu;
            _mainGrid.ContextMenu = menu;

        }

        public void Flow()
        {
            
        }
        
        void SplitVertical(ContentViewModel content, EPosition position)
        {
            if (IsSplitted) return;
            if (position != EPosition.Bottom && position != EPosition.Top) return;
            
            _mainGrid.Children.Remove(_contentGrid);
            _mainGrid.Children.Clear();
            // _contentGrid.Children.Remove(_tabControl);
            // _contentGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star)); // for child 1
            // _contentGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto)); // for splitter
            // _contentGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star)); // for child 2

            LayoutPanel topChild = null, bottomChild = null;

            if (position == EPosition.Top)
            {
                topChild = new LayoutPanel(content);
                bottomChild = new LayoutPanel(_contentGrid);
            }
            else if (position == EPosition.Bottom)
            {
                bottomChild = new LayoutPanel(content);
                topChild = new LayoutPanel(_contentGrid);
            }
            
            Child1 = topChild;
            Child1.Parent = this;
            Child1.Closed += ChildOnClosed;
            Grid.SetRow(Child1,0);
            Grid.SetColumn(Child1,0);
            //_contentGrid.Children.Add(Child1);

            _gridSplitter = new GridSplitter() {HorizontalAlignment = HorizontalAlignment.Stretch, Height = 2, Background = Brushes.Aqua};
            Grid.SetRow(_gridSplitter,1);
            Grid.SetColumn(_gridSplitter,0);
            //_contentGrid.Children.Add(_gridSplitter);

            Child2 = bottomChild;
            Child2.Parent = this;
            Child2.Closed += ChildOnClosed;
            Grid.SetRow(Child2,2);
            Grid.SetColumn(Child2,0);
            //_contentGrid.Children.Add(Child2);
            
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
        }

        void SplitHorizontal(ContentViewModel content, EPosition position)
        {
            if (IsSplitted) return;
            if (position != EPosition.Left && position != EPosition.Right) return;

            _mainGrid.Children.Remove(_contentGrid);
            _mainGrid.Children.Clear();
            // if (_contentGrid.Parent != null)
            // {
            //     (_contentGrid.Parent as Grid).Children.Remove(_contentGrid);
            // }
            
            // _contentGrid.Children.Remove(_tabControl);
            // _contentGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star)); // for child 1
            // _contentGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto)); // for splitter
            // _contentGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star)); // for child 2
            
            LayoutPanel leftChild = null, rightChild = null;

            if (position == EPosition.Left)
            {
                leftChild = new LayoutPanel(content);
                rightChild = new LayoutPanel(_contentGrid);
            }
            else if (position == EPosition.Right)
            {
                rightChild = new LayoutPanel(content);
                leftChild = new LayoutPanel(_contentGrid);
            }
            
            Child1 = leftChild;
            Child1.Parent = this;
            Child1.Closed += ChildOnClosed;
            Grid.SetRow(Child1,0);
            Grid.SetColumn(Child1,0);
            //_contentGrid.Children.Add(Child1);

            _gridSplitter = new GridSplitter() {VerticalAlignment = VerticalAlignment.Stretch, Width = 2, Background = Brushes.Aqua};
            Grid.SetRow(_gridSplitter,0);
            Grid.SetColumn(_gridSplitter,1);
            //_contentGrid.Children.Add(_gridSplitter);
            
            Child2 = rightChild;
            Child2.Parent = this;
            Child2.Closed += ChildOnClosed;
            Grid.SetRow(Child2,0);
            Grid.SetColumn(Child2,2);
            //_contentGrid.Children.Add(Child2);

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
        }

        void ChildOnClosed(object sender)
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
            

            var child = (LayoutPanel)sender;
            Child1.Closed -= this.ChildOnClosed; 
            Child2.Closed -= this.ChildOnClosed;
            
            //TabControl content = null;
            Grid content2 = null;
            
            if (child == Child1)
            {
                Child2.Close();
                //content = Child2.GetContent();
                content2 = Child2.GetContentGrid();
            }
            else if (child == Child2)
            {
                Child1.Close();
                //content = Child1.GetContent();
                content2 = Child1.GetContentGrid();
                
            }

            _contentGrid = content2;
            _mainGrid.Children.Add(_contentGrid);
            _mainGrid.Children.Add(_label);
            
            AddContextMenu();
            _mainGrid.ContextMenu.IsEnabled = true;
            //this.AddTabControl(content);
        }

        void Close()
        {
            //_mainGrid.Children.Remove(_tabControl);
            //_mainGrid.Children.Clear();
            _mainGrid.Children.Remove(_contentGrid);
            _mainGrid.Children.Clear();
            
            Closed?.Invoke(this);
        }

        public Grid GetContentGrid()
        {
            return _contentGrid;
        }
        

        void SubscribeEvents()
        {
            this.AddHandler(PointerReleasedEvent, MouseDownHandler, handledEventsToo: true);
        }

        
        private void MouseDownHandler(object? sender, PointerReleasedEventArgs e)
        {
            // System.Diagnostics.Debug.Write("Mouse pressed: ");
            // switch (e.InitialPressMouseButton)
            // {
            //     case MouseButton.Left: 
            //         System.Diagnostics.Debug.WriteLine("LEFT");
            //         this.SplitHorizontal(); 
            //         break;
            //     case MouseButton.Right: 
            //         System.Diagnostics.Debug.WriteLine("RIGHT");
            //         this.SplitVertical(); 
            //         break;
            //     case MouseButton.Middle: 
            //         System.Diagnostics.Debug.WriteLine("MIDDLE");
            //         this.Close();
            //         break;
            // }
        }


        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        #endregion
    }
}