CREATE   PROCEDURE [dbo].[MSP_RendiAnonimo]
AS
BEGIN
		
	UPDATE T_Pazienti
	SET 
		Cognome= LEFT(Cognome,2)
	    ,Nome = LEFT(Nome,2)
	                                       
	    ,CodiceFiscale='CF' + CONVERT(VARCHAR(10),IDNum)
 		,ComuneNascita='ComNasc' + CONVERT(VARCHAR(10),IDNum)
		,LocalitaNascita='LocNas' + CONVERT(VARCHAR(10),IDNum)		
		,ComuneDomicilio=CONVERT(VARCHAR(10),IDNum)
		,IndirizzoDomicilio=CONVERT(VARCHAR(10),IDNum)
		,LocalitaDomicilio=CONVERT(VARCHAR(10),IDNum)
	    ,IndirizzoResidenza=CONVERT(VARCHAR(10),IDNum)
		,LocalitaResidenza=CONVERT(VARCHAR(10),IDNum)		
		,CodFiscMedicoBase	='CF' + CONVERT(VARCHAR(10),IDNum)	
		,CognomeNomeMedicoBase ='MB' + CONVERT(VARCHAR(10),IDNum)	
	WHERE ID NOT IN ('154F16C4-47A1-4E3E-8DD5-B249CFB68470','76E3C4CB-F806-4612-AF70-27B5243E9120','35B7957F-D6D5-43D5-96FE-B986A8079EAE','edb74860-41a6-4067-9fba-001f1f119192')
	
	UPDATE T_MovPazienti
	SET 
		Cognome= LEFT(Cognome,2)
	    ,Nome = LEFT(Nome,2)
	                                       
	    ,CodiceFiscale='CF' + CONVERT(VARCHAR(10),IDNum)
 		,ComuneNascita='ComNasc' + CONVERT(VARCHAR(10),IDNum)
		,LocalitaNascita='LocNas' + CONVERT(VARCHAR(10),IDNum)		
		,ComuneDomicilio=CONVERT(VARCHAR(10),IDNum)
		,IndirizzoDomicilio=CONVERT(VARCHAR(10),IDNum)
		,LocalitaDomicilio=CONVERT(VARCHAR(10),IDNum)
	    ,IndirizzoResidenza=CONVERT(VARCHAR(10),IDNum)
		,LocalitaResidenza=CONVERT(VARCHAR(10),IDNum)		
		,CodFiscMedicoBase	='CF' + CONVERT(VARCHAR(10),IDNum)	
		,CognomeNomeMedicoBase ='MB' + CONVERT(VARCHAR(10),IDNum)		
	WHERE 
		IDEpisodio NOT IN('7A7D62DD-D50D-4A35-B7A7-16C049B63E34','2BB4C2A9-44AE-4E07-A1BF-E4BF82D85A86','9f6cf344-0d09-40fb-80c9-f23a272512bf','C1E994C9-1E01-4E4C-869C-F08B7C66BB5E')		
				END




SET ANSI_NULLS ON