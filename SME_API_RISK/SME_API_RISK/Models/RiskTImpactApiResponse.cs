namespace SME_API_RISK.Models
{
    public class RiskTImpactApiResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseMsg { get; set; }
        public List<RiskTImpactModels> data { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class RiskTImpactModels
    {
        public int RiskDefineId { get; set; }

        public string Impacts { get; set; } = null!;

        public DateTime? UpdateDate { get; set; }
    }

    

    public class SearchRiskTImpactModels
    {
        public int page { get; set; }
        public int pageSize { get; set; }
   
        public int riskFactorID { get; set; }
       
    }
}
