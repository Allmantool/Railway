
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 10/04/2017 10:02:41
-- Generated from EDMX file: C:\Users\Pavel_Chyzhykau\Documents\GitHub\Railway\NaftanRailway.Domain\Concrete\DbContexts\ORC\ORCModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [Rail];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_krt__Naftan]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[krt_Naftan_orc_sapod] DROP CONSTRAINT [FK_krt__Naftan];
GO
IF OBJECT_ID(N'[dbo].[FK_orc_sbor_orc_krt]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[orc_sbor] DROP CONSTRAINT [FK_orc_sbor_orc_krt];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[krt_Guild18]', 'U') IS NOT NULL
    DROP TABLE [dbo].[krt_Guild18];
GO
IF OBJECT_ID(N'[dbo].[krt_Naftan]', 'U') IS NOT NULL
    DROP TABLE [dbo].[krt_Naftan];
GO
IF OBJECT_ID(N'[dbo].[krt_Naftan_orc_sapod]', 'U') IS NOT NULL
    DROP TABLE [dbo].[krt_Naftan_orc_sapod];
GO
IF OBJECT_ID(N'[dbo].[orc_krt]', 'U') IS NOT NULL
    DROP TABLE [dbo].[orc_krt];
GO
IF OBJECT_ID(N'[dbo].[orc_sbor]', 'U') IS NOT NULL
    DROP TABLE [dbo].[orc_sbor];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'krt_Guild18s'
CREATE TABLE [dbo].[krt_Guild18s] (
    [reportPeriod] datetime  NOT NULL,
    [warehouse] int  NULL,
    [idDeliviryNote] int  NULL,
    [type_doc] tinyint  NOT NULL,
    [idSrcDocument] int  NULL,
    [codeType] bit  NOT NULL,
    [code] int  NOT NULL,
    [sum] decimal(18,2)  NOT NULL,
    [rateVAT] decimal(3,2)  NOT NULL,
    [idScroll] bigint  NULL,
    [idCard] int  NULL,
    [idSapod] int  NULL,
    [scrollColl] bigint  NULL,
    [id] int IDENTITY(1,1) NOT NULL
);
GO

-- Creating table 'krt_Naftans'
CREATE TABLE [dbo].[krt_Naftans] (
    [KEYKRT] bigint  NOT NULL,
    [NKRT] int  NOT NULL,
    [NTREB] int  NOT NULL,
    [DTBUHOTCHET] datetime  NOT NULL,
    [DTTREB] datetime  NULL,
    [DTOPEN] datetime  NULL,
    [DTCLOSE] datetime  NULL,
    [SMTREB] decimal(18,2)  NOT NULL,
    [NDSTREB] decimal(18,2)  NOT NULL,
    [U_KOD] smallint  NOT NULL,
    [P_TYPE] nvarchar(1)  NOT NULL,
    [DATE_OBRABOT] datetime  NOT NULL,
    [IN_REAL] bit  NOT NULL,
    [RecordCount] int  NOT NULL,
    [StartDate_PER] datetime  NOT NULL,
    [EndDate_PER] datetime  NOT NULL,
    [SignAdjustment_list] bit  NOT NULL,
    [Scroll_Sbor] nvarchar(max)  NOT NULL,
    [Confirmed] bit  NOT NULL,
    [ErrorState] tinyint  NULL,
    [ErrorMsg] nvarchar(max)  NULL,
    [CounterVersion] tinyint  NOT NULL
);
GO

-- Creating table 'krt_Naftan_orc_sapods'
CREATE TABLE [dbo].[krt_Naftan_orc_sapods] (
    [keykrt] bigint  NOT NULL,
    [keysbor] bigint  NOT NULL,
    [nomot] nvarchar(10)  NOT NULL,
    [dt] datetime  NOT NULL,
    [gruname] nvarchar(max)  NOT NULL,
    [vidsbr] smallint  NOT NULL,
    [namesbr] nvarchar(max)  NOT NULL,
    [sm_no_nds] decimal(18,2)  NOT NULL,
    [sm_nds] decimal(18,2)  NOT NULL,
    [sm] decimal(18,2)  NOT NULL,
    [stnds] decimal(18,2)  NOT NULL,
    [txt] nvarchar(max)  NULL,
    [UNI_OTPR] nvarchar(50)  NOT NULL,
    [nkrt] nvarchar(10)  NOT NULL,
    [tdoc] tinyint  NOT NULL,
    [ORC_ID_ED] int  NOT NULL,
    [id] int  NULL,
    [id_kart] int  NULL,
    [id_otpr] int  NULL,
    [date_raskr] datetime  NULL,
    [num_doc] nvarchar(10)  NULL,
    [cena] decimal(18,2)  NULL,
    [kol] decimal(18,2)  NULL,
    [summa] decimal(18,2)  NULL,
    [nds] decimal(18,2)  NULL,
    [textm] nvarchar(max)  NULL,
    [ID_ED] int  NULL,
    [ErrorState] tinyint  NULL
);
GO

-- Creating table 'orc_krts'
CREATE TABLE [dbo].[orc_krts] (
    [KEYKRT] bigint  NOT NULL,
    [NKRT] int  NULL,
    [NTREB] int  NULL,
    [DTTREB] datetime  NULL,
    [DTOPEN] datetime  NULL,
    [DTCLOSE] datetime  NULL,
    [SMTREB] decimal(18,2)  NULL,
    [NDSTREB] decimal(18,2)  NULL,
    [U_KOD] int  NULL,
    [P_TYPE] char(1)  NULL,
    [DATE_OBRABOT] datetime  NULL,
    [IN_REAL] bit  NOT NULL
);
GO

-- Creating table 'orc_sbors'
CREATE TABLE [dbo].[orc_sbors] (
    [KEYKRT] bigint  NOT NULL,
    [KEYSBOR] bigint  NOT NULL,
    [NOMOT] nvarchar(20)  NULL,
    [DT] datetime  NULL,
    [GRUNAME] nvarchar(150)  NULL,
    [VIDSBR] int  NULL,
    [NAMESBR] nvarchar(150)  NULL,
    [TXT] nvarchar(20)  NULL,
    [SM] decimal(18,2)  NULL,
    [SM_NDS] decimal(18,2)  NULL,
    [STNDS] decimal(18,2)  NULL,
    [UNI_OTPR] nvarchar(20)  NULL,
    [ID_KART] nvarchar(20)  NULL,
    [NKRT] nvarchar(20)  NULL,
    [P_TYPE] char(1)  NULL,
    [DATE_OBRABOT] datetime  NULL,
    [TDOC] int  NULL,
    [ID_ED] int  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [id] in table 'krt_Guild18s'
ALTER TABLE [dbo].[krt_Guild18s]
ADD CONSTRAINT [PK_krt_Guild18s]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [KEYKRT] in table 'krt_Naftans'
ALTER TABLE [dbo].[krt_Naftans]
ADD CONSTRAINT [PK_krt_Naftans]
    PRIMARY KEY CLUSTERED ([KEYKRT] ASC);
GO

-- Creating primary key on [keykrt], [keysbor] in table 'krt_Naftan_orc_sapods'
ALTER TABLE [dbo].[krt_Naftan_orc_sapods]
ADD CONSTRAINT [PK_krt_Naftan_orc_sapods]
    PRIMARY KEY CLUSTERED ([keykrt], [keysbor] ASC);
GO

-- Creating primary key on [KEYKRT] in table 'orc_krts'
ALTER TABLE [dbo].[orc_krts]
ADD CONSTRAINT [PK_orc_krts]
    PRIMARY KEY CLUSTERED ([KEYKRT] ASC);
GO

-- Creating primary key on [KEYSBOR], [KEYKRT] in table 'orc_sbors'
ALTER TABLE [dbo].[orc_sbors]
ADD CONSTRAINT [PK_orc_sbors]
    PRIMARY KEY CLUSTERED ([KEYSBOR], [KEYKRT] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [keykrt] in table 'krt_Naftan_orc_sapods'
ALTER TABLE [dbo].[krt_Naftan_orc_sapods]
ADD CONSTRAINT [FK_krt__Naftan]
    FOREIGN KEY ([keykrt])
    REFERENCES [dbo].[krt_Naftans]
        ([KEYKRT])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [KEYKRT] in table 'orc_sbors'
ALTER TABLE [dbo].[orc_sbors]
ADD CONSTRAINT [FK_orc_sbor_orc_krt]
    FOREIGN KEY ([KEYKRT])
    REFERENCES [dbo].[orc_krts]
        ([KEYKRT])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_orc_sbor_orc_krt'
CREATE INDEX [IX_FK_orc_sbor_orc_krt]
ON [dbo].[orc_sbors]
    ([KEYKRT]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------