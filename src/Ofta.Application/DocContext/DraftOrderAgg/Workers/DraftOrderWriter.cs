using FluentValidation;
using Nuna.Lib.AutoNumberHelper;
using Nuna.Lib.CleanArchHelper;
using Nuna.Lib.DataTypeExtension;
using Ofta.Domain.DocContext.DraftOrderAgg;

namespace Ofta.Application.DocContext.DraftOrderAgg.Workers;

public interface IDraftOrderWriter: INunaWriterWithReturn<DraftOrderModel>{}
public class DraftOrderWriter: IDraftOrderWriter
{
    private readonly IDraftOrderDal _draftOrderDal;
    private readonly IValidator<DraftOrderModel> _validator;
    private readonly INunaCounterBL _counter;

    public DraftOrderWriter(IDraftOrderDal draftOrderDal, IValidator<DraftOrderModel> validator, INunaCounterBL counter)
    {
        _draftOrderDal = draftOrderDal;
        _validator = validator;
        _counter = counter;
    }

    public DraftOrderModel Save(DraftOrderModel model)
    {
        _validator.ValidateAndThrow(model);
        model.DraftOrderId = model.DraftOrderId.IsNullOrEmpty()
            ? _counter.Generate("DRAF", IDFormatEnum.PREFYYMnnnnnC)
            : model.DraftOrderId;

        var db = _draftOrderDal.GetData(model);
        if (db is null) 
            _draftOrderDal.Insert(model);
        else
            _draftOrderDal.Update(model);

        return model;
    }
}