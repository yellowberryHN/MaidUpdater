using System;
using System.IO;
using Force.Crc32;

namespace MaidUpdater {
    public class UpdateItem : IComparable<UpdateItem> {
        public string path; // path to installed content
        public string packPath;
        public string name;
        public string basePath;
        public string expectedHash = string.Empty;
        public int version;
        public bool install;
        public bool verified;

        public UpdateItem(string[] fileDef, string mainPath) {
            basePath = mainPath;
            path = fileDef[0];
            name = Path.GetFileName(path);
            version = int.Parse(fileDef[1]);
        }

        public UpdateItem(string lst, string mainPath) {
            string[] args = lst.Split(',');
            basePath = mainPath;
            path = args[2];
            packPath = (args[0] == "share" && args[1] != "0")? args[1] : Path.Combine("data",path); 
            expectedHash = args[4];
            name = Path.GetFileName(path);
            version = int.Parse(args[args.Length - 1]);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            UpdateItem item = obj as UpdateItem;
            if (item == null) return false;
            else return Equals(item);
        }
        public override int GetHashCode()
        {
            return path.GetHashCode();
        }
        public bool Equals(UpdateItem other)
        {
            if (other == null) return false;
            return path.Equals(other.path) && version.Equals(other.version);
        }

        public override string ToString()
        {
            return $"{path},{version}";
        }

        public bool verifyIntegrity() {
            Crc32Algorithm crc32 = new Crc32Algorithm();
            string hash = string.Empty;
            string filename = Path.Combine(basePath, packPath);
            if (!File.Exists(filename)) return false; // file doesn't exist
            using (FileStream fs = File.Open(filename, FileMode.Open, FileAccess.Read))
                foreach (byte b in crc32.ComputeHash(fs)) hash += b.ToString("x2").ToUpper();
            verified = hash == expectedHash && !string.IsNullOrEmpty(hash);
            return verified;
        }

        public bool installItem(string cPath) {
            if (install) {
                Program.Log($"Installing {name}: ", true, false);

                string itemPath = Path.Combine(Program.installDir, path);
                string itemDir = Path.GetDirectoryName(itemPath);

                if (File.Exists(itemPath)) File.Delete(itemPath);
                else if (path != name) Directory.CreateDirectory(itemDir);
                try {
                    File.Copy(Path.Combine(cPath, packPath), Path.Combine(Program.installDir, path));
                    Program.Log("success", true);
                    return true;
                }
                catch (Exception e) {
                    Program.Log("fail", true);
                    Program.Log($"Copying {name} failed with error \"{e.Message}\"!", true);
                    return false; 
                }
            }
            return true;
        }

        public int CompareTo(UpdateItem other)
        {
            return name.CompareTo(other.name);
        }
    }
}
