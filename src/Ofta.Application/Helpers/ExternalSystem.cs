using Ofta.Domain.DocContext.DocTypeAgg;
using Ofta.Domain.UserContext.UserOftaAgg;

namespace Ofta.Application.Helpers;

public class ExternalSystemHelper
{
    public static UserTypeEnum GetDestination(IDocTypeKey docTypeKey)
        => docTypeKey.DocTypeId switch
        {
            "DTX01" => UserTypeEnum.HospitalWeb,
            "DTX02" => UserTypeEnum.HospitalWeb,
            "DTX03" => UserTypeEnum.HospitalWeb,
            "DTX04" => UserTypeEnum.HospitalWeb,
            "DTX05" => UserTypeEnum.EMR,
            "DTX06" => UserTypeEnum.HospitalWeb,
            "DTX07" => UserTypeEnum.HospitalWeb,
            "DTX08" => UserTypeEnum.HospitalWeb,
            "DTX09" => UserTypeEnum.HospitalWeb,
            "DTX0A" => UserTypeEnum.HospitalWeb,
            "DTX0B" => UserTypeEnum.HospitalWeb,
            "DTX0C" => UserTypeEnum.EMR,
            "DTX0D" => UserTypeEnum.HospitalWeb,
            "DTX0E" => UserTypeEnum.EMR,
            "DTX0F" => UserTypeEnum.EMR,
            "DTX10" => UserTypeEnum.EMR,
            "DTX11" => UserTypeEnum.EMR,
            "DTX12" => UserTypeEnum.EMR,
            "DTX13" => UserTypeEnum.EMR,
            _ => UserTypeEnum.EMR
        };
}