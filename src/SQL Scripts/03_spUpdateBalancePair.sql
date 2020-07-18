SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE UpdateBalancePair
     @nvc_AccountCredit nvarchar(10),
	 @dcml_BalanceCredit decimal(12, 2),
	 @nvc_AccountDebit nvarchar(10),
	 @dcml_BalanceDebit decimal(12, 2)
AS
BEGIN
	SET NOCOUNT ON;

    UPDATE [dbo].[AccountTable]
	SET BALANCE = (
		CASE 
			WHEN ACCOUNTNUM = @nvc_AccountCredit THEN @dcml_BalanceCredit
			WHEN ACCOUNTNUM = @nvc_AccountDebit THEN @dcml_BalanceDebit
		END
			)
	WHERE ACCOUNTNUM IN (@nvc_AccountCredit, @nvc_AccountDebit)
	
END
GO
