using System;
using System.Collections.Generic;

namespace SME_API_RISK.Entities;

public partial class TInternalControlsReport
{
    public int Id { get; set; }

    public string? Departments { get; set; }

    public string? AssessControlResult { get; set; }

    public string? QuaterFinished { get; set; }

    public string? Q1 { get; set; }

    public string? Q2 { get; set; }

    public string? Q3 { get; set; }

    public string? Q4 { get; set; }

    public string? Result { get; set; }

    public string? ClosedComment { get; set; }

    public DateTime? UpdateDate { get; set; }

    public int? RiskYear { get; set; }
}
