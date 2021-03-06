IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_UserPreference_User]') AND parent_object_id = OBJECT_ID(N'[dbo].[UserPreference]'))
ALTER TABLE [dbo].[UserPreference] DROP CONSTRAINT [FK_UserPreference_User]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_UserPreference_Preference]') AND parent_object_id = OBJECT_ID(N'[dbo].[UserPreference]'))
ALTER TABLE [dbo].[UserPreference] DROP CONSTRAINT [FK_UserPreference_Preference]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_User_Study]') AND parent_object_id = OBJECT_ID(N'[dbo].[User]'))
ALTER TABLE [dbo].[User] DROP CONSTRAINT [FK_User_Study]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_User_Sex]') AND parent_object_id = OBJECT_ID(N'[dbo].[User]'))
ALTER TABLE [dbo].[User] DROP CONSTRAINT [FK_User_Sex]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_User_Role]') AND parent_object_id = OBJECT_ID(N'[dbo].[User]'))
ALTER TABLE [dbo].[User] DROP CONSTRAINT [FK_User_Role]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_User_Position]') AND parent_object_id = OBJECT_ID(N'[dbo].[User]'))
ALTER TABLE [dbo].[User] DROP CONSTRAINT [FK_User_Position]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_User_Language]') AND parent_object_id = OBJECT_ID(N'[dbo].[User]'))
ALTER TABLE [dbo].[User] DROP CONSTRAINT [FK_User_Language]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_User_Country]') AND parent_object_id = OBJECT_ID(N'[dbo].[User]'))
ALTER TABLE [dbo].[User] DROP CONSTRAINT [FK_User_Country]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_User_Agency]') AND parent_object_id = OBJECT_ID(N'[dbo].[User]'))
ALTER TABLE [dbo].[User] DROP CONSTRAINT [FK_User_Agency]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_User_Age]') AND parent_object_id = OBJECT_ID(N'[dbo].[User]'))
ALTER TABLE [dbo].[User] DROP CONSTRAINT [FK_User_Age]
GO
/****** Object:  Table [dbo].[UserPreference]    Script Date: 24/07/2015 12:45:17 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserPreference]') AND type in (N'U'))
DROP TABLE [dbo].[UserPreference]
GO
/****** Object:  Table [dbo].[User]    Script Date: 24/07/2015 12:45:17 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[User]') AND type in (N'U'))
DROP TABLE [dbo].[User]
GO
/****** Object:  Table [dbo].[Study]    Script Date: 24/07/2015 12:45:17 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Study]') AND type in (N'U'))
DROP TABLE [dbo].[Study]
GO
/****** Object:  Table [dbo].[Sex]    Script Date: 24/07/2015 12:45:17 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Sex]') AND type in (N'U'))
DROP TABLE [dbo].[Sex]
GO
/****** Object:  Table [dbo].[Role]    Script Date: 24/07/2015 12:45:17 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Role]') AND type in (N'U'))
DROP TABLE [dbo].[Role]
GO
/****** Object:  Table [dbo].[Preference]    Script Date: 24/07/2015 12:45:17 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Preference]') AND type in (N'U'))
DROP TABLE [dbo].[Preference]
GO
/****** Object:  Table [dbo].[Position]    Script Date: 24/07/2015 12:45:17 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Position]') AND type in (N'U'))
DROP TABLE [dbo].[Position]
GO
/****** Object:  Table [dbo].[Localisation]    Script Date: 24/07/2015 12:45:17 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Localisation]') AND type in (N'U'))
DROP TABLE [dbo].[Localisation]
GO
/****** Object:  Table [dbo].[Language]    Script Date: 24/07/2015 12:45:17 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Language]') AND type in (N'U'))
DROP TABLE [dbo].[Language]
GO
/****** Object:  Table [dbo].[Country]    Script Date: 24/07/2015 12:45:17 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Country]') AND type in (N'U'))
DROP TABLE [dbo].[Country]
GO
/****** Object:  Table [dbo].[Agency]    Script Date: 24/07/2015 12:45:17 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Agency]') AND type in (N'U'))
DROP TABLE [dbo].[Agency]
GO
/****** Object:  Table [dbo].[Age]    Script Date: 24/07/2015 12:45:17 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Age]') AND type in (N'U'))
DROP TABLE [dbo].[Age]
GO
/****** Object:  Table [dbo].[Age]    Script Date: 24/07/2015 12:45:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Age]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Age](
	[AgeCode] [nchar](10) NOT NULL,
 CONSTRAINT [PK_Age] PRIMARY KEY CLUSTERED 
(
	[AgeCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Agency]    Script Date: 24/07/2015 12:45:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Agency]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Agency](
	[AgencyCode] [nchar](10) NOT NULL,
 CONSTRAINT [PK_Agency] PRIMARY KEY CLUSTERED 
(
	[AgencyCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Country]    Script Date: 24/07/2015 12:45:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Country]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Country](
	[CountryCode] [nchar](10) NOT NULL,
 CONSTRAINT [PK_Country] PRIMARY KEY CLUSTERED 
(
	[CountryCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Language]    Script Date: 24/07/2015 12:45:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Language]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Language](
	[LanguageCode] [nchar](10) NOT NULL,
 CONSTRAINT [PK_Language] PRIMARY KEY CLUSTERED 
(
	[LanguageCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Localisation]    Script Date: 24/07/2015 12:45:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Localisation]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Localisation](
	[TableName] [nchar](10) NULL,
	[Code] [nchar](10) NULL,
	[Lang] [nchar](10) NULL,
	[Description] [nvarchar](max) NULL,
	[LocalisationID] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_Localisation] PRIMARY KEY CLUSTERED 
(
	[LocalisationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Position]    Script Date: 24/07/2015 12:45:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Position]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Position](
	[PositionCode] [nchar](10) NOT NULL,
 CONSTRAINT [PK_Position] PRIMARY KEY CLUSTERED 
(
	[PositionCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Preference]    Script Date: 24/07/2015 12:45:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Preference]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Preference](
	[PreferenceCode] [nchar](10) NOT NULL,
 CONSTRAINT [PK_Preference] PRIMARY KEY CLUSTERED 
(
	[PreferenceCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Role]    Script Date: 24/07/2015 12:45:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Role]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Role](
	[RoleCode] [nchar](10) NOT NULL,
	[RoleDescription] [nvarchar](50) NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[RoleCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Sex]    Script Date: 24/07/2015 12:45:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Sex]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Sex](
	[SexCode] [nchar](10) NOT NULL,
 CONSTRAINT [PK_Sex] PRIMARY KEY CLUSTERED 
(
	[SexCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Study]    Script Date: 24/07/2015 12:45:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Study]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Study](
	[StudyCode] [nchar](10) NOT NULL,
 CONSTRAINT [PK_Study] PRIMARY KEY CLUSTERED 
(
	[StudyCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[User]    Script Date: 24/07/2015 12:45:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[User]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[User](
	[UserCode] [nvarchar](40) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Surname] [nvarchar](50) NOT NULL,
	[Password] [nvarchar](50) NOT NULL,
	[Email] [nvarchar](50) NOT NULL,
	[Age] [nchar](10) NULL,
	[Country] [nchar](10) NULL,
	[Position] [nchar](10) NULL,
	[Sex] [nchar](10) NULL,
	[Study] [nchar](10) NULL,
	[Agency] [nchar](10) NULL,
	[Lang] [nchar](10) NULL,
	[Role] [nchar](10) NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[UserCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[UserPreference]    Script Date: 24/07/2015 12:45:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserPreference]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[UserPreference](
	[UserPreferenceCode] [bigint] IDENTITY(1,1) NOT NULL,
	[UserCode] [nvarchar](40) NOT NULL,
	[PreferenceCode] [nchar](10) NOT NULL,
 CONSTRAINT [PK_UserPreference] PRIMARY KEY CLUSTERED 
(
	[UserPreferenceCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
INSERT [dbo].[Age] ([AgeCode]) VALUES (N'83        ')
INSERT [dbo].[Age] ([AgeCode]) VALUES (N'84        ')
INSERT [dbo].[Age] ([AgeCode]) VALUES (N'85        ')
INSERT [dbo].[Age] ([AgeCode]) VALUES (N'86        ')
INSERT [dbo].[Agency] ([AgencyCode]) VALUES (N'101       ')
INSERT [dbo].[Agency] ([AgencyCode]) VALUES (N'102       ')
INSERT [dbo].[Agency] ([AgencyCode]) VALUES (N'103       ')
INSERT [dbo].[Agency] ([AgencyCode]) VALUES (N'104       ')
INSERT [dbo].[Agency] ([AgencyCode]) VALUES (N'105       ')
INSERT [dbo].[Agency] ([AgencyCode]) VALUES (N'106       ')
INSERT [dbo].[Agency] ([AgencyCode]) VALUES (N'107       ')
INSERT [dbo].[Agency] ([AgencyCode]) VALUES (N'108       ')
INSERT [dbo].[Agency] ([AgencyCode]) VALUES (N'109       ')
INSERT [dbo].[Agency] ([AgencyCode]) VALUES (N'110       ')
INSERT [dbo].[Agency] ([AgencyCode]) VALUES (N'111       ')
INSERT [dbo].[Agency] ([AgencyCode]) VALUES (N'112       ')
INSERT [dbo].[Agency] ([AgencyCode]) VALUES (N'113       ')
INSERT [dbo].[Agency] ([AgencyCode]) VALUES (N'114       ')
INSERT [dbo].[Agency] ([AgencyCode]) VALUES (N'115       ')
INSERT [dbo].[Agency] ([AgencyCode]) VALUES (N'116       ')
INSERT [dbo].[Agency] ([AgencyCode]) VALUES (N'117       ')
INSERT [dbo].[Agency] ([AgencyCode]) VALUES (N'118       ')
INSERT [dbo].[Agency] ([AgencyCode]) VALUES (N'119       ')
INSERT [dbo].[Agency] ([AgencyCode]) VALUES (N'120       ')
INSERT [dbo].[Country] ([CountryCode]) VALUES (N'123       ')
INSERT [dbo].[Country] ([CountryCode]) VALUES (N'124       ')
INSERT [dbo].[Country] ([CountryCode]) VALUES (N'125       ')
INSERT [dbo].[Country] ([CountryCode]) VALUES (N'126       ')
INSERT [dbo].[Country] ([CountryCode]) VALUES (N'127       ')
INSERT [dbo].[Country] ([CountryCode]) VALUES (N'128       ')
INSERT [dbo].[Country] ([CountryCode]) VALUES (N'129       ')
INSERT [dbo].[Country] ([CountryCode]) VALUES (N'130       ')
INSERT [dbo].[Country] ([CountryCode]) VALUES (N'132       ')
INSERT [dbo].[Country] ([CountryCode]) VALUES (N'133       ')
INSERT [dbo].[Country] ([CountryCode]) VALUES (N'134       ')
INSERT [dbo].[Country] ([CountryCode]) VALUES (N'135       ')
INSERT [dbo].[Country] ([CountryCode]) VALUES (N'136       ')
INSERT [dbo].[Country] ([CountryCode]) VALUES (N'137       ')
INSERT [dbo].[Country] ([CountryCode]) VALUES (N'138       ')
INSERT [dbo].[Country] ([CountryCode]) VALUES (N'139       ')
INSERT [dbo].[Country] ([CountryCode]) VALUES (N'140       ')
INSERT [dbo].[Country] ([CountryCode]) VALUES (N'141       ')
INSERT [dbo].[Country] ([CountryCode]) VALUES (N'142       ')
INSERT [dbo].[Country] ([CountryCode]) VALUES (N'143       ')
INSERT [dbo].[Country] ([CountryCode]) VALUES (N'144       ')
INSERT [dbo].[Language] ([LanguageCode]) VALUES (N'en        ')
INSERT [dbo].[Language] ([LanguageCode]) VALUES (N'it        ')
SET IDENTITY_INSERT [dbo].[Localisation] ON 

INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'COUNTRY   ', N'123       ', N'it        ', N'Piemonte', 1)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'COUNTRY   ', N'124       ', N'it        ', N'Valle d''Aosta', 2)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'COUNTRY   ', N'125       ', N'it        ', N'Lombardia', 3)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'COUNTRY   ', N'126       ', N'it        ', N'Bolzano-Bozen', 4)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'COUNTRY   ', N'127       ', N'it        ', N'Trento', 5)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'COUNTRY   ', N'128       ', N'it        ', N'Veneto', 6)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'COUNTRY   ', N'129       ', N'it        ', N'Friuli-Venezia Giulia', 7)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'COUNTRY   ', N'130       ', N'it        ', N'Liguria', 8)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'COUNTRY   ', N'132       ', N'it        ', N'Toscana', 9)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'COUNTRY   ', N'133       ', N'it        ', N'Umbria', 10)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'COUNTRY   ', N'134       ', N'it        ', N'Marche', 11)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'COUNTRY   ', N'135       ', N'it        ', N'Lazio', 12)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'COUNTRY   ', N'136       ', N'it        ', N'Abruzzo', 13)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'COUNTRY   ', N'137       ', N'it        ', N'Molise', 14)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'COUNTRY   ', N'138       ', N'it        ', N'Campania', 15)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'COUNTRY   ', N'139       ', N'it        ', N'Puglia', 16)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'COUNTRY   ', N'140       ', N'it        ', N'Basilicata', 17)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'COUNTRY   ', N'141       ', N'it        ', N'Calabria', 18)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'COUNTRY   ', N'142       ', N'it        ', N'Sicilia', 19)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'COUNTRY   ', N'143       ', N'it        ', N'Sardegna', 20)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'COUNTRY   ', N'144       ', N'it        ', N'ESTERO', 21)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'SEX       ', N'm         ', N'it        ', N'Maschio', 22)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'SEX       ', N'f         ', N'it        ', N'Femmina', 23)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'AGE       ', N'83        ', N'it        ', N'Fino a 14 anni', 24)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'AGE       ', N'84        ', N'it        ', N'15 - 24 anni', 25)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'AGE       ', N'85        ', N'it        ', N'25 - 64 anni', 26)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'AGE       ', N'86        ', N'it        ', N'65 anni e oltre', 27)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'STUDY     ', N'87        ', N'it        ', N'Universitario', 28)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'STUDY     ', N'88        ', N'it        ', N'Diploma', 29)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'STUDY     ', N'89        ', N'it        ', N'Licenza media o elementare', 30)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'POSITION  ', N'90        ', N'it        ', N'Imprenditore, amministratore o dirigente', 31)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'POSITION  ', N'91        ', N'it        ', N'Dirigente pubblico', 32)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'POSITION  ', N'92        ', N'it        ', N'Docente / ricercatore', 33)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'POSITION  ', N'93        ', N'it        ', N'Giornalista', 34)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'POSITION  ', N'94        ', N'it        ', N'Libero professionista', 35)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'POSITION  ', N'95        ', N'it        ', N'Lavoratore in proprio', 36)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'POSITION  ', N'96        ', N'it        ', N'Impiegato / quadro', 37)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'POSITION  ', N'97        ', N'it        ', N'Operaio', 38)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'POSITION  ', N'98        ', N'it        ', N'Studente', 39)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'POSITION  ', N'99        ', N'it        ', N'Disoccupato / in cerca di occupazione', 40)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'POSITION  ', N'10        ', N'it        ', N'Altro (pensionato, casalinga, ...)', 41)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'AGENCY    ', N'101       ', N'it        ', N'Istituzioni/Ammin. centrali (Governo, Parlamento, Ministeri)', 42)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'AGENCY    ', N'102       ', N'it        ', N'Ammin. locali (Comune, provincia, camera di commercio,...)', 43)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'AGENCY    ', N'103       ', N'it        ', N'Enti pubblici di ricerca', 44)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'AGENCY    ', N'104       ', N'it        ', N'Enti privati di ricerca', 45)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'AGENCY    ', N'105       ', N'it        ', N'Università', 46)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'AGENCY    ', N'106       ', N'it        ', N'Scuole', 47)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'AGENCY    ', N'107       ', N'it        ', N'ASL, ospedali pubblici, istituti di cura privati', 48)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'AGENCY    ', N'108       ', N'it        ', N'Altre pubbliche amministrazioni', 49)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'AGENCY    ', N'109       ', N'it        ', N'Organizzazioni internazionali', 50)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'AGENCY    ', N'110       ', N'it        ', N'Sindacati, associazioni imprenditoriali', 51)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'AGENCY    ', N'111       ', N'it        ', N'Associazioni di volontariato / no profit', 52)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'AGENCY    ', N'112       ', N'it        ', N'Agricoltura', 53)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'AGENCY    ', N'113       ', N'it        ', N'Industria', 54)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'AGENCY    ', N'114       ', N'it        ', N'Telecomunicazioni', 55)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'AGENCY    ', N'115       ', N'it        ', N'Radio, TV, Stampa / Agenzie di Stampa', 56)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'AGENCY    ', N'116       ', N'it        ', N'Studi professionali', 57)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'AGENCY    ', N'117       ', N'it        ', N'Credito, assicurazioni', 58)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'AGENCY    ', N'118       ', N'it        ', N'Commercio', 59)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'AGENCY    ', N'119       ', N'it        ', N'Altre società di servizi', 60)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'AGENCY    ', N'120       ', N'it        ', N'Altro', 61)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'LANGUAGE  ', N'it        ', N'it        ', N'Italiano', 62)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'LANGUAGE  ', N'en        ', N'it        ', N'Inglese', 63)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'PREFERENCE', N'145       ', N'it        ', N'Statistiche territoriali', 64)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'PREFERENCE', N'146       ', N'it        ', N'Censimenti della popolazione, delle abitazioni e delle attività produttive', 65)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'PREFERENCE', N'147       ', N'it        ', N'Struttura e competitività del sistema delle imprese', 66)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'PREFERENCE', N'148       ', N'it        ', N'Indicatori congiunturali', 67)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'PREFERENCE', N'149       ', N'it        ', N'Sviluppo sostenibile', 68)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'PREFERENCE', N'150       ', N'it        ', N'Ambiente ed energia', 69)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'PREFERENCE', N'151       ', N'it        ', N'Popolazione e famiglie', 70)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'PREFERENCE', N'152       ', N'it        ', N'Condizioni economiche delle famiglie e disuguaglianze', 71)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'PREFERENCE', N'153       ', N'it        ', N'Salute e sanità', 72)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'PREFERENCE', N'154       ', N'it        ', N'Assistenza e previdenza', 73)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'PREFERENCE', N'155       ', N'it        ', N'Istruzione e formazione', 74)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'PREFERENCE', N'156       ', N'it        ', N'Cultura, comunicazione, tempo libero', 75)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'PREFERENCE', N'157       ', N'it        ', N'Giustizia e sicurezza', 76)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'PREFERENCE', N'158       ', N'it        ', N'Opinioni dei cittadini e soddisfazione per la vita', 77)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'PREFERENCE', N'159       ', N'it        ', N'Partecipazione sociale', 78)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'PREFERENCE', N'160       ', N'it        ', N'Conti nazionali', 79)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'PREFERENCE', N'161       ', N'it        ', N'Agricoltura', 80)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'PREFERENCE', N'162       ', N'it        ', N'Industria e costruzioni', 81)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'PREFERENCE', N'163       ', N'it        ', N'Servizi', 82)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'PREFERENCE', N'164       ', N'it        ', N'Pubbliche amministrazioni e istituzioni private', 83)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'PREFERENCE', N'165       ', N'it        ', N'Innovazione, ricerca e società dell''informazione', 84)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'PREFERENCE', N'166       ', N'it        ', N'Commercio con l''estero e internazionalizzazione', 85)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'PREFERENCE', N'167       ', N'it        ', N'Prezzi', 86)
INSERT [dbo].[Localisation] ([TableName], [Code], [Lang], [Description], [LocalisationID]) VALUES (N'PREFERENCE', N'168       ', N'it        ', N'Lavoro', 87)
SET IDENTITY_INSERT [dbo].[Localisation] OFF
INSERT [dbo].[Position] ([PositionCode]) VALUES (N'10        ')
INSERT [dbo].[Position] ([PositionCode]) VALUES (N'90        ')
INSERT [dbo].[Position] ([PositionCode]) VALUES (N'91        ')
INSERT [dbo].[Position] ([PositionCode]) VALUES (N'92        ')
INSERT [dbo].[Position] ([PositionCode]) VALUES (N'93        ')
INSERT [dbo].[Position] ([PositionCode]) VALUES (N'94        ')
INSERT [dbo].[Position] ([PositionCode]) VALUES (N'95        ')
INSERT [dbo].[Position] ([PositionCode]) VALUES (N'96        ')
INSERT [dbo].[Position] ([PositionCode]) VALUES (N'97        ')
INSERT [dbo].[Position] ([PositionCode]) VALUES (N'98        ')
INSERT [dbo].[Position] ([PositionCode]) VALUES (N'99        ')
INSERT [dbo].[Preference] ([PreferenceCode]) VALUES (N'145       ')
INSERT [dbo].[Preference] ([PreferenceCode]) VALUES (N'146       ')
INSERT [dbo].[Preference] ([PreferenceCode]) VALUES (N'147       ')
INSERT [dbo].[Preference] ([PreferenceCode]) VALUES (N'148       ')
INSERT [dbo].[Preference] ([PreferenceCode]) VALUES (N'149       ')
INSERT [dbo].[Preference] ([PreferenceCode]) VALUES (N'150       ')
INSERT [dbo].[Preference] ([PreferenceCode]) VALUES (N'151       ')
INSERT [dbo].[Preference] ([PreferenceCode]) VALUES (N'152       ')
INSERT [dbo].[Preference] ([PreferenceCode]) VALUES (N'153       ')
INSERT [dbo].[Preference] ([PreferenceCode]) VALUES (N'154       ')
INSERT [dbo].[Preference] ([PreferenceCode]) VALUES (N'155       ')
INSERT [dbo].[Preference] ([PreferenceCode]) VALUES (N'156       ')
INSERT [dbo].[Preference] ([PreferenceCode]) VALUES (N'157       ')
INSERT [dbo].[Preference] ([PreferenceCode]) VALUES (N'158       ')
INSERT [dbo].[Preference] ([PreferenceCode]) VALUES (N'159       ')
INSERT [dbo].[Preference] ([PreferenceCode]) VALUES (N'160       ')
INSERT [dbo].[Preference] ([PreferenceCode]) VALUES (N'161       ')
INSERT [dbo].[Preference] ([PreferenceCode]) VALUES (N'162       ')
INSERT [dbo].[Preference] ([PreferenceCode]) VALUES (N'163       ')
INSERT [dbo].[Preference] ([PreferenceCode]) VALUES (N'164       ')
INSERT [dbo].[Preference] ([PreferenceCode]) VALUES (N'165       ')
INSERT [dbo].[Preference] ([PreferenceCode]) VALUES (N'166       ')
INSERT [dbo].[Preference] ([PreferenceCode]) VALUES (N'167       ')
INSERT [dbo].[Preference] ([PreferenceCode]) VALUES (N'168       ')
INSERT [dbo].[Role] ([RoleCode], [RoleDescription]) VALUES (N'SA        ', N'SuperAdministrator')
INSERT [dbo].[Role] ([RoleCode], [RoleDescription]) VALUES (N'User      ', N'All other users')
INSERT [dbo].[Sex] ([SexCode]) VALUES (N'f         ')
INSERT [dbo].[Sex] ([SexCode]) VALUES (N'm         ')
INSERT [dbo].[Study] ([StudyCode]) VALUES (N'87        ')
INSERT [dbo].[Study] ([StudyCode]) VALUES (N'88        ')
INSERT [dbo].[Study] ([StudyCode]) VALUES (N'89        ')
INSERT [dbo].[User] ([UserCode], [Name], [Surname], [Password], [Email], [Age], [Country], [Position], [Sex], [Study], [Agency], [Lang], [Role]) VALUES (N'48062ddf-6de4-4996-8265-59bee81dce3c', N'Admin', N'Admin', N'rkvBIXUKV0yJipyYgpnjgQ==', N'admin@admin.it', N'86        ', N'144       ', N'94        ', N'm         ', N'87        ', N'116       ', N'it        ', N'SA        ')
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_User_Age]') AND parent_object_id = OBJECT_ID(N'[dbo].[User]'))
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_User_Age] FOREIGN KEY([Age])
REFERENCES [dbo].[Age] ([AgeCode])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_User_Age]') AND parent_object_id = OBJECT_ID(N'[dbo].[User]'))
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_Age]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_User_Agency]') AND parent_object_id = OBJECT_ID(N'[dbo].[User]'))
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_User_Agency] FOREIGN KEY([Agency])
REFERENCES [dbo].[Agency] ([AgencyCode])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_User_Agency]') AND parent_object_id = OBJECT_ID(N'[dbo].[User]'))
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_Agency]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_User_Country]') AND parent_object_id = OBJECT_ID(N'[dbo].[User]'))
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_User_Country] FOREIGN KEY([Country])
REFERENCES [dbo].[Country] ([CountryCode])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_User_Country]') AND parent_object_id = OBJECT_ID(N'[dbo].[User]'))
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_Country]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_User_Language]') AND parent_object_id = OBJECT_ID(N'[dbo].[User]'))
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_User_Language] FOREIGN KEY([Lang])
REFERENCES [dbo].[Language] ([LanguageCode])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_User_Language]') AND parent_object_id = OBJECT_ID(N'[dbo].[User]'))
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_Language]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_User_Position]') AND parent_object_id = OBJECT_ID(N'[dbo].[User]'))
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_User_Position] FOREIGN KEY([Position])
REFERENCES [dbo].[Position] ([PositionCode])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_User_Position]') AND parent_object_id = OBJECT_ID(N'[dbo].[User]'))
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_Position]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_User_Role]') AND parent_object_id = OBJECT_ID(N'[dbo].[User]'))
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_User_Role] FOREIGN KEY([Role])
REFERENCES [dbo].[Role] ([RoleCode])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_User_Role]') AND parent_object_id = OBJECT_ID(N'[dbo].[User]'))
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_Role]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_User_Sex]') AND parent_object_id = OBJECT_ID(N'[dbo].[User]'))
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_User_Sex] FOREIGN KEY([Sex])
REFERENCES [dbo].[Sex] ([SexCode])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_User_Sex]') AND parent_object_id = OBJECT_ID(N'[dbo].[User]'))
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_Sex]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_User_Study]') AND parent_object_id = OBJECT_ID(N'[dbo].[User]'))
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_User_Study] FOREIGN KEY([Study])
REFERENCES [dbo].[Study] ([StudyCode])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_User_Study]') AND parent_object_id = OBJECT_ID(N'[dbo].[User]'))
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_Study]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_UserPreference_Preference]') AND parent_object_id = OBJECT_ID(N'[dbo].[UserPreference]'))
ALTER TABLE [dbo].[UserPreference]  WITH CHECK ADD  CONSTRAINT [FK_UserPreference_Preference] FOREIGN KEY([PreferenceCode])
REFERENCES [dbo].[Preference] ([PreferenceCode])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_UserPreference_Preference]') AND parent_object_id = OBJECT_ID(N'[dbo].[UserPreference]'))
ALTER TABLE [dbo].[UserPreference] CHECK CONSTRAINT [FK_UserPreference_Preference]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_UserPreference_User]') AND parent_object_id = OBJECT_ID(N'[dbo].[UserPreference]'))
ALTER TABLE [dbo].[UserPreference]  WITH CHECK ADD  CONSTRAINT [FK_UserPreference_User] FOREIGN KEY([UserCode])
REFERENCES [dbo].[User] ([UserCode])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_UserPreference_User]') AND parent_object_id = OBJECT_ID(N'[dbo].[UserPreference]'))
ALTER TABLE [dbo].[UserPreference] CHECK CONSTRAINT [FK_UserPreference_User]
GO
