using System;
using System.Collections.Generic;

namespace SME_API_RISK.Entities;

public partial class TRiskLeading
{
    public int Id { get; set; }

    public int RiskDefineId { get; set; }

    public string LeadingIndicator { get; set; } = null!;

    public DateTime? UpdateDate { get; set; }
}
