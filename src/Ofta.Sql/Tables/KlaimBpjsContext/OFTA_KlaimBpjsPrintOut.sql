CREATE TABLE OFTA_KlaimBpjsPrintOut(
    KlaimBpjsId  VARCHAR(13) NOT NULL CONSTRAINT DF_OFTA_KlaimBpjsPrintOut_KlaimBpjsId DEFAULT(''),
    KlaimBpjsDocTypeId  VARCHAR(13) NOT NULL CONSTRAINT DF_OFTA_KlaimBpjsPrintOut_KlaimBpjsDocTypeId DEFAULT(''),
    KlaimBpjsPrintOutId  VARCHAR(13) NOT NULL CONSTRAINT DF_OFTA_KlaimBpjsPrintOut_KlaimBpjsPrintOutId DEFAULT(''),
    NoUrut  VARCHAR(13) NOT NULL CONSTRAINT DF_OFTA_KlaimBpjsPrintOut_NoUrut DEFAULT(''),
    DocId  VARCHAR(13) NOT NULL CONSTRAINT DF_OFTA_KlaimBpjsPrintOut_DocId DEFAULT(''),
    DocUrl  VARCHAR(13) NOT NULL CONSTRAINT DF_OFTA_KlaimBpjsPrintOut_DocUrl DEFAULT(''),
    PrintOutReffId  VARCHAR(13) NOT NULL CONSTRAINT DF_OFTA_KlaimBpjsPrintOut_PrintOutReffId DEFAULT(''),
    PrintState  VARCHAR(13) NOT NULL CONSTRAINT DF_OFTA_KlaimBpjsPrintOut_PrintState DEFAULT(''),
    
    CONSTRAINT PK_OFTA_KlaimBpjsPrintOut PRIMARY KEY CLUSTERED(KlaimBpjsPrintOutId)
)
GO

CREATE INDEX IX_OFTA_KlaimBpjsPrintOut_KlaimBpjsId 
    ON OFTA_KlaimBpjsPrintOut(KlaimBpjsId, KlaimBpjsPrintOutId)
GO
