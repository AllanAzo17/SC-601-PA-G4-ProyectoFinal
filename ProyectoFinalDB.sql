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
('admin', 'admin@example.com', 'hashadmin123'),
('user1', 'user1@example.com', 'hashuser1123');
GO

--Tareas
INSERT INTO Tasks (Title, Description, Priority, ScheduledDate, Status, CreatedBy)
VALUES 
('Enviar informe mensual', 'Enviar por correo el informe de ventas', 'Alta', '2025-08-05 10:00:00', 'Pendiente', 1),
('Backup base de datos', 'Respaldo completo de la base de datos del sistema', 'Media', '2025-08-06 02:00:00', 'En Proceso', 1),
('Actualizar dashboard', 'Actualizar gráficas y KPIs en el panel de control', 'Baja', '2025-08-07 15:30:00', 'Pendiente', 2),
('Enviar correos marketing', 'Campaña de marketing Agosto', 'Alta', '2025-08-04 09:00:00', 'Fallida', 2),
('Reprocesar estadísticas', 'Reprocesamiento por error de ingreso de datos', 'Media', '2025-08-03 08:00:00', 'Finalizada', 1);
GO

--Queue de tareas (solo las pendientes o en proceso deben estar en cola)
INSERT INTO TaskQueue (TaskId)
VALUES
(1), --Enviar informe mensual (Alta, Pendiente)
(2), --Backup base de datos (Media, En Proceso)
(3); --Actualizar dashboard (Baja, Pendiente)
GO

--Logs de ejecución
INSERT INTO TaskLogs (TaskId, ExecutionStart, ExecutionEnd, Success, ErrorMessage, LogDetails)
VALUES
(5, '2025-08-03 08:00:00', '2025-08-03 08:15:00', 1, NULL, 'Ejecución finalizada correctamente.'),
(4, '2025-08-04 09:00:00', '2025-08-04 09:01:30', 0, 'Error SMTP: Conexión rechazada', 'Falló al enviar los correos.'),
(2, '2025-08-05 02:00:00', NULL, NULL, NULL, 'En proceso...');
GO

--Notificaciones
INSERT INTO Notifications (TaskId, MessageType, MessageContent, Recipient)
VALUES
(5, 'Email', 'La tarea "Reprocesar estadísticas" se ejecutó con éxito.', 'admin@example.com'),
(4, 'Email', 'La tarea "Enviar correos marketing" falló. Error SMTP: Conexión rechazada.', 'user1@example.com');
GO