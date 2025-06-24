namespace SME_API_RISK.Models
{
    public class RiskBTableApiResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseMsg { get; set; }
        public List<RiskBTableModels> data { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class RiskBTableModels
    {
      

        public int Levels { get; set; }

        public string Performance { get; set; } = null!;

        public string OldWork { get; set; } = null!;

        public string Process { get; set; } = null!;

        public string Report { get; set; } = null!;

        public DateTime? UpdateDate { get; set; }

    }

    public class SearchRiskBTableModels
    {
        public int page { get; set; }
        public int pageSize { get; set; }
   
        public string keyword { get; set; }
       
    }
}
