IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);

CREATE TABLE [AspNetUsers] (
    [Id] nvarchar(450) NOT NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);

CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(128) NOT NULL,
    [ProviderKey] nvarchar(128) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(128) NOT NULL,
    [Name] nvarchar(128) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);

CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);

CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);

CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'00000000000000_CreateIdentitySchema', N'10.0.0');

COMMIT;
GO

BEGIN TRANSACTION;
ALTER TABLE [AspNetUsers] ADD [FullName] nvarchar(max) NULL;

ALTER TABLE [AspNetUsers] ADD [JobTitle] nvarchar(max) NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251121113915_UpdatedApplicationUser2', N'10.0.0');

COMMIT;
GO

BEGIN TRANSACTION;
CREATE TABLE [Competences] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Competences] PRIMARY KEY ([Id])
);

CREATE TABLE [UserCompetences] (
    [UserId] nvarchar(450) NOT NULL,
    [CompetenceId] int NOT NULL,
    [Level] int NOT NULL,
    [YearsOfExperience] int NULL,
    CONSTRAINT [PK_UserCompetences] PRIMARY KEY ([UserId], [CompetenceId]),
    CONSTRAINT [FK_UserCompetences_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserCompetences_Competences_CompetenceId] FOREIGN KEY ([CompetenceId]) REFERENCES [Competences] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_UserCompetences_CompetenceId] ON [UserCompetences] ([CompetenceId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251121153223_AddCompetenceTables', N'10.0.0');

COMMIT;
GO

BEGIN TRANSACTION;
DECLARE @var nvarchar(max);
SELECT @var = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Competences]') AND [c].[name] = N'Name');
IF @var IS NOT NULL EXEC(N'ALTER TABLE [Competences] DROP CONSTRAINT ' + @var + ';');
ALTER TABLE [Competences] ALTER COLUMN [Name] nvarchar(100) NOT NULL;

DECLARE @var1 nvarchar(max);
SELECT @var1 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Competences]') AND [c].[name] = N'Description');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Competences] DROP CONSTRAINT ' + @var1 + ';');
ALTER TABLE [Competences] ALTER COLUMN [Description] nvarchar(500) NOT NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251125103144_initial', N'10.0.0');

COMMIT;
GO

BEGIN TRANSACTION;
CREATE TABLE [MatchSettings] (
    [Id] int NOT NULL IDENTITY,
    [CompetenceWeight] int NOT NULL,
    [ExperienceWeight] int NOT NULL,
    [AvailabilityWeight] int NOT NULL,
    CONSTRAINT [PK_MatchSettings] PRIMARY KEY ([Id])
);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251128142446_AddMatchSettings', N'10.0.0');

COMMIT;
GO

BEGIN TRANSACTION;
ALTER TABLE [AspNetUsers] ADD [AvailabilityPercent] int NOT NULL DEFAULT 0;

CREATE TABLE [Projects] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(200) NOT NULL,
    [Description] nvarchar(1000) NULL,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NULL,
    CONSTRAINT [PK_Projects] PRIMARY KEY ([Id])
);

CREATE TABLE [ProjectRequirements] (
    [Id] int NOT NULL IDENTITY,
    [ProjectId] int NOT NULL,
    [CompetenceId] int NOT NULL,
    [MinLevel] int NOT NULL,
    [MinYearsOfExperience] int NOT NULL,
    CONSTRAINT [PK_ProjectRequirements] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ProjectRequirements_Competences_CompetenceId] FOREIGN KEY ([CompetenceId]) REFERENCES [Competences] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ProjectRequirements_Projects_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [Projects] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_ProjectRequirements_CompetenceId] ON [ProjectRequirements] ([CompetenceId]);

CREATE INDEX [IX_ProjectRequirements_ProjectId] ON [ProjectRequirements] ([ProjectId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251128160745_SyncModelAfterManualEdit', N'10.0.0');

COMMIT;
GO

BEGIN TRANSACTION;
DECLARE @var2 nvarchar(max);
SELECT @var2 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Competences]') AND [c].[name] = N'Name');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Competences] DROP CONSTRAINT ' + @var2 + ';');
ALTER TABLE [Competences] ALTER COLUMN [Name] nvarchar(100) NOT NULL;

DECLARE @var3 nvarchar(max);
SELECT @var3 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Competences]') AND [c].[name] = N'Description');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Competences] DROP CONSTRAINT ' + @var3 + ';');
ALTER TABLE [Competences] ALTER COLUMN [Description] nvarchar(500) NOT NULL;

CREATE TABLE [TeamLeaders] (
    [Id] int NOT NULL IDENTITY,
    [TeamName] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [LeaderUserId] nvarchar(450) NOT NULL,
    [RequiredCapacity] int NOT NULL,
    CONSTRAINT [PK_TeamLeaders] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TeamLeaders_AspNetUsers_LeaderUserId] FOREIGN KEY ([LeaderUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [TeamGrowthPlans] (
    [Id] int NOT NULL IDENTITY,
    [TeamLeaderId] int NOT NULL,
    [Action] nvarchar(max) NOT NULL,
    [Goal] nvarchar(max) NOT NULL,
    [Deadline] datetime2 NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_TeamGrowthPlans] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TeamGrowthPlans_TeamLeaders_TeamLeaderId] FOREIGN KEY ([TeamLeaderId]) REFERENCES [TeamLeaders] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [TeamMembers] (
    [Id] int NOT NULL IDENTITY,
    [TeamLeaderId] int NOT NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_TeamMembers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TeamMembers_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_TeamMembers_TeamLeaders_TeamLeaderId] FOREIGN KEY ([TeamLeaderId]) REFERENCES [TeamLeaders] ([Id])
);

CREATE TABLE [TeamSkillNeeds] (
    [Id] int NOT NULL IDENTITY,
    [TeamLeaderId] int NOT NULL,
    [CompetenceId] int NOT NULL,
    [LevelNeeded] int NOT NULL,
    [Importance] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_TeamSkillNeeds] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TeamSkillNeeds_Competences_CompetenceId] FOREIGN KEY ([CompetenceId]) REFERENCES [Competences] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_TeamSkillNeeds_TeamLeaders_TeamLeaderId] FOREIGN KEY ([TeamLeaderId]) REFERENCES [TeamLeaders] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_TeamGrowthPlans_TeamLeaderId] ON [TeamGrowthPlans] ([TeamLeaderId]);

CREATE INDEX [IX_TeamLeaders_LeaderUserId] ON [TeamLeaders] ([LeaderUserId]);

CREATE INDEX [IX_TeamMembers_TeamLeaderId] ON [TeamMembers] ([TeamLeaderId]);

CREATE INDEX [IX_TeamMembers_UserId] ON [TeamMembers] ([UserId]);

CREATE INDEX [IX_TeamSkillNeeds_CompetenceId] ON [TeamSkillNeeds] ([CompetenceId]);

CREATE INDEX [IX_TeamSkillNeeds_TeamLeaderId] ON [TeamSkillNeeds] ([TeamLeaderId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251128213104_TeamLeaderSystem', N'10.0.0');

COMMIT;
GO

BEGIN TRANSACTION;
ALTER TABLE [TeamLeaders] ADD [RequiredDays] int NOT NULL DEFAULT 0;

ALTER TABLE [TeamLeaders] ADD [RequiredHours] int NOT NULL DEFAULT 0;

ALTER TABLE [TeamLeaders] ADD [RequiredMonths] int NOT NULL DEFAULT 0;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251204105537_AddTeamCapacityFields', N'10.0.0');

COMMIT;
GO

BEGIN TRANSACTION;
DECLARE @var4 nvarchar(max);
SELECT @var4 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[TeamLeaders]') AND [c].[name] = N'RequiredMonths');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [TeamLeaders] DROP CONSTRAINT ' + @var4 + ';');
ALTER TABLE [TeamLeaders] ALTER COLUMN [RequiredMonths] int NULL;

DECLARE @var5 nvarchar(max);
SELECT @var5 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[TeamLeaders]') AND [c].[name] = N'RequiredHours');
IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [TeamLeaders] DROP CONSTRAINT ' + @var5 + ';');
ALTER TABLE [TeamLeaders] ALTER COLUMN [RequiredHours] int NULL;

DECLARE @var6 nvarchar(max);
SELECT @var6 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[TeamLeaders]') AND [c].[name] = N'RequiredDays');
IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [TeamLeaders] DROP CONSTRAINT ' + @var6 + ';');
ALTER TABLE [TeamLeaders] ALTER COLUMN [RequiredDays] int NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251204110949_MakeCapacityNullable', N'10.0.0');

COMMIT;
GO

BEGIN TRANSACTION;
ALTER TABLE [AspNetUsers] ADD [Certificates] nvarchar(max) NULL;

ALTER TABLE [AspNetUsers] ADD [Interests] nvarchar(max) NULL;

ALTER TABLE [AspNetUsers] ADD [Languages] nvarchar(max) NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251208112354_AddProfileFieldsToUser', N'10.0.0');

COMMIT;
GO

BEGIN TRANSACTION;
ALTER TABLE [TeamSkillNeeds] DROP CONSTRAINT [FK_TeamSkillNeeds__Competences_CompetenceId];

ALTER TABLE [UserCompetences] DROP CONSTRAINT [FK_UserCompetences_TeamGrowthPlans_CompetenceId];

DROP TABLE [TeamGrowthPlan];

EXEC sp_rename N'[TeamGrowthPlans].[Description]', N'Action', 'COLUMN';

DECLARE @var7 nvarchar(max);
SELECT @var7 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[TeamSkillNeeds]') AND [c].[name] = N'Importance');
IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [TeamSkillNeeds] DROP CONSTRAINT ' + @var7 + ';');
ALTER TABLE [TeamSkillNeeds] ALTER COLUMN [Importance] nvarchar(max) NOT NULL;

CREATE TABLE [Competences] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [Description] nvarchar(500) NOT NULL,
    CONSTRAINT [PK_Competences] PRIMARY KEY ([Id])
);

ALTER TABLE [TeamGrowthPlans] ADD CONSTRAINT [FK_TeamGrowthPlans_TeamLeaders_TeamLeaderId] FOREIGN KEY ([TeamLeaderId]) REFERENCES [TeamLeaders] ([Id]) ON DELETE CASCADE;

ALTER TABLE [TeamSkillNeeds] ADD CONSTRAINT [FK_TeamSkillNeeds_Competences_CompetenceId] FOREIGN KEY ([CompetenceId]) REFERENCES [Competences] ([Id]) ON DELETE CASCADE;

ALTER TABLE [UserCompetences] ADD CONSTRAINT [FK_UserCompetences_Competences_CompetenceId] FOREIGN KEY ([CompetenceId]) REFERENCES [Competences] ([Id]) ON DELETE CASCADE;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251212105208_Teamleader', N'10.0.0');

COMMIT;
GO

BEGIN TRANSACTION;
CREATE TABLE [ProjectManager] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(200) NOT NULL,
    [Description] nvarchar(1000) NULL,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NULL,
    [RequiredExperienceLevel] nvarchar(max) NOT NULL,
    [PreferredWorkingStyle] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_ProjectManager] PRIMARY KEY ([Id])
);

CREATE TABLE [ProjectRequirements] (
    [Id] int NOT NULL IDENTITY,
    [ProjectId] int NOT NULL,
    [CompetenceId] int NOT NULL,
    [MinLevel] int NOT NULL,
    [MinYearsOfExperience] int NOT NULL,
    CONSTRAINT [PK_ProjectRequirements] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ProjectRequirements_Competences_CompetenceId] FOREIGN KEY ([CompetenceId]) REFERENCES [Competences] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ProjectRequirements_ProjectManager_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [ProjectManager] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [ProjectRoles] (
    [Id] int NOT NULL IDENTITY,
    [ProjectId] int NOT NULL,
    [RoleName] nvarchar(max) NOT NULL,
    [RequiredCount] int NOT NULL,
    CONSTRAINT [PK_ProjectRoles] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ProjectRoles_ProjectManager_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [ProjectManager] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [ProjectTeamMembers] (
    [Id] int NOT NULL IDENTITY,
    [ProjectId] int NOT NULL,
    [UserId] nvarchar(450) NOT NULL,
    [AssignedRole] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_ProjectTeamMembers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ProjectTeamMembers_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]),
    CONSTRAINT [FK_ProjectTeamMembers_ProjectManager_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [ProjectManager] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_ProjectRequirements_CompetenceId] ON [ProjectRequirements] ([CompetenceId]);

CREATE INDEX [IX_ProjectRequirements_ProjectId] ON [ProjectRequirements] ([ProjectId]);

CREATE INDEX [IX_ProjectRoles_ProjectId] ON [ProjectRoles] ([ProjectId]);

CREATE INDEX [IX_ProjectTeamMembers_ProjectId] ON [ProjectTeamMembers] ([ProjectId]);

CREATE INDEX [IX_ProjectTeamMembers_UserId] ON [ProjectTeamMembers] ([UserId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251218101021_AddProjectTables', N'10.0.0');

COMMIT;
GO

BEGIN TRANSACTION;
DECLARE @var8 nvarchar(max);
SELECT @var8 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ProjectManager]') AND [c].[name] = N'PreferredWorkingStyle');
IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [ProjectManager] DROP CONSTRAINT ' + @var8 + ';');
ALTER TABLE [ProjectManager] DROP COLUMN [PreferredWorkingStyle];

DECLARE @var9 nvarchar(max);
SELECT @var9 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ProjectManager]') AND [c].[name] = N'RequiredExperienceLevel');
IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [ProjectManager] DROP CONSTRAINT ' + @var9 + ';');
ALTER TABLE [ProjectManager] DROP COLUMN [RequiredExperienceLevel];

ALTER TABLE [ProjectManager] ADD [ExperienceLevel] nvarchar(max) NULL;

ALTER TABLE [ProjectManager] ADD [RolesNeeded] nvarchar(max) NULL;

ALTER TABLE [ProjectManager] ADD [Skills] nvarchar(max) NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251218111137_AddProjectSync', N'10.0.0');

COMMIT;
GO

BEGIN TRANSACTION;
DECLARE @var10 nvarchar(max);
SELECT @var10 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ProjectManager]') AND [c].[name] = N'Skills');
IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [ProjectManager] DROP CONSTRAINT ' + @var10 + ';');
UPDATE [ProjectManager] SET [Skills] = N'' WHERE [Skills] IS NULL;
ALTER TABLE [ProjectManager] ALTER COLUMN [Skills] nvarchar(1000) NOT NULL;
ALTER TABLE [ProjectManager] ADD DEFAULT N'' FOR [Skills];

DECLARE @var11 nvarchar(max);
SELECT @var11 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ProjectManager]') AND [c].[name] = N'RolesNeeded');
IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [ProjectManager] DROP CONSTRAINT ' + @var11 + ';');
UPDATE [ProjectManager] SET [RolesNeeded] = N'' WHERE [RolesNeeded] IS NULL;
ALTER TABLE [ProjectManager] ALTER COLUMN [RolesNeeded] nvarchar(max) NOT NULL;
ALTER TABLE [ProjectManager] ADD DEFAULT N'' FOR [RolesNeeded];

DECLARE @var12 nvarchar(max);
SELECT @var12 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ProjectManager]') AND [c].[name] = N'ExperienceLevel');
IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [ProjectManager] DROP CONSTRAINT ' + @var12 + ';');
UPDATE [ProjectManager] SET [ExperienceLevel] = N'' WHERE [ExperienceLevel] IS NULL;
ALTER TABLE [ProjectManager] ALTER COLUMN [ExperienceLevel] nvarchar(max) NOT NULL;
ALTER TABLE [ProjectManager] ADD DEFAULT N'' FOR [ExperienceLevel];

DECLARE @var13 nvarchar(max);
SELECT @var13 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ProjectManager]') AND [c].[name] = N'EndDate');
IF @var13 IS NOT NULL EXEC(N'ALTER TABLE [ProjectManager] DROP CONSTRAINT ' + @var13 + ';');
UPDATE [ProjectManager] SET [EndDate] = '0001-01-01T00:00:00.0000000' WHERE [EndDate] IS NULL;
ALTER TABLE [ProjectManager] ALTER COLUMN [EndDate] datetime2 NOT NULL;
ALTER TABLE [ProjectManager] ADD DEFAULT '0001-01-01T00:00:00.0000000' FOR [EndDate];

DECLARE @var14 nvarchar(max);
SELECT @var14 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ProjectManager]') AND [c].[name] = N'Description');
IF @var14 IS NOT NULL EXEC(N'ALTER TABLE [ProjectManager] DROP CONSTRAINT ' + @var14 + ';');
UPDATE [ProjectManager] SET [Description] = N'' WHERE [Description] IS NULL;
ALTER TABLE [ProjectManager] ALTER COLUMN [Description] nvarchar(max) NOT NULL;
ALTER TABLE [ProjectManager] ADD DEFAULT N'' FOR [Description];

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260112160433_AddProjectAssign', N'10.0.0');

COMMIT;
GO

