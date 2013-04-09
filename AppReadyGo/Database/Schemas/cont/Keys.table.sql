﻿CREATE TABLE [cont].[Keys] (
	[ID]			INT IDENTITY(1,1)	NOT NULL,
	[Url]			NVARCHAR(256)		NOT NULL
);
GO

ALTER TABLE [cont].[Keys]
	ADD CONSTRAINT [PK_Keys] PRIMARY KEY CLUSTERED ([ID]);
GO