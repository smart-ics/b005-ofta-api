CREATE TABLE OFTA_KlaimBpjsDocType(
    KlaimBpjsId VARCHAR(13) NOT NULL CONSTRAINT DF_OFTA_KlaimBpjsDocType_KlaimBpjsId DEFAULT(''),
    KlaimBpjsDocTypeId VARCHAR(16) NOT NULL CONSTRAINT DF_OFTA_KlaimBpjsDocType_KlaimBpjsDocId DEFAULT(''),
    NoUrut INT NOT NULL CONSTRAINT DF_OFTA_KlaimBpjsDocType_NoUrut DEFAULT(0),
    DocTypeId VARCHAR(5) NOT NULL CONSTRAINT DF_OFTA_KlaimBpjsDocType_DocTypeId DEFAULT(''),
    DocTypeName VARCHAR(30) NOT NULL CONSTRAINT DF_OFTA_KlaimBpjsDocType_DocTypeName DEFAULT(''),
     
    CONSTRAINT PK_OFTA_KlaimBpjsDocType PRIMARY KEY CLUSTERED (KlaimBpjsDocTypeId),
)
GO

CREATE INDEX IX_OFTA_KlaimBpjsDocType_KlaimBpjsId
    ON OFTA_KlaimBpjsDocType (KlaimBpjsId, KlaimBpjsDocTypeId)

ALTER TABLE OFTA_KlaimBpjsDocType ADD DrafterUserId VARCHAR(13) NOT NULL CONSTRAINT DF_OFTA_KlaimBpjsDocType_DrafterUserId DEFAULT ('')
GO

ALTER TABLE OFTA_KlaimBpjsDocType ADD ToBePrinted BIT NOT NULL CONSTRAINT DF_OFTA_KlaimBpjsDocType_ToBePrinted DEFAULT (1)
GO

UPDATE OFTA_KlaimBpjsDocType SET ToBePrinted = 0 WHERE DocTypeId = 'DTX01'
GO