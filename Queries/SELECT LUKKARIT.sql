SELECT
COUNT(ottelu_id),
COUNT(DISTINCT ottelu_id),
ottelu_id,
kotilukkari_id,
vieraslukkari_id,
kotijoukkue_id,
vierasjoukkue_id
FROM
(SELECT
po.ottelu_id ottelu_id,
otk.pelaaja_id `kotilukkari_id`,
otv.pelaaja_id `vieraslukkari_id`,
otk.joukkue_id kotijoukkue_id,
otv.joukkue_id vierasjoukkue_id
FROM ottelu po, ottelu_tilasto otk, ottelu_tilasto otv
WHERE 1
AND otk.joukkue_id = po.koti_id
AND otv.joukkue_id = po.vieras_id
AND po.ottelu_id = otk.ottelu_id AND po.ottelu_id = otv.ottelu_id
AND po.tila != 'ottelu ei ole vielä alkanut'
AND otk.upp = 'L' and 'L' = otv.upp
ORDER BY ottelu_id DESC) L, pelaaja v, pelaaja k
WHERE (k.pelaaja_id = kotilukkari_id) AND (v.pelaaja_id = vieraslukkari_id);

(SELECT
nimi lukkari,
p.pelaaja_id lukkari_id,
kotib,
po.ottelu_id ottelu_id
FROM puoli_ottelu po, ottelu_tilasto ot, pelaaja p
WHERE po.tila != 'ottelu ei ole vielä alkanut'
AND po.joukkue_id = ot.joukkue_id
AND ot.pelaaja_id = p.pelaaja_id
AND po.ottelu_id = ot.ottelu_id
AND ot.upp = 'L') l