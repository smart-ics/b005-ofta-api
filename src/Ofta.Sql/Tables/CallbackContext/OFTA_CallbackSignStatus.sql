CREATE TABLE OFTA_CallbackSignStatus(
  RequestId VARCHAR(20) NOT NULL CONSTRAINT DF_OFTA_CallbackSignStatus_RequestId DEFAULT(''),
  UserOftaId VARCHAR(13) NOT NULL CONSTRAINT DF_OFTA_CallbackSignStatus_UserOftaId DEFAULT(''),
  Email VARCHAR(50) NOT NULL CONSTRAINT DF_OFTA_CallbackSignStatus_Email DEFAULT(''),
  TilakaName VARCHAR(50) NOT NULL CONSTRAINT DF_OFTA_CallbackSignStatus_TilakaName DEFAULT (''),
  CallbackDate VARCHAR(20) NOT NULL CONSTRAINT DF_OFTA_CallbackSignStatus_CallbackDate DEFAULT('3000-01-01 00:00:00'),
  JsonPayload VARCHAR(MAX) NOT NULL CONSTRAINT DF_OFTA_CallbackSignStatus_JsonPayload DEFAULT (''),
)
GO

CREATE INDEX IX_OFTA_CallbackSignStatus_TilakaName
  ON OFTA_CallbackSignStatus(TilakaName, RequestId)
  WITH(FILLFACTOR = 75)
GO