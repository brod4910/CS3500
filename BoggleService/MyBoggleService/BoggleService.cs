using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using static System.Net.HttpStatusCode;
using System.Data.SqlClient;
using System.Net.Sockets;
using System.Configuration;

namespace BoggleGame
{
    public class BoggleService
    {
        private static HashSet<String> Dictionary = dictionary();
        //for testing purposes
        private static bool board = false;

        // The connection string to the DB
        private static string BoggleDB;

       // private static TcpListener server;

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
            // BoggleDB = ConfigurationManager.ConnectionStrings["BoggleDB"].ConnectionString;

           // BoggleDB = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\BoggleDB.mdf;Integrated Security=True;";
            BoggleDB = ConfigurationManager.ConnectionStrings["BoggleDB"].ConnectionString;
          //  server = new TcpListener(IPAddress.Any, 60000);
        }


        /// <summary>
        /// Returns the hashset of the dictionary for word validation
        /// </summary>
        /// <returns></returns>
        private static HashSet<String> dictionary()
        {
            HashSet<String> dict = new HashSet<string>();

            string line;
            using (StreamReader file = new System.IO.StreamReader("dictionary.txt"))
            {
                while ((line = file.ReadLine()) != null)
                {
                    dict.Add(line);
                }
            }

            return dict;
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
        public Token Register(UserInfo user, out HttpStatusCode status)
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
                status = Forbidden;
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

                        if (command.ExecuteNonQuery() == 0)
                        {
                            command.ExecuteNonQuery();
                        }

                        status = Created;
                        trans.Commit();
                        return new Token() { UserToken = UserID };
                    }
                }
            }
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
        public GameId JoinGame(PostingGame postingGame, out HttpStatusCode status)
        {
            int timeLimit;
            int.TryParse(postingGame.TimeLimit, out timeLimit);
            int GameId = -1;
            GameId ID;

            if (!tokenIsValid(postingGame.UserToken))
            {
                status = Forbidden;
                return null;
            }
            if (timeLimit < 5 || timeLimit > 120)
            {
                status = Forbidden;
                return null;
            }

            if (userInPendingGame(new Token() { UserToken = postingGame.UserToken }))
            {
                status = Conflict;
                return null;
            }

            //Set up connection
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();

                //set up transaction
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    using (SqlCommand command = new SqlCommand("Select Player2, GameID, TimeLimit from Games where Player2 IS NULL", conn, trans))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                GameId = (int)reader["GameID"];
                                timeLimit = (int)reader["TimeLimit"];
                            }
                        }
                    }

                    string query;

                    if (GameId == -1)
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
                            status = Accepted;
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@Player2", postingGame.UserToken);
                            command.Parameters.AddWithValue("@TimeLimit", CalcTimeLimit(timeLimit, postingGame.TimeLimit));
                            //for testing purposes
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
                            status = Created;

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
        /// Cancels the users join request.
        /// If UserToken is invalid or is not a player in the pending game, responds with status 403 (Forbidden).
        /// Otherwise, removes UserToken from the pending game and responds with status 200 (OK).
        /// </summary>
        /// <param name="token"></param>
        public void CancelJoin(Token token, out HttpStatusCode status)
        {
            if (!tokenIsValid(token.UserToken) || !userInPendingGame(token))
            {
                status = Forbidden;
                return;
            }

            //Set up connection
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();

                //set up transaction
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    using (SqlCommand command = new SqlCommand("Delete from Games where Player1 = @UserID and Player2 IS NULL", conn, trans))
                    {
                        command.Parameters.AddWithValue("@UserID", token.UserToken);

                        status = OK;
                        trans.Commit();
                        return;
                    }
                }
            }

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
        public WordScore PlayWord(string GameID, PlayedWord word, out HttpStatusCode status)
        {
            if (!wordIsValid(word) || !tokenIsValid(word.UserToken) || !gameidIsValid(GameID) || !userIsinGame(GameID, word))
            {
                status = Forbidden;
                return null;
            }

            if (userInPendingGame(new Token() { UserToken = word.UserToken }) || CalcTimeLeft(GameID) <= 0)
            {
                status = Conflict;
                return null;
            }

            int gameid;
            int.TryParse(GameID, out gameid);
            string normWord = word.Word.ToUpper().Trim();

            return playtheword(gameid, word, normWord, out status);
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
        public Status Gamestatus(string GameID, string Option, out HttpStatusCode status)
        {
            int enteredID;
            // Check If It parses correctly
            if (!int.TryParse(GameID, out enteredID))
            {
                status = Forbidden;
                return null;
            }
            if (!gameidIsValid(GameID))
            {
                status = Forbidden;
                return null;
            }
            // Get the game
            // SQL check gamestate
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();

                //set up transaction
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    //Check to see if the player posting the game is in a pending game
                    using (SqlCommand command = new SqlCommand("Select * from Games where GameID = @GameID", conn, trans))
                    {
                        command.Parameters.AddWithValue("@GameID", enteredID);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Game is Pending
                                if (userInPendingGame(new Token { UserToken = (string)reader["Player1"] }))
                                {
                                    status = OK;
                                    Status pending = new Status();
                                    pending.GameState = "pending";
                                    return pending;
                                }
                                // Game is Completed
                                else if (CalcTimeLeft(GameID) <= 0)
                                {
                                    status = OK;
                                    Status active = new Status();
                                    active.GameState = "completed";
                                    active.Player1 = new FirstPlayer();
                                    active.Player2 = new SecondPlayer();
                                    active.Board = (string)reader["Board"];
                                    active.Player1.NickName = GetNickname((string)reader["Player1"]);
                                    active.Player2.NickName = GetNickname((string)reader["Player2"]);
                                    active.Player1.Score = SumScore((string)reader["Player1"]);
                                    active.Player2.Score = SumScore((string)reader["Player2"]);
                                    if (Option == null || Option == "no")
                                    {
                                        List<AlreadyPlayedWord> player1 = GetWordsPlayed((string)reader["Player1"]);
                                        List<AlreadyPlayedWord> player2 = GetWordsPlayed((string)reader["Player2"]);
                                        active.Player1.WordsPlayed = player1;
                                        active.Player2.WordsPlayed = player2;
                                        active.TimeLeft = "0";
                                    }
                                    return active;
                                }
                                // Game is Active
                                else
                                {
                                    status = OK;
                                    Status active = new Status();
                                    active.GameState = "active";
                                    active.Player1 = new FirstPlayer();
                                    active.Player2 = new SecondPlayer();
                                    active.Player1.NickName = GetNickname((string)reader["Player1"]);
                                    active.Player2.NickName = GetNickname((string)reader["Player2"]);
                                    active.Player1.Score = SumScore((string)reader["Player1"]);
                                    active.Player2.Score = SumScore((string)reader["Player2"]);
                                    // If brief was not an option
                                    if (Option == null || Option == "no")
                                    {
                                        active.Board = (string)reader["Board"];
                                        active.TimeLimit = (int)reader["TimeLimit"] + "";
                                    }
                                    return active;
                                }
                            }
                        }
                        status = Forbidden;
                        return null;
                    }
                }
            }
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////        Start of helper methods        //////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Plays the word
        /// </summary>
        /// <param name="gameid"></param>
        /// <param name="word"></param>
        /// <param name="normWord"></param>
        /// <returns></returns>

        private WordScore playtheword(int gameid, PlayedWord word, string normWord, out HttpStatusCode status)
        {
            WordScore score = new WordScore();
            string tokenp2 = "";

            //Set up connection
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();

                //set up transaction
                using (SqlTransaction trans = conn.BeginTransaction())
                {

                    using (SqlCommand command = new SqlCommand("Select Player2 from Games where GameID=@GameID", conn, trans))
                    {
                        command.Parameters.AddWithValue("@GameID", gameid);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tokenp2 = (string)reader["Player2"];
                            }
                        }
                    }

                    using (SqlCommand command = new SqlCommand("Select Word from Words where Player=@Player1 or Player=@Player2 order by Word", conn, trans))
                    {
                        command.Parameters.AddWithValue("@Player1", word.UserToken);
                        command.Parameters.AddWithValue("@Player2", tokenp2);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (((string)reader["Word"]).Equals(normWord))
                                {
                                    status = OK;
                                    return new WordScore { Score = 0 };
                                }
                            }
                        }
                    }

                    using (SqlCommand command = new SqlCommand("insert into Words (Player, GameID, Word, Score) values (@UserID, @GameID, @Word, @Score)", conn, trans))
                    {
                        command.Parameters.AddWithValue("@UserID", word.UserToken);
                        command.Parameters.AddWithValue("@GameID", gameid);
                        command.Parameters.AddWithValue("@Word", normWord);

                        if (canBeFormed(gameid + "", normWord))
                        {
                            if (Dictionary.Contains(word.Word) && normWord.Length > 2)
                            {
                                if (normWord.Length == 3 || normWord.Length == 4)
                                {
                                    command.Parameters.AddWithValue("@Score", 1);
                                    score = new WordScore { Score = 1 };
                                }
                                else if (normWord.Length == 5)
                                {
                                    command.Parameters.AddWithValue("@Score", 2);
                                    score = new WordScore { Score = 1 };
                                }
                                else if (normWord.Length == 6)
                                {
                                    command.Parameters.AddWithValue("@Score", 3);
                                    score = new WordScore { Score = 3 };
                                }
                                else if (normWord.Length == 7)
                                {
                                    command.Parameters.AddWithValue("@Score", 5);
                                    score = new WordScore { Score = 5 };
                                }
                                else
                                {
                                    command.Parameters.AddWithValue("@Score", 11);
                                    score = new WordScore { Score = 11 };
                                }
                            }
                            else
                            {
                                command.Parameters.AddWithValue("@Score", 0);
                                score = new WordScore { Score = 0 };
                            }
                        }
                        else if (normWord.Length <= 2)
                        {
                            command.Parameters.AddWithValue("@Score", 0);
                            score = new WordScore { Score = 0 };
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@Score", -1);
                            score = new WordScore { Score = -1 };
                        }


                        if (command.ExecuteNonQuery() == 0)
                        {
                            command.ExecuteNonQuery();
                        }

                        trans.Commit();
                        status = OK;
                        return score;
                    }

                }
            }
        }


        /// <summary>
        /// Checks to see if a user is a pending game
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>

        private bool userInPendingGame(Token token)
        {
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
        /// Checks to se if the boggle board can form the word
        /// </summary>
        /// <param name="GameID"></param>
        /// <param name="word"></param>
        /// <returns></returns>
        private bool canBeFormed(string GameID, string word)
        {
            string board = null;
            int gameid;
            int.TryParse(GameID, out gameid);

            //Set up connection
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();

                //set up transaction
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    using (SqlCommand command = new SqlCommand("Select Board from Games where GameID=@GameID", conn, trans))
                    {
                        command.Parameters.AddWithValue("@GameID", gameid);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                board = (String)reader["Board"];
                            }
                        }
                        trans.Commit();
                    }
                }
            }

            if (new BoggleBoard(board).CanBeFormed(word))
            {
                return true;
            }
            else
            {
                return false;
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
        /// Checks to see if the token is a valid
        /// user token.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private bool tokenIsValid(string token)
        {
            if (token == null || token.Length != 36)
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
                    using (SqlCommand command = new SqlCommand("Select UserID from Users where UserID = @UserID", conn, trans))
                    {
                        command.Parameters.AddWithValue("@UserID", token);

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
        /// Checks to see if GameID is valid in our database
        /// </summary>
        /// <param name="GameID"></param>
        /// <returns></returns>
        private bool gameidIsValid(string GameID)
        {
            if (GameID == null)
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
                        command.Parameters.AddWithValue("@GameID", gameid);
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
        /// Checks to see if the word is valid
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
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
                    using (SqlCommand command = new SqlCommand("Select GameID, Player1, Player2 from Games where GameID=@GameID and (Player1=@UserID or Player2=@UserID)", conn, trans))
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
            DateTime gameStart = DateTime.Now;
            int timeLimit = 0;
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
                    using (SqlCommand command = new SqlCommand("SELECT TimeLimit, StartTime  from Games where GameID=@GameID", conn, trans))
                    {
                        command.Parameters.AddWithValue("@GameID", gameid);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                timeLimit = (int)reader["TimeLimit"];
                                gameStart = reader.GetDateTime(1);
                                //DateTime.TryParse((string)reader["TimeStart"], out gameStart);
                            }
                        }
                        trans.Commit();
                    }
                }
            }

            DateTime now = DateTime.Now;

            int timeElapsed = (int)now.Subtract(gameStart).TotalSeconds;

            return timeLimit - timeElapsed;

        }
        /// <summary>
        /// Makes SQL call to sum the score of all words played by "userID"
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        private int SumScore(string userID)
        {
            int sum = 0;
            //Set up connection
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();
                //set up transaction
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    //Check to see if the player is in the game
                    using (SqlCommand command = new SqlCommand("Select convert(varchar(30),SUM(Score)) as TotalScore from Words where Player=@UserID", conn, trans))
                    {
                        command.Parameters.AddWithValue("@UserID", userID);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (reader.IsDBNull(0))
                                {
                                    return 0;
                                }
                                else
                                {
                                    int.TryParse((string)reader["TotalScore"], out sum);
                                }
                            }
                        }
                    }
                    trans.Commit();
                }
            }
            return sum;
        }
        /// <summary>
        /// SQL to retrieve nickname of "userID" from Users table 
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        private string GetNickname(string userID)
        {
            string name = null;
            //Set up connection
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();
                //set up transaction
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    //Check to see if the player is in the game
                    using (SqlCommand command = new SqlCommand("Select NickName from Users where UserID=@UserID", conn, trans))
                    {
                        command.Parameters.AddWithValue("@UserID", userID);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                name = reader.GetString(0);
                            }
                        }
                    }
                    trans.Commit();
                }
            }
            return name;
        }

        /// <summary>
        /// SQL Call to retrieve all the words played by "userID"
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        private List<AlreadyPlayedWord> GetWordsPlayed(string userID)
        {
            List<AlreadyPlayedWord> words = new List<AlreadyPlayedWord>();
            //Set up connection
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();
                //set up transaction
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    //Check to see if the player is in the game
                    using (SqlCommand command = new SqlCommand("Select Word, score from Words where Player=@UserID", conn, trans))
                    {
                        command.Parameters.AddWithValue("@UserID", userID);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                AlreadyPlayedWord played = new AlreadyPlayedWord();
                                played.Score = (int)reader["Score"];
                                played.Word = (string)reader["Word"];
                                words.Add(played);
                            }
                        }
                        trans.Commit();
                    }
                }
            }
            if (words.Count == 0)
            {
                return null;
            }
            return words;
        }
    }
}