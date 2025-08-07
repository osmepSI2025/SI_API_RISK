using System;
using System.Collections.Generic;

namespace SME_API_RISK.Entities;

public partial class TRiskCtable
{
    public int Id { get; set; }

    public int RiskDefineId { get; set; }

    public string RootCauseType { get; set; } = null!;

    public string RootCauseName { get; set; } = null!;

    public string? Solutions { get; set; }

    public string? QualityCost { get; set; }

    public string? QuantityCost { get; set; }

    public string? QualityBenefit { get; set; }

    public string? QuantityBenefit { get; set; }

    public DateTime? UpdateDate { get; set; }
}
