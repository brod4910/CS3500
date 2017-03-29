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
        private readonly static Dictionary<Token, UserInfo> users = new Dictionary<Token, UserInfo>();
        private readonly static HashSet<PendingGame> PendingGames = new HashSet<PendingGame>();
        private readonly static Dictionary<GameId, Status> activeGames = new Dictionary<GameId, Status>();
        private static readonly object sync = new object();
        private static int gameid = 0;

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
                if(user.Nickname.StartsWith("@"))
                {
                    Thread.Sleep(10000);
                }
                if(user.Nickname == null || user.Nickname.Trim().Length == 0)
                {
                    SetStatus(Forbidden);
                    return null;
                }
                else
                {
                    Token token = new Token();
                    token.UserToken = Guid.NewGuid().ToString();
                    users.Add(token, user);
                    return token;
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
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        public GameId JoinGame(PostGame game)
        {
            lock (sync)
            {
                int timeLimit;
                GameId id = new GameId();
                id.GameID = "" + gameid;
                Status status = new Status();
                status.GameState = "pending";

                int.TryParse(game.TimeLimit, out timeLimit);

                if (timeLimit < 5 || timeLimit < 120 || tokenIsValid(game.UserToken))
                {
                    SetStatus(Forbidden);
                    return null;
                }

                //Check if a pending game contains the usertoken
                foreach (PendingGame pendingGame in PendingGames)
                {
                    if (pendingGame.Player1Token.Equals(game.UserToken) || pendingGame.Player2Token.Equals(game.UserToken))
                    {
                        SetStatus(Conflict);
                        return null;
                    }
                }

                //Creates a new game and adds it to the list of games
                PendingGame g = new PendingGame();
                g.GameState = "pending";

                PendingGames.Add(g);

                //adds a new player to a game
                foreach (PendingGame pendingGame in PendingGames)
                {
                    if(pendingGame.Player1Token == null)
                    {
                        pendingGame.Player1Token = game.UserToken;
                        pendingGame.GameId = "" + gameid;
                        SetStatus(Accepted);
                        break;
                    }
                    else if(pendingGame.Player2Token == null)
                    {
                        pendingGame.Player2Token = game.UserToken;
                        SetStatus(Created);
                        gameid++;
                        PendingGames.Remove(pendingGame);
                        break;
                    }
                }

                if(!activeGames.ContainsKey(id))
                {
                    //prepare status message of pending game.
                    status.TimeLimit = game.TimeLimit;

                    //Set player 2 stats
                    status.Player1.NickName = GetUserInfo(game);
                    status.Player1.Score = "0";

                    //add the game to list of active games
                    activeGames.Add(id, status);
                }
                else if (activeGames.ContainsKey(id))
                {
                    //get status
                    activeGames.TryGetValue(id, out status);

                    //calculate the time limit for the game
                    status.TimeLimit = CalculateTimeLimit(status, timeLimit);
                    status.TimeLeft = status.TimeLimit;

                    //set player 2 stats
                    status.Player2.NickName = GetUserInfo(game);
                    status.Player2.Score = "0";

                    //Set game state to active
                    status.GameState = "active";
                }

                return id;
            }
        }

        private bool stopGame(dynamic info)
        {

        }

        /// <summary>
        /// Calculates the average time from status and t2
        /// </summary>
        /// <param name="status"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        private string CalculateTimeLimit(Status status, int t2)
        {
            int t1;
            int.TryParse(status.TimeLimit, out t1);

            return ((t1 + t2) / 2) + "";
        }

        /// <summary>
        /// Gets the Nickname of the player posting the
        /// game
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        private string GetUserInfo(PostGame game)
        {
            UserInfo info;
            Token tok = new Token();
            tok.UserToken = game.UserToken;
            users.TryGetValue(tok, out info);

            return info.Nickname;
        }

        /// <summary>
        /// Checks to see if the token is a valid
        /// user token.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private bool tokenIsValid(string token)
        {
            Token tok = new Token();
            tok.UserToken = token;

            if(users.ContainsKey(tok))
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}
