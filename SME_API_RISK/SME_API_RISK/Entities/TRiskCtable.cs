using System;
using System.Collections.Generic;

namespace SME_API_RISK.Entities;

public partial class TRiskCtable
{
    public int Id { get; set; }

    public int RiskDefineId { get; set; }

    public string RootCauseType { get; set; } = null!;

    public string RootCauseName { get; set; } = null!;

    public string Solutions { get; set; } = null!;

    public string QualityCost { get; set; } = null!;

    public string QuantityCost { get; set; } = null!;

    public string QualityBenefit { get; set; } = null!;

    public string QuantityBenefit { get; set; } = null!;

    public DateTime? UpdateDate { get; set; }
}
