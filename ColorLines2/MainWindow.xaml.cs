using System.Windows;
using System.Windows.Input;


namespace ColorLines2
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ChangeWindowSize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
            }
            else
            {
                WindowState = WindowState.Normal;
            }
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void DragWindow(object sender, MouseButtonEventArgs e) => DragMove();
    }
}