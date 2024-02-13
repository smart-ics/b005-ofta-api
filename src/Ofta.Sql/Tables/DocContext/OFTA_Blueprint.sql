-- write script create table based on BlueprintModel
CREATE TABLE OFTA_Blueprint(
    BlueprintId VARCHAR(5) NOT NULL CONSTRAINT DF_OFTA_Blueprint_BlueprintId DEFAULT(''),
    BlueprintName VARCHAR(30) NOT NULL CONSTRAINT DF_OFTA_Blueprint_BlueprintName DEFAULT(''),
    
    CONSTRAINT PK_OFTA_Blueprint PRIMARY KEY CLUSTERED(BlueprintId)
)
