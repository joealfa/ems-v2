-- ============================================================================
-- EMS-v2 Database Creation Script
-- Creates the database and all tables matching the EF Core migration snapshot
-- Run on SQL Server (SSMS or sqlcmd)
-- ============================================================================

-- ============================================================================
-- 1. CREATE DATABASE
-- ============================================================================
-- Change the database name below if needed
DECLARE @DbName NVARCHAR(128) = N'EmployeeManagementDb';

IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = @DbName)
BEGIN
    EXEC('CREATE DATABASE [' + @DbName + ']');
    PRINT 'Database created: ' + @DbName;
END
ELSE
BEGIN
    PRINT 'Database already exists: ' + @DbName;
END
GO

-- Switch to the database (update name here if changed above)
USE [EmployeeManagementDb];
GO

-- ============================================================================
-- 2. CREATE TABLES
-- ============================================================================

-- --------------------------------------------------------------------------
-- Persons
-- --------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Persons')
BEGIN
    CREATE TABLE [Persons] (
        [Id]              UNIQUEIDENTIFIER NOT NULL,
        [DisplayId]       BIGINT           NOT NULL,
        [FirstName]       NVARCHAR(100)    NOT NULL,
        [LastName]        NVARCHAR(100)    NOT NULL,
        [MiddleName]      NVARCHAR(100)    NULL,
        [DateOfBirth]     DATE             NOT NULL,
        [Gender]          INT              NOT NULL,
        [CivilStatus]     INT              NOT NULL,
        [ProfileImageUrl] NVARCHAR(2048)   NULL,
        [HasProfileImage] BIT              NOT NULL DEFAULT 0,
        [CreatedBy]       NVARCHAR(256)    NOT NULL,
        [CreatedOn]       DATETIME2        NOT NULL,
        [ModifiedBy]      NVARCHAR(256)    NULL,
        [ModifiedOn]      DATETIME2        NULL,
        [IsDeleted]       BIT              NOT NULL DEFAULT 0,
        CONSTRAINT [PK_Persons] PRIMARY KEY ([Id])
    );

    CREATE UNIQUE INDEX [IX_Persons_DisplayId] ON [Persons] ([DisplayId]);
    PRINT 'Created table: Persons';
END
GO

-- --------------------------------------------------------------------------
-- Schools
-- --------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Schools')
BEGIN
    CREATE TABLE [Schools] (
        [Id]         UNIQUEIDENTIFIER NOT NULL,
        [DisplayId]  BIGINT           NOT NULL,
        [SchoolName] NVARCHAR(200)    NOT NULL,
        [IsActive]   BIT              NOT NULL DEFAULT 1,
        [CreatedBy]  NVARCHAR(256)    NOT NULL,
        [CreatedOn]  DATETIME2        NOT NULL,
        [ModifiedBy] NVARCHAR(256)    NULL,
        [ModifiedOn] DATETIME2        NULL,
        [IsDeleted]  BIT              NOT NULL DEFAULT 0,
        CONSTRAINT [PK_Schools] PRIMARY KEY ([Id])
    );

    CREATE UNIQUE INDEX [IX_Schools_DisplayId] ON [Schools] ([DisplayId]);
    PRINT 'Created table: Schools';
END
GO

-- --------------------------------------------------------------------------
-- Addresses
-- --------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Addresses')
BEGIN
    CREATE TABLE [Addresses] (
        [Id]          UNIQUEIDENTIFIER NOT NULL,
        [DisplayId]   BIGINT           NOT NULL,
        [Address1]    NVARCHAR(200)    NOT NULL,
        [Address2]    NVARCHAR(200)    NULL,
        [Barangay]    NVARCHAR(100)    NULL,
        [City]        NVARCHAR(100)    NOT NULL,
        [Province]    NVARCHAR(100)    NOT NULL,
        [Country]     NVARCHAR(100)    NOT NULL,
        [ZipCode]     NVARCHAR(20)     NULL,
        [IsCurrent]   BIT              NOT NULL DEFAULT 0,
        [IsPermanent] BIT              NOT NULL DEFAULT 0,
        [IsActive]    BIT              NOT NULL DEFAULT 1,
        [AddressType] INT              NOT NULL,
        [PersonId]    UNIQUEIDENTIFIER NULL,
        [SchoolId]    UNIQUEIDENTIFIER NULL,
        [CreatedBy]   NVARCHAR(256)    NOT NULL,
        [CreatedOn]   DATETIME2        NOT NULL,
        [ModifiedBy]  NVARCHAR(256)    NULL,
        [ModifiedOn]  DATETIME2        NULL,
        [IsDeleted]   BIT              NOT NULL DEFAULT 0,
        CONSTRAINT [PK_Addresses] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Addresses_Persons_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [Persons] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Addresses_Schools_SchoolId] FOREIGN KEY ([SchoolId]) REFERENCES [Schools] ([Id]) ON DELETE NO ACTION
    );

    CREATE UNIQUE INDEX [IX_Addresses_DisplayId] ON [Addresses] ([DisplayId]);
    CREATE INDEX [IX_Addresses_PersonId] ON [Addresses] ([PersonId]);
    CREATE INDEX [IX_Addresses_SchoolId] ON [Addresses] ([SchoolId]);
    PRINT 'Created table: Addresses';
END
GO

-- --------------------------------------------------------------------------
-- Contacts
-- --------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Contacts')
BEGIN
    CREATE TABLE [Contacts] (
        [Id]          UNIQUEIDENTIFIER NOT NULL,
        [DisplayId]   BIGINT           NOT NULL,
        [Mobile]      NVARCHAR(20)     NULL,
        [LandLine]    NVARCHAR(20)     NULL,
        [Fax]         NVARCHAR(20)     NULL,
        [Email]       NVARCHAR(256)    NULL,
        [IsActive]    BIT              NOT NULL DEFAULT 1,
        [ContactType] INT              NOT NULL,
        [PersonId]    UNIQUEIDENTIFIER NULL,
        [SchoolId]    UNIQUEIDENTIFIER NULL,
        [CreatedBy]   NVARCHAR(256)    NOT NULL,
        [CreatedOn]   DATETIME2        NOT NULL,
        [ModifiedBy]  NVARCHAR(256)    NULL,
        [ModifiedOn]  DATETIME2        NULL,
        [IsDeleted]   BIT              NOT NULL DEFAULT 0,
        CONSTRAINT [PK_Contacts] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Contacts_Persons_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [Persons] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Contacts_Schools_SchoolId] FOREIGN KEY ([SchoolId]) REFERENCES [Schools] ([Id]) ON DELETE NO ACTION
    );

    CREATE UNIQUE INDEX [IX_Contacts_DisplayId] ON [Contacts] ([DisplayId]);
    CREATE INDEX [IX_Contacts_PersonId] ON [Contacts] ([PersonId]);
    CREATE INDEX [IX_Contacts_SchoolId] ON [Contacts] ([SchoolId]);
    PRINT 'Created table: Contacts';
END
GO

-- --------------------------------------------------------------------------
-- Positions
-- --------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Positions')
BEGIN
    CREATE TABLE [Positions] (
        [Id]          UNIQUEIDENTIFIER NOT NULL,
        [DisplayId]   BIGINT           NOT NULL,
        [TitleName]   NVARCHAR(100)    NOT NULL,
        [Description] NVARCHAR(500)    NULL,
        [IsActive]    BIT              NOT NULL DEFAULT 1,
        [CreatedBy]   NVARCHAR(256)    NOT NULL,
        [CreatedOn]   DATETIME2        NOT NULL,
        [ModifiedBy]  NVARCHAR(256)    NULL,
        [ModifiedOn]  DATETIME2        NULL,
        [IsDeleted]   BIT              NOT NULL DEFAULT 0,
        CONSTRAINT [PK_Positions] PRIMARY KEY ([Id])
    );

    CREATE UNIQUE INDEX [IX_Positions_DisplayId] ON [Positions] ([DisplayId]);
    PRINT 'Created table: Positions';
END
GO

-- --------------------------------------------------------------------------
-- SalaryGrades
-- --------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'SalaryGrades')
BEGIN
    CREATE TABLE [SalaryGrades] (
        [Id]              UNIQUEIDENTIFIER NOT NULL,
        [DisplayId]       BIGINT           NOT NULL,
        [SalaryGradeName] NVARCHAR(50)     NOT NULL,
        [Description]     NVARCHAR(500)    NULL,
        [Step]            INT              NOT NULL DEFAULT 1,
        [MonthlySalary]   DECIMAL(18,2)    NOT NULL DEFAULT 0,
        [IsActive]        BIT              NOT NULL DEFAULT 1,
        [CreatedBy]       NVARCHAR(256)    NOT NULL,
        [CreatedOn]       DATETIME2        NOT NULL,
        [ModifiedBy]      NVARCHAR(256)    NULL,
        [ModifiedOn]      DATETIME2        NULL,
        [IsDeleted]       BIT              NOT NULL DEFAULT 0,
        CONSTRAINT [PK_SalaryGrades] PRIMARY KEY ([Id])
    );

    CREATE UNIQUE INDEX [IX_SalaryGrades_DisplayId] ON [SalaryGrades] ([DisplayId]);
    PRINT 'Created table: SalaryGrades';
END
GO

-- --------------------------------------------------------------------------
-- Items
-- --------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Items')
BEGIN
    CREATE TABLE [Items] (
        [Id]          UNIQUEIDENTIFIER NOT NULL,
        [DisplayId]   BIGINT           NOT NULL,
        [ItemName]    NVARCHAR(100)    NOT NULL,
        [Description] NVARCHAR(500)    NULL,
        [IsActive]    BIT              NOT NULL DEFAULT 1,
        [CreatedBy]   NVARCHAR(256)    NOT NULL,
        [CreatedOn]   DATETIME2        NOT NULL,
        [ModifiedBy]  NVARCHAR(256)    NULL,
        [ModifiedOn]  DATETIME2        NULL,
        [IsDeleted]   BIT              NOT NULL DEFAULT 0,
        CONSTRAINT [PK_Items] PRIMARY KEY ([Id])
    );

    CREATE UNIQUE INDEX [IX_Items_DisplayId] ON [Items] ([DisplayId]);
    PRINT 'Created table: Items';
END
GO

-- --------------------------------------------------------------------------
-- Employments
-- --------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Employments')
BEGIN
    CREATE TABLE [Employments] (
        [Id]                         UNIQUEIDENTIFIER NOT NULL,
        [DisplayId]                  BIGINT           NOT NULL,
        [DepEdId]                    NVARCHAR(50)     NULL,
        [PSIPOPItemNumber]           NVARCHAR(50)     NULL,
        [TINId]                      NVARCHAR(20)     NULL,
        [GSISId]                     NVARCHAR(20)     NULL,
        [PhilHealthId]               NVARCHAR(20)     NULL,
        [DateOfOriginalAppointment]  DATE             NULL,
        [AppointmentStatus]          INT              NOT NULL,
        [EmploymentStatus]           INT              NOT NULL,
        [Eligibility]                INT              NOT NULL,
        [IsActive]                   BIT              NOT NULL DEFAULT 1,
        [PersonId]                   UNIQUEIDENTIFIER NOT NULL,
        [PositionId]                 UNIQUEIDENTIFIER NOT NULL,
        [SalaryGradeId]              UNIQUEIDENTIFIER NOT NULL,
        [ItemId]                     UNIQUEIDENTIFIER NOT NULL,
        [CreatedBy]                  NVARCHAR(256)    NOT NULL,
        [CreatedOn]                  DATETIME2        NOT NULL,
        [ModifiedBy]                 NVARCHAR(256)    NULL,
        [ModifiedOn]                 DATETIME2        NULL,
        [IsDeleted]                  BIT              NOT NULL DEFAULT 0,
        CONSTRAINT [PK_Employments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Employments_Persons_PersonId]      FOREIGN KEY ([PersonId])      REFERENCES [Persons] ([Id])      ON DELETE NO ACTION,
        CONSTRAINT [FK_Employments_Positions_PositionId]   FOREIGN KEY ([PositionId])    REFERENCES [Positions] ([Id])    ON DELETE NO ACTION,
        CONSTRAINT [FK_Employments_SalaryGrades_SalaryGradeId] FOREIGN KEY ([SalaryGradeId]) REFERENCES [SalaryGrades] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Employments_Items_ItemId]           FOREIGN KEY ([ItemId])        REFERENCES [Items] ([Id])        ON DELETE NO ACTION
    );

    CREATE UNIQUE INDEX [IX_Employments_DisplayId] ON [Employments] ([DisplayId]);
    CREATE INDEX [IX_Employments_PersonId] ON [Employments] ([PersonId]);
    CREATE INDEX [IX_Employments_PositionId] ON [Employments] ([PositionId]);
    CREATE INDEX [IX_Employments_SalaryGradeId] ON [Employments] ([SalaryGradeId]);
    CREATE INDEX [IX_Employments_ItemId] ON [Employments] ([ItemId]);
    PRINT 'Created table: Employments';
END
GO

-- --------------------------------------------------------------------------
-- EmploymentSchools
-- --------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'EmploymentSchools')
BEGIN
    CREATE TABLE [EmploymentSchools] (
        [Id]           UNIQUEIDENTIFIER NOT NULL,
        [DisplayId]    BIGINT           NOT NULL,
        [EmploymentId] UNIQUEIDENTIFIER NOT NULL,
        [SchoolId]     UNIQUEIDENTIFIER NOT NULL,
        [StartDate]    DATE             NULL,
        [EndDate]      DATE             NULL,
        [IsCurrent]    BIT              NOT NULL DEFAULT 1,
        [IsActive]     BIT              NOT NULL DEFAULT 1,
        [CreatedBy]    NVARCHAR(256)    NOT NULL,
        [CreatedOn]    DATETIME2        NOT NULL,
        [ModifiedBy]   NVARCHAR(256)    NULL,
        [ModifiedOn]   DATETIME2        NULL,
        [IsDeleted]    BIT              NOT NULL DEFAULT 0,
        CONSTRAINT [PK_EmploymentSchools] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_EmploymentSchools_Employments_EmploymentId] FOREIGN KEY ([EmploymentId]) REFERENCES [Employments] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_EmploymentSchools_Schools_SchoolId]         FOREIGN KEY ([SchoolId])     REFERENCES [Schools] ([Id])     ON DELETE NO ACTION
    );

    CREATE UNIQUE INDEX [IX_EmploymentSchools_DisplayId] ON [EmploymentSchools] ([DisplayId]);
    CREATE INDEX [IX_EmploymentSchools_SchoolId] ON [EmploymentSchools] ([SchoolId]);
    CREATE UNIQUE INDEX [IX_EmploymentSchools_EmploymentId_SchoolId_StartDate]
        ON [EmploymentSchools] ([EmploymentId], [SchoolId], [StartDate])
        WHERE [StartDate] IS NOT NULL;
    PRINT 'Created table: EmploymentSchools';
END
GO

-- --------------------------------------------------------------------------
-- Documents
-- --------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Documents')
BEGIN
    CREATE TABLE [Documents] (
        [Id]            UNIQUEIDENTIFIER NOT NULL,
        [DisplayId]     BIGINT           NOT NULL,
        [FileName]      NVARCHAR(255)    NOT NULL,
        [FileExtension] NVARCHAR(10)     NOT NULL,
        [ContentType]   NVARCHAR(100)    NOT NULL,
        [FileSizeBytes] BIGINT           NOT NULL,
        [DocumentType]  INT              NOT NULL,
        [BlobUrl]       NVARCHAR(2048)   NOT NULL,
        [BlobName]      NVARCHAR(500)    NOT NULL,
        [ContainerName] NVARCHAR(100)    NOT NULL,
        [Description]   NVARCHAR(500)    NULL,
        [PersonId]      UNIQUEIDENTIFIER NOT NULL,
        [CreatedBy]     NVARCHAR(256)    NOT NULL,
        [CreatedOn]     DATETIME2        NOT NULL,
        [ModifiedBy]    NVARCHAR(256)    NULL,
        [ModifiedOn]    DATETIME2        NULL,
        [IsDeleted]     BIT              NOT NULL DEFAULT 0,
        CONSTRAINT [PK_Documents] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Documents_Persons_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [Persons] ([Id]) ON DELETE NO ACTION
    );

    CREATE UNIQUE INDEX [IX_Documents_DisplayId] ON [Documents] ([DisplayId]);
    CREATE INDEX [IX_Documents_PersonId] ON [Documents] ([PersonId]);
    PRINT 'Created table: Documents';
END
GO

-- --------------------------------------------------------------------------
-- Users
-- --------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE [Users] (
        [Id]                UNIQUEIDENTIFIER NOT NULL,
        [GoogleId]          NVARCHAR(100)    NOT NULL,
        [Email]             NVARCHAR(256)    NOT NULL,
        [FirstName]         NVARCHAR(100)    NOT NULL,
        [LastName]          NVARCHAR(100)    NOT NULL,
        [ProfilePictureUrl] NVARCHAR(2048)   NULL,
        [IsActive]          BIT              NOT NULL DEFAULT 1,
        [Role]              NVARCHAR(50)     NOT NULL DEFAULT N'User',
        [CreatedOn]         DATETIME2        NOT NULL,
        [LastLoginOn]       DATETIME2        NULL,
        [ModifiedBy]        NVARCHAR(256)    NULL,
        [ModifiedOn]        DATETIME2        NULL,
        [IsDeleted]         BIT              NOT NULL DEFAULT 0,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
    );

    CREATE UNIQUE INDEX [IX_Users_GoogleId] ON [Users] ([GoogleId]);
    CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);
    PRINT 'Created table: Users';
END
GO

-- --------------------------------------------------------------------------
-- RefreshTokens
-- --------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'RefreshTokens')
BEGIN
    CREATE TABLE [RefreshTokens] (
        [Id]              UNIQUEIDENTIFIER NOT NULL,
        [Token]           NVARCHAR(500)    NOT NULL,
        [ExpiresOn]       DATETIME2        NOT NULL,
        [CreatedOn]       DATETIME2        NOT NULL,
        [CreatedByIp]     NVARCHAR(50)     NULL,
        [RevokedOn]       DATETIME2        NULL,
        [RevokedByIp]     NVARCHAR(50)     NULL,
        [ReplacedByToken] NVARCHAR(500)    NULL,
        [ReasonRevoked]   NVARCHAR(256)    NULL,
        [UserId]          UNIQUEIDENTIFIER NOT NULL,
        CONSTRAINT [PK_RefreshTokens] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RefreshTokens_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );

    CREATE UNIQUE INDEX [IX_RefreshTokens_Token] ON [RefreshTokens] ([Token]);
    CREATE INDEX [IX_RefreshTokens_UserId] ON [RefreshTokens] ([UserId]);
    PRINT 'Created table: RefreshTokens';
END
GO

-- ============================================================================
-- 3. EF CORE MIGRATIONS HISTORY (mark as migrated)
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = '__EFMigrationsHistory')
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId]    NVARCHAR(150) NOT NULL,
        [ProductVersion] NVARCHAR(32)  NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
    PRINT 'Created table: __EFMigrationsHistory';
END
GO

-- ============================================================================
-- Summary
-- ============================================================================
PRINT '';
PRINT '============================================';
PRINT 'Database setup complete!';
PRINT '============================================';
PRINT '';
PRINT 'Tables created:';

SELECT t.name AS [Table], SUM(p.rows) AS [Rows]
FROM sys.tables t
JOIN sys.partitions p ON t.object_id = p.object_id AND p.index_id IN (0, 1)
GROUP BY t.name
ORDER BY t.name;

PRINT '';
PRINT 'Next step: Run seed-data.sql to populate with mock data.';
