namespace SME_API_RISK.Models
{
    public class TInternalControlsActivityApiResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseMsg { get; set; }
        public List<TInternalControlsActivityModels> data { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class TInternalControlsActivityModels
    {
        public string Departments { get; set; } = null!;

        public string Activities { get; set; } = null!;

        public string Process { get; set; } = null!;

        public DateTime? UpdateDate { get; set; }

    }

    public class SearchTInternalControlsActivityModels
    {
        public int page { get; set; }
        public int pageSize { get; set; }
   
        public int riskYear { get; set; }
       
    }
}
