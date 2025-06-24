using System;
using System.Collections.Generic;

namespace SME_API_RISK.Entities;

public partial class TInternalControlsActivity
{
    public int Id { get; set; }

    public string? Departments { get; set; }

    public string? Activities { get; set; }

    public string? Process { get; set; }

    public DateTime? UpdateDate { get; set; }

    public int? RiskYear { get; set; }
}
