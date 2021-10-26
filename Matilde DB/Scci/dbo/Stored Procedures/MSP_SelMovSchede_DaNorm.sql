CREATE PROCEDURE [dbo].[MSP_SelMovSchede_DaNorm] 
	 @selectTop			int					,@dataUltMod		datetime = NULL											AS
BEGIN
	SET NOCOUNT ON;

	

	DECLARE @nRigheMancanti AS INTEGER
	
	CREATE TABLE #tmpID(
		ID UNIQUEIDENTIFIER
	)

	

	IF (@dataUltMod IS NULL)
	BEGIN
				INSERT INTO #tmpID(ID)
		SELECT TOP (@selectTop)
				MS.ID		
			FROM 
				T_MovSchede (NOLOCK) MS
				left join T_MovSchedeNormalizzate N (NOLOCK)
					ON MS.ID = N.IdScheda
			WHERE
				N.IdScheda IS NULL AND
				MS.Storicizzata = 0 AND
				MS.CodStatoScheda <> 'CA' 				
				
		SET @nRigheMancanti = @selectTop - @@ROWCOUNT 
		IF (@nRigheMancanti > 0 )
		BEGIN
			INSERT INTO #tmpID(ID)
			SELECT TOP (@nRigheMancanti)
					MS.ID				
				FROM 
					T_MovSchede (NOLOCK) MS
					inner join T_MovSchedeNormalizzate N (NOLOCK)
						ON MS.ID = N.IdScheda
				WHERE
					MS.Storicizzata = 0 AND
					MS.CodStatoScheda <> 'CA' AND
					MS.DataUltimaModifica > N.DataNormalizzazione 
					END
	END
	ELSE
	BEGIN
		
		INSERT INTO #tmpID(ID)
		SELECT TOP (@selectTop)
				MS.ID		
			FROM 
				T_MovSchede (NOLOCK) MS
				left join T_MovSchedeNormalizzate N (NOLOCK)
					ON MS.ID = N.IdScheda
			WHERE
				N.IdScheda IS NULL AND
				MS.Storicizzata = 0 AND
				MS.CodStatoScheda <> 'CA' 
				AND
				(MS.DataUltimaModifica < @dataUltMod AND
				 MS.DataUltimaModifica >= DATEADD(month, -1, @dataUltMod) ) 				
				
		
		SET @nRigheMancanti = @selectTop - @@ROWCOUNT
		
		IF (@nRigheMancanti > 0 )
		BEGIN
			INSERT INTO #tmpID(ID)
			SELECT TOP (@nRigheMancanti)
					MS.ID				
				FROM 
					T_MovSchede (NOLOCK) MS
					inner join T_MovSchedeNormalizzate N (NOLOCK)
						ON MS.ID = N.IdScheda
				WHERE
					MS.Storicizzata = 0 AND
					MS.CodStatoScheda <> 'CA' AND
					MS.DataUltimaModifica > N.DataNormalizzazione AND
					(MS.DataUltimaModifica < @dataUltMod AND
					 MS.DataUltimaModifica >= DATEADD(month, -1, @dataUltMod) 
					 )	
					END
	END

	SELECT 
			MS.ID, 
			MS.IDEntita, 
			MS.CodEntita, 
			MS.CodScheda, 
			MS.Numero, 
			MS.IDNum, 
			MS.DataUltimaModifica 
	FROM
		#tmpID T INNER JOIN T_MovSchede MS (NOLOCK)
			ON T.ID=MS.ID
	
	DROP TABLE #tmpID
END