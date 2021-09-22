/* Create Database and Select this Database */
CREATE DATABASE IPCameras;
GO
USE IPCameras;
GO
/* Create Tables */
CREATE TABLE EmailSender (
    Id    INT           IDENTITY(1,1) NOT NULL,
    Email NCHAR (255)   NULL,
    Pass  NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED (Id ASC)
);
GO
CREATE TABLE FilesDirs
(
	Id INT NOT NULL PRIMARY KEY,
    Name NCHAR(250) NOT NULL,
    Path NCHAR(250) NOT NULL
);
GO
CREATE TABLE FilesFormats (
    Id   INT NOT NULL IDENTITY(1,1),
    avi  BIT DEFAULT ((0)) NOT NULL,
    mp4  BIT DEFAULT ((1)) NOT NULL,
    history_time TINYINT DEFAULT 1 NOT NULL,
    file_size INT NOT NULL,
    PRIMARY KEY CLUSTERED (Id ASC)
);
GO
CREATE TABLE MyCameras (
    Id                NVARCHAR (50) NOT NULL,
    urls              NCHAR (255)   NOT NULL,
    name              NCHAR (50)    NOT NULL,
    username          NCHAR (50)    NULL,
    password          NCHAR (50)    NULL,
    fps               INT           DEFAULT ((16)) NOT NULL,
    net_stream_port   NCHAR (255)   NULL,
    net_stream_prefix NCHAR (255)   NULL,
    net_stream        BIT           DEFAULT ((0)) NULL,
    Face_Detection    BIT           DEFAULT ((0)) NOT NULL,
    Face_Recognition  BIT           DEFAULT ((0)) NOT NULL,
    Brightness        INT           DEFAULT ((0)) NOT NULL,
    Contrast          INT           DEFAULT ((0)) NOT NULL,
    Darkness          INT           DEFAULT ((0)) NOT NULL,
    Recording         BIT           DEFAULT ((0)) NOT NULL,
    On_Move_Pic       BIT           DEFAULT ((0)) NOT NULL,
    On_Move_Rec       BIT           DEFAULT ((0)) NOT NULL,
    On_Move_SMS       BIT           DEFAULT ((0)) NOT NULL,
    On_Move_EMAIL     BIT           DEFAULT ((0)) NOT NULL,
    Move_Sensitivity  INT           DEFAULT ((2)) NOT NULL,
    Up_req            NCHAR (255)   NULL,
    Down_req          NCHAR (255)   NULL,
    Left_req          NCHAR (255)   NULL,
    Right_req         NCHAR (255)   NULL,
    isEsp32  BIT NOT NULL DEFAULT ((0)),
    PRIMARY KEY CLUSTERED (Id ASC)
);
GO
CREATE TABLE SMS
(
	Id    INT IDENTITY(1,1) NOT NULL,
    AccountSID NCHAR (255)   NOT NULL,
    AccountTOKEN  NCHAR(255) NOT NULL,
    Phone NCHAR(55) NOT NULL,
    PRIMARY KEY CLUSTERED (Id ASC)
);
GO
CREATE TABLE Users (
    Id        INT  IDENTITY(1,1) NOT NULL,
    FirstName NCHAR (255) NOT NULL,
    LastName  NCHAR (255) NOT NULL,
    Email     NCHAR (255) NOT NULL UNIQUE,
    Phone     NCHAR (255) NOT NULL,
    Password  NCHAR (50)  NOT NULL,
    Licences  NCHAR (100) NOT NULL,
    PRIMARY KEY CLUSTERED (Id ASC)
);
GO
CREATE TABLE Logged (
    Id   NCHAR (255) NOT NULL,
    PRIMARY KEY CLUSTERED (Id ASC)
);
GO
/* Insert Some Base Data */
INSERT INTO Users (FirstName, LastName, Email, Phone, Licences, Password)
    VALUES ('Alexandros', 'Platanios', 'alexandrosplatanios15@gmail.com',
        '6949277783', 'Admin', 'Platanios719791');
GO
INSERT INTO Users (FirstName, LastName, Email, Phone, Licences, Password)
    VALUES ('admin', 'admin', 'admin@admin.com',
        '', 'Admin', '1234');
GO
INSERT INTO FilesFormats (avi, mp4, history_time) VALUES (0, 1, 1);
GO
INSERT INTO FilesDirs (id, Name, Path) VALUES (1,'Videos','C:\\IPCameras_Files\\Video');
GO;
INSERT INTO FilesDirs (id, Name, Path) VALUES (2,'Pictures','C:\\IPCameras_Files\\Pictures');
GO
INSERT INTO MyCameras (id,urls,name,username,password,fps,isEsp32 ) VALUES
    (1,'http://192.168.1.30:81/stream','Camera 1','manos','manolis','16','1');
GO
INSERT INTO MyCameras (id,urls,name,username,password,fps,isEsp32 ) VALUES
    (2,'http://192.168.1.31:81/stream','Camera 2','manos','manolis','16','1');
GO




/*
UPDATE MyCameras SET net_stream_prefix='camera1', net_stream_port='80', net_stream='0'
        WHERE urls='http://192.168.1.30:81/stream' AND Name='Camera 1';


SELECT * FROM Users;
SELECT * FROM MyCameras;
SELECT * FROM EmailSender;
SELECT * FROM FilesFormats;


DROP DATABASE IPCameras;
DROP TABLE MyCameras;
DROP TABLE FilesDirs;
DROP TABLE FilesFormats;
*/