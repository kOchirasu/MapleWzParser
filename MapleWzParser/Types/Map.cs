using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MapleWzParser.Types {
    public class Map : MapNode {
        public new Dictionary<int, short[]> Portals { get; set; }
        public Dictionary<int, string> Cmd { get; set; }

        public Dictionary<int, int> Weight { get; set; }

        public Map(int id) : base(id, null, new Dictionary<int, int>()) {
            Weight = new Dictionary<int, int>();
            Portals = new Dictionary<int, short[]>();
            Cmd = new Dictionary<int, string>();
        }

        public void WriteTo(BinaryWriter writer) {
            writer.Write(Id);
            writer.Write(Portals.Count);

            foreach (int key in Portals.Keys) {
                writer.Write(key);
                writer.Write(Portals[key][0]);
                writer.Write(Portals[key][1]);
                writer.Write(Cmd[key]);
            }
            writer.Write(Choice.Count);
            foreach (KeyValuePair<int, int> pair in Choice) {
                writer.Write(pair.Key);
                writer.Write(pair.Value);
            }
        }

        public override string ToString() {
            var sb = new StringBuilder();
            sb.AppendLine(Id.ToString());
            foreach (KeyValuePair<int, short[]> entry in Portals) {
                sb.AppendLine("\t" + entry.Key + " : " + entry.Value[0] + ", " + entry.Value[1]);
            }
            return sb.ToString();
        }
    }
}
