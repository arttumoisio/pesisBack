using System;
using System.IO;
using System.Data.SQLite;
using System.Data;
using Newtonsoft.Json;

namespace pesisBackend
{
    class ApuQueries
    {
        private IDBHandler dBHandler;

        private Filters filters; 
        public ApuQueries()
        {
            dBHandler = new SQLiteDBHandler();
            filters = new Filters();
        }

        public string apuVuodet()
        {
            string query = @"
            BEGIN;
            SELECT DISTINCT kausi
            FROM ottelu o
            WHERE tila != 'ottelu ei ole vielä alkanut'
            ORDER BY kausi DESC;
            COMMIT;
            ";

            return dBHandler.ajaKysely(query);
        }
        public string apuSarjat(
          IBasicParams basicParams)
        {
            string query = @"
            BEGIN;
            SELECT DISTINCT sarja
            FROM ottelu o
            WHERE tila != 'ottelu ei ole vielä alkanut'
            AND kausi BETWEEN @kaudetAlku AND @kaudetLoppu
            ORDER BY sarja;
            COMMIT;
            ";

            return dBHandler.ajaKyselyParametreilla(query, basicParams);
        }
        public string apusarjajaot(
          IBasicParams basicParams)
        {
            string query = @"
            BEGIN;
            SELECT DISTINCT sarjajako sarjavaihe
            FROM ottelu o
            WHERE tila != 'ottelu ei ole vielä alkanut'
            AND kausi BETWEEN @kaudetAlku AND @kaudetLoppu
            GROUP BY sarjajako
            ORDER BY COUNT(ottelu_id) DESC;
            COMMIT;
            ";

            return dBHandler.ajaKyselyParametreilla(query, basicParams);
        }
        public string apuJoukkueet(
            IBasicParams basicParams,
            string koti = "")
        {
            
            string kotiFilter = "AND (joukkue_id = koti_id OR joukkue_id = vieras_id)";
            if (koti == "koti") { kotiFilter = "AND joukkue_id = koti_id"; }
            if (koti == "vieras") { kotiFilter = "AND joukkue_id = vieras_id"; }
            string sarjaFilter = filters.sarja(basicParams.sarja).Filter;
            string sarjajakoFilter = String.IsNullOrEmpty(basicParams.sarjajako) || basicParams.sarjajako == "Eritelty" ? "" : "AND sarjajako = @sarjajako";
            
            string query = $@"
            BEGIN;
            SELECT 
            joukkue,
            sum(koti_id=joukkue_id)>0 koti,
            sum(vieras_id=joukkue_id)>0 vieras
            FROM ottelu o
            INNER JOIN joukkue j 
                ON kausi BETWEEN @kaudetAlku AND @kaudetLoppu
                AND tila != 'ottelu ei ole vielä alkanut'
                {kotiFilter}
                {sarjaFilter}
                {sarjajakoFilter}
            GROUP BY joukkue
            ORDER BY joukkue;
            COMMIT;
            ";

            return dBHandler.ajaKyselyParametreilla(query, basicParams);
        }
        public string apuLukkarit(
            IBasicParams basicParams)
        {
            string sarjaFilter = filters.sarja(basicParams.sarja).Filter;
            string sarjajakoFilter = String.IsNullOrEmpty(basicParams.sarjajako) || basicParams.sarjajako == "Eritelty" ? "" : "AND sarjajako = @sarjajako";
            string query = $@"
            BEGIN;
            SELECT DISTINCT
            nimi lukkari
            FROM ottelu_tilasto ot 
            JOIN pelaaja p ON ot.pelaaja_id = p.pelaaja_id
            JOIN joukkue j ON  ot.joukkue_id = j.joukkue_id
            JOIN ottelu o ON o.ottelu_id = ot.ottelu_id 
            {sarjaFilter}
            {sarjajakoFilter}
            AND kausi BETWEEN @kaudetAlku AND @kaudetLoppu
            WHERE upp='L'
            GROUP by p.pelaaja_id
            ORDER by lukkari;
            COMMIT;

            ";

            return dBHandler.ajaKyselyParametreilla(query, basicParams);
        }
    }

}
