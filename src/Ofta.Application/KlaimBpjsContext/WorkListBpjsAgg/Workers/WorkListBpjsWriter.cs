using FluentValidation;
using Nuna.Lib.CleanArchHelper;
using Nuna.Lib.TransactionHelper;
using Ofta.Application.KlaimBpjsContext.WorkListBpjsAgg.Contracts;
using Ofta.Domain.KlaimBpjsContext.BlueprintAgg;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;
using Ofta.Domain.KlaimBpjsContext.OrderKlaimBpjsAgg;
using Ofta.Domain.KlaimBpjsContext.WorkListBpjsAgg;


namespace Ofta.Application.KlaimBpjsContext.WorkListBpjsAgg.Workers;

public interface IWorkListBpjsWriter : INunaWriterWithReturn<WorkListBpjsModel>, INunaWriterDelete<IOrderKlaimBpjsKey>
{
}

public class WorkListBpjsWriter : IWorkListBpjsWriter
{
    private readonly IWorkListBpjsDal _workListBpjsDal;
    private readonly IValidator<WorkListBpjsModel> _validator;

    public WorkListBpjsWriter(IWorkListBpjsDal workListBpjsDal,
           IValidator<WorkListBpjsModel> validator)
    {
        _workListBpjsDal = workListBpjsDal;
        _validator = validator;
    }

    public WorkListBpjsModel Save(WorkListBpjsModel model)
    {
        //  VALIDATE
        var validationResult = _validator.Validate(model);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        //  WRITE
        var db = _workListBpjsDal.GetData(model);

        using var trans = TransHelper.NewScope();

        if (db is null)
            _workListBpjsDal.Insert(model);
        else
            _workListBpjsDal.Update(model);

        trans.Complete();
        return model;
    }

    public void Delete(IOrderKlaimBpjsKey key)
    {
        using var trans = TransHelper.NewScope();
        _workListBpjsDal.Delete(key);
        trans.Complete();
    }
}
