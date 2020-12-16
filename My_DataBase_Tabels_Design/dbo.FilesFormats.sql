CREATE TABLE [dbo].[FilesFormats] (
    [Id]   INT NOT NULL IDENTITY(1,1),
    [avi]  BIT DEFAULT ((0)) NOT NULL,
    [mp4]  BIT DEFAULT ((0)) NOT NULL,
    [webm] BIT DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

