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
        private static readonly WZFile MapWz = new WZFile(Program.ROOT + "Map.wz", WZVariant.Classic, true, WZReadSelection.EagerParseStrings);

        public static Dictionary<int, Map> ParseMap() {
            Console.WriteLine("Parsing portals from Map.wz...");

            Dictionary<int, Map> data = new Dictionary<int, Map>();
            List<string> regions = new List<string>();
            foreach (var region in MapWz.ResolvePath("Map")) {
                if (Regex.Match(region.Name, "Map[0-9]").Success) {
                    regions.Add(region.Name);
                }
            }
            regions.Sort();
            foreach (string region in regions) {
                Console.WriteLine($"Parsing '{region}'...");

                foreach (var map in MapWz.ResolvePath("Map/" + region)) {
                    int id = int.Parse(Path.GetFileNameWithoutExtension(map.Name));
                    Dictionary<int, PortalInfo> portals = ParseMapNode(map);
                    if (portals.Count == 0)
                        continue; // No portals
                    data[id] = new Map(id, portals);
                }
            }
                
            return data;
        }

        private static Dictionary<int, PortalInfo> ParseMapNode(WZObject map) {
            Dictionary<int, PortalInfo> portals = new Dictionary<int, PortalInfo>();
            int id = int.Parse(Path.GetFileNameWithoutExtension(map.Name));

            foreach (var portal in map.ResolvePath("portal") ?? Enumerable.Empty<WZObject>()) {
                int dst = portal.ResolvePath("tm").ValueOrDefault(999999999);
                if (dst == 999999999 || dst == -1 || dst == id) continue;

                portals[dst] = new PortalInfo {
                    X = (short) portal.ResolvePath("x").ValueOrDie<int>(),
                    Y = (short) portal.ResolvePath("y").ValueOrDie<int>(),
                    Name = portal.ResolvePath("pn").ValueOrDie<string>()
                };
            }
            return portals;
        }
    }
}
