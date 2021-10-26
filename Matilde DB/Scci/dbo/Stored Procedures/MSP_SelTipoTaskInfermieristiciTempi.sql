CREATE PROCEDURE [dbo].[MSP_SelTipoTaskInfermieristiciTempi](@xParametri AS XML )
AS
BEGIN
	
	
				 	
	DECLARE @sCodTipoTaskInfermieristico AS Varchar(20)
	DECLARE @sCodTipoTaskInfermieristicoTempi AS Varchar(20)
					
		SET @sCodTipoTaskInfermieristico=(SELECT	TOP 1 ValoreParametro.CodTipoTaskInfermieristico.value('.','VARCHAR(20)')
					FROM @xParametri.nodes('/Parametri/CodTipoTaskInfermieristico') as ValoreParametro(CodTipoTaskInfermieristico)) 
	IF ISNULL(@sCodTipoTaskInfermieristico,'')='' SET @sCodTipoTaskInfermieristico=NULL				
	
		SET @sCodTipoTaskInfermieristicoTempi=(SELECT	TOP 1 ValoreParametro.CodTipoTaskInfermieristicoTempi.value('.','VARCHAR(20)')
					FROM @xParametri.nodes('/Parametri/CodTipoTaskInfermieristicoTempi') as ValoreParametro(CodTipoTaskInfermieristicoTempi)) 
	IF ISNULL(@sCodTipoTaskInfermieristicoTempi,'')='' SET @sCodTipoTaskInfermieristicoTempi=NULL	
	
			
	SELECT A.CodTipoTaskInfermieristico,
		   P.Descrizione As DescTipoTaskInfermieristico,
		   A.CodTipoTaskInfermieristicoTempi,
		   PT.Descrizione AS DescTipoTaskInfermieristicoTempi
		FROM T_AssTipoTaskInfermieristicoTempi A 
			INNER JOIN T_TipoTaskInfermieristico P ON
				 A.CodTipoTaskInfermieristico = P.Codice
			INNER JOIN T_TipoTaskInfermieristicoTempi PT ON
				 A.CodTipoTaskInfermieristicoTempi = PT.Codice	 
		WHERE 
			A.CodTipoTaskInfermieristico=ISNULL(@sCodTipoTaskInfermieristico,A.CodTipoTaskInfermieristico)	AND
			A.CodTipoTaskInfermieristicoTempi=ISNULL(@sCodTipoTaskInfermieristicoTempi,A.CodTipoTaskInfermieristicoTempi)
	ORDER BY PT.Ordine
		

	RETURN 0
END