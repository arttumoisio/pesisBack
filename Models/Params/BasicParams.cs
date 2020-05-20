using System;
using Newtonsoft.Json;

namespace pesisBackend
{
    class BasicParams : IBasicParams
    {
        public BasicParams(
            int kaudetAlku = 1950,
            int kaudetLoppu = 2222, 
            string sarja = "Miesten Superpesis",
            string sarjajako = "Runkosarja",
            Boolean vuosittain = false
        ){
            this.kaudetAlku = kaudetAlku;
            this.kaudetLoppu = kaudetLoppu;
            this.sarja = sarja;
            this.sarjajako = sarjajako;
            this.vuosittain = vuosittain;
        }

        public int kaudetAlku {get; set;}
        public int kaudetLoppu {get; set;}
        public string sarja {get; set;}
        public string sarjajako {get; set;}
        public bool vuosittain {get; set;}

    }
}