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
            j.joukkue Joukkue,
            kausi Kausi,
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
            SUM(INSTR(tulos,'s')>0)+SUM(INSTR(tulos,'k')>0)-SUM(svaloittaja) 'Aloittava ulkovuoro superissa'
            FROM puoli_ottelu po, joukkue j
            WHERE po.joukkue_id = j.joukkue_id
            AND kausi BETWEEN @vuosialkaen AND @vuosiloppuen
            AND tila != 'ottelu ei ole vielä alkanut'
            {joukkueFilter}
            {kotiFilter}
            {tulosFilter}
            {vastustajaFilter}
            GROUP BY po.joukkue_id ,kausi
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
            j.joukkue Joukkue,
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
            COUNT(DISTINCT kausi) Kaudet
            FROM puoli_ottelu po, joukkue j
            WHERE po.joukkue_id = j.joukkue_id
            AND kausi BETWEEN @vuosialkaen AND @vuosiloppuen
            AND tila != 'ottelu ei ole vielä alkanut'
            {joukkueFilter}
            {kotiFilter}
            {tulosFilter}
            {vastustajaFilter}
            GROUP BY j.joukkue_id
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
        public string haeTuomarit(
            int vuosialkaen, 
            int vuosiloppuen,
            bool vuosittain,
            string kotijoukkue,
            string vierasjoukkue,
            string lukkari,
            string STPT
        )
        {   
            string STPTFilter = filters.stpt(STPT);

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

            string query = $@"
            SELECT 
            tuomari Tuomari,
            COUNT(o.ottelu_id) Ottelut,
            {vuosittainErittely}
            {kotiErittely}
            {vierasErittely}
            {lukkariErittely}
            SUM(kp>vp)  Kotivoitto,
            SUM(kp<vp)  Vierasvoitto,
            ROUND(100.0*SUM(kp>vp)/COUNT(o.ottelu_id),2)  `Kotivoitto-%`,
            ROUND(1.0*SUM(k1j+k2j+v1j+v2j+ks+vs+kk+vk)/COUNT(o.ottelu_id),2) `juoksut/ott`,
            ROUND(1.0*SUM(k1j+k2j+ks+kk)/COUNT(o.ottelu_id),2) `K-juoksut/ott`,
            ROUND(1.0*SUM(v1j+v2j+vs+vk)/COUNT(o.ottelu_id),2) `V-juoksut/ott`,
            1 `Vapaataipaleet/ott`,
            1 `Kärkilyönnit`,
            1 `Kärkilyönti-%`,
            SUM(tuomari = pelituomari) PT,
            SUM(tuomari = syottotuomari) ST
            {vuosittainErittely2}

            FROM tuomari t, ottelu o {lukkariSelect}
            WHERE (tuomari = syottotuomari OR tuomari = pelituomari)
            AND kausi BETWEEN @vuosialkaen AND @vuosiloppuen
            AND tila != 'ottelu ei ole vielä alkanut'
            {kotiFilter}
            {vierasFilter}
            {lukkariFilter}
            {STPTFilter}
            GROUP BY tuomari
            {vuosittainGroup}
            {kotiGroup}
            {vierasGroup}
            {lukkariGroup}
            ORDER BY Ottelut DESC;";

            SQLiteCommand dbCmd = _con.CreateCommand();
            dbCmd.CommandText = query;
            dbCmd.Parameters.AddWithValue("@vuosialkaen", vuosialkaen);
            dbCmd.Parameters.AddWithValue("@vuosiloppuen", vuosiloppuen);
            dbCmd.Parameters.AddWithValue("@kotijoukkue", kotijoukkue);
            dbCmd.Parameters.AddWithValue("@vierasjoukkue", vierasjoukkue);
            dbCmd.Parameters.AddWithValue("@lukkari", lukkari);

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
            AND tila != 'ottelu ei ole vielä alkanut'
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
            WHERE tila != 'ottelu ei ole vielä alkanut'
            ORDER BY kausi
            ";
            dbCmd.CommandText = query;

            return toteutaKysely(dbCmd);
        }
        public string apuSarjaVaiheet()
        {
            SQLiteCommand dbCmd = _con.CreateCommand();
            string query = @"
            SELECT DISTINCT sarjavaihe
            FROM ottelu o
            WHERE tila != 'ottelu ei ole vielä alkanut'
            ";
            dbCmd.CommandText = query;

            return toteutaKysely(dbCmd);
        }
        public string apuLyontiNumerot()
        {
            SQLiteCommand dbCmd = _con.CreateCommand();
            string query = @"
            SELECT DISTINCT pelaaja_nro
            FROM ottelu_tilasto ot, ottelu o
            WHERE tila != 'ottelu ei ole vielä alkanut'
            AND o.ottelu_id = ot.ottelu_id

            ";
            dbCmd.CommandText = query;

            return toteutaKysely(dbCmd);
        }
        public string apuUlkoPeliPaikat()
        {
            SQLiteCommand dbCmd = _con.CreateCommand();
            string query = @"
            SELECT DISTINCT upp
            FROM ottelu_tilasto ot, ottelu o
            WHERE tila != 'ottelu ei ole vielä alkanut'
            AND o.ottelu_id = ot.ottelu_id

            ";
            dbCmd.CommandText = query;

            return toteutaKysely(dbCmd);
        }
        public string apuLukkarit(
            int vuosialkaen, 
            int vuosiloppuen)
        {
            SQLiteCommand dbCmd = _con.CreateCommand();
            string query = @"
            SELECT DISTINCT
            nimi lukkari
            FROM ottelu_tilasto ot, pelaaja p, joukkue j, ottelu o
            WHERE ot.pelaaja_id = p.pelaaja_id
            AND ot.joukkue_id = j.joukkue_id
            AND o.ottelu_id = ot.ottelu_id
            AND kausi BETWEEN @vuosialkaen AND @vuosiloppuen
            AND upp='L'
            GROUP by p.pelaaja_id
            ORDER by COUNT(*) desc

            ";
            dbCmd.CommandText = query;
            dbCmd.Parameters.AddWithValue("@vuosialkaen", vuosialkaen);
            dbCmd.Parameters.AddWithValue("@vuosiloppuen", vuosiloppuen);

            return toteutaKysely(dbCmd);
        }
    }
}
