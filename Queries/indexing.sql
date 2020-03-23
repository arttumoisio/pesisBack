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
