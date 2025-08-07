public class LeadingIndicatorData
{
    public int riskDefineID { get; set; }
    public string leadingIndicator { get; set; }
    public DateTime? updateDate { get; set; }
}

public class RiskLeadingApiResponse
{
    public string responseCode { get; set; }
    public string responseMsg { get; set; }
    public List<LeadingIndicatorData> data { get; set; }
    public DateTime timestamp { get; set; }
}
public class SearchRiskLeadingModels
{


    public int page { get; set; }
    public int pageSize { get; set; }
    public string keyword { get; set; }
    public int riskFactorID { get; set; }

}