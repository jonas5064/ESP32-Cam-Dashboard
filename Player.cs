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
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
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
            StackPanel main_panel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(3),
                HorizontalAlignment = HorizontalAlignment.Center,
                Background = System.Windows.Media.Brushes.Gray
            };
            Grid.SetColumn(main_panel, this.column);
            Grid.SetRow(main_panel, this.row);
            this.parrent_g.Children.Add(main_panel);

            // Add Label
            Label name = new Label
            {
                Content = this.video.CamName,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = System.Windows.Media.Brushes.DarkRed
            };
            main_panel.Children.Add(name);
            // Add Label
            Label date = new Label
            {
                Content = this.video.Date,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = System.Windows.Media.Brushes.DarkRed
            };
            main_panel.Children.Add(date);
            // Add Label
            Label time = new Label
            {
                Content = this.video.Time,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = System.Windows.Media.Brushes.DarkRed
            };
            main_panel.Children.Add(time);

            // Video Player
            Console.WriteLine($"\n\nVideo {this.video.Path}\n");
            this.player = new MediaElement
            {
                Source = new Uri(this.video.Path),
                Height = 233,
                Margin = new Thickness(7, 7, 7, 0),
                LoadedBehavior = MediaState.Manual,
                ScrubbingEnabled = true,
                UnloadedBehavior = MediaState.Close,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            this.player.MediaOpened += (object sender, RoutedEventArgs e) =>
            {

            };
            this.player.Play();
            this.player.Pause();
            this.player.Position = TimeSpan.FromSeconds(0);
            main_panel.Children.Add(player);

            // Add Label Fro Time Spam
            this.time_spam = new Label
            {
                HorizontalAlignment = HorizontalAlignment.Center
            };
            main_panel.Children.Add(this.time_spam);

            // Add Panel Play Stop Pause
            StackPanel panel_Play_Stop_Pause = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            main_panel.Children.Add(panel_Play_Stop_Pause);
            // Add Button
            Button play = new Button
            {
                Content = "Play",
                HorizontalAlignment = HorizontalAlignment.Stretch,
                FontSize = this.buttonsFontSize,
                Padding = new Thickness(3, 0, 3, 0)
            };
            play.Click += (object obj, RoutedEventArgs e) =>
            {
                if (this.player.Source != null)
                {
                    this.player.Play();
                }
            };
            panel_Play_Stop_Pause.Children.Add(play);
            // Add Button
            Button stop = new Button
            {
                Content = "Stop",
                HorizontalAlignment = HorizontalAlignment.Stretch,
                FontSize = this.buttonsFontSize,
                Padding = new Thickness(3, 0, 3, 0)
            };
            stop.Click += (object obj, RoutedEventArgs e) =>
            {
                if (this.player.Source != null)
                {
                    this.player.Stop();
                }
            };
            panel_Play_Stop_Pause.Children.Add(stop);
            // Add Button
            Button pause = new Button
            {
                Content = "Pause",
                HorizontalAlignment = HorizontalAlignment.Stretch,
                FontSize = this.buttonsFontSize,
                Padding = new Thickness(3, 0, 3, 0)
            };
            pause.Click += (object obj, RoutedEventArgs e) =>
            {
                if (this.player.Source != null)
                {
                    this.player.Pause();
                }
            };
            panel_Play_Stop_Pause.Children.Add(pause);

            // Add Panel Backward Forward Open Delete
            StackPanel panel_bakc_forw_open_del = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 7)
            };
            main_panel.Children.Add(panel_bakc_forw_open_del);
            // Add Button
            RepeatButton backard = new RepeatButton
            {
                Content = "Back",
                Interval = 200,
                FontSize = this.buttonsFontSize,
                Padding = new Thickness(3, 0, 3, 0)
            };
            backard.Click += (object obj, RoutedEventArgs e) =>
            {
                if (this.player.Source != null)
                {
                    this.player.Position -= TimeSpan.FromMilliseconds(1000);
                }
            };
            panel_bakc_forw_open_del.Children.Add(backard);
            // Add Button
            RepeatButton forward = new RepeatButton
            {
                Content = "Forw",
                Interval = 200,
                FontSize = this.buttonsFontSize,
                Padding = new Thickness(3, 0, 3, 0)
            };
            forward.Click += (object obj, RoutedEventArgs e) =>
            {
                if (this.player.Source != null)
                {
                    this.player.Position += TimeSpan.FromMilliseconds(1000);
                }
            };
            panel_bakc_forw_open_del.Children.Add(forward);
            // Add Button
            Button open = new Button
            {
                Content = "Open",
                FontSize = this.buttonsFontSize,
                Padding = new Thickness(3, 0, 3, 0)
            };
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
            Button delete = new Button
            {
                Content = "Del",
                FontSize = this.buttonsFontSize,
                Padding = new Thickness(3, 0, 3, 0)
            };
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
            StackPanel main_panel = new StackPanel
            {
                Background = System.Windows.Media.Brushes.Gray,
                Margin = new Thickness(3),
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetRow(main_panel, this.row);
            Grid.SetColumn(main_panel, this.column);
            this.parrent_g.Children.Add(main_panel);

            // Label
            Label label_1 = new Label
            {
                Margin = new Thickness(0, 11, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Content = this.picture.CamName,
                Foreground = System.Windows.Media.Brushes.DarkRed,
                FontSize = 12
            };
            main_panel.Children.Add(label_1);

            // Label
            Label label_2 = new Label
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Content = this.picture.Date,
                Foreground = System.Windows.Media.Brushes.DarkRed,
                FontSize = 12
            };
            main_panel.Children.Add(label_2);

            // Label
            Label label_3 = new Label
            {
                HorizontalAlignment = HorizontalAlignment.Center
            };
            label_2.VerticalAlignment = VerticalAlignment.Center;
            label_3.Content = this.picture.Time;
            label_3.Foreground = System.Windows.Media.Brushes.DarkRed;
            label_3.FontSize = 12;
            main_panel.Children.Add(label_3);

            // Create Media Element
            MediaElement image = new MediaElement
            {
                Source = new Uri(this.picture.Path),
                Margin = new Thickness(1),
                Height = 200,
                VerticalAlignment = VerticalAlignment.Center
            };
            main_panel.Children.Add(image);

            // Buttons StackPanel
            StackPanel panel_b = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = System.Windows.Media.Brushes.Orange,
                Margin = new Thickness(0, 0, 0, 11)
            };
            main_panel.Children.Add(panel_b);
            // Add Button
            Button open = new Button
            {
                Content = "Open",
                FontSize = 12,
                Padding = new Thickness(7, 0, 7, 0)
            };
            open.Click += (object obj, RoutedEventArgs e) =>
            {
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
            Button delete = new Button
            {
                Content = "Delete",
                FontSize = 12,
                Padding = new Thickness(7, 0, 7, 0)
            };
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
