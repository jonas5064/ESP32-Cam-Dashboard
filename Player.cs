using System;
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

        public int buttonsFontSize = 17;
        public Button play;
        public Button stop;
        public Button pause;
        public RepeatButton backard;
        public RepeatButton forward;

        public Grid vid_Grid;
        Grid titleGrid;
        Label name;
        Label date;
        Label time;
        Label time_spam;
        MediaElement player;
        StackPanel panel;

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

        public void Create()
        {
            // Create Video Grid And Add it to Video_Grid
            this.vid_Grid = new Grid();
            vid_Grid.Background = System.Windows.Media.Brushes.Gray;
            vid_Grid.Margin = new Thickness(3);
            Grid.SetRow(vid_Grid, this.row);
            Grid.SetColumn(vid_Grid, this.column);
            parrent.Children.Add(vid_Grid);
            // Add 4 Rows
            RowDefinition row_2 = new RowDefinition();
            row_2.Height = new GridLength(30);
            RowDefinition row_3 = new RowDefinition();
            row_3.Height = new GridLength(0, GridUnitType.Auto);
            RowDefinition row_4 = new RowDefinition();
            row_4.Height = new GridLength(30);
            RowDefinition row_5 = new RowDefinition();
            row_5.Height = new GridLength(40);
            vid_Grid.RowDefinitions.Add(row_2);
            vid_Grid.RowDefinitions.Add(row_3);
            vid_Grid.RowDefinitions.Add(row_4);
            vid_Grid.RowDefinitions.Add(row_5);
            // Add New Grid Grid At Row 0 (Title Grid)
            this.titleGrid = new Grid();
            Grid.SetRow(titleGrid, 0);
            vid_Grid.Children.Add(titleGrid);
            // Tittle Grid Has 3 Columns
            titleGrid.ColumnDefinitions.Add(new ColumnDefinition());
            titleGrid.ColumnDefinitions.Add(new ColumnDefinition());
            titleGrid.ColumnDefinitions.Add(new ColumnDefinition());
            // Add 3 Labels
            this.name = new Label();
            name.Content = this.video.CamName;
            name.HorizontalAlignment = HorizontalAlignment.Center;
            name.Foreground = System.Windows.Media.Brushes.DarkRed;
            Grid.SetColumn(name, 0);
            this.date = new Label();
            date.Content = this.video.Date;
            date.HorizontalAlignment = HorizontalAlignment.Center;
            date.Foreground = System.Windows.Media.Brushes.DarkRed;
            Grid.SetColumn(date, 1);
            this.time = new Label();
            time.Content = this.video.Time;
            time.HorizontalAlignment = HorizontalAlignment.Center;
            time.Foreground = System.Windows.Media.Brushes.DarkRed;
            Grid.SetColumn(time, 2);
            titleGrid.Children.Add(name);
            titleGrid.Children.Add(date);
            titleGrid.Children.Add(time);
            // Video Player
            Console.WriteLine($"\n\nVideo {this.video.Path}\n");
            this.player = new MediaElement();
            this.player.Source = new Uri(this.video.Path);
            this.player.Margin = new Thickness(7, 7, 7, 0);
            this.player.LoadedBehavior = MediaState.Manual;
            this.player.ScrubbingEnabled = true;
            this.player.UnloadedBehavior = MediaState.Close;
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
            // Add New Grid With The Buttons
            this.panel = new StackPanel();
            panel.Orientation = Orientation.Horizontal;
            panel.HorizontalAlignment = HorizontalAlignment.Center;
            panel.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetRow(panel, 3);
            vid_Grid.Children.Add(panel);
            Button play = new Button();
            Button stop = new Button();
            Button pause = new Button();
            RepeatButton backard = new RepeatButton();
            RepeatButton forward = new RepeatButton();
            Button open = new Button();
            play.Content = "Play";
            stop.Content = "Stop";
            pause.Content = "Pause";
            backard.Content = "Back";
            forward.Content = "Forw";
            open.Content = "Open";
            backard.Interval = 200;
            forward.Interval = 200;
            play.FontSize = this.buttonsFontSize;
            stop.FontSize = this.buttonsFontSize;
            pause.FontSize = this.buttonsFontSize;
            backard.FontSize = this.buttonsFontSize;
            forward.FontSize = this.buttonsFontSize;
            open.FontSize = this.buttonsFontSize;
            play.Padding = new Thickness(3,0,3,0);
            stop.Padding = new Thickness(3, 0, 3, 0);
            pause.Padding = new Thickness(3, 0, 3, 0);
            backard.Padding = new Thickness(3, 0, 3, 0);
            forward.Padding = new Thickness(3, 0, 3, 0);
            open.Padding = new Thickness(3, 0, 3, 0);
            play.Click += (object obj, RoutedEventArgs e) =>
            {
                if (this.player.Source != null)
                {
                    this.player.Play();
                }
            };
            stop.Click += (object obj, RoutedEventArgs e) =>
            {
                if (this.player.Source != null)
                {
                    this.player.Stop();
                }
            };
            pause.Click += (object obj, RoutedEventArgs e) =>
            {
                if (this.player.Source != null)
                {
                    this.player.Pause();
                }
            };
            backard.Click += (object obj, RoutedEventArgs e) =>
            {
                if (this.player.Source != null)
                {
                    this.player.Position -= TimeSpan.FromMilliseconds(1000);
                }
            };
            forward.Click += (object obj, RoutedEventArgs e) =>
            {
                if (this.player.Source != null)
                {
                    this.player.Position += TimeSpan.FromMilliseconds(1000);
                }
            };
            open.Click += (object obj, RoutedEventArgs e) =>
            {
                if (! this.fullscreen)
                {
                    //this.parrent.Children.Remove(this.vid_Grid);
                    this.fullscreen = true;
                    this.fullscreen_page = new RecordFullScreen(this);
                    fullscreen_page.Show();
                }
                else
                {
                    this.fullscreen_page.Activate();
                }
                
            };
            panel.Children.Add(play);
            panel.Children.Add(stop);
            panel.Children.Add(pause);
            panel.Children.Add(backard);
            panel.Children.Add(forward);
            panel.Children.Add(open);
            // Print To Console
            Console.WriteLine($"Creating Video Grid: {this.video.CamName}  {this.video.Date}  {this.video.Time}");
        }

    }
}
