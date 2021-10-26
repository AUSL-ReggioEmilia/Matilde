CREATE PROCEDURE [dbo].[MSP_CreaIcone](@sCodEntita VARCHAR(20))
AS
BEGIN
	
	
		
						
	
		IF ISNULL(@sCodEntita,'')='' 
		BEGIN
			DELETE FROM T_Icone
		END
	ELSE
		BEGIN
						DELETE FROM T_Icone
			WHERE CodEntita=@sCodEntita
		END	

		IF 	@sCodEntita='APP' OR isnull(@sCodEntita,'')=''
	BEGIN
		INSERT INTO T_Icone(CodEntita,CodTipo,CodStato,Descrizione)
		SELECT 'APP' AS CodEntita,
			   T.Codice AS CodTipo,
			   S.Codice AS CodStato,
			   T.Descrizione + ' - ' + S.Descrizione
		FROM T_TipoAppuntamento T
			CROSS JOIN T_StatoAppuntamento S
	END

		IF 	@sCodEntita='EVC' OR isnull(@sCodEntita,'')=''
	BEGIN
		INSERT INTO T_Icone(CodEntita,CodTipo,CodStato,Descrizione)
		SELECT 'EVC' AS CodEntita,
			   T.Codice AS CodTipo,
			   S.Codice AS CodStato,
			   T.Descrizione + ' - ' + S.Descrizione
		FROM T_TipoEvidenzaClinica T
			CROSS JOIN T_StatoEvidenzaClinica S
		UNION 	
			SELECT 'EVC' AS CodEntita,
			 T.Codice AS CodTipo,
			 '' AS CodStato,
			 T.Descrizione 
		FROM T_TipoEvidenzaClinica T	 
	END

		IF 	@sCodEntita='WKI' OR isnull(@sCodEntita,'')=''
	BEGIN
		INSERT INTO T_Icone(CodEntita,CodTipo,CodStato,Descrizione)
		SELECT 'WKI' AS CodEntita,
			   T.Codice AS CodTipo,
			   S.Codice AS CodStato,
			   T.Descrizione + ' - ' + S.Descrizione
		FROM T_TipoTaskInfermieristico T
			CROSS JOIN T_StatoTaskInfermieristico S
		UNION 	
			SELECT 'WKI' AS CodEntita,
			 T.Codice AS CodTipo,
			 '' AS CodStato,
			 T.Descrizione 
		FROM T_TipoTaskInfermieristico T		
	END

		IF 	@sCodEntita='EPI' OR isnull(@sCodEntita,'')=''
	BEGIN
		INSERT INTO T_Icone(CodEntita,CodTipo,CodStato,Descrizione)
		SELECT 'EPI' AS CodEntita,
			   T.Codice AS CodTipo,
			   S.Codice AS CodStato,
			   T.Descrizione + ' - ' + S.Descrizione
		FROM T_TipoEpisodio T
			CROSS JOIN T_StatoEpisodio S
		UNION 	
			SELECT 'EPI' AS CodEntita,
			 T.Codice AS CodTipo,
			 '' AS CodStato,
			 T.Descrizione 
		FROM T_TipoEpisodio T		
	END

		IF 	@sCodEntita='PRF' OR isnull(@sCodEntita,'')=''
	BEGIN
		INSERT INTO T_Icone(CodEntita,CodTipo,CodStato,Descrizione)
		SELECT 'PRF' AS CodEntita,
			   T.Codice AS CodTipo,
			   S.Codice AS CodStato,
			   T.Descrizione + ' - ' + S.Descrizione
		FROM T_TipoPrescrizione T
			CROSS JOIN T_StatoPrescrizione S
		UNION 	
			SELECT 'PRF' AS CodEntita,
			 T.Codice AS CodTipo,
			 '' AS CodStato,
			 T.Descrizione 
		FROM T_TipoPrescrizione T	
	END

			IF 	@sCodEntita='DCL' OR isnull(@sCodEntita,'')=''
	BEGIN
		INSERT INTO T_Icone(CodEntita,CodTipo,CodStato,Descrizione)
		SELECT 'DCL' AS CodEntita,
			   T.Codice AS CodTipo,
			   S.Codice AS CodStato,
			   T.Descrizione + ' - ' + S.Descrizione
		FROM T_TipoDiario T
			CROSS JOIN T_StatoDiario S
		UNION 	
			SELECT 'DCL' AS CodEntita,
			 T.Codice AS CodTipo,
			 '' AS CodStato,
			 T.Descrizione 
		FROM T_TipoDiario T	
	END
	
			IF 	@sCodEntita='ALG' OR isnull(@sCodEntita,'')=''
	BEGIN
		INSERT INTO T_Icone(CodEntita,CodTipo,CodStato,Descrizione)
		SELECT 'ALG' AS CodEntita,
			   T.Codice AS CodTipo,
			   S.Codice AS CodStato,
			   T.Descrizione + ' - ' + S.Descrizione
		FROM T_TipoAlertGenerico T
			CROSS JOIN T_StatoAlertGenerico S
		UNION 	
			SELECT 'ALG' AS CodEntita,
			 T.Codice AS CodTipo,
			 '' AS CodStato,
			 T.Descrizione 
		FROM T_TipoAlertGenerico T		
	END
	
		IF 	@sCodEntita='ALA' OR isnull(@sCodEntita,'')=''
	BEGIN
		INSERT INTO T_Icone(CodEntita,CodTipo,CodStato,Descrizione)
		SELECT 'ALA' AS CodEntita,
			   T.Codice AS CodTipo,
			   S.Codice AS CodStato,
			   T.Descrizione + ' - ' + S.Descrizione
		FROM T_TipoAlertAllergiaAnamnesi T
			CROSS JOIN T_StatoAlertAllergiaAnamnesi S
		UNION 	
			SELECT 'ALA' AS CodEntita,
			 T.Codice AS CodTipo,
			 '' AS CodStato,
			 T.Descrizione 
		FROM T_TipoAlertAllergiaAnamnesi T		
	END
	
		IF 	@sCodEntita='PVT' OR isnull(@sCodEntita,'')=''
	BEGIN
		INSERT INTO T_Icone(CodEntita,CodTipo,CodStato,Descrizione)
		SELECT 'PVT' AS CodEntita,
			   T.Codice AS CodTipo,
			   S.Codice AS CodStato,
			   T.Descrizione + ' - ' + S.Descrizione
		FROM T_TipoParametroVitale T
			CROSS JOIN T_StatoParametroVitale S
		UNION 	
			SELECT 'PVT' AS CodEntita,
			 T.Codice AS CodTipo,
			 '' AS CodStato,
			 T.Descrizione 
		FROM T_TipoParametroVitale T		
	END
	
		IF 	@sCodEntita='OE' OR isnull(@sCodEntita,'')=''
	BEGIN
		INSERT INTO T_Icone(CodEntita,CodTipo,CodStato,Descrizione)
		SELECT 'OE' AS CodEntita,
			   T.Codice AS CodTipo,
			   S.Codice AS CodStato,
			   T.Descrizione + ' - ' + S.Descrizione
		FROM T_TipoOrdine T
			CROSS JOIN T_StatoOrdine S
		UNION 	
			SELECT 'OE' AS CodEntita,
			 T.Codice AS CodTipo,
			 '' AS CodStato,
			 T.Descrizione 
		FROM T_TipoOrdine T	
		UNION 	
			SELECT 'OE' AS CodEntita,
			'' AS CodTipo,
			 T.Codice AS CodStato,
			 T.Descrizione 
		FROM T_StatoOrdine T
	END
	
		IF 	@sCodEntita='ALL' OR isnull(@sCodEntita,'')=''
	BEGIN
				INSERT INTO T_Icone(CodEntita,CodTipo,CodStato,Descrizione)
		SELECT 'ALL' AS CodEntita,
			   T.Codice AS CodTipo,
			   E.Codice AS CodStato,
			   T.Descrizione + ' - ' + E.Descrizione
		FROM T_TipoAllegato T
			CROSS JOIN T_EntitaAllegato E
		UNION 	
			SELECT 'ALL' AS CodEntita,
			 T.Codice AS CodTipo,
			 '' AS CodStato,
			 T.Descrizione 
		FROM T_TipoAllegato T				
	END

		IF 	@sCodEntita='ALLFMT' OR isnull(@sCodEntita,'')=''
	BEGIN
	
		INSERT INTO T_Icone(CodEntita,CodTipo,CodStato,Descrizione)
		SELECT 'ALLFMT' AS CodEntita,
			 F.Codice AS CodTipo,
			 '' AS CodStato,
			 F.Descrizione 
		FROM T_FormatoAllegati AS F
	END
	
		IF 	@sCodEntita='VSM' OR isnull(@sCodEntita,'')=''
	BEGIN
		INSERT INTO T_Icone(CodEntita,CodTipo,CodStato,Descrizione)		
		SELECT 'VSM' AS CodEntita,
			   T.Codice AS CodTipo,
			   S.Codice AS CodStato,
			   T.Descrizione + ' - ' + S.Descrizione
		FROM T_ViaSomministrazione T
			CROSS JOIN T_StatoContinuazione S
		UNION	
					
			SELECT 'VSM' AS CodEntita,
			 T.Codice AS CodTipo,
			 '' AS CodStato,
			 T.Descrizione 
		FROM T_ViaSomministrazione T		
	END
	
		IF 	@sCodEntita='TAG' OR isnull(@sCodEntita,'')=''
	BEGIN
		INSERT INTO T_Icone(CodEntita,CodTipo,CodStato,Descrizione)					
			SELECT 'TAG' AS CodEntita,
			 T.Codice AS CodTipo,
			 '' AS CodStato,
			 T.Descrizione 
		FROM T_TipoAgenda T		
	END
	
		IF 	@sCodEntita='FUT' OR isnull(@sCodEntita,'')=''
	BEGIN
		INSERT INTO T_Icone(CodEntita,CodTipo,CodStato,Descrizione)					
			SELECT 'FUT' AS CodEntita,
			 Codice AS CodTipo,
			 '' AS CodStato,
			 T.Descrizione
			 FROM T_SezioniFUT T
						
	END

		IF 	@sCodEntita='CNC' OR isnull(@sCodEntita,'')=''
	BEGIN
		INSERT INTO T_Icone(CodEntita,CodTipo,CodStato,Descrizione)					
			SELECT 'CNC' AS CodEntita,
			 Codice AS CodTipo,
			 '' AS CodStato,
			 T.Descrizione
			 FROM T_StatoConsensoCalcolato T			
	END

		IF 	@sCodEntita='CSG' OR isnull(@sCodEntita,'')=''
	BEGIN
		INSERT INTO T_Icone(CodEntita,CodTipo,CodStato,Descrizione)					
		SELECT 'CSG' AS CodEntita,
			   T.Codice AS CodTipo,
			   S.Codice AS CodStato,
			   T.Descrizione + ' - ' + S.Descrizione
		FROM T_TipoConsegna T
			CROSS JOIN T_StatoConsegna S
		UNION 	
			SELECT 'CSG' AS CodEntita,
			 T.Codice AS CodTipo,
			 '' AS CodStato,
			 T.Descrizione 
		FROM T_TipoConsegna T		
	END

		IF 	@sCodEntita='CSP' OR isnull(@sCodEntita,'')=''
	BEGIN
		INSERT INTO T_Icone(CodEntita,CodTipo,CodStato,Descrizione)					
		SELECT 'CSP' AS CodEntita,
			   T.Codice AS CodTipo,
			   S.Codice AS CodStato,
			   T.Descrizione + ' - ' + S.Descrizione
		FROM T_TipoConsegnaPaziente T
			CROSS JOIN T_StatoConsegnaPaziente S

	END

RETURN 0
	
END