namespace AvaloniaTestMVVM.ViewModels
{
    public class ContentViewModel : ViewModelBase
    {
        private static int i;
        
        public string Title { get; set; }
        
        public object Content { get; set; }

        public ContentViewModel()
        {
            this.Title = "Some View " + i++;
        }
            
        
    }
}