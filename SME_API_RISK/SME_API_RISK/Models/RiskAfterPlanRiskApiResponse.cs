namespace SME_API_RISK.Models
{
    public class AfterPlanRiskQuaterItem
    {
        public int? quaterNo { get; set; }
        public int? l { get; set; }
        public int? i { get; set; }
      //  public DateTime updateDate { get; set; }
    }

    public class AfterPlanRiskDataItem
    {
        public int riskDefineID { get; set; }
        public string riskDefine { get; set; }
        public List<AfterPlanRiskQuaterItem> quaterList { get; set; }
        public DateTime updateDate { get; set; }
    }

    public class RiskAfterPlanRiskApiResponse
    {
        public string responseCode { get; set; }
        public string responseMsg { get; set; }
        public List<AfterPlanRiskDataItem> data { get; set; }

        public DateTime timestamp { get; set; }
    }
    public class SearchRiskAfterPlanApiModels
    {
        public int page { get; set; }
        public int pageSize { get; set; }
        public string keyword { get; set; }
        public int riskFactorID { get; set; }
        public int riskYear { get; set; }

    }
}

