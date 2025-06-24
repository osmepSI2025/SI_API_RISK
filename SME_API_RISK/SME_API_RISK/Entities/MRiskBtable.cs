using System;
using System.Collections.Generic;

namespace SME_API_RISK.Entities;

public partial class MRiskBtable
{
    public int Id { get; set; }

    public int Levels { get; set; }

    public string Performance { get; set; } = null!;

    public string OldWork { get; set; } = null!;

    public string Process { get; set; } = null!;

    public string Report { get; set; } = null!;

    public DateTime? UpdateDate { get; set; }
}
