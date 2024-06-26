﻿using Nuna.Lib.DataAccessHelper;
using Ofta.Domain.KlaimBpjsContext.KlaimBpjsAgg;

namespace Ofta.Application.DocContext.BundleAgg.Contracts;

public interface IBundleDocDal :
    IInsertBulk<KlaimBpjsDocTypeModel>,
    IDelete<IKlaimBpjsKey>,
    IListData<KlaimBpjsDocTypeModel, IKlaimBpjsKey>
{
}