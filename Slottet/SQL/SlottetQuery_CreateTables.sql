-- USE Slottet_Eks
-- GO

CREATE TABLE dbo.SpecialResponsibility ( 
	SpecialResponsibilityID UNIQUEIDENTIFIER PRIMARY KEY NOT NULL, 
	TaskName NVARCHAR NOT NULL 
);
GO

CREATE TABLE dbo.ShiftBoard ( 
	ShiftBoardID UNIQUEIDENTIFIER PRIMARY KEY NOT NULL, 
	ShiftType NVARCHAR NOT NULL, 
	StartDate DATETIME NOT NULL, 
	EndDate DATETIME, 

	SpecialResponsibilityID UNIQUEIDENTIFIER, 

	CONSTRAINT FK_SpecialResponsibility_ShiftBoard 
		FOREIGN KEY (SpecialResponsibilityID) REFERENCES dbo.SpecialResponsibility(SpecialResponsibilityID) 
); 
GO 

CREATE TABLE dbo.DepartmentTask ( 
	DepartmentTaskID UNIQUEIDENTIFIER PRIMARY KEY NOT NULL, 
	DepartmentTaskName NVARCHAR NOT NULL 
);
GO

CREATE TABLE dbo.PHONE ( 
	PhoneID UNIQUEIDENTIFIER PRIMARY KEY NOT NULL, 
	PhoneNumber NVARCHAR NOT NULL 
);
GO

CREATE TABLE dbo.Department ( 
	DepartmentID UNIQUEIDENTIFIER PRIMARY KEY NOT NULL, 
	DepartmentName NVARCHAR NOT NULL, 

	PhoneID UNIQUEIDENTIFIER, 
	DepartmentTaskID UNIQUEIDENTIFIER, 

	CONSTRAINT FK_Phone_Department 
		FOREIGN KEY (PhoneID) REFERENCES dbo.Phone(PhoneID), 
	CONSTRAINT FK_DepartmentTask_Department 
		FOREIGN KEY (DepartmentTaskID) REFERENCES dbo.DepartmentTask(DepartmentTaskID)
);
GO

CREATE TABLE dbo.Staff ( 
	StaffID UNIQUEIDENTIFIER PRIMARY KEY NOT NULL, 
	StaffName NVARCHAR NOT NULL, 
	Initials NVARCHAR NOT NULL, 
	[Role] NVARCHAR NOT NULL, 

	DepartmentID UNIQUEIDENTIFIER, 

	CONSTRAINT FK_Department_Staff 
		FOREIGN KEY (DepartmentID) REFERENCES dbo.Department(DepartmentID) 
);
GO

CREATE TABLE dbo.StaffShift ( 
	ShiftBoardID UNIQUEIDENTIFIER NOT NULL, 
	StaffID UNIQUEIDENTIFIER NOT NULL, 

	CONSTRAINT FK_StaffShift
		PRIMARY KEY (ShiftBoardID, StaffID), 
	CONSTRAINT FK_ShiftBoard_StaffShift 
		FOREIGN KEY (ShiftBoardID) REFERENCES dbo.ShiftBoard(ShiftBoardID), 
	CONSTRAINT FK_Staff_StaffShift 
		FOREIGN KEY (StaffID) REFERENCES dbo.Staff(StaffID) 
);
GO

CREATE TABLE dbo.Medicine ( 
	MedicineID UNIQUEIDENTIFIER PRIMARY KEY NOT NULL, 
	MedicineTime DATETIME NOT NULL,
	MedicineGivenTime DATETIME NOT NULL,
	MedicineRegisteredTime DATETIME NOT NULL
);
GO

CREATE TABLE dbo.GroceryDay (
	GroceryDayID UNIQUEIDENTIFIER PRIMARY KEY NOT NULL,
	GroceryDay NVARCHAR NOT NULL
);
GO

CREATE TABLE dbo.Resident ( 
	ResidentID UNIQUEIDENTIFIER PRIMARY KEY NOT NULL, 
	ResidentName NVARCHAR NOT NULL,
	IsActive BIT NOT NULL, 
	
	GroceryDayID UNIQUEIDENTIFIER NOT NULL,

	CONSTRAINT FK_GroceryDay_Resident
		FOREIGN KEY (GroceryDayID) REFERENCES dbo.GroceryDay(GroceryDayID)
);
GO

CREATE TABLE dbo.ResidentMedicine (
	ResidentID UNIQUEIDENTIFIER NOT NULL,
	MedicineID UNIQUEIDENTIFIER NOT NULL,

	CONSTRAINT PK_ResidentMedicine
		PRIMARY KEY (ResidentID, MedicineID),
	CONSTRAINT FK_Resident_ResidentMedicine
		FOREIGN KEY (ResidentID) REFERENCES dbo.Resident(ResidentID),
	CONSTRAINT FK_Medicine_ResidentMedicine
		FOREIGN KEY (MedicineID) REFERENCES dbo.Medicine(MedicineID)
);
GO

CREATE TABLE dbo.PN ( 
	PNID UNIQUEIDENTIFIER PRIMARY KEY NOT NULL, 
	PNTime TIME NOT NULL,
	PNStatus NVARCHAR NOT NULL
); 
GO 

CREATE TABLE dbo.RiskLevel( 
	RiskLevelID UNIQUEIDENTIFIER PRIMARY KEY NOT NULL, 
	RiskLevelName NVARCHAR NOT NULL 
);
GO

CREATE TABLE dbo.PaymentMethod( 
	PaymentMethodID UNIQUEIDENTIFIER PRIMARY KEY NOT NULL, 
	PaymentMethod NVARCHAR NOT NULL 
); 
GO 

CREATE TABLE dbo.ResidentCard ( 
	ResidentCardID UNIQUEIDENTIFIER PRIMARY KEY NOT NULL, 
	[Status] NVARCHAR NOT NULL, 
	StatusEntryTime DATETIME NOT NULL, 

	StaffID UNIQUEIDENTIFIER, 
	ResidentID UNIQUEIDENTIFIER, 
	RiskLevelID UNIQUEIDENTIFIER, 
	PNID UNIQUEIDENTIFIER, 

	CONSTRAINT FK_Staff_ResidentCard
		FOREIGN KEY (StaffID) REFERENCES dbo.Staff(StaffID),
	CONSTRAINT FK_Resident_ResidentCard 
		FOREIGN KEY (ResidentID) REFERENCES dbo.Resident(ResidentID), 
	CONSTRAINT FK_RiskLevel_ResidentCard 
		FOREIGN KEY (RiskLevelID) REFERENCES dbo.RiskLevel(RiskLevelID), 
	CONSTRAINT FK_PN_ResidentCard 
		FOREIGN KEY (PNID) REFERENCES dbo.PN(PNID)
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