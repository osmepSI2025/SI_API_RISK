namespace SME_API_RISK.Models
{
    public class RiskTTriggerApiResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseMsg { get; set; }
        public List<RiskTTriggersModels> data { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class RiskTTriggersModels
    {
        public int RiskDefineId { get; set; }

        public string Triggers { get; set; } = null!;

        public DateTime? UpdateDate { get; set; }
    }

    

    public class SearchRiskTTriggersModels
    {
        public int page { get; set; }
        public int pageSize { get; set; }
   
        public int riskFactorID { get; set; }
       
    }
}
