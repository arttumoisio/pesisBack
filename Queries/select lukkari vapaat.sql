SELECT
lukk.pelaaja_id lukkari_id,
lukk.lukkari lukkari,
ot.ottelu_id,
ot.joukkue_id
FROM ottelu_tilasto ot
INNER JOIN (SELECT nimi lukkari, otl.pelaaja_id, otl.ottelu_id, otl.joukkue_id 
            FROM ottelu_tilasto otl 
            INNER JOIN pelaaja p ON p.pelaaja_id = otl.pelaaja_id 
            WHERE upp = 'L') lukk on lukk.ottelu_id = ot.ottelu_id AND lukk.joukkue_id = ot.joukkue_id
WHERE pelaaja_nro != 0
AND ot.ottelu_id = 17822
GROUP BY ot.ottelu_id, ot.joukkue_id
