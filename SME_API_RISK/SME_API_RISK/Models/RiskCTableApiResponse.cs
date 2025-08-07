public class CTableSolotion
{
    public string solotions { get; set; }
    public string qualityCost { get; set; }
    public string quantityCost { get; set; }
    public string qualityBenefit { get; set; }
    public string quantityenefit { get; set; }
}

public class CTableRootCause
{
    public string rootCauseType { get; set; }
    public string rootCauseName { get; set; }
    public List<CTableSolotion> solotionsList { get; set; }
    public DateTime updateDate { get; set; }
}

public class CTableData
{
    public int riskDefineID { get; set; }
    public List<CTableRootCause> rootCauseList { get; set; }
}

public class RiskCTableApiResponse
{
    public string responseCode { get; set; }
    public string responseMsg { get; set; }
    public List<CTableData> data { get; set; }
    public DateTime timestamp { get; set; }
}
public class SearchRiskCTableApiModels
{
    public int page { get; set; }
    public int pageSize { get; set; }
    public string keyword { get; set; }
    public int riskFactorID { get; set; }

}