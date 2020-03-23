SELECT
kotijoukkue,
vierasjoukkue,
ot1.upp,
ot2.upp,
ot1.pelaaja_nro,
ot2.pelaaja_nro,
SUM(ot1.vt4)+SUM(ot2.vt4) `Juoksut vapailla`,
SUM(ot1.vt4) `koti Juoksut vapailla`,
SUM(ot2.vt4) `vieras Juoksut vapailla`,
ROUND((SUM(ot1.vt4)+SUM(ot2.vt4))/COUNT(DISTINCT o.ottelu_id)*1.0,2)       `Vapaataipaleet/ott`,
ROUND(SUM(ot2.vt4)/COUNT(DISTINCT o.ottelu_id)*1.0,2)       `koti Vapaataipaleet/ott`,
ROUND(SUM(ot1.vt4)/COUNT(DISTINCT o.ottelu_id)*1.0,2)       `vierasVapaataipaleet/ott`,

SUM(ot1.kl1+ot1.kl2+ot1.kl3+ot1.kl4)+SUM(ot2.kl1+ot2.kl2+ot2.kl3+ot2.kl4)     `Kärkilyönnit`,
SUM(ot1.kl1+ot1.kl2+ot1.kl3+ot1.kl4)     `koti Kärkilyönnit`,
SUM(ot2.kl1+ot2.kl2+ot2.kl3+ot2.kl4)     `vieras Kärkilyönnit`,
ROUND((SUM(ot1.kl1+ot1.kl2+ot1.kl3+ot1.kl4)+SUM(ot2.kl1+ot2.kl2+ot2.kl3+ot2.kl4))/SUM(ot1.kl1+ot1.h1+ot1.p1+ot1.kl2+ot1.h2+ot1.p2+ot1.kl3+ot1.h3+ot1.p3+ot1.kl4+ot1.h4+ot1.p4)*100.0,2)    `Kärkilyönti-%`,
ROUND(SUM(ot1.kl1+ot1.kl2+ot1.kl3+ot1.kl4)/SUM(ot2.kl1+ot2.h1+ot2.p1+ot2.kl2+ot2.h2+ot2.p2+ot2.kl3+ot2.h3+ot2.p3+ot2.kl4+ot2.h4+ot2.p4)*100.0,2)     `Koti Kärkilyönnit`,
ROUND(SUM(ot2.kl1+ot2.kl2+ot2.kl3+ot2.kl4)/SUM(ot1.kl1+ot1.h1+ot1.p1+ot1.kl2+ot1.h2+ot1.p2+ot1.kl3+ot1.h3+ot1.p3+ot1.kl4+ot1.h4+ot1.p4+ot2.kl1+ot2.h1+ot2.p1+ot2.kl2+ot2.h2+ot2.p2+ot2.kl3+ot2.h3+ot2.p3+ot2.kl4+ot2.h4+ot2.p4)*100.0,2)     `Vieras Kärkilyönti-%`,
ROUND((SUM(ot1.kl1+ot1.kl2+ot1.kl3+ot1.kl4)+SUM(ot2.kl1+ot2.kl2+ot2.kl3+ot2.kl4))/COUNT(DISTINCT o.ottelu_id),2)    `Kärkilyönnit/ottelu`,
ROUND(SUM(ot1.kl1+ot1.kl2+ot1.kl3+ot1.kl4)/COUNT(DISTINCT o.ottelu_id),2)     `Koti Kärkilyönnit/ottelu`,
ROUND(SUM(ot2.kl1+ot2.kl2+ot2.kl3+ot2.kl4)/COUNT(DISTINCT o.ottelu_id),2)     `Vieras Kärkilyönnit/ottelu%`,

COUNT(ot1.ottelu_id) ott,
COUNT( DISTINCT ot1.ottelu_id) dist
FROM ottelu o
INNER JOIN ottelu_tilasto ot1 ON ot1.ottelu_id = o.ottelu_id AND ot1.joukkue_id = o.koti_id AND ot1.pelaaja_nro != 13
INNER JOIN ottelu_tilasto ot2 ON ot1.ottelu_id = ot2.ottelu_id AND ot2.joukkue_id = o.vieras_id AND ot1.pelaaja_nro = ot2.pelaaja_nro
WHERE o.ottelu_id = 17822
GROUP by o.ottelu_id, ot1.pelaaja_nro
ORDER BY ot1.pelaaja_nro+0, `Juoksut vapailla` DESC