using System;
using System.Collections.Generic;

namespace SME_API_RISK.Entities;

public partial class MScheduledJob1
{
    public int Id { get; set; }

    public string? JobName { get; set; }

    public short? RunHour { get; set; }

    public short? RunMinute { get; set; }

    public bool? IsActive { get; set; }
}
