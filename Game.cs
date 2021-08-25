using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaidUpdater
{
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
}
