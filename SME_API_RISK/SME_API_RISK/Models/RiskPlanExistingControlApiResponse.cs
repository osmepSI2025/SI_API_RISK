public class RiskPlanExistingControlApiResponse
{
    public string ResponseCode { get; set; }
    public string ResponseMsg { get; set; }
    public List<RiskPlanExistingControlData> data { get; set; }
    public DateTime Timestamp { get; set; }
}

public class RiskPlanExistingControlData
{
    public int RiskDefineID { get; set; }
    public List<RiskPlanExistingControlItem> existingControlList { get; set; }
}

public class RiskPlanExistingControlItem
{
    public string? ExistingControl { get; set; }
    public DateTime? UpdateDate { get; set; }
}
public class SearchRiskPlanExistingControlModels
{
    public int page { get; set; }
    public int pageSize { get; set; }

    public int riskFactorID { get; set; }

}