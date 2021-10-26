CREATE PROCEDURE [dbo].[MSP_SelScreenTile]
	@codScreen	VARCHAR (20)	AS
BEGIN

		
	

				SELECT * FROM T_ScreenTile
	WHERE CodScreen=@codScreen
	ORDER BY Riga ASC,Colonna ASC
	
END