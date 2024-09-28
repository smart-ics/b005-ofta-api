using FluentValidation;
using Nuna.Lib.AutoNumberHelper;
using Nuna.Lib.CleanArchHelper;
using Nuna.Lib.DataTypeExtension;
using Ofta.Application.UserContext.UserOftaAgg.Contracts;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.UserContext.UserOftaAgg.Workers;

public interface IUserWriter : INunaWriterWithReturn<UserOftaModel>
{
}
public class UserWriter : IUserWriter
{
    private readonly IUserOftaDal _userDal;
    private readonly IUserOftaMappingDal _userOftaMappingDal;
    private readonly IValidator<UserOftaModel> _validator;
    private readonly INunaCounterBL _counterBL;

    public UserWriter(IUserOftaDal userDal,
        IUserOftaMappingDal userOftaMappingDal,
        IValidator<UserOftaModel> validator, 
        INunaCounterBL counterBL)
    {
        _userDal = userDal;
        _userOftaMappingDal = userOftaMappingDal;
        _validator = validator;
        _counterBL = counterBL;
    }

    public UserOftaModel Save(UserOftaModel oftaModel)
    {
        _validator.ValidateAndThrow(oftaModel);
        oftaModel.UserOftaId = oftaModel.UserOftaId.IsNullOrEmpty()?
            _counterBL.Generate("USER", IDFormatEnum.PREFYYMnnnnnC): 
            oftaModel.UserOftaId;

        var db = _userDal.GetData(oftaModel);
        if (db is null)
            _userDal.Insert(oftaModel);
        else
            _userDal.Update(oftaModel);
        
        _userOftaMappingDal.Delete(oftaModel);
        _userOftaMappingDal.Insert(oftaModel.ListUserMapping);
        
        return oftaModel;
    }
}