CREATE PROCEDURE [dbo].[MSP_SelPaziente](@xParametri XML)
AS
BEGIN
		
	
				DECLARE @uIDPaziente AS UNIQUEIDENTIFIER	
	DECLARE @uIDEpisodio AS UNIQUEIDENTIFIER
	DECLARE @bDatiEstesi AS Bit	
			
	DECLARE @uIDTmpPaz AS UNIQUEIDENTIFIER	
	DECLARE @sCodStatoConsensoCalcolato AS VARCHAR(20)

	DECLARE @uIDTmpPazFuso AS UNIQUEIDENTIFIER	
	DECLARE @sCodSACFuso AS VARCHAR(50)
	DECLARE @sCodStatoConsensoCalcolatoFuso AS VARCHAR(20)
			
											
		DECLARE @sGUID AS VARCHAR(Max)
	
	SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
		
	SET @sGUID=(SELECT TOP 1 ValoreParametro.IDEpisodio.value('.','VARCHAR(50)')
					  FROM @xParametri.nodes('/Parametri/IDEpisodio') as ValoreParametro(IDEpisodio))
	IF 	ISNULL(@sGUID,'') <> '' SET @uIDEpisodio=CONVERT(UNIQUEIDENTIFIER,	@sGUID)	
	
		SET @bDatiEstesi=(SELECT TOP 1 ValoreParametro.DatiEstesi.value('.','bit')
					  FROM @xParametri.nodes('/Parametri/DatiEstesi') as ValoreParametro(DatiEstesi))	
	SET @bDatiEstesi=ISNULL(@bDatiEstesi,1)
	
	
				IF @uIDEpisodio IS NOT NULL
		BEGIN	
						SET  @uIDTmpPaz= (SELECT TOP 1 IDPaziente						 
							  FROM T_MovPazienti 
							  WHERE IDEpisodio=@uIDEpisodio)
			
			SET @sCodStatoConsensoCalcolato= (SELECT TOP 1 CodStatoConsensoCalcolato FROM T_Pazienti WHERE ID=@uIDTmpPaz) 

						SELECT TOP 1
				@uIDTmpPazFuso=IDPaziente,
				@sCodSACFuso=CodSAC,
				@sCodStatoConsensoCalcolatoFuso	= CodStatoConsensoCalcolato		 
			FROM T_PazientiAlias PA
				INNER JOIN T_Pazienti PZ
					ON PA.IDPaziente=PZ.ID
			WHERE IDPazienteVecchio=@uIDTmpPaz
			
					
						SELECT  				  
					P.IDPaziente AS IDPaziente,
					P.CodSac,
					CASE 
						WHEN ISNULL(@sCodSACFuso,'')='' THEN P.CodSac 
						ELSE @sCodSACFuso
					END AS CodSacFuso,
					P.Cognome,
					P.Nome,
					P.Sesso,
					
										P.DataNascita,
					P.CodComuneNascita,
					P.ComuneNascita,
					P.CodProvinciaNascita, 
					P.ProvinciaNascita,
					
					P.CodiceFiscale,		
					CASE 
						WHEN @bDatiEstesi=1 THEN P.Foto		
						ELSE NULL
					END  AS Foto,
					ISNULL(P.Cognome,'') + ' ' + ISNULL(P.Nome,'') + ' ' + 
						CASE 
							WHEN ISNULL(Sesso,'')='' THEN ''
							ELSE ' (' + Sesso +') '
						END +
						CASE 
							WHEN DataNascita IS NULL THEN ''
							ELSE Convert(varchar(10),DataNascita,105)
						END +
					
						CASE 
							WHEN ISNULL(ComuneNascita,'')='' THEN ''
							ELSE ', ' + ComuneNascita
						END +									
						CASE 
							WHEN ISNULL(P.ProvinciaNascita,'')='' THEN ''
							ELSE ' (' + P.ProvinciaNascita + ')'
						END 					
					AS DescPaziente,
					
					
										P.IndirizzoResidenza AS IndirizzoResidenza,
					P.LocalitaResidenza AS LocalitaResidenza,
					P.CodComuneResidenza,
					P.ComuneResidenza AS ComuneResidenza,
					P.ProvinciaResidenza AS ProvinciaResidenza,	
					P.RegioneResidenza AS RegioneResidenza,						
					P.CAPResidenza AS CAPResidenza,
					
					LTRIM(
						ISNULL(P.IndirizzoResidenza,'') + ' ' + 
						ISNULL(P.ComuneResidenza,'') +									
						CASE 
							WHEN ISNULL(P.ProvinciaResidenza,'')='' THEN ''
							ELSE ' (' + P.ProvinciaResidenza + ')'
						END 
					)	AS Residenza,	
										
										P.IndirizzoDomicilio AS IndirizzoDomicilio,
					P.LocalitaDomicilio AS LocalitaDomicilio,
					P.ComuneDomicilio AS ComuneDomicilio,
					P.ProvinciaDomicilio AS ProvinciaDomicilio,
					P.CAPDomicilio AS CAPDomicilio,
					
					LTRIM(
						ISNULL(P.IndirizzoDomicilio,'') + ' ' + 
						ISNULL(P.ComuneDomicilio,'') +									
						CASE 
							WHEN ISNULL(P.ProvinciaDomicilio,'')='' THEN ''
							ELSE ' (' + P.ProvinciaDomicilio + ')'
						END 
					)	AS Domicilio,	
					
					
										P.CognomeNomeMedicoBase,
					P.CodFiscMedicoBase,
						
					P.ElencoEsenzioni,
					P.DataDecesso,
					ISNULL(@uIDTmpPazFuso,P.IDPaziente) AS IDPazienteFuso,
					ISNULL(CASE 
							WHEN @uIDTmpPazFuso IS NULL THEN @sCodStatoConsensoCalcolato
								ELSE @sCodStatoConsensoCalcolatoFuso
							END,'ND') AS CodStatoConsensoCalcolato
																
			FROM T_MovPazienti AS P			
			WHERE IDEpisodio=@uIDEpisodio
		END
	ELSE
		BEGIN					
						SELECT TOP 1
				@uIDTmpPazFuso=IDPaziente,
				@sCodSACFuso=CodSAC,
				@sCodStatoConsensoCalcolatoFuso	= PZ.CodStatoConsensoCalcolato	
			FROM T_PazientiAlias PA
				INNER JOIN T_Pazienti PZ
					ON PA.IDPaziente=PZ.ID
			WHERE IDPazienteVecchio=@uIDPaziente
			
			
						SELECT  				  
					P.ID AS IDPaziente,
					P.CodSac,
					CASE 
						WHEN ISNULL(@sCodSACFuso,'')='' THEN P.CodSac 
						ELSE @sCodSACFuso
					END AS CodSacFuso,
					P.Cognome,
					P.Nome,
					P.Sesso,
					P.DataNascita,
					P.CodComuneNascita,
					P.ComuneNascita,
					P.CodProvinciaNascita,
					P.ProvinciaNascita,
					P.CodiceFiscale,		
					CASE 
						WHEN @bDatiEstesi=1 THEN P.Foto		
						ELSE NULL
					END  AS Foto,		
					ISNULL(P.Cognome,'') + ' ' + ISNULL(P.Nome,'') + ' ' + 
						CASE 
							WHEN ISNULL(Sesso,'')='' THEN ''
							ELSE ' (' + Sesso +') '
						END +
						CASE 
							WHEN DataNascita IS NULL THEN ''
							ELSE Convert(varchar(10),DataNascita,105)
						END +
					
						CASE 
							WHEN ISNULL(ComuneNascita,'')='' THEN ''
							ELSE ', ' + ComuneNascita
						END +
										
						CASE 
							WHEN ISNULL(P.ProvinciaNascita,'')='' THEN ''
							ELSE ' (' + P.ProvinciaNascita + ')'
						END 
					AS DescPaziente,
					
										P.IndirizzoResidenza AS IndirizzoResidenza,
					P.LocalitaResidenza AS LocalitaResidenza,
					P.CodComuneResidenza,
					P.ComuneResidenza AS ComuneResidenza,
					P.ProvinciaResidenza AS ProvinciaResidenza,
					P.RegioneResidenza AS RegioneResidenza,
					P.CAPResidenza AS CAPResidenza,
					
					LTRIM(
						ISNULL(P.IndirizzoResidenza,'') + ' ' + 
						ISNULL(P.ComuneResidenza,'') +									
						CASE 
							WHEN ISNULL(P.ProvinciaResidenza,'')='' THEN ''
							ELSE ' (' + P.ProvinciaResidenza + ')'
						END 
					)	
					AS Residenza,	
					
					
										P.IndirizzoDomicilio AS IndirizzoDomicilio,
					P.LocalitaDomicilio AS LocalitaDomicilio,
					P.ComuneDomicilio AS ComuneDomicilio,
					P.ProvinciaDomicilio AS ProvinciaDomicilio,
					P.CAPDomicilio AS CAPDomicilio,
					
					LTRIM(
						ISNULL(P.IndirizzoDomicilio,'') + ' ' + 
						ISNULL(P.ComuneDomicilio,'') +									
						CASE 
							WHEN ISNULL(P.ProvinciaDomicilio,'')='' THEN ''
							ELSE ' (' + P.ProvinciaDomicilio + ')'
						END 
					)	AS Domicilio,	
				
										P.CognomeNomeMedicoBase,
					P.CodFiscMedicoBase,
					
					P.ElencoEsenzioni,
					P.DataDecesso,
					ISNULL(@uIDTmpPazFuso,P.ID) AS IDPazienteFuso,
					ISNULL(CASE 
							WHEN @uIDTmpPazFuso IS NULL THEN P.CodStatoConsensoCalcolato
							ELSE @sCodStatoConsensoCalcolatoFuso
							END,'ND')
					 AS CodStatoConsensoCalcolato
			FROM T_Pazienti AS P
			WHERE ID=@uIDPaziente	
		END							  
	RETURN 0
END