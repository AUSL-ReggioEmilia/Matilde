CREATE FUNCTION [dbo].[MF_SelNumerositaSchedaParametro] (@sParametro VARCHAR(20),@uIDScheda UNIQUEIDENTIFIER, @uIDSchedaPadre UNIQUEIDENTIFIER, @sCodEntita VARCHAR(20), @uIDEntita UNIQUEIDENTIFIER, @sCodScheda VARCHAR(20))
RETURNS INTEGER
BEGIN
	
	 DECLARE @nQtaSchedeAttive AS INTEGER
	 DECLARE @nQtaSchedeTotali AS INTEGER
	 DECLARE @nQtaSchedeMassima AS INTEGER
	 DECLARE @nQtaSchedeDisponibili AS INTEGER
	 DECLARE @nMassimoNumero AS INTEGER

	 DECLARE @nOutput AS INTEGER
	 
	 SELECT TOP 1 
		@nQtaSchedeAttive = QtaSchedeAttive ,
		@nQtaSchedeTotali = QtaSchedeTotali,
		@nQtaSchedeMassima = QtaSchedeMassima,
	    @nQtaSchedeDisponibili = QtaSchedeDisponibili,
	    @nMassimoNumero = MassimoNumero
	 FROM dbo.MF_SelNumerositaScheda(@uIDScheda,@uIDSchedaPadre,@sCodEntita, @uIDEntita, @sCodScheda)
	
	 SET @nOutput= CASE  @sParametro
					WHEN 'QtaSchedeAttive' THEN @nQtaSchedeAttive
					WHEN 'QtaSchedeTotali' THEN @nQtaSchedeTotali
					WHEN 'QtaSchedeMassima' THEN @nQtaSchedeMassima
					WHEN 'QtaSchedeDisponibili' THEN @nQtaSchedeDisponibili
					WHEN 'MassimoNumero' THEN @nMassimoNumero
				   ELSE 0
				   END
	 RETURN @nOutput
END