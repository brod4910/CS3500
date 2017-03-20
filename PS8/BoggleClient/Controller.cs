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
        /// The token of the most recently registered user, or "0" if no user
        /// has ever registered
        /// </summary>
        private string userToken;

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
            this.view = view;
            view.RegisterPressed += Register;
            view.CancelPressed += Cancel;
        }

        /// <summary>
        /// Cancels the current operation (currently unimplemented)
        /// </summary>
        private void Cancel()
        {
            tokenSource.Cancel();
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

                this.client = CreateClient(domain);

                using (client)
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
        private void Refresh(bool brief)
        {
            using (client)
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

                    if (brief)
                    {

                    }
                    else
                    {
                        if(items.GameState == "active")
                        {
                            view.GameState = true;
                        }
                        else
                        {
                            view.GameState = false;
                        }
                        view.DisplayBoard(items.Board);
                        view.Time = items.Time;
                    }
                }
                else
                {
                    String errorMessage = "Error " + response.StatusCode + "\n" + response.ReasonPhrase;
                    MessageBox.Show(errorMessage);
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
