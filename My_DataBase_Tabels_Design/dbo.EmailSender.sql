CREATE TABLE [dbo].[EmailSender] (
    [Id]    INT           IDENTITY (1, 1) NOT NULL,
    [Email] NCHAR (255)   NULL,
    [Pass]  NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

