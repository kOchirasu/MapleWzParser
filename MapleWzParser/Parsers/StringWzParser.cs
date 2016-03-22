using System;
using System.Collections.Generic;
using reWZ;

namespace MapleWzParser.Parsers {
    public static class StringWzParser {
        private static readonly WZFile StringWz = new WZFile(Program.ROOT + "String.wz", WZVariant.Classic, true, WZReadSelection.EagerParseStrings);

        public static Dictionary<int, string> ParseEquip() {
            Console.WriteLine("Parsing 'Eqp.img/Eqp' from String.wz...");

            Dictionary<int, string> nameMap = new Dictionary<int, string>();
            foreach (var type in StringWz.ResolvePath("Eqp.img/Eqp")) {
                foreach (var equip in type) {
                    int id = int.Parse(equip.Name);
                    nameMap[id] = equip.ResolvePath("name").ValueOrDefault("Unknown");
                }
            }
            return nameMap;
        }

        public static Dictionary<int, string> ParseUse() {
            return ParseItems("Consume.img");
        }

        public static Dictionary<int, string> ParseSetup() {
            return ParseItems("Ins.img");
        }

        public static Dictionary<int, string> ParseEtc() {
            return ParseItems("Etc.img/Etc");
        }

        public static Dictionary<int, string> ParseCash() {
            return ParseItems("Cash.img");
        }

        public static Dictionary<int, string[]> ParseMap() {
            Console.WriteLine("Parsing 'Map.img' from String.wz...");

            Dictionary<int, string[]> nameMap = new Dictionary<int, string[]>();
            foreach (var region in StringWz.ResolvePath("Map.img")) {
                foreach (var map in region) {
                    int id = int.Parse(map.Name);
                    string mapName = map.ResolvePath("mapName")?.ValueOrDefault("Unknown").Trim().TrimEnd('\n', '\r') ?? "Unknown";
                    string streetName = map.ResolvePath("streetName")?.ValueOrDefault("Unknown").Trim() ?? "Unknown";
                    nameMap[id] = new[] { streetName, mapName };
                }
            }
            return nameMap;
        } 

        private static Dictionary<int, string> ParseItems(string path) {
            Console.WriteLine($"Parsing '{path}' from String.wz...");

            Dictionary<int, string> nameMap = new Dictionary<int, string>();
            foreach (var item in StringWz.ResolvePath(path)) {
                int id = int.Parse(item.Name);
                nameMap[id] = item.ResolvePath("name")?.ValueOrDefault("Unknown") ?? "Unknown";
            }
            return nameMap;
        }
    }
}
