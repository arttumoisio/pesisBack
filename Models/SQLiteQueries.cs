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
    class SQLiteQueries
    {
        private SQLiteConnection _con;
        private const string DB_NAME = "pesistk.db";
        public SQLiteQueries()
        {
            string cwd = Directory.GetCurrentDirectory();
            DirectoryInfo dir = new DirectoryInfo(cwd);
            string absolutePathToDb = null;

            // Etsitään polku tietokannalle.
            while (dir.Parent != null)
            {
                string path = dir.ToString() + $"{Path.DirectorySeparatorChar}Assets{Path.DirectorySeparatorChar}{DB_NAME}";

                Console.WriteLine(path);
                if (File.Exists(path))
                {
                    absolutePathToDb = path;

                    break;
                }
                else dir = dir.Parent;
                Console.WriteLine(path);
            }
            if (absolutePathToDb is null)
            {
                // TODO: Täytyy keskeyttää kaikki!
                System.Diagnostics.Debug.WriteLine("Database not found!");
            }

            var connectionStringBuilder = new SQLiteConnectionStringBuilder();

            //Use DB in project directory.  If it does not exist, create it:
            connectionStringBuilder.DataSource = absolutePathToDb;

             _con = new SQLiteConnection(connectionStringBuilder.ConnectionString);

            _con.Open();
        }
        private string toteutaKysely(SQLiteCommand dbCmd)
        {
            Console.WriteLine(dbCmd.CommandText);
            DataTable dt = new DataTable();
            dt.Load(dbCmd.ExecuteReader());
            return JsonConvert.SerializeObject(dt);  
        }
        private Tuple<string, string, string> koti(string koti = "")
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
            return new Tuple<string, string, string>(kotiGroup,kotiFilter,kotiErittely)
        }

        /// <summary>
        /// // TODO
        /// </summary>
        public string haePelaajatVuosittain(
            int vuosialkaen, 
            int vuosiloppuen, 
            string koti = "", 
            string pisteet = ""
        )
        {
            Tuple<string,string,string> k = this.koti(koti);
            string kotiGroup = k.Item1;
            string kotiFilter = k.Item2;
            string kotiErittely = k.Item2;
            string query = $@"
            SELECT 
            p.nimi Nimi,
            kausi Kausi,
            {kotiErittely}
            joukkue Joukkue,
            SUM(o) Ottelut,
            SUM(ku+ly) 'Lyödyt yht',
            SUM(ku) Kunnarit, 
            SUM(ly) Lyödyt,
            SUM(tu) Tuodut,
            SUM(ly+tu+ku) 'Tehopist. yht',
            SUM(lv) LV,
            SUM(kl1+kl2+kl3+kl4) KL,
            SUM(kl1) `KL->1`, 
            SUM(kl2) `KL->2`, 
            SUM(kl3) `KL->3`, 
            ROUND(1.0*SUM(ku+ly)/SUM(o),2) 'Lyödyt yht per ottelu', 
            ROUND(1.0*SUM(ku)/SUM(o),2) 'Kunnarit per ottelu', 
            ROUND(1.0*SUM(ly)/SUM(o),2) 'Lyödyt per ottelu',
            ROUND(1.0*SUM(tu)/SUM(o),2) 'Tuodut per ottelu',
            ROUND(1.0*SUM(ly+tu+ku)/SUM(o),2) 'Tehopist. yht per ottelu',
            ROUND(1.0*SUM(lv)/SUM(o),2) 'LV per ottelu',
            ROUND(1.0*SUM(kl1+kl2+kl3+kl4)/SUM(o),2) 'KL per ottelu',
            ROUND(1.0*SUM(kl1)/SUM(o),2) 'KL->1 per ottelu',
            ROUND(1.0*SUM(kl2)/SUM(o),2) 'KL->2 per ottelu',
            ROUND(1.0*SUM(kl3)/SUM(o),2) 'KL->3 per ottelu',
            MAX(ku + ly) `Maks lyodyt yht ottelussa`,
            MAX(ku+0) `Maks kunnarit ottelussa`,
            MAX(ly+0) `Maks lyödyt ottelussa`,
            MAX(tu+0) `Maks tuodut ottelussa`,
            MAX(ku+ly+tu) `Maks Tehopist. ottelussa`,
            MAX(kl1+kl2+kl3+kl4) `Maks KL ottelussa`,
            MAX(kl1) `Maks KL->1 ottelussa`,
            MAX(kl2) `Maks KL->2 ottelussa`,
            MAX(kl3) `Maks KL->3 ottelussa`,
            MAX(lv+0) `Maks LV ottelussa`,
            ROUND(1.0*SUM(kl1+kl2+kl3+kl4)/SUM(lv),2) 'KL per LV',
            ROUND(1.0*SUM(kl1)/SUM(lv),2) 'KL->1 per LV',
            ROUND(1.0*SUM(kl2)/SUM(lv),2) 'KL->2 per LV',
            ROUND(1.0*SUM(kl3)/SUM(lv),2) 'KL->3 per LV',
            ROUND(1.0*SUM(kl4)/SUM(lv),2) 'Lyödyt yht per LV',
            ROUND(1.0*SUM(ku)/SUM(lv),3) 'Kunnarit per LV',
            ROUND(1.0*SUM(ly)/SUM(lv),2) 'Lyödyt per LV',
            ROUND(1.0*SUM(tu)/SUM(lv),2) 'Tuodut per LV',
            ROUND(1.0*SUM(kl4+tu)/SUM(lv),2) 'Tehopist. per LV'
            FROM ottelu_tilasto ot, pelaaja p, ottelu o, joukkue j
            WHERE ot.pelaaja_id = p.pelaaja_id AND j.joukkue_id = ot.joukkue_id {kotiFilter}
            AND ot.ottelu_id = o.ottelu_id
            AND kausi BETWEEN @vuosialkaen AND @vuosiloppuen
            GROUP BY ot.pelaaja_id, kausi{kotiGroup}
            ORDER BY `Tehopist. yht` DESC
            ";

            SQLiteCommand dbCmd = _con.CreateCommand();
            dbCmd.CommandText = query;
            dbCmd.Parameters.AddWithValue("@vuosialkaen", vuosialkaen);
            dbCmd.Parameters.AddWithValue("@vuosiloppuen", vuosiloppuen);
            
            return toteutaKysely(dbCmd);

        }
        public string haePelaajat(
            int vuosialkaen, 
            int vuosiloppuen, 
            string koti = "", 
            string pisteet = ""
        )
        {
            Tuple<string,string,string> k = this.koti(koti);
            string kotiGroup = k.Item1;
            string kotiFilter = k.Item2;
            string kotiErittely = k.Item2;

            string pisteetGroup = "";
            string pisteetErittely = "";
            string pisteetFilter = $@"
                    AND (
                    CASE ot.joukkue_id
                        WHEN o.vieras_id THEN vp
                        WHEN o.koti_id THEN kp
                    END
                    ) ";
            switch (pisteet)
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
                        
            SQLiteCommand dbCmd = _con.CreateCommand();
            string query = $@"
            SELECT  
            p.nimi Nimi,
            {kotiErittely}
            {pisteetErittely}
            SUM(o) Ottelut,
            SUM(ku+ly) 'Lyödyt yht', 
            SUM(ku) Kunnarit, 
            SUM(ly) Lyödyt,
            SUM(tu) Tuodut,
            SUM(ly+tu+ku) 'Tehopist. yht',
            SUM(lv) LV,
            SUM(kl1+kl2+kl3+kl4) KL, 
            SUM(kl1) `KL->1`, 
            SUM(kl2) `KL->2`, 
            SUM(kl3) `KL->3`, 
            ROUND(1.0*SUM(ku+ly)/SUM(o),2) 'Lyödyt yht per ottelu', 
            ROUND(1.0*SUM(ku)/SUM(o),2) 'Kunnarit per ottelu', 
            ROUND(1.0*SUM(ly)/SUM(o),2) 'Lyödyt per ottelu',
            ROUND(1.0*SUM(tu)/SUM(o),2) 'Tuodut per ottelu',
            ROUND(1.0*SUM(ly+tu+ku)/SUM(o),2) 'Tehopist. yht per ottelu',
            ROUND(1.0*SUM(lv)/SUM(o),2) 'LV per ottelu',
            ROUND(1.0*SUM(kl1+kl2+kl3+kl4)/SUM(o),2) 'KL per ottelu',
            ROUND(1.0*SUM(kl1)/SUM(o),2) 'KL->1 per ottelu',
            ROUND(1.0*SUM(kl2)/SUM(o),2) 'KL->2 per ottelu',
            ROUND(1.0*SUM(kl3)/SUM(o),2) 'KL->3 per ottelu', 
            ROUND(1.0*SUM(ku+ly)/COUNT(DISTINCT kausi),2) 'Lyödyt yht per kausi', 
            ROUND(1.0*SUM(ku)/COUNT(DISTINCT kausi),2) 'Kunnarit per kausi', 
            ROUND(1.0*SUM(ly)/COUNT(DISTINCT kausi),2) 'Lyödyt per kausi',
            ROUND(1.0*SUM(tu)/COUNT(DISTINCT kausi),2) 'Tuodut per kausi',
            ROUND(1.0*SUM(ly+tu+ku)/COUNT(DISTINCT kausi),2) 'Tehopist. yht per kausi',
            ROUND(1.0*SUM(lv)/COUNT(DISTINCT kausi),2) 'LV per kausi',
            ROUND(1.0*SUM(kl1+kl2+kl3+kl4)/COUNT(DISTINCT kausi),2) 'KL per kausi',
            ROUND(1.0*SUM(kl1)/COUNT(DISTINCT kausi),2) 'KL->1 per kausi',
            ROUND(1.0*SUM(kl2)/COUNT(DISTINCT kausi),2) 'KL->2 per kausi',
            ROUND(1.0*SUM(kl3)/COUNT(DISTINCT kausi),2) 'KL->3 per kausi',
            MAX(ku + ly) `Maks lyodyt yht ottelussa`,
            MAX(ku+0) `Maks kunnarit ottelussa`,
            MAX(ly+0) `Maks lyödyt ottelussa`,
            MAX(tu+0) `Maks tuodut ottelussa`,
            MAX(ku + ly + tu) `Maks Tehopist. ottelussa`,
            MAX(kl1+kl2+kl3+kl4) `Maks KL ottelussa`,
            MAX(kl1) `Maks KL->1 ottelussa`,
            MAX(kl2) `Maks KL->2 ottelussa`,
            MAX(kl3) `Maks KL->3 ottelussa`,
            MAX(lv+0) `Maks LV ottelussa`,
            ROUND(1.0*SUM(kl1+kl2+kl3+kl4)/SUM(lv),2) 'KL per LV',
            ROUND(1.0*SUM(kl1)/SUM(lv),2) 'KL->1 per LV',
            ROUND(1.0*SUM(kl2)/SUM(lv),2) 'KL->2 per LV',
            ROUND(1.0*SUM(kl3)/SUM(lv),2) 'KL->3 per LV',
            ROUND(1.0*SUM(kl4)/SUM(lv),2) 'Lyödyt yht per LV',
            ROUND(1.0*SUM(ku)/SUM(lv),3) 'Kunnarit per LV',
            ROUND(1.0*SUM(ly)/SUM(lv),2) 'Lyödyt per LV',
            ROUND(1.0*SUM(tu)/SUM(lv),2) 'Tuodut per LV',
            ROUND(1.0*SUM(kl4+tu)/SUM(lv),2) 'Tehopist. per LV',
            COUNT(DISTINCT kausi) Kaudet,
            COUNT(DISTINCT joukkue) Joukkueet,
            GROUP_CONCAT(DISTINCT joukkue) Joukkueet
            FROM ottelu_tilasto ot, pelaaja p, ottelu o, joukkue j
            WHERE ot.pelaaja_id = p.pelaaja_id 
            AND ot.joukkue_id = j.joukkue_id
            AND ot.ottelu_id = o.ottelu_id
            {kotiFilter}
            {pisteetFilter}
            AND kausi BETWEEN @vuosialkaen AND @vuosiloppuen
            GROUP BY 
            ot.pelaaja_id 
            {kotiGroup} 
            {pisteetGroup}
            ORDER BY `Tehopist. yht` DESC
            ";
            dbCmd.CommandText = query;
            dbCmd.Parameters.AddWithValue("@vuosialkaen", vuosialkaen);
            dbCmd.Parameters.AddWithValue("@vuosiloppuen", vuosiloppuen);

            return toteutaKysely(dbCmd);
        }

        public string haeJoukkueet(
            int vuosialkaen, 
            int vuosiloppuen,
            string joukkue)
        {
            SQLiteCommand dbCmd = _con.CreateCommand();
            string query = @"
            SELECT 
            kj Joukkue,
            ko+vo Ottelut,
            kp+vp Pisteet,
            k3p+v3p '3p',
            k2p+v2p '2p',
            k1p+v1p '1p',
            k0p+v0p '0p',
            kp 'Pisteet kotona',
            k3p '3p kotona',
            k2p '2p kotona',
            k1p '1p kotona',
            k0p '0p kotona',
            vp 'Pisteet vieraissa',
            v3p '3p vieraissa',
            v2p '2p vieraissa',
            v1p '1p vieraissa',
            v0p '0p vieraissa',
            kju+vju Juoksut,
            vpä+kpä Päästetyt,
            kk Kaudet

            FROM
            (
            SELECT 
            joukkue kj,
            COUNT(*) ko,
            SUM(kp) kp,
            SUM(k3p) 'k3p',
            SUM(k2p) 'k2p',
            SUM(k1p) 'k1p',
            SUM(k0p) 'k0p',
            SUM(k1j+k2j+ks) kju,
            SUM(v1j+v2j+vs) kpä,
            COUNT(DISTINCT kausi) kk

            FROM ottelu o, joukkue j
            WHERE o.koti_id = j.joukkue_id 
            AND kausi BETWEEN @vuosialkaen AND @vuosiloppuen
            AND tila != 'ottelu ei ole vielä alkanut'
            GROUP BY joukkue
            ) t1,
            (
            SELECT 
            joukkue vj,
            COUNT(*) vo,
            SUM(vp) vp,
            SUM(v3p) 'v3p',
            SUM(v2p) 'v2p',
            SUM(v1p) 'v1p',
            SUM(v0p) 'v0p',
            SUM(v1j+v2j+vs) vju,
            SUM(k1j+k2j+ks) vpä,
            COUNT(DISTINCT kausi) vk

            FROM ottelu o, joukkue j
            WHERE o.vieras_id = j.joukkue_id 
            AND kausi BETWEEN @vuosialkaen AND @vuosiloppuen
            AND tila != 'ottelu ei ole vielä alkanut'
            GROUP BY joukkue
            ) t2
            WHERE vj = kj";
            if (joukkue != "Mikä tahansa") {
                query += "AND vj = @joukkue";
            }
            query += @"
            GROUP BY kj
            ORDER BY Pisteet DESC
            ;
            ";
            

            dbCmd.CommandText = query;
            dbCmd.Parameters.AddWithValue("@vuosialkaen", vuosialkaen);
            dbCmd.Parameters.AddWithValue("@vuosiloppuen", vuosiloppuen);
            dbCmd.Parameters.AddWithValue("@joukkue", joukkue);

            return toteutaKysely(dbCmd);
        }
        public string haeJoukkueetVuosittain(
            int vuosialkaen, 
            int vuosiloppuen,
            string joukkue)
        {
            string query = @"
            SELECT 
            kj Joukkue,
            kk Kausi,
            ko+vo Ottelut,
            kp+vp Pisteet,
            k3p+v3p '3p',
            k2p+v2p '2p',
            k1p+v1p '1p',
            k0p+v0p '0p',
            kp 'Pisteet kotona',
            k3p '3p kotona',
            k2p '2p kotona',
            k1p '1p kotona',
            k0p '0p kotona',
            vp 'Pisteet vieraissa',
            v3p '3p vieraissa',
            v2p '2p vieraissa',
            v1p '1p vieraissa',
            v0p '0p vieraissa',
            kju+vju Juoksut,
            vpä+kpä Päästetyt

            FROM
            (
            SELECT 
            joukkue kj,
            COUNT(*) ko,
            SUM(kp) kp,
            SUM(k3p) 'k3p',
            SUM(k2p) 'k2p',
            SUM(k1p) 'k1p',
            SUM(k0p) 'k0p',
            SUM(k1j+k2j+ks) kju,
            SUM(v1j+v2j+vs) kpä,
            kausi kk

            FROM ottelu o, joukkue j
            WHERE o.koti_id = j.joukkue_id 
            AND kausi BETWEEN @vuosialkaen AND @vuosiloppuen
            AND tila != 'ottelu ei ole vielä alkanut'
            GROUP BY joukkue, kausi
            ) t1,
            (
            SELECT 
            joukkue vj,
            COUNT(*) vo,
            SUM(vp) vp,
            SUM(v3p) 'v3p',
            SUM(v2p) 'v2p',
            SUM(v1p) 'v1p',
            SUM(v0p) 'v0p',
            SUM(v1j+v2j+vs) vju,
            SUM(k1j+k2j+ks) vpä,
            kausi vk

            FROM ottelu o, joukkue j
            WHERE o.vieras_id = j.joukkue_id 
            AND kausi BETWEEN @vuosialkaen AND @vuosiloppuen
            AND tila != 'ottelu ei ole vielä alkanut'
            GROUP BY joukkue,kausi
            ) t2
            WHERE kk = vk AND kj = vj ";
            if (joukkue != "Mikä tahansa") {
                query += "AND vj = @joukkue";
            }
            query += @"
            GROUP BY kj,kk
            ORDER BY Pisteet DESC
            ;
            ";

            SQLiteCommand dbCmd = _con.CreateCommand();
            dbCmd.CommandText = query;
            dbCmd.Parameters.AddWithValue("@vuosialkaen", vuosialkaen);
            dbCmd.Parameters.AddWithValue("@vuosiloppuen", vuosiloppuen);
            dbCmd.Parameters.AddWithValue("@joukkue", joukkue);

            return toteutaKysely(dbCmd);
        }
        public string apuJoukkueet(
            int vuosialkaen, 
            int vuosiloppuen)
        {
            SQLiteCommand dbCmd = _con.CreateCommand();
            string query = @"
            SELECT DISTINCT joukkue
            FROM ottelu o, joukkue j
            WHERE kausi BETWEEN @vuosialkaen AND @vuosiloppuen
            AND (joukkue_id = koti_id OR joukkue_id = vieras_id)
            ORDER BY joukkue
            ";
            dbCmd.CommandText = query;
            dbCmd.Parameters.AddWithValue("@vuosialkaen", vuosialkaen);
            dbCmd.Parameters.AddWithValue("@vuosiloppuen", vuosiloppuen);

            return toteutaKysely(dbCmd);
        }
        public string apuVuodet()
        {
            SQLiteCommand dbCmd = _con.CreateCommand();
            string query = @"
            SELECT DISTINCT kausi
            FROM ottelu o
            ORDER BY kausi
            ";
            dbCmd.CommandText = query;

            return toteutaKysely(dbCmd);
        }
    }
}
