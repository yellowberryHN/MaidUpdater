using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MaidUpdater
{
    public class MaidVM : INotifyPropertyChanged
    {
        public static string AppName { get => $"Maid Updater {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}"; }

        private Game _selectedGame;
        public Game SelectedGame { 
            get => _selectedGame;
            set
            {
                PacksAvailable?.Clear();
                PacksSelected?.Clear();
                _selectedGame = value;
            }
        }
        public bool VerboseLogging { get; set; } = true;

        public ObservableCollection<UpdatePack> PacksAvailable { get; set; }

        public ObservableCollection<UpdatePack> PacksSelected { get; set; }

        private string _appLog = string.Empty;
        public string AppLog
        {
            get { return _appLog; }
            set { }
        }

        internal void AppendLog(string text)
        {
            _appLog += text;
            OnPropertyChanged("AppLog");
        }

        public ObservableCollection<Game> GamesInstalled { get; set; }

        public ObservableCollection<Game> Games = new ObservableCollection<Game>()
        {
            new Game(@"SOFTWARE\KISS\カスタムメイド3D2", GameVersion.CM3D2),
            new Game(@"SOFTWARE\KISS\カスタムオーダーメイド3D2", GameVersion.COM3D2),
            new Game(@"SOFTWARE\KISS\CUSTOM ORDER MAID3D 2", GameVersion.COM3D2),
            new Game(@"SOFTWARE\SCourt\CUSTOM ORDER MAID3D 2 It's a Night Magic", GameVersion.COM3D2),
        };

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public MaidVM()
        {
            PacksAvailable = new ObservableCollection<UpdatePack>();
            GamesInstalled = DetectGames();

            ScanButtonCommand = new RelayCommand(new Action<object>(ScanDirs));
            VerifyButtonCommand = new RelayCommand(new Action<object>(VerifySelected));
            InstallButtonCommand = new RelayCommand(new Action<object>(ShowInstaller));
        }
        public void Log(string str, bool verbose = false, bool newline = true)
        {
            if (!verbose || (verbose && VerboseLogging)) AppendLog(str + (newline ? Environment.NewLine : ""));
        }

        ObservableCollection<Game> DetectGames()
        {
            var detected = new ObservableCollection<Game>();
            foreach (Game g in Games)
            {
                var key = Registry.CurrentUser.OpenSubKey(g.regkey);
                string path = string.Empty;
                if (key != null) path = (string)key.GetValue("InstallPath", string.Empty);
                if (path != string.Empty)
                {
                    g.path = path;
                    detected.Add(g);
                }
            }

            if (detected.Count == 0)
            {
                MessageBox.Show("Could not find CM3D2 or COM3D2 install directories on your system!\n\nHint: Are they registered in your registry?", "Maid Updater - Games not detected!", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(1); // TODO: Don't fail right here, ask where it's installed and optionally add to registry.
            }

            return detected;
        }



        void ScanDirs(object obj)
        {
            PacksAvailable?.Clear();
            PacksSelected?.Clear();
            CheckDir();
        }

        void CheckDir(string path = ".")
        {
            string[] dirs = { };
            path = path + "/";
            if (path != "./") Log("Scanning " + path, true);

            try { dirs = Directory.GetDirectories(path); }
            catch (UnauthorizedAccessException)
            {
                Log($"Error scanning {path}, access denied!");
                return;
            }

            foreach (var dir in dirs)
            {
                if (dir.Substring(dir.Length - 4) == "data" && path != "./")
                {
                    if (Array.IndexOf(Directory.GetFiles(path), Path.Combine(path, "update.lst")) != -1)
                    {
                        var version = GameVersion.None;
                        if (File.Exists(Path.Combine(path, "update.ini")))
                        {
                            var ini = new IniFile(Path.Combine(path, "update.ini"));
                            var exe = ini.Read("AppExe", "UPDATER");
                            if (exe.Contains("CM3D2")) version = GameVersion.CM3D2;
                            else if (exe.Contains("COM3D2")) version = GameVersion.COM3D2;
                            if (version != GameVersion.None)
                            {
                                Log($"Found installable content for {version}!", true);
                                PacksAvailable.Add(new UpdatePack(path, version, SelectedGame.version));
                            }
                        }
                    }
                }
                else
                {
                    CheckDir(dir);
                }
            }
            if (path == "./" && PacksAvailable.Count < 1) Log("Scan did not find any content!");
        }

        public ICommand ScanButtonCommand { get; set; }

        void GetSelectedPacks(object obj)
        {
            // holy shit what a hack
            System.Collections.IList items = (System.Collections.IList)obj;
            PacksSelected = new ObservableCollection<UpdatePack>(items.Cast<UpdatePack>().Where(u => u.Supported));
        }

        void VerifySelected(object obj)
        {
            GetSelectedPacks(obj);

            //Log($"Got {PacksSelected.Count} packs");

            Log("Verifying content...");
            foreach (var pack in PacksSelected)
                if (!pack.verifyUpdates())
                {
                    Log($"Failed to verify {pack.name}! Please redownload this DLC and try again!"); 
                    return;
                }
            Log("Content verified successfully!");
        }

        public ICommand VerifyButtonCommand { get; set; }

        private void ShowInstaller(object obj)
        {
            GetSelectedPacks(obj);

            var objPopupwindow = new InstallerWindow();
            objPopupwindow.Show();
        }

        public ICommand InstallButtonCommand { get; set; }

        /*
        public bool installItem(string cPath)
        {
            if (install)
            {
                Log($"Installing {name}: ", true, false);

                string itemPath = Path.Combine(Program.installDir, path);
                string itemDir = Path.GetDirectoryName(itemPath);

                if (File.Exists(itemPath)) File.Delete(itemPath);
                else if (path != name) Directory.CreateDirectory(itemDir);
                try
                {
                    File.Copy(Path.Combine(cPath, packPath), Path.Combine(Program.installDir, path));
                    Log("success", true);
                    return true;
                }
                catch (Exception e)
                {
                    Program.Log("fail", true);
                    Program.Log($"Copying {name} failed with error \"{e.Message}\"!", true);
                    return false;
                }
            }
            return true;
        }

        public bool install()
        {
            foreach (var update in updatelst)
            {
                if (!update.installItem(path)) return false;
            }
            return true;
        }
        */

    }
}
