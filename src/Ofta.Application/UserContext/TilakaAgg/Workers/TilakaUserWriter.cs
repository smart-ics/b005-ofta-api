using Nuna.Lib.CleanArchHelper;
using Ofta.Application.UserContext.TilakaAgg.Contracts;
using Ofta.Domain.UserContext.TilakaAgg;

namespace Ofta.Application.UserContext.TilakaAgg.Workers;

public interface ITilakaUserWriter : INunaWriterWithReturn<TilakaUserModel>{}

public class TilakaUserWriter: ITilakaUserWriter
{
    private readonly ITilakaUserDal _tilakaUserDal;

    public TilakaUserWriter(ITilakaUserDal tilakaUserDal)
    {
        _tilakaUserDal = tilakaUserDal;
    }

    public TilakaUserModel Save(TilakaUserModel model)
    {
        var db = _tilakaUserDal.GetData(model);
        if (db is null)
            _tilakaUserDal.Insert(model);
        else
            _tilakaUserDal.Update(model);

        return model;
    }
}