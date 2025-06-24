using System;
using System.Collections.Generic;

namespace SME_API_RISK.Entities;

public partial class TRiskTrigger
{
    public int RiskDefineId { get; set; }

    public string Triggers { get; set; } = null!;

    public DateTime? UpdateDate { get; set; }
}
