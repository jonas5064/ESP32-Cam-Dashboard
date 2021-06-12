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
        private HttpListener listener;
        public bool run;
        private String ip = "";
        private String port = "";
        private String prefix = "";
        private String url = "";
        private Camera cam;

        public HttpServer(Camera cam)
        {
            this.cam = cam;
            this.ip = this.cam.Net_stream_ip;
            this.port = this.cam.Net_stream_port;
            this.prefix = this.cam.Net_stream_prefix;
            if (!this.ip.Equals("") && !this.port.Equals("") && !this.prefix.Equals(""))
            {
                this.url = $"http://{this.ip}:{this.port}/{this.prefix}/";
                // Create a listener.
                this.listener = new HttpListener();
                // Add Url
                if (!HttpListener.IsSupported)
                {
                    Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                    return;
                }
                this.listener.Prefixes.Add(this.url);
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
                if (this.run)
                {
                    this.listener.Start();
                }
                if (this.listener.IsListening)
                {
                    while (this.run)
                    {
                        try
                        {
                            var context = await this.listener.GetContextAsync();
                            HttpListenerRequest request = context.Request;
                            HttpListenerResponse response = context.Response;
                            // Get Frame Buffer
                            System.Windows.Media.Imaging.BitmapSource bitmapsource = this.cam.video.Frame_GetCurrent();
                            MemoryStream outStream = new MemoryStream();
                            JpegBitmapEncoder enc = new JpegBitmapEncoder();
                            enc.QualityLevel = 100;
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

        public void close()
        {
            this.run = false;
            Console.WriteLine("Stop Listener.");
            this.listener.Stop();
            Console.WriteLine("Close Listener.");
            this.listener.Close();
            //this.listener.Prefixes.Clear();
            //this.listener = new HttpListener();
        }

    }

}
