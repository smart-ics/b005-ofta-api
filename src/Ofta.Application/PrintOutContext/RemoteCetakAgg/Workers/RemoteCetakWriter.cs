using FluentValidation;
using Nuna.Lib.CleanArchHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.KlaimBpjsContext.WorkListBpjsAgg.Contracts;
using Ofta.Application.PrintOutContext.RemoteCetakAgg.Contracts;
using Ofta.Domain.KlaimBpjsContext.WorkListBpjsAgg;
using Ofta.Domain.PrintOutContext.RemoteCetakAgg;

namespace Ofta.Application.PrintOutContext.RemoteCetakAgg.Workers;

public interface IRemoteCetakWriter : INunaWriterWithReturn<RemoteCetakModel>
{
}

public class RemoteCetakWriter : IRemoteCetakWriter
{
    private readonly IRemoteCetakDal _remoteCetakDal;
    private readonly IValidator<RemoteCetakModel> _validator;

    public RemoteCetakWriter(IRemoteCetakDal remoteCetakDal, 
        IValidator<RemoteCetakModel> validator)
    {
        _remoteCetakDal = remoteCetakDal;
        _validator = validator;
    }

    public RemoteCetakModel Save(RemoteCetakModel model)
    {
        //  VALIDATE
        var validationResult = _validator.Validate(model);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        //  SAVE
        var db = _remoteCetakDal.ListData(model) ?.ToList()
                 ?? new List<RemoteCetakModel>();

        var filterDb = db.Where(d => d.JenisDoc == model.JenisDoc &&
                                     d.RemoteAddr == model.RemoteAddr);

        if (!filterDb.Any())
            _remoteCetakDal.Insert(model);
        else
            _remoteCetakDal.Update(model);
        return model;
    }
}

public class RemoveCetakValidator : AbstractValidator<RemoteCetakModel>
{
    public RemoveCetakValidator()
    {
        RuleFor(x => x.KodeTrs)
            .NotEmpty();
        
        RuleFor(x => x.JenisDoc)
            .NotEmpty();
        
        RuleFor(x => x.TglSend)
            .NotEmpty()
            .Must(x => x.IsValidTgl("yyyy-MM-dd"));

        RuleFor(x => x.JamSend)
            .NotEmpty()
            .Must(x => x.IsValidJam("HH:mm:ss"));
        
        RuleFor(x => x.RemoteAddr)
            .NotEmpty();
    }
}