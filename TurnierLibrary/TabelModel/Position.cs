using System;
using System.Collections.Generic;
using System.Text;

namespace TurnierLibrary
{
    public class Position
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Kuerzel { get; set; }
        public Boolean Check_Status { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}
