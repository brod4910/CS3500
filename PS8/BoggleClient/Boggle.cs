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
        /// registered user is set to false
        /// </summary>
        private bool _userRegistered = false;

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
                    control.Enabled = state && UserRegistered;
                }
                else if(control is TextBox)
                {
                    control.Enabled = state && UserRegistered;
                }
            }

            CancelButton.Enabled = !state;
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
    }
}
