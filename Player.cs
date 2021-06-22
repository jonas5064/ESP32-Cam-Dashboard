using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace IPCamera
{
    public class Player
    {

        public Grid parrent;
        public Video video;
        public int column;
        public int row;
        public Boolean fullscreen = false;
        RecordFullScreen fullscreen_page;

        public int buttonsFontSize = 12;
        Label time_spam;
        MediaElement player;

        public Player(Grid parrent, Video video, int column, int row)
        {
            this.parrent = parrent;
            this.video = video;
            this.column = column;
            this.row = row;

            // Threading to Update The Time Spam Label Show The Time Of Video
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (object sender, EventArgs e) =>
            {
                if (this.player.Source != null)
                {
                    if (this.player.NaturalDuration.HasTimeSpan)
                    {
                        this.time_spam.Content = String.Format("{0} / {1}", 
                                                    this.player.Position.ToString(@"mm\:ss"), 
                                                    this.player.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
                    }
                }
            };
            timer.Start();
        }

        public void CreateVideo()
        {
            // Create Video Grid And Add it to Video_Grid
            Grid vid_Grid = new Grid();
            vid_Grid.Background = System.Windows.Media.Brushes.Gray;
            vid_Grid.Margin = new Thickness(3);
            Grid.SetRow(vid_Grid, this.row);
            Grid.SetColumn(vid_Grid, this.column);
            vid_Grid.RowDefinitions.Add(new RowDefinition());
            vid_Grid.RowDefinitions.Add(new RowDefinition());
            vid_Grid.RowDefinitions.Add(new RowDefinition());
            vid_Grid.RowDefinitions.Add(new RowDefinition());
            vid_Grid.RowDefinitions.Add(new RowDefinition());
            vid_Grid.RowDefinitions.Add(new RowDefinition());
            vid_Grid.RowDefinitions.Add(new RowDefinition());
            parrent.Children.Add(vid_Grid);

            // Add Title Panel
            StackPanel panel_title = new StackPanel();
            panel_title.Orientation = Orientation.Vertical;
            panel_title.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetRow(panel_title, 0);
            vid_Grid.Children.Add(panel_title);
            // Add Label
            Label name = new Label();
            name.Content = this.video.CamName;
            name.HorizontalAlignment = HorizontalAlignment.Center;
            name.Foreground = System.Windows.Media.Brushes.DarkRed;
            panel_title.Children.Add(name);
            // Add Label
            Label date = new Label();
            date.Content = this.video.Date;
            date.HorizontalAlignment = HorizontalAlignment.Center;
            date.Foreground = System.Windows.Media.Brushes.DarkRed;
            panel_title.Children.Add(date);
            // Add Label
            Label time = new Label();
            time.Content = this.video.Time;
            time.HorizontalAlignment = HorizontalAlignment.Center;
            time.Foreground = System.Windows.Media.Brushes.DarkRed;
            panel_title.Children.Add(time);

            // Video Player
            Console.WriteLine($"\n\nVideo {this.video.Path}\n");
            this.player = new MediaElement();
            this.player.Source = new Uri(this.video.Path);
            this.player.Height = 233;
            this.player.Margin = new Thickness(7, 7, 7, 0);
            this.player.LoadedBehavior = MediaState.Manual;
            this.player.ScrubbingEnabled = true;
            this.player.UnloadedBehavior = MediaState.Close;
            this.player.HorizontalAlignment = HorizontalAlignment.Center;
            this.player.MediaOpened += (object sender, RoutedEventArgs e) =>
            {

            };
            this.player.Play();
            this.player.Pause();
            this.player.Position = TimeSpan.FromSeconds(0);
            Grid.SetRow(player, 1);
            vid_Grid.Children.Add(player);

            // Add Label Fro Time Spam
            this.time_spam = new Label();
            this.time_spam.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetRow(this.time_spam, 2);
            vid_Grid.Children.Add(this.time_spam);

            
            // Add Panel Play Stop Pause
            StackPanel panel_Play_Stop_Pause = new StackPanel();
            panel_Play_Stop_Pause.Orientation = Orientation.Horizontal;
            panel_Play_Stop_Pause.HorizontalAlignment = HorizontalAlignment.Center;
            panel_Play_Stop_Pause.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetRow(panel_Play_Stop_Pause, 3);
            vid_Grid.Children.Add(panel_Play_Stop_Pause);
            // Add Button
            Button play = new Button();
            play.Content = "Play";
            play.HorizontalAlignment = HorizontalAlignment.Stretch;
            play.FontSize = this.buttonsFontSize;
            play.Padding = new Thickness(3, 0, 3, 0);
            play.Click += (object obj, RoutedEventArgs e) =>
            {
                if (this.player.Source != null)
                {
                    this.player.Play();
                }
            };
            panel_Play_Stop_Pause.Children.Add(play);
            // Add Button
            Button stop = new Button();
            stop.Content = "Stop";
            stop.HorizontalAlignment = HorizontalAlignment.Stretch;
            stop.FontSize = this.buttonsFontSize;
            stop.Padding = new Thickness(3, 0, 3, 0);
            stop.Click += (object obj, RoutedEventArgs e) =>
            {
                if (this.player.Source != null)
                {
                    this.player.Stop();
                }
            };
            panel_Play_Stop_Pause.Children.Add(stop);
            // Add Button
            Button pause = new Button();
            pause.Content = "Pause";
            pause.HorizontalAlignment = HorizontalAlignment.Stretch;
            pause.FontSize = this.buttonsFontSize;
            pause.Padding = new Thickness(3, 0, 3, 0);
            pause.Click += (object obj, RoutedEventArgs e) =>
            {
                if (this.player.Source != null)
                {
                    this.player.Pause();
                }
            };
            panel_Play_Stop_Pause.Children.Add(pause);

            // Add Panel Backward Forward Open Delete
            StackPanel panel_bakc_forw_open_del = new StackPanel();
            panel_bakc_forw_open_del.Orientation = Orientation.Horizontal;
            panel_bakc_forw_open_del.HorizontalAlignment = HorizontalAlignment.Center;
            panel_bakc_forw_open_del.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetRow(panel_bakc_forw_open_del, 4);
            vid_Grid.Children.Add(panel_bakc_forw_open_del);
            // Add Button
            RepeatButton backard = new RepeatButton();
            backard.Content = "Back";
            backard.Interval = 200;
            backard.FontSize = this.buttonsFontSize;
            backard.Padding = new Thickness(3, 0, 3, 0);
            backard.Click += (object obj, RoutedEventArgs e) =>
            {
                if (this.player.Source != null)
                {
                    this.player.Position -= TimeSpan.FromMilliseconds(1000);
                }
            };
            panel_bakc_forw_open_del.Children.Add(backard);
            // Add Button
            RepeatButton forward = new RepeatButton();
            forward.Content = "Forw";
            forward.Interval = 200;
            forward.FontSize = this.buttonsFontSize;
            forward.Padding = new Thickness(3, 0, 3, 0);
            forward.Click += (object obj, RoutedEventArgs e) =>
            {
                if (this.player.Source != null)
                {
                    this.player.Position += TimeSpan.FromMilliseconds(1000);
                }
            };
            panel_bakc_forw_open_del.Children.Add(forward);
            // Add Button
            Button open = new Button();
            open.Content = "Open";
            open.FontSize = this.buttonsFontSize;
            open.Padding = new Thickness(3, 0, 3, 0);
            open.Click += (object obj, RoutedEventArgs e) =>
            {
                if (!this.fullscreen)
                {
                    this.fullscreen = true;
                    this.fullscreen_page = new RecordFullScreen(this);
                    fullscreen_page.Show();
                }
                else
                {
                    this.fullscreen_page.Activate();
                }

            };
            panel_bakc_forw_open_del.Children.Add(open);
            // Add Button
            Button delete = new Button();
            delete.Content = "Del";
            delete.FontSize = this.buttonsFontSize;
            delete.Padding = new Thickness(3, 0, 3, 0);
            delete.Click += (object obj, RoutedEventArgs e) =>
            {
                if (MessageBox.Show("Are you sure?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    File.Delete(this.video.Path);
                }
            };
            panel_bakc_forw_open_del.Children.Add(delete);
        }




    }
}
