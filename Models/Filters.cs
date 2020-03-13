using System;
using System.IO;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data;
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
                joukkueFilter = "AND vj = @joukkue";
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
                kotiFilter = $"AND t3.koti='koti'";
            } else if (koti == "Vieras") {
                kotiFilter = $"AND t3.koti='vieras'";
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
    }
}
