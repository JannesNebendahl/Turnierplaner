using System;

namespace TurnierLibrary
{
    public class Spiel
    {
        public int? Id { get; set; }
        public int? Spieltag { get; set; }
        public int? Zuschauerzahl { get; set; }
        public DateTime Datum { get; set; }
        public int? HeimmannschaftsId { get; set; }
        public int? AuswaertsmannschaftsId { get; set; }
        public string? Heim { get; set; }
        public string? Gast { get; set; }
        public override string ToString()
        {
            return Spieltag.ToString();
        }
    }
}
