using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boggle
{
    /// <summary>
    /// Contains the username of the player
    /// </summary>
    public class UserInfo
    {
        public string Nickname {get; set;}
    }

    /// <summary>
    /// Contains the usertoken
    /// </summary>
    public class Token
    {
        public string UserToken { get; set; }
    }

    /// <summary>
    /// Contains the usertoken of the user and
    /// the desired timelimit
    /// </summary>
    public class Game
    {
        public string UserToken { get; set; }

        public string TimeLimit { get; set; }
    }

    /// <summary>
    /// Contains the GameId of a game.
    /// </summary>
    public class GameId
    {
        public string GameID { get; set; }
    }

    /// <summary>
    /// Contains the score of the word played.
    /// </summary>
    public class score
    {
        public string Score { get; set; }
    }

    /// <summary>
    /// Contains the word to be played.
    /// </summary>
    public class word
    {
        public string UserToken { get; set; }

        public string Word { get; set; }
    }

    /// <summary>
    /// Contains the statistics of any given game.
    /// </summary>
    public class GameStatus
    {
        public string GameState { get; set; }

        public string Board { get; set; }

        public string TimeLimit { get; set; }

        public string TimeLeft { get; set; }

        public class Player1
        {
            public string NickName { get; set; }

            public string Score { get; set; }
        }

        public class Player2
        {
            public string NickName { get; set; }

            public string Score { get; set; }
        }

    }
}