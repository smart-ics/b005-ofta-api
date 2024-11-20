CREATE TABLE OFTA_BulkSignDoc(
    BulkSignId VARCHAR(13) NOT NULL CONSTRAINT DF_OFTA_BulkSignDoc_BulkSignId DEFAULT(''),
    DocId VARCHAR(13) NOT NULL CONSTRAINT DF_OFTA_BulkSignDoc_DocId DEFAULT(''),
    UploadedDocId VARCHAR(255) NOT NULL CONSTRAINT DF_OFTA_BulkSignDoc_UploadedDocId DEFAULT(''),
    RequestBulkSignState BIT NOT NULL CONSTRAINT DF_OFTA_BulkSignDoc_RequestBulkSignState DEFAULT(0),
    NoUrut INT NOT NULL CONSTRAINT DF_OFTA_BulkSignDoc_NoUrut DEFAULT(0),
    SignTag VARCHAR(50) NOT NULL CONSTRAINT OFTA_BulkSignDoc_SignTag DEFAULT(''),
    SignPosition INT NOT NULL CONSTRAINT OFTA_BulkSignDoc_SignPosition DEFAULT(0),
    SignPositionDesc VARCHAR(200) NOT NULL CONSTRAINT OFTA_BulkSignDoc_SignPositionDesc DEFAULT(''), 
    SignUrl VARCHAR(200) NOT NULL CONSTRAINT OFTA_BulkSignDoc_SignUrl DEFAULT(''),
)
GO

CREATE CLUSTERED INDEX CX_OFTA_BulkSignDoc ON OFTA_BulkSignDoc(BulkSignId)
GO

CREATE INDEX IX_OFTA_BulkSignDoc_UploadedDocId
    ON OFTA_BulkSignDoc(UploadedDocId, DocId)
    WITH(FILLFACTOR = 75)
GO