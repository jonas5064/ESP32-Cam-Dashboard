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

        public Grid Parrent_g { get; set; }
        public Video Video { get; set; }
        public Picture Picture { get; set; }
        public Records Record { get; set; }
        public int Column { get; set; }
        public int Row { get; set; }
        public Boolean Fullscreen { get; set; }
        RecordFullScreen Fullscreen_page { get; set; }

        public int ButtonsFontSize { get; set; }
        Label Time_spam { get; set; }
        MediaElement MyPlayer { get; set; }
        

        public Player(Grid parrent, Video video, int column, int row)
        {
            this.Parrent_g = parrent;
            this.Video = video;
            this.Column = column;
            this.Row = row;
            this.Fullscreen = false;
            this.ButtonsFontSize = 12;

            // Threading to Update The Time Spam Label Show The Time Of Video
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += (object sender, EventArgs e) =>
            {
                if (this.MyPlayer.Source != null)
                {
                    if (this.MyPlayer.NaturalDuration.HasTimeSpan)
                    {
                        this.Time_spam.Content = String.Format("{0} / {1}", 
                                                    this.MyPlayer.Position.ToString(@"mm\:ss"), 
                                                    this.MyPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
                    }
                }
            };
            timer.Start();
        }

        public Player(Grid parrent, Picture picture, Records record, int column, int row)
        {
            this.Parrent_g = parrent;
            this.Picture = picture;
            this.Record = record;
            this.Column = column;
            this.Row = row;
        }

        ~Player()
        {
            this.Parrent_g = null;
            this.Video = null;
            this.Parrent_g = null;
            this.Picture = null;
            this.Record = null;
            this.Column = 0;
            this.Row = 0;
        }

        public void CreateVideo()
        {
            // Main Panel Card
            StackPanel main_panel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(3),
                HorizontalAlignment = HorizontalAlignment.Center,
                Background = System.Windows.Media.Brushes.Gray,
                MinWidth = 500
            };
            Grid.SetColumn(main_panel, this.Column);
            Grid.SetRow(main_panel, this.Row);
            this.Parrent_g.Children.Add(main_panel);

            // Add Label
            Label name = new Label
            {
                Content = this.Video.CamName,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = System.Windows.Media.Brushes.DarkRed
            };
            main_panel.Children.Add(name);
            // Add Label
            Label date = new Label
            {
                Content = this.Video.Date,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = System.Windows.Media.Brushes.DarkRed
            };
            main_panel.Children.Add(date);
            // Add Label
            Label time = new Label
            {
                Content = this.Video.Time,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = System.Windows.Media.Brushes.DarkRed
            };
            main_panel.Children.Add(time);

            // Video Player
            Console.WriteLine($"\n\nVideo {this.Video.Path}\n");
            this.MyPlayer = new MediaElement
            {
                Source = new Uri(this.Video.Path),
                Height = 233,
                Margin = new Thickness(7, 7, 7, 0),
                LoadedBehavior = MediaState.Manual,
                ScrubbingEnabled = true,
                UnloadedBehavior = MediaState.Close,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            this.MyPlayer.MediaOpened += (object sender, RoutedEventArgs e) =>
            {

            };
            this.MyPlayer.Play();
            this.MyPlayer.Pause();
            this.MyPlayer.Position = TimeSpan.FromSeconds(0);
            main_panel.Children.Add(this.MyPlayer);

            // Add Label Fro Time Spam
            this.Time_spam = new Label
            {
                HorizontalAlignment = HorizontalAlignment.Center
            };
            main_panel.Children.Add(this.Time_spam);

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
                FontSize = this.ButtonsFontSize,
                Padding = new Thickness(3, 0, 3, 0)
            };
            play.Click += (object obj, RoutedEventArgs e) =>
            {
                if (this.MyPlayer.Source != null)
                {
                    this.MyPlayer.Play();
                }
            };
            panel_Play_Stop_Pause.Children.Add(play);
            // Add Button
            Button stop = new Button
            {
                Content = "Stop",
                HorizontalAlignment = HorizontalAlignment.Stretch,
                FontSize = this.ButtonsFontSize,
                Padding = new Thickness(3, 0, 3, 0)
            };
            stop.Click += (object obj, RoutedEventArgs e) =>
            {
                if (this.MyPlayer.Source != null)
                {
                    this.MyPlayer.Stop();
                }
            };
            panel_Play_Stop_Pause.Children.Add(stop);
            // Add Button
            Button pause = new Button
            {
                Content = "Pause",
                HorizontalAlignment = HorizontalAlignment.Stretch,
                FontSize = this.ButtonsFontSize,
                Padding = new Thickness(3, 0, 3, 0)
            };
            pause.Click += (object obj, RoutedEventArgs e) =>
            {
                if (this.MyPlayer.Source != null)
                {
                    this.MyPlayer.Pause();
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
                FontSize = this.ButtonsFontSize,
                Padding = new Thickness(3, 0, 3, 0)
            };
            backard.Click += (object obj, RoutedEventArgs e) =>
            {
                if (this.MyPlayer.Source != null)
                {
                    this.MyPlayer.Position -= TimeSpan.FromMilliseconds(1000);
                }
            };
            panel_bakc_forw_open_del.Children.Add(backard);
            // Add Button
            RepeatButton forward = new RepeatButton
            {
                Content = "Forw",
                Interval = 200,
                FontSize = this.ButtonsFontSize,
                Padding = new Thickness(3, 0, 3, 0)
            };
            forward.Click += (object obj, RoutedEventArgs e) =>
            {
                if (this.MyPlayer.Source != null)
                {
                    this.MyPlayer.Position += TimeSpan.FromMilliseconds(1000);
                }
            };
            panel_bakc_forw_open_del.Children.Add(forward);
            // Add Button
            Button open = new Button
            {
                Content = "Open",
                FontSize = this.ButtonsFontSize,
                Padding = new Thickness(3, 0, 3, 0)
            };
            open.Click += (object obj, RoutedEventArgs e) =>
            {
                if (!this.Fullscreen)
                {
                    this.Fullscreen = true;
                    this.Fullscreen_page = new RecordFullScreen(this);
                    this.Fullscreen_page.Show();
                }
                else
                {
                    this.Fullscreen_page.Activate();
                }

            };
            panel_bakc_forw_open_del.Children.Add(open);
            // Add Button
            Button delete = new Button
            {
                Content = "Del",
                FontSize = this.ButtonsFontSize,
                Padding = new Thickness(3, 0, 3, 0)
            };
            delete.Click += (object obj, RoutedEventArgs e) =>
            {
                if (MessageBox.Show("Are you sure?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    File.Delete(this.Video.Path);
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
                VerticalAlignment = VerticalAlignment.Center,
                MinWidth = 400
            };
            Grid.SetRow(main_panel, this.Row);
            Grid.SetColumn(main_panel, this.Column);
            this.Parrent_g.Children.Add(main_panel);

            // Label
            Label label_1 = new Label
            {
                Margin = new Thickness(0, 11, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Content = this.Picture.CamName,
                Foreground = System.Windows.Media.Brushes.DarkRed,
                FontSize = 12
            };
            main_panel.Children.Add(label_1);

            // Label
            Label label_2 = new Label
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Content = this.Picture.Date,
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
            label_3.Content = this.Picture.Time;
            label_3.Foreground = System.Windows.Media.Brushes.DarkRed;
            label_3.FontSize = 12;
            main_panel.Children.Add(label_3);

            // Create Media Element
            MediaElement image = new MediaElement
            {
                Source = new Uri(this.Picture.Path),
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
                if (!this.Fullscreen)
                {
                    this.Fullscreen = true;
                    this.Fullscreen_page = new RecordFullScreen(this.Picture, this.Record);
                    this.Fullscreen_page.Show();
                }
                else
                {
                    this.Fullscreen_page.Activate();
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
                    File.Delete(this.Picture.Path);
                }
            };
            panel_b.Children.Add(delete);
        }


    }
}
