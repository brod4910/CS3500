using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.ServiceModel.Web;
using System.Threading;
using static System.Net.HttpStatusCode;

namespace Boggle
{
    public class BoggleService : IBoggleService
    {
        private readonly static Dictionary<string, UserInfo> users = new Dictionary<string, UserInfo>();
        private readonly static HashSet<PendingGame> PendingGames = new HashSet<PendingGame>();
        private readonly static Dictionary<string, Status> activeGames = new Dictionary<string, Status>();
        private static readonly object sync = new object();
        private static int gameid = 0;
        private static bool board = false;

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
            lock (sync)
            {
                if(user.Nickname.Equals("boardtest"))
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
                else
                {
                    Token token = new Token();
                    token.UserToken = Guid.NewGuid().ToString();
                    users.Add(token.UserToken, user);
                    return token;
                }
            }
        }
        /// <summary>
        /// Demo.  You can delete this.
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
        /// Joins a game/Creates a new game
        /// If UserToken is invalid, TimeLimit  5, or TimeLimit  120, responds with status 403 (Forbidden).
        /// Otherwise, if UserToken is already a player in the pending game, responds with status 409 (Conflict).
        /// Otherwise, if there is already one player in the pending game, adds UserToken as the second player. 
        /// The pending game becomes active and a new pending game with no players is created. 
        /// The active game's time limit is the integer average of the time limits requested by the two players. 
        /// Returns the new active game's GameID (which should be the same as the old pending game's GameID). 
        /// Responds with status 201 (Created).
        /// </summary>
        /// <param name="postingGame"></param>
        /// <returns></returns>
        public GameId JoinGame(PostingGame postingGame)
        {
            lock (sync)
            {
                int timeLimit;
                GameId id = new GameId();
                id.GameID = "" + gameid;

                int.TryParse(postingGame.TimeLimit, out timeLimit);

                if (timeLimit < 5 || timeLimit > 120 || !tokenIsValid(postingGame.UserToken))
                {
                    SetStatus(Forbidden);
                    return null;
                }

                //Check if a pending game contains the usertoken
                foreach (PendingGame pendingGame in PendingGames)
                {
                    if (pendingGame.Player1Token != null)
                    {
                        if (pendingGame.Player1Token.Equals(postingGame.UserToken))
                        {
                            SetStatus(Conflict);
                            return null;
                        }
                    }
                }

                //Creates a new game and adds it to the list of games
                if (PendingGames.Count == 0)
                {
                    PendingGame g = new PendingGame();
                    g.GameState = "pending";
                    g.GameId = "" + gameid++;

                    PendingGames.Add(g);
                }

                //adds a new player to a game
                foreach (PendingGame pendingGame in PendingGames)
                {
                    //if the player 1 token is is null add
                    //the users information to the pending game
                    if(pendingGame.Player1Token == null)
                    {
                        pendingGame.Player1Token = postingGame.UserToken;
                        pendingGame.TimeLimit = postingGame.TimeLimit;

                        SetStatus(Accepted);
                        break;
                    }
                    //if player 2 token is null prepare the game to be started
                    //and add it to the list of active games
                    else if(pendingGame.Player2Token == null)
                    {
                        pendingGame.Player2Token = postingGame.UserToken;
                        SetStatus(Created);
                       // SetStatus(OK);
                    }

                    //Make the pending game an active game
                    if(pendingGame.Player1Token != null && pendingGame.Player2Token != null)
                    {
                        Status status = createGame(pendingGame, postingGame, id);

                        activeGames.Add(id.GameID, status);

                        PendingGames.Remove(pendingGame);
                        break;
                    }
                }

                return id;
            }
        }

        /// <summary>
        /// Creates the initial game status for the game
        /// </summary>
        /// <param name="pendingGame"></param>
        /// <param name="postGame"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private Status createGame(PendingGame pendingGame, PostingGame postGame, GameId id)
        {
            Status status = new Status();

            if (board)
            {
                status.Board = new BoggleBoard("NAMEPAINGAINRAIN").ToString();
            }
            else
            {
                status.Board = new BoggleBoard().ToString();
            }

            status.TimeLimit = CalculateTimeLimit(pendingGame, postGame);
            status.GameState = "active";
            status.Player1.NickName = GetUserInfo(pendingGame, null);
            status.Player2.NickName = GetUserInfo(null, postGame);
            status.Player1.Score = "0";
            status.Player2.Score = "0";
            status.TimeLeft = status.TimeLimit;
            status.datetime = DateTime.Now;
            return status;
        }

        /// <summary>
        /// Calculates the average time from status and t2
        /// </summary>
        /// <param name="status"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        private string CalculateTimeLimit(PendingGame pengame, PostingGame posgame)
        {
            int t1;
            int t2;

            int.TryParse(pengame.TimeLimit, out t1);
            int.TryParse(posgame.TimeLimit, out t2);

            return ((t1 + t2) / 2) + "";
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
        /// Gets the Nickname of the player posting the
        /// game
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        private string GetUserInfo(PendingGame pengame, PostingGame posgame)
        {
            if (posgame != null)
            {
                UserInfo info;
                Token tok = new Token();
                tok.UserToken = posgame.UserToken;
                users.TryGetValue(tok.UserToken, out info);

                return info.Nickname;
            }
            else
            {
                UserInfo info;
                Token tok = new Token();
                tok.UserToken = pengame.Player1Token;
                users.TryGetValue(tok.UserToken, out info);

                return info.Nickname;
            }
        }

        /// <summary>
        /// Checks to see if the token is a valid
        /// user token.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private bool tokenIsValid(string token)
        {
            if(users.ContainsKey(token))
            {
                return true;
            }
            else
            {
                return false;
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
            foreach(PendingGame game in PendingGames)
            {
                if(token != null && token.UserToken.Equals(game.Player1Token))
                {
                    game.Player1Token = null;
                    SetStatus(OK);
                    return;
                }
                else if(token != null && token.UserToken.Equals(game.Player2Token))
                {
                    game.Player2Token = null;
                    SetStatus(OK);
                    return;
                }
            }
            SetStatus(Forbidden);
            return;
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
            UserInfo userInfo;
            WordScore wordScore = new WordScore();

            if(word.Word == null || word.Word.Trim().Length == 0 || word.UserToken == null || !tokenIsValid(word.UserToken)
                || !activeGames.ContainsKey(GameID) || GameID == null)
            {
                SetStatus(Forbidden);
                return null;
            }


            activeGames.TryGetValue(GameID, out status);
           users.TryGetValue(word.UserToken, out userInfo);


            if(status.Player1.NickName.Equals(userInfo.Nickname) && status.Player2.NickName.Equals(userInfo.Nickname))
            {
                SetStatus(Forbidden);
                return null;
            }

            if(!status.GameState.Equals("active"))
            {
                SetStatus(Conflict);
                return null;
            }
            else
            {
                if(new BoggleBoard(status.Board).CanBeFormed(word.Word.Trim()))
                {
                    if(status.Player1.NickName.Equals(userInfo.Nickname))
                    {
                        foreach(var Word in status.Player1Words)
                        {
                            if(Word.Word.Equals(word.Word.Trim()))
                            {
                                wordScore.Score = 0;
                                status.Player1Words.Add(new AlreadyPlayedWord() { Score = wordScore.Score, Word = word.Word.Trim() });
                                SetStatus(OK);
                                return wordScore;
                            }
                        }
                        if (wordIsValid(word.Word))
                        {
                            wordScore.Score = 1;
                            status.Player1Words.Add(new AlreadyPlayedWord() { Score = wordScore.Score, Word = word.Word.Trim() });
                            status.Player1.Score += 1;
                            SetStatus(OK);
                            return wordScore;
                        }
                        else
                        {
                            wordScore.Score = -1;
                            status.Player1Words.Add(new AlreadyPlayedWord() { Score = wordScore.Score, Word = word.Word.Trim() });
                            status.Player1.Score += -1;
                            SetStatus(OK);
                            return wordScore;
                        }
                    }
                    else
                    {
                        foreach (var Word in status.Player2Words)
                        {
                            if (Word.Word.Equals(word.Word))
                            {
                                wordScore.Score = 0;
                                status.Player2Words.Add(new AlreadyPlayedWord() { Score = wordScore.Score, Word = word.Word.Trim() });
                                SetStatus(OK);
                                return wordScore;
                            }
                        }
                        if (wordIsValid(word.Word))
                        {
                            wordScore.Score = 1;
                            status.Player2Words.Add(new AlreadyPlayedWord() { Score = wordScore.Score, Word = word.Word.Trim() });
                            status.Player2.Score += 1;
                            SetStatus(OK);
                            return wordScore;
                        }
                        else
                        {
                            wordScore.Score = -1;
                            status.Player2Words.Add(new AlreadyPlayedWord() { Score = wordScore.Score, Word = word.Word.Trim() });
                            status.Player2.Score += -1;
                            SetStatus(OK);
                            return wordScore;
                        }
                    }
                }
                else
                {
                    if (status.Player1.NickName.Equals(userInfo.Nickname))
                    {
                        foreach (var Word in status.Player1Words)
                        {
                            if (Word.Word.Equals(word.Word.Trim()))
                            {
                                wordScore.Score = 0;
                                status.Player1Words.Add(new AlreadyPlayedWord() { Score = wordScore.Score, Word = word.Word.Trim() });
                                SetStatus(OK);
                                return wordScore;
                            }
                        }

                        status.Player1Words.Add(new AlreadyPlayedWord() { Score = wordScore.Score, Word = word.Word.Trim() });
                        status.Player1.Score += -1;
                    }
                    else
                    {
                        foreach (var Word in status.Player2Words)
                        {
                            if (Word.Word.Equals(word.Word))
                            {
                                wordScore.Score = 0;
                                status.Player2Words.Add(new AlreadyPlayedWord() { Score = wordScore.Score, Word = word.Word.Trim() });
                                SetStatus(OK);
                                return wordScore;
                            }
                        }
                        status.Player2Words.Add(new AlreadyPlayedWord() { Score = wordScore.Score, Word = word.Word.Trim() });
                        status.Player2.Score += -1;
                    }
                    wordScore.Score = -1;
                    SetStatus(OK);
                    return wordScore;
                }
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
