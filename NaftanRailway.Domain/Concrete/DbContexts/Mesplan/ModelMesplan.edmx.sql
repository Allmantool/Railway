
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 10/04/2017 09:58:40
-- Generated from EDMX file: C:\Users\Pavel_Chyzhykau\Documents\GitHub\Railway\NaftanRailway.Domain\Concrete\DbContexts\Mesplan\ModelMesplan.edmx
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


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[MesplanModelStoreContainer].[etsng]', 'U') IS NOT NULL
    DROP TABLE [MesplanModelStoreContainer].[etsng];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'etsngs'
CREATE TABLE [dbo].[etsngs] (
    [etsng1] char(6)  NOT NULL,
    [name] char(254)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [etsng1], [name] in table 'etsngs'
ALTER TABLE [dbo].[etsngs]
ADD CONSTRAINT [PK_etsngs]
    PRIMARY KEY CLUSTERED ([etsng1], [name] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------