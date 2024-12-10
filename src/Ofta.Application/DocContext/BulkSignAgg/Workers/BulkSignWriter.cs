using FluentValidation;
using Nuna.Lib.AutoNumberHelper;
using Nuna.Lib.CleanArchHelper;
using Nuna.Lib.DataTypeExtension;
using Nuna.Lib.TransactionHelper;
using Ofta.Application.DocContext.BulkSignAgg.Contracts;
using Ofta.Domain.DocContext.BulkSignAgg;

namespace Ofta.Application.DocContext.BulkSignAgg.Workers;

public interface IBulkSignWriter : INunaWriterWithReturn<BulkSignModel>
{
}

public class BulkSignWriter: IBulkSignWriter
{
    private readonly IBulkSignDal _bulkSignDal;
    private readonly IBulkSignDocDal _bulkSignDocDal;
    private readonly IValidator<BulkSignModel> _validator;
    private readonly INunaCounterBL _counter;

    public BulkSignWriter(IBulkSignDal bulkSignDal, IBulkSignDocDal bulkSignDocDal, IValidator<BulkSignModel> validator, INunaCounterBL counter)
    {
        _bulkSignDal = bulkSignDal;
        _bulkSignDocDal = bulkSignDocDal;
        _validator = validator;
        _counter = counter;
    }

    public BulkSignModel Save(BulkSignModel model)
    {
        _validator.ValidateAndThrow(model);
        model.BulkSignId = model.BulkSignId.IsNullOrEmpty()
            ? _counter.Generate("BULK", IDFormatEnum.PREFYYMnnnnnC)
            : model.BulkSignId;
        
        model.SyncId();

        var db = _bulkSignDal.GetData(model);

        using var trans = TransHelper.NewScope();
        if (db is null)
            _bulkSignDal.Insert(model);
        else
            _bulkSignDal.Update(model);
        
        _bulkSignDocDal.Delete(model);
        _bulkSignDocDal.Insert(model.ListDoc);
        
        trans.Complete();
        return model;
    }
}