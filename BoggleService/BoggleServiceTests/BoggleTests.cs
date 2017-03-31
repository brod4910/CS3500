using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static System.Net.HttpStatusCode;
using System.Diagnostics;
using Newtonsoft.Json;
using Boggle;

namespace Boggle
{
    /// <summary>
    /// Provides a way to start and stop the IIS web server from within the test
    /// cases.  If something prevents the test cases from stopping the web server,
    /// subsequent tests may not work properly until the stray process is killed
    /// manually.
    /// </summary>
    public static class IISAgent
    {
        // Reference to the running process
        private static Process process = null;

        /// <summary>
        /// Starts IIS
        /// </summary>
        public static void Start(string arguments)
        {
            if (process == null)
            {
                ProcessStartInfo info = new ProcessStartInfo(Properties.Resources.IIS_EXECUTABLE, arguments);
                info.WindowStyle = ProcessWindowStyle.Minimized;
                info.UseShellExecute = false;
                process = Process.Start(info);
            }
        }

        /// <summary>
        ///  Stops IIS
        /// </summary>
        public static void Stop()
        {
            if (process != null)
            {
                process.Kill();
            }
        }
    }
    [TestClass]
    public class BoggleTests
    {
        /// <summary>
        /// This is automatically run prior to all the tests to start the server
        /// </summary>
        [ClassInitialize()]
        public static void StartIIS(TestContext testContext)
        {
            IISAgent.Start(@"/site:""BoggleService"" /apppool:""Clr4IntegratedAppPool"" /config:""..\..\..\.vs\config\applicationhost.config""");
        }

        /// <summary>
        /// This is automatically run when all tests have completed to stop the server
        /// </summary>
        [ClassCleanup()]
        public static void StopIIS()
        {
            IISAgent.Stop();
        }

        private RestTestClient client = new RestTestClient("http://localhost:60000/BoggleService.svc/");

        [TestMethod]
        public void TestRegister()
        {
            UserInfo user = new UserInfo();
            user.Nickname = "testRegister";
            Response r = client.DoPostAsync("users", user).Result;
            Assert.AreEqual(OK, r.Status);
        }

        [TestMethod]
        public void TestRegisterNull()
        {
            UserInfo user = new UserInfo();
            user.Nickname = null;
            Response r = client.DoPostAsync("users", user).Result;
            Assert.AreEqual(BadRequest, r.Status);
        }


        [TestMethod]
        public void TestRegisterBlank()
        {
            UserInfo user = new UserInfo();
            user.Nickname = "";
            Response r = client.DoPostAsync("users", user).Result;
            Assert.AreEqual(Forbidden, r.Status);
        }

        [TestMethod]
        public void TestJoinGame()
        {
            client = new RestTestClient("http://localhost:60000/BoggleService.svc/");
            // Register a User
            UserInfo user = new UserInfo();
            user.Nickname = "testJoinGame1";
            Response r = client.DoPostAsync("users", user).Result;
            Assert.AreEqual(OK, r.Status);

            // Join Game
            PostingGame game = new PostingGame();
            game.UserToken = r.Data["UserToken"];
            game.TimeLimit = "30";
            Response f = client.DoPostAsync("games", game).Result;
            Assert.AreEqual(Accepted, f.Status);

            // Register a User
            UserInfo user2 = new UserInfo();
            user2.Nickname = "testJoinGame2";
            Response r2 = client.DoPostAsync("users", user2).Result;
            Assert.AreEqual(OK, r2.Status);

            // Join Game
            PostingGame game2 = new PostingGame();
            game2.UserToken = r2.Data["UserToken"];
            game2.TimeLimit = "30";
            Response f2 = client.DoPostAsync("games", game2).Result;
            Assert.AreEqual(Created, f2.Status);

            string GameID = f2.Data["GameID"];


            int time;

            System.Threading.Thread.Sleep(3000);

            Response f3 = client.DoGetAsync("games/{0}?Brief={1}", GameID, "no").Result;

            string timeleft = f3.Data["TimeLeft"];

            int.TryParse(timeleft, out time);


            Assert.IsTrue(time < 30);

            // Add Test that checks if it is 201 or 202
        }

        [TestMethod]
        public void TestJoinGameTimeError()
        {
            client = new RestTestClient("http://localhost:60000/BoggleService.svc/");
            // Register a User
            UserInfo user = new UserInfo();
            user.Nickname = "testJoinGameTimeError";
            Response r = client.DoPostAsync("users", user).Result;
            Assert.AreEqual(OK, r.Status);

            // Join Game with forbidden timelimit
            PostingGame game = new PostingGame();
            game.UserToken = r.Data["UserToken"];
            game.TimeLimit = "2";
            Response f = client.DoPostAsync("games", game).Result;
            Assert.AreEqual(Forbidden, f.Status);

            game.TimeLimit = "125";
            Response e = client.DoPostAsync("games", game).Result;
            Assert.AreEqual(Forbidden, f.Status);
        }

        [TestMethod]
        public void TestJoinGameMultiple()
        {
            client = new RestTestClient("http://localhost:60000/BoggleService.svc/");
            // Register a User
            UserInfo user = new UserInfo();
            user.Nickname = "testJoinGameMultiple";
            Response r = client.DoPostAsync("users", user).Result;
            Assert.AreEqual(OK, r.Status);

            // Join Game
            PostingGame game = new PostingGame();
            game.UserToken = r.Data["UserToken"];
            game.TimeLimit = "30";
            Response f = client.DoPostAsync("games", game).Result;
            if(f.Status != Accepted && f.Status != Created)
            {
                Assert.IsTrue(false); // Throw error
            }
            System.Threading.Thread.Sleep(1000);

            PostingGame second_game = new PostingGame();
            game.UserToken = r.Data["UserToken"];
            game.TimeLimit = "60";
            Response e = client.DoPostAsync("games", game).Result;
            Assert.AreEqual(Conflict, e.Status);

            PostingGame third_game = new PostingGame();
            game.UserToken = r.Data["UserToken"];
            game.TimeLimit = "70";
            Response g = client.DoPostAsync("games", game).Result;
            Assert.AreEqual(Conflict, g.Status);

            Token cancel = new Token();
            cancel.UserToken = r.Data["UserToken"];
            Console.WriteLine("UserToken of Cancel: " + r.Data["UserToken"]);
            Response h = client.DoPutAsync(cancel, "games").Result;
            Assert.AreEqual(OK, h.Status);
        }


        [TestMethod]
        public void TestCancelGame()
        {
            client = new RestTestClient("http://localhost:60000/BoggleService.svc/");
            // Register a User
            UserInfo user = new UserInfo();
            user.Nickname = "testCancelGame";
            Response r = client.DoPostAsync("users", user).Result;
            Assert.AreEqual(OK, r.Status);

            // Join Game
            PostingGame game = new PostingGame();
            game.UserToken = r.Data["UserToken"];
            game.TimeLimit = "100";
            Response f = client.DoPostAsync("games", game).Result;
            if (f.Status == OK || f.Status == Accepted || f.Status == Created)
            {
                Assert.IsTrue(true);
            }
            // Cancel Game Request
            Token cancel = new Token();
            cancel.UserToken = r.Data["UserToken"];
            Console.WriteLine("UserToken of Cancel: " + r.Data["UserToken"]);
            Response g = client.DoPutAsync(cancel, "games").Result;
            Assert.AreEqual(OK, g.Status);
        }
    
        [TestMethod]
        public void TestCancelGameForbidden()
        {
            client = new RestTestClient("http://localhost:60000/BoggleService.svc/");
            // Register a User
            UserInfo user = new UserInfo();
            user.Nickname = "testCancelGame";
            Response r = client.DoPostAsync("users", user).Result;
            Assert.AreEqual(OK, r.Status);

            // Join Game
            PostingGame game = new PostingGame();
            game.UserToken = r.Data["UserToken"];
            game.TimeLimit = "100";
            Response f = client.DoPostAsync("games", game).Result;
            if (f.Status == OK || f.Status == Accepted || f.Status == Created)
            {
                Assert.IsTrue(true);
            }
            // Cancel Game Request
            System.Threading.Thread.Sleep(1000);
            Token cancel = new Token();
            cancel.UserToken = "25";
            Console.WriteLine("UserToken of Cancel: " + r.Data["UserToken"]);
            Response g = client.DoPutAsync(cancel, "games").Result;
            Assert.AreEqual(Forbidden, g.Status);
        }

        [TestMethod]
        public void TestGameStatus()
        {
            client = new RestTestClient("http://localhost:60000/BoggleService.svc/");
            // Register a User
            UserInfo user = new UserInfo();
            user.Nickname = "testGameStatus";
            Response r = client.DoPostAsync("users", user).Result;
            Assert.AreEqual(OK, r.Status);

            // Join Game
            PostingGame game = new PostingGame();
            game.UserToken = r.Data["UserToken"];
            game.TimeLimit = "30";
            Response f = client.DoPostAsync("games", game).Result;
            if(f.Status == OK || f.Status == Accepted || f.Status == Created)
            {
                Assert.IsTrue(true);
            }
            else
            {
                Assert.IsFalse(true);
            }

            // Register a User
            UserInfo user2 = new UserInfo();
            user2.Nickname = "test2";
            Response r2 = client.DoPostAsync("users", user2).Result;
            Assert.AreEqual(OK, r2.Status);

            // Join Game
            PostingGame game2 = new PostingGame();
            game2.UserToken = r2.Data["UserToken"];
            game2.TimeLimit = "30";
            Response f2 = client.DoPostAsync("games", game2).Result;
            Assert.AreEqual(Accepted, f2.Status);

            // Do Game Status
            string gameId = f.Data["GameID"];
            Response e = client.DoGetAsync("games/{0}", gameId).Result;
            Assert.AreEqual(OK, e.Status);
        }

        [TestMethod]
        public void TestGameStatusPending()
        {
            client = new RestTestClient("http://localhost:60000/BoggleService.svc/");
            // Register a User
            UserInfo user = new UserInfo();
            user.Nickname = "testGameStatus";
            Response r = client.DoPostAsync("users", user).Result;
            Assert.AreEqual(OK, r.Status);

            // Join Game
            PostingGame game = new PostingGame();
            game.UserToken = r.Data["UserToken"];
            game.TimeLimit = "30";
            Response f = client.DoPostAsync("games", game).Result;
            if (f.Status == OK || f.Status == Accepted || f.Status == Created)
            {
                Assert.IsTrue(true);
            }
            else
            {
                Assert.IsFalse(true);
            }

            // Do Game Status
            string gameId = f.Data["GameID"];
            Response e = client.DoGetAsync("games/{0}", gameId).Result;
            Assert.AreEqual(OK, e.Status);
        }

        [TestMethod]
        public void TestGameStatusGameIDInvalid()
        {
            client = new RestTestClient("http://localhost:60000/BoggleService.svc/");
            // Register a User
            UserInfo user = new UserInfo();
            user.Nickname = "testGameStatus";
            Response r = client.DoPostAsync("users", user).Result;
            Assert.AreEqual(OK, r.Status);

            // Join Game
            PostingGame game = new PostingGame();
            game.UserToken = r.Data["UserToken"];
            game.TimeLimit = "30";
            Response f = client.DoPostAsync("games", game).Result;
            if (f.Status == OK || f.Status == Accepted || f.Status == Created)
            {
                Assert.IsTrue(true);
            }
            else
            {
                Assert.IsFalse(true);
            }

            // Register a User
            UserInfo user2 = new UserInfo();
            user2.Nickname = "test2";
            Response r2 = client.DoPostAsync("users", user2).Result;
            Assert.AreEqual(OK, r2.Status);

            // Join Game
            PostingGame game2 = new PostingGame();
            game2.UserToken = r2.Data["UserToken"];
            game2.TimeLimit = "30";
            Response f2 = client.DoPostAsync("games", game2).Result;
            Assert.AreEqual(Created, f2.Status);

            // Do Game Status
            string gameId = "asdf";
            Response e = client.DoGetAsync("games/{0}", gameId).Result;
            Assert.AreEqual(Forbidden, e.Status);
        }

        [TestMethod]
        public void TestGameStatusBrief()
        {
            client = new RestTestClient("http://localhost:60000/BoggleService.svc/");
            // Register a User
            UserInfo user = new UserInfo();
            user.Nickname = "testGameStatusBreif";
            Response r = client.DoPostAsync("users", user).Result;
            Assert.AreEqual(OK, r.Status);

            // Join Game
            PostingGame game = new PostingGame();
            game.UserToken = r.Data["UserToken"];
            game.TimeLimit = "30";
            Response f = client.DoPostAsync("games", game).Result;
            if (f.Status == OK || f.Status == Accepted || f.Status == Created)
            {
                Assert.IsTrue(true);
            }

            // Register a User
            UserInfo user2 = new UserInfo();
            user2.Nickname = "test2";
            Response r2 = client.DoPostAsync("users", user2).Result;
            Assert.AreEqual(OK, r2.Status);

            // Join Game
            PostingGame game2 = new PostingGame();
            game2.UserToken = r2.Data["UserToken"];
            game2.TimeLimit = "30";
            Response f2 = client.DoPostAsync("games", game2).Result;
            Assert.AreEqual(Created, f2.Status);

            // Do Game Status
            string gameId = f2.Data["GameID"];
            Response e = client.DoGetAsync("games/{0}?Brief={1}",gameId, "yes").Result;
            Assert.AreEqual(OK, e.Status);
            if (e.Data["GameState"] != "pending")
            {
                Assert.IsNotNull(e.Data["TimeLeft"]);
                Assert.IsNotNull(e.Data["Player1"]);
                Assert.IsNotNull(e.Data["Player2"]);
            }
        }

        [TestMethod]
        public void TestGameStatusBriefComplete()
        {
            client = new RestTestClient("http://localhost:60000/BoggleService.svc/");
            // Register a User
            UserInfo user = new UserInfo();
            user.Nickname = "testGameStatusBreif";
            Response r = client.DoPostAsync("users", user).Result;
            Assert.AreEqual(OK, r.Status);

            // Join Game
            PostingGame game = new PostingGame();
            game.UserToken = r.Data["UserToken"];
            game.TimeLimit = "5";
            Response f = client.DoPostAsync("games", game).Result;
            if (f.Status == OK || f.Status == Accepted || f.Status == Created)
            {
                Assert.IsTrue(true);
            }

            // Register a User
            UserInfo user2 = new UserInfo();
            user2.Nickname = "test2";
            Response r2 = client.DoPostAsync("users", user2).Result;
            Assert.AreEqual(OK, r2.Status);

            // Join Game
            PostingGame game2 = new PostingGame();
            game2.UserToken = r2.Data["UserToken"];
            game2.TimeLimit = "5";
            Response f2 = client.DoPostAsync("games", game2).Result;
            Assert.AreEqual(Created, f2.Status);

            System.Threading.Thread.Sleep(6000);

            // Do Game Status
            string gameId = f2.Data["GameID"];
            Response e = client.DoGetAsync("games/{0}?Brief={1}", gameId, "yes").Result;
            Assert.AreEqual(OK, e.Status);
            string gamestate = e.Data["GameState"];
            if (gamestate.Equals("completed"))
            {
                Assert.IsNotNull(e.Data["TimeLeft"]);
                Assert.IsNotNull(e.Data["Player1"]);
                Assert.IsNotNull(e.Data["Player2"]);
            }
            else
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void TestGameStatusComplete()
        {
            client = new RestTestClient("http://localhost:60000/BoggleService.svc/");
            // Register a User
            UserInfo user = new UserInfo();
            user.Nickname = "testGameStatusBreif";
            Response r = client.DoPostAsync("users", user).Result;
            Assert.AreEqual(OK, r.Status);

            // Join Game
            PostingGame game = new PostingGame();
            game.UserToken = r.Data["UserToken"];
            game.TimeLimit = "5";
            Response f = client.DoPostAsync("games", game).Result;
            if (f.Status == OK || f.Status == Accepted || f.Status == Created)
            {
                Assert.IsTrue(true);
            }

            // Register a User
            UserInfo user2 = new UserInfo();
            user2.Nickname = "test2";
            Response r2 = client.DoPostAsync("users", user2).Result;
            Assert.AreEqual(OK, r2.Status);

            // Join Game
            PostingGame game2 = new PostingGame();
            game2.UserToken = r2.Data["UserToken"];
            game2.TimeLimit = "5";
            Response f2 = client.DoPostAsync("games", game2).Result;
            Assert.AreEqual(Created, f2.Status);

            System.Threading.Thread.Sleep(6000);

            // Do Game Status
            string gameId = f2.Data["GameID"];
            Response e = client.DoGetAsync("games/{0}?Brief={1}", gameId, "no").Result;
            Assert.AreEqual(OK, e.Status);
            string gamestate = e.Data["GameState"];
            if (gamestate.Equals("completed"))
            {
                Assert.IsNotNull(e.Data["TimeLeft"]);
                Assert.IsNotNull(e.Data["Player1"]);
                Assert.IsNotNull(e.Data["Player2"]);
                Assert.IsNotNull(e.Data["Board"]);
                Assert.IsNotNull(e.Data["Player1"].Score);

            }
            else
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void TestGameStatusForbidden()
        {
            client = new RestTestClient("http://localhost:60000/BoggleService.svc/");
            // Register a User
            UserInfo user = new UserInfo();
            user.Nickname = "testGameStatusForbidden";
            Response r = client.DoPostAsync("users", user).Result;
            Assert.AreEqual(OK, r.Status);

            // Join Game
            PostingGame game = new PostingGame();
            game.UserToken = r.Data["UserToken"];
            game.TimeLimit = "30";
            Response f = client.DoPostAsync("games", game).Result;
            if (f.Status == OK || f.Status == Accepted || f.Status == Created)
            {
                Assert.IsTrue(true);
            }

            // Do Game Status Forbidden
            Response e = client.DoGetAsync("games/{0}", "1234123").Result;
            Assert.AreEqual(Forbidden, e.Status);
        }

        [TestMethod]
        public void TestGameStatusOutput()
        {
            client = new RestTestClient("http://localhost:60000/BoggleService.svc/");
            // Register a User
            UserInfo user = new UserInfo();
            user.Nickname = "testGameStatusOutput";
            Response r = client.DoPostAsync("users", user).Result;
            Assert.AreEqual(OK, r.Status);

            // Join Game
            PostingGame game = new PostingGame();
            game.UserToken = r.Data["UserToken"];
            game.TimeLimit = "31";
            Response f = client.DoPostAsync("games", game).Result;
            //Assert.AreEqual(Accepted, f.Status);

            //// Register a User
            //UserInfo user2 = new UserInfo();
            //user2.Nickname = "test2";
            //Response r2 = client.DoPostAsync("users", user2).Result;
            //Assert.AreEqual(OK, r2.Status);

            //// Join Game
            //PostingGame game2 = new PostingGame();
            //game2.UserToken = r2.Data["UserToken"];
            //game2.TimeLimit = "31";
            //Response f2 = client.DoPostAsync("games", game2).Result;
            //Assert.AreEqual(Created, f2.Status);

            string gameID = f.Data["GameID"];

            // Do Game Status
            Response e = client.DoGetAsync("games/{0}?Brief={1}", gameID, "no").Result; ;
            Assert.AreEqual(OK, e.Status);
            if (e.Data["GameState"] == "active")
            {
                Assert.IsNotNull(e.Data["GameState"]);
                Assert.IsNotNull(e.Data["TimeLeft"]);
                Assert.IsNotNull(e.Data["TimeLimit"]);
                Assert.IsNotNull(e.Data["Board"]);
                Assert.IsNotNull(e.Data["Player1"]);
                Assert.IsNotNull(e.Data["Player2"]);
            }
            else if (e.Data["GameState"] == "completed")
            {
                Assert.IsNotNull(e.Data["GameState"]);
                Assert.IsNotNull(e.Data["TimeLeft"]);
                Assert.IsNotNull(e.Data["TimeLimit"]);
                Assert.IsNotNull(e.Data["Board"]);
                Assert.IsNotNull(e.Data["Player1"]);
                Assert.IsNotNull(e.Data["Player2"]);
            }
            else
            {
                Assert.IsNotNull(e.Data["GameState"]);
            }
            // Add in Completed Test and Pending
        }

        [TestMethod]
        public void TestPlayWord()
        {
            client = new RestTestClient("http://localhost:60000/BoggleService.svc/");
            // Register a User
            UserInfo user = new UserInfo();
            user.Nickname = "test";
            Response r = client.DoPostAsync("users", user).Result;
            Assert.AreEqual(OK, r.Status);

            // Join Game
            PostingGame game = new PostingGame();
            game.UserToken = r.Data["UserToken"];
            game.TimeLimit = "30";
            Response f = client.DoPostAsync("games", game).Result;
            Assert.AreEqual(Accepted, f.Status);

            // Register a User
            UserInfo user2 = new UserInfo();
            user2.Nickname = "test2";
            Response r2 = client.DoPostAsync("users", user2).Result;
            Assert.AreEqual(OK, r2.Status);

            // Join Game
            PostingGame game2 = new PostingGame();
            game2.UserToken = r2.Data["UserToken"];
            game2.TimeLimit = "30";
            Response f2 = client.DoPostAsync("games", game2).Result;
            Assert.AreEqual(Created, f2.Status);

            // Get Game Status
            string gameID = f2.Data["GameID"];
            Response e = client.DoGetAsync("games/{0}", gameID).Result;
            Assert.AreEqual(OK, e.Status);

            // Wait until game has started
            while (e.Data["GameState"] != "active")
            {
                e = client.DoGetAsync("games/{0}", gameID).Result;
                System.Threading.Thread.Sleep(1000);

            }
            char[] chararray = new char[25];

            // Do Play Word
            for(int i = 65; i <= i + 26;i++)
            {
                int j = 0;
                char c = (char)i;
                chararray[j] = c;
                j++;
            }
            PlayedWord word = new PlayedWord();
            word.UserToken = r.Data["UserToken"];
            foreach (char c in chararray)
            {
                word.Word = "" + c;
                string url = String.Format("games/{0}", gameID);
                Response g = client.DoPutAsync(word, url).Result;
                Assert.AreEqual(OK, g.Status);
            }

        }


        [TestMethod]
        public void TestPlayWordWordonBoardP2()
        {
            client = new RestTestClient("http://localhost:60000/BoggleService.svc/");
            // Register a User
            UserInfo user = new UserInfo();
            user.Nickname = "test";
            Response r = client.DoPostAsync("users", user).Result;
            Assert.AreEqual(OK, r.Status);

            // Join Game
            PostingGame game = new PostingGame();
            game.UserToken = r.Data["UserToken"];
            game.TimeLimit = "30";
            Response f = client.DoPostAsync("games", game).Result;
            Assert.AreEqual(Accepted, f.Status);

            // Register a User
            UserInfo user2 = new UserInfo();
            user2.Nickname = "boardtest";
            Response r2 = client.DoPostAsync("users", user2).Result;
            Assert.AreEqual(OK, r2.Status);

            // Join Game
            PostingGame game2 = new PostingGame();
            game2.UserToken = r2.Data["UserToken"];
            game2.TimeLimit = "30";
            Response f2 = client.DoPostAsync("games", game2).Result;
            Assert.AreEqual(Created, f2.Status);

            // Get Game Status
            string gameID = f2.Data["GameID"];
            Response e = client.DoGetAsync("games/{0}", gameID).Result;
            Assert.AreEqual(OK, e.Status);

            // Wait until game has started
            while (e.Data["GameState"] != "active")
            {
                e = client.DoGetAsync("games/{0}", gameID).Result;
                System.Threading.Thread.Sleep(1000);

            }
            string[] stringarray = new string[] { "NAME", "pain", "rain", "GAIN" };

            string[] stringarray2 = new string[] { "NAME", "pain", "rain", "GAIN" };


            PlayedWord word = new PlayedWord();
            word.UserToken = r2.Data["UserToken"];

            foreach (string s in stringarray)
            {
                word.Word = s;
                string url = String.Format("games/{0}", gameID);
                Response g = client.DoPutAsync(word, url).Result;
                Assert.AreEqual(OK, g.Status);
                int score = g.Data["Score"];
                Assert.IsTrue(score == 1);
            }

            foreach (string s in stringarray)
            {
                word.Word = s;
                string url = String.Format("games/{0}", gameID);
                Response g = client.DoPutAsync(word, url).Result;
                Assert.AreEqual(OK, g.Status);
                int score = g.Data["Score"];
                Assert.IsTrue(score == 0);
            }


            // Get Game Status
            Response h = client.DoGetAsync("games/{0}", gameID).Result;
            Assert.AreEqual(OK, e.Status);
            int scoreend = h.Data["Player2"].Score;
            Assert.IsTrue(scoreend == 4);
        }

        [TestMethod]
        public void TestPlayWordWordonBoardP1()
        {
            client = new RestTestClient("http://localhost:60000/BoggleService.svc/");
            // Register a User
            UserInfo user = new UserInfo();
            user.Nickname = "test";
            Response r = client.DoPostAsync("users", user).Result;
            Assert.AreEqual(OK, r.Status);

            // Join Game
            PostingGame game = new PostingGame();
            game.UserToken = r.Data["UserToken"];
            game.TimeLimit = "30";
            Response f = client.DoPostAsync("games", game).Result;
            Assert.AreEqual(Accepted, f.Status);

            // Register a User
            UserInfo user2 = new UserInfo();
            user2.Nickname = "boardtest";
            Response r2 = client.DoPostAsync("users", user2).Result;
            Assert.AreEqual(OK, r2.Status);

            // Join Game
            PostingGame game2 = new PostingGame();
            game2.UserToken = r2.Data["UserToken"];
            game2.TimeLimit = "30";
            Response f2 = client.DoPostAsync("games", game2).Result;
            Assert.AreEqual(Created, f2.Status);

            // Get Game Status
            string gameID = f2.Data["GameID"];
            Response e = client.DoGetAsync("games/{0}", gameID).Result;
            Assert.AreEqual(OK, e.Status);

            // Wait until game has started
            while (e.Data["GameState"] != "active")
            {
                e = client.DoGetAsync("games/{0}", gameID).Result;
                System.Threading.Thread.Sleep(1000);

            }
            string[] stringarray = new string[] {"NAME", "pain", "rain", "GAIN" };

            PlayedWord word = new PlayedWord();
            word.UserToken = r.Data["UserToken"];

            foreach (string s in stringarray)
            {
                word.Word = s;
                string url = String.Format("games/{0}", gameID);
                Response g = client.DoPutAsync(word, url).Result;
                Assert.AreEqual(OK, g.Status);
                int score = g.Data["Score"];
                Assert.IsTrue(score == 1);
            }

            foreach (string s in stringarray)
            {
                word.Word = s;
                string url = String.Format("games/{0}", gameID);
                Response g = client.DoPutAsync(word, url).Result;
                Assert.AreEqual(OK, g.Status);
                int score = g.Data["Score"];
                Assert.IsTrue(score == 0);
            }

            // Get Game Status
            Response h = client.DoGetAsync("games/{0}", gameID).Result;
            Assert.AreEqual(OK, e.Status);
            int scoreend = h.Data["Player1"].Score;
            Assert.IsTrue(scoreend == 4);
        }

        [TestMethod]
        public void TestPlayWordWordNotBoardP1()
        {
            client = new RestTestClient("http://localhost:60000/BoggleService.svc/");
            // Register a User
            UserInfo user = new UserInfo();
            user.Nickname = "test";
            Response r = client.DoPostAsync("users", user).Result;
            Assert.AreEqual(OK, r.Status);

            // Join Game
            PostingGame game = new PostingGame();
            game.UserToken = r.Data["UserToken"];
            game.TimeLimit = "30";
            Response f = client.DoPostAsync("games", game).Result;
            Assert.AreEqual(Accepted, f.Status);

            // Register a User
            UserInfo user2 = new UserInfo();
            user2.Nickname = "boardtest";
            Response r2 = client.DoPostAsync("users", user2).Result;
            Assert.AreEqual(OK, r2.Status);

            // Join Game
            PostingGame game2 = new PostingGame();
            game2.UserToken = r2.Data["UserToken"];
            game2.TimeLimit = "30";
            Response f2 = client.DoPostAsync("games", game2).Result;
            Assert.AreEqual(Created, f2.Status);

            // Get Game Status
            string gameID = f2.Data["GameID"];
            Response e = client.DoGetAsync("games/{0}", gameID).Result;
            Assert.AreEqual(OK, e.Status);

            // Wait until game has started
            while (e.Data["GameState"] != "active")
            {
                e = client.DoGetAsync("games/{0}", gameID).Result;
                System.Threading.Thread.Sleep(1000);

            }
            string[] stringarray = new string[] { "NMI", "pnga", "arng", "GrN" };

            PlayedWord word = new PlayedWord();
            word.UserToken = r.Data["UserToken"];

            foreach (string s in stringarray)
            {
                word.Word = s;
                string url = String.Format("games/{0}", gameID);
                Response g = client.DoPutAsync(word, url).Result;
                Assert.AreEqual(OK, g.Status);
                int score = g.Data["Score"];
                Assert.IsTrue(score == -1);
            }

            // Get Game Status
            Response h = client.DoGetAsync("games/{0}", gameID).Result;
            Assert.AreEqual(OK, e.Status);
            int scoreend = h.Data["Player1"].Score;
            Assert.IsTrue(scoreend == -4);
        }

        [TestMethod]
        public void TestPlayWordWordNotBoardP2()
        {
            client = new RestTestClient("http://localhost:60000/BoggleService.svc/");
            // Register a User
            UserInfo user = new UserInfo();
            user.Nickname = "test";
            Response r = client.DoPostAsync("users", user).Result;
            Assert.AreEqual(OK, r.Status);

            // Join Game
            PostingGame game = new PostingGame();
            game.UserToken = r.Data["UserToken"];
            game.TimeLimit = "30";
            Response f = client.DoPostAsync("games", game).Result;
            Assert.AreEqual(Accepted, f.Status);

            // Register a User
            UserInfo user2 = new UserInfo();
            user2.Nickname = "boardtest";
            Response r2 = client.DoPostAsync("users", user2).Result;
            Assert.AreEqual(OK, r2.Status);

            // Join Game
            PostingGame game2 = new PostingGame();
            game2.UserToken = r2.Data["UserToken"];
            game2.TimeLimit = "30";
            Response f2 = client.DoPostAsync("games", game2).Result;
            Assert.AreEqual(Created, f2.Status);

            // Get Game Status
            string gameID = f2.Data["GameID"];
            Response e = client.DoGetAsync("games/{0}", gameID).Result;
            Assert.AreEqual(OK, e.Status);

            // Wait until game has started
            while (e.Data["GameState"] != "active")
            {
                e = client.DoGetAsync("games/{0}", gameID).Result;
                System.Threading.Thread.Sleep(1000);

            }
            string[] stringarray = new string[] { "NMI", "pnga", "arng", "GrN" };

            PlayedWord word = new PlayedWord();
            word.UserToken = r2.Data["UserToken"];

            foreach (string s in stringarray)
            {
                word.Word = s;
                string url = String.Format("games/{0}", gameID);
                Response g = client.DoPutAsync(word, url).Result;
                Assert.AreEqual(OK, g.Status);
                int score = g.Data["Score"];
                Assert.IsTrue(score == -1);
            }

            // Get Game Status
            Response h = client.DoGetAsync("games/{0}", gameID).Result;
            Assert.AreEqual(OK, e.Status);
            int scoreend = h.Data["Player2"].Score;
            Assert.IsTrue(scoreend == -4);
        }

        [TestMethod]
        public void TestPlayWordNickname()
        {
            client = new RestTestClient("http://localhost:60000/BoggleService.svc/");
            // Register a User
            UserInfo user = new UserInfo();
            user.Nickname = "test";
            Response r = client.DoPostAsync("users", user).Result;
            Assert.AreEqual(OK, r.Status);

            // Join Game
            PostingGame game = new PostingGame();
            game.UserToken = r.Data["UserToken"];
            game.TimeLimit = "30";
            Response f = client.DoPostAsync("games", game).Result;
            Assert.AreEqual(Accepted, f.Status);

            // Register a User
            UserInfo user2 = new UserInfo();
            user2.Nickname = "test";
            Response r2 = client.DoPostAsync("users", user2).Result;
            Assert.AreEqual(OK, r2.Status);

            // Join Game
            PostingGame game2 = new PostingGame();
            game2.UserToken = r2.Data["UserToken"];
            game2.TimeLimit = "30";
            Response f2 = client.DoPostAsync("games", game2).Result;
            Assert.AreEqual(Created, f2.Status);

            // Get Game Status
            string gameID = f2.Data["GameID"];
            Response e = client.DoGetAsync("games/{0}", gameID).Result;
            Assert.AreEqual(OK, e.Status);

            // Wait until game has started
            while (e.Data["GameState"] != "active")
            {
                e = client.DoGetAsync("games/{0}", gameID).Result;
                System.Threading.Thread.Sleep(1000);

            }
            char[] chararray = new char[25];

            // Do Play Word
            for (int i = 65; i <= i + 26; i++)
            {
                int j = 0;
                char c = (char)i;
                chararray[j] = c;
                j++;
            }
            PlayedWord word = new PlayedWord();
            word.UserToken = r.Data["UserToken"];
            foreach (char c in chararray)
            {
                word.Word = "" + c;
                string url = String.Format("games/{0}", gameID);
                Response g = client.DoPutAsync(word, url).Result;
                Assert.AreEqual(Forbidden, g.Status);
            }

        }


        [TestMethod]
        public void TestPlayedWordMultipleTimes()
        {
            client = new RestTestClient("http://localhost:60000/BoggleService.svc/");
            // Register a User
            UserInfo user = new UserInfo();
            user.Nickname = "test";
            Response r = client.DoPostAsync("users", user).Result;
            Assert.AreEqual(OK, r.Status);

            // Join Game
            PostingGame game = new PostingGame();
            game.UserToken = r.Data["UserToken"];
            game.TimeLimit = "30";
            Response f = client.DoPostAsync("games", game).Result;
            Assert.AreEqual(Accepted, f.Status);

            // Register a User
            UserInfo user2 = new UserInfo();
            user2.Nickname = "test2";
            Response r2 = client.DoPostAsync("users", user2).Result;
            Assert.AreEqual(OK, r2.Status);

            // Join Game
            PostingGame game2 = new PostingGame();
            game2.UserToken = r2.Data["UserToken"];
            game2.TimeLimit = "30";
            Response f2 = client.DoPostAsync("games", game2).Result;
            Assert.AreEqual(Created, f2.Status);

            // Get Game Status
            string gameID = f2.Data["GameID"];
            Response e = client.DoGetAsync("games/{0}", gameID).Result;
            Assert.AreEqual(OK, e.Status);

            // Wait until game has started
            while (e.Data["GameState"] != "active")
            {
                e = client.DoGetAsync("games/{0}", gameID).Result;
                System.Threading.Thread.Sleep(1000);

            }
            char[] chararray = new char[25];

            // Do Play Word
            for (int i = 65; i <= i + 26; i++)
            {
                int j = 0;
                char c = (char)i;
                chararray[j] = c;
                j++;
            }
            PlayedWord word = new PlayedWord();
            word.UserToken = r.Data["UserToken"];
            foreach (char c in chararray)
            {
                word.Word = "" + c;
                string url = String.Format("games/{0}", gameID);
                Response g = client.DoPutAsync(word, url).Result;
                Assert.AreEqual(OK, g.Status);
            }

            foreach (char c in chararray)
            {
                word.Word = "" + c;
                string url = String.Format("games/{0}", gameID);
                Response g = client.DoPutAsync(word, url).Result;
                Assert.AreEqual(OK, g.Status);
            }
        }

        [TestMethod]
        public void TestPlayWordForbidden()
        {
            client = new RestTestClient("http://localhost:60000/BoggleService.svc/");
            // Register a User
            UserInfo user = new UserInfo();
            user.Nickname = "test";
            Response r = client.DoPostAsync("users", user).Result;
            Assert.AreEqual(OK, r.Status);

            // Join Game
            PostingGame game = new PostingGame();
            game.UserToken = r.Data["UserToken"];
            game.TimeLimit = "30";
            Response f = client.DoPostAsync("games", game).Result;
            Assert.AreEqual(Accepted, f.Status);

            // Register a User
            UserInfo user2 = new UserInfo();
            user2.Nickname = "test2";
            Response r2 = client.DoPostAsync("users", user2).Result;
            Assert.AreEqual(OK, r2.Status);

            // Join Game
            PostingGame game2 = new PostingGame();
            game2.UserToken = r2.Data["UserToken"];
            game2.TimeLimit = "30";
            Response f2 = client.DoPostAsync("games", game2).Result;
            Assert.AreEqual(Created, f2.Status);

            // Get Game Status
            string gameID = f2.Data["GameID"];
            Response e = client.DoGetAsync("games/{0}", gameID).Result;
            Assert.AreEqual(OK, e.Status);

            // Wait until game has started
            while (e.Data["GameState"] != "active")
            {
                e = client.DoGetAsync("games/{0}", gameID).Result;
                System.Threading.Thread.Sleep(1000);

            }
            // Do Play Word
            PlayedWord word = new PlayedWord();
            word.UserToken = "";
            word.Word = "asd";
            string url = String.Format("games/{0}", gameID);
            Response g = client.DoPutAsync(word, url).Result;
            Assert.AreEqual(Forbidden, g.Status);
        }

        [TestMethod]
        public void TestPlayWordUserTokenFalse()
        {
            client = new RestTestClient("http://localhost:60000/BoggleService.svc/");
            // Register a User
            UserInfo user = new UserInfo();
            user.Nickname = "test";
            Response r = client.DoPostAsync("users", user).Result;
            Assert.AreEqual(OK, r.Status);

            // Join Game
            PostingGame game = new PostingGame();
            game.UserToken = r.Data["UserToken"];
            game.TimeLimit = "30";
            Response f = client.DoPostAsync("games", game).Result;
            if (f.Status == OK || f.Status == Accepted || f.Status == Created)
            {
                Assert.IsTrue(true);
            }

            // Do Play Word
            PlayedWord word = new PlayedWord();
            word.UserToken = "1235235";
            word.Word = "asd";
            string gameID = f.Data["GameID"];
            string url = String.Format("games/{0}", gameID);
            Response e = client.DoPutAsync(word, url).Result;
        }

        [TestMethod]
        public void TestPlayWordConflict()
        {
            client = new RestTestClient("http://localhost:60000/BoggleService.svc/");
            // Register a User
            UserInfo user = new UserInfo();
            user.Nickname = "test";
            Response r = client.DoPostAsync("users", user).Result;
            Assert.AreEqual(OK, r.Status);

            // Join Game
            PostingGame game = new PostingGame();
            game.UserToken = r.Data["UserToken"];
            game.TimeLimit = "30";
            Response f = client.DoPostAsync("games", game).Result;
            if (f.Status == OK || f.Status == Accepted || f.Status == Created)
            {
                Assert.IsTrue(true);
            }
            // Do Play Word
            PlayedWord word = new PlayedWord();
            word.UserToken = "word"; // Missing usertoken
            word.Word = "word";
            Response e = client.DoPutAsync(word, "games/" + f.Data["GameID"]).Result;
            Assert.AreEqual(Forbidden, e.Status);
        }

    }
}