CREATE TABLE OFTA_OrderKlaimBpjs(
    OrderKlaimBpjsId VARCHAR(13) NOT NULL CONSTRAINT DF_OFTA_OrderKlaimBpjs_OrderKlaimBpjsId DEFAULT(''),
    OrderKlaimBpjsDate DATETIME NOT NULL CONSTRAINT DF_OFTA_OrderKlaimBpjs_OrderKlaimBpjsDate DEFAULT(''),
    KlaimBpjsId VARCHAR(13) NOT NULL CONSTRAINT DF_OFTA_OrderKlaimBpjs_KlaimBpjsId DEFAULT(''),
    UserOftaId VARCHAR(13) NOT NULL CONSTRAINT DF_OFTA_OrderKlaimBpjs_UserOftaId DEFAULT(''),
    RegId VARCHAR(10) NOT NULL CONSTRAINT DF_OFTA_OrderKlaimBpjs_RegId DEFAULT(''),
    PasienId VARCHAR(15) NOT NULL CONSTRAINT DF_OFTA_OrderKlaimBpjs_PasienId DEFAULT(''),
    PasienName VARCHAR(50) NOT NULL CONSTRAINT DF_OFTA_OrderKlaimBpjs_PasienName DEFAULT(''),
    NoSep VARCHAR(20) NOT NULL CONSTRAINT DF_OFTA_OrderKlaimBpjs_NoSep DEFAULT(''),
    LayananName VARCHAR(30) NOT NULL CONSTRAINT DF_OFTA_OrderKlaimBpjs_LayananName DEFAULT(''),
    DokterName VARCHAR(30) NOT NULL CONSTRAINT DF_OFTA_OrderKlaimBpjs_DokterName DEFAULT(''),
    RajalRanap INT NOT NULL CONSTRAINT DF_OFTA_OrderKlaimBpjs_RajalRanap DEFAULT(0),
    
    CONSTRAINT PK_OFTA_OrderKlaimBpjs PRIMARY KEY CLUSTERED(OrderKlaimBpjsId),
)
GO