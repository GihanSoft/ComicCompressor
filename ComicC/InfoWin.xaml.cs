namespace ComicC
{
    /// <summary>
    /// Interaction logic for InfoWin.xaml
    /// </summary>
    public partial class InfoWin
    {
        public InfoWin()
        {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.AbsoluteUri);
        }
    }
}
