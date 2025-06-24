using System;
using System.Collections.Generic;

namespace SME_API_RISK.Entities;

public partial class MRiskFactor
{
    public int RiskFactorId { get; set; }

    public int RiskDefineId { get; set; }

    public int RiskYear { get; set; }

    public string RiskRfname { get; set; } = null!;

    public int RiskTypeId { get; set; }

    public string RiskTypeName { get; set; } = null!;

    public string RiskRootCause { get; set; } = null!;

    public string RiskOwnerName { get; set; } = null!;

    public DateTime UpdateDate { get; set; }
}
