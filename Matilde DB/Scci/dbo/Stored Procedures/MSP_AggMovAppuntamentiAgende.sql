CREATE PROCEDURE [dbo].[MSP_AggMovAppuntamentiAgende](@xParametri XML)
AS
BEGIN
		
	
												
		DECLARE @uIDAppuntamentoAgenda AS UNIQUEIDENTIFIER		
	DECLARE @uIDAppuntamento AS UNIQUEIDENTIFIER	
	DECLARE @sCodAgenda AS VARCHAR(20)	
			DECLARE @sCodStatoAppuntamentoAgenda AS VARCHAR(20)
	DECLARE @sCodUtenteRilevazione AS VARCHAR(100)
	DECLARE @sCodUtenteUltimaModifica AS VARCHAR(100)									
	
	DECLARE @sCodRaggr1 AS VARCHAR(20)
	DECLARE @txtDescrRaggr1 VARCHAR(MAX)	
	
	DECLARE @sCodRaggr2 AS VARCHAR(20)
	DECLARE @txtDescrRaggr2 VARCHAR(MAX)
	
	DECLARE @sCodRaggr3 AS VARCHAR(20)
	DECLARE @txtDescrRaggr3 VARCHAR(MAX)
	

		DECLARE @sGUID AS VARCHAR(50)
	DECLARE @uGUID AS UNIQUEIDENTIFIER
	DECLARE @sDataTmp AS VARCHAR(20)	
	DECLARE @sTmp AS VARCHAR(MAX)
	
	DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sSET AS VARCHAR(MAX)
	DECLARE @sWHERE AS VARCHAR(MAX)
	
	
	    DECLARE @xTimeStampBase AS XML	
	DECLARE @xTimeStamp AS XML	
		
		DECLARE @xSchedaMovimento AS XML
	
		DECLARE @xLogPrima AS XML
	DECLARE @xLogDopo AS XML
	DECLARE @xTemp AS XML
	DECLARE @xParLog AS XML
			
		SET @sSQL='UPDATE T_MovAppuntamentiAgende ' + CHAR(13) + CHAR(10) +
			  'SET '  				  						
			
	SET @sSET =''
	
	IF @xParametri.exist('(//IDAppuntamentoAgenda)')=1
	BEGIN
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDAppuntamentoAgenda.value('.','VARCHAR(50)')
						  FROM @xParametri.nodes('/Parametri/IDAppuntamentoAgenda') as ValoreParametro(IDAppuntamentoAgenda))
									  
		IF 	ISNULL(@sGUID,'') <> '' 
			BEGIN
				SET @uIDAppuntamentoAgenda=CONVERT(UNIQUEIDENTIFIER,	@sGUID)				
			END		
		
	END
			
		IF @xParametri.exist('/Parametri/IDAppuntamento')=1
		BEGIN
			SET @sGUID=(SELECT TOP 1 ValoreParametro.IDAppuntamento.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/IDAppuntamento') as ValoreParametro(IDAppuntamento))
							  
				IF 	ISNULL(@sGUID,'') <> '' 
				BEGIN
					SET @uIDAppuntamento=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		
					SET	@sSET= @sSET + ',IDAppuntamento=''' + convert(VARCHAR(50),@uIDAppuntamento) + ''''	+ CHAR(13) + CHAR(10)	
				END
				ELSE
					SET	@sSET= @sSET + ',IDAppuntamento=NULL'	+ CHAR(13) + CHAR(10)									  		
		END			

		IF @xParametri.exist('/Parametri/CodAgenda')=1
	BEGIN
		SET @sCodAgenda=(SELECT TOP 1 ValoreParametro.CodAgenda.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodAgenda') as ValoreParametro(CodAgenda))	
					  
		IF @sCodAgenda <> ''
			BEGIN									
				SET	@sSET= @sSET + ',CodAgenda=''' + @sCodAgenda +''''	+ CHAR(13) + CHAR(10)				
			END	
			ELSE
				SET	@sSET= @sSET + ',CodAgenda=NULL'	+ CHAR(13) + CHAR(10)			
	END
	
	
	
		IF @xParametri.exist('/Parametri/CodStatoAppuntamentoAgenda')=1
		BEGIN
			SET @sCodStatoAppuntamentoAgenda=(SELECT TOP 1 ValoreParametro.CodStatoAppuntamentoAgenda.value('.','VARCHAR(20)')
						  FROM @xParametri.nodes('/Parametri/CodStatoAppuntamentoAgenda') as ValoreParametro(CodStatoAppuntamentoAgenda))	
						  
			IF @sCodStatoAppuntamentoAgenda <> ''
					SET	@sSET= @sSET + ',CodStatoAppuntamentoAgenda=''' + @sCodStatoAppuntamentoAgenda+''''	+ CHAR(13) + CHAR(10)				
				ELSE
					SET	@sSET= @sSET + ',CodStatoAppuntamentoAgenda=NULL'	+ CHAR(13) + CHAR(10)		
		END	
		
		IF @xParametri.exist('/Parametri/CodRaggr1')=1
		BEGIN
			SET @sCodRaggr1=(SELECT TOP 1 ValoreParametro.CodRaggr1.value('.','VARCHAR(50)')
						  FROM @xParametri.nodes('/Parametri/CodRaggr1') as ValoreParametro(CodRaggr1))	
						  
			IF @sCodRaggr1 <> ''
					SET	@sSET= @sSET + ',CodRaggr1=''' + @sCodRaggr1 +''''	+ CHAR(13) + CHAR(10)				
				ELSE
					SET	@sSET= @sSET + ',CodRaggr1=NULL'	+ CHAR(13) + CHAR(10)		
		END	
	
		IF @xParametri.exist('/Parametri/DescrRaggr1')=1
	BEGIN	
		SET @txtDescrRaggr1=(SELECT TOP 1 ValoreParametro.DescrRaggr1.value('.','VARCHAR(MAX)')
					  FROM @xParametri.nodes('/Parametri/DescrRaggr1') as ValoreParametro(DescrRaggr1))	
				  				  
		IF ISNULL(@txtDescrRaggr1,'') <> ''
				SET	@sSET= @sSET +',DescrRaggr1 =
											CONVERT(varchar(max),
													CAST(N'''' AS xml).value(''xs:base64Binary("' + @txtDescrRaggr1
													+ '")'', ''varbinary(max)'') 
												)'  + CHAR(13) + CHAR(10)	
		ELSE
				SET	@sSET= @sSET +',DescrRaggr1=NULL '	+ CHAR(13) + CHAR(10)																
	END	
		
		IF @xParametri.exist('/Parametri/CodRaggr2')=1
		BEGIN
			SET @sCodRaggr2=(SELECT TOP 1 ValoreParametro.CodRaggr2.value('.','VARCHAR(50)')
						  FROM @xParametri.nodes('/Parametri/CodRaggr2') as ValoreParametro(CodRaggr2))	
						  
			IF @sCodRaggr2 <> ''
					SET	@sSET= @sSET + ',CodRaggr2=''' + @sCodRaggr2 +''''	+ CHAR(13) + CHAR(10)				
				ELSE
					SET	@sSET= @sSET + ',CodRaggr2=NULL'	+ CHAR(13) + CHAR(10)		
		END	
		
		IF @xParametri.exist('/Parametri/DescrRaggr2')=1
	BEGIN	
		SET @txtDescrRaggr2=(SELECT TOP 1 ValoreParametro.DescrRaggr2.value('.','VARCHAR(MAX)')
					  FROM @xParametri.nodes('/Parametri/DescrRaggr2') as ValoreParametro(DescrRaggr2))	
				  				  
		IF ISNULL(@txtDescrRaggr2,'') <> ''
				SET	@sSET= @sSET +',DescrRaggr2 =
											CONVERT(varchar(max),
													CAST(N'''' AS xml).value(''xs:base64Binary("' + @txtDescrRaggr2
													+ '")'', ''varbinary(max)'') 
												)'  + CHAR(13) + CHAR(10)	
		ELSE
				SET	@sSET= @sSET +',DescrRaggr2=NULL '	+ CHAR(13) + CHAR(10)																
	END	

		IF @xParametri.exist('/Parametri/CodRaggr3')=1
		BEGIN
			SET @sCodRaggr3=(SELECT TOP 1 ValoreParametro.CodRaggr3.value('.','VARCHAR(50)')
						  FROM @xParametri.nodes('/Parametri/CodRaggr3') as ValoreParametro(CodRaggr3))	
						  
			IF @sCodRaggr3 <> ''
					SET	@sSET= @sSET + ',CodRaggr3=''' + @sCodRaggr3 +''''	+ CHAR(13) + CHAR(10)				
				ELSE
					SET	@sSET= @sSET + ',CodRaggr3=NULL'	+ CHAR(13) + CHAR(10)		
		END	

		IF @xParametri.exist('/Parametri/DescrRaggr3')=1
	BEGIN	
		SET @txtDescrRaggr3=(SELECT TOP 1 ValoreParametro.DescrRaggr3.value('.','VARCHAR(MAX)')
					  FROM @xParametri.nodes('/Parametri/DescrRaggr3') as ValoreParametro(DescrRaggr3))	
				  				  
		IF ISNULL(@txtDescrRaggr3,'') <> ''
				SET	@sSET= @sSET +',DescrRaggr3 =
											CONVERT(varchar(max),
													CAST(N'''' AS xml).value(''xs:base64Binary("' + @txtDescrRaggr3
													+ '")'', ''varbinary(max)'') 
												)'  + CHAR(13) + CHAR(10)	
		ELSE
				SET	@sSET= @sSET +',DescrRaggr3=NULL '	+ CHAR(13) + CHAR(10)																
	END	

		SET @sCodUtenteUltimaModifica=(SELECT TOP 1 ValoreParametro.CodUtenteUltimaModifica.value('.','VARCHAR(100)')
					  FROM @xParametri.nodes('/Parametri/TimeStamp/CodLogin') as ValoreParametro(CodUtenteUltimaModifica))	
	SET @sCodUtenteUltimaModifica=ISNULL(@sCodUtenteUltimaModifica,'')
		
	IF @sCodUtenteUltimaModifica <> ''
		SET	@sSET= @sSET + ',CodUtenteUltimaModifica=''' + @sCodUtenteUltimaModifica + '''' + CHAR(13) + CHAR(10)		
	ELSE
		SET	@sSET= @sSET + ',CodUtenteUltimaModifica=NULL' + CHAR(13) + CHAR(10)	
	
		SET	@sSET= @sSET + ',DataUltimaModifica=getdate() ' + CHAR(13) + CHAR(10)	
		
		SET	@sSET= @sSET + ',DataUltimaModificaUTC=getdate() ' + CHAR(13) + CHAR(10)	
					
	    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	
		SET @xSchedaMovimento=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/SchedaMovimento') as ValoreParametro(TS))					
													
			
			
		IF LEFT(@sSET,1)=',' 
		SET @sSET=RIGHT(@sSET,LEN(@sSET)-1)


	IF LTRIM(RTRIM(@sSET)) <> ''
		BEGIN
				SET @sWHERE =' WHERE ID=''' + convert(varchar(50),@uIDAppuntamentoAgenda) +''''
				
				SET @sSQL=	ISNULL(@sSQL,'') + CHAR(13) + CHAR(10) + 
							ISNULL(@sSET,'') + CHAR(13) + CHAR(10) + 
							ISNULL(@sWHERE,'WHERE 0=1')						
				
																						
				BEGIN TRANSACTION
					PRINT 'SQL=' + @sSQL
					EXEC (@sSQL)
						
				IF @@ERROR=0 AND @@ROWCOUNT >0
					BEGIN 
						COMMIT TRANSACTION
						PRINT 'OK'
					END
				ELSE
					BEGIN
						PRINT 'ERR: ' + CONVERT(VARCHAR(MAX), @@ERROR) 
						PRINT 'ROWCOUNT: ' + CONVERT(VARCHAR(MAX), @@ROWCOUNT) 
						ROLLBACK TRANSACTION						
						
											END

		END			

	RETURN 0
END