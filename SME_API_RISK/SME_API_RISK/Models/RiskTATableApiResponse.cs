namespace SME_API_RISK.Models
{
    public class RiskTATableApiResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseMsg { get; set; }
        public List<RiskTATableModels> data { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class RiskTATableModels
    {
        public int RiskDefineId { get; set; }

        public string LikelihoodDefine { get; set; } 

        public string ImpactDefine { get; set; } 

        public DateTime? UpdateDate { get; set; }

    }

    public class SearchRiskTATableModels
    {
        public int RiskDefineId { get; set; }
    }

}
