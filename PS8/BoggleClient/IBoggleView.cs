using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoggleClient
{
    public interface IBoggleView
    {
        /// <summary>
        /// Fired when user must be registered.
        /// Parameters are name and domain.
        /// </summary>
        event Action<string, string> RegisterPressed;

        /// <summary>
        /// Fired when game time button is pressed
        /// Parameters are name.
        /// </summary>
        event Action<string> CreateGamePressed;

        /// <summary>
        /// Fired when cancelled button is pressed
        /// Parameters none
        /// </summary>
        event Action CancelPressed;

        /// <summary>
        /// Fired when a word is entered for boggle
        /// Parameter is the word entered
        /// </summary>
        event Action<string> WordEntered;

        /// <summary>
        /// Gets game status of game
        /// </summary>
        event Func<bool, bool> GameStatus;

        /// <summary>
        /// Getter and setter for registering a user
        /// </summary>
        bool UserRegistered { get; set;}

        /// <summary>
        /// Getter and setter for game state
        /// </summary>
        bool GameState { get; set; }

        /// <summary>
        /// Getter and setter for time
        /// </summary>
        double Time { get; set; }

        /// <summary>
        /// If state == true, enables all controls that are normally enabled; disables Cancel.
        /// If state == false, disables all controls; enables Cancel.
        /// </summary>
        void EnableControls(bool state);

        /// <summary>
        /// Displays the board on the GUI
        /// </summary>
        /// <param name="board"></param>
        void DisplayBoard(string board);
    }
}
