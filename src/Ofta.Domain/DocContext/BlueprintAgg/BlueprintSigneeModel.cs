﻿using Ofta.Domain.DocContext.BundleSpecAgg;
using Ofta.Domain.DocContext.DocAgg;
using Ofta.Domain.UserOftaContext;

namespace Ofta.Domain.DocContext.BlueprintAgg;

public class BlueprintSigneeModel : IBlueprintKey, IUserOftaKey 
{
    public string BlueprintId { get; set; }
    public string BlueprintDocTypeId { get; set; }
    public string BlueprintSigneeId { get; set; }
    public int NoUrut { get; set; }
    public string UserOftaId { get; set; }
    public string Email { get; set; }
    public string SignTag { get; set; }
    public SignPositionEnum SignPosition { get; set; }
}