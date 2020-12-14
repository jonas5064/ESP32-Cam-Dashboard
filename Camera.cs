using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

//using VisioForge.Controls.UI.WinForms;
using VisioForge.Controls.UI.WPF;
using VisioForge.Types;
// https://help.visioforge.com/sdks_net/html/T_VisioForge_Controls_UI_WPF_VideoCapture.htm
using VisioForge.Types.VideoEffects;

namespace IPCamera
{
    public class Camera
    {
        public String url = "";
        public string name = "";
        public string id = "";
        public bool detection = false;
        public bool recognition = false;
        public int brightness = 0;
        public int contrast = 0;
        public int darkness = 0;
        public bool recording = false;
        public VideoCapture video;
        public static int count = 0;
        public static String DB_connection_string = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Alexp\\source\\repos\\IPCamera\\Database1.mdf;Integrated Security=True";

        public static String pictures_dir;
        public static String videos_dir;

        public Camera(String url, String name, String id)
        {
            this.url = url;
            this.name = name;
            this.id = id;

            // Create an VideoCapture
            this.video = new VideoCapture();
            this.video.IP_Camera_Source = new VisioForge.Types.Sources.IPCameraSourceSettings() { URL = this.url, Type = VisioForge.Types.VFIPSource.RTSP_HTTP_FFMPEG };
            this.video.Audio_PlayAudio = this.video.Audio_RecordAudio = false;
            this.video.Mode = VisioForge.Types.VFVideoCaptureMode.IPPreview;
            this.video.Video_Effects_Enabled = true; // Enable Video Effects

            count++;
        }

        ~Camera()
        {
            count--;
        }

        public int Brightness
        {
            get { return this.brightness; }
            set
            {
                this.brightness = value;
                // Add the efect
                IVFVideoEffectLightness lightness;
                var effect_l = this.video.Video_Effects_Get("Lightness");
                if (effect_l == null)
                {
                    lightness = new VFVideoEffectLightness(true, this.brightness, 0, "Lightness");
                    this.video.Video_Effects_Add(lightness);
                }
                else
                {
                    lightness = effect_l as IVFVideoEffectLightness;
                    if (lightness != null)
                    {
                        lightness.Value = this.brightness;
                    }
                }
            }
        }

        public int Contrast
        {
            get { return this.contrast; }
            set
            {
                this.contrast = value;
                // Add the efect
                IVFVideoEffectContrast contrast;
                var effect_c = this.video.Video_Effects_Get("Contrast");
                if (effect_c == null)
                {
                    contrast = new VFVideoEffectContrast(true, this.contrast, 0, "Contrast");
                    this.video.Video_Effects_Add(contrast);
                }
                else
                {
                    contrast = effect_c as IVFVideoEffectContrast;
                    if (contrast != null)
                    {
                        contrast.Value = this.contrast;
                    }
                }
            }
        }

        public int Darkness
        {
            get { return this.darkness; }
            set
            {
                this.darkness = value;
                // Add the efect
                IVFVideoEffectDarkness darkness;
                var effect_d = this.video.Video_Effects_Get("Darkness");
                if (effect_d == null)
                {
                    darkness = new VFVideoEffectDarkness(true, this.darkness, 0, "Darkness");
                    this.video.Video_Effects_Add(darkness);
                }
                else
                {
                    darkness = effect_d as IVFVideoEffectDarkness;
                    if (darkness != null)
                    {
                        darkness.Value = this.darkness;
                    }
                }
            }
        }

        public bool Detection
        {
            get
            { return this.detection; }
            set
            {
                this.detection = value;
            }
        }

        public bool Recognition
        {
            get { return this.recognition; }
            set
            {
                this.recognition = value;
            }
        }

        // Start / Stop Recording
        public bool Recording
        {
            get { return this.recording; }
            set
            {
                this.recording = value;
                if (value == true) // Recording
                {
                    // Save to DataBase

                }
                else // No Recording
                {
                    // Save to DataBase

                }
            }
        }


        public Action<object, MouseButtonEventArgs> MouseUp { get; internal set; }

        public void start()
        {
            try
            {
                this.video.Start();
                //this.video.StartAsync();
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("No cameras has found!");
            }
        }

        public void stop()
        {
            try
            {
                //this.video.Stop();
                this.video.StopAsync();
            }
            catch (Exception)
            {
                //System.Windows.MessageBox.Show("No cameras has found!");
            }
        }

        public void take_pic()
        {
            DateTime now = DateTime.Now;
            String name = now.ToString("F");
            name = name.Replace(":", ".");
            //name = name.Replace(" ", "_");
            String file = Camera.pictures_dir + "\\" + name + ".jpg";
            System.Windows.MessageBox.Show($"Save Picture  {file}");
            this.video.Frame_Save(file, VisioForge.Types.VFImageFormat.JPEG, 85);
        }

    }
}
