namespace SpreadsheetGUI
{
    partial class SpreadsheetGUI
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
            this.spreadsheetPanel = new SSGui.SpreadsheetPanel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.CellValueLabel = new System.Windows.Forms.Label();
            this.StaticCellValueLabel = new System.Windows.Forms.Label();
            this.SetCellContentsTextBox = new System.Windows.Forms.TextBox();
            this.StaticSetContentsLabel = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // spreadsheetPanel
            // 
            this.spreadsheetPanel.AutoSize = true;
            this.spreadsheetPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.spreadsheetPanel.Location = new System.Drawing.Point(0, 123);
            this.spreadsheetPanel.Name = "spreadsheetPanel";
            this.spreadsheetPanel.Size = new System.Drawing.Size(948, 528);
            this.spreadsheetPanel.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(948, 33);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem1,
            this.saveToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(50, 29);
            this.openToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem1
            // 
            this.openToolStripMenuItem1.Name = "openToolStripMenuItem1";
            this.openToolStripMenuItem1.Size = new System.Drawing.Size(142, 30);
            this.openToolStripMenuItem1.Text = "Open";
            this.openToolStripMenuItem1.Click += new System.EventHandler(this.MenuItemOpen_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(142, 30);
            this.saveToolStripMenuItem.Text = "Save..";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.MenuItemSave_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(142, 30);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(61, 29);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // splitContainer
            // 
            this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer.Location = new System.Drawing.Point(0, 33);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.CellValueLabel);
            this.splitContainer.Panel1.Controls.Add(this.StaticCellValueLabel);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.SetCellContentsTextBox);
            this.splitContainer.Panel2.Controls.Add(this.StaticSetContentsLabel);
            this.splitContainer.Size = new System.Drawing.Size(948, 90);
            this.splitContainer.SplitterDistance = 442;
            this.splitContainer.TabIndex = 2;
            // 
            // CellValueLabel
            // 
            this.CellValueLabel.AutoSize = true;
            this.CellValueLabel.Location = new System.Drawing.Point(136, 55);
            this.CellValueLabel.Name = "CellValueLabel";
            this.CellValueLabel.Size = new System.Drawing.Size(172, 20);
            this.CellValueLabel.TabIndex = 2;
            this.CellValueLabel.Text = "Cell Name:   Cell Value:";
            this.CellValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // StaticCellValueLabel
            // 
            this.StaticCellValueLabel.AutoSize = true;
            this.StaticCellValueLabel.Location = new System.Drawing.Point(92, 17);
            this.StaticCellValueLabel.Name = "StaticCellValueLabel";
            this.StaticCellValueLabel.Size = new System.Drawing.Size(279, 20);
            this.StaticCellValueLabel.TabIndex = 0;
            this.StaticCellValueLabel.Text = "Cell Name and Cell Value of Selection:";
            // 
            // SetCellContentsTextBox
            // 
            this.SetCellContentsTextBox.Location = new System.Drawing.Point(77, 52);
            this.SetCellContentsTextBox.Name = "SetCellContentsTextBox";
            this.SetCellContentsTextBox.Size = new System.Drawing.Size(346, 26);
            this.SetCellContentsTextBox.TabIndex = 3;
            this.SetCellContentsTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SetCellContentsTextBox_KeyPress);
            // 
            // StaticSetContentsLabel
            // 
            this.StaticSetContentsLabel.AutoSize = true;
            this.StaticSetContentsLabel.Location = new System.Drawing.Point(120, 17);
            this.StaticSetContentsLabel.Name = "StaticSetContentsLabel";
            this.StaticSetContentsLabel.Size = new System.Drawing.Size(252, 20);
            this.StaticSetContentsLabel.TabIndex = 1;
            this.StaticSetContentsLabel.Text = "Set Cell Contents of Selected Cell:";
            // 
            // SpreadsheetGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(948, 651);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.spreadsheetPanel);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "SpreadsheetGUI";
            this.Text = "Spreadsheet";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SSGui.SpreadsheetPanel spreadsheetPanel;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem1;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Label CellValueLabel;
        private System.Windows.Forms.Label StaticCellValueLabel;
        private System.Windows.Forms.TextBox SetCellContentsTextBox;
        private System.Windows.Forms.Label StaticSetContentsLabel;
    }
}

