CREATE TABLE OFTA_CallbackRegistration(
  RegistrationId VARCHAR(50) NOT NULL CONSTRAINT DF_OFTA_CallbackRegistration_RegistrationId DEFAULT (''),
  TilakaName VARCHAR(50) NOT NULL CONSTRAINT DF_OFTA_CallbackRegistration_TilakaName DEFAULT (''),
  RegistrationStatus VARCHAR(2) NOT NULL CONSTRAINT DF_OFTA_CallbackRegistration_RegistrationStatus DEFAULT (''),
  RegistrationReasonCode VARCHAR(2) NOT NULL CONSTRAINT DF_OFTA_CallbackRegistration_RegistrationReasonCode DEFAULT (''),
  ManualRegistrationStatus VARCHAR(2) NOT NULL CONSTRAINT DF_OFTA_CallbackRegistration_ManualRegistrationStatus DEFAULT (''),
  CallbackDate VARCHAR(20) NOT NULL CONSTRAINT DF_OFTA_CallbackRegistration_CallbackDate DEFAULT('3000-01-01 00:00:00'),
  JsonPayload VARCHAR(MAX) NOT NULL CONSTRAINT DF_OFTA_CallbackRegistration_JsonPayload DEFAULT (''),
)
GO

CREATE INDEX IX_OFTA_CallbackRegistration_TilakaName
  ON OFTA_CallbackRegistration(TilakaName, RegistrationId)
  WITH(FILLFACTOR = 75)
GO