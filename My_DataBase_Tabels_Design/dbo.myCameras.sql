CREATE TABLE [dbo].[myCameras]
(
	[Id] NVARCHAR(50) NOT NULL PRIMARY KEY, 
    [urls] NCHAR(333) NOT NULL, 
    [name] NCHAR(50) NOT NULL, 
    [Face_Detection] BIT NOT NULL DEFAULT 0, 
    [Face_Recognition] BIT NOT NULL DEFAULT 0, 
    [Brightness] INT NOT NULL DEFAULT 0, 
    [Contrast] INT NOT NULL DEFAULT 0, 
    [Darkness] INT NOT NULL DEFAULT 0
)
