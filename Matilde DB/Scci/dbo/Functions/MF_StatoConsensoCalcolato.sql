CREATE FUNCTION [dbo].[MF_StatoConsensoCalcolato](@uIDPaziente UNIQUEIDENTIFIER)
RETURNS VARCHAR (20)
AS
BEGIN
		
	DECLARE @sOut AS VARCHAR(20)
	DECLARE @sCodStatoConsensoGenerico AS VARCHAR(20)
	DECLARE @sCodStatoConsensoDossier AS VARCHAR(20)
	DECLARE @sCodStatoConsensoDossierStorico AS VARCHAR(20)
	DECLARE @QtaConsensoNegato AS BIT	
	
	SET @sOut='ND'		

	SET @sCodStatoConsensoDossierStorico=(SELECT TOP 1 CodStatoConsenso FROM T_PazientiConsensi WHERE IDPaziente=@uIDPaziente AND CodTipoConsenso='DossierStorico')
	SET @sCodStatoConsensoDossier=(SELECT TOP 1 CodStatoConsenso FROM T_PazientiConsensi WHERE IDPaziente=@uIDPaziente AND CodTipoConsenso='Dossier')
	SET @sCodStatoConsensoGenerico=(SELECT TOP 1 CodStatoConsenso FROM T_PazientiConsensi WHERE IDPaziente=@uIDPaziente AND CodTipoConsenso='Generico')
	
	
		IF (ISNULL(@sCodStatoConsensoGenerico,'') ='NO' OR ISNULL(@sCodStatoConsensoDossier,'')='NO')
	BEGIN		
		SET @sOut ='NO'
	END
	ELSE
	BEGIN				
				IF ISNULL(@sCodStatoConsensoDossierStorico,'') ='SI'
			BEGIN		
				SET @sOut='DS'	
			END
		ELSE	
			BEGIN							
				IF ISNULL(@sCodStatoConsensoDossier,'')='SI'
				BEGIN					
										SET @sOut='DO'
				END
				ELSE
					BEGIN						
						IF ISNULL(@sCodStatoConsensoGenerico,'') ='SI'
							BEGIN						
																SET @sOut='GN'
							END
						ELSE
							BEGIN
																SET @sOut='ND'
							END
					END
			END
	END
	RETURN @sOut
END