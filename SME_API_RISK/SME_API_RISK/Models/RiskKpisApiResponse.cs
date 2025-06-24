namespace SME_API_RISK.Models
{
    public class RiskTKipsApiResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseMsg { get; set; }
        public List<RiskTKpisModels> data { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class RiskTKpisModels
    {
        public int RiskDefineId { get; set; }

        public string Kpis { get; set; } = null!;

        public DateTime? UpdateDate { get; set; }
    }

    

    public class SearchRiskTkpiModels
    {
        public int page { get; set; }
        public int pageSize { get; set; }
   
        public int riskFactorID { get; set; }
       
    }
}
