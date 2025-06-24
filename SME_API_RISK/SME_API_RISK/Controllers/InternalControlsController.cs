using Microsoft.AspNetCore.Mvc;
using SME_API_RISK.Models;
using SME_API_RISK.Service;

namespace SME_API_RISK.Controllers
{
    [Route("api/SYS-RISK")]
    [ApiController]
    public class InternalControlsController : ControllerBase
    {
        private readonly TInternalControlsActivityService _internalControlsActivityService;
        private readonly TInternalControlsEvaluationService _internalControlsEvaluationService;
        private readonly TInternalControlsReportService _internalControlsReportService;
        public InternalControlsController(


            TInternalControlsActivityService internalControlsActivityService
            , TInternalControlsEvaluationService internalControlsEvaluationService
            , TInternalControlsReportService internalControlsReportService
)
        {
             _internalControlsActivityService = internalControlsActivityService;
            _internalControlsEvaluationService = internalControlsEvaluationService;
            _internalControlsReportService = internalControlsReportService;

        }

        [HttpPost("internal-controls/activities")]
        public async Task<ActionResult<IEnumerable<TInternalControlsActivityApiResponse>>> Search_TInternalControlsActivity([FromBody] SearchTInternalControlsActivityModels searchModel)
        {
            var riskFactors = await _internalControlsActivityService.GetAllAsyncSearch_InternalActivity(searchModel);
            return Ok(riskFactors);
        }
        [HttpGet("internal-controls/activities-BatchEndOfDay")]
        public async Task<ActionResult> BatchEndOfDay_activities()
        {
            int currentYear = DateTime.Now.Year;

            foreach (var year in Enumerable.Range(0, 3))
            {
                int christianYear = currentYear - year;
                int buddhistYear = christianYear + 543;
              
                await _internalControlsActivityService.BatchEndOfDay_InternalActivity(buddhistYear);
            }
           

            return Ok();
        }



        [HttpPost("internal-controls/evaluations")]
        public async Task<ActionResult<IEnumerable<TInternalControlsEvaluationApiResponse>>> Search_TInternalControlsEvaluation([FromBody] SearchTInternalControlsEvaluationModels searchModel)
        {
            var riskFactors = await _internalControlsEvaluationService.GetAllAsyncSearch_InternalEvaluation(searchModel);
            return Ok(riskFactors);
        }
        [HttpGet("internal-controls/evaluations-BatchEndOfDay")]
        public async Task<ActionResult> BatchEndOfDay_evaluations()
        {
            int currentYear = DateTime.Now.Year;

            foreach (var year in Enumerable.Range(0, 3))
            {
                int christianYear = currentYear - year;
                int buddhistYear = christianYear + 543;

                await _internalControlsEvaluationService.BatchEndOfDay_InternalEvaluation(buddhistYear);
            }


            return Ok();
        }

        [HttpPost("internal-controls/reports")]
        public async Task<ActionResult<IEnumerable<TInternalControlsReportApiResponse>>> Search_TInternalControlsReports([FromBody] SearchTInternalControlsReportModels searchModel)
        {
            var riskFactors = await _internalControlsReportService.GetAllAsyncSearch_TInternalControlsReport(searchModel);
            return Ok(riskFactors);
        }
        [HttpGet("internal-controls/reports-BatchEndOfDay")]
        public async Task<ActionResult> BatchEndOfDay_reports()
        {
            int currentYear = DateTime.Now.Year;

            foreach (var year in Enumerable.Range(0, 3))
            {
                int christianYear = currentYear - year;
                int buddhistYear = christianYear + 543;

                await _internalControlsReportService.BatchEndOfDay_InternalActivity(buddhistYear);
            }


            return Ok();
        }
    }
}
