using System;
using System.Collections.Generic;

namespace SME_API_RISK.Entities;

public partial class TRiskPlanExistingControl
{
    public int Id { get; set; }

    public int RiskDefineId { get; set; }

    public string? ExistingControl { get; set; }

    public DateTime? UpdateDate { get; set; }
}
