public class LaggingIndicatorData
{
    public int riskDefineID { get; set; }
    public string laggingIndicator { get; set; }
    public DateTime? updateDate { get; set; }
}

public class RiskLaggingApiResponse
{
    public string responseCode { get; set; }
    public string responseMsg { get; set; }
    public List<LaggingIndicatorData> data { get; set; }
    public DateTime timestamp { get; set; }
}
public class SearchRiskLaggingModels
{


    public int page { get; set; }
    public int pageSize { get; set; }
    public string keyword { get; set; }
    public int riskFactorID { get; set; }

}