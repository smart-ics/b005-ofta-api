CREATE TABLE OFTA_UserMapping (
  UserOftaId VARCHAR(13) NOT NULL CONSTRAINT DF_OFTA_UserMapping_UserOftaId DEFAULT (''),
  UserMappingId VARCHAR(13) NOT NULL CONSTRAINT DF_OFTA_UserMapping_UserMappingId DEFAULT (''),
  PegId VARCHAR(10) NOT NULL CONSTRAINT DF_OFTA_UserMapping_PegId DEFAULT (''),
  UserType INT NOT NULL CONSTRAINT DF_OFTA_UserMapping_UserType DEFAULT(0),
)
GO

CREATE INDEX IX_OFTA_UserMapping_UserOftaId
  ON OFTA_UserMapping (UserOftaId, UserMappingId)
GO
