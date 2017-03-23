using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BoggleClient
{
    public partial class Boggle : Form, IBoggleView
    {
        /// <summary>
        /// Fired when user must be registered.
        /// Parameters are name and domain.
        /// </summary>
        public event Action<string, string> RegisterPressed;

        /// <summary>
        /// Fired when game time button is pressed
        /// Parameters are name.
        /// </summary>
        public event Action<string> CreateGamePressed;

        /// <summary>
        /// Fired when cancelled button is pressed
        /// Parameters none
        /// </summary>
        public event Action CancelPressed;

        /// <summary>
        /// Fired when a word is entered for boggle
        /// Parameter is the word entered
        /// </summary>
        public event Action<string> WordEntered;

        /// <summary>
        /// Gets game status of game
        /// </summary>
        public event Func<bool, bool> GameStatus;

        /// <summary>
        /// registered user is set to false
        /// </summary>
        private bool _userRegistered = false;

        /// <summary>
        /// State of game at any given point
        /// False == completed or pending
        /// true == active
        /// </summary>
        private bool _gameState = false;

        /// <summary>
        /// Time left in any given game
        /// </summary>
        private double _time = 0;

        public Boggle()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Getter and setter for registering a user
        /// </summary>
        public bool UserRegistered
        {
            get { return _userRegistered; }

            set
            {
                _userRegistered = value;
            }
        }

        /// <summary>
        /// Getter and setter for game state
        /// </summary>
        public bool GameState
        {
            get { return _gameState; }
            set { _gameState = value; }
        }

        /// <summary>
        /// Getter and setter for time
        /// </summary>
        public double Time
        {
            get { return _time; }
            set { _time = value; }
        }

        /// <summary>
        /// If state == true, enables all controls that are normally enabled; disables Cancel.
        /// If state == false, disables all controls; enables Cancel.
        /// </summary>
        public void EnableControls(bool state)
        {
            RegisterButton.Enabled = state;

            CreateGameButton.Enabled = state && UserRegistered && GameTimeTextBox.Text.Trim().Length > 0 && AreNumbers(GameTimeTextBox.Text);

            foreach (Control control in OptionsSplitContainer.Panel2.Controls)
            {
                if(control is Button)
                {
                    control.Enabled = state && UserRegistered && GameState;
                }
                else if(control is TextBox)
                {
                    control.Enabled = state && UserRegistered && GameState;
                }
            }

            ButtonCancel.Enabled = !state;
        }

        /// <summary>
        /// Checks to see if the input string is a number
        /// </summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
        private bool AreNumbers(string numbers)
        {
            double result;

            return Double.TryParse(numbers, out result);
        }

        /// <summary>
        /// Enables the Register button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Registration_TextChanged(object sender, EventArgs e)
        {
            RegisterButton.Enabled = DomainNameTextBox.Text.Trim().Length > 0 && RegisterUserTextBox.Text.Trim().Length > 0;
        }

        /// <summary>
        /// When words are entered for boggle sregister typed word
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnterWordsTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Enter)
            {
                WordEntered(EnterWordsTextBox.Text);
                e.Handled = true;
                EnterWordsTextBox.Text = "";
            }
        }

        /// <summary>
        /// When the registration button is clicked register user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RegisterButton_Click(object sender, EventArgs e)
        {
            if(RegisterPressed != null)
            {
                RegisterPressed(RegisterUserTextBox.Text.Trim(), DomainNameTextBox.Text.Trim());
            }
        }

        /// <summary>
        /// Cancels previous request that was made.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, EventArgs e)
        {
            if(CancelPressed != null)
            {
                CancelPressed();
            }
        }

        /// <summary>
        /// Creates current game with the set amount of time
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateGameButton_Click(object sender, EventArgs e)
        {
            if(CreateGamePressed != null)
            {
                CreateGamePressed(GameTimeTextBox.Text);
                GameTimeTextBox.Text = "";
            }
        }

        /// <summary>
        /// Enables the create game button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameTimeTextBox_TextChanged(object sender, EventArgs e)
        {
            CreateGameButton.Enabled = GameTimeTextBox.Text.Trim().Length > 0 && AreNumbers(GameTimeTextBox.Text) && UserRegistered;
        }

        /// <summary>
        /// Displays the board on the intial game status
        /// Not Tested
        /// </summary>
        /// <param name="board"></param>
        public void DisplayBoard(string board)
        {
            char[] charArray = board.ToCharArray();

            Letter00.Text = charArray[0].ToString();
            Letter01.Text = charArray[1].ToString();
            Letter02.Text = charArray[2].ToString();
            Letter03.Text = charArray[3].ToString();
            Letter10.Text = charArray[4].ToString();
            Letter11.Text = charArray[5].ToString();
            Letter12.Text = charArray[6].ToString();
            Letter13.Text = charArray[7].ToString();
            Letter20.Text = charArray[8].ToString();
            Letter21.Text = charArray[9].ToString();
            Letter22.Text = charArray[10].ToString();
            Letter23.Text = charArray[11].ToString();
            Letter30.Text = charArray[12].ToString();
            Letter31.Text = charArray[13].ToString();
            Letter32.Text = charArray[14].ToString();
            Letter33.Text = charArray[15].ToString();
        }

        /// <summary>
        /// Displays the words played
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        public void WordsPlayed(dynamic player1, dynamic player2)
        {
            foreach(dynamic word in player1)
            {
                Player1WordsPlayedLabel.Text = word + "\n";
            }

            foreach (dynamic word in player2)
            {
                Player2WordsPlayedLabel.Text = word + "\n";
            }
        }

        /// <summary>
        /// Clears the board
        /// </summary>
        public void ClearBoard()
        {
            string board = "                ";

            DisplayBoard(board);

            Player1ScoreLabel.Text = "";
            Player2ScoreLabel.Text = "";
        }

        /// <summary>
        /// Sets the Clock Time
        /// </summary>
        public void SetTime()
        {
            TimeLabel.Text = "Time Left: " +  Time.ToString();
        }

        public void SetScore(string player1Score, string player2Score)
        {
            Player1ScoreLabel.Text = "Score: " + player1Score;
            Player2ScoreLabel.Text = "Score: " + player2Score;
        }

        /// <summary>
        /// Disables Name and Server Text Boxes
        /// </summary>
        public void DisableNameAndServer(bool state)
        {
            DomainNameTextBox.Enabled = state;
            RegisterUserTextBox.Enabled = state;
        }

        /// <summary>
        /// Disables game time and create button
        /// </summary>
        /// <param name="state"></param>
        public void DisableGameTime(bool state)
        {
            GameTimeTextBox.Enabled = state;
            CreateGameButton.Enabled = state;
        }

        /// <summary>
        /// Enables / Disables the Game Started Button
        /// </summary>
        /// <param name="status"></param>
        public void SetSubmitButton(bool status)
        {
            if(status)
            {
                GameStartedButton.Text = "Play word";
            }
            GameStartedButton.Enabled = status;
        }

        /// <summary>
        /// Sets Player Labels to provided nicknames
        /// </summary>
        public void SetPlayerNicknames(string nick1, string nick2)
        {
            Player1Label.Text = nick1;
            Player2Label.Text = nick2;
        }

        private void GameStartedButton_Click(object sender, EventArgs e)
        {
            WordEntered(EnterWordsTextBox.Text.ToString());
            EnterWordsTextBox.Text = "";
        }
    }
}
