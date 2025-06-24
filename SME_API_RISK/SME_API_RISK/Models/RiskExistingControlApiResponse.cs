public class ExistingControlRiskManageItem
{
    public string rootCauseType { get; set; }
    public string rootCauseName { get; set; }
    public string performances { get; set; }
    public DateTime updateDate { get; set; }
}

public class ExistingControlData
{
    public int riskDefineID { get; set; }
    public List<ExistingControlRiskManageItem> riskManageList { get; set; }
}

public class RiskExistingControlApiResponse
{
    public string responseCode { get; set; }
    public string responseMsg { get; set; }
    public List<ExistingControlData> data { get; set; }
    public DateTime timestamp { get; set; }
}
public class SearchRiskExistingControlApiModels
{
    public int page { get; set; }
    public int pageSize { get; set; }

    public int riskFactorID { get; set; }

}