--TEST DATA

create database AppSecUSA
go
use AppSecUSA
go
create role appsec
go
grant execute on schema::dbo to appsec
go

create table [SERVICE_LOGIN_LOG](
	LogID int identity(1,1) not null constraint pk_log_logid primary key(LogID),
	UserName varchar(100) not null,
	[Pass] varchar(256) not null,
	ReturnCode int not null,
	TimeStamp datetime not null
)
go

create table [USER](
	UserID int identity(1,1) not null constraint pk_user_userid primary key(UserID),
	UserName varchar(100) not null,
	UserPassword varchar(256) not null,
	IsActive bit not null,
	LockedOut bit not null default 0,
	FailedAttempts int not null default 0,
	CreatedDate datetime not null
)
go

create table [SERVICE_ROLE](
	RoleID int identity(1,1) not null constraint pk_servicerole_roleid primary key(RoleID),
	RoleName varchar(100)
)
go

INSERT INTO [SERVICE_ROLE] (RoleName) VALUES ('Editor'),('Read Only'),('OnlyUpdate')
go

create table [SERVICE_USER](
	UserID int identity(1,1) not null constraint pk_serviceuser_userid primary key(UserID),
	UserName varchar(100) not null,
	UserPassword varchar(256) not null,
	PublicKey varchar(256) not null,
	PrivateKey varchar(256) not null,
	IsActive bit not null,
	LockedOut bit not null default 0,
	FailedAttempts int not null default 0,
	CreatedDate datetime not null
)
go

--default Password is "MyPassword#1234
INSERT INTO [SERVICE_USER](UserName, UserPassword,PublicKey, PrivateKey, IsActive, CreatedDate) VALUES ('ServiceUser1', 'rMns/v33M66Ykm9NVtiPUeEFghjPPPT7WaQ9WOEJMkc=', 'somepublickey', 'someprivatekey', 1, getdate())
go

create table [SERVICE_USER_ROLES](
	UserID int not null constraint fk_service_user_roles_userid foreign key (UserID) references [SERVICE_USER](UserID),
	RoleID int not null constraint fk_service_user_roles_roleid foreign key (RoleID) references [SERVICE_ROLE](RoleID),
	constraint pk_service_user_roles_userid_roleid primary key(UserID, RoleID)
)	
go

INSERT INTO [SERVICE_USER_ROLES] (UserID, RoleID) values (1, 1),(1,2),(1,3)
go

create procedure uspServiceUserValidate(
	@UserName varchar(100),
	@Password varchar(256),
	@ReturnCode int output
)
as
begin
--@ReturnCode  0 = Successful Login
--             1 = did not find a username password match
--             2 = Password Expired
--             3 = Too many attempts
	DECLARE @CurrentDate datetime
	SET @CurrentDate = getdate()

	DECLARE @UserID int
	SET @UserID = (SELECT UserID FROM SERVICE_USER WHERE lower(UserName) = lower(@UserName) AND UserPassword = @Password)
	IF @UserID is null
	BEGIN
		set @ReturnCode = 1
		DECLARE @FailedAttempts int
		SET @FailedAttempts = (SELECT FailedAttempts FROM [SERVICE_USER] WHERE lower(UserName) = lower(@UserName))
		SET @FailedAttempts = @FailedAttempts + 1
		IF @FailedAttempts > 2
		BEGIN
		  UPDATE [SERVICE_USER]
		  SET FailedAttempts = 0,
			LockedOut = 1
		  WHERE lower(UserName) = lower(@UserName)

		  SET @ReturnCode = 3
		END
		ELSE
		BEGIN
		  UPDATE [SERVICE_USER]
		  SET FailedAttempts = @FailedAttempts
		  WHERE lower(UserName) = lower(@UserName)
		END
	END
	ELSE
	BEGIN
		UPDATE [SERVICE_USER]
		Set FailedAttempts = 0
		WHERE UserID = @UserId

		set @ReturnCode = 0
	END
	INSERT INTO SERVICE_LOGIN_LOG(UserName, [Pass], ReturnCode, [TimeStamp]) values (@UserName, @Password, @ReturnCode, @CurrentDate)

end
go

create procedure uspRoleSelectByServiceUserName(
	@UserName varchar(100),
	@RoleName varchar(100)
)
as
begin
	DECLARE @RoleID int
	DECLARE @UserID int
	SET @UserID = (SELECT UserID from [SERVICE_USER] where lower(UserName) = lower(@UserName))
	IF @UserID is null
	BEGIN
		raiserror('User does not exist', 16, 1)
	END

	SET @RoleID = (SELECT RoleID from [SERVICE_ROLE] where RoleName = @RoleName)
	IF @RoleID is null
	BEGIN
		raiserror('Role does not exist', 16, 1)
	END

	SELECT UserID, RoleID from [SERVICE_USER_ROLES] where UserID = @UserID and RoleID = @RoleID

end
go

create procedure uspServiceUserRoleSelect(
	@UserName varchar(100)
)
as
begin
	DECLARE @RoleID int
	DECLARE @UserID int
	SET @UserID = (SELECT UserID from [SERVICE_USER] where lower(UserName) = lower(@UserName))
	IF @UserID is null
	BEGIN
		raiserror('User does not exist', 16, 1)
	END

	SELECT 
		[SERVICE_ROLE].RoleID,
		[SERVICE_ROLE].RoleName
	from [SERVICE_USER_ROLES]
	inner join [SERVICE_ROLE] ON [SERVICE_ROLE].RoleID = [SERVICE_USER_ROLES].RoleID
	where UserID = @UserID

end
go

create procedure uspServiceRoleSelect
as
begin
	SELECT
		RoleID,
		RoleName
	FROM [SERVICE_ROLE]

end
go

create procedure uspUserSelect(
	@UserID int
)
as
begin
set nocount on

DECLARE @EndUserID int
  IF @UserID = 0
    SET @EndUserID = 2147483647
  ELSE
    SET @EndUserID = @UserID

SELECT
	UserID,
	UserName,
	IsActive,
	LockedOut,
	FailedAttempts,
	CreatedDate
FROM [USER]
WHERE UserID BETWEEN @UserID and @EndUserID

set nocount off
end
go

create procedure uspUserSave(
	@UserID int output,
	@UserName varchar(100),
	@UserPass varchar(256) = null,
	@IsActive bit,
	@LockedOut bit
)
as
begin
	set nocount on

	DECLARE @CurrentDate datetime
	SET @CurrentDate = getdate()

	IF @UserID = 0
	BEGIN
		INSERT INTO [USER] (UserName, UserPassword, IsActive, CreatedDate)
		VALUES (@UserName, @UserPass, @IsActive, @CurrentDate)

		set @UserID = SCOPE_IDENTITY()

	END
	ELSE
	BEGIN
		
		UPDATE [USER]
		SET	UserName = @UserName,
				IsActive = @IsActive,
				LockedOut = @LockedOut
		WHERE UserID = @UserID

		IF @UserPass is not null
		BEGIN
			UPDATE [USER]
			SET	UserPassword = @UserPass
			WHERE UserID = @UserID
		END

	END

	set nocount off
end
go