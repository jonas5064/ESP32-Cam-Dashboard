using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace IPCamera
{
    public partial class RecordFullScreen : Window
    {
        Player _video;
        Picture _picture;
        Records _record;

        public RecordFullScreen(Player video)
        {
            InitializeComponent();

            this._video = video;

            time_grid.Visibility = Visibility.Visible;
            buttons_grid.Visibility = Visibility.Visible;

            media_element.Source = new Uri(this._video.Video.Path);
            media_element.Margin = new Thickness(7, 7, 7, 0);
            media_element.LoadedBehavior = MediaState.Manual;
            media_element.ScrubbingEnabled = true;
            media_element.UnloadedBehavior = MediaState.Close;
            media_element.MediaOpened += (object sender, RoutedEventArgs e) =>
            {

            };
            media_element.Play();
            media_element.Pause();
            media_element.Position = TimeSpan.FromSeconds(0);


            // Threading to Update The Time Spam Label Show The Time Of Video
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += (object sender, EventArgs e) =>
            {
                if (media_element.Source != null)
                {
                    if (media_element.NaturalDuration.HasTimeSpan)
                    {
                        time_spam.Content = String.Format("{0} / {1}",
                                                    media_element.Position.ToString(@"mm\:ss"),
                                                    media_element.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
                    }
                }
            };
            timer.Start();


        }

        public RecordFullScreen(Picture picture, Records rec)
        {
            InitializeComponent();
            this._picture = picture;
            this._record = rec;
            picture_imfo_grid.Visibility = Visibility.Visible;
            name_pic.Content = this._picture.CamName;
            date_pic.Content = this._picture.Date;
            time_pic.Content = this._picture.Time;
            media_element.Source = new Uri(this._picture.Path);
        }

        ~RecordFullScreen()
        {
            this._video = null;
            this._picture = null;
            this._record = null;
        }

        // On Close Window
        protected override void OnClosed(EventArgs e)
        {
            if(this._video != null)
            {
                this._video.Fullscreen = false;
            }
            if(this._record != null)
            {
                this._record.Fullscreen = false;
            }            
            this.Close();
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            if (media_element.Source != null)
            {
                media_element.Play();
            }
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            if (media_element.Source != null)
            {
                media_element.Stop();
            }
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            if (media_element.Source != null)
            {
                media_element.Pause();
            }
        }

        private void Packward_Click(object sender, RoutedEventArgs e)
        {
            if (media_element.Source != null)
            {
                media_element.Position -= TimeSpan.FromMilliseconds(1000);
            }
        }

        private void Forward_Click(object sender, RoutedEventArgs e)
        {
            if (media_element.Source != null)
            {
                media_element.Position += TimeSpan.FromMilliseconds(1000);
            }
        }
    }
}
