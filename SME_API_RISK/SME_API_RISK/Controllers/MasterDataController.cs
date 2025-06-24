using Microsoft.AspNetCore.Mvc;
using SME_API_RISK.Entities;
using SME_API_RISK.Models;
using SME_API_RISK.Service;

namespace SME_API_RISK.Controllers
{
    [Route("api/SYS-RISK/master")]
    [ApiController]
    public class MasterDataController : ControllerBase
    {
        private readonly MRiskFactorService _riskFactorService;
        private readonly MRiskTypeService _riskTypeService;
        private readonly MRiskUniverseService _riskUnverseService;
        private readonly MRiskOwnerService _riskOwnerService;
        private readonly MRiskLevelService _riskLevelService;
        private readonly MRiskTreatmentOptionService _riskTreatmentOptionService;
        private readonly MRiskBtableService _riskBtableService;

        public MasterDataController(MRiskFactorService riskFactorService, MRiskTypeService riskTypeService
            , MRiskUniverseService riskUnverseService
            , MRiskOwnerService riskOwnerService
            , MRiskLevelService riskLevelService
             
            , MRiskTreatmentOptionService riskTreatmentOptionService
            , MRiskBtableService riskBtableService
            )
        {
            _riskFactorService = riskFactorService;
            _riskTypeService = riskTypeService;
            _riskUnverseService = riskUnverseService;
        
            _riskOwnerService = riskOwnerService;
            _riskLevelService = riskLevelService;
            _riskTreatmentOptionService = riskTreatmentOptionService;
            _riskBtableService = riskBtableService;
  
        }

        [HttpPost("risk-factors")]
        public async Task<ActionResult<IEnumerable<RiskFactorApiResponse>>> Search([FromBody] SearchRiskFactor searchModel)
        {
            var riskFactors = await _riskFactorService.GetAllAsyncSearch_RiskFactor(searchModel);
            return Ok(riskFactors);
        }
        [HttpGet("risk-Factor-BatchEndOfDay")]
        public async Task<ActionResult> BatchEndOfDay_MRiskFactor()
        {
            int currentYear = DateTime.Now.Year;

            foreach (var year in Enumerable.Range(0, 3))
            {
                int christianYear = currentYear - year;
                int buddhistYear = christianYear + 543;
                ///
                await _riskFactorService.BatchEndOfDay_MRiskFactor(buddhistYear,"");
                ///

            }
           
           
            return Ok();
        }

        [HttpPost("risk-types")]
        public async Task<ActionResult<IEnumerable<RiskTypeApiResponse>>> Search([FromBody] SearchRiskType searchModel)
        {
            var riskFactors = await _riskTypeService.GetAllAsyncSearch_RiskType(searchModel);
            return Ok(riskFactors);
        }
        [HttpGet("risk-types-BatchEndOfDay")]
        public async Task<ActionResult> BatchEndOfDay_MRiskType()
        {
            await _riskTypeService.BatchEndOfDay_MRiskType();


            return Ok();
        }
        [HttpPost("risk-universe")]
        public async Task<ActionResult<IEnumerable<RiskUniverseApiResponse>>> Search_RiskUniverse([FromBody] SearchRiskRiskUniverse searchModel)
        {
            var riskFactors = await _riskUnverseService.GetAllAsyncSearch_RiskUniverse(searchModel);
            return Ok(riskFactors);
        }
        [HttpGet("risk-universe-BatchEndOfDay")]
        public async Task<ActionResult> BatchEndOfDay_Muniverse()
        {
            SearchRiskRiskUniverse Msearch = new SearchRiskRiskUniverse
            {

                page = 1,
                pageSize = 1000,
                keyword = ""

            };
            await _riskUnverseService.BatchEndOfDay_MRiskUniverse(Msearch);

            return Ok();
        }
        [HttpPost("risk-Owner")]
        public async Task<ActionResult<IEnumerable<RiskOwnerApiResponse>>> Search_RiskOwner([FromBody] SearchRiskOwnerModels searchModel)
        {
            var riskFactors = await _riskOwnerService.GetAllAsyncSearch_RiskOwner(searchModel);
            return Ok(riskFactors);
        }
        [HttpGet("risk-Owner-BatchEndOfDay")]
        public async Task<ActionResult> BatchEndOfDay_MRiskOwner()
        {
            SearchRiskOwnerModels Msearch = new SearchRiskOwnerModels
            {

                page = 1,
                pageSize = 1000,
                keyword = ""

            };
            await _riskOwnerService.BatchEndOfDay_MRiskOwner(Msearch);

            return Ok();
        }
        [HttpPost("risk-levels")]
        public async Task<ActionResult<IEnumerable<RiskLevelsApiResponse>>> Search_Risklevels()
        {
            var riskFactors = await _riskLevelService.GetAllAsyncSearch_RiskLevels();
            return Ok(riskFactors);
        }
        [HttpGet("risk-levels-BatchEndOfDay")]
        public async Task<ActionResult> BatchEndOfDay_MRisklevels()
        {
            await _riskLevelService.BatchEndOfDay_MRiskLevel();

            return Ok();
        }


        [HttpPost("risk-treatment-option")]
        public async Task<ActionResult<IEnumerable<RiskTreatmentOptionApiResponse>>> Search_RiskTreatmentOption([FromBody] SearchRiskTreatmentOptionModels searchModel)
        {
            var riskFactors = await _riskTreatmentOptionService.GetAllAsyncSearch_RiskTreatmentOption(searchModel);
            return Ok(riskFactors);
        }
        [HttpGet("risk-treatment-option-BatchEndOfDay")]
        public async Task<ActionResult> BatchEndOfDay_RiskTreatmentOption()
        {
            SearchRiskTreatmentOptionModels Msearch = new SearchRiskTreatmentOptionModels
            {

                page = 1,
                pageSize = 100,
                keyword = ""

            };
            await _riskTreatmentOptionService.BatchEndOfDay_MRiskTreatmentOption(Msearch);

            return Ok();
        }
        [HttpPost("risk-B-Table")]
        public async Task<ActionResult<IEnumerable<RiskBTableApiResponse>>> Search_BTable()
        {
            var riskFactors = await _riskBtableService.GetAllAsyncSearch_RiskBTable();
            return Ok(riskFactors);
        }
        [HttpGet("risk-B-Table-BatchEndOfDay")]
        public async Task<ActionResult> BatchEndOfDay_RiskBTable()
        {
            await _riskBtableService.BatchEndOfDay_MRiskBTable();

            return Ok();
        }

     

    }
}
