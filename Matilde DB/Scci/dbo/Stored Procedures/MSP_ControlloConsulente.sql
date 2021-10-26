
CREATE PROCEDURE [dbo].[MSP_ControlloConsulente](@xParametri XML)
AS
BEGIN

		
	
		
		DECLARE @sCodLogin AS VARCHAR(100)	
	DECLARE @sDescrizione AS VARCHAR(100)	
	DECLARE @sCodRuolo AS VARCHAR(20)	
	DECLARE @bInserisciSeMancante AS BIT
	
		
		DECLARE @bEsisteLogin AS BIT
	DECLARE @bEsisteRuolo AS BIT
	
				
		SET @bInserisciSeMancante=(SELECT TOP 1 ValoreParametro.InserisciSeMancante.value('.','BIT')
					  FROM @xParametri.nodes('/Parametri/InserisciSeMancante') as ValoreParametro(InserisciSeMancante))	
	SET @bInserisciSeMancante=ISNULL(@bInserisciSeMancante,0)
	
		SET @sCodLogin=(SELECT TOP 1 ValoreParametro.CodLogin.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/CodLogin') as ValoreParametro(CodLogin))	
	SET @sCodLogin=ISNULL(@sCodLogin,'')
	
		SET @sCodRuolo=(SELECT TOP 1 ValoreParametro.CodRuolo.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/CodRuolo') as ValoreParametro(CodRuolo))	
	SET @sCodRuolo=ISNULL(@sCodRuolo,'')
					  				  
		SET @sDescrizione=(SELECT TOP 1 ValoreParametro.Descrizione.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/Descrizione') as ValoreParametro(Descrizione))	
	SET @sDescrizione=ISNULL(@sDescrizione,@sCodLogin)				  				  

				SET @bEsisteLogin=0
	SET @bEsisteLogin =(SELECT COUNT(*) FROM T_Login WHERE Codice=@sCodLogin)
	
	IF @bEsisteLogin=0 AND @bInserisciSeMancante=1
					INSERT INTO T_Login(Codice,Descrizione,FlagAdmin,FlagObsoleto,FlagSistema)
			SELECT @sCodLogin,@sDescrizione,0,0,0
		

				SET @bEsisteRuolo=0
	SET @bEsisteRuolo =(SELECT COUNT(*) FROM T_AssLoginRuoli WHERE CodLogin=@sCodLogin And CodRuolo=@sCodRuolo)	
	IF @bEsisteRuolo=0 AND  @bInserisciSeMancante=1
						INSERT INTO T_AssLoginRuoli(CodLogin,CodRuolo)
			VALUES (@sCodLogin,@sCodRuolo)	
				
	RETURN 0	
	
END