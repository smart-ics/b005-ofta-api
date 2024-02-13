﻿CREATE TABLE OFTA_TeamUserOfta(
    TeamId VARCHAR(5) NOT NULL CONSTRAINT DF_OFTA_TeamUserOfta_TeamId DEFAULT(''),
    UserOftaId VARCHAR(13) NOT NULL CONSTRAINT DF_OFTA_TeamUserOfta_UserOftaId DEFAULT(''),
    
    CONSTRAINT PK_OFTA_TeamUserOfta PRIMARY KEY CLUSTERED(TeamId, UserOftaId),
)