﻿CREATE TABLE OFTA_Doc(
    DocId VARCHAR(13) NOT NULL CONSTRAINT DF_OFTA_Doc_DocId DEFAULT(''),
    DocDate DATETIME NOT NULL CONSTRAINT DF_OFTA_Doc_DocDate DEFAULT('3000-01-01'),
    DocTypeId VARCHAR(5) NOT NULL CONSTRAINT DF_OFTA_Doc_DocTypeId DEFAULT(''),
    UserOftaId VARCHAR(13) NOT NULL CONSTRAINT DF_OFTA_Doc_UserOftaId DEFAULT(''),
    Email VARCHAR(50) NOT NULL CONSTRAINT DF_OFTA_Doc_Email DEFAULT(''),
    DocState INT NOT NULL CONSTRAINT DF_OFTA_Doc_DocState DEFAULT(0),
    DocName VARCHAR(255) NOT NULL CONSTRAINT DF_OFTA_Doc_Name DEFAULT(''),
    
    RequestedDocUrl VARCHAR(255) NOT NULL CONSTRAINT DF_OFTA_Doc_RequestedDocUrl DEFAULT(''),
    UploadedDocId VARCHAR(255) NOT NULL CONSTRAINT DF_OFTA_Doc_UploadedDocId DEFAULT(''),
    UploadedDocUrl VARCHAR(255) NOT NULL CONSTRAINT DF_OFTA_Doc_UploadedDocUrl DEFAULT(''),
    PublishedDocUrl VARCHAR(255) NOT NULL CONSTRAINT DF_OFTA_Doc_PublishedDocUrl DEFAULT(''),
    
    CONSTRAINT PK_OFTA_Doc PRIMARY KEY CLUSTERED(DocId)
)
GO

CREATE INDEX IX_OFTA_Doc_DocDate 
    ON OFTA_Doc(DocDate, DocId)
    WITH(FILLFACTOR = 90)
GO

CREATE INDEX IX_OFTA_Doc_UploadedDocId
    ON OFTA_Doc(UploadedDocId, DocId)
    WITH(FILLFACTOR = 75)
GO