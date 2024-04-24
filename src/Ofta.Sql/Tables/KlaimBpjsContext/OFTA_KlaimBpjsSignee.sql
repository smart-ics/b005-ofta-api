CREATE TABLE OFTA_KlaimBpjsSignee(
    KlaimBpjsId VARCHAR(13) NOT NULL CONSTRAINT DF_OFTA_KlaimBpjsSignee_KlaimBpjsId DEFAULT(''), 
    KlaimBpjsDocTypeId VARCHAR(16) NOT NULL CONSTRAINT DF_OFTA_KlaimBpjsSignee_KlaimBpjsDocTypeId DEFAULT(''),
    KlaimBpjsPrintOutId VARCHAR(19) NOT NULL CONSTRAINT DF_OFTA_KlaimBpjsSignee_KlaimBpjsPrintOutId DEFAULT(''),
    KlaimBpjsSigneeId VARCHAR(22) NOT NULL CONSTRAINT DF_OFTA_KlaimBpjsSignee_KlaimBpjsSigneeId DEFAULT(''), 
    NoUrut INT NOT NULL CONSTRAINT DF_OFTA_KlaimBpjsSignee_NoUrut DEFAULT(0), 
    UserOftaId VARCHAR(13) NOT NULL CONSTRAINT DF_OFTA_KlaimBpjsSignee_UserOftaId DEFAULT(''), 
    Email VARCHAR(50) NOT NULL CONSTRAINT DF_OFTA_KlaimBpjsSignee_Email DEFAULT(''), 
    SignTag VARCHAR(30) NOT NULL CONSTRAINT DF_OFTA_KlaimBpjsSignee_SignTag DEFAULT(''),
    
    CONSTRAINT PK_OFTA_KlaimBpjsSignee PRIMARY KEY (KlaimBpjsSigneeId),
)