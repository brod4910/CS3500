using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
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
    public class PostingGame
    {
        public string UserToken { get; set; }

        public string TimeLimit { get; set; }
    }

    /// <summary>
    /// Contains the GameId of a game.
    /// </summary>
    [DataContract]
    public class GameId
    {
        [DataMember]
        public string GameID { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string TimeLimit { get; set; }
    }

    /// <summary>
    /// Contains a pending game
    /// </summary>
    [DataContract]
    public class PendingGame
    {
        [DataMember]
        public string GameState { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Player1Token { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Player2Token { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string GameId { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string TimeLimit { get; set; }
    }


    /// <summary>
    /// Contains the score of the word played.
    /// </summary>
    public class WordScore
    {
        public string Score { get; set; }
    }

    /// <summary>
    /// Contains the word to be played.
    /// </summary>
    public class PlayedWord
    {
        public string UserToken { get; set; }

        public string Word { get; set; }
    }

    /// <summary>
    /// Represents a word that has already been played.
    /// </summary>
    public class AlreadyPlayedWord
    {
        public string Word { get; set; }

        public string Score { get; set; }
    }

    /// <summary>
    /// Contains the statistics of any given game.
    /// </summary>
    [DataContract]
    public class Status
    {
        [DataMember]
        public string GameState { get; set; }
        
        [DataMember(EmitDefaultValue =false)]
        public string Board { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string TimeLimit { get; set; }

        [DataMember]
        public string TimeLeft { get; set; }

        [DataMember]
        public FirstPlayer Player1 = new FirstPlayer();

        [DataMember]
        public SecondPlayer Player2 = new SecondPlayer();

        [IgnoreDataMember]
        public DateTime datetime { get; set; }
    }

    [DataContract]
    public class FirstPlayer
    {
        [DataMember(EmitDefaultValue = false)]
        public string NickName { get; set; }

        [DataMember]
        public string Score { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public List<AlreadyPlayedWord> WordsPlayed;
    }

    [DataContract]
    public class SecondPlayer
    {
        [DataMember(EmitDefaultValue = false)]
        public string NickName { get; set; }

        [DataMember]
        public string Score { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public List<AlreadyPlayedWord> WordsPlayed;
    }
}