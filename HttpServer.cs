using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IPCamera
{


    public class HttpServer
    {
        private string[] prefixes;
        private HttpListener listener;
        public bool run;
        public String ip = "";
        public String port = "";

        public HttpServer()
        {
            // Create a listener.
            this.listener = new HttpListener();
        }
        
        public void setup()
        {
            if (!this.ip.Equals("") && !this.port.Equals(""))
            {
                Console.WriteLine("Listener Setup OK.");
                String url = "http://" + ip + ":" + port + "/";
                Console.WriteLine(url);
                this.prefixes = new string[] { url };
                if (!HttpListener.IsSupported)
                {
                    Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                    return;
                }
                // URI prefixes are required,
                // for example "http://contoso.com:8080/index/".
                if (prefixes == null || prefixes.Length == 0)
                    throw new ArgumentException("prefixes");
                // Add the prefixes.
                foreach (string s in prefixes)
                {
                    try
                    {
                        this.listener.Prefixes.Add(s);
                    } catch (System.Net.HttpListenerException) { }
                }
                this.listener.Start();
                Console.WriteLine("Listening...");
            } else
            {
                Console.WriteLine("Listener Setup NOT OK.");
            }
        }

        public async Task ListenAsync()
        {
            if (this.listener != null)
            {
                while (this.run)
                {
                    Console.WriteLine("Listening...");
                    var context = await this.listener.GetContextAsync();
                    HttpListenerRequest request = context.Request;
                    // Obtain a response object.
                    HttpListenerResponse response = context.Response;
                    // Construct a response.   Here will displays the video
                    string responseString = "<HTML><BODY> Hello world!</BODY></HTML>";
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                    response.ContentLength64 = buffer.Length;
                    System.IO.Stream output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    // You must close the output stream.
                    output.Close();
                }
            }
        }

        public void close()
        {
            this.run = false;
            //this.listener.Prefixes.Clear();
            Console.WriteLine("Stop Listener.");
            this.listener.Stop();
            this.listener.Close();
            this.listener = new HttpListener();
        }
    }

}
