using FluentValidation;
using Nuna.Lib.AutoNumberHelper;
using Nuna.Lib.CleanArchHelper;
using Nuna.Lib.DataTypeExtension;
using Nuna.Lib.TransactionHelper;
using Ofta.Application.DocContext.BlueprintAgg.Contracts;
using Ofta.Domain.DocContext.BlueprintAgg;
using Ofta.Domain.DocContext.BundleSpecAgg;

namespace Ofta.Application.DocContext.BlueprintAgg.Workers;

public interface IBlueprintWriter : 
    INunaWriterWithReturn<BlueprintModel>,
    INunaWriterDelete<IBlueprintKey>
{
}

public class BlueprintWriter : IBlueprintWriter
{
    private readonly IBlueprintDal _blueprintDal;
    private readonly IBlueprintDocTypeDal _blueprintDocTypeDal;
    private readonly IBlueprintSigneeDal _blueprintSigneeDal;
    private readonly IValidator<BlueprintModel> _validator;
    private readonly INunaCounterBL _counter;

    public BlueprintWriter(IBlueprintDal blueprintDal, 
        IBlueprintDocTypeDal blueprintDocTypeDal, 
        IBlueprintSigneeDal blueprintSigneeDal, 
        IValidator<BlueprintModel> validator, 
        INunaCounterBL counter)
    {
        _blueprintDal = blueprintDal;
        _blueprintDocTypeDal = blueprintDocTypeDal;
        _blueprintSigneeDal = blueprintSigneeDal;
        _validator = validator;
        _counter = counter;
    }

    public BlueprintModel Save(BlueprintModel model)
    {
        //  VALIDATE
        var valResult = _validator.Validate(model);
        if (!valResult.IsValid)
            throw new ValidationException(valResult.Errors);
        
        //  GENERATE-KEY
        GenerateKey(ref model);
        
        //  WRITE
        var db = _blueprintDal.GetData(model);
        var allSignee = model.ListDocType.SelectMany(x => x.ListSignee).ToList();

        using var trans = TransHelper.NewScope();

        if (db is null)
            _blueprintDal.Insert(model);
        else
            _blueprintDal.Update(model);

        _blueprintDocTypeDal.Delete(model);
        _blueprintSigneeDal.Delete(model);
        _blueprintDocTypeDal.Insert(model.ListDocType);
        _blueprintSigneeDal.Insert(allSignee);
        
        trans.Complete();
        
        return model;        
    }

    private void GenerateKey(ref BlueprintModel model)
    {
        var id = model.BlueprintId;
        if (id.IsNullOrEmpty())
            id = _counter.Generate("BP", IDFormatEnum.PFnnn);
        model.BlueprintId = id;
        
        model.ListDocType
            .ForEach(x =>
            {
                x.BlueprintId = id;
                x.BlueprintDocTypeId = $"{x.BlueprintId}-{x.NoUrut:D2}";
            });
        model.ListDocType
            .ForEach(x => x.ListSignee
                .ForEach(y =>
                {
                    y.BlueprintId = id;
                    y.BlueprintDocTypeId = x.BlueprintDocTypeId;
                    y.BlueprintSigneeId = $"{y.BlueprintDocTypeId}-{y.NoUrut:D2}";
                }));
    }

    public void Delete(IBlueprintKey key)
    {
        using var trans = TransHelper.NewScope();
        
        _blueprintDal.Delete(key);
        _blueprintDocTypeDal.Delete(key);
        _blueprintSigneeDal.Delete(key);
        trans.Complete();
    }
}