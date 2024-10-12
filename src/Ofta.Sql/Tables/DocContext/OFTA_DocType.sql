﻿CREATE TABLE OFTA_DocType(
    DocTypeId VARCHAR(5) NOT NULL CONSTRAINT DF_OFTA_DocType_DocTypeId DEFAULT(''),
    DocTypeName VARCHAR(100) NOT NULL CONSTRAINT DF_OFTA_DocType_DocTypeName DEFAULT(''),
    DocTypeCode VARCHAR(30) NOT NULL CONSTRAINT DF_OFTA_DocType_DocTypeCode DEFAULT(''),
    IsStandard BIT NOT NULL CONSTRAINT DF_OFTA_DocType_IsStandard DEFAULT(0),
    IsActive BIT NOT NULL CONSTRAINT DF_OFTA_DocType_IsActive DEFAULT(0),
    FileUrl VARCHAR(255) NOT NULL CONSTRAINT DF_OFTA_DocType_FileUrl DEFAULT(''),
    JenisDokRemoteCetak VARCHAR(20) NOT NULL CONSTRAINT DF_OFTA_DocType_JenisDokRemoteCetak DEFAULT(''),
    
    CONSTRAINT PK_OFTA_DocType PRIMARY KEY CLUSTERED(DocTypeId) 
)
GO

ALTER TABLE OFTA_DocType ADD DefaultDrafterUserId VARCHAR(13) NOT NULL CONSTRAINT DF_OFTA_DocType_DefaultDrafterUserId DEFAULT ('')
GO