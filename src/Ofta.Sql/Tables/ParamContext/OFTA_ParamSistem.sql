CREATE TABLE OFTA_ParamSistem(
    ParamSistemId VARCHAR(50) NOT NULL CONSTRAINT DF_OFTA_ParamSistem_ParamSistemId DEFAULT(''),
    ParamSistemValue VARCHAR(255) NOT NULL CONSTRAINT DF_OFTA_ParamSistem_ParamSistemValue DEFAULT(''),
    
    CONSTRAINT PK_OFTA_ParamSistem PRIMARY KEY(ParamSistemId)    
)