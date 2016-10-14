CREATE TABLE [dbo].[Transaction] (
	[ID]		   INT IDENTITY(1,1) NOT NULL,
    [Account]      TEXT         NOT NULL,
    [Description]  TEXT         NOT NULL,
    [CurrencyCode] CHAR (3)     NOT NULL,
    [Amount]       DECIMAL (18) NOT NULL
);

