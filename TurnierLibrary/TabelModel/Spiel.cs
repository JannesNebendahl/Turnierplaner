using System;

namespace DemoLibary
{
    public class Spiel
    {
        public int? Id { get; set; }
        public int? Spieltag { get; set; }
        public int? Zuschaueranzahl { get; set; }
        public DateTime? Datum { get; set; }
        public int? Heimmanschaft { get; set; }
        public int? Auswaertsmannschaft { get; set; }
        public override string ToString()
        {
            return Spieltag.ToString();
        }
    }
}
