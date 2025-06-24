public class RiskEmergencyPlanApiResponse
{
    public string ResponseCode { get; set; }
    public string ResponseMsg { get; set; }
    public List<RiskEmergencyPlanData> data { get; set; }
    public DateTime Timestamp { get; set; }
}

public class RiskEmergencyPlanData
{
    public int RiskDefineID { get; set; }
    public List<RiskEmergencyPlanItem> riskManageList { get; set; }
}

public class RiskEmergencyPlanItem
{
    public string RootCauseType { get; set; }
    public string RootCauseName { get; set; }
    public string StandardRiskManage { get; set; }
    public string QplanStart { get; set; }
    public string QplanEnd { get; set; }
    public string Objectives { get; set; }
    public DateTime? UpdateDate { get; set; }
}

    public class SearchRiskEmergencyPlanModels
{
    public int page { get; set; }
    public int pageSize { get; set; }

    public int riskFactorID { get; set; }

}