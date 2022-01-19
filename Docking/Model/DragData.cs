using AvaloniaTestMVVM.Docking.View;
using AvaloniaTestMVVM.ViewModels;

namespace AvaloniaTestMVVM.Docking.Model
{
    public static class DragData
    {
        public static LayoutPanel DragSource { get; set; }

        public static LayoutPanel DragTarget { get; set; }

        public static ContentViewModel DragContent { get; set; }
    }
}