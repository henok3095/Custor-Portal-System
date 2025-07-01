USE [CustorPortalDB]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Notifications]') AND type in (N'U'))
BEGIN
    EXEC('
    CREATE TABLE [dbo].[Notifications] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [UserId] INT NOT NULL,
        [Message] NVARCHAR(MAX) NOT NULL,
        [Link] NVARCHAR(MAX) NOT NULL,
        [Read] BIT NOT NULL DEFAULT 0,
        [Timestamp] DATETIME2 NOT NULL,
        CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Notifications_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserKey])
    );

    CREATE INDEX [IX_Notifications_UserId] ON [dbo].[Notifications] ([UserId]);
    ')
END 