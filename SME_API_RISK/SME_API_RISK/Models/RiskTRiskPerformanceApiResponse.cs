namespace SME_API_RISK.Models
{
    public class RiskTRiskPerformanceApiResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseMsg { get; set; }
        public List<TRiskPerformanceModels> data { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class TRiskPerformanceModels
    {
        public int RiskDefineId { get; set; }

        public int Quarter { get; set; }


        public string Performances { get; set; } = null!;

        public DateTime? UpdateDate { get; set; }
    }

    public class SearchTRiskPerformanceModels
    {
     
        public int RiskDefineId { get; set; }

    }
}
