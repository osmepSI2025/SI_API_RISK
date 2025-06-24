namespace SME_API_RISK.Models
{
    public class RiskOwnerApiResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseMsg { get; set; }
        public List<RiskOwnerModels> data { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class RiskOwnerModels
    {
        public int id { get; set; }
        public string? name { get; set; }     
        public DateTime? updateDate { get; set; }

    }

    public class SearchRiskOwnerModels
    {
        public int page { get; set; }
        public int pageSize { get; set; }
   
        public string keyword { get; set; }
       
    }
}
