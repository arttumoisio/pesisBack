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

        private Filters filters = new Filters(); 
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

        /// <summary>
        /// // TODO
        /// </summary>
        public string haePelaajatVuosittain(
            int vuosialkaen, 
            int vuosiloppuen, 
            string koti = "", 
            string tulos = "",
            string vastustaja = ""

        )
        {
            Tuple<string,string,string> k = filters.koti(koti);
            string kotiGroup = k.Item1;
            string kotiFilter = k.Item2;
            string kotiErittely = k.Item3;

            Tuple<string,string,string> t = filters.tulos(tulos);
            string tulosGroup = t.Item1;
            string tulosFilter = t.Item2;
            string tulosErittely = t.Item3;

            Tuple<string,string,string> v = filters.vastustaja(vastustaja);
            string vastustajaGroup = v.Item1;
            string vastustajaFilter = v.Item2;
            string vastustajaErittely = v.Item3;
            Console.WriteLine($"Tyhjä? {vastustajaErittely}");

            string query = $@"
            SELECT 
            p.nimi Nimi,
            kausi Kausi,
            joukkue Joukkue,
            {kotiErittely}
            {tulosErittely}
            {vastustajaErittely}
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
            WHERE ot.pelaaja_id = p.pelaaja_id 
            AND j.joukkue_id = ot.joukkue_id 
            AND ot.ottelu_id = o.ottelu_id
            {kotiFilter}
            {tulosFilter}
            {vastustajaFilter}
            AND kausi BETWEEN @vuosialkaen AND @vuosiloppuen
            GROUP BY ot.pelaaja_id, kausi
            {kotiGroup}
            {tulosGroup}
            {vastustajaGroup}
            ORDER BY `Tehopist. yht` DESC;";

            SQLiteCommand dbCmd = _con.CreateCommand();
            dbCmd.CommandText = query;
            dbCmd.Parameters.AddWithValue("@vuosialkaen", vuosialkaen);
            dbCmd.Parameters.AddWithValue("@vuosiloppuen", vuosiloppuen);
            dbCmd.Parameters.AddWithValue("@vastustaja", vastustaja);
            
            Console.WriteLine($"Tyhjä? {vastustajaErittely}");
            return toteutaKysely(dbCmd);

        }
        public string haePelaajat(
            int vuosialkaen, 
            int vuosiloppuen, 
            string koti = "", 
            string tulos = "",
            string vastustaja = ""
        )
        {
            Tuple<string,string,string> k = filters.koti(koti);
            string kotiGroup = k.Item1;
            string kotiFilter = k.Item2;
            string kotiErittely = k.Item3;
            
            Tuple<string,string,string> t = filters.tulos(tulos);
            string tulosGroup = t.Item1;
            string tulosFilter = t.Item2;
            string tulosErittely = t.Item3;

            Tuple<string,string,string> v = filters.vastustaja(vastustaja);
            string vastustajaGroup = v.Item1;
            string vastustajaFilter = v.Item2;
            string vastustajaErittely = v.Item3;
            
                        
            SQLiteCommand dbCmd = _con.CreateCommand();
            string query = $@"
            SELECT  
            p.nimi Nimi,
            {kotiErittely}
            {tulosErittely}
            {vastustajaErittely}
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
            {tulosFilter}
            {vastustajaFilter}
            AND kausi BETWEEN @vuosialkaen AND @vuosiloppuen
            GROUP BY 
            ot.pelaaja_id 
            {kotiGroup} 
            {tulosGroup}
            {vastustajaGroup}
            ORDER BY `Tehopist. yht` DESC;";
            dbCmd.CommandText = query;
            dbCmd.Parameters.AddWithValue("@vuosialkaen", vuosialkaen);
            dbCmd.Parameters.AddWithValue("@vuosiloppuen", vuosiloppuen);

            return toteutaKysely(dbCmd);
        }
        public string haeJoukkueetVuosittain(
            int vuosialkaen, 
            int vuosiloppuen,
            string joukkue,
            string koti,
            string tulos,
            string vastustaja )
        {
            string joukkueFilter = filters.joukkue(joukkue);

            Tuple<string,string,string> k = filters.koti(koti, true);
            string kotiGroup = k.Item1;
            string kotiFilter = k.Item2;
            string kotiErittely = k.Item3;
            
            Tuple<string,string,string> t = filters.tulos(tulos, true);
            string tulosGroup = t.Item1;
            string tulosFilter = t.Item2;
            string tulosErittely = t.Item3;

            Tuple<string,string,string> v = filters.vastustaja(vastustaja, true);
            string vastustajaGroup = v.Item1;
            string vastustajaFilter = v.Item2;
            string vastustajaErittely = v.Item3;

            string query = $@"
            SELECT 
            kj Joukkue,
            kk Kausi,
            {kotiErittely}
            {tulosErittely}
            {vastustajaErittely}
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
            'koti' koti,
            koti_id k_id,
            vieras_id kv_id,
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
            AND tila != 'ottelu ei ole vielä alkanut'
            GROUP BY joukkue_id, kausi
            ) t1,
            (
            SELECT 
            joukkue vj,
            'vieras' koti,
            koti_id v_id,
            vieras_id vv_id,
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
            AND tila != 'ottelu ei ole vielä alkanut'
            GROUP BY joukkue_id,kausi
            ) t2
            WHERE kk = vk AND v_id = k_id 
            {joukkueFilter}
            {kotiFilter}
            {tulosFilter}
            {vastustajaFilter}
            AND vk BETWEEN @vuosialkaen AND @vuosiloppuen
            AND kk BETWEEN @vuosialkaen AND @vuosiloppuen
            GROUP BY kj,kk
            {kotiGroup} 
            {tulosGroup}
            {vastustajaGroup}
            ORDER BY Pisteet DESC;";

            SQLiteCommand dbCmd = _con.CreateCommand();
            dbCmd.CommandText = query;
            dbCmd.Parameters.AddWithValue("@vuosialkaen", vuosialkaen);
            dbCmd.Parameters.AddWithValue("@vuosiloppuen", vuosiloppuen);
            dbCmd.Parameters.AddWithValue("@joukkue", joukkue);

            return toteutaKysely(dbCmd);
        }

        public string haeJoukkueet(
            int vuosialkaen, 
            int vuosiloppuen,
            string joukkue,
            string koti,
            string tulos,
            string vastustaja )
        {
            string joukkueFilter = filters.joukkue(joukkue);

            Tuple<string,string,string> k = filters.koti(koti, true);
            string kotiGroup = k.Item1;
            string kotiFilter = k.Item2;
            string kotiErittely = k.Item3;
            
            Tuple<string,string,string> t = filters.tulos(tulos, true);
            string tulosGroup = t.Item1;
            string tulosFilter = t.Item2;
            string tulosErittely = t.Item3;

            Tuple<string,string,string> v = filters.vastustaja(vastustaja, true);
            string vastustajaGroup = v.Item1;
            string vastustajaFilter = v.Item2;
            string vastustajaErittely = v.Item3;

            SQLiteCommand dbCmd = _con.CreateCommand();
            string query = $@"
            SELECT 
            kj Joukkue,
            {kotiErittely}
            {tulosErittely}
            {vastustajaErittely}
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
            'koti' koti,
            koti_id k_id,
            vieras_id kv_id,
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
            'vieras' koti,
            vieras_id v_id,
            koti_id vv_id,
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
            WHERE v_id = k_id 
            {joukkueFilter}
            {kotiFilter}
            {tulosFilter}
            {vastustajaFilter}
            GROUP BY kj
            {kotiGroup} 
            {tulosGroup}
            {vastustajaGroup}
            ORDER BY Pisteet DESC;";

            

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
