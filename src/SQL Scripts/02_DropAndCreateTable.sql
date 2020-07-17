USE [AlterBank]
GO

ALTER TABLE [dbo].[AccountTable] DROP CONSTRAINT [DF_AccountTable_Balance]
GO

/****** Object:  Table [dbo].[AccountTable]    Script Date: 17.07.2020 20:11:49 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AccountTable]') AND type in (N'U'))
DROP TABLE [dbo].[AccountTable]
GO

/****** Object:  Table [dbo].[AccountTable]    Script Date: 17.07.2020 20:11:49 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AccountTable](
	[ACCOUNTNUM] [nvarchar](10) NOT NULL,
	[BALANCE] [decimal](12, 2) NOT NULL,
 CONSTRAINT [PK_AccountTable] PRIMARY KEY CLUSTERED 
(
	[ACCOUNTNUM] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[AccountTable] ADD  CONSTRAINT [DF_AccountTable_Balance]  DEFAULT ((0)) FOR [BALANCE]
GO


