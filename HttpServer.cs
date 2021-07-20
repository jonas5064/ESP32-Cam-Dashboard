using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace IPCamera
{


    public class HttpServer
    {
        private HttpListener Listener { get; set; }
        public bool Run { get; set; }
        private String Ip { get; set; }
        private String Port { get; set; }
        private String Prefix { get; set; }
        private String Url { get; set; }
        private Camera Cam { get; set; }

        public HttpServer(Camera cam)
        {
            this.Cam = cam;
            this.Ip = this.Cam.Net_stream_ip;
            this.Port = this.Cam.Net_stream_port;
            this.Prefix = this.Cam.Net_stream_prefix;
            if (!this.Ip.Equals("") && !this.Port.Equals("") && !this.Prefix.Equals(""))
            {
                this.Url = $"http://{this.Ip}:{this.Port}/{this.Prefix}/";
                // Create a listener.
                this.Listener = new HttpListener();
                // Add Url
                if (!HttpListener.IsSupported)
                {
                    Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                    return;
                }
                this.Listener.Prefixes.Add(this.Url);
            }
            else
            {
                MessageBox.Show("Enter an IP, Port and the Prefix Please.");
            }
        }
        

        public async Task ListenAsync()
        {
            try
            {
                // Start Server
                if (this.Run)
                {
                    this.Listener.Start();
                }
                if (this.Listener.IsListening)
                {
                    while (this.Run)
                    {
                        try
                        {
                            var context = await this.Listener.GetContextAsync();
                            HttpListenerRequest request = context.Request;
                            HttpListenerResponse response = context.Response;
                            // Get Frame Buffer
                            System.Windows.Media.Imaging.BitmapSource bitmapsource = this.Cam.Video.Frame_GetCurrent();
                            MemoryStream outStream = new MemoryStream();
                            JpegBitmapEncoder enc = new JpegBitmapEncoder
                            {
                                QualityLevel = 100
                            };
                            enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                            enc.Save(outStream);
                            byte[] frame = outStream.ToArray(); //.GetBuffer();
                            outStream.Close();
                            // Send Frames
                            response.StatusCode = 200;
                            response.ContentLength64 = frame.Length;
                            response.ContentType = "image/jpeg";
                            response.KeepAlive = true;
                            response.Headers.Add("Refresh", "0");
                            response.OutputStream.Write(frame, 0, frame.Length);
                            response.OutputStream.Close();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Source:{ex.Source}\nStackTrace:{ex.StackTrace}\n{ex.Message}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"\n\nServer isn't Started Yet.\n\n");
                }
            }
            catch(System.NullReferenceException ex)
            {
                Console.WriteLine($"\n\n\n{ex.Message}\n\n\n");
            }
        }

        public void Close()
        {
            this.Run = false;
            Console.WriteLine("Stop Listener.");
            this.Listener.Stop();
            Console.WriteLine("Close Listener.");
            this.Listener.Close();
            //this.listener.Prefixes.Clear();
            //this.listener = new HttpListener();
        }

    }

}
