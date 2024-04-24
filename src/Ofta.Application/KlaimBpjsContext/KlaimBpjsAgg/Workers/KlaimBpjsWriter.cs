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
    private readonly IKlaimBpjsDocTypeDal _klaimBpjsDocTypeDal;
    private readonly IKlaimBpjsPrintOutDal _klaimBpjsPrintDal;
    private readonly IKlaimBpjsSigneeDal _klaimBpjsSigneeDal;
    private readonly IKlaimBpjsEventDal _klaimBpjsJurnalDal;
    private readonly INunaCounterBL _counter;
    private readonly IValidator<KlaimBpjsModel> _validator;

    public KlaimBpjsWriter(IKlaimBpjsDal klaimBpjsDal,
        IKlaimBpjsDocTypeDal klaimBpjsDocTypeDal,
        IKlaimBpjsPrintOutDal klaimBpjsPrintDal,
        IKlaimBpjsSigneeDal klaimBpjsSigneeDal,
        IKlaimBpjsEventDal klaimBpjsJurnalDal, 
        INunaCounterBL counter, 
        IValidator<KlaimBpjsModel> validator)
    {
        _klaimBpjsDal = klaimBpjsDal;
        _klaimBpjsDocTypeDal = klaimBpjsDocTypeDal;
        _klaimBpjsPrintDal = klaimBpjsPrintDal;
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
        model.ListDocType.ForEach(x =>
        {
            x.KlaimBpjsId = model.KlaimBpjsId;
            x.KlaimBpjsDocTypeId = $"{model.KlaimBpjsId}-{x.NoUrut:D2}";
            x.ListPrintOut.ForEach(y =>
            {
                y.KlaimBpjsId = model.KlaimBpjsId;
                y.KlaimBpjsDocTypeId = x.KlaimBpjsDocTypeId;
                y.KlaimBpjsPrintOutId = $"{model.KlaimBpjsId}-{x.NoUrut:D2}-{y.NoUrut:D2}";
                y.ListSign.ForEach(z =>
                {
                    z.KlaimBpjsId = model.KlaimBpjsId;
                    z.KlaimBpjsDocTypeId = x.KlaimBpjsDocTypeId;
                    z.KlaimBpjsPrintOutId = y.KlaimBpjsPrintOutId;
                    z.KlaimBpjsSigneeId = $"{model.KlaimBpjsId}-{x.NoUrut:D2}-{y.NoUrut:D2}-{z.NoUrut:D2}";
                });
            });
        });
        model.ListEvent.ForEach(x =>
        {
            x.KlaimBpjsId = model.KlaimBpjsId;
            x.KlaimBpjsJurnalId = $"{model.KlaimBpjsId}-{x.NoUrut:D2}";
        });
        
        var allPrint = model.ListDocType.SelectMany(x => x.ListPrintOut).ToList();
        var allSignee = allPrint.SelectMany(x => x.ListSign).ToList();
        
        //  WRITE
        using var trans = TransHelper.NewScope();
        
        var db = _klaimBpjsDal.GetData(model);
        if (db is null)
            _klaimBpjsDal.Insert(model);
        else
            _klaimBpjsDal.Update(model);

        _klaimBpjsDocTypeDal.Delete(model);
        _klaimBpjsDocTypeDal.Insert(model.ListDocType);
        
        _klaimBpjsPrintDal.Delete(model);
        _klaimBpjsPrintDal.Insert(allPrint);

        _klaimBpjsSigneeDal.Delete(model);
        _klaimBpjsSigneeDal.Insert(allSignee);

        _klaimBpjsJurnalDal.Delete(model);
        _klaimBpjsJurnalDal.Insert(model.ListEvent);
        
        trans.Complete();
        return model;
    }
}