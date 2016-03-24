namespace MapleWzParser.Types {
    public sealed class PortalInfo {
        public short X { get; set; }
        public short Y { get; set; }
        public string Name { get; set; }

        public override string ToString() {
            return X + "," + Y + ":" + Name;
        }
    }
}
