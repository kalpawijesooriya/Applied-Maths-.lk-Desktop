using appliedmaths.API;
using appliedmaths.Models;
using appliedmaths.ViewModels;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace appliedmaths.Helpers
{


   public class HttpServer 
    {
       
        public static HttpListener listener;
        public static int pageViews = 0;
        public static int requestCount = 0;
        public IVdoCyperOTP _vdoCyperOTP;
      
        public HttpServer(IVdoCyperOTP vdoCyperOTP) {
            _vdoCyperOTP = vdoCyperOTP;
        }
        public  async Task HandleIncomingConnections()
        {
            bool runServer = true;
           
            // While a user hasn't visited the `shutdown` url, keep on handling requests
            while (runServer)
            {
                // Will wait here until we hear from a connection
                HttpListenerContext ctx = await listener.GetContextAsync();

                // Peel out the requests and response objects
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;

                // Print out some info about the request
                Console.WriteLine("Request #: {0}", ++requestCount);
                Console.WriteLine(req.Url.ToString());
                Console.WriteLine(req.HttpMethod);
                Console.WriteLine(req.UserHostName);
                Console.WriteLine(req.UserAgent);
                Console.WriteLine();

                // If `shutdown` url requested w/ POST, then shutdown the server after serving the page
                if ((req.HttpMethod == "GET") && (req.Url.AbsolutePath == "/shutdown"))
                {
                    Console.WriteLine("Shutdown requested");
                    runServer = false;
                }

                // Make sure we don't increment the page views counter if `favicon.ico` is requested
                if (req.Url.AbsolutePath != "/favicon.ico")
                    pageViews += 1;
                try {
                    if ( req.Url.AbsolutePath == "/video")
                    {
                        string pageData = "<!DOCTYPE>" +
                                                     "<html>" +
                                                     "  <head>" +
                                                     "    <title>HttpListener Example</title>" +
                                                     "  </head>" +
                                                     " <script>" +
                                                     " var otp='" + _vdoCyperOTP.otp + "';"+
                                                     " var playBack='" + _vdoCyperOTP.playbackInfo + "';" +
                                                     " </script>" +
                                                     "  <body>" +
                                                     "  <div id='embedBox' style='width:1280px;max-width:100%;height:auto;'></div>" +
                                                     " <script type='text/javascript'>{2}</script>" +
                                                     "<style>{3}</style>"+
                                                     " </body>" +
                                                     "</html>";

                        string disableSubmit = !runServer ? "disabled" : "";
                        string js = @"(function(v, i, d, e, o){v[o] = v[o] ||{}; v[o].add = v[o].add || function V(a) { (v[o].d = v[o].d ||[]).push(a); };if(!v[o].l) { v[o].l=1* new Date(); a=i.createElement(d); m=i.getElementsByTagName(d)[0]; a.async=1; a.src=e;m.parentNode.insertBefore(a, m);}})(window, document, 'script', 'https://player.vdocipher.com/playerAssets/1.6.10/vdo.js ', 'vdo'); vdo.add({ otp:otp,playbackInfo: playBack,theme: '9ae8bbe8dd964ddc9bdb932cca1cb59a',container: document.querySelector('#embedBox'),}); document.addEventListener('contextmenu', function(e) {e.preventDefault();}); document.onkeydown = function(e) { if(event.keyCode == 123) {return false;}if(e.ctrlKey && e.shiftKey && e.keyCode == 'I'.charCodeAt(0)){return false; } if(e.ctrlKey && e.shiftKey && e.keyCode == 'J'.charCodeAt(0)){return false; } if(e.ctrlKey && e.keyCode == 'U'.charCodeAt(0)){ return false; }}";
                        string css = @"iframe{height : 95% !important;} .container-second { position: sticky !important; }";
                        byte[] data = Encoding.UTF8.GetBytes(string.Format(pageData, pageViews, disableSubmit, js, css));
                        resp.ContentType = "text/html";
                        resp.ContentEncoding = Encoding.UTF8;
                        resp.ContentLength64 = data.LongLength;

                        // Write out to the response stream (asynchronously), then close it
                        await resp.OutputStream.WriteAsync(data, 0, data.Length);
                        
                    }
                 
                  
                } catch (Exception ex) 
                { 
                    Console.WriteLine(ex.Message); 
                }
               
            }
        }


        public  void Start()
        {
            try
            {
            
            int port= PortFinder.GetAvailablePort(3000);
            string url= "http://localhost:" + port + "/";
            Application.Current.Resources["internal_server_url"] = url;
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            Console.WriteLine("Listening for connections on {0}", url);

            // Handle requests
            Task listenTask = HandleIncomingConnections();
            listenTask.GetAwaiter().GetResult();

            // Close the listener
            listener.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
