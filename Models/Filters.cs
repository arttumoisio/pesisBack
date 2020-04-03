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
    /// <summary>
    /// // TODO
    /// </summary>
    class Filters
    {

        public string joukkue(string joukkue = ""){
            string joukkueFilter = "";
            if (!String.IsNullOrEmpty(joukkue)){
                joukkueFilter = "AND j.joukkue = @joukkue";

            }
            return joukkueFilter;
        }
        public Modifier koti(string kotiParam = "")
        {
            Modifier koti = new Modifier();
            if (kotiParam == "Koti"){
                koti.Filter = $"AND j.joukkue_id=o.koti_id";
            } else if (kotiParam == "Vieras") {
                koti.Filter = $"AND j.joukkue_id=o.vieras_id";
            } else if (kotiParam == "Eritelty") {
                koti.Erittely = @"
                CASE ot.joukkue_id
                    WHEN o.koti_id THEN 'Koti'
	                WHEN o.vieras_id THEN 'Vieras'
                END `Koti/Vieras`,";
                koti.Group = $", `Koti/Vieras`";
            }
            return koti;
        }
        public Modifier koti(string kotiParam = "", bool j = true)
        {
            Modifier koti = new Modifier();
            if (kotiParam == "Koti"){
                koti.Filter = $"AND koti='koti'";
            } else if (kotiParam == "Vieras") {
                koti.Filter = $"AND koti='vieras'";
            } else if (kotiParam == "Eritelty") {
                koti.Erittely = @"
                koti `Koti/Vieras`,";
                koti.Group = $", `koti`";
            }
            return koti;
        }
        public Modifier tulos(string tulos = "")
        {
            string pisteetGroup = "";
            string pisteetFilter = $@"
                    AND (
                    CASE ot.joukkue_id
                        WHEN o.vieras_id THEN vp+0
                        WHEN o.koti_id THEN kp+0
                    END
                    ) ";
            string pisteetErittely = "";
            switch (tulos)
            {
                case "Voitto":
                    pisteetFilter += ">= 2" ;
                    break;
                case "Tappio":
                    pisteetFilter += "<= 1" ;
                    break;
                case "3p Voitto":
                    pisteetFilter += "= 3" ;
                    break;
                case "2p Voitto":
                    pisteetFilter += "= 2" ;
                    break;
                case "1p Tappio":
                    pisteetFilter += "= 1" ;
                    break;
                case "0p Tappio":
                    pisteetFilter += "= 0" ;
                    break;
                case "Eritelty":
                    pisteetFilter ="";
                    pisteetErittely = @"
                    CASE ot.joukkue_id
                        WHEN o.vieras_id THEN vp || 'P'
                        WHEN o.koti_id THEN kp || 'P'
                    END `Voitto/Tappio`,";
                    pisteetGroup =", `Voitto/Tappio`";
                    break;
                default:
                    pisteetFilter = "";
                    break;
            }
            Modifier pisteetMod = new Modifier();
            pisteetMod.Group = pisteetGroup;
            pisteetMod.Filter = pisteetFilter;
            pisteetMod.Erittely = pisteetErittely;
            return pisteetMod;
        }
        public Modifier tulos(string tulos = "", bool j = true)
        {
            string pisteetGroup = "";
            string pisteetFilter = $@"
                    AND p ";
            string pisteetErittely = "";
            switch (tulos)
            {
                case "Voitto":
                    pisteetFilter += ">= 2" ;
                    break;
                case "Tappio":
                    pisteetFilter += "<= 1" ;
                    break;
                case "3p Voitto":
                    pisteetFilter += "= 3" ;
                    break;
                case "2p Voitto":
                    pisteetFilter += "= 2" ;
                    break;
                case "1p Tappio":
                    pisteetFilter += "= 1" ;
                    break;
                case "0p Tappio":
                    pisteetFilter += "= 0" ;
                    break;
                case "Eritelty":
                    pisteetFilter ="";
                    pisteetErittely = @"
                    p || 'P' `Voitto/Tappio`,";
                    pisteetGroup =", `Voitto/Tappio`";
                    break;
                default:
                    pisteetFilter = "";
                    break;
            }            
            Modifier pisteetMod = new Modifier();
            pisteetMod.Group = pisteetGroup;
            pisteetMod.Filter = pisteetFilter;
            pisteetMod.Erittely = pisteetErittely;
            return pisteetMod;
        }
        public Modifier vastustaja(string vastustaja = "")
        {
            string vastustajaGroup = "";
            string vastustajaFilter = "";
            string vastustajaErittely = "";

            if (vastustaja == "Eritelty") {
                vastustajaGroup = $", `Vastustaja`";
            } else if (!String.IsNullOrEmpty(vastustaja)){
                vastustajaFilter = $@"
                    AND
                    (CASE ot.joukkue_id
                        WHEN o.vieras_id THEN kotijoukkue
                        WHEN o.koti_id THEN vierasjoukkue
                    END) = @vastustaja
                    ";
            }
            if (!String.IsNullOrEmpty(vastustaja)){
                vastustajaErittely = @"
                    CASE ot.joukkue_id
                        
                        WHEN vieras_id THEN kotijoukkue
                        WHEN koti_id THEN vierasjoukkue
                        ELSE ot.joukkue_id ||' '|| koti_id ||' '|| vieras_id
                    END `Vastustaja`,";
            }
            Modifier vastustajaMod = new Modifier();
            vastustajaMod.Group = vastustajaGroup;
            vastustajaMod.Filter = vastustajaFilter;
            vastustajaMod.Erittely = vastustajaErittely;
            return vastustajaMod;
        }
        public Modifier vastustaja(string vastustaja = "", bool j = true)
        {
            string vastustajaGroup = "";
            string vastustajaFilter = "";
            string vastustajaErittely = "";

            if (vastustaja == "Eritelty") {
                vastustajaGroup = $", vastustaja_id";
            } else if (!String.IsNullOrEmpty(vastustaja)){
                vastustajaFilter = $@"
                    AND vastustaja = @vastustaja
                    ";
            }
            if (!String.IsNullOrEmpty(vastustaja)){
                vastustajaErittely = @"
                    vastustaja `Vastustaja`,";
            }
            Modifier vastustajaMod = new Modifier();
            vastustajaMod.Group = vastustajaGroup;
            vastustajaMod.Filter = vastustajaFilter;
            vastustajaMod.Erittely = vastustajaErittely;
            return vastustajaMod;
        }
        public Modifier kotiJoukkue(string kotijoukkue){
            Modifier koti = new Modifier();
            if (!String.IsNullOrEmpty(kotijoukkue))
            {
                koti.Erittely = "kotijoukkue Kotijoukkue,";
                if (kotijoukkue == "Eritelty")
                {
                    koti.Group = ", kotijoukkue";
                } else {
                    koti.Filter = "AND kotijoukkue = @kotijoukkue";
                }
            }
            return koti;
        }
        public Modifier vierasJoukkue(string vierasjoukkue){
            Modifier vieras = new Modifier();
            if (!String.IsNullOrEmpty(vierasjoukkue))
            {
                vieras.Erittely = "vierasjoukkue Vierasjoukkue,";
                if (vierasjoukkue == "Eritelty")
                {
                    vieras.Group = ", vierasjoukkue";
                } else {
                    vieras.Filter = "AND vierasjoukkue = @vierasjoukkue";
                }
            }

            return vieras;
        }
        public Modifier lukkari(string lukkari){
            Modifier lukkariMod = new Modifier();
            if (!String.IsNullOrEmpty(lukkari))
            {
                lukkariMod.Select = @"
INNER JOIN ottelu_tilasto ot ON ot.ottelu_id = o.ottelu_id AND upp = 'L'
INNER JOIN pelaaja p ON p.pelaaja_id = ot.pelaaja_id
                ";
                lukkariMod.Group = ", Lukkari";
                lukkariMod.Filter = @"";
                lukkariMod.Erittely = @"
                p.nimi Lukkari,
                SUM(kp > vp AND ot.joukkue_id = o.koti_id) + SUM(kp < vp AND ot.joukkue_id = o.vieras_id) `Lukkarin voitot`,
                SUM(kp < vp AND ot.joukkue_id = o.koti_id) + SUM(kp > vp AND ot.joukkue_id = o.vieras_id) `Lukkarin tappiot`,
                ROUND(100.0*( SUM(kp > vp AND ot.joukkue_id = o.koti_id) + SUM(kp < vp AND ot.joukkue_id = o.vieras_id) )/COUNT(DISTINCT o.ottelu_id),2) `Lukkarin voitto-%`,
                SUM( ot.joukkue_id = o.koti_id) `Ottelut Lukkarin Kotona`,
                SUM( ot.joukkue_id = o.vieras_id) `Ottelut Lukkarin vieraissa`,
                ROUND( 1.0*SUM( 
                    CASE 
                        WHEN ot.joukkue_id = o.koti_id THEN vttotv
                        WHEN ot.joukkue_id = o.vieras_id THEN vttotk
                    END
                ) / COUNT( DISTINCT o.ottelu_id ),2) `Lukkarin VT / ott`,
                ROUND( 1.0*SUM( 
                    CASE 
                        WHEN ot.joukkue_id = o.koti_id THEN vtv
                        WHEN ot.joukkue_id = o.vieras_id THEN vtk
                    END 
                ) / COUNT( DISTINCT o.ottelu_id ),2) `Lukkarin VT juoksut / ott`,
                ";
                if (lukkari != "Eritelty"){
                    lukkariMod.Filter += " AND Lukkari = @lukkari ";
                }
            }

            return lukkariMod;
        }
        public string stpt(string STPT){
            string Filter = "";
            if(!String.IsNullOrEmpty(STPT)){
                if(STPT == "PT"){ Filter = " AND tuomari = pelituomari "; }
                if(STPT == "ST"){ Filter = " AND tuomari = syottotuomari "; }
            }
            return Filter;
        }
        public Modifier vuosittain(bool vuosittain){
            Modifier vuosittainM = new Modifier();
            vuosittainM.Group = "";
            vuosittainM.Erittely = "";
            vuosittainM.Erittely2 = ", COUNT(DISTINCT kausi) Kaudet";
            if (vuosittain) {
                vuosittainM.Group = ", kausi";
                vuosittainM.Erittely = "kausi Kausi,";
                vuosittainM.Erittely2 = "";
            }
            return vuosittainM;
        }
        public Modifier sarjajako(string sarjajako){
            Modifier sarjaj = new Modifier();
            if (sarjajako == "Eritelty") {
                sarjaj.Erittely = "sarjajako Sarjavaihe,";
                sarjaj.Filter = "";
            } else if (!String.IsNullOrEmpty(sarjajako)) {
                sarjaj.Group = ", sarjajako";
                sarjaj.Filter = "AND sarjajako = @sarjajako";
            }
            return sarjaj;
        }
        public Modifier sarja(string sarja){
            Modifier sarjaMod = new Modifier();
            sarjaMod.Filter = "AND sarja = @sarja";

            if (String.IsNullOrEmpty(sarja)) {
                sarjaMod.Group = "";
                sarjaMod.Filter = "";
                sarjaMod.Erittely = "";
            }
            if(sarja == "Eritelty"){
                sarjaMod.Group = ", sarja";
                sarjaMod.Filter = "";
                sarjaMod.Erittely = "sarja Sarja,";
            }

            return sarjaMod;
        }
    }
}
