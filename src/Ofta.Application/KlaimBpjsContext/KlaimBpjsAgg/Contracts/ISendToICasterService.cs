using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.PrintOutContext.ICasterAgg;


namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;

    public interface ISendToICasterService :
        IRequestResponseService<ICasterModel, bool>,
        IRequestResponseService<ICasterEmrModel, bool>
    {
    }
