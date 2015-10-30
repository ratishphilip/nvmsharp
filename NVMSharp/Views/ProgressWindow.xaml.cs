using WPFSpark;

namespace NVMSharp.Views
{
    /// <summary>
    /// Interaction logic for ProgressWindow.xaml
    /// </summary>
    public partial class ProgressWindow : SparkWindow
    {
        public ProgressWindow()
        {
            InitializeComponent();
        }

        public void SetMessage(string message)
        {
            MsgTB.Text = message;
        }
    }
}
