CREATE TABLE OFTA_TilakaUserRegistration (
  RegistrationId VARCHAR(50) NOT NULL CONSTRAINT DF_OFTA_TilakaUserRegistration_RegistrationId DEFAULT (''),
  UserOftaId VARCHAR(13) NOT NULL CONSTRAINT DF_OFTA_TilakaUserRegistration_OftaUserId DEFAULT (''),
  TilakaId VARCHAR(50) NOT NULL CONSTRAINT DF_OFTA_TilakaUserRegistration_TilakaId DEFAULT (''),
  TilakaName VARCHAR(50) NOT NULL CONSTRAINT DF_OFTA_TilakaUserRegistration_TilakaName DEFAULT (''),
  NamaKTP VARCHAR(255) NOT NULL CONSTRAINT DF_OFTA_TilakaUserRegistration_NamaKTP DEFAULT (''),
  NomorIdentitas VARCHAR(16) NOT NULL CONSTRAINT DF_OFTA_TilakaUserRegistration_NomorIdentitas DEFAULT (''),
  ExpiredDate DATETIME NOT NULL CONSTRAINT DF_OFTA_TilakaUserRegistration_ExpiredDate DEFAULT('3000-01-01'),
  UserState INT NOT NULL CONSTRAINT DF_OFTA_TilakaUserRegistration_UserState DEFAULT(0),
  CertificateState INT NOT NULL CONSTRAINT DF_OFTA_TilakaUserRegistration_CertificateState DEFAULT(0),
  RevokeReason VARCHAR(100) NOT NULL CONSTRAINT DF_OFTA_TilakaUserRegistration_RevokeReason DEFAULT ('')

  CONSTRAINT PK_OFTA_TilakaUserRegistration PRIMARY KEY CLUSTERED(RegistrationId)
)
