using System;
using System.Collections.Generic;
using System.Linq;
using MapleWzParser.Parsers;
using MapleWzParser.Types;

namespace MapleWzParser {
    public static class Pathfinder {
        public static Dictionary<int, Map> Compute() {
            Console.SetBufferSize(150, 5000);
            Dictionary<int, Map> mapData = MapWzParser.ParseMap();
            CleanMapData(mapData);
            Dictionary<int, HashSet<int>> regions = ClusterRegions(mapData);
            EnumeratePaths(mapData, regions);

            return mapData;
        }

        public static void Test(Dictionary<int, Map> mapData) {
            Console.WriteLine("Pathfinder Test started, please enter a src and dst map id \"src dst\"");
            while (true) {
                string enter = Console.ReadLine();
                if (enter == null || enter.Equals("exit")) break;
                string[] data = enter.Split(' ');
                if (data.Length != 2) continue;
                int src, dst;
                if (!int.TryParse(data[0], out src)) continue;
                if (!int.TryParse(data[1], out dst)) continue;
                if (!mapData.ContainsKey(src)) {
                    Console.WriteLine("Invalid map " + src);
                    continue;
                }
                if (!mapData.ContainsKey(dst)) {
                    Console.WriteLine("Invalid map " + dst);
                    continue;
                }

                // Already on map
                if (src == dst) {
                    Console.WriteLine(src);
                    Console.WriteLine("Arrived in 0 moves...");
                    continue;
                }

                // Can arrive in 1 move
                if (mapData[src].Portals.ContainsKey(dst)) {
                    Console.WriteLine(src + " -> " + dst);
                    Console.WriteLine("Arrived in 1 move...");
                    continue;
                }

                // Can't reach dst
                if (!mapData[src].Choice.ContainsKey(dst)) {
                    Console.WriteLine("From " + src + " to " + dst + " is unreachable...");
                    continue;
                }

                Console.Write(src);
                int cur = src;
                int count = 0;
                while (cur != dst) {
                    cur = mapData[cur].Choice[dst];
                    Console.Write(" -> " + cur);
                    count++;
                }
                Console.WriteLine("\nArrived in " + count + " move(s)...");
            }
        }

        private static void CleanMapData(Dictionary<int, Map> mapData) {
            Console.WriteLine("Removing useless maps...");

            HashSet<int> allMaps = new HashSet<int>(mapData.Keys);
            foreach (var node in mapData.Values) {
                foreach (int dst in node.Portals.Keys) {
                    allMaps.Remove(dst);
                }
            }

            // Remove maps that are never a destination?
            foreach (int id in allMaps) {
                mapData.Remove(id);
            }
            // Remove portal destinations that are now invalid
            foreach (var node in mapData.Values) {
                node.Portals = node.Portals.Where(pair => mapData.ContainsKey(pair.Key)).
                    ToDictionary(pair => pair.Key, pair => pair.Value);
            }
        }

        private static Dictionary<int, HashSet<int>> ClusterRegions(Dictionary<int, Map> mapData) {
            Dictionary<int, HashSet<int>> regions = new Dictionary<int, HashSet<int>>();
            HashSet<int> allMaps = new HashSet<int>(mapData.Keys);
            HashSet<int> remMaps = new HashSet<int>();

            int totalRegions = 0;
            int regionCount = 1;
            int removed = 0;

            while (allMaps.Count > 0) {
                int test = allMaps.First();
                // No portals, nothing to check
                if (mapData[test].Portals.Count == 0) {
                    allMaps.Remove(test);
                    continue;
                }
                // Single portal
                if (mapData[test].Portals.Count == 1) {
                    // If portal is 1-way ignore
                    int m = mapData[test].Portals.Keys.First();
                    if (!mapData[m].Portals.ContainsKey(test)) {
                        allMaps.Remove(test);
                        remMaps.Add(test);
                        removed++;
                        continue;
                    }
                }

                HashSet<int> ret = Explore(test, mapData, allMaps, remMaps);
                if (ret.Count == 1) continue; // Ignore this for now (Maps that have multiple choices, but each of those choices have no choice
                regions[regionCount] = ret;

                Console.WriteLine("==================================================");
                Console.WriteLine(string.Join(", ", ret.ToArray()));
                regionCount++;
                totalRegions += ret.Count;
            }
            Console.WriteLine("==================================================");
            Console.WriteLine("Total Maps: " + totalRegions);
            Console.WriteLine("Region Count: " + regionCount);
            Console.WriteLine("Removed: " + removed);
            Console.WriteLine("==================================================");

            return regions;
        }

        private static void EnumeratePaths(Dictionary<int, Map> mapData, Dictionary<int, HashSet<int>> regions) {
            foreach (KeyValuePair<int, HashSet<int>> entry in regions) {
                // No Stops
                foreach (int src in entry.Value) {
                    foreach (int dst in entry.Value) {
                        if (src == dst) continue;
                        // Adjacent
                        if (mapData[src].Portals.ContainsKey(dst)) {
                            mapData[src].Choice[dst] = dst;
                            mapData[src].Weight[dst] = 1;
                        }
                    }
                }

                // Try Stops
                foreach (int stp in entry.Value) {
                    foreach (int src in entry.Value) {
                        if (src == stp) continue;
                        foreach (int dst in entry.Value) {
                            // Path is done
                            if (dst == stp || src == dst) continue;

                            // Path is impossible
                            if (!mapData[stp].Weight.ContainsKey(dst) || mapData[stp].Weight[dst] == 0) continue;
                            if (!mapData[src].Weight.ContainsKey(stp) || mapData[src].Weight[stp] == 0) continue;

                            // Path is possible
                            int weight = mapData[src].Weight[stp] + mapData[stp].Weight[dst];
                            if (!mapData[src].Weight.ContainsKey(dst) || weight < mapData[src].Weight[dst]) {
                                mapData[src].Choice[dst] = mapData[src].Choice[stp];
                                mapData[src].Weight[dst] = weight;
                            }
                        }
                    }
                }
            }
        }

        private static HashSet<int> Explore(int id, IReadOnlyDictionary<int, Map> index, ICollection<int> universe, ICollection<int> ignored) {
            HashSet<int> results = new HashSet<int>();
            Queue<int> queue = new Queue<int>();
            queue.Enqueue(id);
            while (queue.Count > 0) {
                int top = queue.Dequeue();
                if (results.Contains(top)) continue; // Already processed map
                universe.Remove(top);
                results.Add(top);
                foreach (int map in index[top].Portals.Keys) {
                    if (!index[map].Portals.ContainsKey(top)) {
                        // If ignored, add it
                        if (ignored.Contains(map)) {
                            queue.Enqueue(map);
                            ignored.Remove(map);
                        }
                        if (universe.Contains(map)) {
                            queue.Enqueue(map);
                            universe.Remove(map);
                        }
                        continue; // One-way portals, Ignore for now
                    }
                    queue.Enqueue(map);
                }
            }
            return results;
        }
    }
}
