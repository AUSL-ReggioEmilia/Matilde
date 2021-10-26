CREATE PROCEDURE [dbo].[MSP_SchedulaNormalizzazioneSchedeMancanti]
AS
BEGIN

	DECLARE @xXML AS XML
		
	SET @xXML=CONVERT(XML,'<Parametri>
									  <NumeroSchede>100</NumeroSchede>
								</Parametri>')

	INSERT INTO SCCILog.dbo.T_MovDataLog
			(Data,
			 DataUTC,
			 CodUtente,
			 ComputerName,
			 IpAddress,
			 CodEvento,
			 TipoOperazione,
			 Operazione,
			 LogPrima,
			 LogDopo
			 )
	VALUES
			( GETDATE()		
			 ,GETUTCDATE()
			 ,'WebNotificationService'
			 ,@@SERVERNAME
			 ,CONVERT(VARCHAR(50),CONNECTIONPROPERTY('local_net_address'))
			 ,'ScciNDB02'
			 ,1
			 ,'Nuovo'
			 ,NULL
			 ,@xXML
			 )
			
END