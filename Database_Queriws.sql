CREATE TABLE EmailSender (
    Id    INT           AUTO_INCREMENT NOT NULL,
    Email NCHAR (255)   NULL,
    Pass  NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED (Id ASC)
);

CREATE TABLE FilesDirs
(
	Id INT NOT NULL PRIMARY KEY,
    Name NCHAR(250) NOT NULL,
    Path NCHAR(250) NOT NULL
);

CREATE TABLE FilesFormats (
    Id   INT NOT NULL AUTO_INCREMENT,
    avi  BIT DEFAULT ((0)) NOT NULL,
    mp4  BIT DEFAULT ((0)) NOT NULL,
    webm BIT DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED (Id ASC)
);

CREATE TABLE MyCameras (
    Id               NVARCHAR (50) NOT NULL,
    urls              NCHAR (255)   NOT NULL,
    name              NCHAR (50)    NOT NULL,
    username          NCHAR (50)    NULL,
    password          NCHAR (50)    NULL,
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

CREATE TABLE SMS
(
	Id    INT AUTO_INCREMENT NOT NULL,
    AccountSID NCHAR (255)   NOT NULL,
    AccountTOKEN  NCHAR(255) NOT NULL,
    Phone NCHAR(55) NOT NULL,
    PRIMARY KEY CLUSTERED (Id ASC)
);

CREATE TABLE Users (
    Id        INT  AUTO_INCREMENT NOT NULL,
    FirstName NCHAR (255) NOT NULL,
    LastName  NCHAR (255) NOT NULL,
    Email     NCHAR (255) NOT NULL UNIQUE,
    Phone     NCHAR (255) NOT NULL,
    Password  NCHAR (50)  NOT NULL,
    Licences  NCHAR (100) NOT NULL,
    PRIMARY KEY CLUSTERED (Id ASC)
);

CREATE TABLE Logged (
    Id   NCHAR (255) NOT NULL,
    PRIMARY KEY CLUSTERED (Id ASC)
);






INSERT INTO Users (FirstName, LastName, Email, Phone, Licences, Password)
VALUES ('Alexandros', 'Platanios', 'alexandrosplatanios15@gmail.com',
        '6949277783', 'Admin', 'Platanios719791');

INSERT INTO Users (FirstName, LastName, Email, Phone, Licences, Password)
VALUES ('admin', 'admin', 'admin@admin.com',
        '', 'Admin', '1234');

SELECT * FROM Users;


DROP TABLE myCameras;