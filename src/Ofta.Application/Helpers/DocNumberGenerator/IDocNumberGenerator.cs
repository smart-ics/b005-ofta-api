using Ofta.Domain.DocContext.DocTypeAgg;

namespace Ofta.Application.Helpers.DocNumberGenerator;

public interface IDocNumberGenerator
{
    string Generate(IDocTypeKey docType, DateTime dateCreated);
    string FormattingNumber(string numberFormat, int numberValue, DateTime dateCreated);
}