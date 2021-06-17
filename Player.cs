using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace IPCamera
{
    class Player
    {

        public Grid parrent;
        public Video video;
        public int column;
        public int row;
        public int buttonsFontSize = 15;


        Grid vid_Grid;
        Grid titleGrid;
        Label name;
        Label date;
        Label time;
        MediaElement player;
        StackPanel panel;

        public Player(Grid parrent, Video video, int column, int row)
        {
            this.parrent = parrent;
            this.video = video;
            this.column = column;
            this.row = row;
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
            // Add 2 Rows
            RowDefinition row_2 = new RowDefinition();
            row_2.Height = new GridLength(30);
            RowDefinition row_3 = new RowDefinition();
            row_3.Height = new GridLength(0, GridUnitType.Auto);
            RowDefinition row_4 = new RowDefinition();
            row_4.Height = new GridLength(40);
            vid_Grid.RowDefinitions.Add(row_2);
            vid_Grid.RowDefinitions.Add(row_3);
            vid_Grid.RowDefinitions.Add(row_4);
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
            name.Foreground = System.Windows.Media.Brushes.LightGray;
            Grid.SetColumn(name, 0);
            this.date = new Label();
            date.Content = this.video.Date;
            date.Foreground = System.Windows.Media.Brushes.LightGray;
            Grid.SetColumn(date, 1);
            this.time = new Label();
            time.Content = this.video.Time;
            time.Foreground = System.Windows.Media.Brushes.LightGray;
            Grid.SetColumn(time, 2);
            titleGrid.Children.Add(name);
            titleGrid.Children.Add(date);
            titleGrid.Children.Add(time);
            // Video Player
            this.player = new MediaElement();
            player.Source = new Uri(this.video.Path);
            player.Margin = new Thickness(0, 7, 0, 0);
            player.UnloadedBehavior = MediaState.Manual;
            Grid.SetRow(player, 1);
            vid_Grid.Children.Add(player);
            // Add New Grid With The Buttons
            this.panel = new StackPanel();
            panel.Width = 308;
            panel.Height = 50;
            panel.Margin = new Thickness(0, 20, 0, 0);
            panel.Orientation = Orientation.Horizontal;
            panel.HorizontalAlignment = HorizontalAlignment.Center;
            panel.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetRow(panel, 2);
            vid_Grid.Children.Add(panel);
            Button play = new Button();
            Button stop = new Button();
            Button pause = new Button();
            Button details = new Button();
            play.Content = "Play";
            stop.Content = "Stop";
            pause.Content = "Pause";
            details.Content = "Details";
            play.FontSize = this.buttonsFontSize;
            stop.FontSize = this.buttonsFontSize;
            pause.FontSize = this.buttonsFontSize;
            details.FontSize = this.buttonsFontSize;
            play.Width = 77;
            stop.Width = 77;
            pause.Width = 77;
            details.Width = 77;
            play.Height = 33;
            stop.Height = 33;
            pause.Height = 33;
            details.Height = 33;
            play.Click += (object obj, RoutedEventArgs e) =>
            {
                Console.WriteLine("Start.");
                this.player.Play();
            };
            stop.Click += (object obj, RoutedEventArgs e) =>
            {
                Console.WriteLine("Stop.");
                this.player.Stop();
            };
            pause.Click += (object obj, RoutedEventArgs e) =>
            {
                Console.WriteLine("Pause.");
                this.player.Pause();
            };
            details.Click += (object obj, RoutedEventArgs e) =>
            {
                Console.WriteLine("Opens This Player Full Screen.");
            };
            panel.Children.Add(play);
            panel.Children.Add(stop);
            panel.Children.Add(pause);
            panel.Children.Add(details);
            // Print To Console
            Console.WriteLine($"Creating Video Grid: {this.video.CamName}  {this.video.Date}  {this.video.Time}");
        }
    }
}
