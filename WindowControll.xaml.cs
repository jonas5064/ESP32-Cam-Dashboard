using System;
using System.Windows;
using System.Data.SqlClient;
using System.Windows.Input;
using VisioForge.Types;
using System.Windows.Media;
using System.Windows.Controls;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.IO;
using System.Threading;

namespace IPCamera
{
    /// <summary>
    /// Interaction logic for WindowControll.xaml
    /// </summary>
    public partial class WindowControll : Window
    {
        public Camera camera;
        public String url = "";

        public WindowControll(Camera cam)
        {
            InitializeComponent();
            this.DataContext = this;
           
            // Setup this_camera
            this.camera = cam;
            this.url = this.camera.url;
            // Chech if Face_Recognition, Face Detection  is checked
            Face_det.IsChecked = (this.camera.Detection);
            Face_rec.IsChecked = (this.camera.Recognition);
            // Setup Brightness and Contrast Labels and Sliders
            brightness_label.Content = $"Brightness: {this.camera.Brightness}";
            contrast_label.Content   = $"Contrast:   {this.camera.Contrast}";
            darkness_label.Content   = $"Darkness:   {this.camera.Darkness}";
            brightness_slider.Value  = this.camera.Brightness;
            contrast_slider.Value    = this.camera.Contrast;
            darkness_slider.Value    = this.camera.Darkness;
            sensitivity_value_label.Content = $"{this.camera.On_move_sensitivity}";
            sensitivity_slider.Value = this.camera.On_move_sensitivity;
            // Setup recording Button
            if (this.camera.Recording)
            {
                rec_label.Content = "Recording";
                rec_label.Foreground = Brushes.Red;
            }
            else
            {
                rec_label.Content = "Stop Recording";
                rec_label.Foreground = Brushes.Gray;
            }
            // Setup On Movement checkboxes
            sms_checkbox.IsChecked = (this.camera.On_move_sms);
            email_checkbox.IsChecked = (this.camera.On_move_email);
            pic_checkbox.IsChecked = (this.camera.On_move_pic);
            rec_checkbox.IsChecked = (this.camera.On_move_rec);
            // Add Title
            cameras_title.Content = "Cameras Name: " + this.camera.name;
        }



        protected override void OnClosed(EventArgs e)
        {
            this.Close();
        }


        // Face Detection Checked
        private void Face_Detection_Chencked(object sender, EventArgs e)
        {
            if (!this.camera.Detection)
            {
                this.camera.Detection = true;
                try
                {
                    // Update DataBase this Camera Object field Face Detection 1
                    SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
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
                // Restart Camera
                this.camera.Stop();
                this.camera.Start();
            }
        }

        // Face Detection Unchecked
        private void Face_Detection_UNChencked(object sender, EventArgs e)
        {
            if (this.camera.Detection)
            {
                this.camera.Detection = false;
                if (this.camera.Recognition)
                {
                    this.camera.Recognition = false;
                    Face_rec.IsChecked = (this.camera.recognition);
                }
                try
                {
                    // Update DataBase this Camera Object field Face Detection 0
                    SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                    String query = $"UPDATE dbo.MyCameras SET Face_Detection='{0}', Face_Recognition='{0}' WHERE urls='{this.camera.url}' AND Name='{this.camera.name}'";
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
                // Restart Camera
                this.camera.Stop();
                this.camera.Start();
            }
        }

        // Face Recognition Chekced
        private void Face_Recognition_Chencked(object sender, EventArgs e)
        {
            if (!this.camera.Recognition)
            {
                this.camera.Recognition = true;
                if (!this.camera.Detection)
                {
                    this.camera.Detection = true;
                    Face_det.IsChecked = (this.camera.detection);
                }
                try
                {
                    // Update DataBase this Camera Object field Face Detection 1
                    SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                    String query = $"UPDATE dbo.MyCameras SET Face_Recognition='{1}', Face_Detection='{1}' WHERE urls='{this.camera.url}' AND Name='{this.camera.name}'";
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
                // Restart Camera
                this.camera.Stop();
                this.camera.Start();
            }
        }

        // Face Recognition Unchecked
        private void Face_Recognition_UNChencked(object sender, EventArgs e)
        {
            if (this.camera.Recognition)
            {
                this.camera.Recognition = false;
                try
                {
                    // Update DataBase this Camera Object field Face Detection 0
                    SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
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
                // Restart Camera
                this.camera.Stop();
                this.camera.Start();
            }
        }


        // Britness slider function
        private void Brightness_func(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int val = Convert.ToInt32(e.NewValue);
            brightness_label.Content = $"Brightness: {val}";
            this.camera.Brightness = val;
            // Save data to Database
            try
            {
                // Update DataBase this Camera Object field Face Detection 1
                SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                String query = $"UPDATE dbo.MyCameras SET Brightness='{val}' WHERE urls='{this.camera.url}' AND Name='{this.camera.name}'";
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


        // Contrast slider function
        private void Contrast_func(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int val = Convert.ToInt32(e.NewValue);
            contrast_label.Content = $"Contrast: {val}";
            this.camera.Contrast = val;
            // Save data to Database
            try
            {
                // Update DataBase this Camera Object field Face Detection 1
                SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                String query = $"UPDATE dbo.MyCameras SET Contrast='{val}' WHERE urls='{this.camera.url}' AND Name='{this.camera.name}'";
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

        // Darkness slider function
        private void Darkness_func(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int val = Convert.ToInt32(e.NewValue);
            darkness_label.Content = $"Darkness: {val}";
            this.camera.Darkness = val;
            // Save data to Database
            try
            {
                // Update DataBase this Camera Object field Face Detection 1
                SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                String query = $"UPDATE dbo.MyCameras SET Darkness='{val}' WHERE urls='{this.camera.url}' AND Name='{this.camera.name}'";
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

        // UP, DOWN, LEFT,RIGHT use Http request api to controll he camera
        private void GET_request(String url)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                String result = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine(result);
            }
            else
            {
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }
            client.Dispose();
        }

        private void UP_button_click(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("Mouse UP!");
            GET_request("URL_get rest UP");
        }

        private void DOWN_button_click(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("Mouse DOWN!");
            GET_request("URL_get rest DOWN");
        }

        private void LEFT_button_click(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("Mouse LEFT!");
            GET_request("URL_get rest LEFT");
        }

        private void RIGHT_button_click(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("Mouse RIGHT!");
            GET_request("URL_get rest RIGHT");
        }

        private void TAKE_PIC_button_click(object sender, MouseButtonEventArgs e)
        {
            this.camera.Take_pic();
        }

        // Start Recording is checked
        private void Start_REC_button_click(object sender, MouseButtonEventArgs e)
        {
            if (!this.camera.Recording)
            {
                this.camera.Recording = true;
                rec_label.Content = "Recording";
                rec_label.Foreground = Brushes.Red;
                // Update DataBase this Camera Object field Recording 1
                SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                String query = $"UPDATE dbo.myCameras SET Recording='{true}' WHERE urls='{this.camera.url}' AND Name='{this.camera.name}'";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
                // Restart Camera
                this.camera.Stop();
                this.camera.Start();
            }
        }

        // Stop Recording is Checked
        private void Stop_REC_button_click(object sender, MouseButtonEventArgs e)
        {
            if ( this.camera.Recording)
            {
                this.camera.Recording = false;
                rec_label.Content = "Stop Recording";
                rec_label.Foreground = Brushes.Gray;
                // Update DataBase this Camera Object field Recording 0
                SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                String query = $"UPDATE dbo.myCameras SET Recording='{false}' WHERE urls='{this.camera.url}' AND Name='{this.camera.name}'";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
                // Restart Camera
                this.camera.Stop();
                this.camera.Start();
            }
        }



        // SMS CheckBoxes
        private void Sms_chencked(object sender, EventArgs e)
        {
            if (!this.camera.On_move_sms)
            {
                this.camera.On_move_sms = true;
                // Update DataBase this Camera Object field On_Move_SMS 1
                SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                String query = $"UPDATE dbo.myCameras SET On_Move_SMS='{1}' WHERE urls='{this.camera.url}' AND Name='{this.camera.name}'";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
                // Restart Camera
                this.camera.Stop();
                this.camera.Start();
            }
        }
        private void Sms_unchencked(object sender, EventArgs e)
        {
            if (this.camera.On_move_sms)
            {
                this.camera.On_move_sms = false;
                // Update DataBase this Camera Object field On_Move_SMS 0
                SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                String query = $"UPDATE dbo.myCameras SET On_Move_SMS='{0}' WHERE urls='{this.camera.url}' AND Name='{this.camera.name}'";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
                // Restart Camera
                this.camera.Stop();
                this.camera.Start();
            }
        }

        // Email CheckBoxes
        private void Email_chencked(object sender, EventArgs e)
        {
            if (!this.camera.On_move_email)
            {
                this.camera.On_move_email = true;
                // Update DataBase this Camera Object field On_Move_EMAIL 1
                SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                String query = $"UPDATE dbo.myCameras SET On_Move_EMAIL='{1}' WHERE urls='{this.camera.url}' AND Name='{this.camera.name}'";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
                // Restart Camera
                this.camera.Stop();
                this.camera.Start();
            }
        }
        private void Email_unchencked(object sender, EventArgs e)
        {
            if (this.camera.On_move_email)
            {
                this.camera.On_move_email = false;
                // Update DataBase this Camera Object field On_Move_EMAIL 0
                SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                String query = $"UPDATE dbo.myCameras SET On_Move_EMAIL='{0}' WHERE urls='{this.camera.url}' AND Name='{this.camera.name}'";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
                // Restart Camera
                this.camera.Stop();
                this.camera.Start();
            }
        }

        // Picture Checkbox
        private void Pic_chencked(object sender, EventArgs e)
        {
            if (!this.camera.On_move_pic)
            {
                this.camera.On_move_pic = true;
                // Update DataBase this Camera Object field On_Move_Pic 1
                SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                String query = $"UPDATE dbo.myCameras SET On_Move_Pic='{1}' WHERE urls='{this.camera.url}' AND Name='{this.camera.name}'";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
                // Restart Camera
                this.camera.Stop();
                this.camera.Start();
            }
        }
        private void Pic_unchencked(object sender, EventArgs e)
        {
            if (this.camera.On_move_pic)
            {
                this.camera.On_move_pic = false;
                // Update DataBase this Camera Object field On_Move_Pic 0
                SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                String query = $"UPDATE dbo.myCameras SET On_Move_Pic='{0}' WHERE urls='{this.camera.url}' AND Name='{this.camera.name}'";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
                // Restart Camera
                this.camera.Stop();
                this.camera.Start();
            }
        }

        // Recording Checkbox
        private void Rec_chencked(object sender, EventArgs e)
        {
            if (!this.camera.On_move_rec)
            {
                this.camera.On_move_rec = true;
                // Update DataBase this Camera Object field On_Move_Rec 1
                SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                String query = $"UPDATE dbo.myCameras SET On_Move_Rec='{1}' WHERE urls='{this.camera.url}' AND Name='{this.camera.name}'";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
                // Restart Camera
                this.camera.Stop();
                this.camera.Start();
            }
        }
        private void Rec_unchencked(object sender, EventArgs e)
        {
            if (this.camera.On_move_rec)
            {
                this.camera.On_move_rec = false;
                // Update DataBase this Camera Object field On_Move_Rec 0
                SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                String query = $"UPDATE dbo.myCameras SET On_Move_Rec='{0}' WHERE urls='{this.camera.url}' AND Name='{this.camera.name}'";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                if (result < 0)
                    System.Windows.MessageBox.Show("Error inserting data into Database!");
                cn.Close();
                // Restart Camera
                this.camera.Stop();
                this.camera.Start();
            }
        }

        // Set The Sensitivity
        private void Sensitivity_func(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                // Update Cameras Move_Sensitivity
                int val = Convert.ToInt32(e.NewValue);
                sensitivity_value_label.Content = $"{val}";
                if (this.camera != null)
                {
                    this.camera.On_move_sensitivity = val;
                    Console.WriteLine(this.camera.On_move_sensitivity.ToString());
                    // Update DataBases Move_Sensitivity
                    SqlConnection cn = new SqlConnection(Camera.DB_connection_string);
                    String query = $"UPDATE dbo.myCameras SET Move_Sensitivity='{this.camera.On_move_sensitivity}' WHERE urls='{this.camera.url}' AND Name='{this.camera.name}'";
                    SqlCommand cmd = new SqlCommand(query, cn);
                    cn.Open();
                    int result = cmd.ExecuteNonQuery();
                    if (result < 0)
                        System.Windows.MessageBox.Show("Error inserting data into Database!");
                    cn.Close();
                }
            }
            catch (System.Data.SqlClient.SqlException) { }
            catch (System.NullReferenceException) { }
        }

        // Remote Camera Resolution
        private void Resolution_combobox_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (this.url != "")
            {
                ComboBox cmb = sender as ComboBox;
                String selection = cmb.SelectedValue.ToString();
                String order = "";
                if (selection.Contains("QQVGA(160X120)"))
                {
                    order = "0";
                } else if (selection.Contains("HQVGA(240X176)"))
                {
                    order = "3";
                }
                else if (selection.Contains("QVGA(320X240)"))
                {
                    order = "4";
                }
                else if (selection.Contains("CIF(400X296)"))
                {
                    order = "5";
                }
                else if (selection.Contains("VGA(640X480)"))
                {
                    order = "6";
                }
                else if (selection.Contains("SVGA(800X600)"))
                {
                    order = "7";
                }
                else if (selection.Contains("XGA(1024X768)"))
                {
                    order = "8";
                }
                else if (selection.Contains("SXGA(1280X1024)"))
                {
                    order = "9";
                }
                else if (selection.Contains("UXGA(1600X1200)"))
                {
                    order = "10";
                }
                if (!selection.Contains("Change"))
                {
                    try
                    {
                        this.camera.Stop();
                        // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                        // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                        //Console.WriteLine("Old Url: " + this.url);
                        int found = this.url.IndexOf(":81");
                        String ur_l = this.url.Substring(0, found); // = http://192.168.1.50/
                        ur_l += "/control?var=framesize&val=" + order;
                        //Console.WriteLine("New Url: " + ur_l);
                        HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                        request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            if (response.StatusCode.ToString().Equals("OK"))
                            {
                                Thread.Sleep(2000);
                                this.camera.Start();
                            }
                        }
                    } catch(Exception er)
                    {
                        System.Windows.MessageBox.Show("[ERROR] " + er.Message);
                    }
                }
            }
        }

        
        // When Cameras Quality Changed
        private void Quality_changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int val = Convert.ToInt16(e.NewValue);
            if (this.camera != null)
            {
                this.camera.Stop();
                // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                // http://192.168.1.50/control?var=quality&val=10
                int found = this.url.IndexOf(":81");
                String ur_l = this.url.Substring(0, found); // = http://192.168.1.50/
                ur_l += "/control?var=quality&val=" + val;
                //Console.WriteLine("New Url: " + ur_l);
                HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode.ToString().Equals("OK"))
                    {
                        this.camera.Start();
                        //MainWindow.RestartApp();
                    }
                }
            }
        }

        // Remote Cameras Brightness
        private void Brightness_changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int val = Convert.ToInt16(e.NewValue);
            if (this.camera != null)
            {
                this.camera.Stop();
                // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                // http://192.168.1.50/control?var=quality&val=10
                int found = this.url.IndexOf(":81");
                String ur_l = this.url.Substring(0, found); // = http://192.168.1.50/
                ur_l += "/control?var=brightness&val=" + val;
                //Console.WriteLine("New Url: " + ur_l);
                HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode.ToString().Equals("OK"))
                    {
                        this.camera.Start();
                        //MainWindow.RestartApp();
                    }
                }
            }
        }

        // Remote Cameras Contrast
        private void Contrast_changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int val = Convert.ToInt16(e.NewValue);
            if (this.camera != null)
            {
                this.camera.Stop();
                // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                // http://192.168.1.50/control?var=quality&val=10
                int found = this.url.IndexOf(":81");
                String ur_l = this.url.Substring(0, found); // = http://192.168.1.50/
                ur_l += "/control?var=contrast&val=" + val;
                //Console.WriteLine("New Url: " + ur_l);
                HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode.ToString().Equals("OK"))
                    {
                        this.camera.Start();
                        //MainWindow.RestartApp();
                    }
                }
            }
        }

        // Remote Cameras Saturasion
        private void Saturation_changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int val = Convert.ToInt32(e.NewValue);
            if (this.camera != null)
            {
                this.camera.Stop();
                // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                // http://192.168.1.50/control?var=quality&val=10
                int found = this.url.IndexOf(":81");
                String ur_l = this.url.Substring(0, found); // = http://192.168.1.50/
                ur_l += "/control?var=saturation&val=" + val;
                //Console.WriteLine("New Url: " + ur_l);
                HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode.ToString().Equals("OK"))
                    {
                        this.camera.Start();
                        //MainWindow.RestartApp();
                    }
                }
            }
        }

        // Remote Camera Specialeffect
        private void Specialeffect_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (this.url != "")
            {
                ComboBox cmb = sender as ComboBox;
                String selection = cmb.SelectedValue.ToString();
                String order = "";
                if (selection.Contains("No Effect"))
                {
                    order = "0";
                }
                else if (selection.Contains("Negative"))
                {
                    order = "1";
                }
                else if (selection.Contains("Grayscale"))
                {
                    order = "2";
                }
                else if (selection.Contains("Red Tint"))
                {
                    order = "3";
                }
                else if (selection.Contains("Green Tint"))
                {
                    order = "4";
                }
                else if (selection.Contains("Blue Tint"))
                {
                    order = "5";
                }
                else if (selection.Contains("Sepia"))
                {
                    order = "6";
                }
                try
                {
                    this.camera.Stop();
                    // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                    // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                    //Console.WriteLine("Old Url: " + this.url);
                    int found = this.url.IndexOf(":81");
                    String ur_l = this.url.Substring(0, found); // = http://192.168.1.50/
                    ur_l += "/control?var=special_effect&val=" + order;
                    //Console.WriteLine("New Url: " + ur_l);
                    HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                    request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        if (response.StatusCode.ToString().Equals("OK"))
                        {
                            this.camera.Start();
                            //MainWindow.RestartApp();
                        }
                    }
                } catch(Exception)
                {

                }
            }
        }

        // Remotes Camera AWB
        private void AWB_checkbox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox c = sender as CheckBox;
            if (c.IsChecked.Value)
            {
                this.camera.Stop();
                // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                //Console.WriteLine("Old Url: " + this.url);
                int found = this.url.IndexOf(":81");
                String ur_l = this.url.Substring(0, found); // = http://192.168.1.50/
                ur_l += "/control?var=awb&val=1";
                //Console.WriteLine("New Url: " + ur_l);
                HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode.ToString().Equals("OK"))
                    {
                        this.camera.Start();
                        //MainWindow.RestartApp();
                    }
                }
            } else
            {
                this.camera.Stop();
                // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                //Console.WriteLine("Old Url: " + this.url);
                int found = this.url.IndexOf(":81");
                String ur_l = this.url.Substring(0, found); // = http://192.168.1.50/
                ur_l += "/control?var=awb&val=0";
                //Console.WriteLine("New Url: " + ur_l);
                HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode.ToString().Equals("OK"))
                    {
                        this.camera.Start();
                        //MainWindow.RestartApp();
                    }
                }
            }
        }

        // Remotes Camera AWB GAIN
        private void AWB_GAIN_checkbox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox c = sender as CheckBox;
            if (c.IsChecked.Value)
            {
                this.camera.Stop();
                // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                //Console.WriteLine("Old Url: " + this.url);
                int found = this.url.IndexOf(":81");
                String ur_l = this.url.Substring(0, found); // = http://192.168.1.50/
                ur_l += "/control?var=awb_gain&val=1";
                //Console.WriteLine("New Url: " + ur_l);
                HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode.ToString().Equals("OK"))
                    {
                        this.camera.Start();
                        //MainWindow.RestartApp();
                    }
                }
            }
            else
            {
                this.camera.Stop();
                // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                //Console.WriteLine("Old Url: " + this.url);
                int found = this.url.IndexOf(":81");
                String ur_l = this.url.Substring(0, found); // = http://192.168.1.50/
                ur_l += "/control?var=awb_gain&val=0";
                //Console.WriteLine("New Url: " + ur_l);
                HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode.ToString().Equals("OK"))
                    {
                        this.camera.Start();
                        //MainWindow.RestartApp();
                    }
                }
            }
        }

        // Remotes Camera WB Mode
        private void WB_MODE_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (this.url != "")
            {
                ComboBox cmb = sender as ComboBox;
                String selection = cmb.SelectedValue.ToString();
                String order = "";
                if (selection.Contains("Auto"))
                {
                    order = "0";
                }
                else if (selection.Contains("Sunny"))
                {
                    order = "1";
                }
                else if (selection.Contains("Cloudy"))
                {
                    order = "2";
                }
                else if (selection.Contains("Office"))
                {
                    order = "3";
                }
                else if (selection.Contains("Home"))
                {
                    order = "4";
                }
                this.camera.Stop();
                // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                //Console.WriteLine("Old Url: " + this.url);
                int found = this.url.IndexOf(":81");
                String ur_l = this.url.Substring(0, found); // = http://192.168.1.50/
                ur_l += "/control?var=wb_mode&val=" + order;
                //Console.WriteLine("New Url: " + ur_l);
                HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode.ToString().Equals("OK"))
                    {
                        this.camera.Start();
                        //MainWindow.RestartApp();
                    }
                }
            }
        }

        // Remotes Camera AEC Sensor
        private void AEC_SENSOR_checkbox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox c = sender as CheckBox;
            if (c.IsChecked.Value)
            {
                this.camera.Stop();
                // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                //Console.WriteLine("Old Url: " + this.url);
                int found = this.url.IndexOf(":81");
                String ur_l = this.url.Substring(0, found); // = http://192.168.1.50/
                ur_l += "/control?var=aec&val=1";
                //Console.WriteLine("New Url: " + ur_l);
                HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode.ToString().Equals("OK"))
                    {
                        this.camera.Start();
                        //MainWindow.RestartApp();
                    }
                }
            }
            else
            {
                this.camera.Stop();
                // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                //Console.WriteLine("Old Url: " + this.url);
                int found = this.url.IndexOf(":81");
                String ur_l = this.url.Substring(0, found); // = http://192.168.1.50/
                ur_l += "/control?var=aec&val=0";
                //Console.WriteLine("New Url: " + ur_l);
                HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode.ToString().Equals("OK"))
                    {
                        this.camera.Start();
                        //MainWindow.RestartApp();
                    }
                }
            }
        }

        // Remotes Camera AEC DSP SensorSensor
        private void AEC_DSP_checkbox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox c = sender as CheckBox;
            if (c.IsChecked.Value)
            {
                this.camera.Stop();
                // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                //Console.WriteLine("Old Url: " + this.url);
                int found = this.url.IndexOf(":81");
                String ur_l = this.url.Substring(0, found); // = http://192.168.1.50/
                ur_l += "/control?var=aec2&val=1";
                //Console.WriteLine("New Url: " + ur_l);
                HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode.ToString().Equals("OK"))
                    {
                        this.camera.Start();
                        //MainWindow.RestartApp();
                    }
                }
            }
            else
            {
                this.camera.Stop();
                // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                //Console.WriteLine("Old Url: " + this.url);
                int found = this.url.IndexOf(":81");
                String ur_l = this.url.Substring(0, found); // = http://192.168.1.50/
                ur_l += "/control?var=aec2&val=0";
                //Console.WriteLine("New Url: " + ur_l);
                HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode.ToString().Equals("OK"))
                    {
                        this.camera.Start();
                        //MainWindow.RestartApp();
                    }
                }
            }
        }

        // AE LEVEL Changed
        private void AE_LEVEL_changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int val = Convert.ToInt32(e.NewValue);
            if (this.camera != null)
            {
                this.camera.Stop();
                // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                // http://192.168.1.50/control?var=quality&val=10
                int found = this.url.IndexOf(":81");
                String ur_l = this.url.Substring(0, found); // = http://192.168.1.50/
                ur_l += "/control?var=ae_level&val=" + val;
                //Console.WriteLine("New Url: " + ur_l);
                HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode.ToString().Equals("OK"))
                    {
                        this.camera.Start();
                        //MainWindow.RestartApp();
                    }
                }
            }
        }

        // AGC Checked
        private void AGC_checkbox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox c = sender as CheckBox;
            if (c.IsChecked.Value)
            {
                this.camera.Stop();
                // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                //Console.WriteLine("Old Url: " + this.url);
                int found = this.url.IndexOf(":81");
                String ur_l = this.url.Substring(0, found); // = http://192.168.1.50/
                ur_l += "/control?var=agc&val=1";
                //Console.WriteLine("New Url: " + ur_l);
                HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode.ToString().Equals("OK"))
                    {
                        this.camera.Start();
                        //MainWindow.RestartApp();
                    }
                }
            }
            else
            {
                this.camera.Stop();
                // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                //Console.WriteLine("Old Url: " + this.url);
                int found = this.url.IndexOf(":81");
                String ur_l = this.url.Substring(0, found); // = http://192.168.1.50/
                ur_l += "/control?var=agc&val=0";
                //Console.WriteLine("New Url: " + ur_l);
                HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode.ToString().Equals("OK"))
                    {
                        this.camera.Start();
                        //MainWindow.RestartApp();
                    }
                }
            }
        }

        // GAIN CEILING Changed
        private void GAIN_CEILINGL_changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int val = Convert.ToInt32(e.NewValue);
            if (this.camera != null)
            {
                this.camera.Stop();
                // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                // http://192.168.1.50/control?var=quality&val=10
                int found = this.url.IndexOf(":81");
                String ur_l = this.url.Substring(0, found); // = http://192.168.1.50/
                ur_l += "/control?var=agc_gain&val=" + val;
                //Console.WriteLine("New Url: " + ur_l);
                HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode.ToString().Equals("OK"))
                    {
                        this.camera.Start();
                        //MainWindow.RestartApp();
                    }
                }
            }
        }

        // BPC Changed
        private void BPC_checkbox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox c = sender as CheckBox;
            if (c.IsChecked.Value)
            {
                this.camera.Stop();
                // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                //Console.WriteLine("Old Url: " + this.url);
                int found = this.url.IndexOf(":81");
                String ur_l = this.url.Substring(0, found); // = http://192.168.1.50/
                ur_l += "/control?var=bpc&val=1";
                //Console.WriteLine("New Url: " + ur_l);
                HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode.ToString().Equals("OK"))
                    {
                        this.camera.Start();
                        //MainWindow.RestartApp();
                    }
                }
            }
            else
            {
                this.camera.Stop();
                // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                //Console.WriteLine("Old Url: " + this.url);
                int found = this.url.IndexOf(":81");
                String ur_l = this.url.Substring(0, found); // = http://192.168.1.50/
                ur_l += "/control?var=bpc&val=0";
                //Console.WriteLine("New Url: " + ur_l);
                HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode.ToString().Equals("OK"))
                    {
                        this.camera.Start();
                        //MainWindow.RestartApp();
                    }
                }
            }
        }
    }


}
