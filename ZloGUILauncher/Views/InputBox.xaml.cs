using System.Windows;

namespace ZloGUILauncher.Views
{
    /// <summary>
    /// Interaction logic for InputBox.xaml
    /// </summary>
    public partial class InputBox
    {
        public InputBox(string request, string defaultAnswer = "")
        {
            InitializeComponent();
            RequestText.Text = request;
            ResultText.Text = defaultAnswer;
        }
        
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
        public string OutPut => ResultText.Text;
    }
}
