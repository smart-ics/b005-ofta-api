using FluentValidation;
using Nuna.Lib.AutoNumberHelper;
using Nuna.Lib.CleanArchHelper;
using Nuna.Lib.DataTypeExtension;
using Nuna.Lib.TransactionHelper;
using Ofta.Application.DocContext.DocAgg.Contracts;
using Ofta.Domain.DocContext.DocAgg;

namespace Ofta.Application.DocContext.DocAgg.Workers;

public interface IDocWriter : INunaWriterWithReturn<DocModel>
{
}
public class DocWriter : IDocWriter
{
    private readonly IDocDal _docDal;
    private readonly IDocSigneeDal _docSigneeDal;
    private readonly IDocJurnalDal _docJurnalDal;
    private readonly IDocScopeDal _docScopeDal;
    private readonly IValidator<DocModel> _validator;
    private readonly INunaCounterBL _counter;

    public DocWriter(IDocDal docDal, 
        IDocSigneeDal docSigneeDal, 
        IDocJurnalDal docJurnalDal, 
        IValidator<DocModel> validator, 
        INunaCounterBL counter, 
        IDocScopeDal docScopeDal)
    {
        _docDal = docDal;
        _docSigneeDal = docSigneeDal;
        _docJurnalDal = docJurnalDal;
        _validator = validator;
        _counter = counter;
        _docScopeDal = docScopeDal;
    }

    public DocModel Save(DocModel model)
    {
        _validator.ValidateAndThrow(model);
        model.DocId = model.DocId.IsNullOrEmpty() 
            ? _counter.Generate("DOCU", IDFormatEnum.PREFYYMnnnnnC) 
            : model.DocId;
        model.ListJurnal.ForEach(x => x.DocId = model.DocId);
        model.ListScope.ForEach(x => x.DocId = model.DocId);
        model.ListSignees = model.ListSignees.Select((signee, index) =>
        {
            signee.DocId = model.DocId;
            signee.DocSigneeId = $"{model.DocId}-{(index + 1):D2}";
            return signee;
        }).ToList();
        
        var db = _docDal.GetData((IDocKey)model);

        using var trans = TransHelper.NewScope();
        if (db is null)
            _docDal.Insert(model);
        else
            _docDal.Update(model);
        
        _docSigneeDal.Delete(model);
        _docSigneeDal.Insert(model.ListSignees);
        
        _docJurnalDal.Delete(model);
        _docJurnalDal.Insert(model.ListJurnal);

        _docScopeDal.Delete(model);
        _docScopeDal.Insert(model.ListScope);
        
        trans.Complete();
        return model;
    }
}