CREATE PROCEDURE usp_Insert_SpecialResponsibility
	@SpecialResponsibilityID UNIQUEIDENTIFIER,
	@TaskName NVARCHAR(255)
AS
BEGIN
	INSERT INTO dbo.SpecialResponsibility (SpecialResponsibilityID, TaskName)
	VALUES (@SpecialResponsibilityID, @TaskName);
END
GO

CREATE PROCEDURE usp_SelectById_SpecialResponsibility
	@SpecialResponsibilityID UNIQUEINDENTIFIER
AS
BEGIN
	SELECT
		SpecialResponsibilityID,
		TaskName
	FROM dbo.SpecialResponsibility
	WHERE SpecialResponsibilityID = @SpecialResponsibilityID;
END
GO

CREATE PROCEDURE usp_Update_SpecialResponsibility
	@SpecialResponsibilityID UNIQUEIDENTIFIER,
	@TaskName NVARCHAR(255)
AS
BEGIN
	UPDATE Dbo.SpecialResponsibility
	SET TaskName = @TaskName
	WHERE SpecialResponsibilityID = @SpecialResponsibilityID;
END
GO

CREATE PROCEDURE usp_Delete_SpecialResponsibility
	@SpecialResponsibilityID UNIQUEIDENTIFIER

AS
BEGIN
	DELETE FROM dbo.SpecialResponsibility
	WHERE SpecialResponsibilityID = @SpecialResponsibilityID;
END
GO


CREATE PROCEDURE usp_Insert_Resident
	@ResidentID UNIQUEIDENTIFIER,
	@ResidentName NVARCHAR(255),
	@IsActive BIT,
	@GroceryDayID UNIQUEIDENTIFIER
AS
BEGIN
	INSERT INTO dbo.Resident(ResidentID, ResidentName, IsActive,GroceryDayID)
	VALUES (@ResidentID, @ResidentName, @IsActive, @GroceryDayID)
END
GO

CREATE PROCEDURE usp_SelectByID_Resident
	@ResidentID UNIQUEIDENTIFIER

AS
BEGIN
	SELECT
		r.ResidentID,
		r.ResidentName,
		r.IsActive,
		r.GroceryDayID
	FROM dbo.Resident r
	INNER JOIN dbo.GroceryDay g
		ON r.GroceryDayID = g.GroceryDayID
	WHERE r.ResidentID = @ResidentID;
END
GO

CREATE PROCEDURE

	