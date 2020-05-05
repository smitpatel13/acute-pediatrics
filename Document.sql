CREATE TABLE [dbo].[Document]
(
	[Id] INT NOT NULL IDENTITY PRIMARY KEY, 
    [Name] VARCHAR(50) NOT NULL, 
    [DocumentTypeId] INT NOT NULL, 
    [Path] VARCHAR(300) NOT NULL, 
    [TopicId] INT NOT NULL, 
    CONSTRAINT [FK_Document_Topic] FOREIGN KEY ([TopicId]) REFERENCES [Topic]([Id]), 
    CONSTRAINT [FK_Document_DocumentType] FOREIGN KEY ([DocumentTypeId]) REFERENCES [DocumentType]([Id])
)
