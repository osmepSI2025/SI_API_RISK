namespace SME_API_RISK.Models
{
    public class TInternalControlsEvaluationApiResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseMsg { get; set; }
        public List<TInternalControlsEvaluationModels> data { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class TInternalControlsEvaluationModels
    {
        public string Departments { get; set; } = null!;

        public string Activities { get; set; } = null!;

        public string? RiskDescription { get; set; }

        public string? RiskRootCause { get; set; }

        public string? RiskImpactNeg { get; set; }

        public string? ExitingControl { get; set; }

        public int? OldWorkPoint { get; set; }

        public int? ProcessPoint { get; set; }

        public int? ReportPoint { get; set; }

        public string? Result { get; set; }

        public DateTime? UpdateDate { get; set; }

    }

    public class SearchTInternalControlsEvaluationModels
    {
        public int page { get; set; }
        public int pageSize { get; set; }
   
        public int riskYear { get; set; }
       
    }
}
