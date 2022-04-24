using System;
using System.Collections.Generic;
using System.Text;

namespace TurnierLibrary
{
    public class Spieler
    {
        public int? Id { get; set; }
        public string Vorname { get; set; }
        public string Nachname { get; set; }
        public int? Trikotnummer { get; set; }
        public int? MannschaftsId { get; set; }
        public string Name => Vorname + " " + Nachname;
        public override string ToString()
        {
            return Name;
        }
    }
}
