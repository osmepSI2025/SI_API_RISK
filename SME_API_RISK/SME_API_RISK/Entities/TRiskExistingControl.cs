using System;
using System.Collections.Generic;

namespace SME_API_RISK.Entities;

public partial class TRiskExistingControl
{
    public int Id { get; set; }

    public int RiskDefineId { get; set; }

    public string RootCauseType { get; set; } = null!;

    public string RootCauseName { get; set; } = null!;

    public string Performances { get; set; } = null!;

    public DateTime? UpdateDate { get; set; }
}
