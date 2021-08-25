using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace MaidUpdater {
    public class UpdatePack {
        public string path;
        public string name;
        public List<UpdateItem> updatelst = new List<UpdateItem>{};
        public bool finishedReading = false;
        public bool verified = false;
        public GameVersion version;

        public UpdatePack(string dir, GameVersion ver) {
            path = dir;
            name = Path.GetFileName(Path.GetDirectoryName(path)); // goddamn it. -Y
            version = ver;
            readUpdateList();
        }

        public async void readUpdateList() {
            StringBuilder sb = new StringBuilder();
            using (FileStream sourceStream = new FileStream(path+"update.lst",
                FileMode.Open, FileAccess.Read, FileShare.Read,
                bufferSize: 4096, useAsync: true)) {
                byte[] buffer = new byte[0x1000];
                int numRead;
                while ((numRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) != 0) {
                    string str = Encoding.UTF8.GetString(buffer, 0, numRead);
                    sb.Append(str);
                }
                foreach (var lst in sb.ToString().Split(Environment.NewLine.ToCharArray())) {
                    if (new Regex(@".*?,.*?,.*?,\d*,[0-9A-F]{8},\d*").IsMatch(lst)) updatelst.Add(new UpdateItem(lst, path));
                }
                finishedReading = true;
            }
            updatelst = updatelst.Distinct().ToList();
            updatelst.Sort();
        }

        public bool install() {
            foreach (var update in updatelst)
            {
                if (!update.installItem(path)) return false;
            }
            return true;
        }

        public bool verifyUpdates() { // TODO: Skip update check if already exists and is up to date
            Program.Log($"Verifying {name}...", true);
            foreach (var item in updatelst) {
                Program.Log($"Checking {item.name}: ", true, false);
                if (item.verified) Program.Log("skipping", true); // already previously verified, don't waste time
                else if (item.verifyIntegrity()) Program.Log("matches", true);
                else {
                    Program.Log("fail", true);
                    verified = false;
                    break;
                }
                verified = true;
            }
            if(!verified) Program.Log($"Failed to verify {name}! Please redownload this DLC and try again!");
            return verified;
        }
    }
}
