using System;

namespace DemoLibary
{
    public class Mannschaft
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Kuerzel { get; set; }
        public DateTime? Entstehungsjahr { get; set; }
        public int? Kapitan { get; set; }
        public int? Punktestand { get; set; }
        public Boolean Check_Status { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}
