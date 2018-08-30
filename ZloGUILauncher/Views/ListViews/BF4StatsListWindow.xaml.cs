namespace ZloGUILauncher.Views
{
    /// <summary>
    /// Interaction logic for BF4StatsListWindow.xaml
    /// </summary>
    public partial class BF4StatsListWindow 
    {
        public BF4StatsListWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender , System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
