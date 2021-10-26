CREATE FUNCTION [dbo].[MF_PulisciRTF](@sIn AS VARCHAR(MAX))
RETURNS VARCHAR(MAX)
AS
BEGIN
	DECLARE @sOUT AS VARCHAR(MAX)
	DECLARE @sTEMP AS VARCHAR(MAX)
	DECLARE @nIColorTable AS VARCHAR(MAX)
	
	IF ISNULL(@sIn,'') <> ''
	BEGIN
		SET @sOUT=@sIn
		
				IF RIGHT(@sOUT,5)='}\r\n'
			SET @sOUT= LEFT(@sOUT,LEN(@sOUT)-4)
		
				SET @sOUT=
			REPLACE(@sOUT,'’','''')	
					
		
		IF PATINDEX('%\fonttbl%',@sOUT) >0
 		BEGIN
			
			SET @sTEMP = SUBSTRING(@sOUT,PATINDEX('%\fonttbl%',@sOUT)+8,LEN(@sOUT)-PATINDEX('%\fonttbl%',@sOUT)+8)
		
						IF PATINDEX('%\fonttbl%',@sTEMP) > 0
			BEGIN
								SET @sOUT=
					REPLACE(@sOUT,' {\fonttbl{\f0\fnil\fcharset0 Calibri;}} \viewkind4\uc1\pard\f0\fs17','')
				
				SET @sOUT=
					REPLACE(@sOUT,'\line{\fonttbl{\f0\fnil\fcharset0 Calibri;}} \viewkind4\uc1\pard\f0\fs17','\line')
				
																
													
								
								
								
				SET @sOUT=
					REPLACE(@sOUT,'{\fonttbl{\f0\fcharset0 Times New Roman;}{\f2\fcharset0 Calibri;}}','')		
				
				SET @sOUT=
					REPLACE(@sOUT,'\f2','\f0')		
						
								SET @sOUT=
					REPLACE(@sOUT,'A''','\''c0')	
								
			END
						
		END
		
				IF PATINDEX('%\colortbl%',@sOUT) >0
			BEGIN
			
								SET @sTEMP = SUBSTRING(@sOUT,PATINDEX('%\colortbl%',@sOUT)+9,LEN(@sOUT)-PATINDEX('%\colortbl%',@sOUT)+9)
								
								IF PATINDEX('%\colortbl%',@sTEMP) > 0
				BEGIN

				   SET @nIColorTable=PATINDEX('%{\colortbl%',@sOUT)
				   SET @sOUT=REPLACE(@sOUT,'{\colortbl','{\xcolortbl')		
				   SET @sOUT=REPLACE(@sOUT,';\red','\red')			
				   SET @sOUT=REPLACE(@sOUT,'blue255;}','blue255}')			
				   SET @sOUT=REPLACE(@sOUT,'blue0;}','blue0}')		
				   
				   SET @sOUT=LEFT(@sOUT,@nIColorTable-1) +
							'{\colortbl\red0\green0\blue0;\red255\green255\blue255;\red128\green128\blue128;\red128\green128\blue128;\red128\green128\blue128;\red128\green128\blue128;\red128\green128\blue128;\red128\green128\blue128;\red128\green128\blue128;\red128\green128\blue128;\red128\green128\blue128;\red128\green128\blue128;}' 
							+ RIGHT(@sOUT,LEN(@sOUT)-@nIColorTable)									   
				END			
			END
					
					
	END			
	ELSE
		SET @sOUT=@sIN		
		
		
			
	RETURN @sOUT

	END