CREATE TABLE OFTA_CommentReact(
    CommentId VARCHAR(13) NOT NULL CONSTRAINT DF_OFTA_CommentReact_CommentId DEFAULT(''),
    CommentReactDate DATETIME NOT NULL CONSTRAINT DF_OFTA_CommentReact_CommentReactDate DEFAULT('3000-01-01'),
    UserOftaId VARCHAR(13) NOT NULL CONSTRAINT DF_OFTA_CommentReact_UserOftaId DEFAULT(''),
    
    CONSTRAINT PK_OFTA_CommentReact PRIMARY KEY CLUSTERED(CommentId, UserOftaId)
)