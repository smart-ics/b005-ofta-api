using FluentValidation;
using Nuna.Lib.AutoNumberHelper;
using Nuna.Lib.CleanArchHelper;
using Nuna.Lib.DataTypeExtension;
using Ofta.Application.KlaimBpjsContext.OrderKlaimBpjsAgg.Contracts;
using Ofta.Domain.KlaimBpjsContext.OrderKlaimBpjsAgg;

namespace Ofta.Application.KlaimBpjsContext.OrderKlaimBpjsAgg.Workers;

public interface IOrderKlaimBpjsWriter : INunaWriterWithReturn<OrderKlaimBpjsModel>
{
}

public class OrderKlaimBpjsWriter : IOrderKlaimBpjsWriter
{
    private readonly IOrderKlaimBpjsDal _orderKlaimBpjsDal;
    private readonly IValidator<OrderKlaimBpjsModel> _validator;
    private readonly INunaCounterBL _counter;

    public OrderKlaimBpjsWriter(IOrderKlaimBpjsDal orderKlaimBpjsDal,
        IValidator<OrderKlaimBpjsModel> validator,
        INunaCounterBL counter)
    {
        _orderKlaimBpjsDal = orderKlaimBpjsDal;
        _validator = validator;
        _counter = counter;
    }

    public OrderKlaimBpjsModel Save(OrderKlaimBpjsModel model)
    {
        //  VALIDATE
        var validationResult = _validator.Validate(model);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        //  GENERATE-KEY
        if (model.OrderKlaimBpjsId.IsNullOrEmpty())
            model.OrderKlaimBpjsId = _counter.Generate(@"OKLB", IDFormatEnum.PREFYYMnnnnnC);
        
        //  WRITE
        var db = _orderKlaimBpjsDal.GetData(model);
        if (db is null)
            _orderKlaimBpjsDal.Insert(model);
        else
            _orderKlaimBpjsDal.Update(model);
        return model;
    }
}