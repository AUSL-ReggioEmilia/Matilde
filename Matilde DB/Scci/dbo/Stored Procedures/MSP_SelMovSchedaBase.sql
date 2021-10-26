CREATE PROCEDURE [dbo].[MSP_SelMovSchedaBase](@xParametri XML)
AS
BEGIN
	

				
		DECLARE @sCodUA AS VARCHAR(20)	
	DECLARE @sCodEntita AS VARCHAR(20)
	DECLARE @sCodScheda AS VARCHAR(20)
	
	DECLARE @uIDScheda AS UNIQUEIDENTIFIER		
	DECLARE @uIDSchedaPadre AS UNIQUEIDENTIFIER	
	DECLARE @uIDEntita AS UNIQUEIDENTIFIER		
	DECLARE @sStoricizzata AS VARCHAR(20)
	DECLARE @bElencoStorico AS BIT
	DECLARE @nNumero AS INTEGER
			
		DECLARE @sGUID AS VARCHAR(Max)	
	DECLARE @sSQL AS VARCHAR(MAX)
	DECLARE @sWhere AS VARCHAR(Max)
	DECLARE @sTmp AS VARCHAR(Max)
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDScheda.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDScheda') as ValoreParametro(IDScheda))
	IF 	ISNULL(@sGUID,'') <> '' 
		SET @uIDScheda=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		  
	ELSE
		SET @uIDScheda=NULL	
				
		SET @uIDSchedaPadre=(SELECT TOP 1 ValoreParametro.IDSchedaPadre.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDSchedaPadre') as ValoreParametro(IDSchedaPadre))
	
		SET @sCodUA=(SELECT TOP 1 ValoreParametro.CodUA.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodUA') as ValoreParametro(CodUA))	
	SET @sCodUA=ISNULL(@sCodUA,'')
		
		SET @sCodEntita=(SELECT TOP 1 ValoreParametro.CodEntita.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodEntita') as ValoreParametro(CodEntita))	
	SET @sCodEntita=ISNULL(@sCodEntita,'')
	
		SET @sCodScheda=(SELECT TOP 1 ValoreParametro.CodScheda.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/CodScheda') as ValoreParametro(CodScheda))	
	SET @sCodScheda=ISNULL(@sCodScheda,'')
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEntita.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEntita') as ValoreParametro(IDEntita))
	IF 	ISNULL(@sGUID,'') <> '' 
		SET @uIDEntita=CONVERT(UNIQUEIDENTIFIER,	@sGUID)		  
	ELSE
		SET @uIDEntita =NULL	
	
		SET @nNumero=(SELECT TOP 1 ValoreParametro.Numero.value('.','INTEGER')
					  FROM @xParametri.nodes('/Parametri/Numero') as ValoreParametro(Numero))
	SET @nNumero=ISNULL(@nNumero,0)
	
		SET @sStoricizzata=(SELECT TOP 1 ValoreParametro.Storicizzata.value('.','VARCHAR(20)')
					  FROM @xParametri.nodes('/Parametri/Storicizzata') as ValoreParametro(Storicizzata))					  
	SET @sStoricizzata=ISNULL(@sStoricizzata,'TUTTE')
	SET @sStoricizzata=UPPER(@sStoricizzata)

		SET @bElencoStorico=(SELECT TOP 1 ValoreParametro.ElencoStorico.value('.','BIT')
					  FROM @xParametri.nodes('/Parametri/ElencoStorico') as ValoreParametro(ElencoStorico))					  
	SET @bElencoStorico= ISNULL(@bElencoStorico,0)
																
	    DECLARE @xTimeStamp AS XML	

    SET @xTimeStamp=(SELECT TOP 1 ValoreParametro.TS.query('.')
					  FROM @xParametri.nodes('/Parametri/TimeStamp') as ValoreParametro(TS))
	
	SET @xTimeStamp=CONVERT(XML,
						'<Parametri>' + CONVERT(varchar(max),@xTimeStamp) +
						'</Parametri>')
							
	SET @sTmp=''
	SET @sWhere=''
				
		IF @bElencoStorico=0
		BEGIN		
				SET @sSQL= 'SELECT
								M.*,
								S.Descrizione AS DescrScheda,
								AT.Descrizione AS DescrUnitaAtomica,
								U.Descrizione AS DescrUtenteUltimaModifica,
								CASE 
									WHEN MOD.CodUA IS NOT NULL AND ISNULL(S.FirmaDigitale,0)=1 THEN 1
									ELSE 0
								END AS PermessoUAFirma,
								ISNULL(S.CartellaAmbulatorialeCodificata,0) AS CartellaAmbulatorialeCodificata
							FROM 
								T_MovSchede M  WITH (NOLOCK)
									LEFT JOIN T_Login U WITH (NOLOCK)
										ON M.CodUtenteUltimaModifica = U.Codice
									LEFT JOIN T_Schede S WITH (NOLOCK) ON
										M.CodScheda=S.Codice
									LEFT JOIN T_UnitaAtomiche AT WITH (NOLOCK) ON
										M.CodUA =AT.Codice
									LEFT JOIN T_AssUAModuli MOD WITH (NOLOCK) 
										ON (MOD.CodUA =AT.Codice AND 
											MOD.CodModulo=''FirmaD_SCH'')									
									'
				
								IF 	@uIDScheda IS NOT NULL
					BEGIN											
						SET @sTmp= ' AND M.ID=''' + CONVERT(VARCHAR(50),@uIDScheda) + ''''			
						SET @sWhere= @sWhere + @sTmp														
					END			
				
								IF 	@uIDEntita IS NOT NULL
					BEGIN
						SET @sTmp= ' AND M.IDEntita=''' + CONVERT(VARCHAR(50),@uIDEntita) + ''''			
						SET @sWhere= @sWhere + @sTmp	
					END			
				
								IF ISNULL(@sCodEntita,'') <> ''
					BEGIN									
						SET @sTmp= ' AND M.CodEntita=''' + @sCodEntita + ''''			
						SET @sWhere= @sWhere + @sTmp						
					END
				
								IF ISNULL(@sStoricizzata,'') IN ('SI','NO')
					BEGIN
												IF 	ISNULL(@sStoricizzata,'') ='SI'				
							SET @sTmp= ' AND M.Storicizzata=1'			
							
						IF 	ISNULL(@sStoricizzata,'') ='NO'					
							SET @sTmp= ' AND M.Storicizzata=0'			
							
						SET @sWhere= @sWhere + @sTmp						
					END
				
								IF ISNULL(@nNumero,0) <> 0
				BEGIN
					SET @sTmp= ' AND M.Numero=''' + CONVERT(VARCHAR(50), @nNumero) + ''''			
					SET @sWhere= @sWhere + @sTmp			
				END
				
								IF ISNULL(@sCodScheda,'') <> ''
					BEGIN									
						SET @sTmp= ' AND M.CodScheda=''' + @sCodScheda + ''''			
						SET @sWhere= @sWhere + @sTmp						
					END				
				
								IF @uIDScheda IS NULL  AND  @uIDEntita IS NOT NULL
				BEGIN
					BEGIN									
						SET @sTmp= ' AND M.CodStatoScheda <> ''CA'''			
						SET @sWhere= @sWhere + @sTmp						
					END	
				END
						  

								IF @uIDSchedaPadre IS NOT  NULL
				BEGIN
					SET @sTmp= ' AND M.IDSchedaPadre=''' + CONVERT(VARCHAR(50),@uIDSchedaPadre) + ''''			
					SET @sWhere= @sWhere + @sTmp	
				END
								

								IF ISNULL(@sWhere,'')<> ''
				BEGIN	
					SET @sSQL=@sSQL +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)
				END	
				ELSE
				BEGIN
					SET @sSQL=@sSQL +' WHERE 1=0'
				END
					
								SET @sSQL=@sSQL + ' ORDER BY DataCreazione DESC'
				PRINT @sSQL
				
				EXEC (@sSQL)
				
		END
	ELSE
		BEGIN			
						SET @sSQL= 'SELECT
							M.ID,
							M.Storicizzata,
							M.Numero,
							CONVERT(VARCHAR(10), ISNULL(DataUltimaModifica,DataCreazione), 105) + '' '' + CONVERT(varchar(5), ISNULL(DataUltimaModifica,DataCreazione), 14) AS DataUltimaModifica,	
							ISNULL(CodUtenteUltimaModifica,M.CodUtenteRilevazione) AS CodUtenteUltimaModifica,
							U.Descrizione AS DescrUtenteUltimaModifica	
						FROM 
							T_MovSchede M WITH (NOLOCK)
								LEFT JOIN T_Login U WITH (NOLOCK)
									ON M.CodUtenteUltimaModifica = U.Codice'

						
						IF 	@uIDEntita IS NOT NULL
				BEGIN
					SET @sTmp= ' AND M.IDEntita=''' + CONVERT(VARCHAR(50),@uIDEntita) + ''''			
					SET @sWhere= @sWhere + @sTmp	
				END																				
			
						IF ISNULL(@sCodEntita,'') <> ''
				BEGIN									
					SET @sTmp= ' AND M.CodEntita=''' + @sCodEntita + ''''			
					SET @sWhere= @sWhere + @sTmp						
				END
					
						IF ISNULL(@nNumero,0) <> 0
				BEGIN
					SET @sTmp= ' AND M.Numero=''' + CONVERT(VARCHAR(50), @nNumero) + ''''			
						SET @sWhere= @sWhere + @sTmp			
				END	
			   				
						IF ISNULL(@sCodScheda,'') <> ''
				BEGIN									
					SET @sTmp= ' AND M.CodScheda=''' + @sCodScheda + ''''			
					SET @sWhere= @sWhere + @sTmp						
				END					
			
						IF @uIDSchedaPadre IS NOT  NULL
			BEGIN
				SET @sTmp= ' AND M.IDSchedaPadre=''' + CONVERT(VARCHAR(50),@uIDSchedaPadre) + ''''			
				SET @sWhere= @sWhere + @sTmp	
			END

						SET @sTmp= ' AND M.CodStatoScheda <> ''CA'''			
			SET @sWhere= @sWhere + @sTmp	
			
						IF ISNULL(@sWhere,'')<> ''
			BEGIN	
				SET @sSQL=@sSQL +' WHERE ' + RIGHT(@sWhere,len(@sWhere)-5)
			END	

						SET @sSQL=@sSQL + ' ORDER BY ISNULL(DataUltimaModifica,DataCreazione) DESC, M.Storicizzata ASC'
			PRINT @sSQL
			
			EXEC (@sSQL)
				 								
		END	

				
	EXEC MSP_InsMovTimeStamp @xTimeStamp	
				
	RETURN 0
END