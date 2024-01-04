﻿CREATE TABLE OFTA_DocType(
    DocTypeId VARCHAR(5) NOT NULL CONSTRAINT DF_OFTA_DocType_DocTypeId DEFAULT(''),
    DocTypeName VARCHAR(30) NOT NULL CONSTRAINT DF_OFTA_DocType_DocTypeName DEFAULT(''),
    IsTemplate BIT NOT NULL CONSTRAINT DF_OFTA_DocType_IsTemplate DEFAULT(0),
    TemplateUrl VARCHAR(128) NOT NULL CONSTRAINT DF_OFTA_DocType_TemplateUrl DEFAULT(''),
    TemplateType INT NOT NULL CONSTRAINT DF_OFTA_DocType_TemplateType DEFAULT(0),
    IsActive BIT NOT NULL CONSTRAINT DF_OFTA_DocType_IsActive DEFAULT(0),
    
    CONSTRAINT PK_OFTA_DocType PRIMARY KEY CLUSTERED(DocTypeId) 
)
GO