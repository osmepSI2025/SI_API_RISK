namespace SME_API_RISK.Models
{
    public class RiskTDataHistoryApiResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseMsg { get; set; }
        public List<RiskTDataHistoryModels> data { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class RiskTDataHistoryModels
    {
        public int RiskDefineId { get; set; }

        public string DataOld { get; set; } = null!;

        public DateTime? UpdateDate { get; set; }
     
    }

    public class SearchRiskTDataHistoryModels
    {
        public int page { get; set; }
        public int pageSize { get; set; }
        public string keyword { get; set; }
        public int riskFactorID { get; set; }

    }
}
