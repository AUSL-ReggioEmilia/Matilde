CREATE FUNCTION [dbo].[MF_SelNumerositaScheda] (@uIDScheda UNIQUEIDENTIFIER, @uIDSchedaPadre UNIQUEIDENTIFIER,  @sCodEntita VARCHAR(20), @uIDEntita UNIQUEIDENTIFIER, @sCodScheda VARCHAR(20))
RETURNS @retInfoNumerositaScheda TABLE 
	(
	 IDSchedaOutput UNIQUEIDENTIFIER NULL,
	 QtaSchedeAttive INTEGER NULL,
	 QtaSchedeTotali INTEGER NULL,
	 QtaSchedeMassima INTEGER NULL,
	 QtaSchedeDisponibili INTEGER NULL,
	 MassimoNumero INTEGER NULL,
	 Debug VARCHAR(50)
	 )
AS
BEGIN
	
	 DECLARE @nQtaSchedeAttive AS INTEGER
	 DECLARE @nQtaSchedeTotali AS INTEGER
	 DECLARE @nQtaSchedeMassima AS INTEGER
	 DECLARE @nQtaSchedeDisponibili AS INTEGER
	 DECLARE @nMassimoNumero AS INTEGER
	 
	 	 
	 	 DECLARE @sDebug AS VARCHAR(50)

	 SET @sDebug= '0'
	
	 
	 	 SET @nQtaSchedeMassima= (SELECT NumerositaMassima FROM T_Schede  WHERE Codice=@sCodScheda)
	 SET @nQtaSchedeMassima=ISNULL(@nQtaSchedeMassima,1)

	
	 IF @uIDScheda IS NOT NULL AND @uIDSchedaPadre IS NULL AND @sCodEntita IS NULL AND @uIDEntita IS NULL AND @sCodScheda IS NULL
	 BEGIN
								SET @sDebug= '1'
		SELECT TOP 1						 
				@sCodScheda = CodScheda,								@sCodEntita = CodEntita,								@uIDEntita = IDEntita,									@uIDSchedaPadre= IDSchedaPadre				FROM T_MovSchede
		WHERE ID=@uIDScheda
	 END
	
				IF @uIDSchedaPadre IS NULL
	BEGIN
			SET @sDebug= '1' + ISNULL(@sCodScheda,'') + ISNULL(@sCodEntita,'') + ISNULL(CONVERT(VARCHAR(50),@uIDEntita),'')
									  
		SET @nQtaSchedeAttive=( SELECT							
								COUNT(ID) AS QtaSchedeAttive
							FROM T_MovSchede WITH (NOLOCK)					
							WHERE Storicizzata=0 														AND CodStatoScheda <> 'CA'											AND CodScheda=@sCodScheda												AND CodEntita=@sCodEntita												AND IDEntita=@uIDEntita												AND IDSchedaPadre IS NULL										)
			SELECT 							
			@nQtaSchedeTotali=COUNT(ID) , 
			@nMassimoNumero=MAX(Numero)
	FROM T_MovSchede WITH (NOLOCK)					
	WHERE Storicizzata=0 								AND CodScheda=@sCodScheda						AND CodEntita=@sCodEntita						AND IDEntita=@uIDEntita						AND IDSchedaPadre IS NULL				END	
	ELSE
	BEGIN
			SET @sDebug= '2_' + @sCodScheda + ' _ ' + CONVERT(VARCHAR(50),@uIDSchedaPadre)
		
		SET @nQtaSchedeAttive=( SELECT							
								COUNT(ID) AS QtaSchedeAttive
							FROM T_MovSchede WITH (NOLOCK)					
							WHERE Storicizzata=0 														AND CodStatoScheda <> 'CA'											AND CodScheda=@sCodScheda												AND CodEntita=@sCodEntita												AND IDSchedaPadre=@uIDSchedaPadre 							)

			SELECT 							
			@nQtaSchedeTotali=COUNT(ID) , 
			@nMassimoNumero=MAX(Numero)
	FROM T_MovSchede WITH (NOLOCK)					
	WHERE Storicizzata=0 								AND CodScheda=@sCodScheda						AND CodEntita=@sCodEntita						AND IDSchedaPadre=@uIDSchedaPadre 	END	

	 	SET @nQtaSchedeDisponibili= ISNULL(@nQtaSchedeMassima,0) - ISNULL(@nQtaSchedeAttive,0)
	IF @nQtaSchedeDisponibili <=0 SET @nQtaSchedeDisponibili=0

	INSERT @retInfoNumerositaScheda
	SELECT		
		@uIDScheda ,
		@nQtaSchedeAttive,
		@nQtaSchedeTotali,
		@nQtaSchedeMassima,
		@nQtaSchedeDisponibili,
		@nMassimoNumero,
		@sDebug

	RETURN;
END