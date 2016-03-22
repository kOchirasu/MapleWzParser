using System;
using System.Collections.Generic;
using System.Linq;
using MapleWzParser.Types;

namespace MapleWzParser {
    public static class Pathfinder {
        public static void Compute(Dictionary<int, Map> map) {
            Dictionary<int, HashSet<int>> regions = ClusterRegions(map);
            EnumeratePaths(map, regions);
        }

        public static Dictionary<int, HashSet<int>> ClusterRegions(Dictionary<int, Map> map) {
            Dictionary<int, HashSet<int>> regions = new Dictionary<int, HashSet<int>>();
            HashSet<int> allMaps = new HashSet<int>(map.Keys);
            HashSet<int> remMaps = new HashSet<int>();

            int totalRegions = 0;
            int regionCount = 1;
            int removed = 0;

            while (allMaps.Count > 0) {
                int test = allMaps.First();
                // No portals, nothing to check
                if (map[test].Portals.Count == 0) {
                    allMaps.Remove(test);
                    continue;
                }
                // Single portal
                if (map[test].Portals.Count == 1) {
                    // If portal is 1-way ignore
                    int m = map[test].Portals.Keys.First();
                    if (!map[m].Portals.ContainsKey(test)) {
                        allMaps.Remove(test);
                        remMaps.Add(test);
                        removed++;
                        continue;
                    }
                }

                HashSet<int> ret = Explore(test, map, allMaps, remMaps);
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

        public static void EnumeratePaths(Dictionary<int, Map> map, Dictionary<int, HashSet<int>> regions) {
            foreach (KeyValuePair<int, HashSet<int>> entry in regions) {
                Console.Write(entry.Key + ", ");

                // No Stops
                foreach (int src in entry.Value) {
                    foreach (int dst in entry.Value) {
                        if (src == dst) continue;
                        // Adjacent
                        if (map[src].Portals.ContainsKey(dst)) {
                            map[src].Choice[dst] = dst;
                            map[src].Weight[dst] = 1;
                        }
                    }
                }

                // Try Stops
                foreach (int stp in entry.Value) {
                    foreach (int src in entry.Value) {
                        if (src == stp)
                            continue;
                        foreach (int dst in entry.Value) {
                            // Path is done
                            if (dst == stp || src == dst) continue;

                            // Path is impossible
                            if (!map[stp].Weight.ContainsKey(dst) || map[stp].Weight[dst] == 0) continue;
                            if (!map[src].Weight.ContainsKey(stp) || map[src].Weight[stp] == 0) continue;

                            // Path is possible
                            int weight = map[src].Weight[stp] + map[stp].Weight[dst];
                            if (!map[src].Weight.ContainsKey(dst) || weight < map[src].Weight[dst]) {
                                map[src].Choice[dst] = map[src].Choice[stp];
                                map[src].Weight[dst] = weight;
                            }
                        }
                    }
                }
            }
        }

        public static void CleanMapData(Dictionary<int, List<PortalInfo>> mapData) {
            Console.WriteLine("Removing useless maps...");
            HashSet<int> allMaps = new HashSet<int>(mapData.Keys);
            foreach (List<PortalInfo> map in mapData.Values) {
                foreach (var p in map) {
                    allMaps.Remove(p.DstId);
                }
            }
            
            foreach (int id in allMaps) {
                mapData.Remove(id);
            }
        }

        public static HashSet<int> Explore(int id, IReadOnlyDictionary<int, Map> index, ICollection<int> universe, ICollection<int> ignored) {
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
