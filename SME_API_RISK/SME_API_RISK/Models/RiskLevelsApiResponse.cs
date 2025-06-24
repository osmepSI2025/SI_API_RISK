namespace SME_API_RISK.Models
{
    public class RiskLevelsApiResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseMsg { get; set; }
        public List<RiskLevelModels> data { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class RiskLevelModels
    {
        public int Id { get; set; }

        public int Levels { get; set; }

        public string LikelihoodDefine { get; set; } = null!;

        public string ImpactDefine { get; set; } = null!;

        public DateTime? UpdateDate { get; set; }

    }

    public class SearchRiskLevelsModels
    {
        public int page { get; set; }
        public int pageSize { get; set; }

        public string keyword { get; set; }
       
    }
}
