public class RiskTRootCauseApiResponse
{
    public string ResponseCode { get; set; }
    public string ResponseMsg { get; set; }
    public List<RiskTRootCauseData> data { get; set; }
    public DateTime Timestamp { get; set; }
}

public class RiskTRootCauseData
{
    public int RiskDefineID { get; set; }
    public List<RiskTRootCauseItem> rootCauseList { get; set; }
}

public class RiskTRootCauseItem
{
    public string RootCauseType { get; set; }
    public string RootCauseName { get; set; }
    public int Ratio { get; set; }
    public DateTime? UpdateDate { get; set; }
}
public class SearchRiskTRootCauseModels
{
    public int page { get; set; }
    public int pageSize { get; set; }
    public string keyword { get; set; }
    public int riskFactorID { get; set; }

}