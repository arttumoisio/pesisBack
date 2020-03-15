using System;
using Newtonsoft.Json;

namespace pesisBackend
{
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
        public Tuple<string, string, string> koti(string koti = "")
        {
            string kotiGroup = "";
            string kotiFilter = "";
            string kotiErittely = "";
            if (koti == "Koti"){
                kotiFilter = $"AND j.joukkue_id=o.koti_id";
            } else if (koti == "Vieras") {
                kotiFilter = $"AND j.joukkue_id=o.vieras_id";
            } else if (koti == "Eritelty") {
                kotiErittely = @"
                CASE ot.joukkue_id
                WHEN o.koti_id THEN 'Koti'
	            WHEN o.vieras_id THEN 'Vieras'
                END `Koti/Vieras`,";
                kotiGroup = $", `Koti/Vieras`";
            }
            return new Tuple<string, string, string>(kotiGroup,kotiFilter,kotiErittely);
        }
        public Tuple<string, string, string> koti(string koti = "", bool j = true)
        {
            string kotiGroup = "";
            string kotiFilter = "";
            string kotiErittely = "";
            if (koti == "Koti"){
                kotiFilter = $"AND koti='koti'";
            } else if (koti == "Vieras") {
                kotiFilter = $"AND koti='vieras'";
            } else if (koti == "Eritelty") {
                kotiErittely = @"
                koti `Koti/Vieras`,";
                kotiGroup = $", `koti`";
            }
            return new Tuple<string, string, string>(kotiGroup,kotiFilter,kotiErittely);
        }
        public Tuple<string, string, string> tulos(string tulos = "")
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
            return new Tuple<string, string, string>(pisteetGroup,pisteetFilter,pisteetErittely);
        }
        public Tuple<string, string, string> tulos(string tulos = "", bool j = true)
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
            return new Tuple<string, string, string>(pisteetGroup,pisteetFilter,pisteetErittely);
        }
        public Tuple<string, string, string> vastustaja(string vastustaja = "")
        {
            string vastustajaGroup = "";
            string vastustajaFilter = "";
            string vastustajaErittely = "";
            Console.WriteLine(vastustaja);

            Console.WriteLine($"Tyhjä? {vastustajaErittely}");

            if (vastustaja == "Eritelty") {
                Console.WriteLine($"Eritelty {vastustaja}");
                vastustajaGroup = $", `Vastustaja`";
            } else if (!String.IsNullOrEmpty(vastustaja)){
                Console.WriteLine($"Epätyhjä {vastustaja}");
                vastustajaFilter = $@"
                    AND
                    (CASE ot.joukkue_id
                        WHEN o.vieras_id THEN kotijoukkue
                        WHEN o.koti_id THEN vierasjoukkue
                    END) = @vastustaja
                    ";
            }
            if (!String.IsNullOrEmpty(vastustaja))
                vastustajaErittely = @"
                    CASE ot.joukkue_id
                        
                        WHEN vieras_id THEN kotijoukkue
                        WHEN koti_id THEN vierasjoukkue
                        ELSE ot.joukkue_id ||' '|| koti_id ||' '|| vieras_id
                    END `Vastustaja`,";
            Console.WriteLine($"Tyhjä? {vastustajaErittely}");
            return new Tuple<string, string, string>(vastustajaGroup,vastustajaFilter,vastustajaErittely);
        }
        public Tuple<string, string, string> vastustaja(string vastustaja = "", bool j = true)
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
            if (!String.IsNullOrEmpty(vastustaja))
                vastustajaErittely = @"
                    vastustaja `Vastustaja`,";
            Console.WriteLine($"Tyhjä? {vastustajaErittely}");
            return new Tuple<string, string, string>(vastustajaGroup,vastustajaFilter,vastustajaErittely);
        }
        public Tuple<string, string, string> kotiJoukkue(string kotijoukkue){
            string kotiGroup = "";
            string kotiFilter = "";
            string kotiErittely = "";
            if (!String.IsNullOrEmpty(kotijoukkue))
            {
                kotiErittely = "kotijoukkue Kotijoukkue,";
                if (kotijoukkue == "Eritelty")
                {
                    kotiGroup = ", kotijoukkue";
                } else {
                    kotiFilter = "AND kotijoukkue = @kotijoukkue";
                }
            }
            return new Tuple<string, string, string>(kotiGroup,kotiFilter,kotiErittely);
        }
        public Tuple<string, string, string> vierasJoukkue(string vierasjoukkue){
            string vierasGroup = "";
            string vierasFilter = "";
            string vierasErittely = "";
            if (!String.IsNullOrEmpty(vierasjoukkue))
            {
                vierasErittely = "vierasjoukkue Vierasjoukkue,";
                if (vierasjoukkue == "Eritelty")
                {
                    vierasGroup = ", vierasjoukkue";
                } else {
                    vierasFilter = "AND vierasjoukkue = @vierasjoukkue";
                }
            }

            return new Tuple<string, string, string>(vierasGroup,vierasFilter,vierasErittely);
        }
        public Tuple<string, string, string> lukkari(string lukkari){
            string Group = "";
            string Filter = "";
            string Erittely = "";
            if (!String.IsNullOrEmpty(lukkari))
            {
                Erittely = @"
nimi Lukkari,
SUM((ot.joukkue_id == o.koti_id) and (kp>vp))+SUM((ot.joukkue_id == o.vieras_id) and (vp>kp)) `Lukkarin voitot`,
SUM((ot.joukkue_id == o.koti_id) and (kp<vp))+SUM((ot.joukkue_id == o.vieras_id) and (vp<kp)) `Lukkarin tappiot`,
ROUND(100.0*(SUM((ot.joukkue_id == o.koti_id) and (kp>vp))+SUM((ot.joukkue_id == o.vieras_id) and (vp>kp)))/COUNT(DISTINCT o.ottelu_id),2) `Lukkarin voitto-%`,
SUM(ot.joukkue_id == o.koti_id) `Kohtaamiset koti`,
SUM(ot.joukkue_id == o.vieras_id) `Kohtaamiset vieras`,";
                if (lukkari == "Eritelty")
                {
                    Group = ", Lukkari";
                } else {
                    Filter = "AND Lukkari = @lukkari";
                }
            }

            return new Tuple<string, string, string>(Group,Filter,Erittely);
        }
        public Tuple<string, string, string> vuosittain(bool vuosittain){
            string group = "";
            string erittely = "";
            string erittely2 = ", COUNT(DISTINCT kausi) Kaudet";
            if (vuosittain) {
                group = ", kausi";
                erittely = "kausi Kausi,";
                erittely2 = "";
            }
            return new Tuple<string, string, string>(group,erittely,erittely2);
        }
    }
}
