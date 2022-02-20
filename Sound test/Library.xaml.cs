using System;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;

namespace Sound_test
{
    /// <summary>
    /// Interaction logic for Library.xaml
    /// </summary>
    public partial class Library : Window
    {
        ImageSource maximize = new BitmapImage(new Uri($"{Environment.CurrentDirectory}/Assets/Maximize.png"));
        ImageSource unmaximize = new BitmapImage(new Uri($"{Environment.CurrentDirectory}/Assets/UnMaximize.png"));

        public Library()
        {
            InitializeComponent();
        }

        private void SearchDirectories(string path)
        {
            Dispatcher.Invoke(() => { IndexTxt.Text = path; });
            try
            {
                File.AppendAllText($"{Environment.CurrentDirectory}/Library/Library.txt", String.Join('\n', Directory.GetFiles(path, "*.mp4")));
                File.AppendAllText($"{Environment.CurrentDirectory}/Library/Library.txt", String.Join('\n', Directory.GetFiles(path, "*.mp3")));

                foreach (string dir in Directory.GetDirectories(path))
                {
                    SearchDirectories(dir);
                }
            }
            catch { }
        }

        private void BuildInterface()
        {
            string[] files = File.ReadAllLines($"{Environment.CurrentDirectory}/Library/Library.txt");
            for (int i = 0; i < files.Length; i++)
            {
                Dispatcher.Invoke(() => { ContentStack.Children.Add(new CustomControls.LibraryContent(files[i].Split('\\')[^1].Split('.')[0], i % 2 == 0 ? "#FF2D2D30" : "#FF333337", i)); });
            }
        }

        private void IndexFiles_Click(object sender, RoutedEventArgs e)
        {
            if (File.ReadAllLines($"{Environment.CurrentDirectory}/Library/Library.txt").Length > 0)
            {
                MessageBoxResult result = MessageBox.Show("Would you like to wipe the current library?", "[ALERT] - Library Update", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                    File.WriteAllText($"{Environment.CurrentDirectory}/Library/Library.txt", "");
                else if (result == MessageBoxResult.Cancel)
                    return;
            }

            Task.Factory.StartNew(() =>
            {
                SearchDirectories("C:/");
                BuildInterface();
            });
        }

        #region Window Controls
        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void WindowButton_MouseEnter(object sender, MouseEventArgs e)
        {
            Grid grid = sender as Grid;
            grid.Children.OfType<Rectangle>().FirstOrDefault().Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF48484B"));
        }

        private void WindowButton_MouseLeave(object sender, MouseEventArgs e)
        {
            Grid grid = sender as Grid;
            grid.Children.OfType<Rectangle>().FirstOrDefault().Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0048484B"));
        }

        private void Exit_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void MaxState_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                StateImage.Source = maximize;
            }
            else if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
                StateImage.Source = unmaximize;
            }
        }

        private void Minimize_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        #endregion
    }
}
