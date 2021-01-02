CREATE TABLE [dbo].[SMS]
(
	[Id]    INT           IDENTITY (1, 1) NOT NULL,
    [AccountSID] NCHAR (255)   NOT NULL,
    [AccountTOKEN]  NCHAR(255) NOT NULL,
    [Phone] NCHAR(55) NOT NULL, 
    PRIMARY KEY CLUSTERED ([Id] ASC)
)
