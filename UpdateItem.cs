using System;
using System.IO;
using Force.Crc32;

namespace MaidUpdater {
    public class UpdateItem {
        public string path;
        public string name;
        public string cPath;
        public string expectedHash = string.Empty;
        public int version;
        public bool install;

        public UpdateItem(string[] fileDef, string mainPath) {
            cPath = mainPath;
            path = fileDef[0];
            name = Path.GetFileName(path);
            version = int.Parse(fileDef[1]);
        }

        public UpdateItem(string dir, string mainPath) {
            string[] args = dir.Split(',');
            cPath = mainPath;
            path = args[2];
            expectedHash = args[4];
            name = Path.GetFileName(path);
            version = int.Parse(args[args.Length - 1]);
        }

        public bool verifyIntegrity() { // TODO: Make sure the file actually exists, dumbass.
            Crc32Algorithm crc32 = new Crc32Algorithm();
            string hash = string.Empty;
            using (FileStream fs = File.Open(cPath + "data/" + path, FileMode.Open))
                foreach (byte b in crc32.ComputeHash(fs)) hash += b.ToString("x2").ToUpper();
            return hash == expectedHash; //TODO
        }

        public void installItem(string cPath) {
            if (install) {
                string item = $@"{Program.installDir}\{Path.GetFileName(path)}";
                string tmp = Program.installDir + @"\" + path.Replace(@"\" + path.Split('\\')[path.Split('\\').Length - 1], "");
                Program.Log("Installing " + name); //TODO
                if (File.Exists(Program.installDir + @"\" + path)) {
                    File.Delete(Program.installDir + @"\" + path);
                } else if (path != name) {
                    if (!Directory.Exists(tmp)) Directory.CreateDirectory(tmp); Directory.CreateDirectory(tmp);
                }
                string[] args = tmp.Split('\\');
                Array.Resize(ref args, args.Length - 1);
                File.Copy(cPath + @"data\" + path, Program.installDir + @"\" + path); //Wont work if directory name is too long sadly, working on a fix
            }
        }
    }
}
