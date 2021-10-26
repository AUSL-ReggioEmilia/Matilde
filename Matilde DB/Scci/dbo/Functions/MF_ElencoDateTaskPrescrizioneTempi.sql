CREATE FUNCTION [dbo].[MF_ElencoDateTaskPrescrizioneTempi]
(
	@IDPrescrizioneTempi AS UNIQUEIDENTIFIER
)
RETURNS VARCHAR(MAX)
AS
BEGIN
		
	
	DECLARE @sOutput AS VARCHAR(MAX)

	SET @sOutput=NULL
	
	IF @IDPrescrizioneTempi IS NOT NULL
	BEGIN
		SELECT @sOutput= ISNULL(@sOutput,'')
					+Convert(varchar(20),DataProgrammata,105) +' ' +  Convert(varchar(5),DataProgrammata,108) 
					+ ' ' + LEFT(CodStatoTaskInfermieristico,1) + '' 					
					+ CHAR(13) + CHAR(10)
		FROM 
			T_MovTaskInfermieristici
		WHERE			
			CodStatoTaskInfermieristico <> 'CA'	AND 
			IDGruppo=CONVERT(VARCHAR(50),@IDPrescrizioneTempi)
		GROUP BY DataProgrammata,CodStatoTaskInfermieristico	
		ORDER BY DataProgrammata,CodStatoTaskInfermieristico			
	END		
	
	SET @sOutput=ISNULL(@sOutput,'')
	IF @sOutput<> ''
		SET @sOutput=LEFT(@sOutput,LEN(@sOutput)-2)
	
	RETURN @sOutput 
END