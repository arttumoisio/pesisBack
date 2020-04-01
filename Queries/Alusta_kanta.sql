DROP TABLE IF EXISTS puoli_ottelu;
CREATE TABLE puoli_ottelu (
  koti TEXT NOT NULL,
  kotib INTEGER NOT NULL,
  ottelu_id INTEGER NOT NULL,
  kausi TEXT NOT NULL,
  sarja TEXT NOT NULL,
  sarjajako TEXT NOT NULL,
  sarjavaihe TEXT NOT NULL,
  ottelu TEXT NOT NULL,
  tulos TEXT NOT NULL,
  p TEXT NOT NULL,
  `1j` TEXT NOT NULL,
  `2j` TEXT NOT NULL,
  s  TEXT NOT NULL,
  k  TEXT NOT NULL,
  `3p`  TEXT NOT NULL,
  `2p`  TEXT NOT NULL,
  `1p`  TEXT NOT NULL,
  `0p`  TEXT NOT NULL,
    vp TEXT NOT NULL,
  `v1j` TEXT NOT NULL,
  `v2j` TEXT NOT NULL,
  vs  TEXT NOT NULL,
  vk  TEXT NOT NULL,
  `v3p`  TEXT NOT NULL,
  `v2p`  TEXT NOT NULL,
  `v1p`  TEXT NOT NULL,
  `v0p`  TEXT NOT NULL,
  joukkue_id  INTEGER NOT NULL,
  joukkue TEXT NOT NULL,
  vastustaja_id INTEGER NOT NULL,
  vastustaja TEXT NOT NULL,
  tyyppi TEXT NOT NULL,
  aloittaja TEXT NOT NULL,
  svaloittaja TEXT NOT NULL,
  pvm TEXT NOT NULL,
  alk_aika TEXT NOT NULL,
  paat_aika TEXT NOT NULL,
  kentta TEXT,
  paikkakunta TEXT,
  katsojamaara TEXT NOT NULL,
  olosuhteet TEXT,
  pt_id INTEGER NOT NULL,
  pelituomari TEXT,
  st_id INTEGER NOT NULL,
  syottotuomari TEXT,
  `2-tuomari` TEXT,
  `3-tuomari` TEXT,
  takatuomari TEXT,
  kirjuri TEXT,
  tila TEXT NOT NULL,
  muuta TEXT,
  PRIMARY KEY (kotib,ottelu_id)
);
INSERT OR REPLACE INTO puoli_ottelu 
SELECT
  'koti',
  1,
  ottelu_id,
  kausi,
  sarja,
  sarjajako,
  sarjavaihe,
  ottelu,
  tulos,
  `kp`,
  `k1j`,
  `k2j`,
  `ks`,
  `kk`,
  `k3p`,
  `k2p`,
  `k1p`,
  `k0p`,
  `vp`,
  `v1j`,
  `v2j`,
  `vs`,
  `vk`,
  `v3p`,
  `v2p`,
  `v1p`,
  `v0p`,
  koti_id,
  kotijoukkue,
  vieras_id,
  vierasjoukkue,
  tyyppi,
  aloittaja=1,
  svaloittaja=1,
  pvm,
  alk_aika,
  paat_aika,
  kentta,
  paikkakunta,
  katsojamaara,
  olosuhteet,
  pt_id,
  pelituomari,
  st_id,
  syottotuomari,
  `2-tuomari`,
  `3-tuomari`,
  takatuomari,
  kirjuri,
  tila,
  muuta   
  

FROM ottelu
WHERE tila != 'ottelu ei ole vielä alkanut';

INSERT OR REPLACE INTO puoli_ottelu 
SELECT
  'vieras',
  0,
  ottelu_id,
  kausi,
  sarja,
  sarjajako,
  sarjavaihe,
  ottelu,
  tulos,
  `vp`,
  `v1j`,
  `v2j`,
  `vs`,
  `vk`,
  `v3p`,
  `v2p`,
  `v1p`,
  `v0p`,
  `kp`,
  `k1j`,
  `k2j`,
  `ks`,
  `kk`,
  `k3p`,
  `k2p`,
  `k1p`,
  `k0p`,
  vieras_id,
  vierasjoukkue,
  koti_id,
  kotijoukkue,
  tyyppi,
  aloittaja=2,
  svaloittaja=2,
  pvm,
  alk_aika,
  paat_aika,
  kentta,
  paikkakunta,
  katsojamaara,
  olosuhteet,
  pt_id,
  pelituomari,
  st_id,
  syottotuomari,
  `2-tuomari`,
  `3-tuomari`,
  takatuomari,
  kirjuri,
  tila,
  muuta   
  

FROM ottelu
WHERE tila != 'ottelu ei ole vielä alkanut';

DROP TABLE IF EXISTS tuomari;
CREATE TABLE IF NOT EXISTS tuomari(
    tuomari_id INTEGER NOT NULL PRIMARY KEY,
    tuomari TEXT NOT NULL
);

INSERT OR IGNORE INTO tuomari
SELECT DISTINCT 
pt_id,
pelituomari
FROM ottelu;

INSERT OR IGNORE INTO tuomari
SELECT DISTINCT 
st_id,
syottotuomari
FROM ottelu;

CREATE INDEX IF NOT EXISTS idx_o_ottelu_id              ON ottelu (ottelu_id);
CREATE INDEX IF NOT EXISTS idx_o_koti_id                ON ottelu (koti_id);
CREATE INDEX IF NOT EXISTS idx_o_vieras_id              ON ottelu (vieras_id);
CREATE INDEX IF NOT EXISTS idx_o_kotijoukkue            ON ottelu (kotijoukkue);
CREATE INDEX IF NOT EXISTS idx_o_vierasjoukkue          ON ottelu (vierasjoukkue);
CREATE INDEX IF NOT EXISTS idx_o_pelituomari            ON ottelu (pelituomari);
CREATE INDEX IF NOT EXISTS idx_o_syottotuomari          ON ottelu (syottotuomari);
CREATE INDEX IF NOT EXISTS idx_o_pelituomari_i          ON ottelu (ottelu_id,pelituomari);
CREATE INDEX IF NOT EXISTS idx_o_syottotuomari_         ON ottelu (ottelu_id,syottotuomari);
CREATE INDEX IF NOT EXISTS idx_o_tila                   ON ottelu (tila);
CREATE INDEX IF NOT EXISTS idx_o_composite              ON ottelu (ottelu_id,syottotuomari,pelituomari,kausi,tila);
CREATE INDEX IF NOT EXISTS idx_o_composite2             ON ottelu (ottelu_id,pelituomari,syottotuomari,kausi,tila,kotijoukkue,vierasjoukkue);
CREATE INDEX IF NOT EXISTS idx_o_composite2             ON ottelu (ottelu_id,pelituomari,syottotuomari,kausi,tila,kotijoukkue,vierasjoukkue);

CREATE INDEX IF NOT EXISTS idx_po_ottelu_id             ON puoli_ottelu (ottelu_id);
CREATE INDEX IF NOT EXISTS idx_po_kotijoukkue           ON puoli_ottelu (joukkue);
CREATE INDEX IF NOT EXISTS idx_po_vieras_joukkue        ON puoli_ottelu (vastustaja);
CREATE INDEX IF NOT EXISTS idx_po_koti_id               ON puoli_ottelu (joukkue_id);
CREATE INDEX IF NOT EXISTS idx_po_vieras_id             ON puoli_ottelu (vastustaja_id);
CREATE INDEX IF NOT EXISTS idx_po_pelituomari           ON puoli_ottelu (pelituomari);
CREATE INDEX IF NOT EXISTS idx_po_syottotuomari         ON puoli_ottelu (syottotuomari);
CREATE INDEX IF NOT EXISTS idx_po_pelituomari_i         ON puoli_ottelu (ottelu_id,pelituomari);
CREATE INDEX IF NOT EXISTS idx_po_syottotuomari_        ON puoli_ottelu (ottelu_id,syottotuomari);
CREATE INDEX IF NOT EXISTS idx_po_tila                  ON puoli_ottelu (tila);

CREATE INDEX IF NOT EXISTS idx_p_pelaaja_id             ON pelaaja (pelaaja_id);
CREATE INDEX IF NOT EXISTS idx_p_nimi                   ON pelaaja (nimi);
CREATE INDEX IF NOT EXISTS idx_p_nimi_id                ON pelaaja (pelaaja_id, nimi);

CREATE INDEX IF NOT EXISTS idx_j_joukkue_id             ON joukkue (joukkue_id);
CREATE INDEX IF NOT EXISTS idx_j_joukkue_             ON joukkue (joukkue);
CREATE INDEX IF NOT EXISTS idx_j_joukkue_comp             ON joukkue (joukkue_id,joukkue);

CREATE INDEX IF NOT EXISTS idx_t_tuomari_id             ON tuomari (tuomari_id);
CREATE INDEX IF NOT EXISTS idx_t_tuomari                ON tuomari (tuomari);
CREATE INDEX IF NOT EXISTS idx_t_tuomari_comp           ON tuomari (tuomari_id, tuomari);
CREATE INDEX IF NOT EXISTS idx_t_tuomari_comp2           ON tuomari (tuomari, tuomari_id);

CREATE INDEX IF NOT EXISTS idx_ot_ottelu_id             ON ottelu_tilasto (ottelu_id);
CREATE INDEX IF NOT EXISTS idx_ot_ottelu_joukkue_id     ON ottelu_tilasto (ottelu_id,joukkue_id);
CREATE INDEX IF NOT EXISTS idx_ot_joukkue_id            ON ottelu_tilasto (joukkue_id,pelaaja_id);
CREATE INDEX IF NOT EXISTS idx_ot_pelaaja_id            ON ottelu_tilasto (pelaaja_id);
CREATE INDEX IF NOT EXISTS idx_ot_ottelu_pelaaja_id     ON ottelu_tilasto (pelaaja_id, ottelu_id);
CREATE INDEX IF NOT EXISTS idx_ot_ottelu_upp            ON ottelu_tilasto (pelaaja_id, upp);
CREATE INDEX IF NOT EXISTS idx_ot_upp                   ON ottelu_tilasto (upp);
CREATE INDEX IF NOT EXISTS idx_ot_ottelu_composite      ON ottelu_tilasto (joukkue_id,pelaaja_id,ottelu_id,upp);
