using System;
using System.Collections.Generic;

namespace SME_API_RISK.Entities;

public partial class MRiskType
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public DateTime? UpdateDate { get; set; }
}
