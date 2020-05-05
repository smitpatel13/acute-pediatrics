/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

-- Insert static data for Role table
DELETE FROM [dbo].[Role]
INSERT INTO [dbo].[Role]
VALUES(1, 'Educator')
INSERT INTO [dbo].[Role]
VALUES(2, 'Staff')
GO

-- Insert static date for the DocumentType table
DELETE FROM [dbo].[DocumentType]
INSERT INTO [dbo].[DocumentType]
VALUES(1, 'PDF')
INSERT INTO [dbo].[DocumentType]
VALUES(2, 'Video')
GO

-- An initial admin account used only for development
INSERT INTO [dbo].[Account]([Email], [FirstName], [LastName], [Password], [RoleId])
VALUES('admin@email.com', 'Admin', 'Admin', 'Admin', 1)
INSERT INTO [dbo].[Account]([Email], [FirstName], [LastName], [Password], [RoleId])
VALUES('smit@email.com', 'Smit', 'Patel', 'Smit', 2)
INSERT INTO [dbo].[Account]([Email], [FirstName], [LastName], [Password], [RoleId])
VALUES('riket@email.com', 'Riket', 'Patel', 'Riket', 2)
INSERT INTO [dbo].[Account]([Email], [FirstName], [LastName], [Password], [RoleId])
VALUES('christian@email.com', 'Christian', 'Serad', 'Christian', 2)
GO
