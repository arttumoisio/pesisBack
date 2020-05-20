using System;
using Newtonsoft.Json;

namespace pesisBackend
{
    class TuomariParams : ITuomariParams
    {

        public TuomariParams(          
            int kaudetAlku,
            int kaudetLoppu, 
            string sarja,
            string sarjajako,
            Boolean vuosittain,
            string kotijoukkue,
            string vierasjoukkue,
            string lukkari,
            string STPT
        ){
            this.kaudetAlku = kaudetAlku;
            this.kaudetLoppu = kaudetLoppu;
            this.sarja = sarja;
            this.sarjajako = sarjajako;
            this.vuosittain = vuosittain;
            this.kotijoukkue = kotijoukkue;
            this.vierasjoukkue = vierasjoukkue;
            this.lukkari = lukkari;
            this.STPT = STPT;

        }
        public int kaudetAlku {get; set;} = 1990; 
        public int kaudetLoppu {get; set;} = 2222;
        public string sarja {get; set;} = "Miesten Superpesis";
        public string sarjajako  {get; set;} = "Runkosarja";
        public bool vuosittain {get; set;} = false;
        public string kotijoukkue {get; set;} = "";
        public string vierasjoukkue {get; set;} = "";
        public string lukkari {get; set;} = "";
        public string STPT {get; set;} = "";

    }
}