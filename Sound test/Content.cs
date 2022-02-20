using System;
using System.Windows.Controls;

namespace Sound_test
{
    internal class Content
    {
        public static TextBlock title;
        public static MediaElement videoElem;
        public static Slider volume;

        public static string media = "";

        public static void Play()
        {
            title.Text = media.Split('\\')[media.Split('\\').Length - 1].Split('.')[0];

            videoElem.LoadedBehavior = MediaState.Play;
            videoElem.Source = new Uri(media);

            volume.Value = videoElem.Volume;
        }
    }
}
