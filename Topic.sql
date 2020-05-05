CREATE TABLE [dbo].[Topic]
(
	[Id] INT NOT NULL IDENTITY PRIMARY KEY, 
    [Name] VARCHAR(50) NOT NULL, 
    [CategoryId] INT NOT NULL, 
    CONSTRAINT [FK_Topic_Category] FOREIGN KEY ([CategoryId]) REFERENCES [Category]([Id])
)
