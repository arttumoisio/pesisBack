using System;
using System.IO;
using System.Data.SQLite;
using System.Data;
using Newtonsoft.Json;

namespace pesisBackend
{
    /// <SUMmary>
    /// // TODO
    /// </SUMmary>
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

        /// <SUMmary>
        /// // TODO
        /// </SUMmary>
        public string haePelaajatVuosittain(
            int vuosialkaen, 
            int vuosiloppuen, 
            string koti = "", 
            string tulos = "",
            string vastustaja = "",
            string joukkue = "",
            string sarja="",
            string sarjavaihe=""

        )
        {
            
            Tuple<string,string,string> sv = filters.sarjavaihe(sarjavaihe);
            string sarjavaiheGroup = sv.Item1;
            string sarjavaiheFilter = sv.Item2;
            string sarjavaiheErittely = sv.Item3;

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
            
            string joukkueFilter = filters.joukkue(joukkue);

            string query = $@"
            BEGIN;
            SELECT 
            p.nimi Nimi,
            kausi Kausi,
            joukkue Joukkue,
            {sarjavaiheErittely}
            {kotiErittely}
            {tulosErittely}
            {vastustajaErittely}
            COUNT(o.ottelu_id) Ottelut,
            SUM(ku) Kunnarit, 
            SUM(ly) Lyödyt,
            SUM(ku+ly) 'K + L yht.',
            SUM(tu) Tuodut,
            SUM(ly+tu+ku) 'Tehopist. yht',
            SUM(lv) LV,
            SUM(kl1+kl2+kl3+kl4) KL,
            SUM(kl1) `KL->1`, 
            SUM(kl2) `KL->2`, 
            SUM(kl3) `KL->3`,
            ROUND(1.0*SUM(
                CASE 
                    WHEN koti_id = ot.joukkue_id THEN kp > vp
                    WHEN vieras_id = ot.joukkue_id THEN vp > kp
                END
                )/COUNT(DISTINCT o.ottelu_id),2) 'Voitto-%', 
            ROUND(1.0*SUM(ku)/COUNT(o.ottelu_id),2) 'Kunnarit per ottelu',
            ROUND(1.0*SUM(ly)/COUNT(o.ottelu_id),2) 'Lyödyt per ottelu',
            ROUND(1.0*SUM(ku+ly)/COUNT(o.ottelu_id),2) 'K + L per ottelu', 
            ROUND(1.0*SUM(tu)/COUNT(o.ottelu_id),2) 'Tuodut per ottelu',
            ROUND(1.0*SUM(ly+tu+ku)/COUNT(o.ottelu_id),2) 'Tehopist. yht per ottelu',
            ROUND(1.0*SUM(lv)/COUNT(o.ottelu_id),2) 'LV per ottelu',
            ROUND(1.0*SUM(kl1+kl2+kl3+kl4)/COUNT(o.ottelu_id),2) 'KL per ottelu',
            ROUND(1.0*SUM(kl1)/COUNT(o.ottelu_id),2) 'KL->1 per ottelu',
            ROUND(1.0*SUM(kl2)/COUNT(o.ottelu_id),2) 'KL->2 per ottelu',
            ROUND(1.0*SUM(kl3)/COUNT(o.ottelu_id),2) 'KL->3 per ottelu',
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
            ROUND(1.0*SUM(ku)/SUM(lv),3) 'Kunnarit per LV',
            ROUND(1.0*SUM(ly)/SUM(lv),2) 'Lyödyt per LV',
            ROUND(1.0*SUM(kl4)/SUM(lv),2) 'K + L per LV',
            ROUND(1.0*SUM(tu)/SUM(lv),2) 'Tuodut per LV',
            ROUND(1.0*SUM(kl4+tu)/SUM(lv),2) 'Tehopist. per LV',
            SUM(palkinto = 'A') `Kentän paras`,
            SUM(palkinto = 'I') `1 palkinto`,
            SUM(palkinto = 'II') `2 palkinto`,
            SUM(upp = 'L') 'Lukkari',
            SUM(upp = 'S') 'Sieppari',
            SUM(upp = '1V') '1-vahti',
            SUM(upp = '2V') '2-vahti',
            SUM(upp = '3V') '3-vahti',
            SUM(upp = '2P') '2-polttaja',
            SUM(upp = '3P') '3-polttaja',
            SUM(upp = '2K') '2-koppari',
            SUM(upp = '3K') '3-koppari',
            SUM(upp = '') 'jokeri',
            SUM(pelaaja_nro = '1') 'Nro 1',
            SUM(pelaaja_nro = '2') 'Nro 2',
            SUM(pelaaja_nro = '3') 'Nro 3',
            SUM(pelaaja_nro = '4') 'Nro 4',
            SUM(pelaaja_nro = '5') 'Nro 5',
            SUM(pelaaja_nro = '6') 'Nro 6',
            SUM(pelaaja_nro = '7') 'Nro 7',
            SUM(pelaaja_nro = '8') 'Nro 8',
            SUM(pelaaja_nro = '9') 'Nro 9',
            SUM(pelaaja_nro = '10') 'Nro 10',
            SUM(pelaaja_nro = '11') 'Nro 11',
            SUM(pelaaja_nro = '12') 'Nro 12'

            FROM ottelu_tilasto ot
            INNER JOIN ottelu o  
                ON ot.ottelu_id = o.ottelu_id 
                AND sarja = @sarja
                AND tila != 'ottelu ei ole vielä alkanut'
                AND kausi BETWEEN @vuosialkaen AND @vuosiloppuen
            INNER JOIN pelaaja p ON ot.pelaaja_id = p.pelaaja_id 
            INNER JOIN joukkue j ON ot.joukkue_id = j.joukkue_id
            WHERE 1
            {kotiFilter}
            {sarjavaiheFilter}
            {tulosFilter}
            {vastustajaFilter}
            {joukkueFilter}
            
            GROUP BY ot.pelaaja_id, kausi
            {kotiGroup}
            {tulosGroup}
            {vastustajaGroup}
            {sarjavaiheGroup}
            ORDER BY `Tehopist. yht` DESC;
            COMMIT;
            ";

            SQLiteCommand dbCmd = _con.CreateCommand();
            dbCmd.CommandText = query;
            dbCmd.Parameters.AddWithValue("@vuosialkaen", vuosialkaen);
            dbCmd.Parameters.AddWithValue("@vuosiloppuen", vuosiloppuen);
            dbCmd.Parameters.AddWithValue("@vastustaja", vastustaja);
            dbCmd.Parameters.AddWithValue("@joukkue", joukkue);
            dbCmd.Parameters.AddWithValue("@sarjavaihe", sarjavaihe);
            dbCmd.Parameters.AddWithValue("@sarja", sarja);

            Console.WriteLine($"Tyhjä? {vastustajaErittely}");
            return toteutaKysely(dbCmd);

        }
        public string haePelaajat(
            int vuosialkaen, 
            int vuosiloppuen, 
            string koti = "", 
            string tulos = "",
            string vastustaja = "",
            string joukkue = "",
            string sarja="",
            string sarjavaihe=""
        )
        {            
            Tuple<string,string,string> sv = filters.sarjavaihe(sarjavaihe);
            string sarjavaiheGroup = sv.Item1;
            string sarjavaiheFilter = sv.Item2;
            string sarjavaiheErittely = sv.Item3;

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

            string joukkueFilter = filters.joukkue(joukkue);
            
                        
            SQLiteCommand dbCmd = _con.CreateCommand();
            string query = $@"
            BEGIN;
            SELECT  
            p.nimi Nimi,
            {kotiErittely}
            {tulosErittely}
            {vastustajaErittely}
            {sarjavaiheErittely}
            COUNT(o.ottelu_id) Ottelut,
            ROUND(100.0*SUM(
                CASE 
                    WHEN koti_id = ot.joukkue_id THEN kp > vp
                    WHEN vieras_id = ot.joukkue_id THEN vp > kp
                END
                )/COUNT(DISTINCT o.ottelu_id),2) 'Voitto-%', 
            SUM(ku) Kunnarit, 
            SUM(ly) Lyödyt,
            SUM(ku+ly) 'K + L yht.', 
            SUM(tu) Tuodut,
            SUM(ly+tu+ku) 'Tehopist. yht',
            SUM(lv) LV,
            SUM(kl1+kl2+kl3+kl4) KL, 
            SUM(kl1) `KL->1`, 
            SUM(kl2) `KL->2`, 
            SUM(kl3) `KL->3`,
            ROUND(1.0*SUM(ku)/COUNT(o.ottelu_id),2) 'Kunnarit per ottelu', 
            ROUND(1.0*SUM(ly)/COUNT(o.ottelu_id),2) 'Lyödyt per ottelu',
            ROUND(1.0*SUM(ku+ly)/COUNT(o.ottelu_id),2) 'K + L per ottelu', 
            ROUND(1.0*SUM(tu)/COUNT(o.ottelu_id),2) 'Tuodut per ottelu',
            ROUND(1.0*SUM(ly+tu+ku)/COUNT(o.ottelu_id),2) 'Tehopist. yht per ottelu',
            ROUND(1.0*SUM(lv)/COUNT(o.ottelu_id),2) 'LV per ottelu',
            ROUND(1.0*SUM(kl1+kl2+kl3+kl4)/COUNT(o.ottelu_id),2) 'KL per ottelu',
            ROUND(1.0*SUM(kl1)/COUNT(o.ottelu_id),2) 'KL->1 per ottelu',
            ROUND(1.0*SUM(kl2)/COUNT(o.ottelu_id),2) 'KL->2 per ottelu',
            ROUND(1.0*SUM(kl3)/COUNT(o.ottelu_id),2) 'KL->3 per ottelu', 
            ROUND(1.0*SUM(ku)/COUNT(DISTINCT kausi),2) 'Kunnarit per kausi', 
            ROUND(1.0*SUM(ly)/COUNT(DISTINCT kausi),2) 'Lyödyt per kausi',
            ROUND(1.0*SUM(ku+ly)/COUNT(DISTINCT kausi),2) 'K + L per kausi', 
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
            ROUND(1.0*SUM(kl4)/SUM(lv),2) 'K + L per LV',
            ROUND(1.0*SUM(ku)/SUM(lv),3) 'Kunnarit per LV',
            ROUND(1.0*SUM(ly)/SUM(lv),2) 'Lyödyt per LV',
            ROUND(1.0*SUM(tu)/SUM(lv),2) 'Tuodut per LV',
            ROUND(1.0*SUM(kl4+tu)/SUM(lv),2) 'Tehopist. per LV',
            SUM(palkinto = 'A') `Kentän paras`,
            SUM(palkinto = 'I') `1 palkinto`,
            SUM(palkinto = 'II') `2 palkinto`,
            SUM(upp = 'L') 'Lukkari',
            SUM(upp = 'S') 'Sieppari',
            SUM(upp = '1V') '1-vahti',
            SUM(upp = '2V') '2-vahti',
            SUM(upp = '3V') '3-vahti',
            SUM(upp = '2P') '2-polttaja',
            SUM(upp = '3P') '3-polttaja',
            SUM(upp = '2K') '2-koppari',
            SUM(upp = '3K') '3-koppari',
            SUM(upp = '') 'jokeri',
            SUM(pelaaja_nro = '1') 'Nro 1',
            SUM(pelaaja_nro = '2') 'Nro 2',
            SUM(pelaaja_nro = '3') 'Nro 3',
            SUM(pelaaja_nro = '4') 'Nro 4',
            SUM(pelaaja_nro = '5') 'Nro 5',
            SUM(pelaaja_nro = '6') 'Nro 6',
            SUM(pelaaja_nro = '7') 'Nro 7',
            SUM(pelaaja_nro = '8') 'Nro 8',
            SUM(pelaaja_nro = '9') 'Nro 9',
            SUM(pelaaja_nro = '10') 'Nro 10',
            SUM(pelaaja_nro = '11') 'Nro 11',
            SUM(pelaaja_nro = '12') 'Nro 12',
            COUNT(DISTINCT kausi) Kaudet,
            COUNT(DISTINCT joukkue) Joukkueet,
            GROUP_CONCAT(DISTINCT joukkue) Joukkueet

            FROM ottelu_tilasto ot 
            INNER JOIN ottelu o  ON 
                ot.ottelu_id = o.ottelu_id
                AND sarja = @sarja
                AND tila != 'ottelu ei ole vielä alkanut'
                AND kausi BETWEEN @vuosialkaen AND @vuosiloppuen
            INNER JOIN pelaaja p ON ot.pelaaja_id = p.pelaaja_id 
            INNER JOIN joukkue j ON ot.joukkue_id = j.joukkue_id
            WHERE 1
            {sarjavaiheFilter}
            {kotiFilter}
            {tulosFilter}
            {vastustajaFilter}
            {joukkueFilter}
            GROUP BY ot.pelaaja_id 
            {kotiGroup} 
            {tulosGroup}
            {vastustajaGroup}
            {sarjavaiheGroup}
            ORDER BY `Tehopist. yht` DESC;
            COMMIT;
            ";

            dbCmd.CommandText = query;
            dbCmd.Parameters.AddWithValue("@vuosialkaen", vuosialkaen);
            dbCmd.Parameters.AddWithValue("@vuosiloppuen", vuosiloppuen);
            dbCmd.Parameters.AddWithValue("@joukkue", joukkue);
            dbCmd.Parameters.AddWithValue("@vastustaja", vastustaja);
            dbCmd.Parameters.AddWithValue("@sarjavaihe", sarjavaihe);
            dbCmd.Parameters.AddWithValue("@sarja", sarja);

            return toteutaKysely(dbCmd);
        }
        public string haeJoukkueetVuosittain(
            int vuosialkaen, 
            int vuosiloppuen,
            string joukkue,
            string koti,
            string tulos,
            string vastustaja,
            string sarja="",
            string sarjavaihe="" )
        {
            string joukkueFilter = filters.joukkue(joukkue);
                        
            Tuple<string,string,string> sv = filters.sarjavaihe(sarjavaihe);
            string sarjavaiheGroup = sv.Item1;
            string sarjavaiheFilter = sv.Item2;
            string sarjavaiheErittely = sv.Item3;

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
            BEGIN;
            SELECT 
            j.joukkue Joukkue,
            kausi Kausi,
            {sarjavaiheErittely}
            {kotiErittely}
            {tulosErittely}
            {vastustajaErittely}
            count(*) Ottelut,
            SUM(p) Pisteet,
            SUM(`3p`) `3p`,
            SUM(`2p`) `2p`,
            SUM(`1p`) `1p`,
            SUM(`0p`) `0p`,
            SUM(`1j`+`2j`+k+s) Tehdyt,
            SUM(`v1j`+`v2j`+vk+vs) Päästetyt,
            SUM(`1j`) `1j Tehdyt`,
            SUM(`2j`) `2j Tehdyt`,
            SUM(`s`) `S Tehdyt`,
            SUM(`k`) `K Tehdyt`,
            SUM(`v1j`) `1j Päästetyt`,
            SUM(`v2j`) `2j Päästetyt`,
            SUM(`vs`) `S Päästetyt`,
            SUM(`vk`) `K Päästetyt`,
            SUM(INSTR(tulos,'s')=0 AND INSTR(tulos,'k')=0) '2 jakson pelit',
            SUM(INSTR(tulos,'s')>0) 'Supervuoroon',
            SUM(INSTR(tulos,'k')>0) 'Kotariin',
            SUM(aloittaja) 'Aloittava sisävuoro',
            SUM(aloittaja=0) 'Aloittava ulkovuoro',
            SUM(svaloittaja) 'Aloittava sisävuoro superissa',
            SUM(INSTR(tulos,'s')>0)+SUM(INSTR(tulos,'k')>0)-SUM(svaloittaja) 'Aloittava ulkovuoro superissa',
            ROUND(AVG(CAST(CASE WHEN kotib THEN katsojamaara END AS INTEGER)),0) `Katsoja-keskiarvo`,
            ROUND(AVG(CAST(CASE WHEN kotib<>1 THEN katsojamaara END AS INTEGER)),0) `Katsoja-keskiarvo vieraissa`
            FROM joukkue j 
            INNER JOIN puoli_ottelu po ON 
                po.joukkue_id = j.joukkue_id
                AND tila != 'ottelu ei ole vielä alkanut'
                AND sarja = @sarja
                AND kausi BETWEEN @vuosialkaen AND @vuosiloppuen
            WHERE 1
            {sarjavaiheFilter}
            {joukkueFilter}
            {kotiFilter}
            {tulosFilter}
            {vastustajaFilter}
            GROUP BY po.joukkue ,kausi
            {kotiGroup} 
            {tulosGroup}
            {vastustajaGroup}
            {sarjavaiheGroup}
            ORDER BY Pisteet DESC;
            COMMIT;
            ";

            SQLiteCommand dbCmd = _con.CreateCommand();
            dbCmd.CommandText = query;
            dbCmd.Parameters.AddWithValue("@vuosialkaen", vuosialkaen);
            dbCmd.Parameters.AddWithValue("@vuosiloppuen", vuosiloppuen);
            dbCmd.Parameters.AddWithValue("@joukkue", joukkue);
            dbCmd.Parameters.AddWithValue("@vastustaja", vastustaja);
            dbCmd.Parameters.AddWithValue("@sarjavaihe", sarjavaihe);
            dbCmd.Parameters.AddWithValue("@sarja", sarja);

            return toteutaKysely(dbCmd);
        }
        public string haeJoukkueet(
            int vuosialkaen, 
            int vuosiloppuen,
            string joukkue,
            string koti,
            string tulos,
            string vastustaja,
            string sarja="",
            string sarjavaihe="" )
        {
            string joukkueFilter = filters.joukkue(joukkue);
                        
            Tuple<string,string,string> sv = filters.sarjavaihe(sarjavaihe);
            string sarjavaiheGroup = sv.Item1;
            string sarjavaiheFilter = sv.Item2;
            string sarjavaiheErittely = sv.Item3;

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
            BEGIN;
            SELECT 
            j.joukkue Joukkue,
            {sarjavaiheErittely}
            {kotiErittely}
            {tulosErittely}
            {vastustajaErittely}
            count(*) Ottelut,
            SUM(p) Pisteet,
            SUM(`3p`) `3p`,
            SUM(`2p`) `2p`,
            SUM(`1p`) `1p`,
            SUM(`0p`) `0p`,
            SUM(`1j`+`2j`+k+s) Tehdyt,
            SUM(`v1j`+`v2j`+vk+vs) Päästetyt,
            SUM(`1j`) `1j Tehdyt`,
            SUM(`2j`) `2j Tehdyt`,
            SUM(`s`) `S Tehdyt`,
            SUM(`k`) `K Tehdyt`,
            SUM(`v1j`) `1j Päästetyt`,
            SUM(`v2j`) `2j Päästetyt`,
            SUM(`vs`) `S Päästetyt`,
            SUM(`vk`) `K Päästetyt`,
            SUM(INSTR(tulos,'s')=0 AND INSTR(tulos,'k')=0) '2 jakson pelit',
            SUM(INSTR(tulos,'s')>0) 'Supervuoroon',
            SUM(INSTR(tulos,'k')>0) 'Kotariin',
            SUM(aloittaja) 'Aloittava sisävuoro',
            SUM(aloittaja=0) 'Aloittava ulkovuoro',
            SUM(svaloittaja) 'Aloittava sisävuoro superissa',
            SUM(INSTR(tulos,'s')>0)+SUM(INSTR(tulos,'k')>0)-SUM(svaloittaja) 'Aloittava ulkovuoro superissa',
            ROUND(AVG(CAST(CASE WHEN kotib THEN katsojamaara END AS INTEGER)),0) `Katsoja-keskiarvo`,
            ROUND(AVG(CAST(CASE WHEN kotib<>1 THEN katsojamaara END AS INTEGER)),0) `Katsoja-keskiarvo vieraissa`,
            COUNT(DISTINCT kausi) Kaudet
            FROM joukkue j 
            INNER JOIN puoli_ottelu po ON 
                po.joukkue_id = j.joukkue_id
                AND tila != 'ottelu ei ole vielä alkanut'
                AND sarja = @sarja
                AND kausi BETWEEN @vuosialkaen AND @vuosiloppuen
            WHERE 1
            {sarjavaiheFilter}
            {joukkueFilter}
            {kotiFilter}
            {tulosFilter}
            {vastustajaFilter}
            GROUP BY j.joukkue
            {kotiGroup} 
            {tulosGroup}
            {vastustajaGroup}
            {sarjavaiheGroup}
            ORDER BY Pisteet DESC;
            COMMIT;
            ";

            dbCmd.CommandText = query;
            dbCmd.Parameters.AddWithValue("@vuosialkaen", vuosialkaen);
            dbCmd.Parameters.AddWithValue("@vuosiloppuen", vuosiloppuen);
            dbCmd.Parameters.AddWithValue("@joukkue", joukkue);
            dbCmd.Parameters.AddWithValue("@vastustaja", vastustaja);
            dbCmd.Parameters.AddWithValue("@sarjavaihe", sarjavaihe);
            dbCmd.Parameters.AddWithValue("@sarja", sarja);

            return toteutaKysely(dbCmd);
        }
        public string haeTuomarit(
            int vuosialkaen, 
            int vuosiloppuen,
            bool vuosittain,
            string kotijoukkue,
            string vierasjoukkue,
            string lukkari,
            string STPT,
            string sarja="",
            string sarjavaihe=""
        )
        {   
            Console.WriteLine(vuosialkaen);
            Console.WriteLine(vuosiloppuen);
            string STPTFilter = filters.stpt(STPT);
                        
            Tuple<string,string,string> sv = filters.sarjavaihe(sarjavaihe);
            string sarjavaiheGroup = sv.Item1;
            string sarjavaiheFilter = sv.Item2;
            string sarjavaiheErittely = sv.Item3;

            Tuple<string,string,string> vu = filters.vuosittain(vuosittain);
            string vuosittainGroup = vu.Item1;
            string vuosittainErittely = vu.Item2;
            string vuosittainErittely2 = vu.Item3;

            Tuple<string,string,string> k = filters.kotiJoukkue(kotijoukkue);
            string kotiGroup = k.Item1;
            string kotiFilter = k.Item2;
            string kotiErittely = k.Item3;
            
            Tuple<string,string,string> v = filters.vierasJoukkue(vierasjoukkue);
            string vierasGroup = v.Item1;
            string vierasFilter = v.Item2;
            string vierasErittely = v.Item3;

            Tuple<string,string,string, string> l = filters.lukkari(lukkari);
            string lukkariGroup = l.Item1;
            string lukkariFilter = l.Item2;
            string lukkariErittely = l.Item3;
            string lukkariSelect = l.Item4;
            // string explain = "";
            // if ( STPT == "ST"){explain = "EXPLAIN QUERY PLAN";}
            // if ( STPT == "PT"){explain = "EXPLAIN";} 
            string query = $@"
            BEGIN;
            SELECT 
            tuomari Tuomari,
            COUNT(DISTINCT o.ottelu_id) Ottelut,
            {vuosittainErittely}
            {sarjavaiheErittely}
            {kotiErittely}
            {vierasErittely}
            {lukkariErittely}
            SUM(kp>vp)  Kotivoitto,
            SUM(kp<vp)  Vierasvoitto,
            ROUND( 100.0*SUM(kp>vp)/COUNT(DISTINCT o.ottelu_id),2)  `Kotivoitto-%`,
            ROUND( 1.0*SUM(k1j+k2j+v1j+v2j+ks+vs+kk+vk)/COUNT(DISTINCT o.ottelu_id),2) `juoksut/ott`,
            ROUND( 1.0*SUM(k1j+k2j+ks+kk)/COUNT(DISTINCT o.ottelu_id),2) `K-juoksut/ott`,
            ROUND( 1.0*SUM(v1j+v2j+vs+vk)/COUNT(DISTINCT o.ottelu_id),2) `V-juoksut/ott`,
            ROUND( 1.0*SUM( vttot) / COUNT( DISTINCT o.ottelu_id),2) `VT / ott`,
            ROUND( 1.0*SUM( vt ) / COUNT( DISTINCT o.ottelu_id), 2) `VT juoksut / ott`,
            ROUND( 1.0*SUM( vtk ) / COUNT( DISTINCT o.ottelu_id), 2) `VT juoksut koti`,
            ROUND( 1.0*SUM( vtv ) / COUNT( DISTINCT o.ottelu_id), 2) `VT juoksut vieras`,
            SUM(tuomari = pelituomari) PT,
            SUM(tuomari = syottotuomari) ST
            {vuosittainErittely2}

            FROM ottelu o
            INNER JOIN (SELECT DISTINCT tuomari FROM tuomari) t 
                ON tuomari = pelituomari OR tuomari = syottotuomari 
                AND o.kausi BETWEEN @vuosialkaen AND @vuosiloppuen
                AND tila != 'ottelu ei ole vielä alkanut'
                AND sarja = @sarja
            INNER JOIN (
                SELECT
                oo.ottelu_id ottelu_id,
                SUM(vt1+vt2+vt3+vt4) `vttot`,
                SUM(CASE WHEN joukkue_id = koti_id THEN vt1+vt2+vt3+vt4 END) `vttotk`,
                SUM(CASE WHEN joukkue_id = vieras_id THEN vt1+vt2+vt3+vt4 END) `vttotv`,
                SUM(vt4) `vt`,
                SUM(CASE WHEN joukkue_id = koti_id THEN vt4 END) `vtk`,
                SUM(CASE WHEN joukkue_id = vieras_id THEN vt4 END) `vtv`
                FROM ottelu oo 
                INNER JOIN ottelu_tilasto ot 
                    ON ot.ottelu_id = oo.ottelu_id
                    AND kausi BETWEEN @vuosialkaen AND @vuosiloppuen
                    AND tila != 'ottelu ei ole vielä alkanut'
                    AND sarja = @sarja
                GROUP BY oo.ottelu_id
            ) vt ON vt.ottelu_id = o.ottelu_id
            {lukkariSelect}
            WHERE 1
            {sarjavaiheFilter}
            {kotiFilter}
            {vierasFilter}
            {lukkariFilter}
            {STPTFilter}
            GROUP BY tuomari
            {vuosittainGroup}
            {kotiGroup}
            {vierasGroup}
            {lukkariGroup}
            {sarjavaiheGroup}
            ORDER BY 2 DESC
            ;
            COMMIT;
            ";

            SQLiteCommand dbCmd = _con.CreateCommand();
            dbCmd.CommandText = query;
            dbCmd.Parameters.AddWithValue("@vuosialkaen", vuosialkaen);
            dbCmd.Parameters.AddWithValue("@vuosiloppuen", vuosiloppuen);
            dbCmd.Parameters.AddWithValue("@kotijoukkue", kotijoukkue);
            dbCmd.Parameters.AddWithValue("@vierasjoukkue", vierasjoukkue);
            dbCmd.Parameters.AddWithValue("@lukkari", lukkari);
            dbCmd.Parameters.AddWithValue("@sarjavaihe", sarjavaihe);
            dbCmd.Parameters.AddWithValue("@sarja", sarja);

            return toteutaKysely(dbCmd);
        }
        public string apuJoukkueet(
            int vuosialkaen, 
            int vuosiloppuen,
            string sarja = "",
            string sarjavaihe = ""
        )
        {
            string sarjaFilter = filters.sarja(sarja).Item2;
            SQLiteCommand dbCmd = _con.CreateCommand();
            string query = $@"
            BEGIN;
            SELECT DISTINCT joukkue
            FROM ottelu o
            INNER JOIN joukkue j 
                ON kausi BETWEEN @vuosialkaen AND @vuosiloppuen
                AND (joukkue_id = koti_id OR joukkue_id = vieras_id)
                AND tila != 'ottelu ei ole vielä alkanut'
                {sarjaFilter}
            ORDER BY joukkue;
            COMMIT;
            ";
            dbCmd.CommandText = query;
            dbCmd.Parameters.AddWithValue("@vuosialkaen", vuosialkaen);
            dbCmd.Parameters.AddWithValue("@vuosiloppuen", vuosiloppuen);
            dbCmd.Parameters.AddWithValue("@sarja", sarja);

            return toteutaKysely(dbCmd);
        }
        public string apuVuodet()
        {
            SQLiteCommand dbCmd = _con.CreateCommand();
            string query = @"
            BEGIN;
            SELECT DISTINCT kausi
            FROM ottelu o
            WHERE tila != 'ottelu ei ole vielä alkanut'
            ORDER BY kausi;
            COMMIT;
            ";
            dbCmd.CommandText = query;

            return toteutaKysely(dbCmd);
        }
        public string apuSarjaVaiheet(
          int vuosialkaen=1994,
          int vuosiloppuen=2019)
        {
            SQLiteCommand dbCmd = _con.CreateCommand();
            string query = @"
            BEGIN;
            SELECT DISTINCT sarjajako sarjavaihe
            FROM ottelu o
            WHERE tila != 'ottelu ei ole vielä alkanut'
            AND kausi BETWEEN @vuosialkaen AND @vuosiloppuen
            GROUP BY sarjajako
            ORDER BY COUNT(ottelu_id) DESC;
            COMMIT;
            ";
            dbCmd.CommandText = query;
            dbCmd.Parameters.AddWithValue("@vuosialkaen", vuosialkaen);
            dbCmd.Parameters.AddWithValue("@vuosiloppuen", vuosiloppuen);

            return toteutaKysely(dbCmd);
        }
        public string apuSarjat(
          int vuosialkaen=1994,
          int vuosiloppuen=2019)
        {
            SQLiteCommand dbCmd = _con.CreateCommand();
            string query = @"
            BEGIN;
            SELECT DISTINCT sarja
            FROM ottelu o
            WHERE tila != 'ottelu ei ole vielä alkanut'
            AND kausi BETWEEN @vuosialkaen AND @vuosiloppuen
            ORDER BY sarja;
            COMMIT;
            ";
            dbCmd.CommandText = query;
            dbCmd.Parameters.AddWithValue("@vuosialkaen", vuosialkaen);
            dbCmd.Parameters.AddWithValue("@vuosiloppuen", vuosiloppuen);

            return toteutaKysely(dbCmd);
        }
        public string apuLyontiNumerot()
        {
            SQLiteCommand dbCmd = _con.CreateCommand();
            string query = @"
            BEGIN;
            SELECT DISTINCT pelaaja_nro
            FROM ottelu_tilasto ot
            INNER JOIN ottelu o 
                ON tila != 'ottelu ei ole vielä alkanut'
                AND o.ottelu_id = ot.ottelu_id;
            COMMIT;
            ";
            dbCmd.CommandText = query;

            return toteutaKysely(dbCmd);
        }
        public string apuUlkoPeliPaikat()
        {
            SQLiteCommand dbCmd = _con.CreateCommand();
            string query = @"
            BEGIN;
            SELECT DISTINCT upp
            FROM ottelu_tilasto ot, ottelu o
            WHERE tila != 'ottelu ei ole vielä alkanut'
            AND o.ottelu_id = ot.ottelu_id;
            COMMIT;

            ";
            dbCmd.CommandText = query;

            return toteutaKysely(dbCmd);
        }
        public string apuLukkarit(
            int vuosialkaen, 
            int vuosiloppuen,
            string sarja = "",
            string sarjavaihe = "")
        {
            string sarjaFilter = filters.sarja(sarja).Item2;
            SQLiteCommand dbCmd = _con.CreateCommand();
            string query = $@"
            BEGIN;
            SELECT DISTINCT
            nimi lukkari
            FROM ottelu_tilasto ot, pelaaja p, joukkue j, ottelu o
            WHERE ot.pelaaja_id = p.pelaaja_id
            AND ot.joukkue_id = j.joukkue_id
            AND o.ottelu_id = ot.ottelu_id
            AND kausi BETWEEN @vuosialkaen AND @vuosiloppuen
            AND upp='L'
            {sarjaFilter}
            GROUP by p.pelaaja_id
            ORDER by lukkari;
            COMMIT;

            ";
            dbCmd.CommandText = query;
            dbCmd.Parameters.AddWithValue("@vuosialkaen", vuosialkaen);
            dbCmd.Parameters.AddWithValue("@vuosiloppuen", vuosiloppuen);
            dbCmd.Parameters.AddWithValue("@sarja", sarja);

            return toteutaKysely(dbCmd);
        }
    }
}
