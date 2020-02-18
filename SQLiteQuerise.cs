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
        private const string DB_NAME = "pesisKantaHelmi.db";
        public SQLiteQueries()
        {
            string cwd = Directory.GetCurrentDirectory();
            DirectoryInfo dir = new DirectoryInfo(cwd);
            string absolutePathToDb = null;

            // Etsitään polku tietokannalle.
            while (dir.Parent != null)
            {
                System.Diagnostics.Debug.WriteLine(dir.Parent);
                if (File.Exists(dir.Parent + $"{Path.DirectorySeparatorChar}{DB_NAME}"))
                {
                    absolutePathToDb = dir.Parent.ToString() + $"{Path.DirectorySeparatorChar}{DB_NAME}";

                    break;
                }
                else dir = dir.Parent;
            }
            Console.WriteLine(absolutePathToDb);
            if (absolutePathToDb is null)
            {
                // TODO: Täytyy keskeyttää kaikki!
                System.Diagnostics.Debug.WriteLine("Database not found!");
            }

            var connectionStringBuilder = new SQLiteConnectionStringBuilder();

            //Use DB in project directory.  If it does not exist, create it:
            connectionStringBuilder.DataSource = absolutePathToDb;

            Console.WriteLine(connectionStringBuilder.ConnectionString);

             _con = new SQLiteConnection(connectionStringBuilder.ConnectionString);

            _con.Open();
        }

        /// <summary>
        /// // TODO
        /// </summary>
        public string haePelaajatVuosittain(int vuosialkaen, int vuosiloppuen)
        {
            string query = @"
            SELECT 
            ROW_NUMBER() OVER (ORDER BY sum(ku+ly) DESC) Sija, 
            p.nimi Nimi,
            sum(o) Ottelut,
            sum(ku+ly) Yhteensä,
            sum(ku) Kunnarit, 
            sum(ly) Lyödyt, 
            kausi Kausi
            FROM ottelu_tilasto ot, pelaaja p, ottelu oo
            WHERE ot.pelaaja_id = p.pelaaja_id 
            AND oo.ottelu_id = ot.ottelu_id
            AND kausi BETWEEN @vuosialkaen AND @vuosiloppuen
            GROUP BY ot.pelaaja_id, kausi
            ORDER BY Yhteensä DESC
            ";

            SQLiteCommand dbCmd = _con.CreateCommand();
            dbCmd.CommandText = query;
            dbCmd.Parameters.AddWithValue("@vuosialkaen", vuosialkaen);
            dbCmd.Parameters.AddWithValue("@vuosiloppuen", vuosiloppuen);
            
            return toteutaKysely(dbCmd);

        }
        public string haePelaajat(int vuosialkaen, int vuosiloppuen)
        {
            SQLiteCommand dbCmd = _con.CreateCommand();
            string query = @"
            SELECT 
            ROW_NUMBER() OVER (ORDER BY sum(ku+ly) DESC) Sija, 
            p.nimi Nimi,
            sum(o) Ottelut,
            sum(ku+ly) Yhteensä, 
            sum(ku) Kunnarit, 
            sum(ly) Lyödyt, 
            COUNT(DISTINCT kausi) Kaudet
            FROM ottelu_tilasto ot, pelaaja p, ottelu oo
            WHERE ot.pelaaja_id = p.pelaaja_id 
            AND oo.ottelu_id = ot.ottelu_id
            AND kausi BETWEEN @vuosialkaen AND @vuosiloppuen
            GROUP BY ot.pelaaja_id
            ORDER BY Yhteensä DESC
            ";
            dbCmd.CommandText = query;
            dbCmd.Parameters.AddWithValue("@vuosialkaen", vuosialkaen);
            dbCmd.Parameters.AddWithValue("@vuosiloppuen", vuosiloppuen);

            return toteutaKysely(dbCmd);
        }

        private string toteutaKysely(SQLiteCommand dbCmd)
        {
            Console.WriteLine(dbCmd.CommandText);
            DataTable dt = new DataTable();
            dt.Load(dbCmd.ExecuteReader());
            return JsonConvert.SerializeObject(dt);  
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
            k3p+v3p '3p voitot',
            k2p+v2p '2p voitot',
            k1p+v1p '1p tappiot',
            k0p+v0p '0p tappiot',
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
            COUNT(DISTINCT kausi) vk

            FROM ottelu o, joukkue j
            WHERE o.vieras_id = j.joukkue_id 
            AND kausi BETWEEN @vuosialkaen AND @vuosiloppuen
            AND tila != 'ottelu ei ole vielä alkanut'
            GROUP BY joukkue
            ) t2
            WHERE vj = kj
            AND  vj = @joukkue
            GROUP BY t1.kj
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
            SQLiteCommand dbCmd = _con.CreateCommand();
            string query = @"
            SELECT 
            kj Joukkue,
            ko+vo Ottelut,
            kp+vp Pisteet,
            k3p+v3p '3p voitot',
            k2p+v2p '2p voitot',
            k1p+v1p '1p tappiot',
            k0p+v0p '0p tappiot',
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
            kausi vk

            FROM ottelu o, joukkue j
            WHERE o.vieras_id = j.joukkue_id 
            AND kausi BETWEEN @vuosialkaen AND @vuosiloppuen
            AND tila != 'ottelu ei ole vielä alkanut'
            GROUP BY joukkue,kausi
            ) t2
            WHERE kk = vk AND kj = vj AND vj = @joukkue
            GROUP BY kj,kk
            ORDER BY Pisteet DESC
            ;
            ";
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
    }
}
