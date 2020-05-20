using System;
using System.IO;
using System.Data.SQLite;
using System.Data;
using Newtonsoft.Json;

namespace pesisBackend
{
    class SQLiteQueries
    {
        private IDBHandler dBHandler;

        private Filters filters; 
        public SQLiteQueries()
        {
            dBHandler = new SQLiteDBHandler();
            filters = new Filters();
        }
        public string haePelaajat(
            ITeamParams teamParams
        )
        {            
            Modifier sarjajakoMod = filters.sarjajako(teamParams.sarjajako);
            Modifier kotiMod = filters.koti(teamParams.koti, false);
            Modifier tulosMod = filters.tulos(teamParams.tulos, false);
            Modifier vastustajaMod = filters.vastustaja(teamParams.vastustaja, false);
            Modifier joukkueMod = filters.joukkue(teamParams.joukkue);
                        
            string query = $@"
            BEGIN;
            SELECT  
            p.nimi Nimi,
            { (teamParams.vuosittain ? "kausi Kausi, " : "") }
            {joukkueMod.Erittely}
            {kotiMod.Erittely}
            {tulosMod.Erittely}
            {vastustajaMod.Erittely}
            {sarjajakoMod.Erittely}
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
            ROUND(100.0*SUM(
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
            SUM(pelaaja_nro = '10' OR pelaaja_nro = '11' OR pelaaja_nro = '12') 'jokeri',
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
            { (teamParams.vuosittain ? "" : "COUNT(DISTINCT kausi) Kaudet, ") }
            COUNT(DISTINCT joukkue) 'Joukkueet yht.',
            REPLACE( GROUP_CONCAT( DISTINCT joukkue), ',', ', ') Joukkueet

            FROM ottelu_tilasto ot 
            INNER JOIN ottelu o  ON 
                ot.ottelu_id = o.ottelu_id
                AND sarja = @sarja
                AND tila != 'ottelu ei ole vielä alkanut'
                AND kausi BETWEEN @kaudetAlku AND @kaudetLoppu
                {sarjajakoMod.Filter}
                {tulosMod.Filter}
                {vastustajaMod.Filter}
                {joukkueMod.Filter}
            INNER JOIN pelaaja p ON ot.pelaaja_id = p.pelaaja_id 
            INNER JOIN joukkue j 
                ON {kotiMod.Filter}
            GROUP BY ot.pelaaja_id
            { (teamParams.vuosittain ? ", kausi " : "") }
            {kotiMod.Group} 
            {tulosMod.Group}
            {vastustajaMod.Group}
            {sarjajakoMod.Group}
            {joukkueMod.Group}
            ORDER BY `Tehopist. yht` DESC;
            COMMIT;
            ";

            return dBHandler.ajaKyselyParametreilla(query, teamParams);
        }
        public string haeJoukkueet(
            ITeamParams teamParams
        )
        {

            string joukkueFilter = filters.joukkue(teamParams.joukkue).Filter;
            Modifier sarjajakoMod = filters.sarjajako(teamParams.sarjajako);
            Modifier kotiMod = filters.koti(teamParams.koti, true);
            Modifier tulosMod = filters.tulos(teamParams.tulos, true);
            Modifier vastustajaMod = filters.vastustaja(teamParams.vastustaja, true);

            string query = $@"
            BEGIN;
            SELECT 
            j.joukkue Joukkue,
            { (teamParams.vuosittain ? "kausi Kausi, " : "") }
            {sarjajakoMod.Erittely}
            {kotiMod.Erittely}
            {tulosMod.Erittely}
            {vastustajaMod.Erittely}
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
            { (teamParams.vuosittain ? "" : ", COUNT(DISTINCT kausi) Kaudet ") }

            FROM joukkue j 
            INNER JOIN puoli_ottelu po ON 
                po.joukkue_id = j.joukkue_id
                AND tila != 'ottelu ei ole vielä alkanut'
                AND sarja = @sarja
                AND kausi BETWEEN @kaudetAlku AND @kaudetLoppu
                {sarjajakoMod.Filter}
                {joukkueFilter}
                {kotiMod.Filter}
                {tulosMod.Filter}
                {vastustajaMod.Filter}
            GROUP BY j.joukkue
            { (teamParams.vuosittain ? ", kausi " : "") }
            {kotiMod.Group} 
            {tulosMod.Group}
            {vastustajaMod.Group}
            {sarjajakoMod.Group}
            ORDER BY Pisteet DESC;
            COMMIT;
            ";

            return dBHandler.ajaKyselyParametreilla(query, teamParams);
        }
        public string haeTuomarit(
            ITuomariParams tuomariParams
        )
        {
            string STPTFilter = filters.stpt(tuomariParams.STPT);
            Modifier sarjajakoMod = filters.sarjajako(tuomariParams.sarjajako);
            Modifier vuosittainMod = filters.vuosittain(tuomariParams.vuosittain);
            Modifier kotiMod = filters.kotiJoukkue(tuomariParams.kotijoukkue);
            Modifier vierasMod = filters.vierasJoukkue(tuomariParams.vierasjoukkue);
            Modifier lukkariMod = filters.lukkari(tuomariParams.lukkari);
            
            string query = $@"
            BEGIN;
            SELECT 
            tuomari Tuomari,
            COUNT(DISTINCT o.ottelu_id) Ottelut,
            {vuosittainMod.Erittely}
            {sarjajakoMod.Erittely}
            {kotiMod.Erittely}
            {vierasMod.Erittely}
            {lukkariMod.Erittely}
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
            {vuosittainMod.Erittely2}

            FROM ottelu o
            INNER JOIN (SELECT DISTINCT tuomari FROM tuomari) t 
                ON tuomari = pelituomari OR tuomari = syottotuomari 
                AND o.kausi BETWEEN @kaudetAlku AND @kaudetLoppu
                AND tila != 'ottelu ei ole vielä alkanut'
                AND sarja = @sarja
                {sarjajakoMod.Filter}
                {kotiMod.Filter}
                {vierasMod.Filter}
                {STPTFilter}
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
                    AND kausi BETWEEN @kaudetAlku AND @kaudetLoppu
                    AND tila != 'ottelu ei ole vielä alkanut'
                    AND sarja = @sarja
                GROUP BY oo.ottelu_id
            ) vt ON vt.ottelu_id = o.ottelu_id
            {lukkariMod.Filter}
            {lukkariMod.Select}
            
            GROUP BY tuomari
            {vuosittainMod.Group}
            {kotiMod.Group}
            {vierasMod.Group}
            {lukkariMod.Group}
            {sarjajakoMod.Group}
            ORDER BY 2 DESC
            ;
            COMMIT;
            ";

            return dBHandler.ajaKyselyParametreilla(query, tuomariParams);
        }
    }
}