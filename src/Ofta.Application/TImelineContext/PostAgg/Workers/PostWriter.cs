using FluentValidation;
using Nuna.Lib.AutoNumberHelper;
using Nuna.Lib.CleanArchHelper;
using Nuna.Lib.DataTypeExtension;
using Nuna.Lib.TransactionHelper;
using Ofta.Application.TImelineContext.PostAgg.Contracts;
using Ofta.Domain.TImelineContext.PostAgg;

namespace Ofta.Application.TImelineContext.PostAgg.Workers;

public interface IPostWriter : INunaWriterWithReturn<PostModel>
{
}
public class PostWriter : IPostWriter
{
    private readonly IPostDal _postDal;
    private readonly IPostReactDal _postReactDal;
    private readonly IPostVisibilityDal _postVisibilityDal;
    private readonly INunaCounterBL _counter;
    private readonly IValidator<PostModel> _validator;

    public PostWriter(IPostDal postDal, 
        IPostReactDal postReactDal, 
        IPostVisibilityDal postVisibilityDal, 
        INunaCounterBL counter, 
        IValidator<PostModel> validator)
    {
        _postDal = postDal;
        _postReactDal = postReactDal;
        _postVisibilityDal = postVisibilityDal;
        _counter = counter;
        _validator = validator;
    }

    public PostModel Save(PostModel model)
    {
        //  VALIDATE
        var validateResult = _validator.Validate(model);
        if (!validateResult.IsValid)
            throw new ValidationException(validateResult.Errors);
        
        //  GENERATE-KEY
        if (model.PostId.IsNullOrEmpty())
            model.PostId = _counter.Generate("POST", IDFormatEnum.PREFYYMnnnnnC);
        model.ListVisibility.ForEach(x => x.PostId = model.PostId);
        model.ListReact.ForEach(x => x.PostId = model.PostId);
        
        //  SAVE
        using var trans = TransHelper.NewScope();
        var db = _postDal.GetData(model);
        if (db is null)
            _postDal.Insert(model);
        else
            _postDal.Update(model);

        _postVisibilityDal.Delete(model);
        _postVisibilityDal.Insert(model.ListVisibility);

        _postReactDal.Delete(model);
        _postReactDal.Insert(model.ListReact);
        
        trans.Complete();
        return model;
    }
} 