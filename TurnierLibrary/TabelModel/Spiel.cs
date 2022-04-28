using System;

namespace TurnierLibrary
{
    public class Spiel
    {
        public int? Id { get; set; }
        public int? Spieltag { get; set; }
        public int? Zuschauerzahl { get; set; }
        public DateTime? Datum { get; set; }
        public int? HeimmannschaftsID { get; set; }
        public int? AuswaertsmannschaftsID { get; set; }
        public string? Heim { get; set; }
        public string? Gast { get; set; }
        public override string ToString()
        {
            return Spieltag.ToString();
        }
    }
}
