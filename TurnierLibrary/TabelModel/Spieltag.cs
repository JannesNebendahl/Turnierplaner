using System;
using System.Collections.Generic;
using System.Text;

namespace TurnierLibrary.TabelModel
{
    public class Spieltag
    {
        public int Tag { get; set; }
        public Boolean Check_Status { get; set; }
        public override string ToString()
        {
            return Tag.ToString() + ". Spieltag";
        }
        public string Name => Tag.ToString() + ". Spieltag";
    }
}
