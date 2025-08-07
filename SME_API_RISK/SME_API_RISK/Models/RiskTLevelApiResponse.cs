// File: SME_API_RISK/Models/RiskLevelResponseModels.cs
namespace SME_API_RISK.Models
{
    public class TRiskLevelListItem
    {
        public string? riskLevelTitle { get; set; }
        public int? l { get; set; }
        public int? i { get; set; }
        public string? colors { get; set; }
        public DateTime? updateDate { get; set; }
    }

    public class TRiskLevelDataItem
    {
        public int riskDefineID { get; set; }
        public string riskDefine { get; set; }
        public List<TRiskLevelListItem> riskLevelList { get; set; }
    }

    public class RiskTLevelApiResponse
    {
        public string responseCode { get; set; }
        public string responseMsg { get; set; }
        public List<TRiskLevelDataItem> data { get; set; }
        public DateTime timestamp { get; set; }
    }
    public class SearchTRiskLevelApiModels
    {
        public int page { get; set; }
        public int pageSize { get; set; }

        public int riskFactorID { get; set; }
        public int riskYear { get; set; }

        public string keyword { get; set; }
    }
}
