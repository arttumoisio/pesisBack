using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace pesisBackend
{
    /// <summary>
    /// // TODO
    /// </summary>
    class DBQueries
    {
        private SqliteConnection _con;
        private const string DB_NAME = "pesisKantaHelmi.db";
        public DBQueries()
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

            var connectionStringBuilder = new SqliteConnectionStringBuilder();

            //Use DB in project directory.  If it does not exist, create it:
            connectionStringBuilder.DataSource = absolutePathToDb;

             _con = new SqliteConnection(connectionStringBuilder.ConnectionString);

            _con.Open();
        }

        /// <summary>
        /// // TODO
        /// </summary>
        public IList<List<String>> haeRivi(int vuosialkaen, int vuosiloppuen, bool vuosittain)
        {
            SqliteCommand dbCmd = _con.CreateCommand();
            dbCmd.CommandText = @"
            SELECT p.nimi Nimi, sum(ku) Kunnarit, sum(ly) Lyödyt, sum(ku+ly) Yhteensä, kausi
            FROM ottelu_tilasto ot, pelaaja p, ottelu oo
            WHERE ot.pelaaja_id = p.pelaaja_id 
            AND oo.ottelu_id = ot.ottelu_id
            AND kausi BETWEEN 2010 AND 2019
            GROUP BY ot.pelaaja_id, kausi
            ORDER BY Yhteensä DESC
            LIMIT 100;
            ";
            // dbCmd.Parameters.AddWithValue("@vuosi",)
            // if (vuosittain){
            //     dbCmd.Parameters.AddWithValue("@vuosi", ", vuosi");
            //     Console.WriteLine(1);
            // } else {
            //     dbCmd.Parameters.AddWithValue("@vuosi", " ");
            //     Console.WriteLine(2);
            // }
            //dbCmd.Parameters.AddWithValue("@vuosialkaen", vuosialkaen);
            //dbCmd.Parameters.AddWithValue("@vuosiloppuen", vuosiloppuen);

            Console.WriteLine(dbCmd.CommandText);

            IList<List<String>> data = new List<List<String>>();

            using (SqliteDataReader dr = dbCmd.ExecuteReader())
            {
              var rivi = new List<String>();
              for (var ii = 0; ii < dr.FieldCount; ii++)
                {
                  rivi.Add(dr.GetName(ii));
                  Console.WriteLine(dr.GetName(ii));
                }
              data.Add(rivi);

              while (dr.Read())
              {
                rivi = new List<String>();
                for (var i = 0; i < dr.FieldCount; i++)
                {
                 Console.WriteLine(dr.GetValue(i));
                 rivi.Add(dr.GetString(i));
                }
                data.Add(rivi);
              }
            }

            return data;
        }
        
    }
}
