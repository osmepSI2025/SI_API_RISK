namespace SME_API_RISK.Models
{
    public class RiskTreatmentOptionApiResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseMsg { get; set; }
        public List<RiskTreatmentOptionModels> data { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class RiskTreatmentOptionModels
    {
        public int id { get; set; }
        public string? name { get; set; }     
        public DateTime? updateDate { get; set; }

    }

    public class SearchRiskTreatmentOptionModels
    {
        public int page { get; set; }
        public int pageSize { get; set; }
   
        public string keyword { get; set; }
       
    }
}
