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
using System.Windows.Shapes;

namespace IPCamera
{
    /// <summary>
    /// Interaction logic for RecordFullScreen.xaml
    /// </summary>
    public partial class RecordFullScreen : Window
    {
        Player player;
        public RecordFullScreen(Player player)
        {
            InitializeComponent();

            this.player = player;
            main_grid.Children.Add(this.player.vid_Grid);
        }

        // On Close Window
        protected override void OnClosed(EventArgs e)
        {
            main_grid.Children.Remove(this.player.vid_Grid);
            //Grid.SetRow(this.player.vid_Grid, this.player.row);
            //Grid.SetColumn(this.player.vid_Grid, this.player.column);
            this.player.parrent.Children.Add(this.player.vid_Grid);
            this.player.fullscreen = false;
            this.Close();
        }
    }
}
