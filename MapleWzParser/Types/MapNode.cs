using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace MapleWzParser.Types {
    public class MapNode {
        public int Id { get; }
        public IDictionary<int, Tuple<short[], string>> Portals;
        public IDictionary<int, int> Choice { get; set; }

        public MapNode(int id, IDictionary<int, Tuple<short[], string>> portals, IDictionary<int, int> choice) {
            Id = id;
            Portals = portals;
            Choice = choice;
        }

        public MapNode(BinaryReader reader) {
            Id = reader.ReadInt32();

            int count = reader.ReadInt32();
            Dictionary<int, Tuple<short[], string>> portals = new Dictionary<int, Tuple<short[], string>>(count);
            Portals = new ReadOnlyDictionary<int, Tuple<short[], string>>(portals);
            for (int i = 0; i < count; i++) {
                int key = reader.ReadInt32();
                short[] coord = { reader.ReadInt16(), reader.ReadInt16() };
                Tuple<short[], string> tuple = new Tuple<short[], string>(coord, reader.ReadString());
                portals[key] = tuple;
            }

            count = reader.ReadInt32();
            Dictionary<int, int> choice = new Dictionary<int, int>(count);
            Choice = new ReadOnlyDictionary<int, int>(choice);
            for (int i = 0; i < count; i++) {
                int key = reader.ReadInt32();
                int value = reader.ReadInt32();
                choice[key] = value;
            }
        }

        public override bool Equals(object o) {
            var map = o as MapNode;
            return Id == map?.Id;
        }

        public override int GetHashCode() {
            return Id.GetHashCode();
        }
    }
}
