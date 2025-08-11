--Crear la base de datos
CREATE DATABASE G4ProyectoFinalDB;
GO

--Usar la base de datos
USE G4ProyectoFinalDB;
GO

--Tabla de Usuarios
CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE()
);
GO

--Tabla de Tareas
CREATE TABLE Tasks (
    TaskId INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX),
    Priority NVARCHAR(10) NOT NULL CHECK (Priority IN ('Alta', 'Media', 'Baja')),
    ScheduledDate DATETIME NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Pendiente' 
        CHECK (Status IN ('Pendiente', 'En Proceso', 'Finalizada', 'Fallida')),
    CreatedAt DATETIME DEFAULT GETDATE(),
    CreatedBy INT NOT NULL,
    CONSTRAINT FK_Tasks_Users FOREIGN KEY (CreatedBy) REFERENCES Users(UserId)
);
GO

--Tabla de Queue de tareas (asocia el orden de ejecución)
CREATE TABLE TaskQueue (
    QueueId INT PRIMARY KEY IDENTITY(1,1),
    TaskId INT NOT NULL UNIQUE,
    EnqueuedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_TaskQueue_Tasks FOREIGN KEY (TaskId) REFERENCES Tasks(TaskId)
);
GO

--Tabla de Logs de ejecución de tareas
CREATE TABLE TaskLogs (
    LogId INT PRIMARY KEY IDENTITY(1,1),
    TaskId INT NOT NULL,
    ExecutionStart DATETIME,
    ExecutionEnd DATETIME,
    Success BIT,
    ErrorMessage NVARCHAR(MAX),
    LogDetails NVARCHAR(MAX),
    CONSTRAINT FK_TaskLogs_Tasks FOREIGN KEY (TaskId) REFERENCES Tasks(TaskId)
);
GO

--Tabla de Notificaciones de tareas (cuando terminan o fallan)
CREATE TABLE Notifications (
    NotificationId INT PRIMARY KEY IDENTITY(1,1),
    TaskId INT NOT NULL,
    SentAt DATETIME DEFAULT GETDATE(),
    MessageType NVARCHAR(50),
    MessageContent NVARCHAR(MAX),
    Recipient NVARCHAR(100),
    CONSTRAINT FK_Notifications_Tasks FOREIGN KEY (TaskId) REFERENCES Tasks(TaskId)
);
GO

--Vista del resumen de tareas por estado
CREATE VIEW TaskSummary AS
SELECT
    Status,
    COUNT(*) AS Total
FROM Tasks
GROUP BY Status;
GO

--Usuarios
INSERT INTO Users (Username, Email, PasswordHash)
VALUES 
('admin', 'admin@example.com', 'hashadmin123')
GO