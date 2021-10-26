CREATE PROCEDURE [dbo].[MSP_Fillers_DLookup_Note]
	@idpaziente		uniqueidentifier 			,@codTipoAll	varchar(20)				AS
BEGIN	

	

	SELECT	
			MS.AnteprimaTXT
					 						
				FROM 
					T_MovAlertAllergieAnamnesi	M (NOLOCK)					
						LEFT JOIN T_TipoAlertAllergiaAnamnesi T
								ON (M.CodTipoAlertAllergiaAnamnesi=T.Codice) 
						LEFT JOIN 
								(SELECT ID As IDScheda, CodScheda,Versione,IDEntita,AnteprimaRTF,AnteprimaTXT
								 FROM
									T_MovSchede (NOLOCK)
								 WHERE CodEntita='ALA' AND							
									Storicizzata=0 AND
									CodStatoScheda <> 'CA'
								) AS MS
							ON (MS.IDEntita=M.ID AND
								MS.CodScheda=T.CodScheda)					
					 WHERE 
								(M.IDPaziente=@idpaziente
								 OR
								 								 M.IDPaziente IN 
											(SELECT IDPazienteVecchio
											 FROM T_PazientiAlias WITH (NOLOCK)
											 WHERE 
												IDPaziente IN 
													(SELECT IDPaziente
													 FROM T_PazientiAlias WITH (NOLOCK)
													 WHERE IDPazienteVecchio=@idpaziente
													)
											)
										
								 )			
								 AND 			
									 M.CodStatoAlertAllergiaAnamnesi ='AT'
								 AND 			
							 M.CodTipoAlertAllergiaAnamnesi IN (@codTipoAll)
						 ORDER BY M.DataEvento,IDNum DESC 
END