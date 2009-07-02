/****** Object:  UserDefinedDataType [dbo].[id]    Script Date: 05/07/2007 19:44:24 ******/
IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N'id' AND ss.name = N'dbo')
CREATE TYPE [dbo].[id] FROM [varchar](11) NOT NULL
/****** Object:  UserDefinedDataType [dbo].[tid]    Script Date: 05/07/2007 19:44:24 ******/
IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N'tid' AND ss.name = N'dbo')
CREATE TYPE [dbo].[tid] FROM [varchar](6) NOT NULL
/****** Object:  UserDefinedDataType [dbo].[empid]    Script Date: 05/07/2007 19:44:24 ******/
IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N'empid' AND ss.name = N'dbo')
CREATE TYPE [dbo].[empid] FROM [char](9) NOT NULL
/****** Object:  Table [dbo].[publishers]    Script Date: 05/07/2007 19:44:24 ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[publishers]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[publishers](
	[pub_id] [char](4) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[pub_name] [varchar](40) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[city] [varchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[state] [char](2) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[country] [varchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NULL DEFAULT ('USA'),
 CONSTRAINT [UPKCL_pubind] PRIMARY KEY CLUSTERED 
(
	[pub_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
/****** Object:  Table [dbo].[stores]    Script Date: 05/07/2007 19:44:24 ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[stores]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[stores](
	[stor_id] [char](4) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[stor_name] [varchar](40) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[stor_address] [varchar](40) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[city] [varchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[state] [char](2) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[zip] [char](5) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [UPK_storeid] PRIMARY KEY CLUSTERED 
(
	[stor_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
/****** Object:  Table [dbo].[jobs]    Script Date: 05/07/2007 19:44:24 ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[jobs]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[jobs](
	[job_id] [smallint] IDENTITY(1,1) NOT NULL,
	[job_desc] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL DEFAULT ('New Position - title not formalized yet'),
	[min_lvl] [tinyint] NOT NULL,
	[max_lvl] [tinyint] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[job_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
/****** Object:  Table [dbo].[pub_info]    Script Date: 05/07/2007 19:44:24 ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[pub_info]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[pub_info](
	[pub_id] [char](4) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[logo] [image] NULL,
	[pr_info] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [UPKCL_pubinfo] PRIMARY KEY CLUSTERED 
(
	[pub_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
/****** Object:  Table [dbo].[discounts]    Script Date: 05/07/2007 19:44:24 ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[discounts]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[discounts](
	[discounttype] [varchar](40) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[stor_id] [char](4) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[lowqty] [smallint] NULL,
	[highqty] [smallint] NULL,
	[discount] [decimal](4, 2) NOT NULL
) ON [PRIMARY]
END
/****** Object:  Table [dbo].[titleauthor]    Script Date: 05/07/2007 19:44:24 ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[titleauthor]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[titleauthor](
	[au_id] [dbo].[id] NOT NULL,
	[title_id] [dbo].[tid] NOT NULL,
	[au_ord] [tinyint] NULL,
	[royaltyper] [int] NULL,
 CONSTRAINT [UPKCL_taind] PRIMARY KEY CLUSTERED 
(
	[au_id] ASC,
	[title_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
/****** Object:  Table [dbo].[authors]    Script Date: 05/07/2007 19:44:24 ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[authors]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[authors](
	[au_id] [dbo].[id] NOT NULL,
	[au_lname] [varchar](40) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[au_fname] [varchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[phone] [char](12) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL DEFAULT ('UNKNOWN'),
	[address] [varchar](40) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[city] [varchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[state] [char](2) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[zip] [char](5) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[contract] [bit] NOT NULL,
 CONSTRAINT [UPKCL_auidind] PRIMARY KEY CLUSTERED 
(
	[au_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
/****** Object:  Table [dbo].[titles]    Script Date: 05/07/2007 19:44:24 ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[titles]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[titles](
	[title_id] [dbo].[tid] NOT NULL,
	[title] [varchar](80) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[type] [char](12) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL DEFAULT ('UNDECIDED'),
	[pub_id] [char](4) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[price] [money] NULL,
	[advance] [money] NULL,
	[royalty] [int] NULL,
	[ytd_sales] [int] NULL,
	[notes] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[pubdate] [datetime] NOT NULL DEFAULT (getdate()),
 CONSTRAINT [UPKCL_titleidind] PRIMARY KEY CLUSTERED 
(
	[title_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
/****** Object:  Table [dbo].[roysched]    Script Date: 05/07/2007 19:44:24 ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[roysched]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[roysched](
	[title_id] [dbo].[tid] NOT NULL,
	[lorange] [int] NULL,
	[hirange] [int] NULL,
	[royalty] [int] NULL
) ON [PRIMARY]
END
/****** Object:  Table [dbo].[sales]    Script Date: 05/07/2007 19:44:24 ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sales]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[sales](
	[stor_id] [char](4) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[ord_num] [varchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[ord_date] [datetime] NOT NULL,
	[qty] [smallint] NOT NULL,
	[payterms] [varchar](12) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[title_id] [dbo].[tid] NOT NULL,
 CONSTRAINT [UPKCL_sales] PRIMARY KEY CLUSTERED 
(
	[stor_id] ASC,
	[ord_num] ASC,
	[title_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
/****** Object:  Table [dbo].[employee]    Script Date: 05/07/2007 19:44:24 ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[employee]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[employee](
	[emp_id] [dbo].[empid] NOT NULL,
	[fname] [varchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[minit] [char](1) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[lname] [varchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[job_id] [smallint] NOT NULL DEFAULT ((1)),
	[job_lvl] [tinyint] NULL DEFAULT ((10)),
	[pub_id] [char](4) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL DEFAULT ('9952'),
	[hire_date] [datetime] NOT NULL DEFAULT (getdate()),
 CONSTRAINT [PK_emp_id] PRIMARY KEY NONCLUSTERED 
(
	[emp_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END

/****** Object:  Index [employee_ind]    Script Date: 05/07/2007 19:44:24 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[employee]') AND name = N'employee_ind')
CREATE CLUSTERED INDEX [employee_ind] ON [dbo].[employee] 
(
	[lname] ASC,
	[fname] ASC,
	[minit] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
/****** Object:  View [dbo].[titleview]    Script Date: 05/07/2007 19:44:24 ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER OFF
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[titleview]'))
EXEC dbo.sp_executesql @statement = N'
CREATE VIEW titleview
AS
select title, au_ord, au_lname, price, ytd_sales, pub_id
from authors, titles, titleauthor
where authors.au_id = titleauthor.au_id
   AND titles.title_id = titleauthor.title_id

' 
/****** Object:  StoredProcedure [dbo].[byroyalty]    Script Date: 05/07/2007 19:44:24 ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER OFF
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[byroyalty]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE byroyalty @percentage int
AS
select au_id from titleauthor
where titleauthor.royaltyper = @percentage

' 
END
/****** Object:  StoredProcedure [dbo].[reptq2]    Script Date: 05/07/2007 19:44:24 ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER OFF
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[reptq2]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE reptq2 AS
select type, pub_id, titles.title_id, au_ord,
   Name = substring (au_lname, 1,15), ytd_sales
from titles, authors, titleauthor
where titles.title_id = titleauthor.title_id AND authors.au_id = titleauthor.au_id
   AND pub_id is NOT NULL
order by pub_id, type
COMPUTE avg(ytd_sales) BY pub_id, type
COMPUTE avg(ytd_sales) BY pub_id

' 
END
/****** Object:  StoredProcedure [dbo].[reptq3]    Script Date: 05/07/2007 19:44:24 ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER OFF
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[reptq3]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE reptq3 @lolimit money, @hilimit money,
@type char(12)
AS
select pub_id, type, title_id, price
from titles
where price >@lolimit AND price <@hilimit AND type = @type OR type LIKE ''%cook%''
order by pub_id, type
COMPUTE count(title_id) BY pub_id, type

' 
END
/****** Object:  StoredProcedure [dbo].[reptq1]    Script Date: 05/07/2007 19:44:24 ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER OFF
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[reptq1]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE reptq1 AS
select pub_id, title_id, price, pubdate
from titles
where price is NOT NULL
order by pub_id
COMPUTE avg(price) BY pub_id
COMPUTE avg(price)

' 
END
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[dbo].[CK__publisher__pub_i__1DE57479]') AND parent_object_id = OBJECT_ID(N'[dbo].[publishers]'))
ALTER TABLE [dbo].[publishers]  WITH CHECK ADD CHECK  (([pub_id]='1756' OR ([pub_id]='1622' OR ([pub_id]='0877' OR ([pub_id]='0736' OR [pub_id]='1389'))) OR [pub_id] like '99[0-9][0-9]'))
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[dbo].[CK__publisher__pub_i__2D27B809]') AND parent_object_id = OBJECT_ID(N'[dbo].[publishers]'))
ALTER TABLE [dbo].[publishers]  WITH CHECK ADD CHECK  (([pub_id]='1756' OR ([pub_id]='1622' OR ([pub_id]='0877' OR ([pub_id]='0736' OR [pub_id]='1389'))) OR [pub_id] like '99[0-9][0-9]'))
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[dbo].[CK__jobs__max_lvl__1BFD2C07]') AND parent_object_id = OBJECT_ID(N'[dbo].[jobs]'))
ALTER TABLE [dbo].[jobs]  WITH CHECK ADD CHECK  (([max_lvl]<=(250)))
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[dbo].[CK__jobs__max_lvl__2B3F6F97]') AND parent_object_id = OBJECT_ID(N'[dbo].[jobs]'))
ALTER TABLE [dbo].[jobs]  WITH CHECK ADD CHECK  (([max_lvl]<=(250)))
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[dbo].[CK__jobs__min_lvl__1CF15040]') AND parent_object_id = OBJECT_ID(N'[dbo].[jobs]'))
ALTER TABLE [dbo].[jobs]  WITH CHECK ADD CHECK  (([min_lvl]>=(10)))
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[dbo].[CK__jobs__min_lvl__2C3393D0]') AND parent_object_id = OBJECT_ID(N'[dbo].[jobs]'))
ALTER TABLE [dbo].[jobs]  WITH CHECK ADD CHECK  (([min_lvl]>=(10)))
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__pub_info__pub_id__1FCDBCEB]') AND parent_object_id = OBJECT_ID(N'[dbo].[pub_info]'))
ALTER TABLE [dbo].[pub_info]  WITH CHECK ADD FOREIGN KEY([pub_id])
REFERENCES [publishers] ([pub_id])
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__pub_info__pub_id__2F10007B]') AND parent_object_id = OBJECT_ID(N'[dbo].[pub_info]'))
ALTER TABLE [dbo].[pub_info]  WITH CHECK ADD FOREIGN KEY([pub_id])
REFERENCES [publishers] ([pub_id])
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__discounts__stor___1ED998B2]') AND parent_object_id = OBJECT_ID(N'[dbo].[discounts]'))
ALTER TABLE [dbo].[discounts]  WITH CHECK ADD FOREIGN KEY([stor_id])
REFERENCES [stores] ([stor_id])
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__discounts__stor___2E1BDC42]') AND parent_object_id = OBJECT_ID(N'[dbo].[discounts]'))
ALTER TABLE [dbo].[discounts]  WITH CHECK ADD FOREIGN KEY([stor_id])
REFERENCES [stores] ([stor_id])
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__titleauth__au_id__20C1E124]') AND parent_object_id = OBJECT_ID(N'[dbo].[titleauthor]'))
ALTER TABLE [dbo].[titleauthor]  WITH CHECK ADD FOREIGN KEY([au_id])
REFERENCES [authors] ([au_id])
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__titleauth__au_id__300424B4]') AND parent_object_id = OBJECT_ID(N'[dbo].[titleauthor]'))
ALTER TABLE [dbo].[titleauthor]  WITH CHECK ADD FOREIGN KEY([au_id])
REFERENCES [authors] ([au_id])
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__titleauth__title__21B6055D]') AND parent_object_id = OBJECT_ID(N'[dbo].[titleauthor]'))
ALTER TABLE [dbo].[titleauthor]  WITH CHECK ADD FOREIGN KEY([title_id])
REFERENCES [titles] ([title_id])
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__titleauth__title__30F848ED]') AND parent_object_id = OBJECT_ID(N'[dbo].[titleauthor]'))
ALTER TABLE [dbo].[titleauthor]  WITH CHECK ADD FOREIGN KEY([title_id])
REFERENCES [titles] ([title_id])
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[dbo].[CK__authors__au_id__22AA2996]') AND parent_object_id = OBJECT_ID(N'[dbo].[authors]'))
ALTER TABLE [dbo].[authors]  WITH CHECK ADD CHECK  (([au_id] like '[0-9][0-9][0-9]-[0-9][0-9]-[0-9][0-9][0-9][0-9]'))
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[dbo].[CK__authors__au_id__31EC6D26]') AND parent_object_id = OBJECT_ID(N'[dbo].[authors]'))
ALTER TABLE [dbo].[authors]  WITH CHECK ADD CHECK  (([au_id] like '[0-9][0-9][0-9]-[0-9][0-9]-[0-9][0-9][0-9][0-9]'))
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[dbo].[CK__authors__zip__239E4DCF]') AND parent_object_id = OBJECT_ID(N'[dbo].[authors]'))
ALTER TABLE [dbo].[authors]  WITH CHECK ADD CHECK  (([zip] like '[0-9][0-9][0-9][0-9][0-9]'))
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[dbo].[CK__authors__zip__32E0915F]') AND parent_object_id = OBJECT_ID(N'[dbo].[authors]'))
ALTER TABLE [dbo].[authors]  WITH CHECK ADD CHECK  (([zip] like '[0-9][0-9][0-9][0-9][0-9]'))
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__titles__pub_id__24927208]') AND parent_object_id = OBJECT_ID(N'[dbo].[titles]'))
ALTER TABLE [dbo].[titles]  WITH CHECK ADD FOREIGN KEY([pub_id])
REFERENCES [publishers] ([pub_id])
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__titles__pub_id__33D4B598]') AND parent_object_id = OBJECT_ID(N'[dbo].[titles]'))
ALTER TABLE [dbo].[titles]  WITH CHECK ADD FOREIGN KEY([pub_id])
REFERENCES [publishers] ([pub_id])
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__roysched__title___25869641]') AND parent_object_id = OBJECT_ID(N'[dbo].[roysched]'))
ALTER TABLE [dbo].[roysched]  WITH CHECK ADD FOREIGN KEY([title_id])
REFERENCES [titles] ([title_id])
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__roysched__title___34C8D9D1]') AND parent_object_id = OBJECT_ID(N'[dbo].[roysched]'))
ALTER TABLE [dbo].[roysched]  WITH CHECK ADD FOREIGN KEY([title_id])
REFERENCES [titles] ([title_id])
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__sales__stor_id__267ABA7A]') AND parent_object_id = OBJECT_ID(N'[dbo].[sales]'))
ALTER TABLE [dbo].[sales]  WITH CHECK ADD FOREIGN KEY([stor_id])
REFERENCES [stores] ([stor_id])
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__sales__stor_id__35BCFE0A]') AND parent_object_id = OBJECT_ID(N'[dbo].[sales]'))
ALTER TABLE [dbo].[sales]  WITH CHECK ADD FOREIGN KEY([stor_id])
REFERENCES [stores] ([stor_id])
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__sales__title_id__276EDEB3]') AND parent_object_id = OBJECT_ID(N'[dbo].[sales]'))
ALTER TABLE [dbo].[sales]  WITH CHECK ADD FOREIGN KEY([title_id])
REFERENCES [titles] ([title_id])
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__sales__title_id__36B12243]') AND parent_object_id = OBJECT_ID(N'[dbo].[sales]'))
ALTER TABLE [dbo].[sales]  WITH CHECK ADD FOREIGN KEY([title_id])
REFERENCES [titles] ([title_id])
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__employee__job_id__286302EC]') AND parent_object_id = OBJECT_ID(N'[dbo].[employee]'))
ALTER TABLE [dbo].[employee]  WITH CHECK ADD FOREIGN KEY([job_id])
REFERENCES [jobs] ([job_id])
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__employee__job_id__37A5467C]') AND parent_object_id = OBJECT_ID(N'[dbo].[employee]'))
ALTER TABLE [dbo].[employee]  WITH CHECK ADD FOREIGN KEY([job_id])
REFERENCES [jobs] ([job_id])
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__employee__pub_id__29572725]') AND parent_object_id = OBJECT_ID(N'[dbo].[employee]'))
ALTER TABLE [dbo].[employee]  WITH CHECK ADD FOREIGN KEY([pub_id])
REFERENCES [publishers] ([pub_id])
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__employee__pub_id__38996AB5]') AND parent_object_id = OBJECT_ID(N'[dbo].[employee]'))
ALTER TABLE [dbo].[employee]  WITH CHECK ADD FOREIGN KEY([pub_id])
REFERENCES [publishers] ([pub_id])
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[dbo].[CK_emp_id]') AND parent_object_id = OBJECT_ID(N'[dbo].[employee]'))
ALTER TABLE [dbo].[employee]  WITH CHECK ADD  CONSTRAINT [CK_emp_id] CHECK  (([emp_id] like '[A-Z][A-Z][A-Z][1-9][0-9][0-9][0-9][0-9][FM]' OR [emp_id] like '[A-Z]-[A-Z][1-9][0-9][0-9][0-9][0-9][FM]'))
ALTER TABLE [dbo].[employee] CHECK CONSTRAINT [CK_emp_id]


