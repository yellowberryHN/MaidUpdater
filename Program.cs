using System;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Collections.Generic;

namespace MaidUpdater {

    public enum GameVersion
    {
        None = 0,
        CM3D2,
        COM3D2,
    }

    public class Game
    {
        public string executable; // TODO: implement later
        public string regkey;
        public string path;
        public GameVersion version;

        public Game(GameVersion ver, string dir)
        {
            path = dir;
            version = ver;
        }

        public Game(string subkey, GameVersion ver)
        {
            regkey = subkey;
            version = ver;
        }

        public override string ToString()
        {
            return $"{version} [{path}]";
        }
    }

    public static class Program {
        public static string installDir { get => gamesInstalled[gameSelected].path; }
        public static List<Game> gamesInstalled = new List<Game>();
        public static int gameSelected = 0;
        public static GameVersion versionSelected { get => gamesInstalled[gameSelected].version; }
        public static List<UpdatePack> allContent = new List<UpdatePack>();
        public static List<UpdatePack> selected = new List<UpdatePack>();
        public static List<UpdateItem> installedContent = new List<UpdateItem>();
        public static List<string> installed = new List<string>();
        public static RichTextBox console;
        public static bool verboseLogging = false;

        public static List<Game> games = new List<Game>()
        {
            new Game(@"SOFTWARE\KISS\カスタムメイド3D2", GameVersion.CM3D2),
            new Game(@"SOFTWARE\KISS\カスタムオーダーメイド3D2", GameVersion.COM3D2),
            new Game(@"SOFTWARE\KISS\CUSTOM ORDER MAID3D 2", GameVersion.COM3D2),
            new Game(@"SOFTWARE\SCourt\CUSTOM ORDER MAID3D 2 It's a Night Magic", GameVersion.COM3D2),
        };

        public static void Log(string str, bool verbose = false, bool newline = true) {
            if(!verbose || (verbose && Program.verboseLogging)) console.AppendText(str+(newline?Environment.NewLine:"")); // this is gross
        }

        static void DetectGames()
        {
            foreach (var game in games)
            {
                var key = Registry.CurrentUser.OpenSubKey(game.regkey);
                string path = string.Empty;
                if (key != null) path = (string)key.GetValue("InstallPath", string.Empty);
                if (path != string.Empty)
                {
                    gamesInstalled.Add(new Game(game.version, path));
                }
            }

            if (gamesInstalled.Count == 0)
            {
                MessageBox.Show("Could not find CM3D2 or COM3D2 install directories on your system!\n\nHint: Are they registered in your registry?", "Maid Updater - Games not detected!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1); // TODO: Don't fail right here, ask where it's installed and optionally add to registry.
            }

            return;
        }

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            console = new RichTextBox();

            DetectGames();
            Application.Run(new GUI());
        }
    }
}
