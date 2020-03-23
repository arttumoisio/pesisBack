SELECT name FROM sqlite_master WHERE type == 'index';

DROP INDEX IF EXISTS idx_o_ottelu_id  ;
DROP INDEX IF EXISTS idx_o_koti_id  ;
DROP INDEX IF EXISTS idx_o_vieras_id  ;
DROP INDEX IF EXISTS idx_o_kotijoukkue  ;
DROP INDEX IF EXISTS idx_o_vierasjoukkue  ;
DROP INDEX IF EXISTS idx_o_pelituomari  ;
DROP INDEX IF EXISTS idx_o_syottotuomari  ;
DROP INDEX IF EXISTS idx_o_pelituomari_i          ;
DROP INDEX IF EXISTS idx_o_syottotuomari_         ;
DROP INDEX IF EXISTS idx_o_tila                   ;
DROP INDEX IF EXISTS idx_o_composite                ;
DROP INDEX IF EXISTS idx_o_composite2               ;

DROP INDEX IF EXISTS idx_po_ottelu_id       ;
DROP INDEX IF EXISTS idx_po_kotijoukkue   ;
DROP INDEX IF EXISTS idx_po_vieras_joukkue   ;
DROP INDEX IF EXISTS idx_po_koti_id          ;
DROP INDEX IF EXISTS idx_po_vieras_id           ;
DROP INDEX IF EXISTS idx_po_pelituomari       ;
DROP INDEX IF EXISTS idx_po_syottotuomari       ;
DROP INDEX IF EXISTS idx_po_pelituomari_i        ;
DROP INDEX IF EXISTS idx_po_syottotuomari_       ;
DROP INDEX IF EXISTS idx_po_tila       ;

DROP INDEX IF EXISTS idx_p_pelaaja_id   ;
DROP INDEX IF EXISTS idx_p_nimi                   ;
DROP INDEX IF EXISTS idx_p_nimi_id            ;

DROP INDEX IF EXISTS idx_j_joukkue_id   ;
DROP INDEX IF EXISTS idx_j_joukkue_ ;
DROP INDEX IF EXISTS idx_j_joukkue_comp           ;

DROP INDEX IF EXISTS idx_t_tuomari_id   ;
DROP INDEX IF EXISTS idx_t_tuomari   ;
DROP INDEX IF EXISTS idx_t_tuomari_comp         ;

DROP INDEX IF EXISTS idx_ot_ottelu_id         ;
DROP INDEX IF EXISTS idx_ot_ottelu_joukkue_id   ;
DROP INDEX IF EXISTS idx_ot_joukkue_id            ;
DROP INDEX IF EXISTS idx_ot_pelaaja_id         ;
DROP INDEX IF EXISTS idx_ot_ottelu_pelaaja_id     ;
DROP INDEX IF EXISTS idx_ot_ottelu_upp            ;
DROP INDEX IF EXISTS idx_ot_upp;
DROP INDEX IF EXISTS idx_ot_ottelu_composite     ;
