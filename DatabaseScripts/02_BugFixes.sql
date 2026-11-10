-- =============================================
-- Author:      TailorApp System
-- Create date: 2026-10-10
-- Description: Bug-fix changes for the Size table. Safe to run more than once.
--              Run this file, then re-run 01_StoredProcedures.sql.
-- =============================================

USE TailorShopDB;
GO

-- ---------------------------------------------
-- 1. MEASUREMENT COLUMNS: VARCHAR -> NVARCHAR
--    VARCHAR stored Urdu text and Urdu digits as '?'. Lengths stay the same.
-- ---------------------------------------------
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Size') AND name = 'Lambai' AND system_type_id = TYPE_ID('varchar'))
    ALTER TABLE dbo.Size ALTER COLUMN Lambai NVARCHAR(10) NOT NULL;
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Size') AND name = 'Bazo' AND system_type_id = TYPE_ID('varchar'))
    ALTER TABLE dbo.Size ALTER COLUMN Bazo NVARCHAR(10) NOT NULL;
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Size') AND name = 'Tera' AND system_type_id = TYPE_ID('varchar'))
    ALTER TABLE dbo.Size ALTER COLUMN Tera NVARCHAR(10) NOT NULL;
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Size') AND name = 'Calar' AND system_type_id = TYPE_ID('varchar'))
    ALTER TABLE dbo.Size ALTER COLUMN Calar NVARCHAR(10) NOT NULL;
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Size') AND name = 'Chati' AND system_type_id = TYPE_ID('varchar'))
    ALTER TABLE dbo.Size ALTER COLUMN Chati NVARCHAR(10) NOT NULL;
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Size') AND name = 'Kamar' AND system_type_id = TYPE_ID('varchar'))
    ALTER TABLE dbo.Size ALTER COLUMN Kamar NVARCHAR(10) NOT NULL;
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Size') AND name = 'Ghera' AND system_type_id = TYPE_ID('varchar'))
    ALTER TABLE dbo.Size ALTER COLUMN Ghera NVARCHAR(10) NOT NULL;
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Size') AND name = 'ShalwarLambai' AND system_type_id = TYPE_ID('varchar'))
    ALTER TABLE dbo.Size ALTER COLUMN ShalwarLambai NVARCHAR(10) NOT NULL;
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Size') AND name = 'Pancha' AND system_type_id = TYPE_ID('varchar'))
    ALTER TABLE dbo.Size ALTER COLUMN Pancha NVARCHAR(50) NOT NULL;
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Size') AND name = 'Pati' AND system_type_id = TYPE_ID('varchar'))
    ALTER TABLE dbo.Size ALTER COLUMN Pati NVARCHAR(10) NULL;
GO

-- ---------------------------------------------
-- 2. DOUBLE-STITCH TAG SPELLING
--    The form saved this tag with a wrong first letter (U+062F instead of U+0688),
--    so the slip never ticked it. Letters are written as NCHAR codes so the
--    encoding of this file cannot change them.
-- ---------------------------------------------
DECLARE @Salai NVARCHAR(10) = N' ' + NCHAR(0x0633) + NCHAR(0x0644) + NCHAR(0x0627) + NCHAR(0x0626) + NCHAR(0x06CC);
DECLARE @Wrong NVARCHAR(20) = NCHAR(0x062F) + NCHAR(0x0628) + NCHAR(0x0644) + @Salai;
DECLARE @Right NVARCHAR(20) = NCHAR(0x0688) + NCHAR(0x0628) + NCHAR(0x0644) + @Salai;

UPDATE dbo.Size
SET OtherDetails = REPLACE(OtherDetails COLLATE Latin1_General_BIN2, @Wrong, @Right)
WHERE OtherDetails COLLATE Latin1_General_BIN2 LIKE N'%' + @Wrong + N'%';

UPDATE dbo.Size
SET Design = REPLACE(Design COLLATE Latin1_General_BIN2, @Wrong, @Right)
WHERE Design COLLATE Latin1_General_BIN2 LIKE N'%' + @Wrong + N'%';
GO
