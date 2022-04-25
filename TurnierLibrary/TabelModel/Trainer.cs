using System;
using System.Collections.Generic;
using System.Text;

namespace TurnierLibrary
{
    public class Trainer
    {
        public int TrainerId { get; set; }
        public string Vorname { get; set; }
        public string Nachname { get; set; }
        public DateTime? Amtsantritt { get; set; }
        public int? Mannschaft { get; set; }
    }
}

