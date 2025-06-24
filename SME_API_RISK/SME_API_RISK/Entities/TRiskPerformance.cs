using System;
using System.Collections.Generic;

namespace SME_API_RISK.Entities;

public partial class TRiskPerformance
{
    public int RiskDefineId { get; set; }

    public int Quarter { get; set; }

    public string Performances { get; set; } = null!;

    public DateTime? UpdateDate { get; set; }
}
