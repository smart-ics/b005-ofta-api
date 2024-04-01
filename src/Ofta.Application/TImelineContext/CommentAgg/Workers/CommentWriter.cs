using System.Net.Http.Headers;
using FluentValidation;
using Nuna.Lib.AutoNumberHelper;
using Nuna.Lib.CleanArchHelper;
using Nuna.Lib.DataTypeExtension;
using Nuna.Lib.TransactionHelper;
using Ofta.Application.TImelineContext.CommentAgg.Contracts;
using Ofta.Domain.TImelineContext.CommentAgg;

namespace Ofta.Application.TImelineContext.CommentAgg.Workers;

public interface ICommentWriter : 
    INunaWriterWithReturn<CommentModel>
{
    void Delete(CommentModel model);
}
public class CommentWriter : ICommentWriter
{
    private readonly ICommentDal _commentDal;
    private readonly ICommentReactDal _commentReactDal;
    private readonly INunaCounterBL _counter;
    private readonly IValidator<CommentModel> _validator;

    public CommentWriter(ICommentDal commentDal, 
        ICommentReactDal commentReactDal, 
        INunaCounterBL counter, 
        IValidator<CommentModel> validator)
    {
        _commentDal = commentDal;
        _commentReactDal = commentReactDal;
        _counter = counter;
        _validator = validator;
    }

    public CommentModel Save(CommentModel model)
    {
        //  VALIDATE
        var validateResult = _validator.Validate(model);
        if (!validateResult.IsValid)
            throw new ValidationException(validateResult.Errors);
        
        //  GENERATE-KEY
        if (model.CommentId.IsNullOrEmpty())
            model.CommentId = _counter.Generate("COMN", IDFormatEnum.PREFYYMnnnnnC);
        model.ListReact.ForEach(x => x.CommentId = model.CommentId);
        
        //  SAVE
        using var trans = TransHelper.NewScope();
        var db = _commentDal.GetData(model);
        if (db is null)
            _commentDal.Insert(model);
        else
            _commentDal.Update(model);

        _commentReactDal.Delete(model);
        _commentReactDal.Insert(model.ListReact);
        trans.Complete();
        return model;
    }

    public void Delete(CommentModel model)
    {
        using var trans = TransHelper.NewScope();
        _commentDal.Delete(model);
        _commentReactDal.Delete(model);
        trans.Complete();
    }
}