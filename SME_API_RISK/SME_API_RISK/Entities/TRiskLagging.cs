using System;
using System.Collections.Generic;

namespace SME_API_RISK.Entities;

public partial class TRiskLagging
{
    public int Id { get; set; }

    public int RiskDefineId { get; set; }

    public string LaggingIndicator { get; set; } = null!;

    public DateTime? UpdateDate { get; set; }
}
