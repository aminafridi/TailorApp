-- Speeds up the register-number search and the duplicate check.
-- Not UNIQUE: some old records share a number from before the rule, and the
-- stored procedures now refuse new duplicates. Safe to run more than once.
USE TailorShopDB;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Size_RegisterNo' AND object_id = OBJECT_ID('dbo.Size'))
    CREATE NONCLUSTERED INDEX IX_Size_RegisterNo ON dbo.Size (RegisterNo);
GO
