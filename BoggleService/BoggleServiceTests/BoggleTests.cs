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

        /// <summary>
        /// Note that DoGetAsync (and the other similar methods) returns a Response object, which contains
        /// the response Stats and the deserialized JSON response (if any).  See RestTestClient.cs
        /// for details.
        /// </summary>
        [TestMethod]
        public void TestMethod1()
        {
            Response r = client.DoGetAsync("word?index={0}", "-5").Result;
            Assert.AreEqual(Forbidden, r.Status);

            r = client.DoGetAsync("word?index={0}", "5").Result;
            Assert.AreEqual(OK, r.Status);

            string word = (string)r.Data;
            Assert.AreEqual("AAL", word);
        }

        [TestMethod]
        public void TestRegister()
        {
            UserInfo user = new UserInfo();
            user.Nickname = "test";
            Response r = client.DoPostAsync("users", user).Result;
            Assert.AreEqual(OK, r.Status);
        }

        [TestMethod]
        public void TestRegisterNull()
        {
            UserInfo user = new UserInfo();
            user.Nickname = "";
            Response r = client.DoPostAsync("users", user).Result;
            Assert.AreEqual(Forbidden, r.Status);
        }

        [TestMethod]
        public void TestJoinGame()
        {
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

            // Add Test that checks if it is 201 or 202
        }

        [TestMethod]
        public void TestJoinGameTimeError()
        {
            // Register a User
            UserInfo user = new UserInfo();
            user.Nickname = "test";
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
        public void TestCancelGame()
        {
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
            Assert.AreEqual(OK, f.Status);

            // Cancel Game Request
            Token cancel = new Token();
            cancel.UserToken = r.Data["UserToken"];
            Response e = client.DoPutAsync("games", cancel.UserToken).Result;
            Assert.AreEqual(OK, f.Status);
        }

        [TestMethod]
        public void TestGameStatus()
        {
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
            Assert.AreEqual(OK, f.Status);

            // Do Game Status
            Response e = client.DoGetAsync("games/{0}", f.Data["GameID"]).Result;
            Assert.AreEqual(OK, e.Status);
        }

        [TestMethod]
        public void TestGameStatusForbidden()
        {
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
            Assert.AreEqual(OK, f.Status);

            // Do Game Status Forbidden
            Response e = client.DoGetAsync("games/{0}", "1234123").Result;
            Assert.AreEqual(Forbidden, e.Status);
        }

        [TestMethod]
        public void TestGameStatusOutput()
        {
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
            Assert.AreEqual(OK, f.Status);

            // Do Game Status
            Response e = client.DoGetAsync("games/{0}", f.Data["GameID"]).Result;
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
            // Add in Completed Test and Pending
        }

        [TestMethod]
        public void TestPlayWord()
        {
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
            Assert.AreEqual(OK, f.Status);

            // Do Play Word
            PlayedWord word = new PlayedWord();
            word.UserToken = r.Data["UserToken"];
            word.Word = "word";
            Response e = client.DoPutAsync(word, "games/" + f.Data["GameID"]).Result;
            Assert.AreEqual(OK, e.Status);
        }

        [TestMethod]
        public void TestPlayWordForbidden()
        {
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
            Assert.AreEqual(OK, f.Status);

            // Do Play Word
            PlayedWord word = new PlayedWord();
            word.UserToken = ""; // Missing usertoken
            word.Word = "word";
            Response e = client.DoPutAsync(word, "games/" + f.Data["GameID"]).Result;
            Assert.AreEqual(Forbidden, e.Status);
        }

        [TestMethod]
        public void TestPlayWordConflict()
        {
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
            Assert.AreEqual(OK, f.Status);
            
            // Find better way to check if game state
            /*
            // Do Play Word
            PlayedWord word = new PlayedWord();
            word.UserToken = "word"; // Missing usertoken
            word.Word = "word";
            Response e = client.DoPutAsync(word, "games/" + f.Data["GameID"]).Result;
            Assert.AreEqual(Forbidden, e.Status);
            */
        }

    }
}