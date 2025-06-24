using System;
using System.Collections.Generic;

namespace SME_API_RISK.Entities;

public partial class MRiskOwner
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime? UpdateDate { get; set; }
}
