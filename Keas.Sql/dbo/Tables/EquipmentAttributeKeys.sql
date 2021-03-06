CREATE TABLE [dbo].[EquipmentAttributeKeys]
(
	[Id]			INT				NOT NULL PRIMARY KEY IDENTITY, 
	[TeamId]		INT				NULL, 
	[Key]			NVARCHAR(MAX)	NOT NULL, 
	[Description]	NVARCHAR(50)	NOT NULL, 
    CONSTRAINT [FK_EquipmentAttributeKeys_Teams_TeamId] FOREIGN KEY ([TeamId]) REFERENCES [dbo].[Teams] ([Id]) ON DELETE CASCADE
)
