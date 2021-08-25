using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MaidUpdater { //TODO: Allow installing multiple files (DO THIS!!!)
    public partial class Installer : Form
    {
        public Installer()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            setScanning(true);
            foreach (var pack in Program.selected) { // TODO: Make this check if other files exist
                foreach (var file in pack.updatelst) {
                    bool install = false;
                    int currentVersion = 0;
                    bool skip = false;
                    foreach(var tmp in Program.selected)
                    {
                        // if file exists in other selected pack, if version of this one is lower, skip it.
                        if(tmp.updatelst.Exists(x => (x.name == file.name && x.version > file.version))) { skip = true; break; }
                    }
                    if(!skip)
                    {
                        var installedFile = Program.installedContent.Find(x => x.path == file.path);
                        if (installedFile != null)
                        {
                            if (installedFile.version == file.version) install = false; // do not install if same version
                            else if (installedFile.version < file.version) install = true; // install if older
                            currentVersion = installedFile.version;
                        }
                        else install = true;
                        if (!install && currentVersion == 0 || install) installButton.Enabled = file.install = install = true;
                        string strVer = currentVersion == 0 ? "none" : currentVersion.ToString();
                        var ind = selectedContent.Items.Add($"{file.name} | {strVer} -> {file.version}", install);
                    }
                }
            }
            setScanning(false);
        }

        private void cancelButton_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void selectedContent_ItemCheck(object sender, ItemCheckEventArgs e) {
            if (!scanning) e.NewValue = e.CurrentValue;
        }

        private void setScanning(bool enabled)
        {
            scanning = enabled;
            if (scanning) selectedContent.BackColor = Color.FromKnownColor(KnownColor.Window);
            else selectedContent.BackColor = Color.FromKnownColor(KnownColor.Control);
        }

        private void installButton_Click(object sender, EventArgs e) {
            foreach (var content in Program.selected) {
                if (content.verified) {
                    if(!content.install())
                    {
                        Program.Log($"[FATAL] {content.name} failed to install! Your game install may be broken.");
                        this.Close();
                        return;
                    }
                } else {
                    Program.Log($"[FATAL] {content.name} somehow wasn't verified, aborting! Your game install may be broken.");
                    this.Close();
                    return;
                }
            }
            this.Close();
            Program.Log("Updating update.lst"); // TODO: Sanity check the shit out of this. Don't want to be breaking game installs.
            foreach (var content in Program.selected) {
                if (content.verified) {
                    foreach (var item in content.updatelst) {
                        if (item.install) {
                            Program.Log($"Setting {item.name} installed version to {item.version}", true);
                            int installedIndex = -1;
                            foreach (var installed in Program.installed) {
                                if (installed.Contains(item.path)) {
                                    installedIndex = Program.installed.IndexOf(installed);
                                    break;
                                }
                            }
                            if (installedIndex != -1) {
                                Program.installed[installedIndex] = item.ToString();
                                Program.installedContent[installedIndex] = item;
                            } else {
                                Program.installed.Add(item.ToString());
                                Program.installedContent.Add(item);
                            }
                        }
                    }
                }
            }
            Program.installed.Sort();
            File.WriteAllText(Program.installDir + @"\update.lst", string.Join(Environment.NewLine, Program.installed));
        }
    }
}
