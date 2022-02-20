using System.Windows.Controls;
using System.Windows.Media;
using System.IO;
using System;

namespace Sound_test.CustomControls
{
    /// <summary>
    /// Interaction logic for LibraryContent.xaml
    /// </summary>
    public partial class LibraryContent : UserControl
    {
        int refIndex = 0;

        public LibraryContent(string filename, string color, int row)
        {
            InitializeComponent();

            FileName.Text = filename;
            Background.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));

            refIndex = row;
        }

        private void PlayButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Sound_test.Content.media = File.ReadAllLines($"{Environment.CurrentDirectory}/Library/Library.txt")[refIndex];
            Sound_test.Content.Play();
        }
    }
}
