using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Media;
using Microsoft.Win32;
using System.Windows.Threading;

namespace Sound_test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool editing = false;
        bool loop = false;

        #region images
        ImageSource maximize   = new BitmapImage(new Uri($"{Environment.CurrentDirectory}/Assets/Maximize.png"));
        ImageSource unmaximize = new BitmapImage(new Uri($"{Environment.CurrentDirectory}/Assets/UnMaximize.png"));

        ImageSource loopOn  = new BitmapImage(new Uri($"{Environment.CurrentDirectory}/Assets/LoopOn.png"));
        ImageSource loopOff = new BitmapImage(new Uri($"{Environment.CurrentDirectory}/Assets/LoopOff.png"));

        ImageSource play  = new BitmapImage(new Uri($"{Environment.CurrentDirectory}/Assets/Play.png"));
        ImageSource pause = new BitmapImage(new Uri($"{Environment.CurrentDirectory}/Assets/Pause.png"));

        ImageSource top    = new BitmapImage(new Uri($"{Environment.CurrentDirectory}/Assets/TopMostTrue.png"));
        ImageSource notTop = new BitmapImage(new Uri($"{Environment.CurrentDirectory}/Assets/TopMostFalse.png"));
        #endregion

        DispatcherTimer hideTimer = new DispatcherTimer();
        DispatcherTimer showTimer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();

            DispatcherTimer timer = new DispatcherTimer();

            #region timer set up
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();

            hideTimer.Interval = TimeSpan.FromMilliseconds(10);
            hideTimer.Tick += Hide_Tick;

            showTimer.Interval = TimeSpan.FromMilliseconds(10);
            showTimer.Tick += Show_Tick;
            #endregion
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "MP3 files|*.mp3|MP4 files|*.mp4";

            if (dialog.ShowDialog() == true)
            {
                title.Text = dialog.FileName.Split('\\')[dialog.FileName.Split('\\').Length - 1].Split('.')[0];

                VideoElem.LoadedBehavior = MediaState.Play;
                VideoElem.Source = new Uri(dialog.FileName);

                VolumeSlider.Value = VideoElem.Volume;
            }
        }
        private void VideoElem_MediaOpened(object sender, RoutedEventArgs e)
        {
            PlayPauseImage_MouseLeftButtonDown(sender, null);
        }

        #region timer ticks
        void Timer_Tick(object sender, EventArgs e)
        {
            if (VideoElem.Source != null && VideoElem.NaturalDuration.HasTimeSpan)
            {
                ProgressSlider.Maximum = VideoElem.NaturalDuration.TimeSpan.TotalSeconds;
                ProgressSlider.Value = VideoElem.Position.TotalSeconds;
                Status.Text = String.Format("{0} / {1}", VideoElem.Position.ToString(@"mm\:ss"), VideoElem.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
            }
            else
            {
                Status.Text = "No file selected...";
            }
        }

        void Show_Tick(object sender, EventArgs e)
        {
            bool title = AnimateRow(TitleBar, 40, 2);
            bool controls = AnimateRow(Controls, 60, 3);

            if (title && controls)
                showTimer.Stop();
        }

        void Hide_Tick(object sender, EventArgs e)
        {
            bool title = AnimateRow(TitleBar, 0, -2);
            bool controls = AnimateRow(Controls, 0, -3);

            if (title && controls)
                hideTimer.Stop();
        }

        bool AnimateRow(RowDefinition row, int target, int modifier)
        {
            if (modifier < 0 && row.ActualHeight <= target)
            {
                row.Height = new GridLength(target, GridUnitType.Pixel);
                return true;
            }
            else if (modifier > 0 && row.ActualHeight >= target)
            {
                row.Height = new GridLength(target, GridUnitType.Pixel);
                return true;
            }

            row.Height = new GridLength(row.ActualHeight + modifier, GridUnitType.Pixel);
            return false;
        }
        #endregion

        #region Progress Slider
        private void ProgressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (VideoElem.Source != null && VideoElem.NaturalDuration.HasTimeSpan && editing)
            {
                VideoElem.Position = new TimeSpan(0,0,Convert.ToInt32(ProgressSlider.Value));

                ProgressSlider.Maximum = VideoElem.NaturalDuration.TimeSpan.TotalSeconds;
                ProgressSlider.Value = VideoElem.Position.TotalSeconds;
                Status.Text = String.Format("{0} / {1}", VideoElem.Position.ToString(@"mm\:ss"), VideoElem.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
            }
        }

        private void ProgressSlider_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            editing = true;
            VideoElem.Pause();
        }

        private void ProgressSlider_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            editing = false;
            VideoElem.Play();
        }
        #endregion

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
            } else if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
                StateImage.Source = unmaximize;
            }
        }

        private void Minimize_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            hideTimer.Stop();
            showTimer.Start();
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            showTimer.Stop();
            hideTimer.Start();
        }
        #endregion

        #region Media Control Events
        private void LoopImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            loop = loop ? false : true;
            LoopImage.Source = loop ? loopOn : loopOff;
        }

        private void VideoElem_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (loop)
            {
                VideoElem.Position = new TimeSpan(0, 0, 1);
                VideoElem.Play();
            }
        }

        private void PlayPauseImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            VideoElem.LoadedBehavior = MediaState.Manual;
            if (PlayPauseImage.Source == play && VideoElem.Source != null)
            {
                VideoElem.Play();
                PlayPauseImage.Source = pause;
            }
            else if (VideoElem.Source != null)
            {
                VideoElem.Pause();
                PlayPauseImage.Source = play;
            }
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            VideoElem.Volume = VolumeSlider.Value;
        }

        private void SettingsBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SettingsPanel.Visibility = SettingsPanel.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        private void TopMostbtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Topmost = TopMostbtn.Source == top ? false : true;
            TopMostbtn.Source = TopMostbtn.Source == top ? notTop : top;
        }
        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Sound_test.Content.title = title;
            Sound_test.Content.videoElem = VideoElem;
            Sound_test.Content.volume = VolumeSlider;
        }

        private void Library_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            new Library().Show();
        }
    }
}
