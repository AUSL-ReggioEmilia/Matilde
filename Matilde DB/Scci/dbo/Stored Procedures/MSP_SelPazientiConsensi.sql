CREATE PROCEDURE [dbo].[MSP_SelPazientiConsensi](@xParametri XML)
AS
BEGIN
		
	
				DECLARE @uIDPaziente AS UNIQUEIDENTIFIER	
	DECLARE @sCodTipoConsenso AS VARCHAR(20)
														
		DECLARE @sGUID AS VARCHAR(Max)
	
		SET @sGUID=(SELECT TOP 1 ValoreParametro.IDPaziente.value('.','VARCHAR(50)')
				FROM @xParametri.nodes('/Parametri/IDPaziente') as ValoreParametro(IDPaziente))
	IF ISNULL(@sGUID,'') <> '' SET @uIDPaziente=CONVERT(UNIQUEIDENTIFIER, @sGUID)	
	
		SET @sCodTipoConsenso=(SELECT TOP 1 ValoreParametro.CodTipoConsenso.value('.','VARCHAR(20)')
							FROM @xParametri.nodes('/Parametri/CodTipoConsenso') as ValoreParametro(CodTipoConsenso))	
	SET @sCodTipoConsenso=ISNULL(@sCodTipoConsenso,'')
		
				IF @uIDPaziente IS NOT NULL 
		BEGIN
			IF @sCodTipoConsenso <> ''
			BEGIN							
								SELECT  				  
						P.IDPaziente,
						P.CodTipoConsenso,
						P.CodSistemaProvenienza,
						P.IDProvenienza,
						P.CodStatoConsenso,
						P.DataConsenso,
						P.DataDisattivazione,
						P.CodOperatore,
						P.CognomeOperatore,
						P.NomeOperatore,
						P.ComputerOperatore																
				FROM T_PazientiConsensi AS P			
				WHERE IDPaziente=@uIDPaziente AND CodTipoConsenso=@sCodTipoConsenso
			END
			ELSE
				BEGIN
					SELECT  				  
							P.IDPaziente,
							P.CodTipoConsenso,
							P.CodSistemaProvenienza,
							P.IDProvenienza,
							P.CodStatoConsenso,
							P.DataConsenso,
							P.DataDisattivazione,
							P.CodOperatore,
							P.CognomeOperatore,
							P.NomeOperatore,
							P.ComputerOperatore																
					FROM T_PazientiConsensi AS P			
					WHERE IDPaziente=@uIDPaziente 
				END
		END
					  
	RETURN 0
END