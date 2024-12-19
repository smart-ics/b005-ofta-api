using System.Text.Json;
using Nuna.Lib.CleanArchHelper;
using Nuna.Lib.ValidationHelper;
using Ofta.Application.CallbackContext.CallbackSignStatusAgg.Contracts;
using Ofta.Application.CallbackContext.CallbackSignStatusAgg.UseCases;
using Ofta.Application.Helpers;
using Ofta.Application.UserContext.TilakaAgg.Workers;
using Ofta.Domain.CallbackContext.CallbackSignStatusAgg;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.UserContext.TilakaAgg;

namespace Ofta.Application.CallbackContext.CallbackSignStatusAgg.Workers;

public interface ICallbackSignStatusBuilder : INunaBuilder<CallbackSignStatusModel>
{
    ICallbackSignStatusBuilder Create(string requestId);
    ICallbackSignStatusBuilder Attach(CallbackSignStatusModel model);
    ICallbackSignStatusBuilder Load(ICallbackSignStatusKey key);
    ICallbackSignStatusBuilder CallbackDate();
    ICallbackSignStatusBuilder UserInfo(ITilakaNameKey key);
    ICallbackSignStatusBuilder AddDocument(string docId, string downloadUrl, string signState);
    ICallbackSignStatusBuilder JsonPayload(ReceiveCallbackSignStatusCommand payload);
}

public class CallbackSignStatusBuilder: ICallbackSignStatusBuilder
{
    private CallbackSignStatusModel _aggregate;
    private readonly ITglJamDal _tglJamDal;
    private readonly ITilakaUserBuilder _tilakaUserBuilder;
    private readonly ICallbackSignStatusDal _callbackSignStatusDal;
    private readonly ICallbackSignStatusDocDal _callbackSignStatusDocDal;

    public CallbackSignStatusBuilder(ITglJamDal tglJamDal, ITilakaUserBuilder tilakaUserBuilder, ICallbackSignStatusDal callbackSignStatusDal, ICallbackSignStatusDocDal callbackSignStatusDocDal)
    {
        _tglJamDal = tglJamDal;
        _tilakaUserBuilder = tilakaUserBuilder;
        _callbackSignStatusDal = callbackSignStatusDal;
        _callbackSignStatusDocDal = callbackSignStatusDocDal;
    }

    public CallbackSignStatusModel Build()
    {
        _aggregate.RemoveNull();
        return _aggregate;
    }

    public ICallbackSignStatusBuilder Create(string requestId)
    {
        _aggregate = new CallbackSignStatusModel
        {
            RequestId = requestId,
            CallbackDate = _tglJamDal.Now,
            ListDoc = new List<CallbackSignStatusDocModel>()
        };

        return this;
    }

    public ICallbackSignStatusBuilder Attach(CallbackSignStatusModel model)
    {
        _aggregate = model;
        return this;
    }

    public ICallbackSignStatusBuilder Load(ICallbackSignStatusKey key)
    {
        _aggregate = _callbackSignStatusDal.GetData(key) 
            ?? throw new KeyNotFoundException("CallbackSignStatus not found");

        _aggregate.ListDoc = _callbackSignStatusDocDal.ListData(_aggregate).ToList();
        return this;
    }

    public ICallbackSignStatusBuilder CallbackDate()
    {
        _aggregate.CallbackDate = _tglJamDal.Now;
        return this;
    }

    public ICallbackSignStatusBuilder UserInfo(ITilakaNameKey key)
    {
        var tilakaUser = _tilakaUserBuilder
            .Load(key)
            .Build();

        _aggregate.UserOftaId = tilakaUser.UserOftaId;
        _aggregate.Email = tilakaUser.Email;
        _aggregate.TilakaName = tilakaUser.TilakaName;

        return this;
    }

    public ICallbackSignStatusBuilder AddDocument(string docId, string downloadUrl, string signState)
    {
        var newCallbackDoc = new CallbackSignStatusDocModel
        {
            RequestId = _aggregate.RequestId,
            TilakaName = _aggregate.TilakaName,
            UploadedDocId = docId,
            DownloadDocUrl = downloadUrl,
            UserSignState = SignStateEnum.NotSigned
        };

        if (signState == "DONE")
            newCallbackDoc.UserSignState = SignStateEnum.Signed;
        
        _aggregate.ListDoc.RemoveAll(x => x.UploadedDocId == docId);
        _aggregate.ListDoc.Add(newCallbackDoc);

        return this;
    }

    public ICallbackSignStatusBuilder JsonPayload(ReceiveCallbackSignStatusCommand payload)
    {
        _aggregate.JsonPayload = JsonSerializer.Serialize(payload);
        return this;
    }
}