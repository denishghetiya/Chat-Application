USE [master]
GO
/****** Object:  Database [ChatApplication]    Script Date: 05-10-2026 16:29:45 ******/
CREATE DATABASE [ChatApplication]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'ChatApplication', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLSERVER1\MSSQL\DATA\ChatApplication.mdf' , SIZE = 73728KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'ChatApplication_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLSERVER1\MSSQL\DATA\ChatApplication_log.ldf' , SIZE = 139264KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [ChatApplication] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [ChatApplication].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [ChatApplication] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [ChatApplication] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [ChatApplication] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [ChatApplication] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [ChatApplication] SET ARITHABORT OFF 
GO
ALTER DATABASE [ChatApplication] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [ChatApplication] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [ChatApplication] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [ChatApplication] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [ChatApplication] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [ChatApplication] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [ChatApplication] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [ChatApplication] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [ChatApplication] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [ChatApplication] SET  DISABLE_BROKER 
GO
ALTER DATABASE [ChatApplication] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [ChatApplication] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [ChatApplication] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [ChatApplication] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [ChatApplication] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [ChatApplication] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [ChatApplication] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [ChatApplication] SET RECOVERY FULL 
GO
ALTER DATABASE [ChatApplication] SET  MULTI_USER 
GO
ALTER DATABASE [ChatApplication] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [ChatApplication] SET DB_CHAINING OFF 
GO
ALTER DATABASE [ChatApplication] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [ChatApplication] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [ChatApplication] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [ChatApplication] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'ChatApplication', N'ON'
GO
ALTER DATABASE [ChatApplication] SET QUERY_STORE = ON
GO
ALTER DATABASE [ChatApplication] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [ChatApplication]
GO
/****** Object:  User [denish]    Script Date: 05-10-2026 16:29:46 ******/
CREATE USER [denish] FOR LOGIN [denish] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_ddladmin] ADD MEMBER [denish]
GO
ALTER ROLE [db_datareader] ADD MEMBER [denish]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [denish]
GO
/****** Object:  Table [dbo].[FriendList]    Script Date: 05-10-2026 16:29:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FriendList](
	[FriendId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[FriendUserId] [int] NULL,
	[CreatedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[FriendId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FriendRequests]    Script Date: 05-10-2026 16:29:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FriendRequests](
	[RequestId] [int] IDENTITY(1,1) NOT NULL,
	[FromUserId] [int] NULL,
	[ToUserId] [int] NULL,
	[Status] [nvarchar](20) NULL,
	[RequestedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[RequestId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GroupList]    Script Date: 05-10-2026 16:29:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GroupList](
	[GroupId] [int] IDENTITY(1,1) NOT NULL,
	[GroupName] [nvarchar](100) NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[GroupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GroupMembers]    Script Date: 05-10-2026 16:29:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GroupMembers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[GroupId] [int] NULL,
	[UserId] [int] NULL,
	[IsAdmin] [bit] NULL,
	[CreatedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Messages]    Script Date: 05-10-2026 16:29:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Messages](
	[MessageId] [int] IDENTITY(1,1) NOT NULL,
	[FromUserId] [int] NULL,
	[ToUserId] [int] NULL,
	[GroupId] [int] NULL,
	[MessageText] [nvarchar](max) NULL,
	[FileUrl] [nvarchar](300) NULL,
	[SentAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[MessageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 05-10-2026 16:29:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](max) NOT NULL,
	[Email] [nvarchar](max) NOT NULL,
	[Password] [nvarchar](max) NOT NULL,
	[ImageName] [nvarchar](max) NULL,
	[ResetToken] [nvarchar](max) NULL,
	[ResetTokenExpiry] [datetime] NULL,
	[IsOnline] [bit] NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[FriendList] ON 
GO
INSERT [dbo].[FriendList] ([FriendId], [UserId], [FriendUserId], [CreatedDate]) VALUES (2105, 1, 2, CAST(N'2026-04-18T16:43:53.597' AS DateTime))
GO
INSERT [dbo].[FriendList] ([FriendId], [UserId], [FriendUserId], [CreatedDate]) VALUES (2106, 2008, 2, CAST(N'2026-04-18T16:44:27.983' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[FriendList] OFF
GO
SET IDENTITY_INSERT [dbo].[GroupList] ON 
GO
INSERT [dbo].[GroupList] ([GroupId], [GroupName], [CreatedBy], [CreatedDate]) VALUES (2016, N'Group A', 1, CAST(N'2025-08-19T12:17:33.877' AS DateTime))
GO
INSERT [dbo].[GroupList] ([GroupId], [GroupName], [CreatedBy], [CreatedDate]) VALUES (3016, N'Group B', 1, CAST(N'2026-04-18T16:47:30.580' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[GroupList] OFF
GO
SET IDENTITY_INSERT [dbo].[GroupMembers] ON 
GO
INSERT [dbo].[GroupMembers] ([Id], [GroupId], [UserId], [IsAdmin], [CreatedDate]) VALUES (2038, 2016, 1, 1, CAST(N'2025-08-19T12:17:33.947' AS DateTime))
GO
INSERT [dbo].[GroupMembers] ([Id], [GroupId], [UserId], [IsAdmin], [CreatedDate]) VALUES (2040, 2016, 2, NULL, CAST(N'2025-08-19T16:23:21.857' AS DateTime))
GO
INSERT [dbo].[GroupMembers] ([Id], [GroupId], [UserId], [IsAdmin], [CreatedDate]) VALUES (3022, 3016, 1, 1, CAST(N'2026-04-18T16:47:30.680' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[GroupMembers] OFF
GO
SET IDENTITY_INSERT [dbo].[Messages] ON 
GO
INSERT [dbo].[Messages] ([MessageId], [FromUserId], [ToUserId], [GroupId], [MessageText], [FileUrl], [SentAt]) VALUES (1, 1, 2, NULL, N'2SZfY/+p129whAL+EAOhQA==', NULL, CAST(N'2025-08-18T18:45:22.683' AS DateTime))
GO
INSERT [dbo].[Messages] ([MessageId], [FromUserId], [ToUserId], [GroupId], [MessageText], [FileUrl], [SentAt]) VALUES (2, 1, 2, NULL, N'2FAMbZunUuXU/3qknijKeg==', NULL, CAST(N'2025-08-18T18:45:29.213' AS DateTime))
GO
INSERT [dbo].[Messages] ([MessageId], [FromUserId], [ToUserId], [GroupId], [MessageText], [FileUrl], [SentAt]) VALUES (3, 1, 3, NULL, N'2SZfY/+p129whAL+EAOhQA==', NULL, CAST(N'2025-08-18T18:45:38.620' AS DateTime))
GO
INSERT [dbo].[Messages] ([MessageId], [FromUserId], [ToUserId], [GroupId], [MessageText], [FileUrl], [SentAt]) VALUES (4, 1, 3, NULL, N'2FAMbZunUuXU/3qknijKeg==', NULL, CAST(N'2025-08-18T18:45:44.903' AS DateTime))
GO
INSERT [dbo].[Messages] ([MessageId], [FromUserId], [ToUserId], [GroupId], [MessageText], [FileUrl], [SentAt]) VALUES (7, 1, 2, NULL, N'2SZfY/+p129whAL+EAOhQA==', NULL, CAST(N'2025-08-19T12:16:27.233' AS DateTime))
GO
INSERT [dbo].[Messages] ([MessageId], [FromUserId], [ToUserId], [GroupId], [MessageText], [FileUrl], [SentAt]) VALUES (8, 1, 2, NULL, N'2FAMbZunUuXU/3qknijKeg==', NULL, CAST(N'2025-08-19T12:16:33.840' AS DateTime))
GO
INSERT [dbo].[Messages] ([MessageId], [FromUserId], [ToUserId], [GroupId], [MessageText], [FileUrl], [SentAt]) VALUES (9, 1, NULL, 2016, N'2FAMbZunUuXU/3qknijKeg==', NULL, CAST(N'2025-08-19T12:17:44.523' AS DateTime))
GO
INSERT [dbo].[Messages] ([MessageId], [FromUserId], [ToUserId], [GroupId], [MessageText], [FileUrl], [SentAt]) VALUES (10, 1, NULL, 2016, N'264Y5aoAaZtbBblmibaHhg==', NULL, CAST(N'2025-08-19T12:17:50.530' AS DateTime))
GO
INSERT [dbo].[Messages] ([MessageId], [FromUserId], [ToUserId], [GroupId], [MessageText], [FileUrl], [SentAt]) VALUES (11, 1, NULL, 2016, N'2FAMbZunUuXU/3qknijKeg==', NULL, CAST(N'2025-08-19T15:44:32.347' AS DateTime))
GO
INSERT [dbo].[Messages] ([MessageId], [FromUserId], [ToUserId], [GroupId], [MessageText], [FileUrl], [SentAt]) VALUES (12, 1, 2, NULL, N'2SZfY/+p129whAL+EAOhQA==', NULL, CAST(N'2025-08-19T15:45:24.393' AS DateTime))
GO
INSERT [dbo].[Messages] ([MessageId], [FromUserId], [ToUserId], [GroupId], [MessageText], [FileUrl], [SentAt]) VALUES (13, 1, 2, NULL, N'2SZfY/+p129whAL+EAOhQA==', NULL, CAST(N'2025-08-19T16:04:47.813' AS DateTime))
GO
INSERT [dbo].[Messages] ([MessageId], [FromUserId], [ToUserId], [GroupId], [MessageText], [FileUrl], [SentAt]) VALUES (14, 1, 2, NULL, N'hu3JR0hlbI+gs63yjQi97A==', NULL, CAST(N'2025-08-19T16:08:55.890' AS DateTime))
GO
INSERT [dbo].[Messages] ([MessageId], [FromUserId], [ToUserId], [GroupId], [MessageText], [FileUrl], [SentAt]) VALUES (15, 1, 2, NULL, N'DXU9SXyYluHwiOwFdDsnMA==', NULL, CAST(N'2025-08-19T16:09:01.573' AS DateTime))
GO
INSERT [dbo].[Messages] ([MessageId], [FromUserId], [ToUserId], [GroupId], [MessageText], [FileUrl], [SentAt]) VALUES (16, 2, 1, NULL, N'LMrEIQhfB9ISwUworLNtww==', NULL, CAST(N'2025-08-19T16:09:10.167' AS DateTime))
GO
INSERT [dbo].[Messages] ([MessageId], [FromUserId], [ToUserId], [GroupId], [MessageText], [FileUrl], [SentAt]) VALUES (17, 2, 1, NULL, N'oBODqXT8zbUUCZC3sbpjZA==', NULL, CAST(N'2025-08-19T16:09:12.770' AS DateTime))
GO
INSERT [dbo].[Messages] ([MessageId], [FromUserId], [ToUserId], [GroupId], [MessageText], [FileUrl], [SentAt]) VALUES (18, 1, NULL, 2016, N'2FAMbZunUuXU/3qknijKeg==', NULL, CAST(N'2025-08-19T16:10:32.950' AS DateTime))
GO
INSERT [dbo].[Messages] ([MessageId], [FromUserId], [ToUserId], [GroupId], [MessageText], [FileUrl], [SentAt]) VALUES (19, 1, NULL, 2016, N'2FAMbZunUuXU/3qknijKeg==', NULL, CAST(N'2025-08-19T16:10:40.877' AS DateTime))
GO
INSERT [dbo].[Messages] ([MessageId], [FromUserId], [ToUserId], [GroupId], [MessageText], [FileUrl], [SentAt]) VALUES (20, 1, NULL, 2016, N'2FAMbZunUuXU/3qknijKeg==', NULL, CAST(N'2025-08-19T16:23:53.230' AS DateTime))
GO
INSERT [dbo].[Messages] ([MessageId], [FromUserId], [ToUserId], [GroupId], [MessageText], [FileUrl], [SentAt]) VALUES (21, 1, 2, NULL, N'2SZfY/+p129whAL+EAOhQA==', NULL, CAST(N'2025-08-19T16:24:07.623' AS DateTime))
GO
INSERT [dbo].[Messages] ([MessageId], [FromUserId], [ToUserId], [GroupId], [MessageText], [FileUrl], [SentAt]) VALUES (22, 2, 1, NULL, N'2SZfY/+p129whAL+EAOhQA==', NULL, CAST(N'2025-08-19T16:45:52.167' AS DateTime))
GO
INSERT [dbo].[Messages] ([MessageId], [FromUserId], [ToUserId], [GroupId], [MessageText], [FileUrl], [SentAt]) VALUES (23, 1, 2, NULL, N'2SZfY/+p129whAL+EAOhQA==', NULL, CAST(N'2025-08-19T16:45:59.800' AS DateTime))
GO
INSERT [dbo].[Messages] ([MessageId], [FromUserId], [ToUserId], [GroupId], [MessageText], [FileUrl], [SentAt]) VALUES (24, 1, 2, NULL, N'2SZfY/+p129whAL+EAOhQA==', NULL, CAST(N'2025-08-19T16:46:26.480' AS DateTime))
GO
INSERT [dbo].[Messages] ([MessageId], [FromUserId], [ToUserId], [GroupId], [MessageText], [FileUrl], [SentAt]) VALUES (25, 1, 2, NULL, N'2SZfY/+p129whAL+EAOhQA==', NULL, CAST(N'2025-08-19T16:47:31.017' AS DateTime))
GO
INSERT [dbo].[Messages] ([MessageId], [FromUserId], [ToUserId], [GroupId], [MessageText], [FileUrl], [SentAt]) VALUES (26, 1, 2, NULL, N'2SZfY/+p129whAL+EAOhQA==', NULL, CAST(N'2025-08-19T16:47:41.257' AS DateTime))
GO
INSERT [dbo].[Messages] ([MessageId], [FromUserId], [ToUserId], [GroupId], [MessageText], [FileUrl], [SentAt]) VALUES (27, 1, 2, NULL, N'2SZfY/+p129whAL+EAOhQA==', NULL, CAST(N'2025-08-19T17:03:08.377' AS DateTime))
GO
INSERT [dbo].[Messages] ([MessageId], [FromUserId], [ToUserId], [GroupId], [MessageText], [FileUrl], [SentAt]) VALUES (1002, 1, 2008, NULL, N'2SZfY/+p129whAL+EAOhQA==', NULL, CAST(N'2026-04-18T16:45:16.503' AS DateTime))
GO
INSERT [dbo].[Messages] ([MessageId], [FromUserId], [ToUserId], [GroupId], [MessageText], [FileUrl], [SentAt]) VALUES (1003, 2008, 1, NULL, N'O2FYU2F8c7O0x3fje/eRqw==', NULL, CAST(N'2026-04-18T16:45:27.830' AS DateTime))
GO
INSERT [dbo].[Messages] ([MessageId], [FromUserId], [ToUserId], [GroupId], [MessageText], [FileUrl], [SentAt]) VALUES (1004, 1, 2008, NULL, N'3hJhzD7mD2Zpb5QHUi/BVQ==', NULL, CAST(N'2026-04-18T16:46:07.190' AS DateTime))
GO
INSERT [dbo].[Messages] ([MessageId], [FromUserId], [ToUserId], [GroupId], [MessageText], [FileUrl], [SentAt]) VALUES (1005, 1, NULL, 3016, N'hu3JR0hlbI+gs63yjQi97A==', NULL, CAST(N'2026-04-18T16:48:01.283' AS DateTime))
GO
INSERT [dbo].[Messages] ([MessageId], [FromUserId], [ToUserId], [GroupId], [MessageText], [FileUrl], [SentAt]) VALUES (1006, 2008, NULL, 3016, N'K57jij3qfAP7GsqW0sVJbw==', NULL, CAST(N'2026-04-18T16:48:11.980' AS DateTime))
GO
INSERT [dbo].[Messages] ([MessageId], [FromUserId], [ToUserId], [GroupId], [MessageText], [FileUrl], [SentAt]) VALUES (1007, 2, NULL, 3016, N'2FAMbZunUuXU/3qknijKeg==', NULL, CAST(N'2026-04-18T16:48:59.913' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[Messages] OFF
GO
SET IDENTITY_INSERT [dbo].[Users] ON 
GO
INSERT [dbo].[Users] ([UserId], [Username], [Email], [Password], [ImageName], [ResetToken], [ResetTokenExpiry], [IsOnline]) VALUES (1, N'denish', N'denish@gmail.com', N'a', N'shiv_20250714161325647.png', N'a8e3722d-650a-47a3-9e79-d1e85b11780a', CAST(N'2025-07-20T09:36:32.113' AS DateTime), 1)
GO
INSERT [dbo].[Users] ([UserId], [Username], [Email], [Password], [ImageName], [ResetToken], [ResetTokenExpiry], [IsOnline]) VALUES (2, N'Jay', N'jay@gmail.com', N'a', N'jay photo_20250716180042084.jpg', NULL, NULL, 1)
GO
INSERT [dbo].[Users] ([UserId], [Username], [Email], [Password], [ImageName], [ResetToken], [ResetTokenExpiry], [IsOnline]) VALUES (3, N'Parth', N'parth@gmail.com', N'a', N'a_20250716180305892.jpg', NULL, NULL, 0)
GO
INSERT [dbo].[Users] ([UserId], [Username], [Email], [Password], [ImageName], [ResetToken], [ResetTokenExpiry], [IsOnline]) VALUES (4, N'lalo', N'lalo@gmail.com', N'a', N'DALL·E 2025-01-17 14.54.30 - A scenic view of Udaipur, India, showcasing the iconic Lake Pichola with its serene waters, the Lake Palace beautifully situated in the middle of the _20250729155930336.webp', NULL, NULL, 0)
GO
INSERT [dbo].[Users] ([UserId], [Username], [Email], [Password], [ImageName], [ResetToken], [ResetTokenExpiry], [IsOnline]) VALUES (5, N'Ronak', N'ronak@gmail.com', N'a', N'shiv_20250714171747818.png', NULL, NULL, 0)
GO
INSERT [dbo].[Users] ([UserId], [Username], [Email], [Password], [ImageName], [ResetToken], [ResetTokenExpiry], [IsOnline]) VALUES (6, N'Meet', N'meet@gmail.com', N'a', N'shiv_20250714171828936.png', NULL, NULL, 0)
GO
INSERT [dbo].[Users] ([UserId], [Username], [Email], [Password], [ImageName], [ResetToken], [ResetTokenExpiry], [IsOnline]) VALUES (7, N'test', N'test@gmail.com', N'a', N's1_20250720154010433.png', N'7f47dad5-3ace-4592-b413-1d13b7472e02', CAST(N'2025-07-20T09:34:28.160' AS DateTime), 0)
GO
INSERT [dbo].[Users] ([UserId], [Username], [Email], [Password], [ImageName], [ResetToken], [ResetTokenExpiry], [IsOnline]) VALUES (8, N'Ashish', N'Ashish@gmail.com', N'a', N'DALL·E 2025-01-17 14.54.30 - A scenic view of Udaipur, India, showcasing the iconic Lake Pichola with its serene waters, the Lake Palace beautifully situated in the middle of the _20250729160305910.webp', NULL, NULL, 0)
GO
INSERT [dbo].[Users] ([UserId], [Username], [Email], [Password], [ImageName], [ResetToken], [ResetTokenExpiry], [IsOnline]) VALUES (1008, N'Nirav', N'nirav@gmail.com', N'a', N's1_20250813165324885.png', NULL, NULL, 0)
GO
INSERT [dbo].[Users] ([UserId], [Username], [Email], [Password], [ImageName], [ResetToken], [ResetTokenExpiry], [IsOnline]) VALUES (1009, N'Ravi', N'ravi@gmail.com', N'a', N'shiv_20250813165348407.png', NULL, NULL, 0)
GO
INSERT [dbo].[Users] ([UserId], [Username], [Email], [Password], [ImageName], [ResetToken], [ResetTokenExpiry], [IsOnline]) VALUES (1010, N'Fun', N'fun@gmail.com', N'a', N'shiv_20250813165427310.png', NULL, NULL, 0)
GO
INSERT [dbo].[Users] ([UserId], [Username], [Email], [Password], [ImageName], [ResetToken], [ResetTokenExpiry], [IsOnline]) VALUES (2008, N'Hiren', N'hiren@gmail.com', N'a', N'1000053384_20260418164036690.webp', NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[Users] OFF
GO
ALTER TABLE [dbo].[FriendRequests] ADD  DEFAULT (getdate()) FOR [RequestedAt]
GO
ALTER TABLE [dbo].[Messages] ADD  DEFAULT (getdate()) FOR [SentAt]
GO
ALTER TABLE [dbo].[FriendList]  WITH CHECK ADD  CONSTRAINT [fk_FriendList_FriendUserId_Users_UserId] FOREIGN KEY([FriendUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[FriendList] CHECK CONSTRAINT [fk_FriendList_FriendUserId_Users_UserId]
GO
ALTER TABLE [dbo].[FriendList]  WITH CHECK ADD  CONSTRAINT [fk_FriendList_UserId_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[FriendList] CHECK CONSTRAINT [fk_FriendList_UserId_Users_UserId]
GO
ALTER TABLE [dbo].[FriendRequests]  WITH CHECK ADD  CONSTRAINT [fk_FriendRequests_FromUserId_Users_UserId] FOREIGN KEY([FromUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[FriendRequests] CHECK CONSTRAINT [fk_FriendRequests_FromUserId_Users_UserId]
GO
ALTER TABLE [dbo].[FriendRequests]  WITH CHECK ADD  CONSTRAINT [fk_FriendRequests_ToUserId_Users_UserId] FOREIGN KEY([ToUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[FriendRequests] CHECK CONSTRAINT [fk_FriendRequests_ToUserId_Users_UserId]
GO
ALTER TABLE [dbo].[GroupList]  WITH CHECK ADD  CONSTRAINT [fk_Groups_CreatedBy_Users_UserId] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[GroupList] CHECK CONSTRAINT [fk_Groups_CreatedBy_Users_UserId]
GO
ALTER TABLE [dbo].[GroupMembers]  WITH CHECK ADD  CONSTRAINT [fk_GroupMembers_GroupId_Groups_GroupId] FOREIGN KEY([GroupId])
REFERENCES [dbo].[GroupList] ([GroupId])
GO
ALTER TABLE [dbo].[GroupMembers] CHECK CONSTRAINT [fk_GroupMembers_GroupId_Groups_GroupId]
GO
ALTER TABLE [dbo].[GroupMembers]  WITH CHECK ADD  CONSTRAINT [fk_GroupMembers_UserId_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[GroupMembers] CHECK CONSTRAINT [fk_GroupMembers_UserId_Users_UserId]
GO
ALTER TABLE [dbo].[Messages]  WITH CHECK ADD  CONSTRAINT [fk_Messages_FromUserId_Users_UserId] FOREIGN KEY([FromUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Messages] CHECK CONSTRAINT [fk_Messages_FromUserId_Users_UserId]
GO
ALTER TABLE [dbo].[Messages]  WITH CHECK ADD  CONSTRAINT [fk_Messages_GroupId_Groups_GroupId] FOREIGN KEY([GroupId])
REFERENCES [dbo].[GroupList] ([GroupId])
GO
ALTER TABLE [dbo].[Messages] CHECK CONSTRAINT [fk_Messages_GroupId_Groups_GroupId]
GO
ALTER TABLE [dbo].[Messages]  WITH CHECK ADD  CONSTRAINT [fk_Messages_ToUserId_Users_UserId] FOREIGN KEY([ToUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Messages] CHECK CONSTRAINT [fk_Messages_ToUserId_Users_UserId]
GO
ALTER TABLE [dbo].[FriendRequests]  WITH CHECK ADD CHECK  (([Status]='Rejected' OR [Status]='Accepted' OR [Status]='Pending'))
GO
USE [master]
GO
ALTER DATABASE [ChatApplication] SET  READ_WRITE 
GO
