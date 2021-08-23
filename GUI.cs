using System;
using System.IO;
using System.Windows.Forms;

namespace MaidUpdater {
    public partial class GUI : Form {
        public GUI() {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            if (!File.Exists(Program.installDir + @"\update.lst"))
            {
                MessageBox.Show("Game update list not found!", "Are you sure you have the game installed?", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
            string line;
            StreamReader file = new StreamReader(Program.installDir + @"\update.lst");
            while ((line = file.ReadLine()) != null) {
                Program.installed.Add(line);
                Program.installedContent.Add(new UpdateItem(line.Split(','), Program.installDir));
            }
            file.Close();
            Program.console.BackColor = consolePlaceholder.BackColor; //workaround for making console logging global with VS Form designer
            Program.console.HideSelection = false;
            Program.console.Location = consolePlaceholder.Location;
            Program.console.Name = "console";
            Program.console.ReadOnly = true;
            Program.console.Size = consolePlaceholder.Size;
            Program.console.TabIndex = 2;
            Program.console.Text = "";
            Program.console.WordWrap = consolePlaceholder.WordWrap;
            Controls.Add(Program.console);
        }

        public void checkDir(string path) {

            string[] dirs = {};
            path = path + "/";
            if (path != "./") Program.Log("Scanning " + path);

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
                    if (Array.IndexOf(Directory.GetFiles(path), path + "update.lst") != -1)
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
                                foreach (var game in Program.games)
                                {
                                    if (game.version == GameVersion.CM3D2 && version != GameVersion.CM3D2)
                                        supported = false;
                                    else supported = true;
                                }
                                Program.Log("Found installable content!");
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
        }

        public void contentSelector_SelectedIndexChanged(object sender, EventArgs e) {
            Program.selected.Clear();
            Program.hasVerified = false;
            foreach (var index in contentSelector.SelectedIndices) {
                Program.selected.Add(Program.allContent[Int32.Parse(index.ToString())]);
            }
        }

        public void scanButt_Click(object sender, EventArgs e) {
            contentSelector.Items.Clear();
            Program.selected.Clear();
            Program.allContent.Clear();
            Program.console.ResetText();
            Program.Log("Scanning all directories...");
            checkDir(".");
        }

        public void installButton_Click(object sender, EventArgs e) {
            Program.Log("Installing content...");
            if (contentSelector.SelectedItems.Count == 0) {
                MessageBox.Show("No installable content selected", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Program.Log("Warning: No installable content selected");
                return;
            }
            string line;
            StreamReader file = new StreamReader(Program.installDir + @"\update.lst");
            while ((line = file.ReadLine()) != null) {
                Program.installed.Add(line);
                Program.installedContent.Add(new UpdateItem(line.Split(','), Program.installDir));
            }
            file.Close();
            Form installDialog = new Installer();
            installDialog.ShowDialog();
        }

        private void verifyButt_Click(object sender, EventArgs e) {
            Program.hasVerified = true;
            foreach (var content in Program.selected) content.verifyUpdates();
        }
    }
}
