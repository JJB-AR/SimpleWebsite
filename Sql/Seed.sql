USE SimpleWebsiteDB;
GO

IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = N'admin' OR Email = N'admin@simplewebsite.local')
BEGIN
    INSERT INTO Users (Username, Password, Email, IsAdmin, IsActive, CreatedAt)
    VALUES (N'admin', N'Admin123!', N'admin@simplewebsite.local', 1, 1, GETDATE());
END
GO

IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = N'user' OR Email = N'user@simplewebsite.local')
BEGIN
    INSERT INTO Users (Username, Password, Email, IsAdmin, IsActive, CreatedAt)
    VALUES (N'user', N'User123!', N'user@simplewebsite.local', 0, 1, GETDATE());
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM Content c
    INNER JOIN Users u ON u.UserID = c.USERID
    WHERE u.Username = N'user'
)
BEGIN
    INSERT INTO Content (USERID, EducationalAttainment, HobbiesAndInterests, Skills)
    SELECT UserID, N'College student', N'Programming and web design', N'C#, ASP.NET, SQL'
    FROM Users
    WHERE Username = N'user';
END
GO
