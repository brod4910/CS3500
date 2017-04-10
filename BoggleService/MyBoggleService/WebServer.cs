using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Boggle
{
    class WebServer
    {
        private static BoggleService boggle;
        private TcpListener server;
        private readonly ReaderWriterLockSlim sync = new ReaderWriterLockSlim();
        /// <summary>
        /// Launches a Webserver on Port 600000. Keeps the main
        /// thread active so we can send output to console.
        /// </summary>
        public static void Main()
        {
            new WebServer();
            Console.Read();
        }

        /// <summary>
        /// Creates a TcpListner binding
        /// </summary>
        public WebServer()
        {
            boggle = new BoggleService();
            server = new TcpListener(IPAddress.Any, 60000);
            server.Start();
            server.BeginAcceptSocket(ConnectionRequested, null);
        }

        /// <summary>
        /// Recieves the request and closes the socket
        /// </summary>
        /// <param name="result"></param>
        private void ConnectionRequested(IAsyncResult result)
        {
            Socket s = server.EndAcceptSocket(result);
            server.BeginAcceptSocket(ConnectionRequested, null);
            // Another line or lines here. but not sure what.
        }
    }
}
