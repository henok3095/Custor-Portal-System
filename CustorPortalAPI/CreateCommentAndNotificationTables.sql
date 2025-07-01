-- Create Comments table
CREATE TABLE [dbo].[Comments] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [TaskId] INT NULL,
    [DocumentId] INT NULL,
    [Text] NVARCHAR(MAX) NOT NULL,
    [UserId] INT NOT NULL,
    [Timestamp] DATETIME2 NOT NULL,
    [Mentions] NVARCHAR(MAX) NOT NULL DEFAULT '[]',
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [DeletedAt] DATETIME2 NULL,
    CONSTRAINT [PK_Comments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Comments_Tasks] FOREIGN KEY ([TaskId]) REFERENCES [dbo].[Tasks] ([TaskKey]),
    CONSTRAINT [FK_Comments_Files] FOREIGN KEY ([DocumentId]) REFERENCES [dbo].[Files] ([FileKey]),
    CONSTRAINT [FK_Comments_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserKey])
);

-- Create indexes for Comments
CREATE INDEX [IX_Comments_TaskId] ON [dbo].[Comments] ([TaskId]);
CREATE INDEX [IX_Comments_DocumentId] ON [dbo].[Comments] ([DocumentId]);
CREATE INDEX [IX_Comments_UserId] ON [dbo].[Comments] ([UserId]);

-- Create Notifications table
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

-- Create index for Notifications
CREATE INDEX [IX_Notifications_UserId] ON [dbo].[Notifications] ([UserId]); 