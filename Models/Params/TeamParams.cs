using System;
using Newtonsoft.Json;

namespace pesisBackend
{
    class TeamParams : ITeamParams
    {
        public TeamParams(
            int kaudetAlku,
            int kaudetLoppu, 
            string sarja,
            string sarjajako,
            Boolean vuosittain,
            string koti,
            string tulos,
            string vastustaja,
            string joukkue
        ){
            this.kaudetAlku = kaudetAlku;
            this.kaudetLoppu = kaudetLoppu;
            this.sarja = sarja;
            this.sarjajako = sarjajako;
            this.vuosittain = vuosittain;
            this.koti = koti;
            this.tulos = tulos;
            this.vastustaja = vastustaja;
            this.joukkue = joukkue;
        }

        public int kaudetAlku {get; set;} = 1990; 
        public int kaudetLoppu {get; set;} = 2222;
        public string sarja {get; set;} = "Miesten Superpesis";
        public string sarjajako {get; set;} = "";
        public bool vuosittain {get; set;} = false;
        public string koti {get; set;} = "";
        public string tulos {get; set;} = "";
        public string vastustaja {get; set;} = "";
        public string joukkue {get; set;} = "";

    }
}