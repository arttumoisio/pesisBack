select
*
FROM
(SELECT 
vp,
kp,
t1.koti kkoti,
t2.koti vkoti

            FROM
            (
            SELECT 
            joukkue kj,
            'koti' koti,
            koti_id k_id,
            vieras_id kv_id,
            COUNT(*) ko,
            SUM(kp) kp,
            SUM(k3p) 'k3p',
            SUM(k2p) 'k2p',
            SUM(k1p) 'k1p',
            SUM(k0p) 'k0p',
            SUM(k1j+k2j+ks) kju,
            SUM(v1j+v2j+vs) kpä,
            kausi kk

            FROM ottelu o, joukkue j
            WHERE o.koti_id = j.joukkue_id
            AND tila != 'ottelu ei ole vielä alkanut'
            GROUP BY joukkue_id, kausi
            ) t1,
            (
            SELECT 
            joukkue vj,
            'vieras' koti,
            koti_id v_id,
            vieras_id vv_id,
            COUNT(*) vo,
            SUM(vp) vp,
            SUM(v3p) 'v3p',
            SUM(v2p) 'v2p',
            SUM(v1p) 'v1p',
            SUM(v0p) 'v0p',
            SUM(v1j+v2j+vs) vju,
            SUM(k1j+k2j+ks) vpä,
            kausi vk

            FROM ottelu o, joukkue j
            WHERE o.vieras_id = j.joukkue_id 
            AND tila != 'ottelu ei ole vielä alkanut'
            GROUP BY joukkue_id,kausi
            ) t2
            WHERE kk = vk AND v_id = k_id 
            AND vk BETWEEN 2010 AND 2020
            AND kk BETWEEN 2010 AND 2020
            GROUP BY kj,kk
            ORDER BY vp+kp DESC
) t3
GROUP by kkoti, vkoti