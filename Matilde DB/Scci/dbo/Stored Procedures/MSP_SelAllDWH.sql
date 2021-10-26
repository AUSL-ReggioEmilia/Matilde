CREATE PROCEDURE MSP_SelAllDWH AS
BEGIN
SELECT 
    ALLDWH.CodDWH,
    ISNULL(SE.Codice, '') AS CodStatoEvidenzaClinica,
    SE.Descrizione
FROM 
    T_AllDWHStatoEvidenzaClinica ALLDWH LEFT JOIN T_StatoEvidenzaClinica SE ON
        ALLDWH.CodStatoEvidenzaClinica = SE.Codice 

SELECT
    ALLDWH.CodDWH,
    ISNULL(TE.Codice, '') AS CodTipoEvidenzaClinica,
    TE.Descrizione
FROM 
    T_AllDWHTipoEvidenzaClinica ALLDWH LEFT JOIN T_TipoEvidenzaClinica TE ON
        ALLDWH.CodTipoEvidenzaClinica = TE.Codice
END