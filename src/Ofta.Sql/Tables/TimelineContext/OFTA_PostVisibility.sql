CREATE TABLE OFTA_PostVisibility(
    PostId VARCHAR(13) NOT NULL CONSTRAINT DF_OFTA_PostVisibility_PostId DEFAULT(''),
    VisibilityReff VARCHAR(13) NOT NULL CONSTRAINT DF_OFTA_PostVisibility_VisibilityReff DEFAULT(''),
    
    CONSTRAINT PK_OFTA_PostVisibility PRIMARY KEY CLUSTERED(PostId, VisibilityReff)
)