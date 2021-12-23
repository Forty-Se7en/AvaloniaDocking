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
        #region Events

        public event Action<object> Closed;
        
        #endregion
        
        #region Fields
        
        private Grid _mainGrid;
        private TabControl _tabControl;
        private TabItem _tabItem;
        private GridSplitter _gridSplitter;
        
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
            _mainGrid = this.FindControl<Grid>("MainGrid");
            if (content is TabControl tabControl)
            {
                AddTabControl(tabControl);
            }
            else if (content is ContentViewModel contentViewModel)
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
                tabControl = new TabControl();
            _tabControl = tabControl;
            _mainGrid.Children.Add(_tabControl);
        }
        
        void InsertContent(ContentViewModel content)
        {
            _tabItem = new TabItem();
            _tabItem.Header = content.Title;
            _tabItem.Content = content.Content;

            var items = _tabControl.Items.Cast<object>().ToList();
            items.Add(_tabItem);

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
            
            _mainGrid.Children.Remove(_tabControl);
            _mainGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star)); // for child 1
            _mainGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto)); // for splitter
            _mainGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star)); // for child 2

            LayoutPanel topChild = null, bottomChild = null;

            if (position == EPosition.Top)
            {
                topChild = new LayoutPanel(content);
                bottomChild = new LayoutPanel(_tabControl);
            }
            else if (position == EPosition.Bottom)
            {
                bottomChild = new LayoutPanel(content);
                topChild = new LayoutPanel(_tabControl);
            }
            
            Child1 = topChild;
            Child1.Parent = this;
            Child1.Closed += ChildOnClosed;
            Grid.SetRow(Child1,0);
            Grid.SetColumn(Child1,0);
            _mainGrid.Children.Add(Child1);

            _gridSplitter = new GridSplitter() {HorizontalAlignment = HorizontalAlignment.Stretch, Height = 2, Background = Brushes.Aqua};
            Grid.SetRow(_gridSplitter,1);
            Grid.SetColumn(_gridSplitter,0);
            _mainGrid.Children.Add(_gridSplitter);

            Child2 = bottomChild;
            Child2.Parent = this;
            Child2.Closed += ChildOnClosed;
            Grid.SetRow(Child2,2);
            Grid.SetColumn(Child2,0);
            _mainGrid.Children.Add(Child2);

            Orientation = EOrientation.Vertical;
            IsSplitted = true;
            _mainGrid.ContextMenu.IsEnabled = false;
        }

        void SplitHorizontal(ContentViewModel content, EPosition position)
        {
            if (IsSplitted) return;
            if (position != EPosition.Left && position != EPosition.Right) return;
            
            _mainGrid.Children.Remove(_tabControl);
            _mainGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star)); // for child 1
            _mainGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto)); // for splitter
            _mainGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star)); // for child 2
            
            LayoutPanel leftChild = null, rightChild = null;

            if (position == EPosition.Left)
            {
                leftChild = new LayoutPanel(content);
                rightChild = new LayoutPanel(_tabControl);
            }
            else if (position == EPosition.Right)
            {
                rightChild = new LayoutPanel(content);
                leftChild = new LayoutPanel(_tabControl);
            }
            
            Child1 = leftChild;
            Child1.Parent = this;
            Child1.Closed += ChildOnClosed;
            Grid.SetRow(Child1,0);
            Grid.SetColumn(Child1,0);
            _mainGrid.Children.Add(Child1);

            _gridSplitter = new GridSplitter() {VerticalAlignment = VerticalAlignment.Stretch, Width = 2, Background = Brushes.Aqua};
            Grid.SetRow(_gridSplitter,0);
            Grid.SetColumn(_gridSplitter,1);
            _mainGrid.Children.Add(_gridSplitter);
            
            Child2 = rightChild;
            Child2.Parent = this;
            Child2.Closed += ChildOnClosed;
            Grid.SetRow(Child2,0);
            Grid.SetColumn(Child2,2);
            _mainGrid.Children.Add(Child2);

            Orientation = EOrientation.Horizontal;
            IsSplitted = true;
            _mainGrid.ContextMenu.IsEnabled = false;
        }

        void ChildOnClosed(object sender)
        {
            _mainGrid.Children.Remove(Child1);
            _mainGrid.Children.Remove(Child2);
            _mainGrid.Children.Clear();
            _mainGrid.RowDefinitions.Clear();
            _mainGrid.ColumnDefinitions.Clear();
            
            Orientation = EOrientation.None;
            IsSplitted = false;
            _mainGrid.ContextMenu.IsEnabled = true;

            var child = (LayoutPanel)sender;
            Child1.Closed -= this.ChildOnClosed;
            Child2.Closed -= this.ChildOnClosed;
            
            
            TabControl content = null;
            
            if (child == Child1)
            {
                content = Child2.GetContent();
                Child2.Close();
            }
            else if (child == Child2)
            {
                content = Child1.GetContent();
                Child1.Close();
            }
            
            
            this.AddTabControl(content);
        }

        void Close()
        {
            _mainGrid.Children.Remove(_tabControl);
            _mainGrid.Children.Clear();
            Closed?.Invoke(this);
        }

        public TabControl GetContent()
        {
            return _tabControl;
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