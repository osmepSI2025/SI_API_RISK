using System;
using System.Collections.Generic;

namespace SME_API_RISK.Entities;

public partial class TRiskPerformance
{
    public int Id { get; set; }

    public int RiskDefineId { get; set; }

    public int Quarter { get; set; }

    public string? Performances { get; set; }

    public DateTime? UpdateDate { get; set; }
}
