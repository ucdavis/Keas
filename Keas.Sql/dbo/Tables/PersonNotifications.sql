CREATE TABLE [dbo].[PersonNotifications]
(
	[Id] INT IDENTITY (1, 1) NOT NULL, 
    [Pending] BIT NOT NULL, 
    [NotificationEmail] NVARCHAR(256) NULL, 
    [PersonEmail] NVARCHAR(256) NOT NULL, 
    [PersonName] NVARCHAR(256) NOT NULL, 
    [PersonId] INT NULL, 
    [ActionDate] DATETIME2 NOT NULL, 
    [ActorName] NVARCHAR(256) NOT NULL, 
    [Action] NVARCHAR(50) NOT NULL, 
    [TeamId] INT NULL, 
    [NotificationDate] DATETIME2 NULL, 
    [ActorId] NVARCHAR(50) NOT NULL, 
    [Notes] NVARCHAR(256) NULL, 
    CONSTRAINT [PK_PersonNotifications] PRIMARY KEY CLUSTERED ([Id] ASC)
)
