CREATE TABLE OFTA_KlaimBpjsPrintOut(
    KlaimBpjsId  VARCHAR(13) NOT NULL CONSTRAINT DF_OFTA_KlaimBpjsPrintOut_KlaimBpjsId DEFAULT(''),
    KlaimBpjsDocTypeId  VARCHAR(16) NOT NULL CONSTRAINT DF_OFTA_KlaimBpjsPrintOut_KlaimBpjsDocTypeId DEFAULT(''),
    KlaimBpjsPrintOutId  VARCHAR(19) NOT NULL CONSTRAINT DF_OFTA_KlaimBpjsPrintOut_KlaimBpjsPrintOutId DEFAULT(''),
    NoUrut  INT NOT NULL CONSTRAINT DF_OFTA_KlaimBpjsPrintOut_NoUrut DEFAULT(0),
    DocId  VARCHAR(13) NOT NULL CONSTRAINT DF_OFTA_KlaimBpjsPrintOut_DocId DEFAULT(''),
    DocUrl  VARCHAR(255) NOT NULL CONSTRAINT DF_OFTA_KlaimBpjsPrintOut_DocUrl DEFAULT(''),
    PrintOutReffId  VARCHAR(30) NOT NULL CONSTRAINT DF_OFTA_KlaimBpjsPrintOut_PrintOutReffId DEFAULT(''),
    PrintState  INT NOT NULL CONSTRAINT DF_OFTA_KlaimBpjsPrintOut_PrintState DEFAULT(0),
    
    CONSTRAINT PK_OFTA_KlaimBpjsPrintOut PRIMARY KEY CLUSTERED(KlaimBpjsPrintOutId)
)
GO

CREATE INDEX IX_OFTA_KlaimBpjsPrintOut_KlaimBpjsId 
    ON OFTA_KlaimBpjsPrintOut(KlaimBpjsId, KlaimBpjsPrintOutId)
GO
