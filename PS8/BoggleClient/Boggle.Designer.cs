namespace BoggleClient
{
    partial class Boggle
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.OptionsSplitContainer = new System.Windows.Forms.SplitContainer();
            this.CancelButton = new System.Windows.Forms.Button();
            this.CreateGameButton = new System.Windows.Forms.Button();
            this.GameTimeTextBox = new System.Windows.Forms.TextBox();
            this.GameTimeLabel = new System.Windows.Forms.Label();
            this.DomainLabel = new System.Windows.Forms.Label();
            this.UsernameLabel = new System.Windows.Forms.Label();
            this.DomainNameTextBox = new System.Windows.Forms.TextBox();
            this.RegisterUserTextBox = new System.Windows.Forms.TextBox();
            this.RegisterButton = new System.Windows.Forms.Button();
            this.BogglePanel = new System.Windows.Forms.Panel();
            this.GameStartedButton = new System.Windows.Forms.Button();
            this.EnterWordsLabel = new System.Windows.Forms.Label();
            this.EnterWordsTextBox = new System.Windows.Forms.TextBox();
            this.Letter03 = new System.Windows.Forms.Label();
            this.Letter02 = new System.Windows.Forms.Label();
            this.Letter01 = new System.Windows.Forms.Label();
            this.Letter00 = new System.Windows.Forms.Label();
            this.Letter10 = new System.Windows.Forms.Label();
            this.Letter11 = new System.Windows.Forms.Label();
            this.Letter12 = new System.Windows.Forms.Label();
            this.Letter13 = new System.Windows.Forms.Label();
            this.Letter30 = new System.Windows.Forms.Label();
            this.Letter31 = new System.Windows.Forms.Label();
            this.Letter32 = new System.Windows.Forms.Label();
            this.Letter33 = new System.Windows.Forms.Label();
            this.Letter20 = new System.Windows.Forms.Label();
            this.Letter21 = new System.Windows.Forms.Label();
            this.Letter22 = new System.Windows.Forms.Label();
            this.Letter23 = new System.Windows.Forms.Label();
            this.Scorepanel = new System.Windows.Forms.Panel();
            this.Player2ScoreLabel = new System.Windows.Forms.Label();
            this.Player2Label = new System.Windows.Forms.Label();
            this.TimeLabel = new System.Windows.Forms.Label();
            this.Player1ScoreLabel = new System.Windows.Forms.Label();
            this.Player1Label = new System.Windows.Forms.Label();
            this.Player1WordsPlayedLabel = new System.Windows.Forms.Label();
            this.Player2WordsPlayedLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.OptionsSplitContainer)).BeginInit();
            this.OptionsSplitContainer.Panel1.SuspendLayout();
            this.OptionsSplitContainer.Panel2.SuspendLayout();
            this.OptionsSplitContainer.SuspendLayout();
            this.Scorepanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // OptionsSplitContainer
            // 
            this.OptionsSplitContainer.Location = new System.Drawing.Point(1, 0);
            this.OptionsSplitContainer.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.OptionsSplitContainer.Name = "OptionsSplitContainer";
            // 
            // OptionsSplitContainer.Panel1
            // 
            this.OptionsSplitContainer.Panel1.Controls.Add(this.CancelButton);
            this.OptionsSplitContainer.Panel1.Controls.Add(this.CreateGameButton);
            this.OptionsSplitContainer.Panel1.Controls.Add(this.GameTimeTextBox);
            this.OptionsSplitContainer.Panel1.Controls.Add(this.GameTimeLabel);
            this.OptionsSplitContainer.Panel1.Controls.Add(this.DomainLabel);
            this.OptionsSplitContainer.Panel1.Controls.Add(this.UsernameLabel);
            this.OptionsSplitContainer.Panel1.Controls.Add(this.DomainNameTextBox);
            this.OptionsSplitContainer.Panel1.Controls.Add(this.RegisterUserTextBox);
            this.OptionsSplitContainer.Panel1.Controls.Add(this.RegisterButton);
            this.OptionsSplitContainer.Panel1.Controls.Add(this.BogglePanel);
            // 
            // OptionsSplitContainer.Panel2
            // 
            this.OptionsSplitContainer.Panel2.Controls.Add(this.GameStartedButton);
            this.OptionsSplitContainer.Panel2.Controls.Add(this.EnterWordsLabel);
            this.OptionsSplitContainer.Panel2.Controls.Add(this.EnterWordsTextBox);
            this.OptionsSplitContainer.Size = new System.Drawing.Size(591, 101);
            this.OptionsSplitContainer.SplitterDistance = 301;
            this.OptionsSplitContainer.SplitterWidth = 3;
            this.OptionsSplitContainer.TabIndex = 0;
            // 
            // CancelButton
            // 
            this.CancelButton.Enabled = false;
            this.CancelButton.Location = new System.Drawing.Point(242, 54);
            this.CancelButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(54, 29);
            this.CancelButton.TabIndex = 8;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // CreateGameButton
            // 
            this.CreateGameButton.Enabled = false;
            this.CreateGameButton.Location = new System.Drawing.Point(164, 55);
            this.CreateGameButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.CreateGameButton.Name = "CreateGameButton";
            this.CreateGameButton.Size = new System.Drawing.Size(60, 42);
            this.CreateGameButton.TabIndex = 7;
            this.CreateGameButton.Text = "Create Game";
            this.CreateGameButton.UseVisualStyleBackColor = true;
            this.CreateGameButton.Click += new System.EventHandler(this.CreateGameButton_Click);
            // 
            // GameTimeTextBox
            // 
            this.GameTimeTextBox.Location = new System.Drawing.Point(93, 68);
            this.GameTimeTextBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.GameTimeTextBox.Name = "GameTimeTextBox";
            this.GameTimeTextBox.Size = new System.Drawing.Size(68, 20);
            this.GameTimeTextBox.TabIndex = 6;
            this.GameTimeTextBox.TextChanged += new System.EventHandler(this.GameTimeTextBox_TextChanged);
            // 
            // GameTimeLabel
            // 
            this.GameTimeLabel.AutoSize = true;
            this.GameTimeLabel.Location = new System.Drawing.Point(13, 70);
            this.GameTimeLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.GameTimeLabel.Name = "GameTimeLabel";
            this.GameTimeLabel.Size = new System.Drawing.Size(64, 13);
            this.GameTimeLabel.TabIndex = 5;
            this.GameTimeLabel.Text = "Game Time:";
            // 
            // DomainLabel
            // 
            this.DomainLabel.AutoSize = true;
            this.DomainLabel.Location = new System.Drawing.Point(13, 36);
            this.DomainLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.DomainLabel.Name = "DomainLabel";
            this.DomainLabel.Size = new System.Drawing.Size(77, 13);
            this.DomainLabel.TabIndex = 4;
            this.DomainLabel.Text = "Domain Name:";
            // 
            // UsernameLabel
            // 
            this.UsernameLabel.AutoSize = true;
            this.UsernameLabel.Location = new System.Drawing.Point(13, 10);
            this.UsernameLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.UsernameLabel.Name = "UsernameLabel";
            this.UsernameLabel.Size = new System.Drawing.Size(58, 13);
            this.UsernameLabel.TabIndex = 3;
            this.UsernameLabel.Text = "Username:";
            // 
            // DomainNameTextBox
            // 
            this.DomainNameTextBox.Location = new System.Drawing.Point(93, 34);
            this.DomainNameTextBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.DomainNameTextBox.Name = "DomainNameTextBox";
            this.DomainNameTextBox.Size = new System.Drawing.Size(146, 20);
            this.DomainNameTextBox.TabIndex = 2;
            this.DomainNameTextBox.TextChanged += new System.EventHandler(this.Registration_TextChanged);
            // 
            // RegisterUserTextBox
            // 
            this.RegisterUserTextBox.Location = new System.Drawing.Point(93, 8);
            this.RegisterUserTextBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.RegisterUserTextBox.Name = "RegisterUserTextBox";
            this.RegisterUserTextBox.Size = new System.Drawing.Size(146, 20);
            this.RegisterUserTextBox.TabIndex = 1;
            this.RegisterUserTextBox.TextChanged += new System.EventHandler(this.Registration_TextChanged);
            // 
            // RegisterButton
            // 
            this.RegisterButton.Enabled = false;
            this.RegisterButton.Location = new System.Drawing.Point(242, 20);
            this.RegisterButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.RegisterButton.Name = "RegisterButton";
            this.RegisterButton.Size = new System.Drawing.Size(54, 21);
            this.RegisterButton.TabIndex = 0;
            this.RegisterButton.Text = "Register";
            this.RegisterButton.UseVisualStyleBackColor = true;
            this.RegisterButton.Click += new System.EventHandler(this.RegisterButton_Click);
            // 
            // BogglePanel
            // 
            this.BogglePanel.Location = new System.Drawing.Point(7, 156);
            this.BogglePanel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.BogglePanel.Name = "BogglePanel";
            this.BogglePanel.Size = new System.Drawing.Size(576, 283);
            this.BogglePanel.TabIndex = 1;
            // 
            // GameStartedButton
            // 
            this.GameStartedButton.Enabled = false;
            this.GameStartedButton.Location = new System.Drawing.Point(79, 70);
            this.GameStartedButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.GameStartedButton.Name = "GameStartedButton";
            this.GameStartedButton.Size = new System.Drawing.Size(126, 25);
            this.GameStartedButton.TabIndex = 17;
            this.GameStartedButton.Text = "Game Not Started";
            this.GameStartedButton.UseVisualStyleBackColor = true;
            this.GameStartedButton.Click += new System.EventHandler(this.GameStartedButton_Click);
            // 
            // EnterWordsLabel
            // 
            this.EnterWordsLabel.AutoSize = true;
            this.EnterWordsLabel.Location = new System.Drawing.Point(97, 12);
            this.EnterWordsLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.EnterWordsLabel.Name = "EnterWordsLabel";
            this.EnterWordsLabel.Size = new System.Drawing.Size(90, 13);
            this.EnterWordsLabel.TabIndex = 16;
            this.EnterWordsLabel.Text = "Enter words here:";
            // 
            // EnterWordsTextBox
            // 
            this.EnterWordsTextBox.Enabled = false;
            this.EnterWordsTextBox.Location = new System.Drawing.Point(26, 36);
            this.EnterWordsTextBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.EnterWordsTextBox.Name = "EnterWordsTextBox";
            this.EnterWordsTextBox.Size = new System.Drawing.Size(233, 20);
            this.EnterWordsTextBox.TabIndex = 16;
            this.EnterWordsTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.EnterWordsTextBox_KeyPress);
            // 
            // Letter03
            // 
            this.Letter03.AutoSize = true;
            this.Letter03.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Letter03.Location = new System.Drawing.Point(471, 168);
            this.Letter03.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Letter03.Name = "Letter03";
            this.Letter03.Size = new System.Drawing.Size(0, 31);
            this.Letter03.TabIndex = 0;
            // 
            // Letter02
            // 
            this.Letter02.AutoSize = true;
            this.Letter02.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Letter02.Location = new System.Drawing.Point(341, 168);
            this.Letter02.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Letter02.Name = "Letter02";
            this.Letter02.Size = new System.Drawing.Size(0, 31);
            this.Letter02.TabIndex = 1;
            // 
            // Letter01
            // 
            this.Letter01.AutoSize = true;
            this.Letter01.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Letter01.Location = new System.Drawing.Point(211, 168);
            this.Letter01.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Letter01.Name = "Letter01";
            this.Letter01.Size = new System.Drawing.Size(0, 31);
            this.Letter01.TabIndex = 2;
            // 
            // Letter00
            // 
            this.Letter00.AutoSize = true;
            this.Letter00.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Letter00.Location = new System.Drawing.Point(81, 168);
            this.Letter00.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Letter00.Name = "Letter00";
            this.Letter00.Size = new System.Drawing.Size(0, 31);
            this.Letter00.TabIndex = 3;
            // 
            // Letter10
            // 
            this.Letter10.AutoSize = true;
            this.Letter10.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Letter10.Location = new System.Drawing.Point(81, 244);
            this.Letter10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Letter10.Name = "Letter10";
            this.Letter10.Size = new System.Drawing.Size(0, 31);
            this.Letter10.TabIndex = 7;
            // 
            // Letter11
            // 
            this.Letter11.AutoSize = true;
            this.Letter11.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Letter11.Location = new System.Drawing.Point(211, 244);
            this.Letter11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Letter11.Name = "Letter11";
            this.Letter11.Size = new System.Drawing.Size(0, 31);
            this.Letter11.TabIndex = 6;
            // 
            // Letter12
            // 
            this.Letter12.AutoSize = true;
            this.Letter12.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Letter12.Location = new System.Drawing.Point(341, 244);
            this.Letter12.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Letter12.Name = "Letter12";
            this.Letter12.Size = new System.Drawing.Size(0, 31);
            this.Letter12.TabIndex = 5;
            // 
            // Letter13
            // 
            this.Letter13.AutoSize = true;
            this.Letter13.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Letter13.Location = new System.Drawing.Point(471, 244);
            this.Letter13.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Letter13.Name = "Letter13";
            this.Letter13.Size = new System.Drawing.Size(0, 31);
            this.Letter13.TabIndex = 4;
            // 
            // Letter30
            // 
            this.Letter30.AutoSize = true;
            this.Letter30.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Letter30.Location = new System.Drawing.Point(81, 380);
            this.Letter30.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Letter30.Name = "Letter30";
            this.Letter30.Size = new System.Drawing.Size(0, 31);
            this.Letter30.TabIndex = 15;
            // 
            // Letter31
            // 
            this.Letter31.AutoSize = true;
            this.Letter31.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Letter31.Location = new System.Drawing.Point(211, 380);
            this.Letter31.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Letter31.Name = "Letter31";
            this.Letter31.Size = new System.Drawing.Size(0, 31);
            this.Letter31.TabIndex = 14;
            // 
            // Letter32
            // 
            this.Letter32.AutoSize = true;
            this.Letter32.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Letter32.Location = new System.Drawing.Point(341, 380);
            this.Letter32.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Letter32.Name = "Letter32";
            this.Letter32.Size = new System.Drawing.Size(0, 31);
            this.Letter32.TabIndex = 13;
            // 
            // Letter33
            // 
            this.Letter33.AutoSize = true;
            this.Letter33.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Letter33.Location = new System.Drawing.Point(471, 380);
            this.Letter33.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Letter33.Name = "Letter33";
            this.Letter33.Size = new System.Drawing.Size(0, 31);
            this.Letter33.TabIndex = 12;
            // 
            // Letter20
            // 
            this.Letter20.AutoSize = true;
            this.Letter20.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Letter20.Location = new System.Drawing.Point(81, 305);
            this.Letter20.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Letter20.Name = "Letter20";
            this.Letter20.Size = new System.Drawing.Size(0, 31);
            this.Letter20.TabIndex = 11;
            // 
            // Letter21
            // 
            this.Letter21.AutoSize = true;
            this.Letter21.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Letter21.Location = new System.Drawing.Point(211, 305);
            this.Letter21.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Letter21.Name = "Letter21";
            this.Letter21.Size = new System.Drawing.Size(0, 31);
            this.Letter21.TabIndex = 10;
            // 
            // Letter22
            // 
            this.Letter22.AutoSize = true;
            this.Letter22.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Letter22.Location = new System.Drawing.Point(341, 305);
            this.Letter22.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Letter22.Name = "Letter22";
            this.Letter22.Size = new System.Drawing.Size(0, 31);
            this.Letter22.TabIndex = 9;
            // 
            // Letter23
            // 
            this.Letter23.AutoSize = true;
            this.Letter23.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Letter23.Location = new System.Drawing.Point(471, 305);
            this.Letter23.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Letter23.Name = "Letter23";
            this.Letter23.Size = new System.Drawing.Size(0, 31);
            this.Letter23.TabIndex = 8;
            // 
            // Scorepanel
            // 
            this.Scorepanel.Controls.Add(this.Player2ScoreLabel);
            this.Scorepanel.Controls.Add(this.Player2Label);
            this.Scorepanel.Controls.Add(this.TimeLabel);
            this.Scorepanel.Controls.Add(this.Player1ScoreLabel);
            this.Scorepanel.Controls.Add(this.Player1Label);
            this.Scorepanel.Location = new System.Drawing.Point(8, 105);
            this.Scorepanel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Scorepanel.Name = "Scorepanel";
            this.Scorepanel.Size = new System.Drawing.Size(576, 47);
            this.Scorepanel.TabIndex = 0;
            // 
            // Player2ScoreLabel
            // 
            this.Player2ScoreLabel.AutoSize = true;
            this.Player2ScoreLabel.Location = new System.Drawing.Point(479, 18);
            this.Player2ScoreLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Player2ScoreLabel.Name = "Player2ScoreLabel";
            this.Player2ScoreLabel.Size = new System.Drawing.Size(38, 13);
            this.Player2ScoreLabel.TabIndex = 4;
            this.Player2ScoreLabel.Text = "Score:";
            // 
            // Player2Label
            // 
            this.Player2Label.AutoSize = true;
            this.Player2Label.Location = new System.Drawing.Point(333, 18);
            this.Player2Label.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Player2Label.Name = "Player2Label";
            this.Player2Label.Size = new System.Drawing.Size(48, 13);
            this.Player2Label.TabIndex = 3;
            this.Player2Label.Text = "Player 2:";
            // 
            // TimeLabel
            // 
            this.TimeLabel.AutoSize = true;
            this.TimeLabel.Location = new System.Drawing.Point(236, 18);
            this.TimeLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.TimeLabel.Name = "TimeLabel";
            this.TimeLabel.Size = new System.Drawing.Size(54, 13);
            this.TimeLabel.TabIndex = 2;
            this.TimeLabel.Text = "Time Left:";
            // 
            // Player1ScoreLabel
            // 
            this.Player1ScoreLabel.AutoSize = true;
            this.Player1ScoreLabel.Location = new System.Drawing.Point(141, 18);
            this.Player1ScoreLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Player1ScoreLabel.Name = "Player1ScoreLabel";
            this.Player1ScoreLabel.Size = new System.Drawing.Size(38, 13);
            this.Player1ScoreLabel.TabIndex = 1;
            this.Player1ScoreLabel.Text = "Score:";
            // 
            // Player1Label
            // 
            this.Player1Label.AutoSize = true;
            this.Player1Label.Location = new System.Drawing.Point(18, 18);
            this.Player1Label.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Player1Label.Name = "Player1Label";
            this.Player1Label.Size = new System.Drawing.Size(48, 13);
            this.Player1Label.TabIndex = 0;
            this.Player1Label.Text = "Player 1:";
            // 
            // Player1WordsPlayedLabel
            // 
            this.Player1WordsPlayedLabel.AutoSize = true;
            this.Player1WordsPlayedLabel.Location = new System.Drawing.Point(100, 220);
            this.Player1WordsPlayedLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Player1WordsPlayedLabel.Name = "Player1WordsPlayedLabel";
            this.Player1WordsPlayedLabel.Size = new System.Drawing.Size(76, 13);
            this.Player1WordsPlayedLabel.TabIndex = 16;
            this.Player1WordsPlayedLabel.Text = "Words Played:";
            // 
            // Player2WordsPlayedLabel
            // 
            this.Player2WordsPlayedLabel.AutoSize = true;
            this.Player2WordsPlayedLabel.Location = new System.Drawing.Point(411, 220);
            this.Player2WordsPlayedLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Player2WordsPlayedLabel.Name = "Player2WordsPlayedLabel";
            this.Player2WordsPlayedLabel.Size = new System.Drawing.Size(76, 13);
            this.Player2WordsPlayedLabel.TabIndex = 17;
            this.Player2WordsPlayedLabel.Text = "Words Played:";
            // 
            // Boggle
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(594, 452);
            this.Controls.Add(this.Player2WordsPlayedLabel);
            this.Controls.Add(this.Player1WordsPlayedLabel);
            this.Controls.Add(this.Scorepanel);
            this.Controls.Add(this.Letter30);
            this.Controls.Add(this.Letter31);
            this.Controls.Add(this.Letter32);
            this.Controls.Add(this.Letter33);
            this.Controls.Add(this.Letter20);
            this.Controls.Add(this.Letter21);
            this.Controls.Add(this.Letter22);
            this.Controls.Add(this.Letter23);
            this.Controls.Add(this.Letter10);
            this.Controls.Add(this.Letter11);
            this.Controls.Add(this.Letter12);
            this.Controls.Add(this.Letter13);
            this.Controls.Add(this.Letter00);
            this.Controls.Add(this.Letter01);
            this.Controls.Add(this.Letter02);
            this.Controls.Add(this.Letter03);
            this.Controls.Add(this.OptionsSplitContainer);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MaximumSize = new System.Drawing.Size(610, 491);
            this.MinimumSize = new System.Drawing.Size(610, 491);
            this.Name = "Boggle";
            this.Text = "Boggle";
            this.OptionsSplitContainer.Panel1.ResumeLayout(false);
            this.OptionsSplitContainer.Panel1.PerformLayout();
            this.OptionsSplitContainer.Panel2.ResumeLayout(false);
            this.OptionsSplitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.OptionsSplitContainer)).EndInit();
            this.OptionsSplitContainer.ResumeLayout(false);
            this.Scorepanel.ResumeLayout(false);
            this.Scorepanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer OptionsSplitContainer;
        private System.Windows.Forms.Label DomainLabel;
        private System.Windows.Forms.Label UsernameLabel;
        private System.Windows.Forms.TextBox DomainNameTextBox;
        private System.Windows.Forms.TextBox RegisterUserTextBox;
        private System.Windows.Forms.Button RegisterButton;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Button CreateGameButton;
        private System.Windows.Forms.TextBox GameTimeTextBox;
        private System.Windows.Forms.Label GameTimeLabel;
        private System.Windows.Forms.Panel BogglePanel;
        private System.Windows.Forms.Label Letter03;
        private System.Windows.Forms.Label Letter02;
        private System.Windows.Forms.Label Letter01;
        private System.Windows.Forms.Label Letter00;
        private System.Windows.Forms.Label Letter10;
        private System.Windows.Forms.Label Letter11;
        private System.Windows.Forms.Label Letter12;
        private System.Windows.Forms.Label Letter13;
        private System.Windows.Forms.Label Letter30;
        private System.Windows.Forms.Label Letter31;
        private System.Windows.Forms.Label Letter32;
        private System.Windows.Forms.Label Letter33;
        private System.Windows.Forms.Label Letter20;
        private System.Windows.Forms.Label Letter21;
        private System.Windows.Forms.Label Letter22;
        private System.Windows.Forms.Label Letter23;
        private System.Windows.Forms.TextBox EnterWordsTextBox;
        private System.Windows.Forms.Label EnterWordsLabel;
        private System.Windows.Forms.Button GameStartedButton;
        private System.Windows.Forms.Panel Scorepanel;
        private System.Windows.Forms.Label Player2ScoreLabel;
        private System.Windows.Forms.Label Player2Label;
        private System.Windows.Forms.Label TimeLabel;
        private System.Windows.Forms.Label Player1ScoreLabel;
        private System.Windows.Forms.Label Player1Label;
        private System.Windows.Forms.Label Player1WordsPlayedLabel;
        private System.Windows.Forms.Label Player2WordsPlayedLabel;
    }
}

