CREATE TABLE [dbo].[UserProfile](
	[UserId] [int] IDENTITY(43,1) NOT NULL,
	[UserName] [nvarchar](56) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Email] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[IsActive] [bit] NOT NULL CONSTRAINT [DF_UserProfile_IsActive]  DEFAULT 'False'
)

GO
ALTER TABLE [dbo].[UserProfile] ADD  CONSTRAINT [PK__UserProfile__0000000000000006] PRIMARY KEY 
(
	[UserId]
)
GO
CREATE TABLE [dbo].[webpages_Membership](
	[UserId] [int] NOT NULL,
	[CreateDate] [datetime] NULL,
	[ConfirmationToken] [nvarchar](128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[IsConfirmed] [bit] NULL CONSTRAINT [DF_webpages_Membership_IsConfirmed]  DEFAULT 0,
	[LastPasswordFailureDate] [datetime] NULL,
	[PasswordFailuresSinceLastSuccess] [int] NOT NULL CONSTRAINT [DF_webpages_Membership_PasswordFailuresSinceLastSuccess]  DEFAULT 0,
	[Password] [nvarchar](128) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[PasswordChangedDate] [datetime] NULL,
	[PasswordSalt] [nvarchar](128) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[PasswordVerificationToken] [nvarchar](128) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[PasswordVerificationTokenExpirationDate] [datetime] NULL
)

GO
ALTER TABLE [dbo].[webpages_Membership] ADD  CONSTRAINT [PK__webpages_Membership__0000000000000039] PRIMARY KEY 
(
	[UserId]
)
GO
CREATE TABLE [dbo].[webpages_OAuthMembership](
	[Provider] [nvarchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[ProviderUserId] [nvarchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[UserId] [int] NOT NULL
)

GO
ALTER TABLE [dbo].[webpages_OAuthMembership] ADD  CONSTRAINT [PK__webpages_OAuthMembership__0000000000000017] PRIMARY KEY 
(
	[Provider],
	[ProviderUserId]
)
GO
CREATE TABLE [dbo].[webpages_Roles](
	[RoleId] [int] IDENTITY(33,1) NOT NULL,
	[RoleName] [nvarchar](256) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
)

GO
ALTER TABLE [dbo].[webpages_Roles] ADD  CONSTRAINT [PK__webpages_Roles__0000000000000043] PRIMARY KEY 
(
	[RoleId]
)
GO
CREATE TABLE [dbo].[webpages_UsersInRoles](
	[UserId] [int] NOT NULL,
	[RoleId] [int] NOT NULL
)

GO
ALTER TABLE [dbo].[webpages_UsersInRoles] ADD  CONSTRAINT [PK__webpages_UsersInRoles__0000000000000052] PRIMARY KEY 
(
	[UserId],
	[RoleId]
)
GO
ALTER TABLE [dbo].[UserProfile] ADD  CONSTRAINT [UQ__UserProfile__000000000000000B] UNIQUE 
(
	[UserName]
)
GO
ALTER TABLE [dbo].[webpages_Roles] ADD  CONSTRAINT [UQ__webpages_Roles__0000000000000048] UNIQUE 
(
	[RoleName]
)
GO
ALTER TABLE [dbo].[webpages_UsersInRoles]  ADD  CONSTRAINT [fk_RoleId] FOREIGN KEY([RoleId])
REFERENCES [webpages_Roles] ([RoleId])
GO
ALTER TABLE [dbo].[webpages_UsersInRoles]  ADD  CONSTRAINT [fk_UserId] FOREIGN KEY([UserId])
REFERENCES [UserProfile] ([UserId])
GO
