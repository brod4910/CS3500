using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.ServiceModel.Web;
using System.Threading;
using static System.Net.HttpStatusCode;
using System.Data.SqlClient;

namespace Boggle
{
    public class BoggleService : IBoggleService
    {
        private readonly static Dictionary<string, UserInfo> users = new Dictionary<string, UserInfo>();
        private readonly static HashSet<PendingGame> PendingGames = new HashSet<PendingGame>();
        private readonly static Dictionary<string, Status> activeGames = new Dictionary<string, Status>();
        private readonly static HashSet<String> Dictionary = dictionary();
        private static readonly object sync = new object();
        private static bool board = false;

        // The connection string to the DB
        private static string BoggleDB;

        static BoggleService()
        {
            // Saves the connection string for the database.  A connection string contains the
            // information necessary to connect with the database server.  When you create a
            // DB, there is generally a way to obtain the connection string.  From the Server
            // Explorer pane, obtain the properties of DB to see the connection string.

            // The connection string of my ToDoDB.mdf shows as
            //
            //    Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename="C:\Users\zachary\Source\CS 3500 S16\examples\ToDoList\ToDoListDB\App_Data\ToDoDB.mdf";Integrated Security=True
            //
            // Unfortunately, this is absolute pathname on my computer, which means that it
            // won't work if the solution is moved.  Fortunately, it can be shorted to
            //
            //    Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename="|DataDirectory|\ToDoDB.mdf";Integrated Security=True
            //
            // You should shorten yours this way as well.
            //
            // Rather than build the connection string into the program, I store it in the Web.config
            // file where it can be easily found and changed.  You should do that too.
            BoggleDB = ConfigurationManager.ConnectionStrings["BoggleDB"].ConnectionString;
        }

        /// <summary>
        /// The most recent call to SetStatus determines the response code used when
        /// an http response is sent.
        /// </summary>
        /// <param name="status"></param>
        private static void SetStatus(HttpStatusCode status)
        {
            WebOperationContext.Current.OutgoingResponse.StatusCode = status;
        }

        /// <summary>
        /// Returns a Stream version of index.html.
        /// </summary>
        /// <returns></returns>
        public Stream API()
        {
            SetStatus(OK);
            WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";
            return File.OpenRead(AppDomain.CurrentDomain.BaseDirectory + "index.html");
        }


        /// <summary>
        /// Registers a new user.
        /// If Nickname is null, or is empty when trimmed, responds with status 403 (Forbidden).
        /// Otherwise, creates a new user with a unique UserToken and the trimmed Nickname. 
        /// The returned UserToken should be used to identify the user in subsequent requests. 
        /// Responds with status 201 (Created).
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Token Register(UserInfo user)
        {

            if (user.Nickname != null && user.Nickname.Equals("boardtest"))
            {
                board = true;
            }
            else
            {
                board = false;
            }

            if (user.Nickname == null || user.Nickname.Trim().Length == 0)
            {
                SetStatus(Forbidden);
                return null;
            }

            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();

                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    using (SqlCommand command = new SqlCommand("insert into Users (UserID, Nickname) values (@UserID, @Nickname)", conn, trans))
                    {
                        string UserID = Guid.NewGuid().ToString();

                        command.Parameters.AddWithValue("@UserID", UserID);
                        command.Parameters.AddWithValue("@Nickname", user.Nickname);

                       if(command.ExecuteNonQuery() == 0)
                       {
                            command.ExecuteNonQuery();
                       }

                        SetStatus(Created);
                        trans.Commit();
                        return new Token() { UserToken = UserID };
                    }
                }
            }
        }
        /// <summary>
        /// Checks to see if the word is valid in the dictionary
        /// </summary>
        private bool wordIsValid(string word)
        {
            if (word == null || word.Trim().Length == 0)
            {
                return false;
            }

            string line;
            using (StreamReader file = new System.IO.StreamReader(AppDomain.CurrentDomain.BaseDirectory + "dictionary.txt"))
            {
                while ((line = file.ReadLine()) != null)
                {
                    if(line.Equals(word.Trim().ToUpper()))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the hashset of the dictionary for word validation
        /// </summary>
        /// <returns></returns>
        private static HashSet<String> dictionary()
        {
            HashSet<String> dict = new HashSet<string>();

            string line;
            using (StreamReader file = new System.IO.StreamReader(AppDomain.CurrentDomain.BaseDirectory + "dictionary.txt"))
            {
                while ((line = file.ReadLine()) != null)
                {
                    dict.Add(line);
                }
            }

            return dict;
        }

        /// <summary>
        /// Joins a game/Creates a new game
        /// If UserToken is invalid, TimeLimit  5, or TimeLimit  120, responds with status 403 (Forbidden).
        /// Otherwise, if UserToken is already a player in the pending game, responds with status 409 (Conflict).
        /// Otherwise, if there is already one player in the pending game, adds UserToken as the second player. 
        /// The pending game becomes active and a new pending game with no players is created. 
        /// The active game's time limit is the integer average of the time limits requested by the two players. 
        /// Returns the new active game's GameID (which should be the same as the old pending game's GameID). 
        /// Responds with status 201 (Created).
        /// Otherwise, adds UserToken as the first player of the pending game, 
        /// and the TimeLimit as the pending game's requested time limit. 
        /// Returns the pending game's GameID. Responds with status 202 (Accepted).
        /// </summary>
        /// <param name="postingGame"></param>
        /// <returns></returns>
        public GameId JoinGame(PostingGame postingGame)
        {
            int timeLimit;
            int.TryParse(postingGame.TimeLimit, out timeLimit);
            int GameId = -1;
            GameId ID;

            if (timeLimit < 5 || timeLimit > 120)
            {
                SetStatus(Forbidden);
                return null;
            }

            //Set up connection
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();

                //set up transaction
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    //Check to see if the player posting the game is in a pending game
                    using (SqlCommand command = new SqlCommand("Select Player1, Player2 from Games where Player1 = @UserID and Player2 IS NULL", conn, trans))
                    {
                        command.Parameters.AddWithValue("@UserID", postingGame.UserToken);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while(reader.HasRows)
                            {
                                SetStatus(Conflict);
                                trans.Commit();
                                return null;
                            }
                        }
                    }

                    using (SqlCommand command = new SqlCommand("Select Player2, GameID, TimeLimit from Games where Player2 IS NULL", conn, trans))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if(reader.Read())
                            {
                                GameId = (int)reader["GameID"];
                                timeLimit = (int)reader["TimeLimit"];
                            }
                        }
                    }

                    string query;

                    if(GameId == -1)
                    {
                        query = "insert into Games (Player1, TimeLimit) output inserted.GameID values(@Player1, @TimeLimit)";
                    }
                    else
                    {
                        query = "update Games set Player2=@Player2, TimeLimit=@TimeLimit, Board=@Board, StartTime=@StartTime where GameID=@GameID";
                    }

                    using (SqlCommand command = new SqlCommand(query, conn, trans))
                    {
                        if (GameId == -1)
                        {
                            command.Parameters.AddWithValue("@Player1", postingGame.UserToken);
                            command.Parameters.AddWithValue("@TimeLimit", postingGame.TimeLimit);
                            ID = new GameId() { GameID = command.ExecuteScalar().ToString() };
                            SetStatus(Accepted);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@Player2", postingGame.UserToken);
                            command.Parameters.AddWithValue("@TimeLimit", CalcTimeLimit(timeLimit, postingGame.TimeLimit));
                            if (board)
                            {
                                command.Parameters.AddWithValue("@Board", new BoggleBoard("NAMEPAINRAINGAIN").ToString());
                            }
                            else
                            {
                                command.Parameters.AddWithValue("@Board", new BoggleBoard().ToString());
                            }
                            command.Parameters.AddWithValue("@StartTime", DateTime.Now);
                            command.Parameters.AddWithValue("@GameID", GameId);
                            ID = new GameId() { GameID = GameId + "" };
                            SetStatus(Created);

                            if (command.ExecuteNonQuery() == 0)
                            {
                                command.ExecuteNonQuery();
                            }
                        }

                        trans.Commit();
                        return ID;
                    }
                }
            }
        }

        /// <summary>
        /// Calculates the time limit of the game
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2unparsed"></param>
        /// <returns></returns>
        private int CalcTimeLimit(int t1, string t2unparsed)
        {
            int t2;
            int.TryParse(t2unparsed, out t2);

            return (t1 + t2) / 2;
        }

        /// <summary>
        /// Calculates the time left of the game
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        private int CalculateTimeLeft(Status status)
        {
            int timeLimit;
            int.TryParse(status.TimeLimit, out timeLimit);

            DateTime start = status.datetime;

            DateTime now = DateTime.Now;

            int timeElapsed = (int) now.Subtract(start).TotalSeconds;

            return timeLimit - timeElapsed;
        }

        /// <summary>
        /// Checks to see if the token is a valid
        /// user token.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private bool tokenIsValid(string token)
        {
            if(token == null)
            {
                return false;
            }

            //Set up connection
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();

                //set up transaction
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    //Check to see if the player posting the game is in a pending game
                    using (SqlCommand command = new SqlCommand("Select UserID from Users where UserID = @UserID", conn, trans))
                    {
                        command.Parameters.AddWithValue("@UserID", token);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if(reader.HasRows)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Cancels the users join request.
        /// If UserToken is invalid or is not a player in the pending game, responds with status 403 (Forbidden).
        /// Otherwise, removes UserToken from the pending game and responds with status 200 (OK).
        /// </summary>
        /// <param name="token"></param>
        public void CancelJoin(Token token)
        {
            if(!tokenIsValid(token.UserToken))
            {
                SetStatus(Forbidden);
                return;
            }

            //Set up connection
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();

                //set up transaction
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    //Check to see if the player in a pending game
                    using (SqlCommand command = new SqlCommand("Select Player1, Player2 from Games where Player1 = @UserID and Player2 IS NULL", conn, trans))
                    {
                        command.Parameters.AddWithValue("@UserID", token.UserToken);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            //If the reader does not have rows
                            //user is not in a pending game
                            if(!reader.HasRows)
                            {
                                SetStatus(Forbidden);
                                trans.Commit();
                                return;
                            }
                        }
                    }

                    using (SqlCommand command = new SqlCommand("Delete from Games where Player1 = @UserID and Player2 IS NULL", conn, trans))
                    {
                        command.Parameters.AddWithValue("@UserID", token.UserToken);

                        if(command.ExecuteNonQuery() == 0)
                        {
                            command.ExecuteNonQuery();
                        }
                        trans.Commit();
                        return;
                    }
                }
            }

        }

        /// <summary>
        /// Checks to see if GameID is valid in our database
        /// </summary>
        /// <param name="GameID"></param>
        /// <returns></returns>
        private bool gameidIsValid(string GameID)
        {
            if(GameID == null)
            {
                return false;
            }

            int gameid;
            int.TryParse(GameID, out gameid);

            //Set up connection
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();
                //set up transaction
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    //Check to see if the player is in the game
                    using (SqlCommand command = new SqlCommand("Select GameID from Games where GameID=@GameID", conn, trans))
                    {
                        command.Parameters.AddWithValue("@GameID", GameID);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if(reader.HasRows)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
            }

        }

        private bool wordIsValid(PlayedWord word)
        {
            if (word.Word.Trim().Length == 0 || word.Word == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Checks to see if the user exists in the game
        /// </summary>
        /// <param name="GameID"></param>
        /// <param name="word"></param>
        /// <returns></returns>
        private bool userIsinGame(string GameID, PlayedWord word)
        {
            //Set up connection
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();
                //set up transaction
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    //Check to see if the player is in the game
                    using (SqlCommand command = new SqlCommand("Select GameID, Player1, Player2 from Games where GameID=@GameID and (Player1=@UserID || Player2=@UserID)", conn, trans))
                    {
                        command.Parameters.AddWithValue("@GameID", GameID);
                        command.Parameters.AddWithValue("@UserID", word.UserToken);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Calculates the time left in any given game
        /// true == active game
        /// false == completed game
        /// </summary>
        /// <param name="GameID"></param>
        /// <returns></returns>
        private int CalcTimeLeft(string GameID)
        {
            DateTime gameStart;
            int timeLimit;

            //Set up connection
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();
                //set up transaction
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    //Check to see if the player is in the game
                    using (SqlCommand command = new SqlCommand("Select GameID from Games where GameID=@GameID", conn, trans))
                    {
                        command.Parameters.AddWithValue("@GameID", GameID);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            gameStart = (DateTime)reader["StartTime"];
                            timeLimit = (int)reader["TimeLimit"];
                        }
                    }
                }
            }


            DateTime now = DateTime.Now;

            int timeElapsed = (int)now.Subtract(gameStart).TotalSeconds;

            return timeLimit - timeElapsed;

        }


        /// <summary>
        /// Play a word in a game.
        /// If Word is null or empty when trimmed, or if GameID or UserToken is missing or invalid, or 
        /// if UserToken is not a player in the game identified by GameID, responds with response code 403 (Forbidden).
        /// Otherwise, if the game state is anything other than "active", responds with response code 409 (Conflict).
        /// Otherwise, records the trimmed Word as being played by UserToken in the game identified by GameID. 
        /// Returns the score for Word in the context of the game (e.g. if Word has been played before the score is zero). 
        /// Responds with status 200 (OK). Note: The word is not case sensitive.
        /// </summary>
        /// <param name="GameID"></param>
        /// <param name="word"></param>
        /// <returns></returns>
        public WordScore PlayWord(string GameID, PlayedWord word)
        {
            Status status;

            if(!wordIsValid(word) || !tokenIsValid(word.UserToken) || !gameidIsValid(GameID) || !userIsinGame(GameID, word))
            {
                SetStatus(Forbidden);
                return null;
            }

            if(CalcTimeLeft(GameID) <= 0)
            {
                SetStatus(Conflict);
                return null;
            }


        }

        /// <summary>
        /// Get game status information.
        /// If GameID is invalid, responds with status 403 (Forbidden).
        /// Otherwise, returns information about the game named by GameID as illustrated below. 
        /// Note that the information returned depends on whether "Brief=yes" was included as a 
        /// parameter as well as on the state of the game. 
        /// Responds with status code 200 (OK). 
        /// Note: The Board and Words are not case sensitive.
        /// </summary>
        /// <param name="GameID"></param>
        /// <param name="Option"></param>
        /// <returns></returns>
        public Status Gamestatus(string GameID, string Option)
        {
            int enteredID;
            // Check If It parses correctly
            if (!int.TryParse(GameID, out enteredID))
            {
                SetStatus(Forbidden);
                return null;
            }
            foreach (PendingGame game in PendingGames)
            {
                if (game.GameId.Equals(GameID))
                {
                    SetStatus(OK);
                    Status pending = new Status();
                    pending.GameState = game.GameState;
                    return pending;
                }
            }
            foreach (var game in activeGames)
            {
                if (game.Key.Equals(GameID))
                {
                    SetStatus(OK);
                    Status active = new Status();

                    if (CalculateTimeLeft(game.Value) <= 0)
                    {
                        if (Option != null && Option.Equals("yes"))
                        {
                            active.GameState = "completed";
                            active.TimeLeft = "0";
                            active.Player1.NickName = game.Value.Player1.NickName;
                            active.Player1.Score = game.Value.Player1.Score;
                            active.Player2.NickName = game.Value.Player2.NickName;
                            active.Player2.Score = game.Value.Player2.Score;
                            SetStatus(OK);
                            return active;
                        }
                        else
                        {
                            active.GameState = "completed";
                            active.Board = game.Value.Board;
                            active.TimeLeft = "0";
                            active.Player1.NickName = game.Value.Player1.NickName;
                            active.Player1.WordsPlayed = game.Value.Player1Words;
                            active.Player1.Score = game.Value.Player1.Score;
                            active.Player2.NickName = game.Value.Player2.NickName;
                            active.Player2.WordsPlayed = game.Value.Player2Words;
                            active.Player2.Score = game.Value.Player2.Score;
                            SetStatus(OK);
                            return active;
                        }
                    }
                    else
                    {
                        active.GameState = game.Value.GameState;
                        if (Option != null && Option.Equals("yes"))
                        {
                            active.TimeLeft = CalculateTimeLeft(game.Value) + "";
                            active.Player1.NickName = game.Value.Player1.NickName;
                            active.Player1.Score = game.Value.Player1.Score;
                            active.Player2.NickName = game.Value.Player2.NickName;
                            active.Player2.Score = game.Value.Player2.Score;
                            return active;
                        }
                        else
                        {
                            active.TimeLeft = CalculateTimeLeft(game.Value) + "";
                            active.Player1.NickName = game.Value.Player1.NickName;
                            active.Player1.Score = game.Value.Player1.Score;
                            active.Player2.NickName = game.Value.Player2.NickName;
                            active.Player2.Score = game.Value.Player2.Score;
                            active.Board = game.Value.Board;
                            active.TimeLimit = game.Value.TimeLimit;
                            return active;
                        }
                    }
                }
                // Add a completed game option
            }

            SetStatus(Forbidden);
            return null;

        }
    }
}
