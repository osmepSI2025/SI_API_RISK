using Microsoft.AspNetCore.Mvc;
using SME_API_RISK.Models;
using SME_API_RISK.Service;

namespace SME_API_RISK.Controllers
{
    [Route("api/SYS-RISK/risk/")]
    [ApiController]
    public class RiskController : ControllerBase

    {
        private readonly TRiskimpactService _riskimpactService;
        private readonly TRiskKpiService _riskKpiService;
        private readonly TRiskTriggerService _riskTriggerService;
        private readonly TRiskAtableService _riskAtableService;
        private readonly TRiskDataHistoryService _riskDataHistoryService;
        private readonly TRiskPerformanceService _riskPerformanceService;
        private readonly TRiskRootCauseService _riskRootCauseService;
        private readonly TRiskPlanExistingControlService _riskPlanExistingControlService;
        private readonly TRiskEmergencyPlanService _riskEmergencyPlanService;
        private readonly TRiskLeadingService _riskLeadingService;
        private readonly TRiskLaggingService _riskLaggingService;
        private readonly TRiskCtableService _riskCtableService;
        private readonly TRiskExistingControlService _riskExistingControlService;
        private readonly TRiskResultService  _riskResultService;
        private readonly TRiskLevelService _riskLevelService;
        private readonly TRiskAfterPlanService _riskAfterPlanService;

        public RiskController(


        TRiskimpactService riskimpactService
        , TRiskKpiService riskKpiService
           , TRiskTriggerService riskTriggerService
              , TRiskAtableService riskAtableService
                , TRiskDataHistoryService riskDataHistoryService
             , TRiskPerformanceService riskPerformanceService
            ,TRiskRootCauseService riskRootCauseService
            ,TRiskPlanExistingControlService riskPlanExistingControlService
            , TRiskEmergencyPlanService riskEmergencyPlanService
            , TRiskLeadingService riskLeadingService
            , TRiskLaggingService riskLaggingService
             ,TRiskCtableService riskCtableService
            , TRiskExistingControlService riskExistingControlService
            , TRiskResultService riskResultService
            , TRiskLevelService riskLevelService
            , TRiskAfterPlanService riskAfterPlanService
       )
        {

            _riskimpactService = riskimpactService;
            _riskKpiService = riskKpiService;
            _riskTriggerService = riskTriggerService;
            _riskAtableService = riskAtableService;
            _riskDataHistoryService = riskDataHistoryService;
            _riskPerformanceService = riskPerformanceService;
            _riskRootCauseService = riskRootCauseService;
            _riskPlanExistingControlService = riskPlanExistingControlService;
            _riskEmergencyPlanService = riskEmergencyPlanService;
            _riskLeadingService = riskLeadingService;
            _riskLaggingService = riskLaggingService;
            _riskCtableService = riskCtableService;
            _riskExistingControlService = riskExistingControlService;
            _riskResultService = riskResultService;
            _riskLevelService = riskLevelService;
            _riskAfterPlanService = riskAfterPlanService;
        }
        [HttpPost("kpis")]
        public async Task<ActionResult<IEnumerable<RiskTKipsApiResponse>>> Search_RiskKpi([FromBody] SearchRiskTkpiModels searchModel)
        {
            var riskFactors = await _riskKpiService.GetAllAsyncSearch_RiskTkpi(searchModel);
            return Ok(riskFactors);
        }
        [HttpGet("kpis-BatchEndOfDay")]
        public async Task<ActionResult> BatchEndOfDay_MRiskTKpi()
        {
            SearchRiskTkpiModels searchModel = new SearchRiskTkpiModels();
          
            searchModel.pageSize = 1000;
            searchModel.riskFactorID = 0;
            await _riskKpiService.BatchEndOfDay_MRiskTKpi(searchModel);

            return Ok();
        }
        [HttpPost("impacts")]
        public async Task<ActionResult<IEnumerable<RiskTImpactApiResponse>>> Search_Riskimpact([FromBody] SearchRiskTImpactModels searchModel)
        {
            var riskFactors = await _riskimpactService.GetAllAsyncSearch_RiskTImpact(searchModel);
            return Ok(riskFactors);
        }
        [HttpGet("impacts-BatchEndOfDay")]
        public async Task<ActionResult> BatchEndOfDay_MRiskimpact()
        {
            SearchRiskTImpactModels searchModel = new SearchRiskTImpactModels();
           
            searchModel.pageSize = 1000;
            searchModel.riskFactorID = 0;
            await _riskimpactService.BatchEndOfDay_MRiskTImpact(searchModel);

            return Ok();
        }
        [HttpPost("triggers")]
        public async Task<ActionResult<IEnumerable<RiskTTriggerApiResponse>>> Search_Risktrigger([FromBody] SearchRiskTTriggersModels searchModel)
        {
            var riskFactors = await _riskTriggerService.GetAllAsyncSearch_RiskTTrigger(searchModel);
            return Ok(riskFactors);
        }
        [HttpGet("triggers-BatchEndOfDay")]
        public async Task<ActionResult> BatchEndOfDay_MRisktrigger()
        {
            SearchRiskTTriggersModels searchModel = new SearchRiskTTriggersModels();
          
            searchModel.pageSize = 1000;
            searchModel.riskFactorID = 0;
            await _riskTriggerService.BatchEndOfDay_MRiskTTrigger(searchModel);

            return Ok();
        }
        [HttpPost("root-causes")]
        public async Task<ActionResult<IEnumerable<RiskTRootCauseApiResponse>>> Search_RiskRootCauses([FromBody] SearchRiskTRootCauseModels searchModel)
        {
            var riskFactors = await _riskRootCauseService.GetAllAsyncSearch_RiskTRootCause(searchModel);
            return Ok(riskFactors);
        }
        [HttpGet("root-causes-BatchEndOfDay")]
        public async Task<ActionResult> BatchEndOfDay_rootcauses()
        {
            SearchRiskTRootCauseModels searchModel = new SearchRiskTRootCauseModels();
            searchModel.pageSize = 1000;
            searchModel.riskFactorID = 0;
            await _riskRootCauseService.BatchEndOfDay_MRiskRootCauses(searchModel);

            return Ok();
        }


        [HttpPost("plan-existing-controls")]
        public async Task<ActionResult<IEnumerable<RiskPlanExistingControlApiResponse>>> Search_RiskExistingControls([FromBody] SearchRiskPlanExistingControlModels searchModel)
        {
            var riskFactors = await _riskPlanExistingControlService.GetAllAsyncSearch_PlanExistingControl(searchModel);
            return Ok(riskFactors);
        }
        [HttpGet("plan-existing-controls-BatchEndOfDay")]
        public async Task<ActionResult> BatchEndOfDay_RiskExistingControls()
        {
            SearchRiskPlanExistingControlModels searchModel = new SearchRiskPlanExistingControlModels();
            searchModel.pageSize = 1000;
            searchModel.riskFactorID = 0;
            await _riskPlanExistingControlService.BatchEndOfDay_RiskExistingControls(searchModel);

            return Ok();
        }

        [HttpPost("emergency-plan")]
        public async Task<ActionResult<IEnumerable<RiskEmergencyPlanApiResponse>>> Search_RiskEmergencyPlan([FromBody] SearchRiskEmergencyPlanModels searchModel)
        {
            var riskFactors = await _riskEmergencyPlanService.GetAllAsyncSearch_EmergencyPlan(searchModel);
            return Ok(riskFactors);
        }
        [HttpGet("emergency-plan-BatchEndOfDay")]
        public async Task<ActionResult> BatchEndOfDay_RiskEmergencyPlan()
        {
            SearchRiskEmergencyPlanModels searchModel = new SearchRiskEmergencyPlanModels();
            searchModel.pageSize = 1000;
            searchModel.riskFactorID = 0;

            await _riskEmergencyPlanService.BatchEndOfDay_MRiskEmergencyPlan(searchModel);

            return Ok();
        }



        [HttpPost("leading")]
        public async Task<ActionResult<IEnumerable<RiskLeadingApiResponse>>> Search_Riskleading([FromBody] SearchRiskLeadingModels searchModel)
        {
            var riskFactors = await _riskLeadingService.GetAllAsyncSearch_RiskLeading(searchModel);
            return Ok(riskFactors);
        }
        [HttpGet("leading-BatchEndOfDay")]
        public async Task<ActionResult> BatchEndOfDay_Riskleading()
        {
            SearchRiskLeadingModels searchModel = new SearchRiskLeadingModels();
            searchModel.pageSize = 1000;
            searchModel.riskFactorID =0;
            await _riskLeadingService.BatchEndOfDay_RiskLeading(searchModel);

            return Ok();
        }


        [HttpPost("lagging")]
        public async Task<ActionResult<IEnumerable<RiskLaggingApiResponse>>> Search_Risklagging([FromBody] SearchRiskLaggingModels searchModel)
        {
            var riskFactors = await _riskLaggingService.GetAllAsyncSearch_RiskLagging(searchModel);
            return Ok(riskFactors);
        }
        [HttpGet("lagging-BatchEndOfDay")]
        public async Task<ActionResult> BatchEndOfDay_Risklagging()
        {
            SearchRiskLaggingModels searchModel = new SearchRiskLaggingModels();
            searchModel.pageSize = 1000;
            searchModel.riskFactorID = 0;
            await _riskLaggingService.BatchEndOfDay_RiskLagging(searchModel);

            return Ok();
        }



        [HttpPost("a-table")]
        public async Task<ActionResult<IEnumerable<RiskTATableApiResponse>>> Search_Riskatable([FromBody] SearchRiskTATableModels searchModel)
        {
            var riskFactors = await _riskAtableService.GetAllAsyncSearch_RiskTAtable(searchModel);
            return Ok(riskFactors);
        }
        [HttpGet("a-table-BatchEndOfDay")]
        public async Task<ActionResult> BatchEndOfDay_atable()
        {
            SearchRiskTATableModels searchModel = new SearchRiskTATableModels();
            searchModel.pageSize = 1000;
            searchModel.riskFactorID = 0;
            await _riskAtableService.BatchEndOfDay_MRiskTAtable(searchModel);

            return Ok();
        }
        [HttpPost("data-history")]
        public async Task<ActionResult<IEnumerable<RiskTDataHistoryApiResponse>>> Search_RiskDatahistory([FromBody] SearchRiskTDataHistoryModels searchModel)
        {
            var riskFactors = await _riskDataHistoryService.GetAllAsyncSearch_RiskTRiskDataHistory(searchModel);
            return Ok(riskFactors);
        }
        [HttpGet("data-history-BatchEndOfDay")]
        public async Task<ActionResult> BatchEndOfDay_datahistory()
        {
            SearchRiskTDataHistoryModels searchModel =new SearchRiskTDataHistoryModels();
            searchModel.pageSize = 1000;
            searchModel.riskFactorID = 0;
            await _riskDataHistoryService.BatchEndOfDay_RiskTRiskDataHistory(searchModel);

            return Ok();
        }

        [HttpPost("c-table")]
        public async Task<ActionResult<IEnumerable<RiskCTableApiResponse>>> Search_Riskctable([FromBody] SearchRiskCTableApiModels searchModel)
        {
            var riskFactors = await _riskCtableService.SearchCTableAsync(searchModel);
            return Ok(riskFactors);
        }

        [HttpGet("c-table-BatchEndOfDay")]
        public async Task<ActionResult> BatchEndOfDay_Riskctable()
        {
            SearchRiskCTableApiModels searchModel = new SearchRiskCTableApiModels();
            searchModel.pageSize = 1000;
            searchModel.riskFactorID = 0;
            await _riskCtableService.BatchEndOfDay_MRiskCtable(searchModel);

            return Ok();
        }
        [HttpPost("after-plan-risk")]
        public async Task<ActionResult<IEnumerable<RiskAfterPlanRiskApiResponse>>> SearchRiskAfterPlanAsync([FromBody] SearchRiskAfterPlanApiModels searchModel)
        {
            var riskFactors = await _riskAfterPlanService.SearchRiskAfterPlanAsync(searchModel);
            return Ok(riskFactors);
        }

        [HttpGet("after-plan-risk-BatchEndOfDay")]
        public async Task<ActionResult> BatchEndOfDay_TRiskAfterPlan()
        {
            SearchRiskAfterPlanApiModels searchModel = new SearchRiskAfterPlanApiModels();
            searchModel.pageSize = 1000;
            searchModel.riskFactorID = 0;
            searchModel.riskYear = 0;
            await _riskAfterPlanService.BatchEndOfDay_TRiskAfterPlan(searchModel);

            return Ok();
        }


        [HttpPost("performances")]
        public async Task<ActionResult<IEnumerable<RiskTRiskPerformanceApiResponse>>> Search_Riskperformances([FromBody] SearchTRiskPerformanceModels searchModel)
        {
            var riskFactors = await _riskPerformanceService.GetAllAsyncSearch_RiskTRiskPerformance(searchModel);
            return Ok(riskFactors);
        }
        [HttpGet("performances-BatchEndOfDay")]
        public async Task<ActionResult> BatchEndOfDay_RiskPerformancesy()
        {
            SearchTRiskPerformanceModels searchModel =new SearchTRiskPerformanceModels();
            searchModel.pageSize = 1000;
            searchModel.page = 1;
            searchModel.riskFactorID = 0;
            searchModel.riskYear = 0;

            await _riskPerformanceService.BatchEndOfDay_RiskPerformancesy(searchModel);

            return Ok();
        }
   

        [HttpPost("existing-controls")]
        public async Task<ActionResult<IEnumerable<RiskExistingControlApiResponse>>> Search_ExistingControl([FromBody] SearchRiskExistingControlApiModels searchModel)
        {
            var riskFactors = await _riskExistingControlService.SearchRiskExistingControlAsync(searchModel);
            return Ok(riskFactors);
        }

        [HttpGet("existing-controls-BatchEndOfDay")]
        public async Task<ActionResult> BatchEndOfDay_ExistingControl()
        {
            SearchRiskExistingControlApiModels searchModel = new SearchRiskExistingControlApiModels();
            searchModel.pageSize = 1000;
            searchModel.riskFactorID = 0;
            searchModel.page = 1;
            await _riskExistingControlService.BatchEndOfDay_MExistingControl(searchModel);

            return Ok();
        }
        
        
        [HttpPost("result")]
        public async Task<ActionResult<IEnumerable<RiskResultApiResponse>>> Search_RiskResult([FromBody] SearchRiskResultModels searchModel)
        {
            var riskFactors = await _riskResultService.SearchRiskResullAsync(searchModel);
            return Ok(riskFactors);
        }

        [HttpGet("result-BatchEndOfDay")]
        public async Task<ActionResult> BatchEndOfDay_RiskResult()
        {
            SearchRiskResultModels searchModel =new SearchRiskResultModels();
            searchModel.pageSize = 1000;
            searchModel.riskFactorID = 0;
            searchModel.page = 1;
            await _riskResultService.BatchEndOfDay_RiskReult(searchModel);

            return Ok();
        }

        [HttpPost("risk-risk-levels")]
        public async Task<ActionResult<IEnumerable<RiskTLevelApiResponse>>> Search_RiskResult2([FromBody] SearchTRiskLevelApiModels searchModel)
        {
            var riskFactors = await _riskLevelService.SearchRiskTLevelAsync(searchModel);
            return Ok(riskFactors);
        }

        [HttpGet("risk-levels-BatchEndOfDay")]
        public async Task<ActionResult> BatchEndOfDay_RiskTLevel()
        {
            await _riskLevelService.BatchEndOfDay_TRiskLevel(null);

            return Ok();
        }

    

        
    }

}
