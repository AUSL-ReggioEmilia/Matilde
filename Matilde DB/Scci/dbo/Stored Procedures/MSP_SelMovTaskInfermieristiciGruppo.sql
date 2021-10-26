CREATE PROCEDURE [dbo].[MSP_SelMovTaskInfermieristiciGruppo](@xParametri XML)
AS
BEGIN
		
	
											
		DECLARE @sCodSistema AS VARCHAR(50)
	DECLARE @sIDSistema AS VARCHAR(50)
	DECLARE @sIDGruppo AS VARCHAR(50)
	DECLARE @sCodTipoTaskInfermieristico AS VARCHAR(20)
	DECLARE @sCodStatoTaskInfermieristico AS VARCHAR(1800)
	
		DECLARE @uIDTaskIngermieristico AS UNIQUEIDENTIFIER	
	DECLARE @xTmpPar AS XML	
	DECLARE @sCodStatoTask AS VARCHAR(20)
	
	
	    DECLARE @xTimeStampBase AS XML	
	DECLARE @xTimeStamp AS XML	

		DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sWhere AS VARCHAR(Max)
	DECLARE @sTmp AS VARCHAR(Max)

		IF @xParametri.exist('/Parametri/CodSistema')=1
		BEGIN
			SET @sCodSistema=(SELECT TOP 1 ValoreParametro.CodSistema.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/CodSistema') as ValoreParametro(CodSistema))		
			SET @sCodSistema=ISNULL(@sCodSistema,'')
		END
		
		IF @xParametri.exist('/Parametri/IDSistema')=1
		BEGIN
			SET @sIDSistema=(SELECT TOP 1 ValoreParametro.IDSistema.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/IDSistema') as ValoreParametro(IDSistema))					
		END
			
								
		IF @xParametri.exist('/Parametri/IDGruppo')=1
		BEGIN
			SET @sIDGruppo=(SELECT TOP 1 ValoreParametro.IDGruppo.value('.','VARCHAR(50)')
							  FROM @xParametri.nodes('/Parametri/IDGruppo') as ValoreParametro(IDGruppo))					
		END
			
		IF @xParametri.exist('/Parametri/CodTipoTaskInfermieristico')=1
		BEGIN
			SET @sCodTipoTaskInfermieristico=(SELECT TOP 1 ValoreParametro.CodTipoTaskInfermieristico.value('.','VARCHAR(20)')
							  FROM @xParametri.nodes('/Parametri/CodTipoTaskInfermieristico') as ValoreParametro(CodTipoTaskInfermieristico))					
		END
	
		SET @sCodStatoTaskInfermieristico=''
	SELECT	@sCodStatoTaskInfermieristico =  @sCodStatoTaskInfermieristico +
														CASE 
								WHEN @sCodStatoTaskInfermieristico='' THEN ''
								ELSE  ','
							END + ''''
						  + ValoreParametro.CodStatoTaskInfermieristico.value('.','VARCHAR(1800)')
						  + ''''
						 FROM @xParametri.nodes('/Parametri/CodStatoTaskInfermieristico') as ValoreParametro(CodStatoTaskInfermieristico)

	SET @sCodStatoTaskInfermieristico=LTRIM(RTRIM(@sCodStatoTaskInfermieristico))
	IF	@sCodStatoTaskInfermieristico='''''' SET @sCodStatoTaskInfermieristico=''
	SET @sCodStatoTaskInfermieristico=UPPER(@sCodStatoTaskInfermieristico)
	
		SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
	  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
					
				
	SET @sSQL='SELECT ID
				FROM T_MovTaskInfermieristici'			   
				
	
					
		IF @sCodStatoTaskInfermieristico=''
		SET @sWhere=' AND ISNULL(CodStatoTaskInfermieristico,'''') IN (''PR'')'				   
	ELSE
		SET @sWhere=' AND ISNULL(CodStatoTaskInfermieristico,'''') IN ('+ @sCodStatoTaskInfermieristico + ')'				   
	
		IF ISNULL(@sIDGruppo,'') <> ''
		BEGIN
			SET @sTmp= ' AND IDGruppo=''' + convert(varchar(50),@sIDGruppo) +''''
			SET @sWhere= @sWhere + @sTmp								
		END	
	
		IF ISNULL(@sIDSistema,'') <> ''
		BEGIN
			SET @sTmp= ' AND IDSistema=''' + convert(varchar(50),@sIDSistema) +''''
			SET @sWhere= @sWhere + @sTmp								
		END	
	
		IF @sCodSistema IS NOT NULL
		BEGIN
			SET @sTmp= ' AND CodSistema=''' + convert(varchar(50),@sCodSistema) +''''
			SET @sWhere= @sWhere + @sTmp								
		END	
		
		IF @sCodTipoTaskInfermieristico IS NOT NULL
		BEGIN
			SET @sTmp= ' AND CodTipoTaskInfermieristico=''' + convert(varchar(50),@sCodTipoTaskInfermieristico) +''''
			SET @sWhere= @sWhere + @sTmp								
		END	
	
	IF ISNULL(@sWhere,'')<> ''
	BEGIN	
		SET @sSQL=@sSQL +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)
	END	
	
	SET @sSQL=@sSQL +' ORDER BY DataProgrammata ASC'
	
	PRINT @sSQL
	 	
	EXEC (@sSQL)
		
	RETURN 0
END