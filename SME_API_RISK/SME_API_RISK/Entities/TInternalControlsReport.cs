using System;
using System.Collections.Generic;

namespace SME_API_RISK.Entities;

public partial class TInternalControlsReport
{
    public int Id { get; set; }

    public string Departments { get; set; } = null!;

    public string AssessControlResult { get; set; } = null!;

    public string QuaterFinished { get; set; } = null!;

    public string Q1 { get; set; } = null!;

    public string Q2 { get; set; } = null!;

    public string Q3 { get; set; } = null!;

    public string Q4 { get; set; } = null!;

    public string Result { get; set; } = null!;

    public string ClosedComment { get; set; } = null!;

    public DateTime? UpdateDate { get; set; }

    public int? RiskYear { get; set; }
}
