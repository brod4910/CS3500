using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BoggleGame
{
    class SocketClient
    {
        // Incoming/outgoing is UTF8-encoded.  This is a multi-byte encoding.  The first 128 Unicode characters
        // (which corresponds to the old ASCII character set and contains the common keyboard characters) are
        // encoded into a single byte.  The rest of the Unicode characters can take from 2 to 4 bytes to encode.
        private static System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();

        // Buffer size for reading incoming bytes
        private const int BUFFER_SIZE = 1024;

        // The socket through which we communicate with the remote client
        private Socket socket;

        // Text that has been received from the client but not yet dealt with
        private StringBuilder incoming;

        // Text that needs to be sent to the client but which we have not yet started sending
        private StringBuilder outgoing;

        private BoggleService service;

        // For decoding incoming UTF8-encoded byte streams.
        private Decoder decoder = encoding.GetDecoder();

        // Buffers that will contain incoming bytes and characters
        private byte[] incomingBytes = new byte[BUFFER_SIZE];
        private char[] incomingChars = new char[BUFFER_SIZE];

        // Records whether an asynchronous send attempt is ongoing
        private bool sendIsOngoing = false;

        // For synchronizing sends
        private readonly object sendSync = new object();

        // Bytes that we are actively trying to send, along with the
        // index of the leftmost byte whose send has not yet been completed
        private byte[] pendingBytes = new byte[0];
        private int pendingIndex = 0;

        public SocketClient(Socket s)
        {
            // Record the socket and clear incoming
            socket = s;
            service = new BoggleService();
            incoming = new StringBuilder();
            outgoing = new StringBuilder();

            // Ask the socket to call MessageReceive as soon as up to 1024 bytes arrive.
            socket.BeginReceive(incomingBytes, 0, incomingBytes.Length,
                                SocketFlags.None, MessageReceived, null);
        }

        /// <summary>
        /// Closes the Socket
        /// </summary>
        public void Shutdown()
        {
            try
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Attempts to send the entire outgoing string.
        /// This method should not be called unless sendSync has been acquired.
        /// </summary>
        private void SendBytes()
        {
            // If we're in the middle of the process of sending out a block of bytes,
            // keep doing that.
            if (pendingIndex < pendingBytes.Length)
            {
                Console.WriteLine("\tSending " + (pendingBytes.Length - pendingIndex) + " bytes");
                socket.BeginSend(pendingBytes, pendingIndex, pendingBytes.Length - pendingIndex,
                                 SocketFlags.None, MessageSent, null);
            }

            // If we're not currently dealing with a block of bytes, make a new block of bytes
            // out of outgoing and start sending that.
            else if (outgoing.Length > 0)
            {
                pendingBytes = encoding.GetBytes(outgoing.ToString());
                pendingIndex = 0;
                Console.WriteLine("\tConverting " + outgoing.Length + " chars into " + pendingBytes.Length + " bytes, sending them");
                outgoing.Clear();
                socket.BeginSend(pendingBytes, 0, pendingBytes.Length,
                                 SocketFlags.None, MessageSent, null);
            }

            // If there's nothing to send, shut down for the time being.
            else
            {
                Console.WriteLine("Shutting down send mechanism\n");
                sendIsOngoing = false;
            }
        }

        /// <summary>
        /// Called when a message has been successfully sent
        /// </summary>
        private void MessageSent(IAsyncResult result)
        {
            // Find out how many bytes were actually sent
            int bytesSent = socket.EndSend(result);
            Console.WriteLine("\t" + bytesSent + " bytes were successfully sent");

            // Get exclusive access to send mechanism
            lock (sendSync)
            {
                // The socket has been closed
                if (bytesSent == 0)
                {
                    socket.Close();
                    Console.WriteLine("Socket closed");
                }

                // Update the pendingIndex and keep trying
                else
                {
                    pendingIndex += bytesSent;
                    SendBytes();
                }
            }
        }

        /// <summary>
        /// Sends a string to the client
        /// </summary>
        private void SendMessage(string lines)
        {
            // Get exclusive access to send mechanism
            lock (sendSync)
            {
                // Append the message to the outgoing lines
                outgoing.Append(lines);

                // If there's not a send ongoing, start one.
                if (!sendIsOngoing)
                {
                    Console.WriteLine("Appending a " + lines.Length + " char line, starting send mechanism");
                    sendIsOngoing = true;
                    SendBytes();
                }
                else
                {
                    Console.WriteLine("\tAppending a " + lines.Length + " char line, send mechanism already running");
                }
            }
        }

        /// <summary>
        /// Called when some data has been received.
        /// </summary>
        private void MessageReceived(IAsyncResult result)
        {
            // Figure out how many bytes have come in
            int bytesRead = socket.EndReceive(result);
            string requestType = null;
            int gameid;
            object requestParam = null;

            // If no bytes were received, it means the client closed its side of the socket.
            // Report that to the console and close our socket.
            if (bytesRead == 0)
            {
                Console.WriteLine("Socket closed");
                socket.Close();
            }

            // Otherwise, decode and display the incoming bytes.  Then request more bytes.
            else
            {
                // Convert the bytes into characters and appending to incoming
                int charsRead = decoder.GetChars(incomingBytes, 0, bytesRead, incomingChars, 0, false);
                incoming.Append(incomingChars, 0, charsRead);
                //Console.WriteLine(incoming);

                int lastNewline = -1;
                int start = 0;
                for(int i = 0; i < incoming.Length; i++)
                {
                    if(incoming[i] == '\n')
                    {
                        String line = incoming.ToString(start, i + 1 - start);

                        else if(requestType == null)
                        {
                            requestParam = parseData(line, out requestType);

                            if(requestParam is int)
                            {
                                if(requestType == "PUT")
                                {
                                    gameid = (int)requestParam;
                                }
                                //request type is GET status
                                else
                                {
                                    GetGames(line, requestParam.ToString());
                                }
                            }
                        }
                        lastNewline = i;
                        start = i + 1;
                    }
                }
                incoming.Remove(0, lastNewline + 1);

                // Ask for some more data
                socket.BeginReceive(incomingBytes, 0, incomingBytes.Length,
                    SocketFlags.None, MessageReceived, null);
            }
        }

        private void GetGames(string url, string gameid)
        {
            // Parse Regex
            Regex r = new Regex(@"^/BoggleService.svc/games/(\d+)Brief=(.*)?$");
            Match m = r.Match(url);
            string isBrief = m.Groups[2].Value;

            Status gameStatus;
            HttpStatusCode serviceStatus;
            // Make Call
            if (isBrief == "yes")
            {
                gameStatus = service.Gamestatus(gameid, "yes", out  serviceStatus);
            }
            else
            {
                gameStatus = service.Gamestatus(gameid, "no", out serviceStatus);
            }
            // Serialize
            String s = JsonConvert.SerializeObject(gameStatus, new JsonSerializerSettings
            { DefaultValueHandling = DefaultValueHandling.Ignore });
            // Send Back with appropriate headers
            SendMessage("HTTP/1.1 " + (int)serviceStatus + " " + serviceStatus.ToString() + "\r\n");
            SendMessage("Content-Type: application/json\r\n");
            SendMessage("Content-Length: " + s.Length + "\r\n");
            SendMessage("\r\n");
            SendMessage(s);
        }

        private void CancelGame(string url)
        {
            HttpStatusCode serviceStatus;
            Token token = JsonConvert.DeserializeObject<Token>(url);
            this.service.CancelJoin(token, out serviceStatus);
            SendMessage("HTTP/1.1 " + (int)serviceStatus + " " + serviceStatus.ToString() + "\r\n");
            SendMessage("\r\n");
        }

        private void RegisterUser(string url)
        {
            HttpStatusCode serviceStatus;
            UserInfo user = JsonConvert.DeserializeObject<UserInfo>(url);
            Token gameStatus = service.Register(user, out serviceStatus);

            String s = JsonConvert.SerializeObject(gameStatus, new JsonSerializerSettings
            { DefaultValueHandling = DefaultValueHandling.Ignore });

            // Send Back with appropriate headers
            SendMessage("HTTP/1.1 " + (int)serviceStatus + " " + serviceStatus.ToString() + "\r\n");
            SendMessage("Content-Type: application/json\r\n");
            SendMessage("Content-Length: " + s.Length + "\r\n");
            SendMessage("\r\n");
            SendMessage(s);
        }

        private void JoinGame(string url)
        {
            HttpStatusCode serviceStatus;
            PostingGame user = JsonConvert.DeserializeObject<PostingGame>(url);
            GameId gameStatus = service.JoinGame(user, out serviceStatus);

            String s = JsonConvert.SerializeObject(gameStatus, new JsonSerializerSettings
            { DefaultValueHandling = DefaultValueHandling.Ignore });

            // Send Back with appropriate headers
            SendMessage("HTTP/1.1 " + (int)serviceStatus + " " + serviceStatus.ToString() + "\r\n");
            SendMessage("Content-Type: application/json\r\n");
            SendMessage("Content-Length: " + s.Length + "\r\n");
            SendMessage("\r\n");
            SendMessage(s);
        }


        private object parseData(String line, out string requestType)
        {
            //change this part. Make it general for all lines.
            //Maybe a more complex regex that is useful for every line. Determing what work
            //must be done on the line Etc.
            Regex r;
            Match m;
            string regexString = @"(?:(/BoggleService.svc/))";

            if (isHttpRequest(line, out requestType))
            {
                if (requestType == "GET")
                {
                    if ((m = (r = new Regex(regexString.Insert(23, "games/(\\d+)"))).Match(line)).Success)
                    {
                        int gameID;

                        int.TryParse(m.Groups[1].Value, out gameID);

                        return gameID;
                    }
                }
                else if (requestType == "PUT")
                {
                    if((m = (r = new Regex(regexString.Insert(23, "games"))).Match(line)).Success)
                    {
                        return "cancel";
                    }
                    else if ((m = (r = new Regex(regexString.Insert(23, "games/(\\d+)"))).Match(line)).Success)
                    {
                        int gameID;

                        int.TryParse(m.Groups[1].Value, out gameID);

                        return gameID;
                    }
                }
                //request type == POST
                else
                {
                    if ((m = (r = new Regex(regexString.Insert(23,"users"))).Match(line)).Success)
                    {
                        return "users";
                    }
                    else if((m = (r = new Regex(regexString.Insert(23, "games"))).Match(line)).Success)
                    {
                        return "games";
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Determines if the next incoming bytes is the request body
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private bool nextIsRequestBody(string line)
        {
            if(line.Length == 4)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks to see if the line is an http request line
        /// </summary>
        /// <param name="line"></param>
        /// <param name="RequestType"></param>
        /// <returns></returns>
        private bool isHttpRequest(string line, out string RequestType)
        {
            if(line[0] == 'G')
            {
                RequestType = "GET";
                return true;
            }
            else if(line[0] == 'P')
            {
                if(line[1] == 'U')
                {
                    RequestType = "PUT";
                    return true;
                }
                else
                {
                    RequestType = "POST";
                    return true;
                }
            }
            else
            {
                RequestType = null;
                return false;
            }
        }
    }
}
