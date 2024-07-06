CREATE TABLE OFTA_KlaimBpjsMergerFile(
    KlaimBpjsId  VARCHAR(13) NOT NULL CONSTRAINT DF_OFTA_KlaimBpjsMergerFile_KlaimBpjsId DEFAULT(''),
    DocId  VARCHAR(13) NOT NULL CONSTRAINT DF_OFTA_KlaimBpjsMergerFile_DocId DEFAULT(''),
    DocUrl  VARCHAR(255) NOT NULL CONSTRAINT DF_OFTA_KlaimBpjsMergerFile_DocUrl DEFAULT('')
    
    CONSTRAINT PK_OFTA_KlaimBpjsMergerFile PRIMARY KEY CLUSTERED(KlaimBpjsId)
)
GO

CREATE INDEX IX_OFTA_KlaimBpjsMergerFileId 
    ON OFTA_KlaimBpjsMergerFile(KlaimBpjsId)
GO
