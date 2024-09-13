using Nuna.Lib.CleanArchHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;

public class AssesmentDto
{
    public string AssesmentId { get; set; }
    public string PaperId { get; set; }
}
public interface IListAssesmentService : INunaService<IEnumerable<AssesmentDto>, string>
{
}
