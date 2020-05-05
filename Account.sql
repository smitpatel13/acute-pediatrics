CREATE TABLE [dbo].[Account]
(
	[Id] INT NOT NULL IDENTITY PRIMARY KEY, 
	[Email] VARCHAR(50) NOT NULL,
    [FirstName] VARCHAR(50) NOT NULL, 
	[LastName] VARCHAR(50) NOT NULL, 
    [Password] VARCHAR(50) NOT NULL, 
    [RoleId] INT NOT NULL, 
    CONSTRAINT [FK_Account_Role] FOREIGN KEY ([RoleId]) REFERENCES [Role]([Id])
)
