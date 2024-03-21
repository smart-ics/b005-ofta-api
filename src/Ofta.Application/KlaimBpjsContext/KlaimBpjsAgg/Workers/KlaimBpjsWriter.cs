using FluentValidation;
using Nuna.Lib.AutoNumberHelper;
using Nuna.Lib.CleanArchHelper;
using Nuna.Lib.DataTypeExtension;
using Nuna.Lib.TransactionHelper;
using Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Contracts;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

namespace Ofta.Application.KlaimBpjsContext.KlaimBpjsAgg.Workers;

public interface IKlaimBpjsWriter : INunaWriterWithReturn<KlaimBpjsModel>
{
}
public class KlaimBpjsWriter : IKlaimBpjsWriter
{
    private readonly IKlaimBpjsDal _klaimBpjsDal;
    private readonly IKlaimBpjsDocDal _klaimBpjsDocDal;
    private readonly IKlaimBpjsSigneeDal _klaimBpjsSigneeDal;
    private readonly IKlaimBpjsEventDal _klaimBpjsJurnalDal;
    private readonly INunaCounterBL _counter;
    private readonly IValidator<KlaimBpjsModel> _validator;

    public KlaimBpjsWriter(IKlaimBpjsDal klaimBpjsDal,
        IKlaimBpjsDocDal klaimBpjsDocDal,
        IKlaimBpjsSigneeDal klaimBpjsSigneeDal,
        IKlaimBpjsEventDal klaimBpjsJurnalDal,
        INunaCounterBL counter, 
        IValidator<KlaimBpjsModel> validator)
    {
        _klaimBpjsDal = klaimBpjsDal;
        _klaimBpjsDocDal = klaimBpjsDocDal;
        _klaimBpjsSigneeDal = klaimBpjsSigneeDal;
        _klaimBpjsJurnalDal = klaimBpjsJurnalDal;
        _counter = counter;
        _validator = validator;
    }

    public KlaimBpjsModel Save(KlaimBpjsModel model)
    {
        //  VALIDATE
        var validationResult = _validator.Validate(model);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        //  GENERATE-ID
        if (model.KlaimBpjsId.IsNullOrEmpty())
            model.KlaimBpjsId = _counter.Generate("KLBP", IDFormatEnum.PREFYYMnnnnnC);
        model.ListDoc.ForEach(x =>
        {
            x.KlaimBpjsId = model.KlaimBpjsId;
            x.KlaimBpjsDocId = $"{model.KlaimBpjsId}-{x.NoUrut:D2}";
            x.ListSign.ForEach(y =>
            {
                y.KlaimBpjsId = model.KlaimBpjsId;
                y.KlaimBpjsDocId = x.KlaimBpjsDocId;
                y.KlaimBpjsSigneeId = $"{model.KlaimBpjsId}-{x.NoUrut:D2}-{y.NoUrut:D2}";
            });
        });
        model.ListEvent.ForEach(x =>
        {
            x.KlaimBpjsId = model.KlaimBpjsId;
            x.KlaimBpjsJurnalId = $"{model.KlaimBpjsId}-{x.NoUrut:D2}";
        });
        
        var allSignee = model.ListDoc.SelectMany(x => x.ListSign).ToList();
        
        //  WRITE
        using var trans = TransHelper.NewScope();
        
        var db = _klaimBpjsDal.GetData(model);
        if (db is null)
            _klaimBpjsDal.Insert(model);
        else
            _klaimBpjsDal.Update(model);

        _klaimBpjsDocDal.Delete(model);
        _klaimBpjsDocDal.Insert(model.ListDoc);
        
        _klaimBpjsSigneeDal.Delete(model);
        _klaimBpjsSigneeDal.Insert(allSignee);

        _klaimBpjsJurnalDal.Delete(model);
        _klaimBpjsJurnalDal.Insert(model.ListEvent);
        
        trans.Complete();
        return model;
    }
}