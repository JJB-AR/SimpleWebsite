-- Simple stored procedures for user management



IF OBJECT_ID('GetUserByUsername','P') IS NOT NULL
    DROP PROCEDURE GetUserByUsername;
GO
--------
CREATE PROCEDURE GetUserByUsername
    @Username NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT UserID, Username, Email, CreatedAt, IsAdmin
    FROM Users
    WHERE Username = @Username;
END
GO 

-------

IF OBJECT_ID('GetAllUsers','P') IS NOT NULL
    DROP PROCEDURE GetAllUsers;
GO
--------

CREATE PROCEDURE GetAllUsers
AS
BEGIN
    SET NOCOUNT ON;
    SELECT UserID, Username, Email, CreatedAt, IsAdmin
    FROM Users;
END
GO
-------

IF OBJECT_ID('usp_RegisterUser','P') IS NOT NULL
    DROP PROCEDURE usp_RegisterUser;
GO
-------
CREATE PROCEDURE usp_RegisterUser
    @Username NVARCHAR(50),
    @Email NVARCHAR(256),
    @Password NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM Users WHERE Username = @Username OR Email = @Email)
    BEGIN
        -- indicate conflict
        SELECT -1 AS NewUserId;
        RETURN;
    END

    INSERT INTO Users (Username, Email, Password, IsActive, CreatedAt)
    VALUES (@Username, @Email, @Password, 1, SYSUTCDATETIME());

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS NewUserId;
END
GO
-------
IF OBJECT_ID('usp_AuthenticateUser','P') IS NOT NULL
    DROP PROCEDURE usp_AuthenticateUser;
GO
-------
CREATE PROCEDURE usp_AuthenticateUser
    @UsernameOrEmail NVARCHAR(256),
    @Password NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT UserID, Username, Email, IsAdmin, IsActive
    FROM Users
    WHERE (Username = @UsernameOrEmail OR Email = @UsernameOrEmail)
      AND Password = @Password;
END
GO
-------
IF OBJECT_ID('usp_SetUserActive','P') IS NOT NULL
    DROP PROCEDURE usp_SetUserActive;
GO
-------
CREATE PROCEDURE usp_SetUserActive
    @UserId INT,
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Users
    SET IsActive = @IsActive
    WHERE UserID = @UserId;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO
