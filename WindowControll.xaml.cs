using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Net.Http;
using System.Net;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using System.Linq;

namespace IPCamera
{
    /// <summary>
    /// Interaction logic for WindowControll.xaml
    /// </summary>
    public partial class WindowControll : Window
    {
        public MyCamera Camera { get; set; }
        public String Url { get; set; }
        public bool Remote_start_setup { get; set; }
        public bool Remote_detection { get; set; }
        public bool Remote_recognition { get; set; }
        String _cam_resolution_order = "";
        public WindowControll(MyCamera cam)
        {
            try
            {
                InitializeComponent();
            } catch (Exception ex)
            {
                Console.WriteLine($"Source:{ex.Source}\nStackTrace:{ex.StackTrace}\n{ex.Message}");
            }
            this.Remote_start_setup = false;
            this.Remote_detection = false;
            this.DataContext = this;
            this.Remote_recognition = false;
            // Setup this_camera
            this.Camera = cam;
            this.Url = this.Camera.urls;
            // Add Title
            cameras_title.Content = this.Camera.name;
            // Chech if Face_Recognition, Face Detection  is checked
            Face_det.IsChecked = (this.Camera.Face_Detection);
            Face_rec.IsChecked = (this.Camera.Face_Recognition);
            // Setup Brightness and Contrast Labels and Sliders
            brightness_label.Content = $"Brightness: {this.Camera.Brightness}";
            contrast_label.Content   = $"Contrast:   {this.Camera.Contrast}";
            darkness_label.Content   = $"Darkness:   {this.Camera.Darkness}";
            brightness_slider.Value  = this.Camera.Brightness;
            contrast_slider.Value    = this.Camera.Contrast;
            darkness_slider.Value    = this.Camera.Darkness;
            sensitivity_value_label.Content = $"{this.Camera.Move_Sensitivity}";
            sensitivity_slider.Value = this.Camera.Move_Sensitivity;
            // Setup recording Button
            if (this.Camera.Recording)
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
            sms_checkbox.IsChecked = (this.Camera.On_Move_SMS);
            email_checkbox.IsChecked = (this.Camera.On_Move_EMAIL);
            pic_checkbox.IsChecked = (this.Camera.On_Move_Pic);
            rec_checkbox.IsChecked = (this.Camera.On_Move_Rec);
            // Setuo Network streaming Settings
            network_streaming_checkbox.IsChecked = this.Camera.net_stream;
            network_streaming_port.Text = Convert.ToString(this.Camera.net_stream_port);
            network_streaming_prefix.Text = this.Camera.net_stream_prefix;
            // Setup Remotes Cameras Settisng
            if(this.Camera.isEsp32)
            {
                Update_remote_cameras_status();
            } else
            {
                remote_resolution.IsEnabled = false;
                cameras_quality_slider.IsEnabled = false;
                cameras_brightness_slider.IsEnabled = false;
                cameras_contrast_slider.IsEnabled = false;
                cameras_saturation_slider.IsEnabled = false;
                remote_specialeffect.IsEnabled = false;
                cameras_awb_checkbox.IsEnabled = false;
                cameras_awb_gain_checkbox.IsEnabled = false;
                remote_wb_mode.IsEnabled = false;
                cameras_aec_sensor_checkbox.IsEnabled = false;
                cameras_aec_dsp_checkbox.IsEnabled = false;
                cameras_ae_level_slider.IsEnabled = false;
                cameras_agc_checkbox.IsEnabled = false;
                cameras_gain_ceiling_slider.IsEnabled = false;
                cameras_bpc_checkbox.IsEnabled = false;
                cameras_wpc_checkbox.IsEnabled = false;
                cameras_raw_gma_checkbox.IsEnabled = false;
                cameras_lens_correction_checkbox.IsEnabled = false;
                cameras_h_mirror_checkbox.IsEnabled = false;
                cameras_v_flip_checkbox.IsEnabled = false;
                cameras_dcw_downsize_en_checkbox.IsEnabled = false;
                cameras_color_bar_checkbox.IsEnabled = false;
                cameras_face_detection_checkbox.IsEnabled = false;
                cameras_face_recognition_checkbox.IsEnabled = false;
                cameras_get_still_button.IsEnabled = false;
                cameras_start_stream_button.IsEnabled = false;
                cameras_enroll_face_button.IsEnabled = false;
                cameras_reboot_button.IsEnabled = false;
                cameras_hostpot_button.IsEnabled = false;
            }
        }
        private void X_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Update_remote_cameras_status()
        {
            try
            {
                // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                //Console.WriteLine("Old Url: " + this.Url);
                int found = this.Url.IndexOf(":81");
                String ur_l = this.Url.Substring(0, found); // = http://192.168.1.50/
                ur_l += "/status?username=" + this.Camera.username + "&password=" + this.Camera.password;
                Console.WriteLine("New Url: " + ur_l);
                HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode.ToString().Equals("OK"))
                    {
                        Stream ReceiveStream = response.GetResponseStream();
                        StreamReader reader = new StreamReader(ReceiveStream);
                        string responseFromServer = reader.ReadToEnd();
                        dynamic data = JObject.Parse(responseFromServer);

                        // Set Remote Rezolution
                        String val = Convert.ToString(data.framesize).Trim();
                        _cam_resolution_order = val;
                        switch (val)
                        {
                            case "0": this.remote_resolution.SelectedIndex = 8; break;
                            case "3": this.remote_resolution.SelectedIndex = 7; break;
                            case "4": this.remote_resolution.SelectedIndex = 6; break;
                            case "5": this.remote_resolution.SelectedIndex = 5; break;
                            case "6": this.remote_resolution.SelectedIndex = 4; break;
                            case "7": this.remote_resolution.SelectedIndex = 3; break;
                            case "8": this.remote_resolution.SelectedIndex = 2; break;
                            case "9": this.remote_resolution.SelectedIndex = 1; break;
                            case "10": this.remote_resolution.SelectedIndex = 0; break;
                        };
                        // Set remote Quality 10 / 63
                        val = Convert.ToString(data.quality).Trim();
                        cameras_quality_slider.Value = Convert.ToDouble(val);
                        // Set remote Britness  -2 / 2
                        val = Convert.ToString(data.brightness).Trim();
                        cameras_brightness_slider.Value = Convert.ToDouble(val);
                        // Set remote Contrast  -2 / 2
                        val = Convert.ToString(data.contrast).Trim();
                        cameras_contrast_slider.Value = Convert.ToDouble(val);
                        // Set remote Saturation  -2 / 2
                        val = Convert.ToString(data.saturation).Trim();
                        cameras_saturation_slider.Value = Convert.ToDouble(val);
                        // Set Remote Special Effect
                        val = Convert.ToString(data.special_effect).Trim();
                        switch (val)
                        {
                            case "0": this.remote_specialeffect.SelectedIndex = 0; break;
                            case "1": this.remote_specialeffect.SelectedIndex = 1; break;
                            case "2": this.remote_specialeffect.SelectedIndex = 2; break;
                            case "3": this.remote_specialeffect.SelectedIndex = 3; break;
                            case "4": this.remote_specialeffect.SelectedIndex = 4; break;
                            case "5": this.remote_specialeffect.SelectedIndex = 5; break;
                            case "6": this.remote_specialeffect.SelectedIndex = 6; break;
                        };
                        // Set remote AWB  0/1
                        val = Convert.ToString(data.awb).Trim();
                        cameras_awb_checkbox.IsChecked = val == "1";
                        // Set remote AWB_Gain  0/1
                        val = Convert.ToString(data.awb_gain).Trim();
                        cameras_awb_gain_checkbox.IsChecked = val == "1";
                        // Set Remote Rezolution
                        val = Convert.ToString(data.wb_mode).Trim();
                        switch (val)
                        {
                            case "0": this.remote_wb_mode.SelectedIndex = 0; break;
                            case "1": this.remote_wb_mode.SelectedIndex = 1; break;
                            case "2": this.remote_wb_mode.SelectedIndex = 2; break;
                            case "3": this.remote_wb_mode.SelectedIndex = 3; break;
                            case "4": this.remote_wb_mode.SelectedIndex = 4; break;
                        };
                        // Set remote AEC  0/1
                        val = Convert.ToString(data.aec).Trim();
                        cameras_aec_sensor_checkbox.IsChecked = val == "1";
                        // Set remote AEC dsp  0/1
                        val = Convert.ToString(data.aec2).Trim();
                        cameras_aec_dsp_checkbox.IsChecked = val == "1";
                        // Set remote AE Level  -2 / 2
                        val = Convert.ToString(data.ae_level).Trim();
                        cameras_ae_level_slider.Value = Convert.ToDouble(val);
                        // Set remote AGC 0/1
                        val = Convert.ToString(data.agc).Trim();
                        cameras_agc_checkbox.IsChecked = val == "1";
                        // Set remote Gain Ceiling -2 / 2
                        val = Convert.ToString(data.gainceiling).Trim();
                        cameras_gain_ceiling_slider.Value = Convert.ToDouble(val);
                        // Set remote BPC 0/1
                        val = Convert.ToString(data.bpc).Trim();
                        cameras_bpc_checkbox.IsChecked = val == "1";
                        // Set remote WPC 0/1
                        val = Convert.ToString(data.wpc).Trim();
                        cameras_wpc_checkbox.IsChecked = val == "1";
                        // Set remote RAW GMA 0/1
                        val = Convert.ToString(data.raw_gma).Trim();
                        cameras_raw_gma_checkbox.IsChecked = val == "1";
                        // Set remote LENS Corection 0/1
                        val = Convert.ToString(data.lenc).Trim();
                        cameras_lens_correction_checkbox.IsChecked = val == "1";
                        // Set remote H Mirror 0/1
                        val = Convert.ToString(data.hmirror).Trim();
                        cameras_h_mirror_checkbox.IsChecked = val == "1";
                        // Set remote H Mirror 0/1
                        val = Convert.ToString(data.vflip).Trim();
                        cameras_v_flip_checkbox.IsChecked = val == "1";
                        // Set remote DCW 0/1
                        val = Convert.ToString(data.dcw).Trim();
                        cameras_dcw_downsize_en_checkbox.IsChecked = val == "1";
                        // Set Color Bar 0/1
                        val = Convert.ToString(data.colorbar).Trim();
                        cameras_color_bar_checkbox.IsChecked = val == "1";
                        // Set Face Detection 0/1
                        val = Convert.ToString(data.face_detect).Trim();
                        cameras_face_detection_checkbox.IsChecked = val == "1";
                        this.Remote_detection = val == "1";
                        // Set Face Recognition 0/1
                        val = Convert.ToString(data.face_recognize).Trim();
                        cameras_face_recognition_checkbox.IsChecked = val == "1";
                        this.Remote_recognition = val == "1";
                        this.Remote_start_setup = true;
                    }
                }
            }
            catch (System.Net.WebException ex)
            {
                Console.WriteLine($"Source:{ex.Source}\nStackTrace:{ex.StackTrace}\n{ex.Message}");
            }
            catch (System.ArgumentOutOfRangeException ex)
            {
                Console.WriteLine($"Source:{ex.Source}\nStackTrace:{ex.StackTrace}\n{ex.Message}");
            }
        }
        protected override void OnClosed(EventArgs e)
        {
            this.Close();
        }
        private void Face_Detection_Chencked(object sender, EventArgs e)
        {
            if (!this.Camera.Face_Detection)
            {
                this.Camera.Face_Detection = true;
                MainWindow.Main_window.DBModels.SaveChanges();
                CameraServcies cs = (from s in MainWindow.Main_window.camerasServicies where s.cameraId == this.Camera.Id select s).FirstOrDefault();
                cs.Stop();
                cs.Start();
            }
        }
        private void Face_Detection_UNChencked(object sender, EventArgs e)
        {
            if (this.Camera.Face_Detection)
            {
                this.Camera.Face_Detection = false;
                if (this.Camera.Face_Recognition)
                {
                    this.Camera.Face_Recognition = false;
                    Face_rec.IsChecked = (this.Camera.Face_Recognition);
                }
                MainWindow.Main_window.DBModels.SaveChanges();
                CameraServcies cs = (from s in MainWindow.Main_window.camerasServicies where s.cameraId == this.Camera.Id select s).FirstOrDefault();
                cs.Stop();
                cs.Start();
            }
        }
        private void Face_Recognition_Chencked(object sender, EventArgs e)
        {
            if (!this.Camera.Face_Recognition)
            {
                this.Camera.Face_Recognition = true;
                if (!this.Camera.Face_Detection)
                {
                    this.Camera.Face_Detection = true;
                    Face_det.IsChecked = (this.Camera.Face_Detection);
                }
                MainWindow.Main_window.DBModels.SaveChanges();
                CameraServcies cs = (from s in MainWindow.Main_window.camerasServicies where s.cameraId == this.Camera.Id select s).FirstOrDefault();
                cs.Stop();
                cs.Start();
            }
        }
        private void Face_Recognition_UNChencked(object sender, EventArgs e)
        {
            if (this.Camera.Face_Recognition)
            {
                this.Camera.Face_Recognition = false;
                MainWindow.Main_window.DBModels.SaveChanges();
                CameraServcies cs = (from s in MainWindow.Main_window.camerasServicies where s.cameraId == this.Camera.Id select s).FirstOrDefault();
                cs.Stop();
                cs.Start();
            }
        }
        private void Brightness_func(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int val = Convert.ToInt32(e.NewValue);
            brightness_label.Content = $"Brightness: {val}";
            this.Camera.Brightness = val;
        }
        private void Contrast_func(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int val = Convert.ToInt32(e.NewValue);
            contrast_label.Content = $"Contrast: {val}";
            this.Camera.Contrast = val;
        }
        private void Darkness_func(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int val = Convert.ToInt32(e.NewValue);
            darkness_label.Content = $"Darkness: {val}";
            this.Camera.Darkness = val;
        }
        private void GET_request(String url)
        {
            try
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
            catch(System.InvalidOperationException ex)
            {
                Console.WriteLine($"Source:{ex.Source}\nStackTrace:{ex.StackTrace}\n{ex.Message}");
            }
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
            CameraServcies cs = (from s in MainWindow.Main_window.camerasServicies where s.cameraId == this.Camera.Id select s).FirstOrDefault();
            cs.Take_pic();
        }
        private void Start_REC_button_click(object sender, MouseButtonEventArgs e)
        {
            if (!this.Camera.Recording)
            {
                this.Camera.Recording = true;
                // Setup Field
                rec_label.Content = "Recording";
                rec_label.Foreground = Brushes.Red;
                this.Camera.Recording = true;
                MainWindow.Main_window.DBModels.SaveChanges();
            }
        }
        private void Stop_REC_button_click(object sender, MouseButtonEventArgs e)
        {
            if (this.Camera.Recording)
            {
                this.Camera.Recording = false;
                // Setup Field
                rec_label.Content = "Stop Recording";
                rec_label.Foreground = Brushes.Gray;
                this.Camera.Recording = false;
                MainWindow.Main_window.DBModels.SaveChanges();
            }
        }
        private void Sms_chencked(object sender, EventArgs e)
        {
            if (!this.Camera.On_Move_SMS)
            {
                this.Camera.On_Move_SMS = true;
            }
        }
        private void Sms_unchencked(object sender, EventArgs e)
        {
            if (this.Camera.On_Move_SMS)
            {
                this.Camera.On_Move_SMS = false;
            }
        }
        private void Email_chencked(object sender, EventArgs e)
        {
            if (!this.Camera.On_Move_EMAIL)
            {
                this.Camera.On_Move_EMAIL = true;
            }
        }
        private void Email_unchencked(object sender, EventArgs e)
        {
            if (this.Camera.On_Move_EMAIL)
            {
                this.Camera.On_Move_EMAIL = false;
            }
        }
        private void Pic_chencked(object sender, EventArgs e)
        {
            if (!this.Camera.On_Move_Pic)
            {
                this.Camera.On_Move_Pic = true;
            }
        }
        private void Pic_unchencked(object sender, EventArgs e)
        {
            if (this.Camera.On_Move_Pic)
            {
                this.Camera.On_Move_Pic = false;
            }
        }
        private void Rec_chencked(object sender, EventArgs e)
        {
            if (!this.Camera.On_Move_Rec)
            {
                this.Camera.On_Move_Rec = true;
            }
        }
        private void Rec_unchencked(object sender, EventArgs e)
        {
            if (this.Camera.On_Move_Rec)
            {
                this.Camera.On_Move_Rec = false;
            }
        }
        private void Sensitivity_func(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.Camera != null)
            {
                // Update Cameras Move_Sensitivity
                int val = Convert.ToInt32(e.NewValue);
                sensitivity_value_label.Content = $"{val}";
                this.Camera.Move_Sensitivity = val;
            }
        }
        private void Resolution_combobox_Changed(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (this.Remote_start_setup)
                {
                    CameraServcies cs = (from s in MainWindow.Main_window.camerasServicies where s.cameraId == this.Camera.Id select s).FirstOrDefault();
                    cs.Stop();
                    ComboBox cmb = sender as ComboBox;
                    String selection = cmb.SelectedValue.ToString();
                    if (selection.Contains("QQVGA(160X120)"))
                    {
                        _cam_resolution_order = "0";
                    } else if (selection.Contains("HQVGA(240X176)"))
                    {
                        _cam_resolution_order = "3";
                    }
                    else if (selection.Contains("QVGA(320X240)"))
                    {
                        _cam_resolution_order = "4";
                    }
                    else if (selection.Contains("CIF(400X296)"))
                    {
                        _cam_resolution_order = "5";
                    }
                    else if (selection.Contains("VGA(640X480)"))
                    {
                        _cam_resolution_order = "6";
                    }
                    else if (selection.Contains("SVGA(800X600)"))
                    {
                        _cam_resolution_order = "7";
                    }
                    else if (selection.Contains("XGA(1024X768)"))
                    {
                        _cam_resolution_order = "8";
                    }
                    else if (selection.Contains("SXGA(1280X1024)"))
                    {
                        _cam_resolution_order = "9";
                    }
                    else if (selection.Contains("UXGA(1600X1200)"))
                    {
                        _cam_resolution_order = "10";
                    }
                    if (!selection.Contains("Change"))
                    {
                        try
                        {
                            // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                            // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                            //Console.WriteLine("Old Url: " + this.Url);
                            int found = this.Url.IndexOf(":81");
                            String ur_l = this.Url.Substring(0, found); // = http://192.168.1.50/
                            ur_l += "/control?var=framesize&val=" + _cam_resolution_order;
                            //Console.WriteLine("New Url: " + ur_l);
                            HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                            request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                            {
                                if (response.StatusCode.ToString().Equals("OK"))
                                {
                                    Thread.Sleep(5000);
                                    cs.Start();
                                }
                            }
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine($"Source:{ex.Source}\nStackTrace:{ex.StackTrace}\n{ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Source:{ex.Source}\n{ex.Message}");
            }
        }
        private void Quality_changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                int val = Convert.ToInt16(e.NewValue);
                if (this.Remote_start_setup)
                {
                    CameraServcies cs = (from s in MainWindow.Main_window.camerasServicies where s.cameraId == this.Camera.Id select s).FirstOrDefault();
                    cs.Stop();
                    // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                    // http://192.168.1.50/control?var=quality&val=10
                    int found = this.Url.IndexOf(":81");
                    String ur_l = this.Url.Substring(0, found); // = http://192.168.1.50/
                    ur_l += "/control?var=quality&val=" + val;
                    //Console.WriteLine("New Url: " + ur_l);
                    HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                    request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        if (response.StatusCode.ToString().Equals("OK"))
                        {
                            cs.Start();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Source:{ex.Source}\n{ex.Message}");
            }
        }
        private void Brightness_changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                int val = Convert.ToInt16(e.NewValue);
                if (this.Remote_start_setup)
                {
                    CameraServcies cs = (from s in MainWindow.Main_window.camerasServicies where s.cameraId == this.Camera.Id select s).FirstOrDefault();
                    cs.Stop();
                    // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                    // http://192.168.1.50/control?var=quality&val=10
                    int found = this.Url.IndexOf(":81");
                    String ur_l = this.Url.Substring(0, found); // = http://192.168.1.50/
                    ur_l += "/control?var=brightness&val=" + val;
                    //Console.WriteLine("New Url: " + ur_l);
                    HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                    request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        if (response.StatusCode.ToString().Equals("OK"))
                        {
                            cs.Start();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Source:{ex.Source}\n{ex.Message}");
            }
        }
        private void Contrast_changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                int val = Convert.ToInt16(e.NewValue);
                if (this.Remote_start_setup)
                {
                    CameraServcies cs = (from s in MainWindow.Main_window.camerasServicies where s.cameraId == this.Camera.Id select s).FirstOrDefault();
                    cs.Stop();
                    // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                    // http://192.168.1.50/control?var=quality&val=10
                    int found = this.Url.IndexOf(":81");
                    String ur_l = this.Url.Substring(0, found); // = http://192.168.1.50/
                    ur_l += "/control?var=contrast&val=" + val;
                    //Console.WriteLine("New Url: " + ur_l);
                    HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                    request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        if (response.StatusCode.ToString().Equals("OK"))
                        {
                            cs.Start();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Source:{ex.Source}\n{ex.Message}");
            }
        }
        private void Saturation_changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                int val = Convert.ToInt32(e.NewValue);
                if (this.Remote_start_setup)
                {
                    CameraServcies cs = (from s in MainWindow.Main_window.camerasServicies where s.cameraId == this.Camera.Id select s).FirstOrDefault();
                    cs.Stop();
                    // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                    // http://192.168.1.50/control?var=quality&val=10
                    int found = this.Url.IndexOf(":81");
                    String ur_l = this.Url.Substring(0, found); // = http://192.168.1.50/
                    ur_l += "/control?var=saturation&val=" + val;
                    //Console.WriteLine("New Url: " + ur_l);
                    HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                    request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        if (response.StatusCode.ToString().Equals("OK"))
                        {
                            cs.Start();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Source:{ex.Source}\n{ex.Message}");
            }
        }
        private void Specialeffect_Changed(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (this.Remote_start_setup)
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
                        CameraServcies cs = (from s in MainWindow.Main_window.camerasServicies where s.cameraId == this.Camera.Id select s).FirstOrDefault();
                        cs.Stop();
                        // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                        // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                        //Console.WriteLine("Old Url: " + this.Url);
                        int found = this.Url.IndexOf(":81");
                        String ur_l = this.Url.Substring(0, found); // = http://192.168.1.50/
                        ur_l += "/control?var=special_effect&val=" + order;
                        //Console.WriteLine("New Url: " + ur_l);
                        HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                        request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            if (response.StatusCode.ToString().Equals("OK"))
                            {
                                cs.Start();
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine($"Source:{ex.Source}\nStackTrace:{ex.StackTrace}\n{ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Source:{ex.Source}\n{ex.Message}");
            }
        }
        private void AWB_checkbox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.Remote_start_setup)
                {
                    CameraServcies cs = (from s in MainWindow.Main_window.camerasServicies where s.cameraId == this.Camera.Id select s).FirstOrDefault();
                    cs.Stop();
                    CheckBox c = sender as CheckBox;
                    if (c.IsChecked.Value)
                    {
                        // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                        // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                        //Console.WriteLine("Old Url: " + this.Url);
                        int found = this.Url.IndexOf(":81");
                        String ur_l = this.Url.Substring(0, found); // = http://192.168.1.50/
                        ur_l += "/control?var=awb&val=1";
                        //Console.WriteLine("New Url: " + ur_l);
                        HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                        request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            if (response.StatusCode.ToString().Equals("OK"))
                            {
                                cs.Start();
                            }
                        }
                    }
                    else
                    {
                        // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                        // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                        //Console.WriteLine("Old Url: " + this.Url);
                        int found = this.Url.IndexOf(":81");
                        String ur_l = this.Url.Substring(0, found); // = http://192.168.1.50/
                        ur_l += "/control?var=awb&val=0";
                        //Console.WriteLine("New Url: " + ur_l);
                        HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                        request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            if (response.StatusCode.ToString().Equals("OK"))
                            {
                                cs.Start();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Source:{ex.Source}\n{ex.Message}");
            }
        }
        private void AWB_GAIN_checkbox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.Remote_start_setup)
                {
                    CameraServcies cs = (from s in MainWindow.Main_window.camerasServicies where s.cameraId == this.Camera.Id select s).FirstOrDefault();
                    cs.Stop();
                    CheckBox c = sender as CheckBox;
                    if (c.IsChecked.Value)
                    {
                        // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                        // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                        //Console.WriteLine("Old Url: " + this.Url);
                        int found = this.Url.IndexOf(":81");
                        String ur_l = this.Url.Substring(0, found); // = http://192.168.1.50/
                        ur_l += "/control?var=awb_gain&val=1";
                        //Console.WriteLine("New Url: " + ur_l);
                        HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                        request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            if (response.StatusCode.ToString().Equals("OK"))
                            {
                                cs.Start();
                            }
                        }
                    }
                    else
                    {
                        // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                        // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                        //Console.WriteLine("Old Url: " + this.Url);
                        int found = this.Url.IndexOf(":81");
                        String ur_l = this.Url.Substring(0, found); // = http://192.168.1.50/
                        ur_l += "/control?var=awb_gain&val=0";
                        //Console.WriteLine("New Url: " + ur_l);
                        HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                        request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            if (response.StatusCode.ToString().Equals("OK"))
                            {
                                cs.Start();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Source:{ex.Source}\n{ex.Message}");
            }
        }
        private void WB_MODE_Changed(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (this.Remote_start_setup)
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
                    CameraServcies cs = (from s in MainWindow.Main_window.camerasServicies where s.cameraId == this.Camera.Id select s).FirstOrDefault();
                    cs.Stop();
                    // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                    // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                    //Console.WriteLine("Old Url: " + this.Url);
                    int found = this.Url.IndexOf(":81");
                    String ur_l = this.Url.Substring(0, found); // = http://192.168.1.50/
                    ur_l += "/control?var=wb_mode&val=" + order;
                    //Console.WriteLine("New Url: " + ur_l);
                    HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                    request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        if (response.StatusCode.ToString().Equals("OK"))
                        {
                            cs.Start();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Source:{ex.Source}\n{ex.Message}");
            }
        }
        private void AEC_SENSOR_checkbox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.Remote_start_setup)
                {
                    CameraServcies cs = (from s in MainWindow.Main_window.camerasServicies where s.cameraId == this.Camera.Id select s).FirstOrDefault();
                    cs.Stop();
                    CheckBox c = sender as CheckBox;
                    if (c.IsChecked.Value)
                    {
                        // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                        // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                        //Console.WriteLine("Old Url: " + this.Url);
                        int found = this.Url.IndexOf(":81");
                        String ur_l = this.Url.Substring(0, found); // = http://192.168.1.50/
                        ur_l += "/control?var=aec&val=1";
                        //Console.WriteLine("New Url: " + ur_l);
                        HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                        request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            if (response.StatusCode.ToString().Equals("OK"))
                            {
                                cs.Start();
                            }
                        }
                    }
                    else
                    {
                        // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                        // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                        //Console.WriteLine("Old Url: " + this.Url);
                        int found = this.Url.IndexOf(":81");
                        String ur_l = this.Url.Substring(0, found); // = http://192.168.1.50/
                        ur_l += "/control?var=aec&val=0";
                        //Console.WriteLine("New Url: " + ur_l);
                        HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                        request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            if (response.StatusCode.ToString().Equals("OK"))
                            {
                                cs.Start();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Source:{ex.Source}\n{ex.Message}");
            }
        }
        private void AEC_DSP_checkbox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.Remote_start_setup)
                {
                    CameraServcies cs = (from s in MainWindow.Main_window.camerasServicies where s.cameraId == this.Camera.Id select s).FirstOrDefault();
                    cs.Stop();
                    CheckBox c = sender as CheckBox;
                    if (c.IsChecked.Value)
                    {
                        // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                        // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                        //Console.WriteLine("Old Url: " + this.Url);
                        int found = this.Url.IndexOf(":81");
                        String ur_l = this.Url.Substring(0, found); // = http://192.168.1.50/
                        ur_l += "/control?var=aec2&val=1";
                        //Console.WriteLine("New Url: " + ur_l);
                        HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                        request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            if (response.StatusCode.ToString().Equals("OK"))
                            {
                                cs.Start();
                            }
                        }
                    }
                    else
                    {
                        // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                        // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                        //Console.WriteLine("Old Url: " + this.Url);
                        int found = this.Url.IndexOf(":81");
                        String ur_l = this.Url.Substring(0, found); // = http://192.168.1.50/
                        ur_l += "/control?var=aec2&val=0";
                        //Console.WriteLine("New Url: " + ur_l);
                        HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                        request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            if (response.StatusCode.ToString().Equals("OK"))
                            {
                                cs.Start();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Source:{ex.Source}\n{ex.Message}");
            }
        }
        private void AE_LEVEL_changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                int val = Convert.ToInt32(e.NewValue);
                if (this.Remote_start_setup)
                {
                    CameraServcies cs = (from s in MainWindow.Main_window.camerasServicies where s.cameraId == this.Camera.Id select s).FirstOrDefault();
                    cs.Stop();
                    // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                    // http://192.168.1.50/control?var=quality&val=10
                    int found = this.Url.IndexOf(":81");
                    String ur_l = this.Url.Substring(0, found); // = http://192.168.1.50/
                    ur_l += "/control?var=ae_level&val=" + val;
                    //Console.WriteLine("New Url: " + ur_l);
                    HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                    request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        if (response.StatusCode.ToString().Equals("OK"))
                        {
                            cs.Start();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Source:{ex.Source}\n{ex.Message}");
            }
        }
        private void AGC_checkbox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.Remote_start_setup)
                {
                    CameraServcies cs = (from s in MainWindow.Main_window.camerasServicies where s.cameraId == this.Camera.Id select s).FirstOrDefault();
                    cs.Stop();
                    CheckBox c = sender as CheckBox;
                    if (c.IsChecked.Value)
                    {
                        // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                        // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                        //Console.WriteLine("Old Url: " + this.Url);
                        int found = this.Url.IndexOf(":81");
                        String ur_l = this.Url.Substring(0, found); // = http://192.168.1.50/
                        ur_l += "/control?var=agc&val=1";
                        //Console.WriteLine("New Url: " + ur_l);
                        HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                        request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            if (response.StatusCode.ToString().Equals("OK"))
                            {
                                cs.Start();
                            }
                        }
                    }
                    else
                    {
                        // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                        // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                        //Console.WriteLine("Old Url: " + this.Url);
                        int found = this.Url.IndexOf(":81");
                        String ur_l = this.Url.Substring(0, found); // = http://192.168.1.50/
                        ur_l += "/control?var=agc&val=0";
                        //Console.WriteLine("New Url: " + ur_l);
                        HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                        request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            if (response.StatusCode.ToString().Equals("OK"))
                            {
                                cs.Start();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Source:{ex.Source}\n{ex.Message}");
            }
        }
        private void GAIN_CEILINGL_changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                int val = Convert.ToInt32(e.NewValue);
                if (this.Remote_start_setup)
                {
                    CameraServcies cs = (from s in MainWindow.Main_window.camerasServicies where s.cameraId == this.Camera.Id select s).FirstOrDefault();
                    cs.Stop();
                    // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                    // http://192.168.1.50/control?var=quality&val=10
                    int found = this.Url.IndexOf(":81");
                    String ur_l = this.Url.Substring(0, found); // = http://192.168.1.50/
                    ur_l += "/control?var=agc_gain&val=" + val;
                    //Console.WriteLine("New Url: " + ur_l);
                    HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                    request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        if (response.StatusCode.ToString().Equals("OK"))
                        {
                            cs.Start();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Source:{ex.Source}\n{ex.Message}");
            }
        }
        private void BPC_checkbox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.Remote_start_setup)
                {
                    CameraServcies cs = (from s in MainWindow.Main_window.camerasServicies where s.cameraId == this.Camera.Id select s).FirstOrDefault();
                    cs.Stop();
                    CheckBox c = sender as CheckBox;
                    if (c.IsChecked.Value)
                    {
                        // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                        // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                        //Console.WriteLine("Old Url: " + this.Url);
                        int found = this.Url.IndexOf(":81");
                        String ur_l = this.Url.Substring(0, found); // = http://192.168.1.50/
                        ur_l += "/control?var=bpc&val=1";
                        //Console.WriteLine("New Url: " + ur_l);
                        HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                        request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            if (response.StatusCode.ToString().Equals("OK"))
                            {
                                cs.Start();
                            }
                        }
                    }
                    else
                    {
                        // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                        // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                        //Console.WriteLine("Old Url: " + this.Url);
                        int found = this.Url.IndexOf(":81");
                        String ur_l = this.Url.Substring(0, found); // = http://192.168.1.50/
                        ur_l += "/control?var=bpc&val=0";
                        //Console.WriteLine("New Url: " + ur_l);
                        HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                        request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            if (response.StatusCode.ToString().Equals("OK"))
                            {
                                cs.Start();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Source:{ex.Source}\n{ex.Message}");
            }
        }
        private void WPC_checkbox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.Remote_start_setup)
                {
                    CameraServcies cs = (from s in MainWindow.Main_window.camerasServicies where s.cameraId == this.Camera.Id select s).FirstOrDefault();
                    cs.Stop();
                    CheckBox c = sender as CheckBox;
                    if (c.IsChecked.Value)
                    {
                        // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                        // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                        //Console.WriteLine("Old Url: " + this.Url);
                        int found = this.Url.IndexOf(":81");
                        String ur_l = this.Url.Substring(0, found); // = http://192.168.1.50/
                        ur_l += "/control?var=wpc&val=1";
                        //Console.WriteLine("New Url: " + ur_l);
                        HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                        request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            if (response.StatusCode.ToString().Equals("OK"))
                            {
                                cs.Start();
                            }
                        }
                    }
                    else
                    {
                        // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                        // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                        //Console.WriteLine("Old Url: " + this.Url);
                        int found = this.Url.IndexOf(":81");
                        String ur_l = this.Url.Substring(0, found); // = http://192.168.1.50/
                        ur_l += "/control?var=wpc&val=0";
                        //Console.WriteLine("New Url: " + ur_l);
                        HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                        request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            if (response.StatusCode.ToString().Equals("OK"))
                            {
                                cs.Start();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Source:{ex.Source}\n{ex.Message}");
            }
        }
        private void RAW_GMA_checkbox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.Remote_start_setup)
                {
                    CameraServcies cs = (from s in MainWindow.Main_window.camerasServicies where s.cameraId == this.Camera.Id select s).FirstOrDefault();
                    cs.Stop();
                    CheckBox c = sender as CheckBox;
                    if (c.IsChecked.Value)
                    {
                        // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                        // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                        //Console.WriteLine("Old Url: " + this.Url);
                        int found = this.Url.IndexOf(":81");
                        String ur_l = this.Url.Substring(0, found); // = http://192.168.1.50/
                        ur_l += "/control?var=raw_gma&val=1";
                        //Console.WriteLine("New Url: " + ur_l);
                        HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                        request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            if (response.StatusCode.ToString().Equals("OK"))
                            {
                                cs.Start();
                            }
                        }
                    }
                    else
                    {
                        // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                        // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                        //Console.WriteLine("Old Url: " + this.Url);
                        int found = this.Url.IndexOf(":81");
                        String ur_l = this.Url.Substring(0, found); // = http://192.168.1.50/
                        ur_l += "/control?var=raw_gma&val=0";
                        //Console.WriteLine("New Url: " + ur_l);
                        HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                        request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            if (response.StatusCode.ToString().Equals("OK"))
                            {
                                cs.Start();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Source:{ex.Source}\n{ex.Message}");
            }
        }
        private void LENS_CORRECTION_checkbox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.Remote_start_setup)
                {
                    CameraServcies cs = (from s in MainWindow.Main_window.camerasServicies where s.cameraId == this.Camera.Id select s).FirstOrDefault();
                    cs.Stop();
                    CheckBox c = sender as CheckBox;
                    if (c.IsChecked.Value)
                    {
                        // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                        // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                        //Console.WriteLine("Old Url: " + this.Url);
                        int found = this.Url.IndexOf(":81");
                        String ur_l = this.Url.Substring(0, found); // = http://192.168.1.50/
                        ur_l += "/control?var=lenc&val=1";
                        //Console.WriteLine("New Url: " + ur_l);
                        HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                        request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            if (response.StatusCode.ToString().Equals("OK"))
                            {
                                cs.Start();
                            }
                        }
                    }
                    else
                    {
                        // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                        // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                        //Console.WriteLine("Old Url: " + this.Url);
                        int found = this.Url.IndexOf(":81");
                        String ur_l = this.Url.Substring(0, found); // = http://192.168.1.50/
                        ur_l += "/control?var=lenc&val=0";
                        //Console.WriteLine("New Url: " + ur_l);
                        HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                        request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            if (response.StatusCode.ToString().Equals("OK"))
                            {
                                cs.Start();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Source:{ex.Source}\n{ex.Message}");
            }
        }
        private void H_MIRROR_checkbox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.Remote_start_setup)
                {
                    CameraServcies cs = (from s in MainWindow.Main_window.camerasServicies where s.cameraId == this.Camera.Id select s).FirstOrDefault();
                    cs.Stop();
                    CheckBox c = sender as CheckBox;
                    if (c.IsChecked.Value)
                    {
                        // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                        // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                        //Console.WriteLine("Old Url: " + this.Url);
                        int found = this.Url.IndexOf(":81");
                        String ur_l = this.Url.Substring(0, found); // = http://192.168.1.50/
                        ur_l += "/control?var=hmirror&val=1";
                        //Console.WriteLine("New Url: " + ur_l);
                        HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                        request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            if (response.StatusCode.ToString().Equals("OK"))
                            {
                                cs.Start();
                            }
                        }
                    }
                    else
                    {
                        // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                        // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                        //Console.WriteLine("Old Url: " + this.Url);
                        int found = this.Url.IndexOf(":81");
                        String ur_l = this.Url.Substring(0, found); // = http://192.168.1.50/
                        ur_l += "/control?var=hmirror&val=0";
                        //Console.WriteLine("New Url: " + ur_l);
                        HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                        request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            if (response.StatusCode.ToString().Equals("OK"))
                            {
                                cs.Start();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Source:{ex.Source}\n{ex.Message}");
            }
        }
        private void V_FLIP_checkbox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.Remote_start_setup)
                {
                    CameraServcies cs = (from s in MainWindow.Main_window.camerasServicies where s.cameraId == this.Camera.Id select s).FirstOrDefault();
                    cs.Stop();
                    CheckBox c = sender as CheckBox;
                    if (c.IsChecked.Value)
                    {
                        // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                        // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                        //Console.WriteLine("Old Url: " + this.Url);
                        int found = this.Url.IndexOf(":81");
                        String ur_l = this.Url.Substring(0, found); // = http://192.168.1.50/
                        ur_l += "/control?var=vflip&val=1";
                        //Console.WriteLine("New Url: " + ur_l);
                        HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                        request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            if (response.StatusCode.ToString().Equals("OK"))
                            {
                                cs.Start();
                            }
                        }
                    }
                    else
                    {
                        // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                        // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                        //Console.WriteLine("Old Url: " + this.Url);
                        int found = this.Url.IndexOf(":81");
                        String ur_l = this.Url.Substring(0, found); // = http://192.168.1.50/
                        ur_l += "/control?var=vflip&val=0";
                        //Console.WriteLine("New Url: " + ur_l);
                        HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                        request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            if (response.StatusCode.ToString().Equals("OK"))
                            {
                                cs.Start();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Source:{ex.Source}\n{ex.Message}");
            }
        }
        private void DCW_DOWNSIZE_checkbox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.Remote_start_setup)
                {
                    CameraServcies cs = (from s in MainWindow.Main_window.camerasServicies where s.cameraId == this.Camera.Id select s).FirstOrDefault();
                    cs.Stop();
                    CheckBox c = sender as CheckBox;
                    if (c.IsChecked.Value)
                    {
                        // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                        // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                        //Console.WriteLine("Old Url: " + this.Url);
                        int found = this.Url.IndexOf(":81");
                        String ur_l = this.Url.Substring(0, found); // = http://192.168.1.50/
                        ur_l += "/control?var=dcw&val=1";
                        //Console.WriteLine("New Url: " + ur_l);
                        HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                        request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            if (response.StatusCode.ToString().Equals("OK"))
                            {
                                cs.Start();
                            }
                        }
                    }
                    else
                    {
                        // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                        // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                        //Console.WriteLine("Old Url: " + this.Url);
                        int found = this.Url.IndexOf(":81");
                        String ur_l = this.Url.Substring(0, found); // = http://192.168.1.50/
                        ur_l += "/control?var=dcw&val=0";
                        //Console.WriteLine("New Url: " + ur_l);
                        HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                        request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            if (response.StatusCode.ToString().Equals("OK"))
                            {
                                cs.Start();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Source:{ex.Source}\n{ex.Message}");
            }
        }
        private void COLOR_BAR_checkbox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.Remote_start_setup)
                {
                    CameraServcies cs = (from s in MainWindow.Main_window.camerasServicies where s.cameraId == this.Camera.Id select s).FirstOrDefault();
                    cs.Stop();
                    CheckBox c = sender as CheckBox;
                    if (c.IsChecked.Value)
                    {
                        // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                        // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                        //Console.WriteLine("Old Url: " + this.Url);
                        int found = this.Url.IndexOf(":81");
                        String ur_l = this.Url.Substring(0, found); // = http://192.168.1.50/
                        ur_l += "/control?var=colorbar&val=1";
                        //Console.WriteLine("New Url: " + ur_l);
                        HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                        request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            if (response.StatusCode.ToString().Equals("OK"))
                            {
                                cs.Start();
                            }
                        }
                    }
                    else
                    {
                        // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                        // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                        //Console.WriteLine("Old Url: " + this.Url);
                        int found = this.Url.IndexOf(":81");
                        String ur_l = this.Url.Substring(0, found); // = http://192.168.1.50/
                        ur_l += "/control?var=colorbar&val=0";
                        //Console.WriteLine("New Url: " + ur_l);
                        HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                        request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            if (response.StatusCode.ToString().Equals("OK"))
                            {
                                cs.Start();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Source:{ex.Source}\n{ex.Message}");
            }
        }
        private void FACE_DETECTION_checkbox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.Remote_start_setup)
                {
                    if (Convert.ToInt16(_cam_resolution_order) <= 5)
                    {
                        CameraServcies cs = (from s in MainWindow.Main_window.camerasServicies where s.cameraId == this.Camera.Id select s).FirstOrDefault();
                        cs.Stop();
                        CheckBox c = sender as CheckBox;
                        if (c.IsChecked.Value)
                        {
                            // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                            // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                            //Console.WriteLine("Old Url: " + this.Url);
                            int found = this.Url.IndexOf(":81");
                            String ur_l = this.Url.Substring(0, found); // = http://192.168.1.50/
                            ur_l += "/control?var=face_detect&val=1";
                            //Console.WriteLine("New Url: " + ur_l);
                            HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                            request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                            {
                                if (response.StatusCode.ToString().Equals("OK"))
                                {
                                    this.Remote_detection = true;
                                    cs.Start();
                                }
                            }
                        }
                        else
                        {
                            // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                            // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                            //Console.WriteLine("Old Url: " + this.Url);
                            int found = this.Url.IndexOf(":81");
                            String ur_l = this.Url.Substring(0, found); // = http://192.168.1.50/
                            ur_l += "/control?var=face_detect&val=0";
                            //Console.WriteLine("New Url: " + ur_l);
                            HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                            request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                            {
                                if (response.StatusCode.ToString().Equals("OK"))
                                {
                                    this.Remote_detection = false;
                                    this.Remote_recognition = false;
                                    cs.Start();
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Cameras Resolution lowest from 680x480");
                        cameras_face_detection_checkbox.IsChecked = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Source:{ex.Source}\n{ex.Message}");
            }
        }
        private void FACE_RECOGNITION_checkbox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.Remote_start_setup)
                {
                    CheckBox c = sender as CheckBox;
                    if (this.Remote_detection)
                    {
                        CameraServcies cs = (from s in MainWindow.Main_window.camerasServicies where s.cameraId == this.Camera.Id select s).FirstOrDefault();
                        cs.Stop();
                        if (c.IsChecked.Value)
                        {
                            // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                            // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                            //Console.WriteLine("Old Url: " + this.Url);
                            int found = this.Url.IndexOf(":81");
                            String ur_l = this.Url.Substring(0, found); // = http://192.168.1.50/
                            ur_l += "/control?var=face_recognize&val=1";
                            //Console.WriteLine("New Url: " + ur_l);
                            HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                            request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                            {
                                if (response.StatusCode.ToString().Equals("OK"))
                                {
                                    this.Remote_recognition = true;
                                    cs.Start();
                                }
                            }
                        }
                        else
                        {
                            // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                            // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                            //Console.WriteLine("Old Url: " + this.Url);
                            int found = this.Url.IndexOf(":81");
                            String ur_l = this.Url.Substring(0, found); // = http://192.168.1.50/
                            ur_l += "/control?var=face_recognize&val=0";
                            //Console.WriteLine("New Url: " + ur_l);
                            HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                            request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                            {
                                if (response.StatusCode.ToString().Equals("OK"))
                                {
                                    this.Remote_recognition = false;
                                    cs.Start();
                                }
                            }
                        }
                    }
                    else
                    {
                        c.IsChecked = false;
                        MessageBox.Show("Face Detection is disabled");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Source:{ex.Source}\n{ex.Message}");
            }
        }
        private void GET_STILL_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                // Create Random a number with 13 digits
                Random ran = new Random();
                int num_1 = ran.Next(0000000, 10000000);
                int num_2 = ran.Next(000000, 10000000);
                String val = num_1.ToString() + num_2.ToString();
                // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                //Console.WriteLine("Old Url: " + this.Url);
                int found = this.Url.IndexOf(":81");
                String ur_l = this.Url.Substring(0, found); // = http://192.168.1.50/
                ur_l += "/capture?_cb=0";
                //Console.WriteLine("New Url: " + ur_l);
                HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode.ToString().Equals("OK"))
                    {
                        CameraServcies cs = (from s in MainWindow.Main_window.camerasServicies where s.cameraId == this.Camera.Id select s).FirstOrDefault();
                        cs.Start();
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Source:{ex.Source}\n{ex.Message}");
            }
        }
        private void RESTART_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                CameraServcies cs = (from s in MainWindow.Main_window.camerasServicies where s.cameraId == this.Camera.Id select s).FirstOrDefault();
                cs.Stop();
                // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                //Console.WriteLine("Old Url: " + this.Url);
                int found = this.Url.IndexOf(":81");
                String ur_l = this.Url.Substring(0, found); // = http://192.168.1.50/
                ur_l += "/restart?username=" + this.Camera.username + "&password=" + this.Camera.password;
                Console.WriteLine("New Url: " + ur_l);
                HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                try
                {
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        if (response.StatusCode.ToString().Equals("OK"))
                        {
                            Thread.Sleep(5000);
                            ((CheckBox)cameras_face_detection_checkbox).IsChecked = false;
                            this.Remote_detection = false;
                            ((CheckBox)cameras_face_recognition_checkbox).IsChecked = false;
                            this.Remote_recognition = false;
                            cs.Start();
                        }
                    }
                }
                catch (System.Net.WebException ex)
                {
                    Console.WriteLine($"Source:{ex.Source}\nStackTrace:{ex.StackTrace}\n{ex.Message}");
                    cs.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Source:{ex.Source}\n{ex.Message}");
            }
        }
        private void ENROLL_FACE_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.Remote_recognition)
                {
                    // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                    // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                    //Console.WriteLine("Old Url: " + this.Url);
                    int found = this.Url.IndexOf(":81");
                    String ur_l = this.Url.Substring(0, found); // = http://192.168.1.50/
                    ur_l += "/control?var=face_enroll&val=1";
                    Console.WriteLine("New Url: " + ur_l);
                    HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                    request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                    try
                    {
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            if (response.StatusCode.ToString().Equals("OK"))
                            {
                            }
                        }
                    }
                    catch (System.Net.WebException ex)
                    {
                        Console.WriteLine($"Source:{ex.Source}\nStackTrace:{ex.StackTrace}\n{ex.Message}");
                    }
                } else
                {
                    MessageBox.Show("Face Recognition is disabled");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Source:{ex.Source}\n{ex.Message}");
            }
        }
        private void Reboot_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                CameraServcies cs = (from s in MainWindow.Main_window.camerasServicies where s.cameraId == this.Camera.Id select s).FirstOrDefault();
                cs.Stop();
                // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                //Console.WriteLine("Old Url: " + this.Url);
                int found = this.Url.IndexOf(":81");
                String ur_l = this.Url.Substring(0, found); // = http://192.168.1.50/
                ur_l += "/reboot?username=" + this.Camera.username + "&password=" + this.Camera.password;
                Console.WriteLine("New Url: " + ur_l);
                HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                try
                {
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        if (response.StatusCode.ToString().Equals("OK"))
                        {
                        }
                    }
                }
                catch (System.Net.WebException ex)
                {
                    Console.WriteLine($"Source:{ex.Source}\nStackTrace:{ex.StackTrace}\n{ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Source:{ex.Source}\n{ex.Message}");
            }
        }
        private void Hostpot_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                CameraServcies cs = (from s in MainWindow.Main_window.camerasServicies where s.cameraId == this.Camera.Id select s).FirstOrDefault();
                cs.Stop();
                // Url now = http://192.168.1.50:81/stream?username=alexandrosplatanios&password=Platanios719791
                // Expected Url = http://192.168.1.50/control?var=framesize&val=0
                //Console.WriteLine("Old Url: " + this.Url);
                int found = this.Url.IndexOf(":81");
                String ur_l = this.Url.Substring(0, found); // = http://192.168.1.50/
                ur_l += "/hostpot?username=" + this.Camera.username + "&password=" + this.Camera.password;
                Console.WriteLine("New Url: " + ur_l);
                HttpWebRequest request = WebRequest.CreateHttp(ur_l);
                request.Method = "GET"; // or "POST", "PUT", "PATCH", "DELETE", etc.
                try
                {
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        if (response.StatusCode.ToString().Equals("OK"))
                        {
                        
                        }
                    }
                }
                catch (System.Net.WebException ex)
                {
                    Console.WriteLine($"Source:{ex.Source}\nStackTrace:{ex.StackTrace}\n{ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Source:{ex.Source}\n{ex.Message}");
            }
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void Network_stream_check(object sender, RoutedEventArgs e)
        {
            CheckBox c = sender as CheckBox;
            if (c.IsChecked.Value)
            {
                if (network_streaming_port.Text.Length > 0)
                {
                    this.Camera.net_stream = true;
                } else
                {
                    MessageBox.Show("Fill ip & port!");
                    c.IsChecked = false;
                }
            }
            else
            {
                this.Camera.net_stream = false;
            }
        }
        private void Network_streaming_port_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox n = sender as TextBox;
            if (Int32.Parse(n.Text) >= 8000)
            {
                this.Camera.net_stream_port = (String)n.Text;
            }
            else
            {
                MessageBox.Show("Enter a Port Bigger from 8000");
            }
        }
        private void Network_streaming_prefix_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox n = sender as TextBox;
            this.Camera.net_stream_prefix = (String)n.Text;
        }
        private void Start_Clicked(object sender, RoutedEventArgs e)
        {
            CameraServcies cs = (from s in MainWindow.Main_window.camerasServicies where s.cameraId == this.Camera.Id select s).FirstOrDefault();
            cs.Start();
        }
        private void Stop_Clicked(object sender, RoutedEventArgs e)
        {
            CameraServcies cs = (from s in MainWindow.Main_window.camerasServicies where s.cameraId == this.Camera.Id select s).FirstOrDefault();
            cs.Stop();
        }
    }
 }
