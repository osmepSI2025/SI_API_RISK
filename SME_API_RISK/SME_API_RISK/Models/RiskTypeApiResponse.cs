namespace SME_API_RISK.Models
{
    public class RiskTypeApiResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseMsg { get; set; }
        public List<RiskTypeModels> data { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class RiskTypeModels
    {
        public int id { get; set; }
        public string? name { get; set; }     
        public DateTime? updateDate { get; set; }

    }

    public class SearchRiskType
    {
        public int page { get; set; }
        public int pageSize { get; set; }
   
        public string keyword { get; set; }
       
    }
}
