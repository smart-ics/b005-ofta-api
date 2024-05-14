using FluentValidation;
using Nuna.Lib.CleanArchHelper;
using Ofta.Application.KlaimBpjsContext.WorkListBpjsAgg.Contracts;
using Ofta.Domain.KlaimBpjsContext.WorkListBpjsAgg;


namespace Ofta.Application.KlaimBpjsContext.WorkListBpjsAgg.Workers;

public interface IWorkListBpjsWriter : INunaWriterWithReturn<WorkListBpjsModel>
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
        if (db is null)
            _workListBpjsDal.Insert(model);
        else
            _workListBpjsDal.Update(model);
        return model;
    }
}
