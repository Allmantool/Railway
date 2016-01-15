SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[v_pam_vag](
	[id_vag] [int] NOT NULL,
	[id_ved] [int] NOT NULL,
	[npam] [char](5) COLLATE SQL_Latin1_General_CP1251_CI_AS NULL,
	[npp] [smallint] NULL,
	[nomvag] [char](8) COLLATE SQL_Latin1_General_CP1251_CI_AS NULL,
	[sob_s] [char](2) COLLATE SQL_Latin1_General_CP1251_CI_AS NULL,
	[sob_v] [smallint] NULL,
	[sogl] [char](1) COLLATE SQL_Latin1_General_CP1251_CI_AS NULL,
	[tipvag] [smallint] NULL,
	[kodgr] [char](6) COLLATE SQL_Latin1_General_CP1251_CI_AS NULL,
	[kodop] [smallint] NULL,
	[d_pod] [datetime] NULL,
	[d_vig] [datetime] NULL,
	[d_ok] [datetime] NULL,
	[d_ub] [datetime] NULL,
	[t_man] [int] NULL,
	[t_nor] [int] NULL,
	[t_dop] [int] NULL,
	[krat] [numeric](18, 3) NULL,
	[prim] [varchar](77) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[prob] [int] NULL,
	[prsvnor] [int] NULL,
	[id_otpr1] [int] NULL,
	[id_otpr2] [int] NULL,
	[no_plat_podach] [int] NULL
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[v_pam_sb](
	[id_ved] [int] NOT NULL,
	[kod_sb] [char](3) COLLATE SQL_Latin1_General_CP1251_CI_AS NULL,
	[sum_sb] [numeric](18, 3) NULL,
	[nds_sb] [numeric](18, 3) NULL,
	[kodusl] [smallint] NULL,
	[textm] [ntext] COLLATE SQL_Latin1_General_CP1251_CI_AS NULL,
	[d_sb] [datetime] NULL,
	[kol_oper] [int] NULL
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[v_pam](
	[id_ved] [int] NOT NULL,
	[id_ved_dbf] [char](11) COLLATE SQL_Latin1_General_CP1251_CI_AS NULL,
	[kodls] [char](2) COLLATE SQL_Latin1_General_CP1251_CI_AS NULL,
	[nved] [char](6) COLLATE SQL_Latin1_General_CP1251_CI_AS NULL,
	[dved] [datetime] NULL,
	[dzakr] [datetime] NULL,
	[kodkl] [char](6) COLLATE SQL_Latin1_General_CP1251_CI_AS NULL,
	[ndog] [varchar](20) COLLATE SQL_Latin1_General_CP1251_CI_AS NULL,
	[r_oba] [numeric](18, 3) NULL,
	[pr_put] [smallint] NULL,
	[put] [char](6) COLLATE SQL_Latin1_General_CP1251_CI_AS NULL,
	[pr_lok] [smallint] NULL,
	[pr_lokf] [smallint] NULL,
	[prlokub] [smallint] NULL,
	[pr_razg] [smallint] NULL,
	[nkrt] [char](6) COLLATE SQL_Latin1_General_CP1251_CI_AS NULL,
	[id_kart] [int] NULL,
	[n_tab] [smallint] NULL,
	[v_ob] [smallint] NULL,
	[nper] [char](3) COLLATE SQL_Latin1_General_CP1251_CI_AS NULL,
	[state] [int] NULL,
	[pr_external_work] [int] NULL,
 CONSTRAINT [PK_v_pam] PRIMARY KEY CLUSTERED 
(
	[id_ved] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[v_otpr](
	[id] [int] NOT NULL,
	[state] [int] NULL,
	[n_visas] [bigint] NULL,
	[num_zag] [char](12) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[uni_otpr] [char](10) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[stan_oper] [char](6) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[oper] [smallint] NULL,
	[date_oper] [datetime] NULL,
	[n_otpr] [char](8) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[vid_doc] [smallint] NULL,
	[cod_vidot] [smallint] NULL,
	[cod_v_soob] [smallint] NULL,
	[skor] [smallint] NULL,
	[date_p] [datetime] NULL,
	[date_o] [datetime] NULL,
	[cod_st_otpr] [char](6) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[cod_coun_otpr] [smallint] NULL,
	[cod_kl_otpr] [char](20) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[nod_kl_otpr] [smallint] NULL,
	[cod_st_nazn] [char](6) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[cod_coun_nazn] [smallint] NULL,
	[cod_klient_pol] [char](20) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[nod_kl_pol] [smallint] NULL,
	[rast_bel] [int] NULL,
	[cod_currency] [char](3) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[cod_o_mas] [smallint] NULL,
	[pogr] [char](1) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[paragr] [char](2) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[glava] [char](2) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[g6] [varchar](300) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[cena] [numeric](16, 2) NULL,
	[shema] [smallint] NULL,
	[plata] [numeric](18, 2) NULL,
	[rod_ps] [char](4) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[cod_vidras] [smallint] NULL,
	[cod_vidrab] [smallint] NULL,
	[g4] [text] COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[g16] [char](34) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[codrsd] [char](10) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[g8] [varchar](40) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[g91] [varchar](189) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[cod_adm_o] [smallint] NULL,
	[cod_adm_n] [smallint] NULL,
	[date_dostav] [datetime] NULL,
	[massa_rasch] [int] NULL,
	[g5] [varchar](200) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[g11] [text] COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[g18] [varchar](52) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[g19] [varchar](32) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[g20] [varchar](143) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[g26] [varchar](72) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[g1] [varchar](250) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[cod_tvk_etsng] [char](6) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[cod_tvk_algng] [char](8) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[massa_otpr] [int] NULL,
	[cod_upak] [smallint] NULL,
	[tvg_k_mest] [smallint] NULL,
	[tvg_tara] [smallint] NULL,
	[tvk_zn_otpr] [char](10) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[rem_isk_tarif] [varchar](30) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[tiptarif] [varchar](10) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[dogovor_kl_kl] [varchar](50) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[cod_st_sort_kont] [char](6) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[cod_plat] [char](8) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[name_plat] [varchar](80) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[nod_plat] [smallint] NULL,
	[type_doc] [int] NULL,
	[vid_vag_pf] [smallint] NULL,
	[container_train] [smallint] NULL,
	[cod_tip_tar] [int] NULL,
	[pl_provoz] [numeric](18, 2) NULL,
	[nds_pl_provoz] [numeric](18, 2) NULL,
	[pl_prov] [numeric](18, 2) NULL,
	[nds_pl_prov] [numeric](18, 2) NULL,
	[pl_dsb] [numeric](18, 2) NULL,
	[nds_pl_dsb] [numeric](18, 2) NULL,
	[pl_all] [numeric](18, 2) NULL,
	[pl_pr_ot] [numeric](18, 2) NULL,
	[pl_prov_ot] [numeric](18, 2) NULL,
	[pl_prib_tranzit] [numeric](18, 2) NULL,
	[pl_ot_tran] [numeric](18, 2) NULL,
	[pr_nds_pl_provoz] [int] NULL,
	[pr_nds_dsb] [int] NULL,
	[k_usr_br] [numeric](18, 2) NULL,
	[k_rur_br] [numeric](18, 2) NULL,
	[tp_stavka] [numeric](18, 4) NULL,
	[g8_1] [varchar](125) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[adr_kl_1000] [varchar](200) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[fio_kl_1000] [varchar](200) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[ves_one_places] [numeric](18, 3) NULL,
	[vidkoforrr] [int] NULL,
	[extract_passport] [bit] NULL,
	[stan_izlom0] [char](6) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[stan_izlom1] [char](6) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[stan_izlom2] [char](6) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[kof_tp] [numeric](7, 4) NULL,
	[nds_tp] [numeric](7, 4) NULL,
	[g8_stik] [varchar](50) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[pogr_ves] [numeric](18, 6) NULL,
	[save_dispatcher_g20] [bit] NULL,
	[num_doc_reforwarding] [char](8) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[stan_transfer] [char](6) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[rate_divide_scales] [smallint] NULL,
	[rem_vid_ras] [varchar](200) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[rem_skidki] [varchar](200) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[skidka] [smallint] NULL,
	[kof_skid] [numeric](18, 4) NULL,
	[pr_external_work] [int] NULL,
	[adr_otpr] [varchar](250) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[adr_pol] [varchar](250) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[nam_otpr] [varchar](130) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[nam_pol] [varchar](130) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[fio_tk] [varchar](15) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[id_plant] [char](36) COLLATE Cyrillic_General_CI_AS NULL,
	[class_dan] [char](3) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[num_oon] [smallint] NULL,
	[num_dan_kart] [smallint] NULL,
	[id_mes_plan] [int] NULL,
	[return_empty_vag] [numeric](18, 2) NULL,
	[nds_return_empty] [numeric](18, 2) NULL,
	[prib_return_empty] [numeric](18, 2) NULL,
 CONSTRAINT [PK_v_otpr] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[v_o_v](
	[id] [bigint] NOT NULL,
	[id_otpr] [int] NOT NULL,
	[n_vag] [varchar](8) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[prov] [smallint] NULL,
	[texn] [smallint] NULL,
	[massa] [int] NULL,
	[prim] [varchar](30) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[pr_zam] [smallint] NULL,
	[cod_vidot] [smallint] NULL,
	[ves_n_ob] [int] NULL,
	[n_sx] [smallint] NULL,
	[npp] [smallint] NULL,
	[n_vag_p] [char](12) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[pr_v_k] [smallint] NULL,
	[cod_gruz_old] [char](6) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[sobs] [smallint] NULL,
	[cod_negab] [char](4) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[poluchatel] [char](20) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[nom_scheme1] [char](3) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[nom_scheme2] [char](3) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[n_ob_in_tara] [bit] NULL,
	[cod_stan_nazn] [char](6) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[proveren_tara] [numeric](18, 2) NULL,
	[pl_v] [numeric](18, 2) NULL,
	[pl_prov] [numeric](18, 2) NULL,
	[nds_pl_v] [numeric](18, 2) NULL,
	[nds_pl_prov] [numeric](18, 2) NULL,
	[pl_pereadr_prib] [numeric](18, 2) NULL,
	[pl_pereadr_prov_prib] [numeric](18, 2) NULL,
	[ndspl_pereadr_prib] [numeric](18, 2) NULL,
	[ndspl_pereadr_prov_prib] [numeric](18, 2) NULL,
	[pl_otpr] [numeric](18, 2) NULL,
	[pl_prov_otpr] [numeric](18, 2) NULL,
	[pl_pereadr] [numeric](18, 2) NULL,
	[pl_pereadr_prov] [numeric](18, 2) NULL,
	[ndspl_pereadr] [numeric](18, 2) NULL,
	[ndspl_pereadr_prov] [numeric](18, 2) NULL,
	[poniz_kof_tp] [numeric](18, 3) NULL,
	[pr_skid] [smallint] NULL,
	[num_tel_is_tar] [char](15) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[kol_vag_metan] [int] NULL,
	[is_print] [bit] NULL,
	[kof_skid] [numeric](18, 4) NULL,
	[is_skid] [bit] NULL,
 CONSTRAINT [PK_v_o_v] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[v_nach](
	[id] [int] NOT NULL,
	[id_kart] [int] NOT NULL,
	[id_otpr] [int] NULL,
	[uni_otpr] [char](10) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[cod_kl] [char](6) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[nod_kl] [smallint] NULL,
	[type_doc] [int] NULL,
	[oper] [smallint] NULL,
	[cod_sbor] [char](6) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[date_raskr] [datetime] NULL,
	[num_doc] [char](8) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[cena] [numeric](18, 2) NULL,
	[kol] [numeric](18, 2) NULL,
	[summa] [numeric](18, 2) NULL,
	[nds] [numeric](18, 2) NULL,
	[cod_currency] [char](3) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[num_kart_sd] [char](3) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[textm] [ntext] COLLATE SQL_Latin1_General_CP1251_CI_AS NULL,
	[id_ed] [int] NULL,
 CONSTRAINT [PK_v_nach] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[v_kart](
	[id] [int] NOT NULL,
	[uni_kart] [char](10) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[num_kart] [char](6) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[cod_stan] [char](6) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[date_okrt] [datetime] NULL,
	[cod_pl] [char](6) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[nod_pl] [smallint] NULL,
	[mfo] [char](9) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[rash] [char](13) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[num_plat_por] [int] NULL,
	[date_plat_por] [datetime] NULL,
	[type_kart] [smallint] NULL,
	[cod_ls] [char](2) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[num_dogovor] [char](10) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[cod_currency] [char](3) COLLATE SQL_Latin1_General_CP1251_CS_AS NULL,
	[summa] [numeric](18, 2) NULL,
	[state] [bigint] NULL,
	[num_fdu93] [varchar](40) COLLATE Cyrillic_General_CI_AS NULL,
	[date_fdu93] [datetime] NULL,
	[pr_external_work] [int] NULL,
	[state_dbf] [bigint] NULL,
	[date_zkrt] [datetime] NULL,
 CONSTRAINT [PK_v_kart] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[v_akt_vag](
	[id] [int] NOT NULL,
	[id_akt] [int] NOT NULL,
	[nomvag] [char](12) COLLATE SQL_Latin1_General_CP1251_CI_AS NULL
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[v_akt_sb](
	[id] [int] NOT NULL,
	[id_akt] [int] NOT NULL,
	[id_vag] [int] NOT NULL,
	[kod_sb] [char](3) COLLATE SQL_Latin1_General_CP1251_CI_AS NULL,
	[sum_sb] [numeric](18, 3) NULL,
	[nds_sb] [numeric](18, 3) NULL,
	[textm] [ntext] COLLATE SQL_Latin1_General_CP1251_CI_AS NULL
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[v_akt](
	[id] [int] NOT NULL,
	[kodls] [char](2) COLLATE SQL_Latin1_General_CP1251_CI_AS NULL,
	[nakt] [char](6) COLLATE SQL_Latin1_General_CP1251_CI_AS NULL,
	[dakt] [datetime] NULL,
	[kodkl] [char](6) COLLATE SQL_Latin1_General_CP1251_CI_AS NULL,
	[nkrt] [char](6) COLLATE SQL_Latin1_General_CP1251_CI_AS NULL,
	[id_kart] [int] NULL,
	[state] [int] NULL,
	[pr_external_work] [int] NULL,
 CONSTRAINT [PK_v_akt] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[krt_Naftan](
	[KEYKRT] [bigint] NOT NULL,
	[NKRT] [int] NOT NULL,
	[NTREB] [int] NOT NULL,
	[DTBUHOTCHET] [date] NOT NULL,
	[DTTREB] [date] NULL,
	[DTOPEN] [date] NULL,
	[DTCLOSE] [date] NULL,
	[SMTREB] [numeric](18, 2) NOT NULL,
	[NDSTREB] [numeric](18, 2) NOT NULL,
	[U_KOD] [smallint] NOT NULL,
	[P_TYPE] [nvarchar](1) COLLATE Cyrillic_General_CI_AS NOT NULL,
	[DATE_OBRABOT] [datetime] NOT NULL,
	[IN_REAL] [bit] NOT NULL,
	[RecordCount] [int] NOT NULL,
	[StartDate_PER] [date] NOT NULL,
	[EndDate_PER] [date] NOT NULL,
	[SignAdjustment_list] [bit] NOT NULL,
	[Scroll_Sbor] [nvarchar](120) COLLATE Cyrillic_General_CI_AS NOT NULL,
	[Confirmed] [bit] NOT NULL,
 CONSTRAINT [PK_krt_naftan] PRIMARY KEY CLUSTERED 
(
	[KEYKRT] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[etsng](
	[etsng] [char](6) COLLATE Cyrillic_General_CI_AS NOT NULL,
	[name] [char](254) COLLATE Cyrillic_General_CI_AS NOT NULL,
 CONSTRAINT [PK_etsng] PRIMARY KEY CLUSTERED 
(
	[etsng] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[orc_krt](
	[KEYKRT] [bigint] NOT NULL,
	[NKRT] [int] NULL,
	[NTREB] [int] NULL,
	[DTTREB] [datetime] NULL,
	[DTOPEN] [datetime] NULL,
	[DTCLOSE] [datetime] NULL,
	[SMTREB] [numeric](18, 2) NULL,
	[NDSTREB] [numeric](18, 2) NULL,
	[U_KOD] [int] NULL,
	[P_TYPE] [char](1) COLLATE Cyrillic_General_CI_AS NULL,
	[DATE_OBRABOT] [datetime] NULL,
	[IN_REAL] [bit] NOT NULL,
 CONSTRAINT [PK_orc_krt] PRIMARY KEY CLUSTERED 
(
	[KEYKRT] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[orc_sbor](
	[KEYKRT] [bigint] NOT NULL,
	[KEYSBOR] [bigint] NOT NULL,
	[NOMOT] [nvarchar](20) COLLATE Cyrillic_General_CI_AS NULL,
	[DT] [datetime] NULL,
	[GRUNAME] [nvarchar](150) COLLATE Cyrillic_General_CI_AS NULL,
	[VIDSBR] [int] NULL,
	[NAMESBR] [nvarchar](150) COLLATE Cyrillic_General_CI_AS NULL,
	[TXT] [nvarchar](20) COLLATE Cyrillic_General_CI_AS NULL,
	[SM] [numeric](18, 2) NULL,
	[SM_NDS] [numeric](18, 2) NULL,
	[STNDS] [numeric](18, 2) NULL,
	[UNI_OTPR] [nvarchar](20) COLLATE Cyrillic_General_CI_AS NULL,
	[ID_KART] [nvarchar](20) COLLATE Cyrillic_General_CI_AS NULL,
	[NKRT] [nvarchar](20) COLLATE Cyrillic_General_CI_AS NULL,
	[P_TYPE] [char](1) COLLATE Cyrillic_General_CI_AS NULL,
	[DATE_OBRABOT] [datetime] NULL,
	[TDOC] [int] NULL,
	[ID_ED] [int] NULL,
 CONSTRAINT [PK_orc_sbor] PRIMARY KEY CLUSTERED 
(
	[KEYKRT] ASC,
	[KEYSBOR] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[krt_Naftan_orc_sapod](
	[keykrt] [bigint] NOT NULL,
	[keysbor] [bigint] NOT NULL,
	[nomot] [nvarchar](10) COLLATE Cyrillic_General_CI_AS NOT NULL,
	[dt] [date] NOT NULL,
	[gruname] [nvarchar](50) COLLATE Cyrillic_General_CI_AS NOT NULL,
	[vidsbr] [smallint] NOT NULL,
	[namesbr] [nvarchar](100) COLLATE Cyrillic_General_CI_AS NOT NULL,
	[sm_no_nds] [numeric](18, 2) NOT NULL,
	[sm_nds] [numeric](18, 2) NOT NULL,
	[sm] [numeric](18, 2) NOT NULL,
	[stnds] [numeric](18, 2) NOT NULL,
	[UNI_OTPR] [nvarchar](50) COLLATE Cyrillic_General_CI_AS NOT NULL,
	[nper] [int] NOT NULL,
	[nkrt] [nvarchar](10) COLLATE Cyrillic_General_CI_AS NOT NULL,
	[p_type] [nvarchar](1) COLLATE Cyrillic_General_CI_AS NOT NULL,
	[DtBuhOtchet] [date] NOT NULL,
	[date_obrabot] [datetime] NOT NULL,
	[tdoc] [tinyint] NOT NULL,
	[ORC_ID_ED] [int] NOT NULL,
	[id] [int] NULL,
	[id_kart] [int] NULL,
	[id_otpr] [int] NULL,
	[type_doc] [smallint] NULL,
	[cod_sbor] [smallint] NULL,
	[date_raskr] [date] NULL,
	[num_doc] [nvarchar](10) COLLATE Cyrillic_General_CI_AS NULL,
	[cena] [numeric](18, 2) NULL,
	[kol] [numeric](18, 2) NULL,
	[summa] [numeric](18, 2) NULL,
	[nds] [numeric](18, 2) NULL,
	[textm] [nvarchar](max) COLLATE Cyrillic_General_CI_AS NULL,
	[ID_ED] [int] NULL,
 CONSTRAINT [PK_krt_Naftan_orc_sapod] PRIMARY KEY CLUSTERED 
(
	[keykrt] ASC,
	[keysbor] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* =============================================
Author:	<P. Chizhikov>
Create date:<December 2015>
Description:<Every day update krt_Naftan (through Sql Agent) + add some additional information about scroll list>

Получение инфомрации из ORC + доп. выборки => информация по номеру перечня в krt_Naftan
* Вся инф. из orc_krt
+ кол-во сборов
+ дата начала и окончания сборов по перечню
+ наличие в перечне корректирующих сборов
+ наименование сборов
+ признак обработки
============================================= */
CREATE PROCEDURE [dbo].[sp_UpdateKrt_Naftan] 
AS
BEGIN
-- SET NOCOUNT ON added to prevent extra result sets from  interfering with SELECT statements.
SET NOCOUNT ON;

WITH SOURCE_KRT AS (
SELECT ok.KEYKRT,OK.NKRT,OK.NTREB,CAST(OK.DTTREB AS DATE) AS [DTTREB],CAST(OK.DTOPEN AS DATE) AS [DTOPEN],CAST(OK.DTCLOSE AS DATE) AS [DTCLOSE],
	OK.SMTREB,OK.NDSTREB,CAST(OK.U_KOD AS SMALLINT) AS [U_KOD],CAST(OK.P_TYPE AS NVARCHAR(1)) AS [P_TYPE],OK.DATE_OBRABOT,OK.IN_REAL,
	CAST(ISNULL((SELECT MAX(kn.DTBUHOTCHET) FROM krt_Naftan AS kn),DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0)) AS DATE) AS [BUH],
	CAST((SELECT COUNT(os.KEYKRT) FROM orc_sbor AS os WHERE ok.KEYKRT = OS.KEYKRT) AS INT) AS [RecordCount],
	CAST((SELECT MIN(os.DT) AS  [StartDate] FROM orc_sbor AS os WHERE ok.KEYKRT = OS.KEYKRT) AS DATE)  AS [StartDate],
	CAST((SELECT MAX(os.DT) AS  [StartDate] FROM orc_sbor AS os WHERE ok.KEYKRT = OS.KEYKRT) AS DATE)  AS [EndDate],
	CAST((CASE WHEN (SELECT COUNT(os.KEYKRT) FROM orc_sbor AS os WHERE ok.KEYKRT = OS.KEYKRT AND OS.NKRT LIKE N'S%') > 0 THEN 1 ELSE 0 END) AS BIT) AS  [SignAdjustment_list],
	STUFF((SELECT ',' + CAST (VIDSBR AS NVARCHAR(3)) AS [text()] FROM orc_sbor AS os WHERE ok.KEYKRT = OS.KEYKRT GROUP BY os.VIDSBR ORDER BY os.VIDSBR FOR XML PATH('')),1,1,'') AS [Scroll_sbor] 
FROM dbo.orc_krt AS ok 
WHERE ok.KEYKRT >= 15072000159909 --during and after 2015
)

MERGE INTO [krt_Naftan] AS TGT
USING SOURCE_KRT AS SRC
	ON SRC.KEYKRT =TGT.KEYKRT 
WHEN MATCHED AND (
	TGT.NKRT <> SRC.NKRT OR
	TGT.NTREB <> SRC.NTREB OR
	TGT.DTTREB <> SRC.DTTREB OR
	TGT.DTOPEN <> SRC.DTOPEN OR
	TGT.DTCLOSE <> SRC.DTCLOSE OR
	TGT.SMTREB <> SRC.SMTREB OR
	TGT.NDSTREB <> SRC.NDSTREB OR
	TGT.U_KOD <> SRC.U_KOD OR
	TGT.P_TYPE <> SRC.P_TYPE OR
	datediff(hour,TGT.DATE_OBRABOT, SRC.DATE_OBRABOT) > 1 OR
	TGT.IN_REAL <> SRC.IN_REAL OR
	TGT.[RecordCount] <> SRC.[RecordCount] OR
	TGT.[StartDate_PER] <> SRC.[StartDate] OR
	TGT.[EndDate_PER] <> SRC.[EndDate] OR
	TGT.[SignAdjustment_list] <> SRC.[SignAdjustment_list] OR
	TGT.[Scroll_sbor] <> SRC.[Scroll_sbor]) 
THEN UPDATE SET
	TGT.KEYKRT = SRC.KEYKRT,
	TGT.NKRT = SRC.NKRT,
	TGT.NTREB = SRC.NTREB,
	TGT.DTBUHOTCHET = SRC.BUH,
	TGT.DTTREB = SRC.DTTREB,
	TGT.DTOPEN = SRC.DTOPEN,
	TGT.DTCLOSE = SRC.DTCLOSE,
	TGT.SMTREB = SRC.SMTREB,
	TGT.NDSTREB = SRC.NDSTREB,
	TGT.U_KOD = SRC.U_KOD,
	TGT.P_TYPE =SRC.P_TYPE,
	TGT.DATE_OBRABOT = SRC.DATE_OBRABOT,
	TGT.IN_REAL = SRC.IN_REAL,
	TGT.[RecordCount] = SRC.[RecordCount],
	TGT.[StartDate_PER] = SRC.[StartDate],
	TGT.[EndDate_PER] = SRC.[EndDate],
	TGT.[SignAdjustment_list] = SRC.[SignAdjustment_list],
	TGT.[Scroll_sbor] = SRC.[Scroll_sbor]
WHEN NOT MATCHED BY TARGET THEN
INSERT (KEYKRT,NKRT,NTREB,DTBUHOTCHET,DTOPEN,DTCLOSE,SMTREB,NDSTREB,U_KOD,P_TYPE,DATE_OBRABOT,IN_REAL,[RecordCount],[StartDate_PER],[EndDate_PER],[SignAdjustment_list],[Scroll_sbor])
VALUES (SRC.KEYKRT,SRC.NKRT,SRC.NTREB,SRC.BUH,SRC.DTOPEN,SRC.DTCLOSE,SRC.SMTREB,SRC.NDSTREB,SRC.U_KOD,SRC.P_TYPE,SRC.DATE_OBRABOT,SRC.IN_REAL,SRC.[RecordCount],SRC.[StartDate],SRC.[EndDate],SRC.[SignAdjustment_list],SRC.[Scroll_sbor]);
END
RETURN

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* =============================================
Author:	<P. Chizhikov>
Create date:<December 2015>
Description:<Result talbe compare ORC(orc_krt) and Sapod(v_nach) DB>
*Таблица получение бух. отчетности dbo.krt_Naftan_orc_sapod 
(получение детализированной таблицы по номеру перечня)
============================================= */
CREATE PROCEDURE sp_fill_krt_Naftan_orc_sapod
(@KEYKRT AS BIGINT,	@START_DATE AS DATETIME)
AS
BEGIN
SET NOCOUNT ON;

DECLARE @END_DATE AS DATETIME = DATEADD(DAY,-1,DATEADD(MONTH,1,@START_DATE));
/*ДЛЯ ПРЕДОТВРАЩЕНИЯ МНОЖЕСТВЕННЫХ УДАЛЕННЫХ ЗАПРОСОВ*/
DECLARE @NACH_RESULT TABLE(																		
	NACH_ID INT PRIMARY KEY NOT NULL,ID_KART INT,ID_OTPR INT,ID INT,DATE_RASKR DATE,
	SM  DECIMAL(18,2),CENA DECIMAL(18,2),KOL DECIMAL(18,2),SUMMA DECIMAL(18,2),NDS DECIMAL(18,2),
	NUM_DOC NVARCHAR(8),TYPE_DOC TINYINT,NUM_KART NVARCHAR(8),
	COD_SBOR SMALLINT,TEXTM NVARCHAR(MAX),ID_ED INT
);

INSERT INTO	@NACH_RESULT(NACH_ID,ID_KART,ID_OTPR,ID,DATE_RASKR,SM,CENA,KOL,SUMMA,NDS,NUM_DOC,TYPE_DOC,NUM_KART,COD_SBOR,TEXTM,ID_ED)	
SELECT D.ID AS [NACH_ID],D.ID_KART,D.ID_OTPR,C.ID
	,D.DATE_RASKR
	,CONVERT(DECIMAL(18,2),D.SUMMA + D.NDS) AS [SM]
	,CONVERT(DECIMAL(18,2),D.CENA) AS [CENA]
	,CONVERT(DECIMAL(18,2),D.KOL) AS [KOL]
	,CONVERT(DECIMAL(18,2),D.SUMMA) AS [SUMMA]
	,CONVERT(DECIMAL(18,2),D.NDS) AS [NDS]
	,LTRIM(RTRIM(D.NUM_DOC)) COLLATE SQL_Latin1_General_CP1251_CS_AS AS [NUM_DOC]
	,D.TYPE_DOC
	,LTRIM(RTRIM(C.NUM_KART)) COLLATE SQL_Latin1_General_CP1251_CS_AS AS [NUM_KART]
	,CASE WHEN CHARINDEX(N'.',RTRIM(LTRIM(D.cod_sbor)),1) > 1 THEN CAST(LEFT(D.cod_sbor,CHARINDEX(N'.',RTRIM(LTRIM(D.cod_sbor)),1)-1) AS SMALLINT) ELSE CAST(RTRIM(LTRIM(D.cod_sbor)) AS SMALLINT) END AS [COD_SBOR]
	,CAST(D.[TEXTM] AS NVARCHAR(MAX)),D.ID_ED
FROM [TSC-SRV].[OBD].dbo.v_kart AS C INNER JOIN [TSC-SRV].[OBD].dbo.v_nach AS D 
	ON D.id_kart = C.id	
WHERE (D.DATE_RASKR BETWEEN DATEADD(DAY,-7,@START_DATE) AND DATEADD(DAY,7,@END_DATE)) AND 
	 (D.COD_KL IN (N'3494',N'349402') OR C.COD_PL IN (N'3494',N'349402'));

WITH ORC_RESULT(KEYKRT,KEYSBOR,NPER,P_TYPE,DTBUHOTCHET,NOMOT,DT,GRUNAME,VIDSBR,NAMESBR,SM,SM_NDS,STNDS,DATE_OBRABOT,TDOC,ID_ED,UNI_OTPR,NKRT) 
AS (
	SELECT	A.KEYKRT,B.KEYSBOR,A.NKRT AS [NPER],A.P_TYPE,A.DTBUHOTCHET,
		LTRIM(RTRIM(B.NOMOT)) AS [NOMOT],
	    CONVERT(DATE,B.DT,106) AS [DT],B.GRUNAME,B.VIDSBR,B.NAMESBR,
		CONVERT(DECIMAL(18,2),B.[SM]) AS [SM],
		CONVERT(DECIMAL(18,2),B.[SM_NDS]) AS [SM_NDS],
		CONVERT(DECIMAL(18,2),B.[STNDS]) AS [STNDS],
		(CONVERT(NVARCHAR,B.DATE_OBRABOT,105) + N' ' + (CONVERT(NVARCHAR(5),B.DATE_OBRABOT,108))) AS [DATE_OBRABOT],
		B.TDOC,B.ID_ED,B.UNI_OTPR,
		LTRIM(RTRIM(B.NKRT)) AS [NKRT]
	FROM dbo.[krt_naftan] AS A INNER JOIN dbo.[orc_sbor] AS B
		ON B.KEYKRT = A.KEYKRT 
	WHERE B.KEYKRT = @KEYKRT 
),GROUPING_SELECT ([RANK],NUMBER,KEYKRT,KEYSBOR,[ORC_ID_ED],VIDSBR,[COUNT])
AS(
   	SELECT ROW_NUMBER() OVER(PARTITION BY A.KEYKRT,A.VIDSBR,A.[ORC_ID_ED] ORDER BY A.[ORC_ID_ED]) AS [RANK],
		   ROW_NUMBER() OVER(PARTITION BY A.KEYSBOR ORDER BY A.KEYKRT ) AS [NUMBER],
		   A.KEYKRT,A.KEYSBOR,A.[ORC_ID_ED],A.VIDSBR,
		   COUNT(A.KEYKRT) AS [COUNT]	
	FROM (SELECT A.ID_ED AS [ORC_ID_ED],S.ID_ED AS [NACH_ID_ED],A.VIDSBR,A.KEYKRT,A.KEYSBOR,A.TDOC,S.[NACH_ID]
		  FROM ORC_RESULT AS A LEFT OUTER JOIN (SELECT D.NACH_ID,D.SM,D.cod_sbor,D.NUM_DOC,D.TEXTM,D.ID_ED,D.SUMMA,D.NDS FROM @NACH_RESULT AS D) AS s	
				ON (A.SM = (S.SUMMA + S.NDS) AND S.NUM_DOC = A.NOMOT AND S.COD_SBOR = A.VIDSBR AND S.ID_ED <> A.ID_ED)) AS a	
	GROUP BY A.[ORC_ID_ED],A.VIDSBR,A.KEYKRT,A.KEYSBOR,A.TDOC
)
/**********************************************************************************************************************************************************/
INSERT INTO dbo.krt_Naftan_orc_sapod
SELECT T.KEYKRT,T.KEYSBOR,T.NOMOT,T.[DT],T.GRUNAME,T.VIDSBR,T.NAMESBR,(T.[SM]-T.[SM_NDS]) AS [SM_NO_NDS],T.[SM_NDS],T.[SM],T.[STNDS],T.[UNI_OTPR],
T.[NPER],T.[NKRT],T.[P_TYPE],T.DTBUHOTCHET,T.[DATE_OBRABOT],T.TDOC,T.[ORC_ID_ED],T.[NACH_ID],T.[ID_KART],T.ID_OTPR,T.TYPE_DOC,T.COD_SBOR,
T.[DATE_RASKR],T.NUM_DOC,
T.CENA,T.KOL,T.SUMMA,T.NDS,T.TEXTM,T.ID_ED
FROM (
/*JOIN ON TDOC=0*/																						 
SELECT A.KEYKRT,A.KEYSBOR,A.NOMOT,A.DT,A.GRUNAME,A.VIDSBR,A.NAMESBR,(A.[SM]-A.[SM_NDS]) AS [SM_NO_NDS],A.SM_NDS,A.SM,A.STNDS,A.[UNI_OTPR],A.NPER,
	   A.NKRT,A.P_TYPE,A.DTBUHOTCHET,A.DATE_OBRABOT,A.TDOC,A.ID_ED AS [ORC_ID_ED],
	   NULL AS [NACH_ID],NULL AS ID_KART,NULL AS ID_OTPR,NULL AS TYPE_DOC, NULL AS COD_SBOR,NULL AS DATE_RASKR,
	   NULL AS NUM_DOC,NULL AS CENA,NULL AS KOL,NULL AS SUMMA,NULL AS NDS,NULL AS TEXTM,NULL AS ID_ED
FROM ORC_RESULT AS A 
WHERE A.TDOC = 0

UNION ALL

SELECT A.KEYKRT,A.KEYSBOR,A.NOMOT,A.DT,A.GRUNAME,A.VIDSBR,A.NAMESBR,(A.[SM]-A.[SM_NDS]) AS [SM_NO_NDS],A.SM_NDS,A.SM,A.STNDS,A.[UNI_OTPR],A.NPER,
	   A.NKRT,A.P_TYPE,A.DTBUHOTCHET,A.DATE_OBRABOT,A.TDOC,A.ID_ED AS [ORC_ID_ED],
	   S.[NACH_ID],S.ID_KART,S.ID_OTPR,S.TYPE_DOC,S.COD_SBOR,S.DATE_RASKR,S.NUM_DOC,S.CENA,S.KOL,S.SUMMA,S.NDS,S.TEXTM,S.ID_ED
FROM ORC_RESULT AS A 
	OUTER APPLY(
		SELECT TOP 1 S.[NACH_ID],S.ID_KART,S.ID_OTPR,S.TYPE_DOC,S.COD_SBOR,S.DATE_RASKR,S.NUM_DOC,S.CENA,S.KOL,S.SUMMA,S.NDS,S.TEXTM,S.ID_ED
		FROM  @NACH_RESULT AS S		
			WHERE A.ID_ED = S.ID_ED AND A.VIDSBR = S.COD_SBOR) AS S	     
WHERE A.TDOC IN (1,2,3,4) AND A.KEYSBOR NOT IN (SELECT KEYSBOR FROM GROUPING_SELECT WHERE [RANK] >1) 

UNION ALL

/*JOIN ON OTHER PARAMETERS*/
SELECT Q.KEYKRT,Q.KEYSBOR,Q.NOMOT,Q.DT,Q.GRUNAME,Q.VIDSBR,Q.NAMESBR,(Q.[SM]-Q.[SM_NDS]) AS [SM_NO_NDS],Q.SM_NDS,Q.SM,Q.STNDS,Q.[UNI_OTPR],Q.NPER,
Q.NKRT,Q.P_TYPE,Q.DTBUHOTCHET,Q.DATE_OBRABOT,Q.TDOC,Q.ID_ED AS [ORC_ID_ED],
Q.[NACH_ID],Q.ID_KART,Q.ID_OTPR,Q.TYPE_DOC,Q.COD_SBOR,Q.DATE_RASKR,Q.NUM_DOC,Q.CENA,Q.KOL,Q.SUMMA,Q.NDS,Q.TEXTM,Q.[NACH_ID_ED] AS [ID_ED]
FROM (
	SELECT
	ROW_NUMBER() OVER(PARTITION BY A.KEYKRT,A.KEYSBOR ORDER BY S.[NACH_ID_ED], A.KEYKRT,A.KEYSBOR ) AS [NUMBER],
	(DENSE_RANK() OVER(ORDER BY A.KEYSBOR) % COUNT(A.KEYSBOR) OVER(PARTITION BY A.KEYSBOR)) + 1 AS [OST],
	A.*,S.*
		FROM ORC_RESULT AS A INNER JOIN (
			SELECT D.[NACH_ID],D.ID_KART,D.ID_OTPR,D.TYPE_DOC,D.COD_SBOR,D.DATE_RASKR,D.NUM_DOC,D.CENA,D.KOL,D.SUMMA,D.NDS,D.TEXTM,D.ID_ED AS [NACH_ID_ED]
				FROM @NACH_RESULT AS D) AS S
			ON (A.SM = (S.SUMMA + S.NDS))
				AND	S.NUM_DOC = A.NOMOT 
					AND S.COD_SBOR = A.VIDSBR 					
			WHERE A.KEYSBOR IN (SELECT KEYSBOR FROM GROUPING_SELECT WHERE [RANK] >1) 
		) AS Q  
	WHERE [NUMBER] = [OST]
) AS T
WHERE T.KEYKRT = @KEYKRT
ORDER BY T.KEYKRT,T.KEYSBOR,T.NKRT,T.VIDSBR	--КАРТОЧКА
END
RETURN

GO
ALTER TABLE [dbo].[krt_Naftan] ADD  CONSTRAINT [DF_krt_naftan_Confirmed]  DEFAULT ((0)) FOR [Confirmed]
GO
ALTER TABLE [dbo].[orc_sbor]  WITH CHECK ADD  CONSTRAINT [FK_orc_sbor_orc_krt] FOREIGN KEY([KEYKRT])
REFERENCES [dbo].[orc_krt] ([KEYKRT])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[orc_sbor] CHECK CONSTRAINT [FK_orc_sbor_orc_krt]
GO
ALTER TABLE [dbo].[krt_Naftan_orc_sapod]  WITH CHECK ADD  CONSTRAINT [FK_krt_Naftan_orc_sapod_krt_naftan] FOREIGN KEY([keykrt])
REFERENCES [dbo].[krt_Naftan] ([KEYKRT])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[krt_Naftan_orc_sapod] CHECK CONSTRAINT [FK_krt_Naftan_orc_sapod_krt_naftan]
GO
