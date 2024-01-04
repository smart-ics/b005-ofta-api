using FluentValidation;
using Nuna.Lib.AutoNumberHelper;
using Nuna.Lib.CleanArchHelper;
using Nuna.Lib.DataTypeExtension;
using Nuna.Lib.TransactionHelper;
using Ofta.Application.DocContext.DocTypeAgg.Contracts;
using Ofta.Domain.DocContext.DocTypeAgg;

namespace Ofta.Application.DocContext.DocTypeAgg.Workers;

public interface IDocTypeWriter : INunaWriterWithReturn<DocTypeModel>
{
}
public class DocTypeWriter : IDocTypeWriter
{
    private readonly IDocTypeDal _docTypeDal;
    private readonly IDocTypeTagDal _docTypeTagDal;
    private readonly IValidator<DocTypeModel> _validator;
    private readonly INunaCounterBL _counter;

    public DocTypeWriter(IDocTypeDal docTypeDal, 
        IDocTypeTagDal docTypeTagDal, 
        IValidator<DocTypeModel> validator, 
        INunaCounterBL counter)
    {
        _docTypeDal = docTypeDal;
        _docTypeTagDal = docTypeTagDal;
        _validator = validator;
        _counter = counter;
    }

    public DocTypeModel Save(DocTypeModel model)
    {
        //  VALIDATE
        _validator.ValidateAndThrow(model);
        
        //  GEN-ID
        if (model.DocTypeId.IsNullOrEmpty())
            model.DocTypeId = _counter.Generate("DT", IDFormatEnum.PFnnn);
        model.ListTag.ForEach(x => x.DocTypeId = model.DocTypeId);
        
        //  WRITE-TO-DB
        var db = _docTypeDal.GetData(model);

        using var trans = TransHelper.NewScope();

        if (db is null)
            _docTypeDal.Insert(model);
        else
            _docTypeDal.Update(model);

        _docTypeTagDal.Delete(model);
        _docTypeTagDal.Insert(model.ListTag);

        trans.Complete();
        
        return model;
    }
}