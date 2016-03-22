using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using MapleWzParser.Types;
using reWZ;
using reWZ.WZProperties;

namespace MapleWzParser.Parsers {
    public static class MapWzParser {
        private static readonly WZFile MapWz = new WZFile(Program.ROOT + "Map.wz", WZVariant.Classic, true);

        public static Dictionary<int, List<PortalInfo>> ParseMap() {
            Console.WriteLine("Parsing portals from Map.wz...");

            Dictionary<int, List<PortalInfo>> data = new Dictionary<int, List<PortalInfo>>();
            foreach (var region in MapWz.ResolvePath("Map")) {
                if (!Regex.Match(region.Name, "Map[0-9]").Success) continue;
                Console.WriteLine($"Parsing '{region.Name}'...");

                foreach (var map in region) {
                    int id = int.Parse(Path.GetFileNameWithoutExtension(map.Name));
                    List<PortalInfo> portals = ParseMapNode(map);
                    if (portals.Count == 0) continue; // No portals
                    data[id] = portals;
                }
            }
            return data;
        }

        private static List<PortalInfo> ParseMapNode(WZObject map) {
            List<PortalInfo> portals = new List<PortalInfo>();
            int id = int.Parse(Path.GetFileNameWithoutExtension(map.Name));

            foreach (var portal in map.ResolvePath("portal") ?? Enumerable.Empty<WZObject>()) {
                int dst = portal.ResolvePath("tm").ValueOrDefault(999999999);
                if (dst == 999999999 || dst == -1 || dst == id) continue;

                portals.Add(new PortalInfo {
                    DstId = dst,
                    X = (short) portal.ResolvePath("x").ValueOrDie<int>(),
                    Y = (short) portal.ResolvePath("y").ValueOrDie<int>(),
                    Cmd = portal.ResolvePath("pn").ValueOrDie<string>()
                });
            }
            return portals;
        }
    }
}
