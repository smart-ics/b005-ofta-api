CREATE TABLE OFTA_DraftOrder(
    DraftOrderId VARCHAR(13) NOT NULL CONSTRAINT DF_OFTA_DraftOrder_DraftOrderId DEFAULT (''),
    DraftOrderDate DATETIME NOT NULL CONSTRAINT DF_OFTA_DraftOrder_DraftOrderDate DEFAULT ('3000-01-01'),
    RequesterUserId VARCHAR(13) NOT NULL CONSTRAINT DF_OFTA_DraftOrder_RequesterUserId DEFAULT (''),
    DrafterUserId VARCHAR(13) NOT NULL CONSTRAINT DF_OFTA_DraftOrder_DrafterUserId DEFAULT (''),
    DocTypeId VARCHAR(5) NOT NULL CONSTRAINT DF_OFTA_DraftOrder_DocTypeId DEFAULT (''),
    Context VARCHAR(30) NOT NULL CONSTRAINT DF_OFTA_DraftOrder_Context DEFAULT (''),
    ContextReffId VARCHAR(13) NOT NULL CONSTRAINT DF_OFTA_DraftOrder_ContextReffId DEFAULT (''),

    CONSTRAINT PK_OFTA_DraftOrder PRIMARY KEY CLUSTERED(DraftOrderId)
)