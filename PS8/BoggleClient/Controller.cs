using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

namespace BoggleClient
{
    public class Controller
    {

        /// <summary>
        /// The view controlled by this Controller
        /// </summary>
        private IBoggleView view;

        /// <summary>
        /// Timer For Events
        /// </summary>
        private System.Timers.Timer eventTimer;

        private bool gameHasStarted = false;

        /// <summary>
        /// Holds the registered domain
        /// </summary>
        private string domain;

        /// <summary>
        /// Holds the registered game ID
        /// </summary>
        private string gameId;
        /// <summary>
        /// The token of the most recently registered user, or "0" if no user
        /// has ever registered
        /// </summary>
        private string userToken;

        /// <summary>
        /// String representing the time entered
        /// </summary>
        private string timeLimit;

        private System.Windows.Forms.Timer time = new System.Windows.Forms.Timer();
        /// <summary>
        /// For canceling the current operation
        /// </summary>
        private CancellationTokenSource tokenSource;

        /// <summary>
        /// Client used throughout game
        /// </summary>
        private HttpClient client;
        
        /// <summary>
        /// GameId for any given game
        /// </summary>
        private string GameId;

        public Controller(IBoggleView view)
        {
            time.Interval = 1000;
            this.view = view;
            eventTimer = new System.Timers.Timer(1000);
            view.RegisterPressed += Register;
            view.CancelPressed += Cancel;
            view.CreateGamePressed += HandleCreateGamePressed;
            view.GameStatus += GameStatus;
            view.WordEntered += WordSubmitted;
        }

        /// <summary>
        /// Cancels the current operation (currently unimplemented)
        /// </summary>
        private void Cancel()
        {
            tokenSource.Cancel();
        }

        /// <summary>
        /// Handles when Create Game is pushed, sends request for new game to boggle api
        /// </summary>
        private async void HandleCreateGamePressed(string timeLimit)
        {
            try
            {
                using (HttpClient client = CreateClient(this.domain))
                {
                    dynamic data = new ExpandoObject();
                    data.UserToken = userToken;
                    data.TimeLimit = timeLimit;

                    tokenSource = new CancellationTokenSource();
                    StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("games", content, tokenSource.Token);

                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        dynamic items = JsonConvert.DeserializeObject(result);
                        this.gameId = items.GameID;
                        // Need to Use a thread timer instead of just spinning
                        while(!GameStatus(false))
                        {
                            Thread.Sleep(1000);
                        }
                        // Set Board
                        view.SetTime();
                    }
                    else
                    {
                        Console.WriteLine("Error submitting: " + response.StatusCode);
                        Console.WriteLine(response.ReasonPhrase);
                    }

                }
            }
            catch (TaskCanceledException)
            {

            }
            finally
            {
                view.EnableControls(true);
            }
        }

        /// <summary>
        /// Registers a user with the given username and domain
        /// </summary>
        /// <param name="username"></param>
        /// <param name="domain"></param>
        private async void Register(string username, string domain)
        {
            try
            {
                view.EnableControls(false);
                this.domain = domain;

                using (HttpClient client = CreateClient(this.domain))
                {
                    //Create the parameter
                    dynamic user = new ExpandoObject();
                    user.Nickname = username;

                    //compose and send the post request
                    tokenSource = new CancellationTokenSource();
                    StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("users", content, tokenSource.Token);

                    if(response.IsSuccessStatusCode)
                    {
                        String result = response.Content.ReadAsStringAsync().Result;
                        dynamic items = JsonConvert.DeserializeObject(result);
                        userToken = items.UserToken;
                        view.UserRegistered = true;
                        view.DisableNameAndServer();
                    }
                    else
                    {
                        String errorMessage = "Error " + response.StatusCode + "\n" + response.ReasonPhrase;
                        MessageBox.Show(errorMessage);
                    }
                }
            }
            catch (TaskCanceledException)
            {

            }
            finally
            {
                view.EnableControls(true);
            }
        }

        /// <summary>
        /// Refreshes the display when something has changed
        /// </summary>
        private bool GameStatus(bool brief)
        {
            using (HttpClient client = CreateClient(this.domain))
            {
                HttpResponseMessage response;

                if (brief)
                {
                    String url = String.Format("games/{0}?Brief={1}", this.gameId, "no");
                    //String url = String.Format("games/{0}", this.gameId);
                    response = client.GetAsync(url).Result;
                }
                else
                {
                    //String url = String.Format("games/{0}?Brief={1}", this.gameId, "yes");
                    String url = String.Format("games/{0}", this.gameId);
                    response = client.GetAsync(url).Result;
                }


                if (response.IsSuccessStatusCode)
                {
                    String result = response.Content.ReadAsStringAsync().Result;
                    dynamic items = JsonConvert.DeserializeObject(result);

                    if (items.GameState != "pending")
                    {
                        if (brief)
                        {

                        }
                        else
                        {
                            if (items.GameState == "active") // Need to use a timer to invoke calls every second to update score and timer
                            {
                                view.GameState = true;
                                view.DisplayBoard((string) items.Board);
                                view.Time = items.TimeLeft;
                                view.SetSubmitButton(true);
                                view.SetPlayerNicknames((string) items.Player1.Nickname, (string) items.Player2.Nickname);
                                return true;
                            }
                            else
                            {
                                view.GameState = false;
                                return false;
                            }
                        }
                    }
                    {
                        return false;
                    }
                
                }
                else
                {
                    String errorMessage = "Error " + response.StatusCode + "\n" + response.ReasonPhrase;
                    MessageBox.Show(errorMessage);
                    return false;
                }
            }

        }

        /// <summary>
        /// Makes HTTP Put request to play the word
        /// </summary>
        /// <param name="word"></param>
        private async void WordSubmitted(string word)
        {
            using (HttpClient client = CreateClient(this.domain))
            {
                dynamic data = new ExpandoObject();
                data.UserToken = userToken;
                data.Word = word;

                tokenSource = new CancellationTokenSource();
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                String url = String.Format("games/{0}", this.gameId);
                HttpResponseMessage response = await client.PutAsync(url, content, tokenSource.Token);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    dynamic items = JsonConvert.DeserializeObject(result);
                }
            }
        }

        /// <summary>
        /// Creates the client used for this instance
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        private static HttpClient CreateClient(string domain)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(domain + "/BoggleService.svc/");

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            return client;
        }
    }
}
