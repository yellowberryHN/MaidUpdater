using System;
using System.IO;
using System.Windows.Forms;

namespace MaidUpdater {
    public partial class GUI : Form {
        public GUI() {
            InitializeComponent();
            Text = $"Maid Updater {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}";
        }

        protected override void OnLoad(EventArgs e) {
            SwitchGame(0);
            gameSelector.DataSource = Program.gamesInstalled;
            base.OnLoad(e);
            
            Program.console.BackColor = consolePlaceholder.BackColor; //workaround for making console logging global with VS Form designer
            Program.console.HideSelection = false;
            Program.console.Location = consolePlaceholder.Location;
            Program.console.Name = "console";
            Program.console.ReadOnly = true;
            Program.console.Size = consolePlaceholder.Size;
            Program.console.TabIndex = 2;
            Program.console.WordWrap = consolePlaceholder.WordWrap;
            Controls.Add(Program.console);
        }

        public void SwitchGame(int index)
        {
            if (index < 0 || index > Program.gamesInstalled.Count - 1) return;
            Program.gameSelected = index;
            var updateLstPath = Path.Combine(Program.installDir, "update.lst");
            if (!File.Exists(updateLstPath))
            {
                MessageBox.Show("Game update list not found!", "Are you sure you have the game installed properly?", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
            Program.installed.Clear();
            Program.installedContent.Clear();
            contentSelector.Items.Clear();
            contentSelector.DisabledIndices.Clear();
            Program.selected.Clear();
            Program.allContent.Clear();
            var lines = File.ReadLines(updateLstPath);
            foreach (var line in lines)
            {
                Program.installed.Add(line);
                Program.installedContent.Add(new UpdateItem(line.Split(','), Program.installDir));
            }
            Program.Log($"Using {Program.gamesInstalled[index].version} from {Program.installDir}", true);
        }

        public void checkDir(string path) {

            string[] dirs = {};
            path = path + "/";
            if (path != "./") Program.Log("Scanning " + path, true);

            try { dirs = Directory.GetDirectories(path); }
            catch (UnauthorizedAccessException)
            {
                Program.Log($"Error scanning {path}, access denied!");
                return;
            }

            foreach (var dir in dirs)
            {
                if (dir.Substring(dir.Length - 4) == "data" && path != "./")
                {
                    if (Array.IndexOf(Directory.GetFiles(path), Path.Combine(path, "update.lst")) != -1)
                    {
                        var version = GameVersion.None;
                        if(File.Exists(Path.Combine(path, "update.ini")))
                        {
                            var ini = new IniFile(Path.Combine(path, "update.ini"));
                            var exe = ini.Read("AppExe", "UPDATER");
                            if (exe.Contains("CM3D2")) version = GameVersion.CM3D2;
                            else if (exe.Contains("COM3D2")) version = GameVersion.COM3D2;
                            if(version != GameVersion.None)
                            {
                                bool supported = false;
                                if (Program.versionSelected == GameVersion.CM3D2 && version != GameVersion.CM3D2)
                                    supported = false;
                                /*else if (File.Exists(Path.Combine(path, "kiss_update_core.dll")))
                                {
                                    Program.Log("New generation updater detected, no clue how to handle these, disabling!", true);
                                    supported = false;
                                }*/
                                else supported = true;
                                Program.Log($"Found installable content for {version}!", true);
                                var ind = contentSelector.Items.Add(Path.GetFileName(Path.GetDirectoryName(path)));
                                if(!supported) contentSelector.DisableItem(ind);
                                Program.allContent.Add(new UpdatePack(path, version));
                            }
                        }
                    }
                }
                else
                {
                    checkDir(dir);
                }
            }
            if (path == "./" && contentSelector.Items.Count < 1) Program.Log("Scan did not find any content!");
        }

        public void contentSelector_SelectedIndexChanged(object sender, EventArgs e) {
            verifyButton.Enabled = contentSelector.SelectedIndex != -1;
            installButton.Enabled = contentSelector.Items.Count > contentSelector.DisabledIndices.Count;
            if (!verifyButton.Enabled) installButton.Text = "&Install All";
            else installButton.Text = "&Install Selected";
            Program.selected.Clear();
            foreach (var index in contentSelector.SelectedIndices) {
                Program.selected.Add(Program.allContent[int.Parse(index.ToString())]); // a surprisingly intelligent hack
            }
        }

        public void scanButton_Click(object sender, EventArgs e) {
            contentSelector.Items.Clear();
            contentSelector.DisabledIndices.Clear();
            Program.selected.Clear();
            Program.allContent.Clear();
            //Program.console.ResetText();
            Program.Log("Scanning all directories...");
            checkDir(".");
            installButton.Enabled = contentSelector.Items.Count > contentSelector.DisabledIndices.Count;
        }

        public void installButton_Click(object sender, EventArgs e) {
            if(contentSelector.SelectedItems.Count < 1)
            {
                for (int i = 0; i < Program.allContent.Count; i++)
                {
                    if(!contentSelector.DisabledIndices.Contains(i)) Program.selected.Add(Program.allContent[i]);
                }
            }
            bool verified = false;
            if(Program.selected.Count < 1)
            {
                MessageBox.Show("No content available to install!", "Maid Updater - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            foreach (var content in Program.selected)
            {
                if (!(verified = content.verifyUpdates()))
                {
                    contentSelector.Items.Clear();
                    Program.selected.Clear();
                    installButton.Enabled = false;
                    return;
                }
                if (Program.versionSelected == GameVersion.COM3D2 && content.version == GameVersion.CM3D2)
                    foreach (var file in content.updatelst)
                        file.path = file.path.Replace("GameData", "GameData_20"); // If installing old content, use compat directory.
            }
            Form installDialog = new Installer();
            installDialog.ShowDialog();
        }

        private void verifyButton_Click(object sender, EventArgs e) {
            Program.Log("Verifying content...");
            foreach (var content in Program.selected) if(!content.verifyUpdates()) return;
            Program.Log("Content verified successfully!");
        }

        private void gameSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (gameSelector.SelectedIndex == Program.gameSelected) return;
            SwitchGame(gameSelector.SelectedIndex);
        }

        private void verboseCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Program.verboseLogging = verboseCheckBox.Checked;
        }

        private void aboutButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Original (cm3d2-updater) by thepotatomaster\nClean-up, fixes, COM3D2 support by Yellowberry\n\nSpecial thanks to the meido community!", "Maid Updater - About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void scanBgWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            contentSelector.Items.Clear();
            contentSelector.DisabledIndices.Clear();
            Program.selected.Clear();
            Program.allContent.Clear();
            Program.Log("Scanning all directories...");
            checkDir(".");
            installButton.Enabled = contentSelector.Items.Count > contentSelector.DisabledIndices.Count;
            this.Cursor = Cursors.Default;
        }
    }
}
