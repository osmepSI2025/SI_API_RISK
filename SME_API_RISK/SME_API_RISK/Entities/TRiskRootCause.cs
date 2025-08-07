using System;
using System.Collections.Generic;

namespace SME_API_RISK.Entities;

public partial class TRiskRootCause
{
    public int Id { get; set; }

    public int RiskDefineId { get; set; }

    public string RootCauseType { get; set; } = null!;

    public string RootCauseName { get; set; } = null!;

    public int Ratio { get; set; }

    public DateTime? UpdateDate { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }
}
