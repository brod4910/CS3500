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
        public Boggle()
        {
            InitializeComponent();
        }

        private bool _userRegistered = false;

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
            GameTimeButton.Enabled = state && UserRegistered && GameTimeTextBox.Text.Length > 0;

            foreach (Control control in OptionsSplitContainer.Panel2.Controls)
            {
                if(control is Button)
                {
                    control.Enabled = state;
                }
                else if(control is TextBox)
                {
                    control.Enabled = state;
                }
            }

            CancelButton.Enabled = !state;
        }

        /// <summary>
        /// Fired when user must be registered.
        /// Parameters are name and domain.
        /// </summary>
        public event Action<string, string> RegisterPressed;
    }
}
