namespace SME_API_RISK.Models
{
    public class RiskFactorApiResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseMsg { get; set; }
        public List<RiskFactorModels> data { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class RiskFactorModels
    {
        public int RiskDefineID { get; set; }
        public int RiskYear { get; set; }
        public string RiskRFName { get; set; }
        public int RiskTypeId { get; set; }
        public string RiskTypeName { get; set; }
        public string RiskRootCause { get; set; }
        public string RiskOwnerName { get; set; }
        public DateTime UpdateDate { get; set; }
    }

    public class SearchRiskFactor
    {
        public int page { get; set; }
        public int pageSize { get; set; }
        public int riskYear { get; set; }
        public string keyword { get; set; }
       
    }
}
