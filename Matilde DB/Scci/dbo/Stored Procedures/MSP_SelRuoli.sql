CREATE PROCEDURE [dbo].[MSP_SelRuoli](@xParametri XML)
AS
BEGIN

	
	
	
	
				 	
	
	DECLARE @sCodLogin AS VARCHAR(100)
	DECLARE @bDatiEstesi AS Bit		
	
	SET @sCodLogin=(SELECT TOP 1 ValoreParametro.CodLogin.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/CodLogin') as ValoreParametro(CodLogin))	
										
	SET @bDatiEstesi=(SELECT TOP 1 ValoreParametro.DatiEstesi.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/DatiEstesi') as ValoreParametro(DatiEstesi))	

				
			IF  ISNULL(@bDatiEstesi,0)=0 
		BEGIN
				SELECT	
					R.Codice, 
					R.Descrizione,
					R.CodTipoDiario,
					0 AS  DaFirmare,
					ISNULL(R.NumMaxCercaEpi,0) AS NumMaxCercaEpi,
					ISNULL(R.RichiediPassword,0) AS RichiediPassword,
					ISNULL(R.LimitaEVCAmbulatoriale,0) AS LimitaEVCAmbulatoriale
				FROM 
					T_AssLoginRuoli A
						INNER JOIN T_Ruoli R
							 ON A.CodRuolo = R.Codice	
				WHERE 
					A.CodLogin=@sCodLogin
				ORDER BY Descrizione					
		END 						  
	ELSE
		BEGIN
						DECLARE @nSec INTEGER
			SET @nSec= (SELECT datepart(second,getdate()))			
			
			SELECT	
					R.Codice, 
					R.Descrizione,
					R.CodTipoDiario,
					0 AS  DaFirmare,
					ISNULL(R.NumMaxCercaEpi,0) AS NumMaxCercaEpi,
					ISNULL(R.RichiediPassword,0) AS RichiediPassword,
					ISNULL(R.LimitaEVCAmbulatoriale,0) AS LimitaEVCAmbulatoriale
				FROM 
					T_AssLoginRuoli A
						INNER JOIN T_Ruoli R
							 ON A.CodRuolo = R.Codice	
				WHERE 
					A.CodLogin=@sCodLogin	
				ORDER BY Descrizione		
			
		END
		
		
	

										
	
	
END