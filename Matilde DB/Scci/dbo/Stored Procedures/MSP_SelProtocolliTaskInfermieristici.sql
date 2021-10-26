CREATE PROCEDURE [dbo].[MSP_SelProtocolliTaskInfermieristici](@xParametri AS XML )
AS
BEGIN
	
	
				 
	DECLARE @sCodTipoTaskInfermieristico AS Varchar(20)
	DECLARE @sCodTipoProtocollo AS Varchar(20)
	DECLARE @sCodProtocollo AS Varchar(20)
	DECLARE @bContinuita AS BIT
				
		SET @sCodTipoTaskInfermieristico=(SELECT	TOP 1 ValoreParametro.CodTipoTaskInfermieristico.value('.','VARCHAR(20)')
					FROM @xParametri.nodes('/Parametri/CodTipoTaskInfermieristico') as ValoreParametro(CodTipoTaskInfermieristico))	
	IF ISNULL(@sCodTipoTaskInfermieristico,'')='' SET @sCodTipoTaskInfermieristico=NULL
	
		SET @sCodProtocollo=(SELECT	TOP 1 ValoreParametro.CodProtocollo.value('.','VARCHAR(20)')
					FROM @xParametri.nodes('/Parametri/CodProtocollo') as ValoreParametro(CodProtocollo)) 
	IF ISNULL(@sCodProtocollo,'')='' SET @sCodProtocollo=NULL	
	
		SET @sCodTipoProtocollo=(SELECT	TOP 1 ValoreParametro.CodTipoProtocollo.value('.','VARCHAR(20)')
					FROM @xParametri.nodes('/Parametri/CodTipoProtocollo') as ValoreParametro(CodTipoProtocollo)) 
	IF ISNULL(@sCodTipoProtocollo,'')='' SET @sCodTipoProtocollo=NULL				
	
		SET @bContinuita=(SELECT	TOP 1 ValoreParametro.Continuita.value('.','BIT')
					FROM @xParametri.nodes('/Parametri/Continuita') as ValoreParametro(Continuita)) 
	
			
	IF @sCodTipoTaskInfermieristico IS NULL
		BEGIN
												SELECT P.Codice,
				   P.Descrizione,
				   P.Continuita,
				   P.Durata,
				   P.CodTipoProtocollo
			FROM T_Protocolli P
			WHERE P.Codice=@sCodProtocollo AND
				  P.CodTipoProtocollo=ISNULL(@sCodTipoProtocollo,CodTipoProtocollo) AND 				  P.Continuita= ISNULL(@bContinuita,Continuita)											
		END
	ELSE
		BEGIN
												SELECT P.Codice,
				   P.Descrizione,
				   P.Continuita,
				   P.Durata,
				   P.CodTipoProtocollo
			FROM
				T_AssTipoTaskInfermieristicoProtocolli A
					INNER JOIN T_Protocolli P
						ON A.CodProtocollo =P.Codice
			WHERE
				A.CodTipoTaskInfermieristico=@sCodTipoTaskInfermieristico AND
				P.Codice=ISNULL(@sCodProtocollo,P.Codice)	AND										P.CodTipoProtocollo=ISNULL(@sCodTipoProtocollo,CodTipoProtocollo)	AND 				P.Continuita= ISNULL(@bContinuita,Continuita)									END	
	
	RETURN 0

END