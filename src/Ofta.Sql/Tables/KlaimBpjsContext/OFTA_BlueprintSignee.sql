CREATE TABLE OFTA_BlueprintSignee(
    BlueprintId VARCHAR(5) NOT NULL CONSTRAINT DF_OFTA_BlueprintSignee_BlueprintId DEFAULT(''), 
    BlueprintDocTypeId VARCHAR(8) NOT NULL CONSTRAINT DF_OFTA_BlueprintSignee_BlueprintDocTypeId DEFAULT(''), 
    BlueprintSigneeId VARCHAR(11) NOT NULL CONSTRAINT DF_OFTA_BlueprintSignee_BlueprintSigneeId DEFAULT(''), 
    NoUrut INT NOT NULL CONSTRAINT DF_OFTA_BlueprintSignee_NoUrut DEFAULT(0),
    
    UserOftaId VARCHAR(13) NOT NULL CONSTRAINT DF_OFTA_BlueprintSignee_UserOftaId DEFAULT(''), 
    Email VARCHAR(50) NOT NULL CONSTRAINT DF_OFTA_BlueprintSignee_Email DEFAULT(''), 
    SignTag VARCHAR(50) NOT NULL CONSTRAINT DF_OFTA_BlueprintSignee_SignTag DEFAULT(''), 
    SignPosition INT NOT NULL CONSTRAINT DF_OFTA_BlueprintSignee_SignPosition DEFAULT(0), 

    CONSTRAINT PK_OFTA_BlueprintSignee PRIMARY KEY CLUSTERED (BlueprintSigneeId)
)