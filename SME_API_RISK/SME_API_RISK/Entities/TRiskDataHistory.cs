using System;
using System.Collections.Generic;

namespace SME_API_RISK.Entities;

public partial class TRiskDataHistory
{
    public int RiskDefineId { get; set; }

    public string DataOld { get; set; } = null!;

    public DateTime? UpdateDate { get; set; }
}
