﻿CREATE TABLE OFTA_PostReact(
    PostId VARCHAR(13) CONSTRAINT OFTA_PostReact_PostId DEFAULT(''),
    PostReactDate DATETIME CONSTRAINT OFTA_PostReact_PostReactDate DEFAULT(''),
    UserOftaId VARCHAR(13) CONSTRAINT OFTA_PostReact_UserOftaId DEFAULT(''),
    
    CONSTRAINT PK_OFTA_PostReact PRIMARY KEY CLUSTERED (PostId, UserOftaId)
)