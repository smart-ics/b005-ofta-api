CREATE TABLE OFTA_BlueprintDocType(
    BlueprintId VARCHAR(5) NOT NULL CONSTRAINT DF_OFTA_BlueprintDocType_BlueprintId DEFAULT(''), 
    BlueprintDocTypeId VARCHAR(8) NOT NULL CONSTRAINT DF_OFTA_BlueprintDocType_BlueprintDocTypeId DEFAULT(''), 
    NoUrut INT NOT NULL CONSTRAINT DF_OFTA_BlueprintDocType_NoUrut DEFAULT(0), 
    DocTypeId VARCHAR(5) NOT NULL CONSTRAINT DF_OFTA_BlueprintDocType_DocTypeId DEFAULT(''), 

    CONSTRAINT PK_BlueprintDocType PRIMARY KEY CLUSTERED (BlueprintDocTypeId)
)