CREATE PROCEDURE [dbo].[MSP_SelTipoPrescrizioniTempi](@xParametri AS XML )
AS
BEGIN
	
	
				 	
	DECLARE @sCodTipoPrescrizione AS Varchar(20)
	DECLARE @sCodTipoPrescrizioneTempi AS Varchar(20)
					
		SET @sCodTipoPrescrizione=(SELECT	TOP 1 ValoreParametro.CodTipoPrescrizione.value('.','VARCHAR(20)')
					FROM @xParametri.nodes('/Parametri/CodTipoPrescrizione') as ValoreParametro(CodTipoPrescrizione)) 
	IF ISNULL(@sCodTipoPrescrizione,'')='' SET @sCodTipoPrescrizione=NULL				
	
		SET @sCodTipoPrescrizioneTempi=(SELECT	TOP 1 ValoreParametro.CodTipoPrescrizioneTempi.value('.','VARCHAR(20)')
					FROM @xParametri.nodes('/Parametri/CodTipoPrescrizioneTempi') as ValoreParametro(CodTipoPrescrizioneTempi)) 
	IF ISNULL(@sCodTipoPrescrizioneTempi,'')='' SET @sCodTipoPrescrizioneTempi=NULL	
	
			
	SELECT A.CodTipoPrescrizione,
		   P.Descrizione As DescTipoPrescrizione,
		   A.CodTipoPrescrizioneTempi,
		   PT.Descrizione AS DescTipoPrescrizioneTempi
		FROM T_AssTipoPrescrizioneTempi A 
			INNER JOIN T_TipoPrescrizione P ON
				 A.CodTipoPrescrizione = P.Codice
			INNER JOIN T_TipoPrescrizioneTempi PT ON
				 A.CodTipoPrescrizioneTempi = PT.Codice	 
		WHERE 
			A.CodTipoPrescrizione=ISNULL(@sCodTipoPrescrizione,A.CodTipoPrescrizione)	AND
			A.CodTipoPrescrizioneTempi=ISNULL(@sCodTipoPrescrizioneTempi,A.CodTipoPrescrizioneTempi)
	ORDER BY PT.Ordine
		

	RETURN 0
END