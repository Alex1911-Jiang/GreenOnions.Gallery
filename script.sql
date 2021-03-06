USE [GreenOnionsGallery]
GO
/****** Object:  Table [dbo].[PixivIllustrator]    Script Date: 2021/5/16 18:41:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PixivIllustrator](
	[Id] [bigint] NOT NULL,
	[Illustrator] [nvarchar](50) NOT NULL,
	[Alias] [nvarchar](50) NULL,
 CONSTRAINT [PK_PixivAuthors] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PixivPictures]    Script Date: 2021/5/16 18:41:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PixivPictures](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Pid] [bigint] NOT NULL,
	[P] [int] NOT NULL,
	[PageCount] [int] NOT NULL,
	[Title] [nvarchar](900) NOT NULL,
	[Uid] [bigint] NOT NULL,
	[Url] [varchar](900) NOT NULL,
	[Grading] [int] NOT NULL,
	[Width] [int] NOT NULL,
	[Height] [int] NOT NULL,
	[Tags] [varchar](900) NULL,
 CONSTRAINT [PK_AnimePictures] PRIMARY KEY CLUSTERED 
(
	[Url] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RegisterVerify]    Script Date: 2021/5/16 18:41:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RegisterVerify](
	[Email] [varchar](320) NOT NULL,
	[EmailVerifyCode] [nchar](4) NOT NULL,
	[CreateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_RegisterVerify] PRIMARY KEY CLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TwitterIllustrator]    Script Date: 2021/5/16 18:41:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TwitterIllustrator](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Illustrator] [nvarchar](50) NOT NULL,
	[UserName] [nvarchar](50) NULL,
	[Alias] [nvarchar](50) NULL,
 CONSTRAINT [PK_TwitterAuthors] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TwitterPictures]    Script Date: 2021/5/16 18:41:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TwitterPictures](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Url] [varchar](900) NOT NULL,
	[Source] [varchar](900) NOT NULL,
	[Title] [nvarchar](900) NOT NULL,
	[Uid] [bigint] NOT NULL,
	[Grading] [int] NOT NULL,
	[Width] [int] NOT NULL,
	[Height] [int] NOT NULL,
	[Tags] [varchar](900) NULL,
 CONSTRAINT [PK_TwitterPictures] PRIMARY KEY CLUSTERED 
(
	[Url] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 2021/5/16 18:41:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Account] [varchar](30) NOT NULL,
	[Password] [nchar](32) NOT NULL,
	[NickName] [varchar](30) NOT NULL,
	[Email] [varchar](200) NOT NULL,
	[Permission] [int] NOT NULL,
	[LastLoginTime] [datetime] NOT NULL,
	[CreateTime] [datetime] NOT NULL,
	[ApiKey] [varchar](32) NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[PixivPictures] ADD  CONSTRAINT [DF_AnimePictures_SinglePicture]  DEFAULT ((1)) FOR [PageCount]
GO
ALTER TABLE [dbo].[PixivPictures]  WITH CHECK ADD  CONSTRAINT [FK_PixivPictures_PixivIllustrator] FOREIGN KEY([Uid])
REFERENCES [dbo].[PixivIllustrator] ([Id])
GO
ALTER TABLE [dbo].[PixivPictures] CHECK CONSTRAINT [FK_PixivPictures_PixivIllustrator]
GO
ALTER TABLE [dbo].[TwitterPictures]  WITH CHECK ADD  CONSTRAINT [FK_TwitterPictures_TwitterIllustrator] FOREIGN KEY([Uid])
REFERENCES [dbo].[TwitterIllustrator] ([Id])
GO
ALTER TABLE [dbo].[TwitterPictures] CHECK CONSTRAINT [FK_TwitterPictures_TwitterIllustrator]
GO
