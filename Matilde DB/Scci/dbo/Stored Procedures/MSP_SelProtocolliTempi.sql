CREATE PROCEDURE [dbo].[MSP_SelProtocolliTempi](@xParametri AS XML )
AS
BEGIN
	
	
				 	
	DECLARE @sCodProtocollo AS Varchar(20)
	DECLARE @sCodice AS Varchar(30)
					
		SET @sCodProtocollo=(SELECT	TOP 1 ValoreParametro.CodProtocollo.value('.','VARCHAR(20)')
					FROM @xParametri.nodes('/Parametri/CodProtocollo') as ValoreParametro(CodProtocollo)) 
	IF ISNULL(@sCodProtocollo,'')='' SET @sCodProtocollo=NULL				
	
		SET @sCodice	=(SELECT	TOP 1 ValoreParametro.Codice.value('.','VARCHAR(30)')
					FROM @xParametri.nodes('/Parametri/Codice') as ValoreParametro(Codice)) 
	IF ISNULL(@sCodice,'')='' SET @sCodice=NULL
	
			
	SELECT * FROM 
		(SELECT 			
				T.CodProtocollo,
				P.Descrizione AS DescProtocollo,
				T.Codice,			
				T.Descrizione AS DescTempo,
				T.Delta,
				P.CodTipoProtocollo,
				CASE 
					WHEN T.Ora IS NOT NULL  THEN 
							CONVERT(DATETIME, 
									'1900-01-01 ' + RIGHT('00' + CONVERT(VARCHAR(2),DATEPART(HOUR,T.Ora)),2) 
									+ ':' + RIGHT(CONVERT(VARCHAR(2),DATEPART(MINUTE,T.Ora)),2)
									,120
									)
					ELSE NULL
				END AS ORA			
		FROM T_ProtocolliTempi T INNER JOIN T_Protocolli P ON
				T.CodProtocollo = P.Codice
		WHERE 
			T.CodProtocollo=ISNULL(@sCodProtocollo,T.CodProtocollo)	AND
			T.Codice=ISNULL(@sCodice,T.Codice)
		) AS Q
	ORDER BY Delta,Ora	
		

	RETURN 0
END