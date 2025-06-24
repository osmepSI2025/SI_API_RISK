public class ResultRiskManageItem
{
    public string rootCauseType { get; set; }
    public string rootCauseName { get; set; }
    public string performances { get; set; }
    public string status { get; set; }
    public DateTime updateDate { get; set; }
}

public class ResultData
{
    public int riskDefineID { get; set; }
    public List<ResultRiskManageItem> riskManageList { get; set; }
}

public class RiskResultApiResponse
{
    public string responseCode { get; set; }
    public string responseMsg { get; set; }
    public List<ResultData> data { get; set; }
    public DateTime timestamp { get; set; }
}

public class SearchRiskResultModels
{
    public int page { get; set; }
    public int pageSize { get; set; }

    public int riskFactorID { get; set; }

}