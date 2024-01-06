﻿CREATE TABLE OFTA_DocJurnal(
    DocId VARCHAR(13) NOT NULL CONSTRAINT DF_OFTA_DocJurnal_DocId DEFAULT(''),
    NoUrut INT NOT NULL CONSTRAINT DF_OFTA_DocJurnal_NoUrut DEFAULT(0),
    JurnalDate DATETIME NOT NULL CONSTRAINT DF_OFTA_DocJurnal_JurnalDate DEFAULT('3000-01-01'),
    JurnalDesc VARCHAR(50) NOT NULL CONSTRAINT DF_OFTA_DocJurnal_JurnalDesc DEFAULT(''),
    DocState INT NOT NULL CONSTRAINT DF_OFTA_DocJurnal_DocState DEFAULT(0),
)
GO

CREATE CLUSTERED INDEX CX_OFTA_DocJurnal_DocId ON OFTA_DocJurnal(DocId)
GO