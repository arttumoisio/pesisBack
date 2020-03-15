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