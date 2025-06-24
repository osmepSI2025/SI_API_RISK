using System;
using System.Collections.Generic;

namespace SME_API_RISK.Entities;

public partial class TRiskEmergencyPlan
{
    public int Id { get; set; }

    public int RiskDefineId { get; set; }

    public string RootCauseType { get; set; } = null!;

    public string RootCauseName { get; set; } = null!;

    public string StandardRiskManage { get; set; } = null!;

    public string QplanStart { get; set; } = null!;

    public string QplanEnd { get; set; } = null!;

    public string Objectives { get; set; } = null!;

    public DateTime? UpdateDate { get; set; }
}
