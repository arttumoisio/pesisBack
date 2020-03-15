SELECT name FROM sqlite_master WHERE type == 'index';

DROP INDEX IF EXISTS idx_o_ottelu_id;
DROP INDEX IF EXISTS idx_o_koti_id;
DROP INDEX IF EXISTS idx_o_vieras_id;
DROP INDEX IF EXISTS idx_po_ottelu_id;
DROP INDEX IF EXISTS idx_po_koti_id;
DROP INDEX IF EXISTS idx_po_vieras_id;
DROP INDEX IF EXISTS idx_p_pelaaja_id;
DROP INDEX IF EXISTS idx_j_joukkue_id;
DROP INDEX IF EXISTS idx_t_tuomari_id;
DROP INDEX IF EXISTS idx_ot_ottelu_id;
DROP INDEX IF EXISTS idx_ot_joukkue_id;
DROP INDEX IF EXISTS idx_ot_pelaaja_id;
