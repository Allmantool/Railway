
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 10/04/2017 10:01:38
-- Generated from EDMX file: C:\Users\Pavel_Chyzhykau\Documents\GitHub\Railway\NaftanRailway.Domain\Concrete\DbContexts\OBD\OBDModel.edmx
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

IF OBJECT_ID(N'[OBDEntitiesStoreContainer].[v_02_podhod]', 'U') IS NOT NULL
    DROP TABLE [OBDEntitiesStoreContainer].[v_02_podhod];
GO
IF OBJECT_ID(N'[OBDEntitiesStoreContainer].[v_akt]', 'U') IS NOT NULL
    DROP TABLE [OBDEntitiesStoreContainer].[v_akt];
GO
IF OBJECT_ID(N'[OBDEntitiesStoreContainer].[v_akt_vag]', 'U') IS NOT NULL
    DROP TABLE [OBDEntitiesStoreContainer].[v_akt_vag];
GO
IF OBJECT_ID(N'[OBDEntitiesStoreContainer].[v_kart]', 'U') IS NOT NULL
    DROP TABLE [OBDEntitiesStoreContainer].[v_kart];
GO
IF OBJECT_ID(N'[OBDEntitiesStoreContainer].[v_nach]', 'U') IS NOT NULL
    DROP TABLE [OBDEntitiesStoreContainer].[v_nach];
GO
IF OBJECT_ID(N'[OBDEntitiesStoreContainer].[v_o_v]', 'U') IS NOT NULL
    DROP TABLE [OBDEntitiesStoreContainer].[v_o_v];
GO
IF OBJECT_ID(N'[OBDEntitiesStoreContainer].[v_OPER_ASUS]', 'U') IS NOT NULL
    DROP TABLE [OBDEntitiesStoreContainer].[v_OPER_ASUS];
GO
IF OBJECT_ID(N'[OBDEntitiesStoreContainer].[v_otpr]', 'U') IS NOT NULL
    DROP TABLE [OBDEntitiesStoreContainer].[v_otpr];
GO
IF OBJECT_ID(N'[OBDEntitiesStoreContainer].[v_pam]', 'U') IS NOT NULL
    DROP TABLE [OBDEntitiesStoreContainer].[v_pam];
GO
IF OBJECT_ID(N'[OBDEntitiesStoreContainer].[v_pam_vag]', 'U') IS NOT NULL
    DROP TABLE [OBDEntitiesStoreContainer].[v_pam_vag];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'v_karts'
CREATE TABLE [dbo].[v_karts] (
    [id] int  NOT NULL,
    [num_kart] char(6)  NULL,
    [date_okrt] datetime  NULL,
    [cod_pl] char(6)  NULL,
    [type_kart] smallint  NULL,
    [cod_ls] char(2)  NULL,
    [summa] decimal(18,2)  NULL,
    [state] bigint  NULL,
    [date_fdu93] datetime  NULL,
    [date_zkrt] datetime  NULL
);
GO

-- Creating table 'v_nachs'
CREATE TABLE [dbo].[v_nachs] (
    [id] int  NOT NULL,
    [id_kart] int  NOT NULL,
    [id_otpr] int  NULL,
    [cod_kl] char(6)  NULL,
    [type_doc] int  NULL,
    [oper] smallint  NULL,
    [cod_sbor] char(6)  NULL,
    [date_raskr] datetime  NULL,
    [num_doc] char(8)  NULL,
    [cena] decimal(18,2)  NULL,
    [kol] decimal(18,2)  NULL,
    [summa] decimal(18,2)  NULL,
    [nds] decimal(18,2)  NULL,
    [textm] nvarchar(max)  NULL,
    [id_ed] int  NULL
);
GO

-- Creating table 'v_o_vs'
CREATE TABLE [dbo].[v_o_vs] (
    [id] bigint  NOT NULL,
    [id_otpr] int  NOT NULL,
    [n_vag] varchar(8)  NULL,
    [massa] int  NULL
);
GO

-- Creating table 'v_OPER_ASUSs'
CREATE TABLE [dbo].[v_OPER_ASUSs] (
    [id] bigint  NOT NULL,
    [obj_id] bigint  NULL,
    [id_vag] bigint  NULL,
    [cod_oper] char(2)  NULL,
    [time_oper] datetime  NULL,
    [in_vgn] char(8)  NULL,
    [ves_gruz] decimal(12,3)  NULL,
    [plomb] smallint  NULL,
    [primech] varchar(15)  NULL,
    [cod_grpl] char(4)  NULL,
    [cod_gruz] char(6)  NULL
);
GO

-- Creating table 'v_otprs'
CREATE TABLE [dbo].[v_otprs] (
    [id] int  NOT NULL,
    [state] int  NULL,
    [oper] smallint  NULL,
    [date_oper] datetime  NULL,
    [n_otpr] char(8)  NULL,
    [cod_kl_otpr] char(20)  NULL,
    [cod_klient_pol] char(20)  NULL,
    [g6] varchar(300)  NULL,
    [g4] varchar(max)  NULL,
    [g16] char(34)  NULL,
    [g8] varchar(350)  NULL,
    [g11] varchar(max)  NULL,
    [cod_tvk_etsng] char(6)  NULL,
    [cod_tvk_algng] char(8)  NULL,
    [massa_otpr] int  NULL,
    [name_plat] varchar(140)  NULL,
    [type_doc] int  NULL,
    [adr_otpr] varchar(250)  NULL,
    [adr_pol] varchar(250)  NULL,
    [nam_otpr] varchar(140)  NULL,
    [nam_pol] varchar(140)  NULL,
    [fio_tk] varchar(15)  NULL,
    [eid] bigint  NULL,
    [eid_zag] bigint  NULL
);
GO

-- Creating table 'v_pams'
CREATE TABLE [dbo].[v_pams] (
    [id_ved] int  NOT NULL,
    [nved] char(6)  NULL,
    [dved] datetime  NULL,
    [dzakr] datetime  NULL,
    [kodkl] char(6)  NULL,
    [nkrt] char(6)  NULL,
    [id_kart] int  NULL,
    [state] int  NULL
);
GO

-- Creating table 'v_pam_vags'
CREATE TABLE [dbo].[v_pam_vags] (
    [id_vag] int  NOT NULL,
    [id_ved] int  NOT NULL,
    [nomvag] char(8)  NULL,
    [kodgr] char(6)  NULL,
    [kodop] smallint  NULL,
    [d_pod] datetime  NULL,
    [d_ub] datetime  NULL
);
GO

-- Creating table 'v_akts'
CREATE TABLE [dbo].[v_akts] (
    [id] int  NOT NULL,
    [kodls] char(2)  NULL,
    [dakt] datetime  NULL,
    [kodkl] char(6)  NULL,
    [nkrt] char(6)  NULL,
    [id_kart] int  NULL,
    [state] int  NULL,
    [nakt] char(6)  NULL
);
GO

-- Creating table 'v_akt_vags'
CREATE TABLE [dbo].[v_akt_vags] (
    [id] int  NOT NULL,
    [id_akt] int  NOT NULL,
    [nomvag] char(12)  NULL
);
GO

-- Creating table 'v_02_podhods'
CREATE TABLE [dbo].[v_02_podhods] (
    [n_vag] char(8)  NOT NULL,
    [massa_t] int  NULL,
    [st_nazn] char(5)  NULL,
    [kod_etsng] char(5)  NULL,
    [kod_pol] char(4)  NULL,
    [prim] char(6)  NULL,
    [date_oper_v] datetime  NULL,
    [oper] smallint  NULL,
    [n_otpr] char(8)  NULL,
    [pr_v] smallint  NOT NULL,
    [kod_stan_oper] nvarchar(max)  NOT NULL,
    [date_oper_t] datetime  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [id] in table 'v_karts'
ALTER TABLE [dbo].[v_karts]
ADD CONSTRAINT [PK_v_karts]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id], [id_kart] in table 'v_nachs'
ALTER TABLE [dbo].[v_nachs]
ADD CONSTRAINT [PK_v_nachs]
    PRIMARY KEY CLUSTERED ([id], [id_kart] ASC);
GO

-- Creating primary key on [id], [id_otpr] in table 'v_o_vs'
ALTER TABLE [dbo].[v_o_vs]
ADD CONSTRAINT [PK_v_o_vs]
    PRIMARY KEY CLUSTERED ([id], [id_otpr] ASC);
GO

-- Creating primary key on [id] in table 'v_OPER_ASUSs'
ALTER TABLE [dbo].[v_OPER_ASUSs]
ADD CONSTRAINT [PK_v_OPER_ASUSs]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'v_otprs'
ALTER TABLE [dbo].[v_otprs]
ADD CONSTRAINT [PK_v_otprs]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id_ved] in table 'v_pams'
ALTER TABLE [dbo].[v_pams]
ADD CONSTRAINT [PK_v_pams]
    PRIMARY KEY CLUSTERED ([id_ved] ASC);
GO

-- Creating primary key on [id_vag], [id_ved] in table 'v_pam_vags'
ALTER TABLE [dbo].[v_pam_vags]
ADD CONSTRAINT [PK_v_pam_vags]
    PRIMARY KEY CLUSTERED ([id_vag], [id_ved] ASC);
GO

-- Creating primary key on [id] in table 'v_akts'
ALTER TABLE [dbo].[v_akts]
ADD CONSTRAINT [PK_v_akts]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id], [id_akt] in table 'v_akt_vags'
ALTER TABLE [dbo].[v_akt_vags]
ADD CONSTRAINT [PK_v_akt_vags]
    PRIMARY KEY CLUSTERED ([id], [id_akt] ASC);
GO

-- Creating primary key on [n_vag] in table 'v_02_podhods'
ALTER TABLE [dbo].[v_02_podhods]
ADD CONSTRAINT [PK_v_02_podhods]
    PRIMARY KEY CLUSTERED ([n_vag] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [id_kart] in table 'v_nachs'
ALTER TABLE [dbo].[v_nachs]
ADD CONSTRAINT [FK_v_kartv_nach]
    FOREIGN KEY ([id_kart])
    REFERENCES [dbo].[v_karts]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_v_kartv_nach'
CREATE INDEX [IX_FK_v_kartv_nach]
ON [dbo].[v_nachs]
    ([id_kart]);
GO

-- Creating foreign key on [id_otpr] in table 'v_o_vs'
ALTER TABLE [dbo].[v_o_vs]
ADD CONSTRAINT [FK_v_otprv_o_v]
    FOREIGN KEY ([id_otpr])
    REFERENCES [dbo].[v_otprs]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_v_otprv_o_v'
CREATE INDEX [IX_FK_v_otprv_o_v]
ON [dbo].[v_o_vs]
    ([id_otpr]);
GO

-- Creating foreign key on [id_akt] in table 'v_akt_vags'
ALTER TABLE [dbo].[v_akt_vags]
ADD CONSTRAINT [FK_v_aktv_akt_vag]
    FOREIGN KEY ([id_akt])
    REFERENCES [dbo].[v_akts]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_v_aktv_akt_vag'
CREATE INDEX [IX_FK_v_aktv_akt_vag]
ON [dbo].[v_akt_vags]
    ([id_akt]);
GO

-- Creating foreign key on [id_ved] in table 'v_pam_vags'
ALTER TABLE [dbo].[v_pam_vags]
ADD CONSTRAINT [FK_v_pamv_pam_vag]
    FOREIGN KEY ([id_ved])
    REFERENCES [dbo].[v_pams]
        ([id_ved])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_v_pamv_pam_vag'
CREATE INDEX [IX_FK_v_pamv_pam_vag]
ON [dbo].[v_pam_vags]
    ([id_ved]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------