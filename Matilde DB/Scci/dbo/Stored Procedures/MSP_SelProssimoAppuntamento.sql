CREATE PROCEDURE [dbo].[MSP_SelProssimoAppuntamento](@xParametri AS XML )
AS
BEGIN
	

				 
	DECLARE @sCodAgenda AS VARCHAR(20)
	DECLARE @sCodTipoAppuntamento AS VARCHAR(20)
	DECLARE @sOraPredefinita AS VARCHAR(5)
	DECLARE @sOraInizio AS VARCHAR(5)
	DECLARE @sOraFine AS VARCHAR(5)
	DECLARE @dData AS DATETIME	
	
	
		DECLARE @sDataTmp AS VARCHAR(20)	
	DECLARE @sSQL AS VARCHAR(MAX)	
	DECLARE @dDataMax AS DATETIME	
	DECLARE @sOra AS VARCHAR(5)
					
	
		SET @sCodAgenda=(SELECT TOP 1 ValoreParametro.CodAgenda.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodAgenda') as ValoreParametro(CodAgenda))		
	
		SET @sCodTipoAppuntamento=(SELECT TOP 1 ValoreParametro.CodTipoAppuntamento.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodTipoAppuntamento') as ValoreParametro(CodTipoAppuntamento))						
	
		SET @sOraPredefinita=(SELECT TOP 1 ValoreParametro.OraPredefinita.value('.','VARCHAR(5)')
					  FROM @xParametri.nodes('/Parametri/OraPredefinita') as ValoreParametro(OraPredefinita))						
	
		SET @sOraInizio=(SELECT TOP 1 ValoreParametro.OraInizio.value('.','VARCHAR(5)')
					  FROM @xParametri.nodes('/Parametri/OraInizio') as ValoreParametro(OraInizio))						
	
		SET @sOraFine=(SELECT TOP 1 ValoreParametro.OraFine.value('.','VARCHAR(5)')
					  FROM @xParametri.nodes('/Parametri/OraFine') as ValoreParametro(OraFine))		

		SET @sDataTmp=(SELECT TOP 1 ValoreParametro.DataInizio.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/Data') as ValoreParametro(DataInizio))					  
	SET @sDataTmp=ISNULL(@sDataTmp,'')
	IF @sDataTmp<> '' 
		BEGIN
						SET @sDataTmp= SUBSTRING(@sDataTmp,4,2) + '-' 
						+ LEFT(@sDataTmp,2) + '-' +
						+ SUBSTRING(@sDataTmp,7,4) +
						' ' + RIGHT(@sDataTmp,5)
			IF ISDATE(@sDataTmp)=1
				SET	@dData=CONVERT(DATETIME,@sDataTmp,120)						
			ELSE
				SET	@dData =NULL			
		END			
	
		
				
	IF ISNULL(@sOraInizio,'') <> '' AND ISNULL(@sOraFine,'') <> '' 
	BEGIN
				SET @dDataMax=(SELECT MAX(DataFine)
							FROM T_MovAppuntamenti APP
								INNER JOIN T_MovAppuntamentiAgende AGE
									ON APP.ID=AGE.IDAppuntamento
							WHERE AGE.CodAgenda=@sCodAgenda AND
								  APP.CodTipoAppuntamento=@sCodTipoAppuntamento AND
								  APP.CodStatoAppuntamento NOT IN ('CA','AN','TR') AND
								  AGE.CodStatoAppuntamentoAgenda <> 'CA' AND
								  DataFine >= CONVERT(datetime,REPLACE(CONVERT(VARCHAR(10),@dData,102) + ' ' + @sOraInizio,'.','/'),120)	 AND	
								  DataFine <  CONVERT(datetime,REPLACE(CONVERT(VARCHAR(10),@dData,102) + ' ' + @sOraFine,'.','/'),120)						  
						  )

		IF @dDataMax IS NULL
		BEGIN
						IF ISNULL(@sOraInizio,'') <> ''
				SET @sOra=@sOraInizio
			ELSE
				BEGIN
										SET @sOra='00:00'
				END
		END
		ELSE	
		BEGIN
			SET @sOra=CONVERT(VARCHAR(5),@dDataMax,14)
		END
	
	END
	ELSE
	BEGIN	
				SET @dDataMax=(SELECT MAX(DataFine)
					FROM T_MovAppuntamenti APP
						INNER JOIN T_MovAppuntamentiAgende AGE
							ON APP.ID=AGE.IDAppuntamento
					WHERE AGE.CodAgenda=@sCodAgenda AND
						  						  APP.CodStatoAppuntamento NOT IN ('CA','AN','TR') AND
						  AGE.CodStatoAppuntamentoAgenda <> 'CA' AND
						  DataFine >= CONVERT(datetime,CONVERT(VARCHAR(10),@dData,105),105) AND
						  DataFine < CONVERT(datetime,CONVERT(VARCHAR(10),@dData+1,105),105) 						  
				  )
		

		IF @dDataMax IS NULL
		BEGIN
						IF ISNULL(@sOraPredefinita,'') <> ''
				SET @sOra=@sOraPredefinita
			ELSE
				BEGIN
										SET @sOra='00:00'
				END
		END
		ELSE	
		BEGIN
			SET @sOra=CONVERT(VARCHAR(5),@dDataMax,14)
		END
	END			  
	
	
	
	SELECT @sOra AS Ora
	
	RETURN 0
END