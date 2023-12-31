CREATE LOGIN oftaLogin
	WITH PASSWORD = 'ofta123!'
GO

CREATE USER oftaUser FOR LOGIN oftaLogin
GO

sp_addrolemember 'db_datareader', 'oftaUser'
GO

sp_addrolemember 'db_datawriter', 'oftaUser'
GO

sp_addrolemember 'db_ddladmin', 'oftaUser'
GO
