﻿CREATE TABLE OFTA_DocScope(
    DocId VARCHAR(13) NOT NULL CONSTRAINT DF_OFTA_DocScope_DocId DEFAULT(''),
    ScopeType INT NOT NULL CONSTRAINT DF_OFTA_DocScope_ScopeType DEFAULT(0),
    ScopeReffId VARCHAR(13) NOT NULL CONSTRAINT DF_OFTA_DocScope_ScopeReffId DEFAULT('')

    CONSTRAINT PK_OFTA_DocScope PRIMARY KEY CLUSTERED(DocId, ScopeReffId)
)