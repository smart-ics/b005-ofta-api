CREATE TABLE OFTA_CommentReact(
    CommentId VARCHAR(13) NOT NULL CONSTRAINT DF_OFTA_CommentReact_CommentId DEFAULT(''),
    CommentReactDate VARCHAR(13) NOT NULL CONSTRAINT DF_OFTA_CommentReact_CommentReactDate DEFAULT(''),
    UserOftaId VARCHAR(13) NOT NULL CONSTRAINT DF_OFTA_CommentReact_UserOftaId DEFAULT(''),
    
    CONSTRAINT PK_OFTA_CommentReact PRIMARY KEY CLUSTERED(CommentId, UserOftaId)
)