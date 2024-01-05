using FluentValidation;
using Nuna.Lib.AutoNumberHelper;
using Nuna.Lib.CleanArchHelper;
using Nuna.Lib.DataTypeExtension;
using Ofta.Application.UserContext.Contracts;
using Ofta.Domain.UserContext;

namespace Ofta.Application.UserContext.Workers;

public interface IUserWriter : INunaWriterWithReturn<UserModel>
{
}
public class UserWriter : IUserWriter
{
    private readonly IUserDal _userDal;
    private readonly IValidator<UserModel> _validator;
    private readonly INunaCounterBL _counterBL;

    public UserWriter(IUserDal userDal, 
        IValidator<UserModel> validator, 
        INunaCounterBL counterBL)
    {
        _userDal = userDal;
        _validator = validator;
        _counterBL = counterBL;
    }

    public UserModel Save(UserModel model)
    {
        _validator.ValidateAndThrow(model);
        model.UserId = model.UserId.IsNullOrEmpty()?
            _counterBL.Generate("USER", IDFormatEnum.PREFYYMnnnnnC): 
            model.UserId;

        var db = _userDal.GetData(model);
        if (db is null)
            _userDal.Insert(model);
        else
            _userDal.Update(model);
        return model;
    }
}