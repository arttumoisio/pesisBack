CREATE INDEX IF NOT EXISTS idx_o_ottelu_id ON ottelu (ottelu_id);
CREATE INDEX IF NOT EXISTS idx_o_koti_id ON ottelu (koti_id);
CREATE INDEX IF NOT EXISTS idx_o_vieras_id ON ottelu (vieras_id);

CREATE INDEX IF NOT EXISTS idx_po_ottelu_id ON puoli_ottelu (ottelu_id);
CREATE INDEX IF NOT EXISTS idx_po_koti_id ON puoli_ottelu (joukkue_id);
CREATE INDEX IF NOT EXISTS idx_po_vieras_id ON puoli_ottelu (vastustaja_id);

CREATE INDEX IF NOT EXISTS idx_p_pelaaja_id ON pelaaja (pelaaja_id);

CREATE INDEX IF NOT EXISTS idx_j_joukkue_id ON joukkue (joukkue_id);

CREATE INDEX IF NOT EXISTS idx_t_tuomari_id ON tuomari (tuomari_id);

CREATE INDEX IF NOT EXISTS idx_ot_ottelu_id ON ottelu_tilasto (ottelu_id);
CREATE INDEX IF NOT EXISTS idx_ot_ottelu_joukkue_id ON ottelu_tilasto (ottelu_id,joukkue_id);
CREATE INDEX IF NOT EXISTS idx_ot_joukkue_id ON ottelu_tilasto (joukkue_id);
CREATE INDEX IF NOT EXISTS idx_ot_pelaaja_id ON ottelu_tilasto (pelaaja_id);
CREATE INDEX IF NOT EXISTS idx_ot_ottelu_pelaaja_id ON ottelu_tilasto (ottelu_id, pelaaja_id);
