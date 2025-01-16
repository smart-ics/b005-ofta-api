CREATE TABLE OFTA_DocTypeNumberValue(
    DocTypeId VARCHAR(5) NOT NULL CONSTRAINT DF_OFTA_DocTypeNumberValue_DocTypeId DEFAULT(''),
    Value INT NOT NULL CONSTRAINT DF_OFTA_DocTypeNumberValue_Value DEFAULT(0),
    PeriodeHari INT NOT NULL CONSTRAINT DF_OFTA_DocTypeNumberValue_PeriodeHari DEFAULT(0),
    PeriodeBulan INT NOT NULL CONSTRAINT DF_OFTA_DocTypeNumberValue_PeriodeBulan DEFAULT(0),
    PeriodeTahun INT NOT NULL CONSTRAINT DF_OFTA_DocTypeNumberValue_PeriodeTahun DEFAULT(0),
)
GO

CREATE CLUSTERED INDEX CX_OFTA_DocTypeNumberValue ON OFTA_DocTypeNumberValue(DocTypeId)
GO