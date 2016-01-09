using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emitter;

namespace Service
{
    /// <summary>
    /// Represents a base class for a micro service.
    /// </summary>
    public abstract class ServiceBase
    {
        /// <summary>
        /// The name of our service.
        /// </summary>
        protected string Name;

        /// <summary>
        /// Connection to the emitter message broker.
        /// </summary>
        protected Connection Emitter;

        /// <summary>
        /// Http listener.
        /// </summary>
        protected HttpListener Http;

        /// <summary>
        /// Constructs a new service.
        /// </summary>
        public ServiceBase()
        {
            this.Name = this.GetType().Name;
        }

        /// <summary>
        /// Starts the service.
        /// </summary>
        public void Start()
        {
            // Establish the connection
            Console.WriteLine("[" + Name + "] Connecting to Emitter...");
            this.Emitter = Connection.Establish("192.168.56.102", null);


            Console.WriteLine("[" + Name + "] Creating HttpListener...");
            this.Http = new HttpListener();
            this.Http.Prefixes.Add("http://*:80/");
            this.Http.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            this.Http.Start();

            ThreadPool.QueueUserWorkItem((o) =>
            {
                while (this.Http.IsListening)
                {
                    ThreadPool.QueueUserWorkItem((c) =>
                    {
                        var ctx = c as HttpListenerContext;
                        try
                        {
                            this.ProcessHttpRequest(ctx);
                        }
                        catch { } 
                        finally { ctx.Response.OutputStream.Close(); }
                    }, this.Http.GetContext());
                }
            });

            // Run in the same thread context
            Console.WriteLine("[" + Name + "] Running...");
            this.Run();
        }

        /// <summary>
        /// Stops the service.
        /// </summary>
        public void Stop()
        {
            this.Emitter.Disconnect();
            this.Http.Stop();
        }

        /// <summary>
        /// Main method for the service.
        /// </summary>
        protected abstract void Run();

        /// <summary>
        /// Handles HTTP requests.
        /// </summary>
        protected void ProcessHttpRequest(HttpListenerContext ctx)
        {
            var response = "";
            try
            {
                if (ctx.Request.Url.PathAndQuery.StartsWith("/health"))
                {
                    response = "200: I am " + this.Name + " and I am healthy";
                    ctx.Response.StatusCode = 200;
                }
                else
                {
                    response = "404: Page Not Found";
                    ctx.Response.StatusCode = 404;
                }
            }
            finally
            {
                var buffer = Encoding.UTF8.GetBytes(response);
                ctx.Response.ContentLength64 = buffer.Length;
                ctx.Response.OutputStream.Write(buffer, 0, buffer.Length);
            }
        }
    }
}
