using System;
using System.Collections.Generic;

namespace SME_API_RISK.Entities;

public partial class MRiskLevel
{
    public int Id { get; set; }

    public int Levels { get; set; }

    public string LikelihoodDefine { get; set; } = null!;

    public string ImpactDefine { get; set; } = null!;

    public DateTime? UpdateDate { get; set; }
}
