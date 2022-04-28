using System;
using System.Collections.Generic;
using System.Text;

namespace TurnierLibrary
{
    public class Tor
    {
        public int? TorId { get; set; }
        public int? Zeitstempel { get; set; }
        public int? Spieler { get; set; }
        public int? Mannschaft { get; set; }
        public string? Typ { get; set; }
        public int? SpielID { get; set; }
        public string? MannschaftString { get; set; }
        public string? SpielerString { get; set; }
    }
}
