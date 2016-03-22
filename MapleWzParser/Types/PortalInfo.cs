namespace MapleWzParser.Types {
    public sealed class PortalInfo {
        public int DstId { get; set; }
        public short X { get; set; }
        public short Y { get; set; }
        public string Cmd { get; set; }

        public override string ToString() {
            return DstId + ":" + X + "," + Y + ":" + Cmd;
        }
    }
}
