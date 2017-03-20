using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading;
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
            view.RegisterPressed += Register;
            view.CancelPressed += Cancel;
            view.CreateGamePressed += HandleCreateGamePressed;
            view.GameStatus += GameStatus;
        }

        /// <summary>
        /// Cancels the current operation (currently unimplemented)
        /// </summary>
        private void Cancel()
        {
            tokenSource.Cancel();
        }

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
                       // time.Start();
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
                    String url = String.Format(" games/{0}&Brief={1]", GameId, "no");
                    response = client.GetAsync(url).Result;
                }
                else
                {
                    String url = String.Format(" games/{0}&Brief={1]", GameId, "yes");
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
                            if (items.GameState == "active")
                            {
                                view.GameState = true;
                                view.DisplayBoard(items.Board);
                                view.Time = items.Time;
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
