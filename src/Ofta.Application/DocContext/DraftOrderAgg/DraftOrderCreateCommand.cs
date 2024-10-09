namespace Ofta.Application.DocContext.DraftOrderAgg;

public record DraftOrderKlaimBpjsCommand(
    string DocTypeId,
    string DrafterUserId,
    string RequesterUserId,
    string ContextReffId);
