using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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

        //array for request
        //[0] = Request Type
        //[1] = users || games
        //[2] = params(gameid)
        //[3] = params(Brief)
        //[4] = content-length
        //[5] = request body
        private object[] requestParams = new object[6];

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
                for (int i = 0; i < incoming.Length; i++)
                {
                    if (incoming[i] == '\n')
                    {
                        String line = incoming.ToString(start, i + 1 - start);

                        parseData(line);

                        lastNewline = i;
                        start = i + 1;
                    }
                    else if (incoming[i] == '}')
                    {
                        String line = incoming.ToString(start, i + 1 - start);

                        parseData(line);
                        ConfigureProperRequest();
                        lastNewline = i;
                        start = i + 1;
                    }
                }
                incoming.Remove(0, lastNewline + 1);

                socket.BeginReceive(incomingBytes, 0, incomingBytes.Length,
                    SocketFlags.None, MessageReceived, null);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////        Start of helper methods        //////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        private void ConfigureProperRequest()
        {
            if ((string)requestParams[0] == "POST")
            {
                if ((string)requestParams[1] == "games")
                {
                    JoinGame((string)requestParams[5]);
                }
                //else we are posting to users
                else
                {
                    RegisterUser((string)requestParams[5]);
                }
            }
            else if ((string)requestParams[1] == "PUT")
            {
                //Cancel game
                if ((string)requestParams[1] == "cancel")
                {
                    CancelGame((string)requestParams[5]);
                }
                //else play word
                else
                {
                    PlayWord((string)requestParams[5], (string)requestParams[2]);
                }
            }
            //Else its a get Method
            else
            {
                GetGames((string)requestParams[5], (string)requestParams[2]);
            }

        }

        private void GetGames(string requestBody, string gameid)
        {
            string isBrief = (string)requestParams[3];

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

        private void getAPIPage()
        {
            SendMessage("HTTP/1.1 200 OK\n");
            var API = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "..\\index.html");
            SendMessage("Content-Length: text/html\r\n");
            SendMessage("\r\n");
            SendMessage(API);
        }


        private void CancelGame(string requestBody)
        {
            HttpStatusCode serviceStatus;
            Token token = JsonConvert.DeserializeObject<Token>(requestBody);
            this.service.CancelJoin(token, out serviceStatus);
            SendMessage("HTTP/1.1 " + (int)serviceStatus + " " + serviceStatus.ToString() + "\r\n");
            SendMessage("\r\n");
        }

        private void RegisterUser(string RequestBody)
        {
            HttpStatusCode serviceStatus;
            UserInfo user = JsonConvert.DeserializeObject<UserInfo>(RequestBody);
            Token token = service.Register(user, out serviceStatus);

            String s = JsonConvert.SerializeObject(token, new JsonSerializerSettings
            { DefaultValueHandling = DefaultValueHandling.Ignore });

            // Send Back with appropriate headers
            SendMessage("HTTP/1.1 " + (int)serviceStatus + " " + serviceStatus.ToString() + "\r\n");
            SendMessage("Content-Type: application/json\r\n");
            SendMessage("Content-Length: " + s.Length + "\r\n");
            SendMessage("\r\n");
            SendMessage(s);
        }

        private void JoinGame(string requestBody)
        {
            HttpStatusCode serviceStatus;
            PostingGame user = JsonConvert.DeserializeObject<PostingGame>(requestBody);
            GameId gameID = service.JoinGame(user, out serviceStatus);

            String s = JsonConvert.SerializeObject(gameID, new JsonSerializerSettings
            { DefaultValueHandling = DefaultValueHandling.Ignore });

            // Send Back with appropriate headers
            SendMessage("HTTP/1.1 " + (int)serviceStatus + " " + serviceStatus.ToString() + "\r\n");
            SendMessage("Content-Type: application/json\r\n");
            SendMessage("Content-Length: " + s.Length + "\r\n");
            SendMessage("\r\n");
            SendMessage(s);
        }
        
        private void PlayWord(string requestBody, string gameId)
        {
            HttpStatusCode serviceStatus;
            PlayedWord user = JsonConvert.DeserializeObject<PlayedWord>(requestBody);

            WordScore wordScore = service.PlayWord(gameId, user, out serviceStatus);

            String s = JsonConvert.SerializeObject(wordScore, new JsonSerializerSettings
            { DefaultValueHandling = DefaultValueHandling.Ignore });

            // Send Back with appropriate headers
            SendMessage("HTTP/1.1 " + (int)serviceStatus + " " + serviceStatus.ToString() + "\r\n");
            SendMessage("Content-Type: application/json\r\n");
            SendMessage("Content-Length: " + s.Length + "\r\n");
            SendMessage("\r\n");
            SendMessage(s);
        }

        private void parseData(String line)
        {
            //change this part. Make it general for all lines.
            //Maybe a more complex regex that is useful for every line. Determing what work
            //must be done on the line Etc.
            Regex r;
            Match m;
            string regexString = @"(?:(/BoggleService.svc/))";
            string requestType;

            if (isHttpRequest(line, out requestType))
            {
                if (requestType == "GET")
                {
                    if ((m = (r = new Regex(regexString.Insert(23, "games/(\\d+)?Brief=(.*)"))).Match(line)).Success)
                    {
                        requestParams[0] = "GET";
                        requestParams[1] = "games";
                        requestParams[2] = m.Groups[1].Value;
                        requestParams[3] = m.Groups[2].Value;
                    }
                }
                else if (requestType == "PUT")
                {
                    if((m = (r = new Regex(regexString.Insert(23, "games"))).Match(line)).Success)
                    {
                        requestParams[0] = "PUT";
                        requestParams[1] = "games";
                    }
                    else if ((m = (r = new Regex(regexString.Insert(23, "games/(\\d+)"))).Match(line)).Success)
                    {
                        requestParams[0] = "PUT";
                        requestParams[1] = "games";
                        requestParams[2] = m.Groups[1].Value;
                    }
                }
                //request type == POST
                else
                {
                    if ((m = (r = new Regex(regexString.Insert(23,"users"))).Match(line)).Success)
                    {
                        requestParams[0] = "POST";
                        requestParams[1] = "users";
                    }
                    else if((m = (r = new Regex(regexString.Insert(23, "games"))).Match(line)).Success)
                    {
                        requestParams[0] = "POST";
                        requestParams[1] = "games";
                    }
                }
            }
            else if(isContentLength(line))
            {
                string length = line.Substring(16);
                requestParams[4] = int.Parse(length);
            }
            else if(isRequestBody(line))
            {
                requestParams[5] = line;
            }
        }

        private bool isRequestBody(string line)
        {
            if(line[0] == '{')
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determines if it is the content-length line
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private bool isContentLength(string line)
        {
            if(line[0] == 'C')
            {
                if(line.Contains("Length"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determines if the next incoming bytes is the request body
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private bool nextIsRequestBody(string line)
        {
            if(line.Length == 2)
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
