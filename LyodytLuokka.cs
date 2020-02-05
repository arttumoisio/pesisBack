using System;

namespace pesisBackend
{
    public class LyodytLuokka
    {
        public string pelaaja {get; set; }
        public int lyodyt {get; set; }
        public int kunnarit {get; set; }
        public int yritykset {get; set; }

        public int ottelut {get; set; }
    }

        public class Lyhyt
    {
        public string pelaaja {get; set; }
        public int lyodyt {get; set; }
        public int kunnarit {get; set; }

        public int yht => (int)lyodyt + (int)kunnarit;


        
    }
}
