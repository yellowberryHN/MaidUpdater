using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
                foreach (var file in sb.ToString().Split('\n')) {
                    if (new Regex(@"0,0,.*?,\d*,[0-9A-F]{8},\d*").IsMatch(file)) updatelst.Add(new UpdateItem(file, path));
                }
                finishedReading = true;
            }
        }

        public void install() {
            foreach (var update in updatelst) update.installItem(path);
        }

        public void verifyUpdates() { // TODO: Skip update check if already exists and is up to date
            Program.Log($"Verifying {name}...");
            foreach (var item in updatelst) {
                Program.Log($"Checking {item.name}: ", false);
                if (item.verifyIntegrity()) {
                    Program.Log("matches");
                    verified = true;
                } else {
                    Program.Log("hash was not verified!\nPlease redownload this DLC and try again");
                    verified = false;
                    break;
                }
            }
        }
    }
}
