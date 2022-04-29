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
        public int? Typ { get; set; }
        public int? SpielID { get; set; }
        public string? MannschaftString { get; set; }
        public string? SpielerString { get; set; }
        public string? Vorname { get; set; }
        public string? Nachname { get; set; }
        public int? Toranzahl { get; set; }
        public int? Platzierung { get; set; }
        public double? avgSpiel { get; set; }
        public string? TypString { get; set; }
    }
}
