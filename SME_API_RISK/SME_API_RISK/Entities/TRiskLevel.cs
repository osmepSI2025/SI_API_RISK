using System;
using System.Collections.Generic;

namespace SME_API_RISK.Entities;

public partial class TRiskLevel
{
    public int Id { get; set; }

    public int RiskDefineId { get; set; }

    public string? RiskDefine { get; set; }

    public string? RiskLevelTitle { get; set; }

    public int? L { get; set; }

    public int? I { get; set; }

    public string? Colors { get; set; }

    public int? YearBudget { get; set; }

    public DateTime? UpdateDate { get; set; }
}
