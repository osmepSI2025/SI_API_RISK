using System;
using System.Collections.Generic;

namespace SME_API_RISK.Entities;

public partial class TRiskimpact
{
    public int RiskDefineId { get; set; }

    public string Impacts { get; set; } = null!;

    public DateTime? UpdateDate { get; set; }
}
