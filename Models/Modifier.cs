using System;
using Newtonsoft.Json;

namespace pesisBackend
{
    class Modifier
    {
        public Modifier()
        {
            Group = "";
            Filter = "";
            Erittely = "";
            Erittely2 = "";
            Select = "";
        }
        public string Group {get; set;}
        public string Filter {get; set;}
        public string Erittely {get; set;}
        public string Erittely2 {get; set;}
        public string Select {get; set;}
    }
}