CREATE TABLE OFTA_CallbackCertificateStatus(
  RegistrationId VARCHAR(50) NOT NULL CONSTRAINT DF_OFTA_CallbackCertificateStatus_RegistrationId DEFAULT (''),
  TilakaName VARCHAR(50) NOT NULL CONSTRAINT DF_OFTA_CallbackCertificateStatus_TilakaName DEFAULT (''),
  CertificateStatus VARCHAR(2) NOT NULL CONSTRAINT DF_OFTA_CallbackCertificateStatus_CertificateState DEFAULT (''),
  CallbackDate VARCHAR(20) NOT NULL CONSTRAINT DF_OFTA_CallbackCertificateStatus_CallbackDate DEFAULT('3000-01-01 00:00:00'),
  JsonPayload VARCHAR(MAX) NOT NULL CONSTRAINT DF_OFTA_CallbackCertificateStatus_JsonPayload DEFAULT (''),
)
GO

CREATE INDEX IX_OFTA_CallbackCertificateStatus_TilakaName
  ON OFTA_CallbackCertificateStatus(TilakaName, RegistrationId)
  WITH(FILLFACTOR = 75)
GO