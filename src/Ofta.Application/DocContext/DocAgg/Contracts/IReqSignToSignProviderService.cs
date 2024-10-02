using iTextSharp.text;
using Nuna.Lib.CleanArchHelper;
using Ofta.Domain.DocContext.DocAgg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ofta.Application.DocContext.DocAgg.Contracts;


public record ReqSignToSignProviderRequest(DocModel doc, String DocIdTilaka);

public class ReqSignToSignProviderResponse
{
    public List<DocSigneeModel> Signees { get; set; } = new List<DocSigneeModel>();
}
public interface IReqSignToSignProviderService
    : INunaService<ReqSignToSignProviderResponse, ReqSignToSignProviderRequest>
{
}