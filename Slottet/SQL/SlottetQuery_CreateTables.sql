-- USE Slottet_Eks
-- GO

CREATE TABLE dbo.Department
( 
	DepartmentID UNIQUEIDENTIFIER PRIMARY KEY NOT NULL, 
	DepartmentName NVARCHAR NOT NULL 
);
GO

CREATE TABLE dbo.DepartmentTask
( 
	DepartmentTaskID UNIQUEIDENTIFIER PRIMARY KEY NOT NULL, 
	DepartmentTaskName NVARCHAR NOT NULL,
	
	DepartmentID UNIQUEIDENTIFIER NOT NULL,

	CONSTRAINT FK_Department_DepartmentTask
		FOREIGN KEY (DepartmentID) REFERENCES dbo.Department(DepartmentID)
);
GO

CREATE TABLE dbo.Phone
( 
	PhoneID UNIQUEIDENTIFIER PRIMARY KEY NOT NULL, 
	PhoneNumber NVARCHAR NOT NULL,
	
	DepartmentID UNIQUEIDENTIFIER NOT NULL,

	CONSTRAINT FK_Department_Phone
		FOREIGN KEY (DepartmentID) REFERENCES dbo.Department(DepartmentID)
);
GO

CREATE TABLE dbo.Staff
( 
	StaffID UNIQUEIDENTIFIER PRIMARY KEY NOT NULL, 
	StaffName NVARCHAR NOT NULL, 
	Initials NVARCHAR NOT NULL, 
	[Role] NVARCHAR NOT NULL, 
	DepartmentID UNIQUEIDENTIFIER NOT NULL,

	CONSTRAINT FK_Department_Staff
		FOREIGN KEY (DepartmentID) REFERENCES dbo.Department(DepartmentID)
);
GO

CREATE TABLE dbo.ShiftBoard
( 
	ShiftBoardID UNIQUEIDENTIFIER PRIMARY KEY NOT NULL, 
	ShiftType NVARCHAR NOT NULL, 
	StartDate DATETIME NOT NULL, 
	EndDate DATETIME NOT NULL
);
GO

CREATE TABLE dbo.SpecialResponsibility
( 
	SpecialResponsibilityID UniqueIDENTIFIER PRIMARY KEY NOT NULL,
	TaskName NVARCHAR NOT NULL,

	ShiftBoardID UNIQUEIDENTIFIER NOT NULL,

	CONSTRAINT FK_ShiftBoard_SpecialResponsibility
		FOREIGN KEY (ShiftBoardID) REFERENCES dbo.ShiftBoard(ShiftBoardID)
);
GO

CREATE TABLE dbo.StaffShift
( 
	ShiftBoardID UNIQUEIDENTIFIER NOT NULL, 
	StaffID UNIQUEIDENTIFIER NOT NULL, 

	CONSTRAINT PK_StaffShift
		PRIMARY KEY (ShiftBoardID, StaffID), 

	CONSTRAINT FK_ShiftBoard_StaffShift 
		FOREIGN KEY (ShiftBoardID) REFERENCES dbo.ShiftBoard(ShiftBoardID), 

	CONSTRAINT FK_Staff_StaffShift 
		FOREIGN KEY (StaffID) REFERENCES dbo.Staff(StaffID) 
);
GO

CREATE TABLE dbo.GroceryDay
( 
	GroceryDayID UNIQUEIDENTIFIER PRIMARY KEY NOT NULL,
	GroceryDayName NVARCHAR NOT NULL
);
GO	

CREATE TABLE dbo.Resident
( 
	ResidentID UNIQUEIDENTIFIER PRIMARY KEY NOT NULL, 
	ResidentName NVARCHAR NOT NULL,
	IsActive BIT NOT NULL, 

	GroceryDayID UNIQUEIDENTIFIER NOT NULL,

	CONSTRAINT FK_GroceryDay_Resident
		FOREIGN KEY (GroceryDayID) REFERENCES dbo.GroceryDay(GroceryDayID)
);
GO

CREATE TABLE dbo.Medicine
( 
	MedicineID UNIQUEIDENTIFIER PRIMARY KEY NOT NULL, 
	MedicineTime DATETIME NOT NULL,
	MedicineGivenTime DATETIME NOT NULL,
	MedicineRegisteredTime DATETIME NOT NULL,

	ResidentID UNIQUEIDENTIFIER NOT NULL,

	CONSTRAINT FK_Resident_Medicine
		FOREIGN KEY (ResidentID) REFERENCES dbo.Resident(ResidentID)
);
GO

CREATE TABLE dbo.RiskLevel( 
    RiskLevelID UNIQUEIDENTIFIER PRIMARY KEY NOT NULL, 
    RiskLevelName NVARCHAR NOT NULL 
);
GO

CREATE TABLE dbo.ResidentStatus
( 
	ResidentStatusID UNIQUEIDENTIFIER PRIMARY KEY NOT NULL, 
	Status NVARCHAR NOT NULL, 
	StatusEntryTime DATETIME NOT NULL, 
	 
	ResidentID UNIQUEIDENTIFIER NOT NULL,
	RiskLevelID UNIQUEIDENTIFIER NOT NULL,
	 
	CONSTRAINT FK_Resident_ResidentStatus 
		FOREIGN KEY (ResidentID) REFERENCES dbo.Resident(ResidentID), 
	CONSTRAINT FK_RiskLevel_ResidentStatus 
		FOREIGN KEY (RiskLevelID) REFERENCES dbo.RiskLevel(RiskLevelID)
);
GO

CREATE TABLE dbo.StaffResidentStatus
( 
	StaffID UNIQUEIDENTIFIER NOT NULL, 
	ResidentStatusID UNIQUEIDENTIFIER NOT NULL,
	
	CONSTRAINT PK_StaffResidentStatus
		PRIMARY KEY (StaffID, ResidentStatusID), 
	CONSTRAINT FK_Staff_StaffResidentStatus 
		FOREIGN KEY (StaffID) REFERENCES dbo.Staff(StaffID), 
	CONSTRAINT FK_ResidentStatus_StaffResidentStatus 
		FOREIGN KEY (ResidentStatusID) REFERENCES dbo.ResidentStatus(ResidentStatusID) 
);
GO

CREATE TABLE dbo.PN ( 
    PNID UNIQUEIDENTIFIER PRIMARY KEY NOT NULL, 
    PNTime TIME NOT NULL,
    PNStatus NVARCHAR NOT NULL,

	ResidentStatusID UNIQUEIDENTIFIER NOT NULL,

	CONSTRAINT FK_ResidentStatus_PN
		FOREIGN KEY (ResidentStatusID) REFERENCES dbo.ResidentStatus(ResidentStatusID)
); 
GO 

CREATE TABLE dbo.PaymentMethod( 
	PaymentMethodID UNIQUEIDENTIFIER PRIMARY KEY NOT NULL, 
	PaymentMethodName NVARCHAR NOT NULL 
);
GO

CREATE TABLE dbo.ResidentPaymentMethod( 
	ResidentID UNIQUEIDENTIFIER NOT NULL, 
	PaymentMethodID UNIQUEIDENTIFIER NOT NULL,

	CONSTRAINT PK_ResidentPaymentMethod
		PRIMARY KEY (ResidentID, PaymentMethodID), 
	CONSTRAINT FK_Resident_ResidentPaymentMethod 
		FOREIGN KEY (ResidentID) REFERENCES dbo.Resident(ResidentID), 
	CONSTRAINT FK_PaymentMethod_ResidentPaymentMethod 
		FOREIGN KEY (PaymentMethodID) REFERENCES dbo.PaymentMethod(PaymentMethodID)
);
GO

