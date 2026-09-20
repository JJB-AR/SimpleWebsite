-- Creating necessary tables for the database, I need users information also admin sooo...
-- Jonas Balante 
-- 9/20/2026

USE SimpleWebsiteDB;

CREATE TABLE Users (
	UserID INT PRIMARY KEY IDENTITY(1,1), -- Auto-incrementing primary key
	Username NVARCHAR(50) NOT NULL UNIQUE,
	Password NVARCHAR(255) NOT NULL,
	Email NVARCHAR(100) NOT NULL UNIQUE,
	CreatedAt DATETIME DEFAULT GETDATE(), -- Timestamp for when the user was created
	IsAdmin BIT DEFAULT 0,
	IsActive BIT DEFAULT 1 -- Indicates if the user is active or not
);

CREATE TABLE Content (
	USERID INT FOREIGN KEY REFERENCES Users(UserID),
	ContentID INT PRIMARY KEY IDENTITY(1,1), -- Auto-incrementing primary key
	--Eductional Attainment, Hobbies and Interests, and Skills
	EducationalAttainment NVARCHAR(255),
	HobbiesAndInterests NVARCHAR(255),
	Skills NVARCHAR(255)
);