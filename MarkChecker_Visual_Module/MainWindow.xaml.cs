using MDM.Views.MarkChecker.Pages;
using System.Windows;
using System.Windows.Controls;



namespace MarkChecker_Visual_Module
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {

        public ucMarkCheckerMain MainPage => this.MarkCheckerMain;

        public MainWindow()
        {
            InitializeComponent();
        }
    }
}
