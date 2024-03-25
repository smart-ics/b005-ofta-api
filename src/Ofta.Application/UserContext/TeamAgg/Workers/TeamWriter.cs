using FluentValidation;
using Nuna.Lib.AutoNumberHelper;
using Nuna.Lib.CleanArchHelper;
using Nuna.Lib.DataTypeExtension;
using Nuna.Lib.TransactionHelper;
using Ofta.Application.UserContext.TeamAgg.Contracts;
using Ofta.Domain.UserContext.TeamAgg;

namespace Ofta.Application.UserContext.TeamAgg.Workers;

public interface ITeamWriter : INunaWriterWithReturn<TeamModel>
{
}
public class TeamWriter : ITeamWriter
{
    private readonly ITeamDal _teamDal;
    private readonly ITeamUserOftaDal _teamUserOftaDal;
    private readonly INunaCounterBL _counter;
    private readonly IValidator<TeamModel> _validator;

    public TeamWriter(ITeamDal teamDal, 
        INunaCounterBL counter, 
        IValidator<TeamModel> validator, 
        ITeamUserOftaDal teamUserOftaDal)
    {
        _teamDal = teamDal;
        _counter = counter;
        _validator = validator;
        _teamUserOftaDal = teamUserOftaDal;
    }

    public TeamModel Save(TeamModel model)
    {
        //  VALIDATE
        var validationResult = _validator.Validate(model);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        //  GENERATE KEY
        if (model.TeamId.IsNullOrEmpty())
            model.TeamId = _counter.Generate("TM", IDFormatEnum.PFnnn);
        model.ListUserOfta.ForEach(x => x.TeamId = model.TeamId);
        
        //  WRITE-DB
        var db = _teamDal.GetData(model);

        using var trans = TransHelper.NewScope();
        if (db is null)
            _teamDal.Insert(model);
        else
            _teamDal.Update(model);

        _teamUserOftaDal.Delete(model);
        _teamUserOftaDal.Insert(model.ListUserOfta);
        trans.Complete();
        
        return model;
    }
}