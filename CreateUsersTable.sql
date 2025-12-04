-- SQL Script to create Users table for TrySystem
-- Run this script in SQL Server Management Studio (SSMS)
-- Make sure you're connected to the correct database (TrySystemDB)

USE TrySystemDB;
GO

-- Create Users table if it doesn't exist
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Users' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Users (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Username NVARCHAR(100) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(256) NOT NULL,
        DateCreated DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
        LastLogin DATETIME2 NULL
    );
    
    PRINT 'Users table created successfully.';
END
ELSE
BEGIN
    PRINT 'Users table already exists.';
END
GO

-- Optional: Create an index on Username for faster lookups
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Users_Username' AND object_id = OBJECT_ID('dbo.Users'))
BEGIN
    CREATE INDEX IX_Users_Username ON dbo.Users(Username);
    PRINT 'Index on Username created successfully.';
END
ELSE
BEGIN
    PRINT 'Index on Username already exists.';
END
GO

-- Verify the table was created
SELECT 
    TABLE_NAME,
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Users'
ORDER BY ORDINAL_POSITION;
GO

