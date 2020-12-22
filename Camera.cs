using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

//using VisioForge.Controls.UI.WinForms;
using VisioForge.Controls.UI.WPF;
using VisioForge.Types;
using VisioForge.Types.OutputFormat;
// https://help.visioforge.com/sdks_net/html/T_VisioForge_Controls_UI_WPF_VideoCapture.htm
using VisioForge.Types.VideoEffects;

namespace IPCamera
{
    public class Camera
    {
        public String url = "";
        public string name = "";
        public string id = "";
        public int row = 0;
        public int coll = 0;
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
        public static bool avi_format = false;
        public static bool mp4_format = false;
        public static bool webm_format = false;


        public Camera(String url, String name, String id, bool rec)
        {
            this.url = url;
            this.name = name;
            this.id = id;

            // Create an VideoCapture
            this.video = new VideoCapture
            {
                IP_Camera_Source = new VisioForge.Types.Sources.IPCameraSourceSettings()
                {
                    URL = this.url/*,
                    Type = VisioForge.Types.VFIPSource.Auto_LAV*/
                }
            };
            this.video.OnError += OnError;
            this.video.MouseUp += CamerasFocused;
            this.video.Audio_PlayAudio = this.video.Audio_RecordAudio = false;
            this.video.Video_Effects_Enabled = true;
            this.video.IP_Camera_Source.Type = VisioForge.Types.VFIPSource.HTTP_MJPEG_LowLatency;
            this.Recording = rec;

            count++;
        }


        ~Camera()
        {
            count--;
        }

        // Return this video capture
        public VideoCapture Get()
        {
            return this.video;
        }

        // Setup Brightness Effext
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

        // Setup Contrast Effext
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

        // Setup Drkness Effext
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
            get { return this.detection; }
            set
            {
                this.detection = value;
                if (this.detection)
                {
                    /*
                    this.video.Face_Tracking = new FaceTrackingSettings
                    {
                        ColorMode = CamshiftMode.RGB,
                        Highlight = true,
                        MinimumWindowSize = 25,
                        ScalingMode = ObjectDetectorScalingMode.GreaterToSmaller,
                        ScaleFactor = (float)1.7,
                        SearchMode = ObjectDetectorSearchMode.Single
                    };
                    */
                    this.video.Face_Tracking = new FaceTrackingSettings()
                    {
                        Highlight = true
                    };
                    this.video.OnFaceDetected += (object sender, AFFaceDetectionEventArgs e) =>
                    {
                        foreach (Rectangle faceRectangle in e.FaceRectangles)
                        {
                            // If Recognition is enable
                            if (this.recognition)
                            {

                            }
                            else
                            {
                                Console.WriteLine($"Face Detection:   left-right({faceRectangle.Left}, {faceRectangle.Right}), " +
                                    $"top-bottom({faceRectangle.Top}, {faceRectangle.Bottom}),  width-height({faceRectangle.Width}, " +
                                    $"{faceRectangle.Height})  {Environment.NewLine}");
                            }
                        }
                    };
                }
                else
                {
                    this.video.Face_Tracking = new FaceTrackingSettings();
                    this.video.OnFaceDetected += (object sender, AFFaceDetectionEventArgs e) => { };
                }
            }
        }


        public bool Recognition
        {
            get { return this.recognition; }
            set { this.recognition = value; }
        }

        // Start / Stop Recording
        public bool Recording
        {
            get { return this.recording; }
            set { 
                this.recording = value;
            }
        }

        // Start the Camera
        public void Start()
        {
            if (this.video.Status != VisioForge.Types.VFVideoCaptureStatus.Work)
            {
                try
                {
                    // If Rcording is enable setup recording mode
                    Setup_recording_mode();
                    // Start Cameres
                    this.video.Start();
                    //this.video.StartAsync();
                }
                catch (System.AccessViolationException)
                {
                    throw new NotImplementedException();
                }
            }
        }

        // Stop The Camera
        public void Stop()
        {
            if (this.video.Status == VisioForge.Types.VFVideoCaptureStatus.Work)
            {
                try
                {
                    this.video.Stop();
                    //this.video.StopAsync();
                }
                catch (System.AccessViolationException)
                {
                    throw new NotImplementedException();
                }
                
            }
        }

        // Take Picture
        public void Take_pic()
        {
            DateTime now = DateTime.Now;
            String date = now.ToString("F");
            date = date.Replace(":", ".");
            String dir_path = Camera.pictures_dir + "\\" + this.name;
            if (! Directory.Exists(dir_path))
            {
                Directory.CreateDirectory(dir_path);
            }
            String file = dir_path + "\\" + date + ".jpg";
            this.video.Frame_Save(file, VisioForge.Types.VFImageFormat.JPEG, 85);
        }

        
        // Setup Recording Mode
        private void Setup_recording_mode()
        {
            if (this.Recording)
            {
                // Video mode == capture
                this.video.Mode = VFVideoCaptureMode.IPCapture;
                // Setup the right file name
                DateTime now = DateTime.Now;
                String date = now.ToString("F");
                date = date.Replace(":", ".");
                String dir_path = Camera.videos_dir + "\\" + this.name;
                if (!Directory.Exists(dir_path)) // Directory with the name of the camera
                {
                    Directory.CreateDirectory(dir_path);
                }
                // Start Recording
                // AVI
                if (avi_format)
                {
                    String file = dir_path + "\\" + date + ".avi";
                    this.video.Output_Filename = file;
                    this.video.Output_Format = new VFAVIOutput();
                }
                // MP4
                if (mp4_format)
                {
                    String file = dir_path + "\\" + date + ".mp4";
                    this.video.Output_Filename = file;
                    this.video.Output_Format = new VFMP4v8v10Output();
                }
                // WEBM
                if (webm_format)
                {
                    String file = dir_path + "\\" + date + ".webm";
                    this.video.Output_Filename = file;
                    this.video.Output_Format = new VFWebMOutput();
                }
            }
            else
            {
                // Setup video mode to preview
                this.video.Mode = VisioForge.Types.VFVideoCaptureMode.IPPreview;
            }
        }
        

        // On Error EVnt
        private void OnError(object sender, VisioForge.Types.ErrorsEventArgs e)
        {
            System.Windows.MessageBox.Show($"[OnError]   {e.Message}");
            //throw new NotImplementedException();
        }

        // When click on camera
        public void CamerasFocused(object sender, MouseButtonEventArgs e)
        {
            WindowControll win_controll = new WindowControll(this);
            win_controll.Show();
        }

    }
}
