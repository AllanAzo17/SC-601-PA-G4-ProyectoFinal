CREATE DATABASE ProyectoFinalDB
GO

USE ProyectoFinalDB
GO

CREATE TABLE Queues (
	QueueID INT IDENTITY (1,1) PRIMARY KEY,
	Priority INT NOT NULL
);
GO

CREATE TABLE Tasks (
	TaskID INT IDENTITY (1,1) PRIMARY KEY,
	Status VARCHAR(50) NOT NULL,
	Description VARCHAR(255) NOT NULL,
	ExecutionDate DATE NOT NULL,
	Package VARCHAR(255),
	QueueID INT,
	FOREIGN KEY (QueueID) REFERENCES Queues(QueueID)
);
GO

INSERT INTO Queues (Priority) VALUES (1);
INSERT INTO Queues (Priority) VALUES (2);
INSERT INTO Queues (Priority) VALUES (3);
INSERT INTO Queues (Priority) VALUES (2);
INSERT INTO Queues (Priority) VALUES (1);
GO

INSERT INTO Tasks (Status, Description, ExecutionDate, Package, QueueID)
VALUES 
('Pending', 'Generate daily report', '2025-06-15', 'DailyReportGen', 1),
('Completed', 'Backup system', '2025-06-13', 'SysBackup', 2),
('In Progress', 'Update software version', '2025-06-14', 'UpdaterV2', 3),
('Failed', 'Export client data', '2025-06-12', 'DataExporter', 4),
('Scheduled', 'Monthly billing cycle', '2025-07-01', 'BillCycle', 5);
GO