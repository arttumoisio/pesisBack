using System;
using System.IO;
using System.Data.SQLite;
using System.Data;
using Newtonsoft.Json;

namespace pesisBackend
{
    class SQLiteDBHandler : IDBHandler
    {

        private SQLiteConnection _con;
        private const string DB_NAME = "pesistk.db";

        public SQLiteDBHandler()
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
        private string toteutaKysely(SQLiteCommand dbCmd){
             
            Console.WriteLine(dbCmd.CommandText);
            DataTable dt = new DataTable();
            dt.Load(dbCmd.ExecuteReader());
            return JsonConvert.SerializeObject(dt); 

        }

        public string ajaKyselyParametreilla(string query, IBasicParams basicParams){
            SQLiteCommand dbCmd = luoKomento(query);
            addBasicParams(basicParams, dbCmd);
            return toteutaKysely(dbCmd);
        }
        public string ajaKyselyParametreilla(string query, ITuomariParams tuomariParams){
            SQLiteCommand dbCmd = luoKomento(query);
            addTuomariParams(tuomariParams, dbCmd);
            return toteutaKysely(dbCmd);
        }
        public string ajaKyselyParametreilla(string query, ITeamParams teamParams){
            SQLiteCommand dbCmd = luoKomento(query);
            addTeamParams(teamParams, dbCmd);
            return toteutaKysely(dbCmd);
        }
        public string ajaKysely(string query){
            SQLiteCommand dbCmd = luoKomento(query);
            return toteutaKysely(dbCmd);
        }

        private SQLiteCommand luoKomento(string query)
        {
            SQLiteCommand dbCmd = _con.CreateCommand();
            dbCmd.CommandText = query;
            return dbCmd;
        }
        private static void addTeamParams(ITeamParams teamParams, SQLiteCommand dbCmd)
        {
            addBasicParams(teamParams, dbCmd);
            dbCmd.Parameters.AddWithValue("@joukkue", teamParams.joukkue);
            dbCmd.Parameters.AddWithValue("@vastustaja", teamParams.vastustaja);
        }
        private static void addTuomariParams(ITuomariParams tuomariParams, SQLiteCommand dbCmd)
        {
            addBasicParams(tuomariParams, dbCmd);
            dbCmd.Parameters.AddWithValue("@kotijoukkue", tuomariParams.kotijoukkue);
            dbCmd.Parameters.AddWithValue("@vierasjoukkue", tuomariParams.vierasjoukkue);
            dbCmd.Parameters.AddWithValue("@lukkari", tuomariParams.lukkari);
        }

        private static void addBasicParams(IBasicParams basicParams, SQLiteCommand dbCmd)
        {
            dbCmd.Parameters.AddWithValue("@kaudetAlku", basicParams.kaudetAlku);
            dbCmd.Parameters.AddWithValue("@kaudetLoppu", basicParams.kaudetLoppu);
            dbCmd.Parameters.AddWithValue("@sarja", basicParams.sarja);
            dbCmd.Parameters.AddWithValue("@sarjajako", basicParams.sarjajako);
        }
    }
}