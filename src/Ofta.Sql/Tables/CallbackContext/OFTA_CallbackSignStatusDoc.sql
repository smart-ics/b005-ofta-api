CREATE TABLE OFTA_CallbackSignStatusDoc(
  RequestId VARCHAR(20) NOT NULL CONSTRAINT DF_OFTA_CallbackSignStatusDoc_RequestId DEFAULT(''),
  TilakaName VARCHAR(50) NOT NULL CONSTRAINT DF_OFTA_CallbackSignStatusDoc_TilakaName DEFAULT(''),
  UploadedDocId VARCHAR(255) NOT NULL CONSTRAINT DF_OFTA_CallbackSignStatusDoc_UploadedDocId DEFAULT(''),
  UserSignState INT NOT NULL CONSTRAINT OFTA_CallbackSignStatusDoc_UserSignState DEFAULT(0),
  DownloadDocUrl VARCHAR(MAX) NOT NULL CONSTRAINT DF_OFTA_CallbackSignStatusDoc_DownloadDocUrl DEFAULT(''),
)
GO

CREATE INDEX IX_OFTA_CallbackSignStatusDoc_UploadedDocId
  ON OFTA_CallbackSignStatusDoc(UploadedDocId, TilakaName)
  WITH(FILLFACTOR = 75)
GO

