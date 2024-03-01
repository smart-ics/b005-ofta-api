﻿CREATE TABLE OFTA_KlaimBpjs(
    KlaimBpjsId VARCHAR(13) NOT NULL CONSTRAINT DF_OFTA_KlaimBpjs_KlaimBpjsId DEFAULT(''),
    KlaimBpjsDate DATETIME NOT NULL CONSTRAINT DF_OFTA_KlaimBpjs_KlaimBpjsDate DEFAULT('3000-01-01'),
    OrderKlaimBpjsId VARCHAR(13) NOT NULL CONSTRAINT DF_OFTA_KlaimBpjs_OrderKlaimBpjsId DEFAULT(''),
    UserOftaId VARCHAR(13) NOT NULL CONSTRAINT DF_OFTA_KlaimBpjs_UserOftaId DEFAULT(''),
    BundleState INT NOT NULL CONSTRAINT DF_OFTA_KlaimBpjs_BundleState DEFAULT(0),
    RegId VARCHAR(10) NOT NULL CONSTRAINT DF_OFTA_KlaimBpjs_RegId DEFAULT(''),
    PasienId VARCHAR(15) NOT NULL CONSTRAINT DF_OFTA_KlaimBpjs_PasienId DEFAULT(''),
    PasienName VARCHAR(60) NOT NULL CONSTRAINT DF_OFTA_KlaimBpjs_PasienName DEFAULT(''),
    NoSep VARCHAR(20) NOT NULL CONSTRAINT DF_OFTA_KlaimBpjs_NoSep DEFAULT(''),
    LayananName VARCHAR(50) NOT NULL CONSTRAINT DF_OFTA_KlaimBpjs_LayananName DEFAULT(''),
    DokterName VARCHAR(50) NOT NULL CONSTRAINT DF_OFTA_KlaimBpjs_DokterName DEFAULT(''),
    RajalRanap INT NOT NULL CONSTRAINT DF_OFTA_KlaimBpjs_RajalRanap DEFAULT(0),
    
    CONSTRAINT PK_OFTA_KlaimBpjs PRIMARY KEY CLUSTERED (KlaimBpjsId)
)