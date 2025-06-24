using System;
using System.Collections.Generic;

namespace SME_API_RISK.Entities;

public partial class TRiskKpi
{
    public int RiskDefineId { get; set; }

    public string? Kpis { get; set; }

    public DateTime? UpdateDate { get; set; }
}
