    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Quartz;
    using SME_API_RISK.Entities;
    using SME_API_RISK.Models;
    using SME_API_RISK.Service;
    using SME_API_RISK.Services;

public class ScheduledJobPuller : IJob
{
    private readonly RISKDBContext _dbContext;
    private readonly ILogger<ScheduledJobPuller> _logger;
    private readonly MRiskFactorService _mRiskFactorService;
    private readonly IServiceProvider _serviceProvider;
    private readonly MRiskUniverseService _mRiskUniverseService;
    private readonly MRiskOwnerService _mRiskOwnerService;
    private readonly MRiskLevelService _mRiskLevelService;
    private readonly MRiskTreatmentOptionService _mRiskTreatmentOptionService;
    private readonly MRiskBtableService _mRiskBtableService;
    private readonly TRiskKpiService _tRiskKpiService;
    private readonly TRiskimpactService _tRiskimpactService;
    private readonly TRiskTriggerService _tRiskTriggerService;
    private readonly TRiskRootCauseService _tRiskRootCauseService;
    private readonly TRiskPlanExistingControlService _tRiskPlanExistingControlService;
    private readonly TRiskEmergencyPlanService _tRiskEmergencyPlanService;
    private readonly TRiskLeadingService _tRiskLeadingService;
    private readonly TRiskLaggingService _tRiskLaggingService;
    private readonly TRiskAtableService _tRiskAtableService;
    private readonly TRiskDataHistoryService _tRiskDataHistoryService;
    private readonly TRiskCtableService _tRiskCtableService;
    private readonly TRiskAfterPlanService _tRiskAfterPlanService;
    private readonly TRiskPerformanceService _tRiskPerformanceService;
    private readonly TRiskExistingControlService _tRiskExistingControlService;
    private readonly TRiskResultService _tRiskResultService;
    private readonly TRiskLevelService _tRiskLevelService;
    private readonly TInternalControlsActivityService _tInternalControlsActivityService;
    private readonly TInternalControlsEvaluationService _tInternalControlsEvaluationService;
    private readonly TInternalControlsReportService _tInternalControlsReportService;



    public ScheduledJobPuller(
            RISKDBContext dbContext,
            ILogger<ScheduledJobPuller> logger,
            MRiskFactorService mRiskFactorService,
            IServiceProvider serviceProvider
            , MRiskUniverseService mRiskUniverseService
        , MRiskOwnerService mRiskOwnerService
        , MRiskLevelService mRiskLevelService
        , MRiskTreatmentOptionService mRiskTreatmentOptionService
         , MRiskBtableService mRiskBtableService
        , TRiskKpiService tRiskKpiService
        , TRiskimpactService tRiskimpactService
        , TRiskTriggerService tRiskTriggerService
        , TRiskRootCauseService tRiskRootCauseService
        , TRiskPlanExistingControlService tRiskPlanExistingControlService
        , TRiskEmergencyPlanService tRiskEmergencyPlanService
        , TRiskLeadingService tRiskLeadingService
            , TRiskLaggingService tRiskLaggingService
            , TRiskAtableService tRiskAtableService
            , TRiskDataHistoryService tRiskDataHistoryService
            , TRiskCtableService tRiskCtableService
            , TRiskAfterPlanService tRiskAfterPlanService
            , TRiskPerformanceService tRiskPerformanceService
            , TRiskExistingControlService tRiskExistingControlService
            , TRiskResultService tRiskResultService
            , TRiskLevelService tRiskLevelService
            , TInternalControlsActivityService tInternalControlsActivityService
            , TInternalControlsEvaluationService tInternalControlsEvaluationService
            , TInternalControlsReportService tInternalControlsReportService


        )
    {
        _dbContext = dbContext;
        _logger = logger;
        _mRiskFactorService = mRiskFactorService;
        _serviceProvider = serviceProvider;
        _logger.LogInformation("ScheduledJobPuller started.");
        _mRiskUniverseService = mRiskUniverseService;
        _mRiskOwnerService = mRiskOwnerService;
        _mRiskLevelService = mRiskLevelService;
        _mRiskTreatmentOptionService = mRiskTreatmentOptionService;
        _mRiskBtableService = mRiskBtableService;
        _tRiskKpiService = tRiskKpiService;
        _tRiskimpactService = tRiskimpactService;
        _tRiskTriggerService = tRiskTriggerService;
        _tRiskRootCauseService = tRiskRootCauseService;
        _tRiskPlanExistingControlService = tRiskPlanExistingControlService;
        _tRiskEmergencyPlanService = tRiskEmergencyPlanService;
        _tRiskLeadingService = tRiskLeadingService;
        _tRiskLaggingService = tRiskLaggingService;
        _tRiskAtableService = tRiskAtableService;
        _tRiskDataHistoryService = tRiskDataHistoryService;
        _tRiskCtableService = tRiskCtableService;
        _tRiskAfterPlanService = tRiskAfterPlanService;
        _tRiskPerformanceService = tRiskPerformanceService;
        _tRiskExistingControlService = tRiskExistingControlService;
        _tRiskResultService = tRiskResultService;
        _tRiskLevelService = tRiskLevelService;
        _tInternalControlsActivityService = tInternalControlsActivityService;
        _tInternalControlsEvaluationService = tInternalControlsEvaluationService;
        _tInternalControlsReportService = tInternalControlsReportService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        // สร้าง scope ใหม่สำหรับ Job นี้
        using (var scope = _serviceProvider.CreateScope())
        {
            // ดึงค่า jobName จาก JobDataMap
            var jobName = context.JobDetail.JobDataMap.GetString("JobName");
            _logger.LogInformation($"Executing job: {jobName}");

            try
            {
                var serviceProvider = scope.ServiceProvider;
                switch (jobName)
                {
                    case "risk-factors":
                        await _mRiskFactorService.BatchEndOfDay_MRiskFactor_schedule();
                        break;
                    case "risk-types":
                        await _serviceProvider.GetRequiredService<MRiskTypeService>().BatchEndOfDay_MRiskType();
                        break;
                    case "risk-universe":
                        SearchRiskRiskUniverse Msearch = new SearchRiskRiskUniverse
                        {
                            page = 1,
                            pageSize = 1000,
                            keyword = ""
                        };
                        await _mRiskUniverseService.BatchEndOfDay_MRiskUniverse(Msearch);
                        break;
                    case "risk-owners":
                        SearchRiskOwnerModels MRo = new SearchRiskOwnerModels
                        {
                            page = 1,
                            pageSize = 1000,
                            keyword = ""
                        };
                        await _mRiskOwnerService.BatchEndOfDay_MRiskOwner(MRo);
                        break;
                    case "risk-levels":
                        await _mRiskLevelService.BatchEndOfDay_MRiskLevel();
                        break;
                    case "position":
                        SearchRiskTreatmentOptionModels MRt = new SearchRiskTreatmentOptionModels
                        {
                            page = 1,
                            pageSize = 1000,
                            keyword = ""
                        };
                        await _mRiskTreatmentOptionService.BatchEndOfDay_MRiskTreatmentOption(MRt);
                        break;
                    case "b-table":
                        await _mRiskBtableService.BatchEndOfDay_MRiskBTable();
                        break;
                    case "kpis":
                        SearchRiskTkpiModels mkpi = new SearchRiskTkpiModels();
                        await _tRiskKpiService.BatchEndOfDay_MRiskTKpi(mkpi);
                        break;
                    case "impacts":
                        SearchRiskTImpactModels mimp = new SearchRiskTImpactModels();
                        await _tRiskimpactService.BatchEndOfDay_MRiskTImpact(mimp);
                        break;
                    case "triggers":
                        SearchRiskTTriggersModels mtrigger = new SearchRiskTTriggersModels();
                        await _tRiskTriggerService.BatchEndOfDay_MRiskTTrigger(mtrigger);
                        break;
                    case "root-causes":
                        await _tRiskRootCauseService.BatchEndOfDay_MRiskRootCauses(null);
                        break;
                    case "plan-existing-controls":
                        await _tRiskPlanExistingControlService.BatchEndOfDay_RiskExistingControls(null);
                        break;
                    case "emergency-plan":
                        await _tRiskEmergencyPlanService.BatchEndOfDay_MRiskEmergencyPlan(null);
                        break;
                    case "leading":
                        await _tRiskLeadingService.BatchEndOfDay_RiskLeading(null);
                        break;
                    case "lagging":
                        await _tRiskLaggingService.BatchEndOfDay_RiskLagging(null);
                        break;
                    case "a-table":
                        await _tRiskAtableService.BatchEndOfDay_MRiskTAtable(null);
                        break;
                    case "data-history":
                        await _tRiskDataHistoryService.BatchEndOfDay_RiskTRiskDataHistory(null);
                        break;
                    case "c-table":
                        await _tRiskCtableService.BatchEndOfDay_MRiskCtable(null);
                        break;
                    case "after-plan-risk":
                        await _tRiskAfterPlanService.BatchEndOfDay_TRiskAfterPlan(null);
                        break;
                    case "performances":
                        await _tRiskPerformanceService.BatchEndOfDay_RiskPerformancesy(null);
                        break;
                    case "existing-controls":
                        await _tRiskExistingControlService.BatchEndOfDay_MExistingControl(null);
                        break;
                    case "result":
                        await _tRiskResultService.BatchEndOfDay_RiskReult(null);
                        break;
                    case "risk-risk-levels":
                        await _tRiskLevelService.BatchEndOfDay_TRiskLevel(null);
                        break;
                    case "activities":
                        await _tInternalControlsActivityService.BatchEndOfDay_activities_schedule();
                        break;
                    case "evaluations":
                        await _tInternalControlsEvaluationService.BatchEndOfDay_evaluations_schedule();
                        break;
                    case "reports":
                        await _tInternalControlsReportService.BatchEndOfDay_reports_schedule();
                        break;
                    default:
                        // Optionally log unknown job
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error executing job {jobName}.");
            }
        }
    }


}

    // Service สำหรับการลงทะเบียนและรัน Job โดยใช้ IHostedService