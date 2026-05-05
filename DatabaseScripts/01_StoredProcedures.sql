-- =============================================
-- Author:      TailorApp System
-- Create date: 2026-05-05
-- Description: Stored Procedures for TailorShop Management System
-- =============================================

USE TailorShopDB;
GO

-- ---------------------------------------------
-- USERS PROCEDURES
-- ---------------------------------------------
CREATE OR ALTER PROCEDURE sp_Users_Authenticate
    @LoginName NVARCHAR(100),
    @Password NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT UserID, Name, LoginName, Password, Status
    FROM Users
    WHERE LoginName = @LoginName AND Password = @Password AND Status = 1;
END
GO

-- ---------------------------------------------
-- CUSTOMERS PROCEDURES
-- ---------------------------------------------
CREATE OR ALTER PROCEDURE sp_Customers_GetPaged
    @Search NVARCHAR(100) = NULL,
    @Offset INT,
    @PageSize INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @TotalCount INT;
    
    SELECT @TotalCount = COUNT(*)
    FROM Customer c
    WHERE (@Search IS NULL 
           OR c.CustomerName LIKE '%' + @Search + '%'
           OR c.MobileNo1 LIKE '%' + @Search + '%'
           OR c.MobileNo2 LIKE '%' + @Search + '%'
           OR EXISTS (SELECT 1 FROM Size s WHERE s.Customer_ID = c.CustomerID AND CAST(s.RegisterNo AS VARCHAR) LIKE '%' + @Search + '%'));
           
    SELECT ISNULL(@TotalCount, 0) AS TotalCount;

    SELECT 
        c.CustomerID, c.CustomerName, c.MobileNo1, c.MobileNo2, c.Status,
        (SELECT COUNT(*) FROM Size s WHERE s.Customer_ID = c.CustomerID) AS TotalMeasurements,
        (SELECT STRING_AGG(rn, ', ') FROM (SELECT DISTINCT CAST(RegisterNo AS VARCHAR) AS rn FROM Size s WHERE s.Customer_ID = c.CustomerID) t) AS RegisterNo
    FROM Customer c
    WHERE (@Search IS NULL 
           OR c.CustomerName LIKE '%' + @Search + '%'
           OR c.MobileNo1 LIKE '%' + @Search + '%'
           OR c.MobileNo2 LIKE '%' + @Search + '%'
           OR EXISTS (SELECT 1 FROM Size s WHERE s.Customer_ID = c.CustomerID AND CAST(s.RegisterNo AS VARCHAR) LIKE '%' + @Search + '%'))
    ORDER BY c.CustomerID DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END
GO

CREATE OR ALTER PROCEDURE sp_Customers_GetById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        c.CustomerID, c.CustomerName, c.MobileNo1, c.MobileNo2, c.Status,
        (SELECT COUNT(*) FROM Size s WHERE s.Customer_ID = c.CustomerID) AS TotalMeasurements
    FROM Customer c
    WHERE c.CustomerID = @Id;
END
GO

CREATE OR ALTER PROCEDURE sp_Customers_GetDashboardStats
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        COUNT(*) AS TotalCustomers,
        SUM(CASE WHEN Status = 1 THEN 1 ELSE 0 END) AS ActiveCustomers,
        (SELECT COUNT(*) FROM Size) AS TotalMeasurements
    FROM Customer;

    SELECT TOP 6 
        c.CustomerID, c.CustomerName, c.MobileNo1, c.MobileNo2, c.Status,
        (SELECT COUNT(*) FROM Size s WHERE s.Customer_ID = c.CustomerID) AS TotalMeasurements
    FROM Customer c
    ORDER BY c.CustomerID DESC;
END
GO

CREATE OR ALTER PROCEDURE sp_Customers_Create
    @CustomerName NVARCHAR(100),
    @MobileNo1 NVARCHAR(50) = NULL,
    @MobileNo2 NVARCHAR(50) = NULL,
    @Status BIT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Customer (CustomerName, MobileNo1, MobileNo2, Status)
    VALUES (@CustomerName, @MobileNo1, @MobileNo2, @Status);
    
    SELECT CAST(SCOPE_IDENTITY() AS INT);
END
GO

CREATE OR ALTER PROCEDURE sp_Customers_Update
    @CustomerID INT,
    @CustomerName NVARCHAR(100),
    @MobileNo1 NVARCHAR(50) = NULL,
    @MobileNo2 NVARCHAR(50) = NULL,
    @Status BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Customer 
    SET CustomerName = @CustomerName,
        MobileNo1 = @MobileNo1,
        MobileNo2 = @MobileNo2,
        Status = @Status
    WHERE CustomerID = @CustomerID;
END
GO

CREATE OR ALTER PROCEDURE sp_Customers_Delete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    -- Delete associated sizes first to maintain integrity
    DELETE FROM Size WHERE Customer_ID = @Id;
    -- Delete the customer
    DELETE FROM Customer WHERE CustomerID = @Id;
END
GO

-- ---------------------------------------------
-- SIZES PROCEDURES
-- ---------------------------------------------
CREATE OR ALTER PROCEDURE sp_Sizes_GetByCustomerId
    @CustomerId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT s.*, c.CustomerName
    FROM Size s
    INNER JOIN Customer c ON c.CustomerID = s.Customer_ID
    WHERE s.Customer_ID = @CustomerId
    ORDER BY s.RegisterNo DESC;
END
GO

CREATE OR ALTER PROCEDURE sp_Sizes_GetById
    @SizeId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT s.*, c.CustomerName
    FROM Size s
    INNER JOIN Customer c ON c.CustomerID = s.Customer_ID
    WHERE s.SizeID = @SizeId;
END
GO

CREATE OR ALTER PROCEDURE sp_Sizes_GetNextRegisterNo
    @CustomerId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ISNULL(MAX(RegisterNo), 0) + 1
    FROM Size WHERE Customer_ID = @CustomerId;
END
GO

CREATE OR ALTER PROCEDURE sp_Sizes_Create
    @Customer_ID INT,
    @RegisterNo INT,
    @Lambai NVARCHAR(50),
    @Bazo NVARCHAR(50),
    @BazoType INT,
    @BazoDetail NVARCHAR(MAX) = NULL,
    @Tera NVARCHAR(50),
    @Calar NVARCHAR(50),
    @CalarType INT,
    @CalarDetail NVARCHAR(MAX) = NULL,
    @Chati NVARCHAR(50),
    @Kamar NVARCHAR(50),
    @Ghera NVARCHAR(50),
    @GheraType INT,
    @ShalwarLambai NVARCHAR(50),
    @Pancha NVARCHAR(50),
    @IsDoubleSidePocket BIT,
    @IsFrontPocket BIT,
    @IsShalwarPocket BIT,
    @IsCheckPatiKaj BIT,
    @Pati NVARCHAR(MAX) = NULL,
    @Design NVARCHAR(MAX) = NULL,
    @OtherDetails NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Size (
        Customer_ID, RegisterNo, Lambai, Bazo, BazoType, BazoDetail,
        Tera, Calar, CalarType, CalarDetail, Chati, Kamar,
        Ghera, GheraType, ShalwarLambai, Pancha,
        IsDoubleSidePocket, IsFrontPocket, IsShalwarPocket, IsCheckPatiKaj,
        Pati, Design, OtherDetails
    ) VALUES (
        @Customer_ID, @RegisterNo, @Lambai, @Bazo, @BazoType, @BazoDetail,
        @Tera, @Calar, @CalarType, @CalarDetail, @Chati, @Kamar,
        @Ghera, @GheraType, @ShalwarLambai, @Pancha,
        @IsDoubleSidePocket, @IsFrontPocket, @IsShalwarPocket, @IsCheckPatiKaj,
        @Pati, @Design, @OtherDetails
    );
    SELECT CAST(SCOPE_IDENTITY() AS INT);
END
GO

CREATE OR ALTER PROCEDURE sp_Sizes_Update
    @SizeID INT,
    @Lambai NVARCHAR(50),
    @Bazo NVARCHAR(50),
    @BazoType INT,
    @BazoDetail NVARCHAR(MAX) = NULL,
    @Tera NVARCHAR(50),
    @Calar NVARCHAR(50),
    @CalarType INT,
    @CalarDetail NVARCHAR(MAX) = NULL,
    @Chati NVARCHAR(50),
    @Kamar NVARCHAR(50),
    @Ghera NVARCHAR(50),
    @GheraType INT,
    @ShalwarLambai NVARCHAR(50),
    @Pancha NVARCHAR(50),
    @IsDoubleSidePocket BIT,
    @IsFrontPocket BIT,
    @IsShalwarPocket BIT,
    @IsCheckPatiKaj BIT,
    @Pati NVARCHAR(MAX) = NULL,
    @Design NVARCHAR(MAX) = NULL,
    @OtherDetails NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Size SET
        Lambai = @Lambai, Bazo = @Bazo, BazoType = @BazoType, BazoDetail = @BazoDetail,
        Tera = @Tera, Calar = @Calar, CalarType = @CalarType, CalarDetail = @CalarDetail,
        Chati = @Chati, Kamar = @Kamar, Ghera = @Ghera, GheraType = @GheraType,
        ShalwarLambai = @ShalwarLambai, Pancha = @Pancha,
        IsDoubleSidePocket = @IsDoubleSidePocket, IsFrontPocket = @IsFrontPocket,
        IsShalwarPocket = @IsShalwarPocket, IsCheckPatiKaj = @IsCheckPatiKaj,
        Pati = @Pati, Design = @Design, OtherDetails = @OtherDetails
    WHERE SizeID = @SizeID;
END
GO

CREATE OR ALTER PROCEDURE sp_Sizes_Delete
    @SizeId INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM Size WHERE SizeID = @SizeId;
END
GO
