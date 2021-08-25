namespace MaidUpdater {
    public partial class GUI {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        public void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GUI));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.installButton = new System.Windows.Forms.Button();
            this.scanButt = new System.Windows.Forms.Button();
            this.verifyButton = new System.Windows.Forms.Button();
            this.consolePlaceholder = new System.Windows.Forms.RichTextBox();
            this.gameSelector = new System.Windows.Forms.ComboBox();
            this.helpToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.verboseCheckBox = new System.Windows.Forms.CheckBox();
            this.aboutButton = new System.Windows.Forms.Button();
            this.contentSelector = new MaidUpdater.ListBoxEx();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(399, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Available Content:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Console:";
            // 
            // installButton
            // 
            this.installButton.Enabled = false;
            this.installButton.Location = new System.Drawing.Point(12, 309);
            this.installButton.Name = "installButton";
            this.installButton.Size = new System.Drawing.Size(100, 23);
            this.installButton.TabIndex = 1;
            this.installButton.Text = "&Install All";
            this.installButton.UseVisualStyleBackColor = true;
            this.installButton.Click += new System.EventHandler(this.installButton_Click);
            // 
            // scanButt
            // 
            this.scanButt.Location = new System.Drawing.Point(402, 309);
            this.scanButt.Name = "scanButt";
            this.scanButt.Size = new System.Drawing.Size(175, 23);
            this.scanButt.TabIndex = 5;
            this.scanButt.Text = "&Scan Directory";
            this.scanButt.UseVisualStyleBackColor = true;
            this.scanButt.Click += new System.EventHandler(this.scanButton_Click);
            // 
            // verifyButton
            // 
            this.verifyButton.Enabled = false;
            this.verifyButton.Location = new System.Drawing.Point(118, 309);
            this.verifyButton.Name = "verifyButton";
            this.verifyButton.Size = new System.Drawing.Size(100, 23);
            this.verifyButton.TabIndex = 2;
            this.verifyButton.Text = "&Verify Contents";
            this.verifyButton.UseVisualStyleBackColor = true;
            this.verifyButton.Click += new System.EventHandler(this.verifyButton_Click);
            // 
            // consolePlaceholder
            // 
            this.consolePlaceholder.BackColor = System.Drawing.SystemColors.ControlLight;
            this.consolePlaceholder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.consolePlaceholder.Enabled = false;
            this.consolePlaceholder.Location = new System.Drawing.Point(12, 25);
            this.consolePlaceholder.Name = "consolePlaceholder";
            this.consolePlaceholder.Size = new System.Drawing.Size(384, 275);
            this.consolePlaceholder.TabIndex = 10;
            this.consolePlaceholder.TabStop = false;
            this.consolePlaceholder.Text = "";
            this.consolePlaceholder.Visible = false;
            // 
            // gameSelector
            // 
            this.gameSelector.FormattingEnabled = true;
            this.gameSelector.Location = new System.Drawing.Point(12, 338);
            this.gameSelector.Name = "gameSelector";
            this.gameSelector.Size = new System.Drawing.Size(565, 21);
            this.gameSelector.TabIndex = 6;
            this.helpToolTip.SetToolTip(this.gameSelector, "Not seeing one of your games? Check your registry!");
            this.gameSelector.SelectedIndexChanged += new System.EventHandler(this.gameSelector_SelectedIndexChanged);
            // 
            // helpToolTip
            // 
            this.helpToolTip.IsBalloon = true;
            this.helpToolTip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.helpToolTip.ToolTipTitle = "Hint";
            // 
            // verboseCheckBox
            // 
            this.verboseCheckBox.AutoSize = true;
            this.verboseCheckBox.Location = new System.Drawing.Point(290, 313);
            this.verboseCheckBox.Name = "verboseCheckBox";
            this.verboseCheckBox.Size = new System.Drawing.Size(106, 17);
            this.verboseCheckBox.TabIndex = 4;
            this.verboseCheckBox.Text = "V&erbose Logging";
            this.verboseCheckBox.UseVisualStyleBackColor = true;
            this.verboseCheckBox.CheckedChanged += new System.EventHandler(this.verboseCheckBox_CheckedChanged);
            // 
            // aboutButton
            // 
            this.aboutButton.Location = new System.Drawing.Point(224, 309);
            this.aboutButton.Name = "aboutButton";
            this.aboutButton.Size = new System.Drawing.Size(60, 23);
            this.aboutButton.TabIndex = 3;
            this.aboutButton.Text = "&About...";
            this.aboutButton.UseVisualStyleBackColor = true;
            this.aboutButton.Click += new System.EventHandler(this.aboutButton_Click);
            // 
            // contentSelector
            // 
            this.contentSelector.BackColor = System.Drawing.SystemColors.Window;
            this.contentSelector.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.contentSelector.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.contentSelector.FormattingEnabled = true;
            this.contentSelector.Location = new System.Drawing.Point(402, 25);
            this.contentSelector.Name = "contentSelector";
            this.contentSelector.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.contentSelector.Size = new System.Drawing.Size(175, 275);
            this.contentSelector.TabIndex = 0;
            this.contentSelector.SelectedIndexChanged += new System.EventHandler(this.contentSelector_SelectedIndexChanged);
            // 
            // GUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(589, 371);
            this.Controls.Add(this.aboutButton);
            this.Controls.Add(this.verboseCheckBox);
            this.Controls.Add(this.gameSelector);
            this.Controls.Add(this.consolePlaceholder);
            this.Controls.Add(this.verifyButton);
            this.Controls.Add(this.scanButt);
            this.Controls.Add(this.installButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.contentSelector);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "GUI";
            this.Text = "Maid Updater";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public MaidUpdater.ListBoxEx contentSelector;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.Button installButton;
        public System.Windows.Forms.Button scanButt;
        private System.Windows.Forms.Button verifyButton;
        private System.Windows.Forms.RichTextBox consolePlaceholder;
        private System.Windows.Forms.ComboBox gameSelector;
        private System.Windows.Forms.ToolTip helpToolTip;
        private System.Windows.Forms.CheckBox verboseCheckBox;
        private System.Windows.Forms.Button aboutButton;
    }
}

