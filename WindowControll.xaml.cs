using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VisioForge.Controls.UI.WPF;
using System.Data.SqlClient;

namespace IPCamera
{
    /// <summary>
    /// Interaction logic for WindowControll.xaml
    /// </summary>
    public partial class WindowControll : Window
    {
        private Camera camera;
        //private VideoCapture camera;

        public WindowControll(Camera cam)
        {
            InitializeComponent();
            this.DataContext = this;
            // Setup this_camera
            this.camera = cam;
            // Chech if Face_Recognition, Face Detection  is checked
            updateFaceDetecRecog();
            Start_cam();
        }

        protected override void OnClosed(EventArgs e)
        {
            MainWindow.RestartApp();
            this.Close();
        }

        // Check the database and set the values to checkboxes
        public void updateFaceDetecRecog()
        {
            String detection = "";
            String recognition = "";
            try
            {
                // Select from database Face_Detection and Face_Recognition
                SqlConnection cn = new SqlConnection(MainWindow.DB_connection_string);
                String query = $"SELECT TOP 1 Face_Detection, Face_Recognition FROM dbo.MyCameras WHERE urls='{this.camera.url}' AND Name='{this.camera.name}'";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        detection = reader["Face_Detection"].ToString().Trim();
                        recognition = reader["Face_Recognition"].ToString().Trim();
                    }
                }
                cn.Close();
            }
            catch (System.Data.SqlClient.SqlException se)
            {
                System.Windows.MessageBox.Show("Error selecting Face_Detection and Face_Recognition from Database!  [ERROR CODE]: " + se);
            }
            // Set the CheckBoxes
            Face_det.IsChecked = (detection == "True" ? true : false);
            Face_rec.IsChecked = (recognition == "True"? true : false);
        }


        // Create And Start Video Capture
        private void Start_cam()
        {
            this.camera.video.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            this.camera.video.VerticalAlignment = VerticalAlignment.Stretch;
            this.camera.video.Margin = new Thickness(0, 0, 0, 0);
            this.camera.video.Width = Double.NaN;
            this.camera.video.Height = Double.NaN;
            vidoe_grid.Children.Add(this.camera.video);
        }



        private void Face_Detection_Chencked(object sender, EventArgs e)
        {
            try
            {
                // Update DataBase this Camera Object field Face Detection 1
                SqlConnection cn = new SqlConnection(MainWindow.DB_connection_string);
                String query = $"UPDATE dbo.MyCameras SET Face_Detection='{1}' WHERE urls='{this.camera.url}' AND Name='{this.camera.name}'";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
            }
            catch (System.Data.SqlClient.SqlException se)
            {
                System.Windows.MessageBox.Show("Error updateting Face_Detection true into Database!  [ERROR CODE]: " + se);
            }
        }
        private void Face_Detection_UNChencked(object sender, EventArgs e)
        {
            try
            {
                // Update DataBase this Camera Object field Face Detection 0
                SqlConnection cn = new SqlConnection(MainWindow.DB_connection_string);
                String query = $"UPDATE dbo.MyCameras SET Face_Detection='{0}' WHERE urls='{this.camera.url}' AND Name='{this.camera.name}'";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
            }
            catch (System.Data.SqlClient.SqlException se)
            {
                System.Windows.MessageBox.Show("Error updateting Face_Detection true into Database!  [ERROR CODE]: " + se);
            }
        }

        private void Face_Recognition_Chencked(object sender, EventArgs e)
        {
            try
            {
                // Update DataBase this Camera Object field Face Detection 1
                SqlConnection cn = new SqlConnection(MainWindow.DB_connection_string);
                String query = $"UPDATE dbo.MyCameras SET Face_Recognition='{1}' WHERE urls='{this.camera.url}' AND Name='{this.camera.name}'";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
            }
            catch (System.Data.SqlClient.SqlException se)
            {
                System.Windows.MessageBox.Show("Error updateting Face_Detection true into Database!  [ERROR CODE]: " + se);
            }
        }
        private void Face_Recognition_UNChencked(object sender, EventArgs e)
        {
            try
            {
                // Update DataBase this Camera Object field Face Detection 0
                SqlConnection cn = new SqlConnection(MainWindow.DB_connection_string);
                String query = $"UPDATE dbo.MyCameras SET Face_Recognition='{0}' WHERE urls='{this.camera.url}' AND Name='{this.camera.name}'";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
            }
            catch (System.Data.SqlClient.SqlException se)
            {
                System.Windows.MessageBox.Show("Error updateting Face_Detection true into Database!  [ERROR CODE]: " + se);
            }
        }


        private void UP_button_click(object sender, MouseButtonEventArgs e)
        {
            System.Windows.MessageBox.Show("Mouse UP!");
        }

        private void DOWN_button_click(object sender, MouseButtonEventArgs e)
        {
            System.Windows.MessageBox.Show("Mouse DOWN!");
        }

        private void LEFT_button_click(object sender, MouseButtonEventArgs e)
        {
            System.Windows.MessageBox.Show("Mouse LEFT!");
        }

        private void RIGHT_button_click(object sender, MouseButtonEventArgs e)
        {
            System.Windows.MessageBox.Show("Mouse RIGHT!");
        }

        private void TAKE_PIC_button_click(object sender, MouseButtonEventArgs e)
        {
            System.Windows.MessageBox.Show("Mouse TAKE PICTURE!");
        }

    }
}
