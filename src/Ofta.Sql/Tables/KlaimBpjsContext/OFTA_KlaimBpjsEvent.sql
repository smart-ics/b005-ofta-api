CREATE TABLE OFTA_KlaimBpjsEvent(
    KlaimBpjsId VARCHAR(13) NOT NULL CONSTRAINT OFTA_KlaimBpjsEvent_KlaimBpjsId DEFAULT(''),
    KlaimBpjsJurnalId VARCHAR(16) NOT NULL CONSTRAINT OFTA_KlaimBpjsEvent_KlaimBpjsJurnalId DEFAULT(''),
    NoUrut INT NOT NULL CONSTRAINT OFTA_KlaimBpjsEvent_NoUrut DEFAULT(0),
    EventDate DATETIME NOT NULL CONSTRAINT OFTA_KlaimBpjsEvent_EventDate DEFAULT('3000-01-01'),
    Description VARCHAR(50) NOT NULL CONSTRAINT OFTA_KlaimBpjsEvent_Description DEFAULT(''),
    
    CONSTRAINT PK_OFTA_KlaimBpjsEvent PRIMARY KEY (KlaimBpjsId, NoUrut),
)
GO
