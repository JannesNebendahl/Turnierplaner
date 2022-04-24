using System;
using System.Collections.Generic;
using System.Text;

namespace TurnierLibrary.TabelModel
{
    public class Schiedsrichter
    {
        public String Vorname { get; set; }
        public String Nachname { get; set; }
        public string Name => Vorname + " " + Nachname;
        public override string ToString()
        {
            return Name;
        }
    }
}
