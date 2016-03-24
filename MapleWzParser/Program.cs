using System;
using System.Collections.Generic;
using MapleWzParser.Parsers;
using MapleWzParser.Types;

namespace MapleWzParser {
    public class Program {
        public const string ROOT = @"C:\Nexon\MapleStory\";
        public const string OUTPUT = @"C:\Nexon\MapleStory\";

        public static void Main(string[] args) {
            /*Serializer.Save(OUTPUT + "equip.item", StringWzParser.ParseEquip());
            Serializer.Save(OUTPUT + "use.item", StringWzParser.ParseUse());
            Serializer.Save(OUTPUT + "setup.item", StringWzParser.ParseSetup());
            Serializer.Save(OUTPUT + "etc.item", StringWzParser.ParseEtc());
            Serializer.Save(OUTPUT + "cash.item", StringWzParser.ParseCash());

            Serializer.Save(OUTPUT + "name.map", StringWzParser.ParseMap());*/

            //Serializer.Save(OUTPUT + "node.map", Pathfinder.Compute());

            Console.WriteLine("Complete!");
            Console.WriteLine("Press ENTER to continue.");
            Console.Read();
        }
    }
}
