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

        public Grid parrent_g;
        public StackPanel parrent_s;
        public Video video;
        public Picture picture;
        public Records record;
        public int column;
        public int row;
        public Boolean fullscreen = false;
        RecordFullScreen fullscreen_page;

        public int buttonsFontSize = 12;
        Label time_spam;
        MediaElement player;
        

        public Player(Grid parrent, Video video, int column, int row)
        {
            this.parrent_g = parrent;
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

        public Player(Grid parrent, Picture picture, Records record, int column, int row)
        {
            this.parrent_g = parrent;
            this.picture = picture;
            this.record = record;
            this.column = column;
            this.row = row;
        }

        ~Player()
        {
            this.parrent_g = null;
            this.video = null;
            this.parrent_g = null;
            this.picture = null;
            this.record = null;
            this.column = 0;
            this.row = 0;
        }

        public void CreateVideo()
        {
            // Main Panel Card
            StackPanel main_panel = new StackPanel();
            main_panel.Orientation = Orientation.Vertical;
            main_panel.Margin = new Thickness(3);
            main_panel.HorizontalAlignment = HorizontalAlignment.Center;
            main_panel.Background = System.Windows.Media.Brushes.Gray;
            Grid.SetColumn(main_panel, this.column);
            Grid.SetRow(main_panel, this.row);
            this.parrent_g.Children.Add(main_panel);

            // Add Label
            Label name = new Label();
            name.Content = this.video.CamName;
            name.HorizontalAlignment = HorizontalAlignment.Center;
            name.Foreground = System.Windows.Media.Brushes.DarkRed;
            main_panel.Children.Add(name);
            // Add Label
            Label date = new Label();
            date.Content = this.video.Date;
            date.HorizontalAlignment = HorizontalAlignment.Center;
            date.Foreground = System.Windows.Media.Brushes.DarkRed;
            main_panel.Children.Add(date);
            // Add Label
            Label time = new Label();
            time.Content = this.video.Time;
            time.HorizontalAlignment = HorizontalAlignment.Center;
            time.Foreground = System.Windows.Media.Brushes.DarkRed;
            main_panel.Children.Add(time);

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
            main_panel.Children.Add(player);

            // Add Label Fro Time Spam
            this.time_spam = new Label();
            this.time_spam.HorizontalAlignment = HorizontalAlignment.Center;
            main_panel.Children.Add(this.time_spam);

            // Add Panel Play Stop Pause
            StackPanel panel_Play_Stop_Pause = new StackPanel();
            panel_Play_Stop_Pause.Orientation = Orientation.Horizontal;
            panel_Play_Stop_Pause.HorizontalAlignment = HorizontalAlignment.Center;
            panel_Play_Stop_Pause.VerticalAlignment = VerticalAlignment.Center;
            main_panel.Children.Add(panel_Play_Stop_Pause);
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
            panel_bakc_forw_open_del.Margin = new Thickness(0,0,0,7);
            main_panel.Children.Add(panel_bakc_forw_open_del);
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


        public void CreatePicture()
        {
            // Main Panel Card
            StackPanel main_panel = new StackPanel();
            main_panel.Background = System.Windows.Media.Brushes.Gray;
            main_panel.Margin = new Thickness(3);
            main_panel.Orientation = Orientation.Vertical;
            main_panel.HorizontalAlignment = HorizontalAlignment.Center;
            main_panel.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetRow(main_panel, this.row);
            Grid.SetColumn(main_panel, this.column);
            this.parrent_g.Children.Add(main_panel);

            // Label
            Label label_1 = new Label();
            label_1.Margin = new Thickness(0,11,0,0);
            label_1.HorizontalAlignment = HorizontalAlignment.Center;
            label_1.VerticalAlignment = VerticalAlignment.Center;
            label_1.Content = this.picture.CamName;
            label_1.Foreground = System.Windows.Media.Brushes.DarkRed;
            label_1.FontSize = 12;
            main_panel.Children.Add(label_1);

            // Label
            Label label_2 = new Label();
            label_2.HorizontalAlignment = HorizontalAlignment.Center;
            label_2.VerticalAlignment = VerticalAlignment.Center;
            label_2.Content = this.picture.Date;
            label_2.Foreground = System.Windows.Media.Brushes.DarkRed;
            label_2.FontSize = 12;
            main_panel.Children.Add(label_2);

            // Label
            Label label_3 = new Label();
            label_3.HorizontalAlignment = HorizontalAlignment.Center;
            label_2.VerticalAlignment = VerticalAlignment.Center;
            label_3.Content = this.picture.Time;
            label_3.Foreground = System.Windows.Media.Brushes.DarkRed;
            label_3.FontSize = 12;
            main_panel.Children.Add(label_3);

            // Create Media Element
            MediaElement image = new MediaElement();
            image.Source = new Uri(this.picture.Path);
            image.Margin = new Thickness(1);
            image.Height = 200;
            image.VerticalAlignment = VerticalAlignment.Center;
            main_panel.Children.Add(image);

            // Buttons StackPanel
            StackPanel panel_b = new StackPanel();
            panel_b.Orientation = Orientation.Horizontal;
            panel_b.HorizontalAlignment = HorizontalAlignment.Center;
            panel_b.VerticalAlignment = VerticalAlignment.Center;
            panel_b.Background = System.Windows.Media.Brushes.Orange;
            panel_b.Margin = new Thickness(0,0,0,11);
            main_panel.Children.Add(panel_b);
            // Add Button
            Button open = new Button();
            open.Content = "Open";
            open.FontSize = 12;
            open.Padding = new Thickness(7, 0, 7, 0);
            open.Click += (object obj, RoutedEventArgs e) =>
            {
                RecordFullScreen fullscreen_page;
                if (!this.fullscreen)
                {
                    this.fullscreen = true;
                    this.fullscreen_page = new RecordFullScreen(this.picture, this.record);
                    this.fullscreen_page.Show();
                }
                else
                {
                    this.fullscreen_page.Activate();
                }
            };
            panel_b.Children.Add(open);
            // Add Button
            Button delete = new Button();
            delete.Content = "Delete";
            delete.FontSize = 12;
            delete.Padding = new Thickness(7, 0, 7, 0);
            delete.Click += (object obj, RoutedEventArgs e) =>
            {
                if (MessageBox.Show("Are you sure?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    File.Delete(this.picture.Path);
                }
            };
            panel_b.Children.Add(delete);
        }


    }
}
