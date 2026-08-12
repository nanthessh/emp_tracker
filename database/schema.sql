-- =============================================
-- EmpTracker Database Schema & Stored Procedures
-- =============================================

USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'EmpTrackerDB')
    CREATE DATABASE EmpTrackerDB;
GO

USE EmpTrackerDB;
GO


USE EmpTrackerDB;
SELECT UserId, Name, Email, Role, 
       LEFT(PasswordHash, 20) AS HashPreview 
FROM Users;

DELETE FROM Users WHERE Name = 'Nanthessh' AND Role = 'Employee';

-- Add correct employees
INSERT INTO Users (Name, Email, PasswordHash, Role)
VALUES 
('John Employee', 'john@emptracker.com', '$2a$11$zCDEcGFj9g4VvxB70NDSlOclU5bfb.5JKTVOVuuO8veuOTs7VxmEC', 'Employee'),
('Jane Smith', 'jane@emptracker.com', '$2a$11$zCDEcGFj9g4VvxB70NDSlOclU5bfb.5JKTVOVuuO8veuOTs7VxmEC', 'Employee'),
('Kumar Raja', 'kumar@emptracker.com', '$2a$11$zCDEcGFj9g4VvxB70NDSlOclU5bfb.5JKTVOVuuO8veuOTs7VxmEC', 'Employee');

SELECT * FROM Users;


-- =============================================
-- Tables
-- =============================================

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
CREATE TABLE Users (
    UserId       INT IDENTITY(1,1) PRIMARY KEY,
    Name         NVARCHAR(100)  NOT NULL,
    Email        NVARCHAR(150)  NOT NULL UNIQUE,
    PasswordHash NVARCHAR(256)  NOT NULL,
    Role         NVARCHAR(20)   NOT NULL DEFAULT 'Employee'  -- Admin | Employee
);
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Tasks' AND xtype='U')
CREATE TABLE Tasks (
    TaskId      INT IDENTITY(1,1) PRIMARY KEY,
    Title       NVARCHAR(200)  NOT NULL,
    Description NVARCHAR(1000) NULL,
    AssignedTo  INT            NOT NULL REFERENCES Users(UserId),
    Priority    NVARCHAR(20)   NOT NULL DEFAULT 'Medium',  -- Low | Medium | High
    Status      NVARCHAR(20)   NOT NULL DEFAULT 'Pending', -- Pending | InProgress | Completed
    DueDate     DATETIME       NOT NULL,
    CreatedAt   DATETIME       NOT NULL DEFAULT GETDATE()
);
GO

-- =============================================
-- Seed: Default Admin
-- Password: Admin@123  (BCrypt hash)
-- =============================================


-- Fix Admin password hash
DELETE FROM Users WHERE Email = 'admin@emptracker.com';
INSERT INTO Users (Name, Email, PasswordHash, Role)
VALUES ('Administrator', 'admin@emptracker.com',
        '$2a$11$zCDEcGFj9g4VvxB70NDSlOclU5bfb.5JKTVOVuuO8veuOTs7VxmEC', 'Admin');

-- Add Employee users for task assignment
INSERT INTO Users (Name, Email, PasswordHash, Role)
VALUES 
('John Employee', 'john@emptracker.com',
 '$2a$11$zCDEcGFj9g4VvxB70NDSlOclU5bfb.5JKTVOVuuO8veuOTs7VxmEC', 'Employee'),
('Jane Smith', 'jane@emptracker.com',
 '$2a$11$zCDEcGFj9g4VvxB70NDSlOclU5bfb.5JKTVOVuuO8veuOTs7VxmEC', 'Employee'),
('Kumar Raja', 'kumar@emptracker.com',
 '$2a$11$zCDEcGFj9g4VvxB70NDSlOclU5bfb.5JKTVOVuuO8veuOTs7VxmEC', 'Employee');

-- Verify
SELECT * FROM Users;

 

select * from Users

EXEC sp_GetAllEmployees;

SELECT * FROM Users ORDER BY UserId;

DELETE FROM Users WHERE Name = 'Nanthessh' AND Role = 'Employee';


SELECT UserId, Name, Role FROM Users WHERE Role = 'Employee';

-- =============================================
-- Stored Procedures: Auth
-- =============================================

CREATE OR ALTER PROCEDURE sp_GetUserByEmail
    @Email NVARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT UserId, Name, Email, PasswordHash, Role FROM Users WHERE Email = @Email;
END
GO

-- =============================================
-- Stored Procedures: Users
-- =============================================

SELECT DB_NAME();
exec sp_GetAllEmployees

DROP PROCEDURE IF EXISTS GetAllEmployees;


CREATE OR ALTER PROCEDURE sp_GetAllEmployees
AS
BEGIN
    SET NOCOUNT ON;
    SELECT UserId, Name, Email, Role FROM Users WHERE Role = 'Employee';
END
GO

CREATE OR ALTER PROCEDURE sp_CreateUser
    @Name         NVARCHAR(100),
    @Email        NVARCHAR(150),
    @PasswordHash NVARCHAR(256),
    @Role         NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Users (Name, Email, PasswordHash, Role)
    VALUES (@Name, @Email, @PasswordHash, @Role);
    SELECT SCOPE_IDENTITY() AS UserId;
END
GO

-- =============================================
-- Stored Procedures: Tasks
-- =============================================

CREATE OR ALTER PROCEDURE sp_GetAllTasks
    @Search   NVARCHAR(200) = NULL,
    @Status   NVARCHAR(20)  = NULL,
    @Priority NVARCHAR(20)  = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT t.TaskId, t.Title, t.Description, t.AssignedTo,
           u.Name AS AssigneeName, t.Priority, t.Status, t.DueDate, t.CreatedAt
    FROM Tasks t
    INNER JOIN Users u ON t.AssignedTo = u.UserId
    WHERE (@Search   IS NULL OR t.Title    LIKE '%' + @Search   + '%')
      AND (@Status   IS NULL OR t.Status   = @Status)
      AND (@Priority IS NULL OR t.Priority = @Priority)
    ORDER BY t.CreatedAt DESC;
END
GO

CREATE OR ALTER PROCEDURE sp_GetTasksByUser
    @UserId   INT,
    @Search   NVARCHAR(200) = NULL,
    @Status   NVARCHAR(20)  = NULL,
    @Priority NVARCHAR(20)  = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT t.TaskId, t.Title, t.Description, t.AssignedTo,
           u.Name AS AssigneeName, t.Priority, t.Status, t.DueDate, t.CreatedAt
    FROM Tasks t
    INNER JOIN Users u ON t.AssignedTo = u.UserId
    WHERE t.AssignedTo = @UserId
      AND (@Search   IS NULL OR t.Title    LIKE '%' + @Search   + '%')
      AND (@Status   IS NULL OR t.Status   = @Status)
      AND (@Priority IS NULL OR t.Priority = @Priority)
    ORDER BY t.CreatedAt DESC;
END
GO

CREATE OR ALTER PROCEDURE sp_GetTaskById
    @TaskId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT t.TaskId, t.Title, t.Description, t.AssignedTo,
           u.Name AS AssigneeName, t.Priority, t.Status, t.DueDate, t.CreatedAt
    FROM Tasks t
    INNER JOIN Users u ON t.AssignedTo = u.UserId
    WHERE t.TaskId = @TaskId;
END
GO

CREATE OR ALTER PROCEDURE sp_CreateTask
    @Title       NVARCHAR(200),
    @Description NVARCHAR(1000),
    @AssignedTo  INT,
    @Priority    NVARCHAR(20),
    @Status      NVARCHAR(20),
    @DueDate     DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Tasks (Title, Description, AssignedTo, Priority, Status, DueDate)
    VALUES (@Title, @Description, @AssignedTo, @Priority, @Status, @DueDate);
    SELECT SCOPE_IDENTITY() AS TaskId;
END
GO

CREATE OR ALTER PROCEDURE sp_UpdateTask
    @TaskId      INT,
    @Title       NVARCHAR(200),
    @Description NVARCHAR(1000),
    @AssignedTo  INT,
    @Priority    NVARCHAR(20),
    @Status      NVARCHAR(20),
    @DueDate     DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Tasks
    SET Title = @Title, Description = @Description, AssignedTo = @AssignedTo,
        Priority = @Priority, Status = @Status, DueDate = @DueDate
    WHERE TaskId = @TaskId;
END
GO

CREATE OR ALTER PROCEDURE sp_UpdateTaskStatus
    @TaskId INT,
    @Status NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Tasks SET Status = @Status WHERE TaskId = @TaskId;
END
GO

CREATE OR ALTER PROCEDURE sp_DeleteTask
    @TaskId INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM Tasks WHERE TaskId = @TaskId;
END
GO

-- =============================================
-- Stored Procedure: Dashboard Stats
-- =============================================

CREATE OR ALTER PROCEDURE sp_GetDashboardStats
    @UserId INT = NULL,   -- NULL = Admin (all tasks)
    @Role   NVARCHAR(20) = 'Admin'
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        COUNT(*)                                          AS TotalTasks,
        SUM(CASE WHEN Status = 'Pending'    THEN 1 ELSE 0 END) AS PendingTasks,
        SUM(CASE WHEN Status = 'InProgress' THEN 1 ELSE 0 END) AS InProgressTasks,
        SUM(CASE WHEN Status = 'Completed'  THEN 1 ELSE 0 END) AS CompletedTasks,
        SUM(CASE WHEN Priority = 'High'     THEN 1 ELSE 0 END) AS HighPriorityTasks
    FROM Tasks
    WHERE (@Role = 'Admin' OR AssignedTo = @UserId);
END
GO

CREATE OR ALTER PROCEDURE sp_GetRecentTasks
    @UserId INT = NULL,
    @Role   NVARCHAR(20) = 'Admin',
    @Top    INT = 5
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP (@Top)
           t.TaskId, t.Title, t.Priority, t.Status, t.DueDate,
           u.Name AS AssigneeName
    FROM Tasks t
    INNER JOIN Users u ON t.AssignedTo = u.UserId
    WHERE (@Role = 'Admin' OR t.AssignedTo = @UserId)
    ORDER BY t.CreatedAt DESC;
END
GO
