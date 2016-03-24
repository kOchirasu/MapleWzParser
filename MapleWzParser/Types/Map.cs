using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MapleWzParser.Types {
    public class Map : MapNode {
        public IDictionary<int, int> Weight { get; set; }

        public Map(int id, IDictionary<int, PortalInfo> portals) : base(id, portals, new Dictionary<int, int>()) {
            Weight = new Dictionary<int, int>();
        }

        public void WriteTo(BinaryWriter writer) {
            writer.Write(Id);

            writer.Write(Portals.Count);
            foreach (int key in Portals.Keys) {
                writer.Write(key);
                writer.Write(Portals[key].X);
                writer.Write(Portals[key].Y);
                writer.Write(Portals[key].Name);
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
            foreach (KeyValuePair<int, PortalInfo> entry in Portals) {
                sb.AppendLine("\t" + entry.Key + ":" + entry.Value);
            }
            return sb.ToString();
        }
    }
}
