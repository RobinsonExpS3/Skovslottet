USE Slottet

CREATE TABLE dbo.SpecialResponsibility ( 
	SpecialResponsibilityID UNIQUEIDENTIFIER PRIMARY KEY NOT NULL, 
	Task NVARCHAR NOT NULL ); 
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
	DepartmentTask NVARCHAR NOT NULL 
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
	MedicineGivenTime DATETIME 
); 
GO 

CREATE TABLE dbo.Resident ( 
	ResidentID UNIQUEIDENTIFIER PRIMARY KEY NOT NULL, 
	ResidentName NVARCHAR NOT NULL,
	GroceryDay DATE NOT NULL, 
	IsActive BIT NOT NULL, 
	
	MedicineID UNIQUEIDENTIFIER,

	CONSTRAINT FK_Medicine_Resident
		FOREIGN KEY (MedicineID) REFERENCES dbo.Medicine(MedicineID) 
); 
GO 

CREATE TABLE dbo.PN ( 
	PNID UNIQUEIDENTIFIER PRIMARY KEY NOT NULL, 
	PNTime NVARCHAR NOT NULL
); 
GO 

CREATE TABLE dbo.RiskLevel( 
	RiskLevelID UNIQUEIDENTIFIER PRIMARY KEY NOT NULL, 
	RiskLevel NVARCHAR NOT NULL 
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
	[Date] DATETIME NOT NULL, 

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
	ResidentCardID UNIQUEIDENTIFIER NOT NULL, 
	PaymentMethodID UNIQUEIDENTIFIER NOT NULL,

	CONSTRAINT PK_ResidentPaymentMethod
		PRIMARY KEY (ResidentCardID, PaymentMethodID), 
	CONSTRAINT FK_ResidentCard_ResidentPaymentMethod 
		FOREIGN KEY (ResidentCardID) REFERENCES dbo.ResidentCard(ResidentCardID), 
	CONSTRAINT FK_PaymentMethod_ResidentPaymentMethod 
		FOREIGN KEY (PaymentMethodID) REFERENCES dbo.PaymentMethod(PaymentMethodID)
); 
GO



ALTER TABLE dbo.Resident 
	ALTER COLUMN GroceryDay NVARCHAR NOT NULL;
GO 

	