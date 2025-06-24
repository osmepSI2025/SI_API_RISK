using System;
using System.Collections.Generic;

namespace SME_API_RISK.Entities;

public partial class TInternalControlsEvaluation
{
    public int Id { get; set; }

    public string Departments { get; set; } = null!;

    public string Activities { get; set; } = null!;

    public string? RiskDescription { get; set; }

    public string? RiskRootCause { get; set; }

    public string? RiskImpactNeg { get; set; }

    public string? ExitingControl { get; set; }

    public int? OldWorkPoint { get; set; }

    public int? ProcessPoint { get; set; }

    public int? ReportPoint { get; set; }

    public string? Result { get; set; }

    public DateTime? UpdateDate { get; set; }

    public int? RiskYear { get; set; }
}
