USE [EcoStudentDB]
GO
/****** Object:  Table [dbo].[Achievements]    Script Date: 20.09.2025 0:53:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Achievements](
	[AchievementId] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[IconPath] [nvarchar](255) NULL,
	[RequiredPoints] [int] NOT NULL,
	[MaterialType] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[AchievementId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Challenges]    Script Date: 20.09.2025 0:53:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Challenges](
	[ChallengeId] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](150) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[TargetValue] [float] NOT NULL,
	[MaterialType] [nvarchar](50) NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[RewardPoints] [int] NOT NULL,
	[IsGroupChallenge] [bit] NULL,
	[IsActive] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[ChallengeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Groups]    Script Date: 20.09.2025 0:53:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Groups](
	[GroupId] [int] IDENTITY(1,1) NOT NULL,
	[GroupName] [nvarchar](100) NOT NULL,
	[Faculty] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[GroupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[News]    Script Date: 20.09.2025 0:53:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[News](
	[NewsId] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](150) NOT NULL,
	[Content] [nvarchar](max) NOT NULL,
	[PublishDate] [datetime] NULL,
	[ImagePath] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[NewsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Promotions]    Script Date: 20.09.2025 0:53:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Promotions](
	[PromotionId] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](200) NOT NULL,
	[Description] [nvarchar](1000) NOT NULL,
	[PointsCost] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[Category] [nvarchar](100) NULL,
	[DiscountValue] [nvarchar](50) NULL,
 CONSTRAINT [PK_Promotions] PRIMARY KEY CLUSTERED 
(
	[PromotionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RecyclingPoints]    Script Date: 20.09.2025 0:53:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RecyclingPoints](
	[PointId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
	[Address] [nvarchar](255) NULL,
	[WorkHours] [nvarchar](100) NULL,
	[AcceptedTypes] [nvarchar](255) NULL,
	[ContactPhone] [nvarchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[PointId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RecyclingSubmissions]    Script Date: 20.09.2025 0:53:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RecyclingSubmissions](
	[SubmissionId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[PointId] [int] NOT NULL,
	[MaterialType] [nvarchar](50) NOT NULL,
	[WeightKg] [float] NOT NULL,
	[SubmissionDate] [datetime] NULL,
	[PhotoPath] [nvarchar](255) NULL,
	[IsVerified] [bit] NULL,
	[PointsAwarded] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[SubmissionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserAchievements]    Script Date: 20.09.2025 0:53:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserAchievements](
	[UserAchievementId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[AchievementId] [int] NOT NULL,
	[DateEarned] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[UserAchievementId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserChallenges]    Script Date: 20.09.2025 0:53:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserChallenges](
	[UserChallengeId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[ChallengeId] [int] NOT NULL,
	[CurrentValue] [float] NULL,
	[IsCompleted] [bit] NULL,
	[CompletionDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[UserChallengeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserPromotions]    Script Date: 20.09.2025 0:53:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserPromotions](
	[UserPromotionId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[PromotionId] [int] NOT NULL,
	[PurchaseDate] [datetime2](7) NOT NULL,
	[IsUsed] [bit] NOT NULL,
	[UsedDate] [datetime2](7) NULL,
 CONSTRAINT [PK_UserPromotions] PRIMARY KEY CLUSTERED 
(
	[UserPromotionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 20.09.2025 0:53:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[FullName] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](150) NOT NULL,
	[PasswordHash] [nvarchar](255) NOT NULL,
	[GroupId] [int] NULL,
	[RegistrationDate] [datetime] NULL,
	[TotalPoints] [int] NULL,
	[AvatarPath] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Groups] ON 

INSERT [dbo].[Groups] ([GroupId], [GroupName], [Faculty]) VALUES (1, N'111', N'Не указан')
INSERT [dbo].[Groups] ([GroupId], [GroupName], [Faculty]) VALUES (2, N'52', N'Не указан')
SET IDENTITY_INSERT [dbo].[Groups] OFF
GO
SET IDENTITY_INSERT [dbo].[Promotions] ON 

INSERT [dbo].[Promotions] ([PromotionId], [Title], [Description], [PointsCost], [IsActive], [CreatedDate], [Category], [DiscountValue]) VALUES (4, N'Скидка 30% в столовой', N'Получите скидку 30% на любой обед в университетской столовой.', 500, 1, CAST(N'2025-09-19T23:43:12.2300000' AS DateTime2), N'Питание', N'30%')
INSERT [dbo].[Promotions] ([PromotionId], [Title], [Description], [PointsCost], [IsActive], [CreatedDate], [Category], [DiscountValue]) VALUES (5, N'Бесплатная печать 10 страниц', N'Распечатайте до 10 страниц бесплатно в библиотеке университета.', 250, 1, CAST(N'2025-09-19T23:43:12.2300000' AS DateTime2), N'Услуги', N'Бесплатно')
INSERT [dbo].[Promotions] ([PromotionId], [Title], [Description], [PointsCost], [IsActive], [CreatedDate], [Category], [DiscountValue]) VALUES (6, N'Скидка 50% на кофе', N'Получите скидку 50% на любой кофе в кафе университета.', 250, 1, CAST(N'2025-09-19T23:43:12.2300000' AS DateTime2), N'Питание', N'50%')
SET IDENTITY_INSERT [dbo].[Promotions] OFF
GO
SET IDENTITY_INSERT [dbo].[RecyclingPoints] ON 

INSERT [dbo].[RecyclingPoints] ([PointId], [Name], [Latitude], [Longitude], [Address], [WorkHours], [AcceptedTypes], [ContactPhone]) VALUES (3, N'Пункт сдачи "У столовой"', 55.7558, 37.6176, N'Возле столовой университета', N'08:00-20:00', N'Пластик, Бумага, Стекло, Металл, Батарейки', N'+7 (495) 123-45-67')
INSERT [dbo].[RecyclingPoints] ([PointId], [Name], [Latitude], [Longitude], [Address], [WorkHours], [AcceptedTypes], [ContactPhone]) VALUES (4, N'Пункт сдачи "У входа"', 55.75, 37.62, N'Возле главного входа в университет', N'07:00-22:00', N'Пластик, Бумага, Стекло, Металл, Батарейки', N'+7 (495) 234-56-78')
INSERT [dbo].[RecyclingPoints] ([PointId], [Name], [Latitude], [Longitude], [Address], [WorkHours], [AcceptedTypes], [ContactPhone]) VALUES (5, N'Пункт сдачи "У библиотеки"', 55.76, 37.615, N'Возле библиотеки университета', N'08:00-21:00', N'Пластик, Бумага, Стекло, Металл, Батарейки', N'+7 (495) 345-67-89')
SET IDENTITY_INSERT [dbo].[RecyclingPoints] OFF
GO
SET IDENTITY_INSERT [dbo].[RecyclingSubmissions] ON 

INSERT [dbo].[RecyclingSubmissions] ([SubmissionId], [UserId], [PointId], [MaterialType], [WeightKg], [SubmissionDate], [PhotoPath], [IsVerified], [PointsAwarded]) VALUES (1, 1, 3, N'Бумага', 2, CAST(N'2025-09-19T18:17:19.247' AS DateTime), NULL, 0, 10)
INSERT [dbo].[RecyclingSubmissions] ([SubmissionId], [UserId], [PointId], [MaterialType], [WeightKg], [SubmissionDate], [PhotoPath], [IsVerified], [PointsAwarded]) VALUES (2, 1, 5, N'Батарейки', 2, CAST(N'2025-09-19T18:24:05.670' AS DateTime), NULL, 0, 100)
INSERT [dbo].[RecyclingSubmissions] ([SubmissionId], [UserId], [PointId], [MaterialType], [WeightKg], [SubmissionDate], [PhotoPath], [IsVerified], [PointsAwarded]) VALUES (3, 1, 4, N'Пластик', 10, CAST(N'2025-09-19T23:15:26.563' AS DateTime), NULL, 0, 100)
INSERT [dbo].[RecyclingSubmissions] ([SubmissionId], [UserId], [PointId], [MaterialType], [WeightKg], [SubmissionDate], [PhotoPath], [IsVerified], [PointsAwarded]) VALUES (4, 1, 5, N'Стекло', 1000, CAST(N'2025-09-20T00:08:20.067' AS DateTime), NULL, 0, 15000)
INSERT [dbo].[RecyclingSubmissions] ([SubmissionId], [UserId], [PointId], [MaterialType], [WeightKg], [SubmissionDate], [PhotoPath], [IsVerified], [PointsAwarded]) VALUES (5, 7, 4, N'Пластик', 1000, CAST(N'2025-09-20T00:43:03.507' AS DateTime), NULL, 0, 10000)
SET IDENTITY_INSERT [dbo].[RecyclingSubmissions] OFF
GO
SET IDENTITY_INSERT [dbo].[UserPromotions] ON 

INSERT [dbo].[UserPromotions] ([UserPromotionId], [UserId], [PromotionId], [PurchaseDate], [IsUsed], [UsedDate]) VALUES (1, 1, 6, CAST(N'2025-09-19T23:48:46.2978470' AS DateTime2), 1, CAST(N'2025-09-20T00:23:50.8451359' AS DateTime2))
INSERT [dbo].[UserPromotions] ([UserPromotionId], [UserId], [PromotionId], [PurchaseDate], [IsUsed], [UsedDate]) VALUES (2, 1, 6, CAST(N'2025-09-19T23:49:13.6330210' AS DateTime2), 1, CAST(N'2025-09-19T23:56:47.7474446' AS DateTime2))
INSERT [dbo].[UserPromotions] ([UserPromotionId], [UserId], [PromotionId], [PurchaseDate], [IsUsed], [UsedDate]) VALUES (3, 1, 5, CAST(N'2025-09-20T00:08:27.9588377' AS DateTime2), 1, CAST(N'2025-09-20T00:23:49.1955038' AS DateTime2))
INSERT [dbo].[UserPromotions] ([UserPromotionId], [UserId], [PromotionId], [PurchaseDate], [IsUsed], [UsedDate]) VALUES (4, 1, 4, CAST(N'2025-09-20T00:08:29.6422027' AS DateTime2), 1, CAST(N'2025-09-20T00:23:47.4926577' AS DateTime2))
INSERT [dbo].[UserPromotions] ([UserPromotionId], [UserId], [PromotionId], [PurchaseDate], [IsUsed], [UsedDate]) VALUES (5, 1, 5, CAST(N'2025-09-20T00:30:05.1984916' AS DateTime2), 1, CAST(N'2025-09-20T00:30:26.6617093' AS DateTime2))
INSERT [dbo].[UserPromotions] ([UserPromotionId], [UserId], [PromotionId], [PurchaseDate], [IsUsed], [UsedDate]) VALUES (6, 1, 6, CAST(N'2025-09-20T00:30:13.9660629' AS DateTime2), 1, CAST(N'2025-09-20T00:30:25.2300031' AS DateTime2))
INSERT [dbo].[UserPromotions] ([UserPromotionId], [UserId], [PromotionId], [PurchaseDate], [IsUsed], [UsedDate]) VALUES (7, 1, 4, CAST(N'2025-09-20T00:30:19.7104087' AS DateTime2), 1, CAST(N'2025-09-20T00:30:23.3781937' AS DateTime2))
INSERT [dbo].[UserPromotions] ([UserPromotionId], [UserId], [PromotionId], [PurchaseDate], [IsUsed], [UsedDate]) VALUES (8, 1, 6, CAST(N'2025-09-20T00:30:32.8515081' AS DateTime2), 1, CAST(N'2025-09-20T00:30:44.7625348' AS DateTime2))
INSERT [dbo].[UserPromotions] ([UserPromotionId], [UserId], [PromotionId], [PurchaseDate], [IsUsed], [UsedDate]) VALUES (9, 1, 4, CAST(N'2025-09-20T00:30:35.6444024' AS DateTime2), 1, CAST(N'2025-09-20T00:30:43.3167783' AS DateTime2))
INSERT [dbo].[UserPromotions] ([UserPromotionId], [UserId], [PromotionId], [PurchaseDate], [IsUsed], [UsedDate]) VALUES (10, 1, 5, CAST(N'2025-09-20T00:30:38.7616989' AS DateTime2), 1, CAST(N'2025-09-20T00:30:42.0611721' AS DateTime2))
INSERT [dbo].[UserPromotions] ([UserPromotionId], [UserId], [PromotionId], [PurchaseDate], [IsUsed], [UsedDate]) VALUES (11, 1, 5, CAST(N'2025-09-20T00:30:49.3244619' AS DateTime2), 1, CAST(N'2025-09-20T00:32:14.4380747' AS DateTime2))
INSERT [dbo].[UserPromotions] ([UserPromotionId], [UserId], [PromotionId], [PurchaseDate], [IsUsed], [UsedDate]) VALUES (12, 1, 5, CAST(N'2025-09-20T00:36:08.4509148' AS DateTime2), 1, CAST(N'2025-09-20T00:36:29.6403864' AS DateTime2))
INSERT [dbo].[UserPromotions] ([UserPromotionId], [UserId], [PromotionId], [PurchaseDate], [IsUsed], [UsedDate]) VALUES (13, 1, 6, CAST(N'2025-09-20T00:36:10.1636685' AS DateTime2), 1, CAST(N'2025-09-20T00:36:15.9505242' AS DateTime2))
INSERT [dbo].[UserPromotions] ([UserPromotionId], [UserId], [PromotionId], [PurchaseDate], [IsUsed], [UsedDate]) VALUES (14, 1, 4, CAST(N'2025-09-20T00:36:11.2552780' AS DateTime2), 1, CAST(N'2025-09-20T00:36:14.4024507' AS DateTime2))
INSERT [dbo].[UserPromotions] ([UserPromotionId], [UserId], [PromotionId], [PurchaseDate], [IsUsed], [UsedDate]) VALUES (15, 7, 5, CAST(N'2025-09-20T00:43:07.7025170' AS DateTime2), 1, CAST(N'2025-09-20T00:43:15.4795486' AS DateTime2))
INSERT [dbo].[UserPromotions] ([UserPromotionId], [UserId], [PromotionId], [PurchaseDate], [IsUsed], [UsedDate]) VALUES (16, 7, 6, CAST(N'2025-09-20T00:43:09.2567323' AS DateTime2), 1, CAST(N'2025-09-20T00:43:14.1903968' AS DateTime2))
INSERT [dbo].[UserPromotions] ([UserPromotionId], [UserId], [PromotionId], [PurchaseDate], [IsUsed], [UsedDate]) VALUES (17, 7, 4, CAST(N'2025-09-20T00:43:10.4411451' AS DateTime2), 1, CAST(N'2025-09-20T00:43:12.7466813' AS DateTime2))
SET IDENTITY_INSERT [dbo].[UserPromotions] OFF
GO
SET IDENTITY_INSERT [dbo].[Users] ON 

INSERT [dbo].[Users] ([UserId], [FullName], [Email], [PasswordHash], [GroupId], [RegistrationDate], [TotalPoints], [AvatarPath]) VALUES (1, N'Ааа Ааа Ааа', N'qq@gmail.com', N'$2a$11$p9pv1jOWZUYQnJayWXdejOPT.zY.yN67MTNgX5x25xF.H.2/LKAp2', 1, CAST(N'2025-09-19T15:04:12.553' AS DateTime), 11160, NULL)
INSERT [dbo].[Users] ([UserId], [FullName], [Email], [PasswordHash], [GroupId], [RegistrationDate], [TotalPoints], [AvatarPath]) VALUES (2, N'Ббб Ббб Ббб', N'ww@gmail.com', N'$2a$11$tMHMfj08eMh4./J/RWVqX.0JmoUUeZ4sJkTanDeMcHXnRY8IoIENG', NULL, CAST(N'2025-09-19T16:20:53.940' AS DateTime), 0, NULL)
INSERT [dbo].[Users] ([UserId], [FullName], [Email], [PasswordHash], [GroupId], [RegistrationDate], [TotalPoints], [AvatarPath]) VALUES (3, N'А А А', N'ee@gmail.com', N'$2a$11$62P3tYcX.DrSBM/ZnHCU4OT1TKYyNxWcqCJMB1j0jPuyudGdYKjyu', NULL, CAST(N'2025-09-19T16:21:35.573' AS DateTime), 0, NULL)
INSERT [dbo].[Users] ([UserId], [FullName], [Email], [PasswordHash], [GroupId], [RegistrationDate], [TotalPoints], [AvatarPath]) VALUES (4, N'У У У', N'qqq@gmail.com', N'$2a$11$wPRqX/DoKEWMU.Nt.L7ksu2wVL0qJtMGdwuMKn5S/larGyshzJEDG', NULL, CAST(N'2025-09-19T16:38:10.683' AS DateTime), 0, NULL)
INSERT [dbo].[Users] ([UserId], [FullName], [Email], [PasswordHash], [GroupId], [RegistrationDate], [TotalPoints], [AvatarPath]) VALUES (5, N'А А А', N'yy@gmail.com', N'$2a$11$UlEPhIT2iI8VGkF.HJdTFOUSNsHtBDXe.4Y195OFO7wu2q2rTGNqm', NULL, CAST(N'2025-09-19T17:34:23.943' AS DateTime), 0, NULL)
INSERT [dbo].[Users] ([UserId], [FullName], [Email], [PasswordHash], [GroupId], [RegistrationDate], [TotalPoints], [AvatarPath]) VALUES (6, N'Р Р Р', N'aa@gmail.com', N'$2a$11$Mu3a2z5pMzoYxP58JUbEwev1VT7BDhVa9jEfqIB8FUH70fTDcEpyG', NULL, CAST(N'2025-09-19T17:39:05.137' AS DateTime), 0, NULL)
INSERT [dbo].[Users] ([UserId], [FullName], [Email], [PasswordHash], [GroupId], [RegistrationDate], [TotalPoints], [AvatarPath]) VALUES (7, N'Р О Р', N'ff@gmail.com', N'$2a$11$KzCmzOnoLdzRWiHiv8hfD.hRkgVSJ2cv3o74v4w1t9oo2W12xHLfu', 2, CAST(N'2025-09-20T00:39:44.160' AS DateTime), 9000, NULL)
SET IDENTITY_INSERT [dbo].[Users] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Users__A9D1053409C4592B]    Script Date: 20.09.2025 0:53:41 ******/
ALTER TABLE [dbo].[Users] ADD UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Challenges] ADD  DEFAULT ((0)) FOR [IsGroupChallenge]
GO
ALTER TABLE [dbo].[Challenges] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[News] ADD  DEFAULT (getdate()) FOR [PublishDate]
GO
ALTER TABLE [dbo].[Promotions] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Promotions] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[RecyclingSubmissions] ADD  DEFAULT (getdate()) FOR [SubmissionDate]
GO
ALTER TABLE [dbo].[RecyclingSubmissions] ADD  DEFAULT ((0)) FOR [IsVerified]
GO
ALTER TABLE [dbo].[UserAchievements] ADD  DEFAULT (getdate()) FOR [DateEarned]
GO
ALTER TABLE [dbo].[UserChallenges] ADD  DEFAULT ((0)) FOR [CurrentValue]
GO
ALTER TABLE [dbo].[UserChallenges] ADD  DEFAULT ((0)) FOR [IsCompleted]
GO
ALTER TABLE [dbo].[UserPromotions] ADD  DEFAULT (getdate()) FOR [PurchaseDate]
GO
ALTER TABLE [dbo].[UserPromotions] ADD  DEFAULT ((0)) FOR [IsUsed]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (getdate()) FOR [RegistrationDate]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT ((0)) FOR [TotalPoints]
GO
ALTER TABLE [dbo].[RecyclingSubmissions]  WITH CHECK ADD FOREIGN KEY([PointId])
REFERENCES [dbo].[RecyclingPoints] ([PointId])
GO
ALTER TABLE [dbo].[RecyclingSubmissions]  WITH CHECK ADD FOREIGN KEY([PointId])
REFERENCES [dbo].[RecyclingPoints] ([PointId])
GO
ALTER TABLE [dbo].[RecyclingSubmissions]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[RecyclingSubmissions]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[UserAchievements]  WITH CHECK ADD FOREIGN KEY([AchievementId])
REFERENCES [dbo].[Achievements] ([AchievementId])
GO
ALTER TABLE [dbo].[UserAchievements]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[UserAchievements]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[UserChallenges]  WITH CHECK ADD FOREIGN KEY([ChallengeId])
REFERENCES [dbo].[Challenges] ([ChallengeId])
GO
ALTER TABLE [dbo].[UserChallenges]  WITH CHECK ADD FOREIGN KEY([ChallengeId])
REFERENCES [dbo].[Challenges] ([ChallengeId])
GO
ALTER TABLE [dbo].[UserChallenges]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[UserChallenges]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[UserPromotions]  WITH CHECK ADD  CONSTRAINT [FK_UserPromotions_Promotions_PromotionId] FOREIGN KEY([PromotionId])
REFERENCES [dbo].[Promotions] ([PromotionId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserPromotions] CHECK CONSTRAINT [FK_UserPromotions_Promotions_PromotionId]
GO
ALTER TABLE [dbo].[UserPromotions]  WITH CHECK ADD  CONSTRAINT [FK_UserPromotions_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserPromotions] CHECK CONSTRAINT [FK_UserPromotions_Users_UserId]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD FOREIGN KEY([GroupId])
REFERENCES [dbo].[Groups] ([GroupId])
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD FOREIGN KEY([GroupId])
REFERENCES [dbo].[Groups] ([GroupId])
GO
