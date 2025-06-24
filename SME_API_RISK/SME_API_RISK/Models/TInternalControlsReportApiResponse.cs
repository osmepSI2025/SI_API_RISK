namespace SME_API_RISK.Models
{
    public class TInternalControlsReportApiResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseMsg { get; set; }
        public List<TInternalControlsReportModels> data { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class TInternalControlsReportModels
    {
        public string Departments { get; set; } = null!;

        public string AssessControlResult { get; set; } = null!;

        public string QuaterFinished { get; set; } = null!;

        public string Q1 { get; set; } = null!;

        public string Q2 { get; set; } = null!;

        public string Q3 { get; set; } = null!;

        public string Q4 { get; set; } = null!;

        public string Result { get; set; } = null!;

        public string ClosedComment { get; set; } = null!;

        public DateTime? UpdateDate { get; set; }

    }

    public class SearchTInternalControlsReportModels
    {
        public int page { get; set; }
        public int pageSize { get; set; }
   
        public int riskYear { get; set; }
       
    }
}
