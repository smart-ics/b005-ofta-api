CREATE TABLE OFTA_CallbackSignStatus(
  CallbackDate DATETIME NOT NULL CONSTRAINT DF_OFTA_CallbackSignStatus_CallbackDate DEFAULT('3000-01-01'),
  Payload VARCHAR(MAX) NOT NULL CONSTRAINT DF_OFTA_CallbackSignStatus_Payload DEFAULT(''),
)
GO