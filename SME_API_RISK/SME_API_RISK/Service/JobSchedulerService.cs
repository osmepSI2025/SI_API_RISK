using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SME_API_RISK.Entities;
using SME_API_RISK.Models;
using SME_API_RISK.Service;
using SME_API_RISK.Services;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class JobSchedulerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public JobSchedulerService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<RISKDBContext>();
                var now = DateTime.Now;
                var jobs = await db.MScheduledJobs
                    .Where(j => j.IsActive == true && j.RunHour == now.Hour && j.RunMinute == now.Minute)
                    .ToListAsync(stoppingToken);

                foreach (var job in jobs)
                {
                    _ = RunJobAsync(job.JobName, scope.ServiceProvider);
                }
            }

            // Check every minute
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private async Task RunJobAsync(string jobName, IServiceProvider serviceProvider)
    {
        switch (jobName)
        {
            case "risk-factors":
                await serviceProvider.GetRequiredService<MRiskFactorService>().BatchEndOfDay_MRiskFactor_schedule();
                break;
            case "risk-types":
                await serviceProvider.GetRequiredService<MRiskTypeService>().BatchEndOfDay_MRiskType();
                break;
            case "risk-universe":
                SearchRiskRiskUniverse Msearch = new SearchRiskRiskUniverse
                {

                    page = 1,
                    pageSize = 1000,
                    keyword = ""

                };
                await serviceProvider.GetRequiredService<MRiskUniverseService>().BatchEndOfDay_MRiskUniverse(Msearch);
                break;
            case "risk-owners":
                SearchRiskOwnerModels MRo = new SearchRiskOwnerModels
                {

                    page = 1,
                    pageSize = 1000,
                    keyword = ""

                };
                await serviceProvider.GetRequiredService<MRiskOwnerService>().BatchEndOfDay_MRiskOwner(MRo);
                break;

            case "risk-levels":
                await serviceProvider.GetRequiredService<MRiskLevelService>().BatchEndOfDay_MRiskLevel();
                break;
            case "position":

                SearchRiskTreatmentOptionModels MRt = new SearchRiskTreatmentOptionModels
                {

                             page = 1,
                             pageSize = 1000,
                             keyword = ""

                         };
                await serviceProvider.GetRequiredService<MRiskTreatmentOptionService>().BatchEndOfDay_MRiskTreatmentOption(MRt);
                break;
            case "b-table":
                await serviceProvider.GetRequiredService<MRiskBtableService>().BatchEndOfDay_MRiskBTable();
                break;
            case "kpis":
                SearchRiskTkpiModels mkpi = new SearchRiskTkpiModels();
                await serviceProvider.GetRequiredService<TRiskKpiService>().BatchEndOfDay_MRiskTKpi(mkpi);
                break;
            case "impacts":
                SearchRiskTImpactModels mimp = new SearchRiskTImpactModels();
                await serviceProvider.GetRequiredService<TRiskimpactService>().BatchEndOfDay_MRiskTImpact(mimp);
                break;
            case "triggers":
                SearchRiskTTriggersModels mtrigger = new SearchRiskTTriggersModels();
                await serviceProvider.GetRequiredService<TRiskTriggerService>().BatchEndOfDay_MRiskTTrigger(mtrigger);
                break;
            case "root-causes":

                await serviceProvider.GetRequiredService<TRiskRootCauseService>().BatchEndOfDay_MRiskRootCauses(null);
                break;
            case "plan-existing-controls":
        
                await serviceProvider.GetRequiredService<TRiskPlanExistingControlService>().BatchEndOfDay_RiskExistingControls(null);
                break;
            case "emergency-plan":
         
                await serviceProvider.GetRequiredService<TRiskEmergencyPlanService>().BatchEndOfDay_MRiskEmergencyPlan(null);
                break;
            case "leading":
               
                await serviceProvider.GetRequiredService<TRiskLeadingService>().BatchEndOfDay_RiskLeading(null);
                break;
            case "lagging":
                
                await serviceProvider.GetRequiredService<TRiskLaggingService>().BatchEndOfDay_RiskLagging(null);
                break;
            case "a-table":

                await serviceProvider.GetRequiredService<TRiskAtableService>().BatchEndOfDay_MRiskTAtable(null);
                break;
            case "data-history":

                await serviceProvider.GetRequiredService<TRiskDataHistoryService>().BatchEndOfDay_RiskTRiskDataHistory(null);
                break;
            case "c-table":

                await serviceProvider.GetRequiredService<TRiskCtableService>().BatchEndOfDay_MRiskCtable(null);
                break;
            case "after-plan-risk":

                await serviceProvider.GetRequiredService<TRiskAfterPlanService>().BatchEndOfDay_TRiskAfterPlan(null);
                break;
            case "performances":

                await serviceProvider.GetRequiredService<TRiskPerformanceService>().BatchEndOfDay_RiskPerformancesy(null);
                break;
            case "existing-controls":

                await serviceProvider.GetRequiredService<TRiskExistingControlService>().BatchEndOfDay_MExistingControl(null);
                break;
            case "result":
             
                await serviceProvider.GetRequiredService<TRiskResultService>().BatchEndOfDay_RiskReult(null);
                break;
            case "risk-risk-levels":

                await serviceProvider.GetRequiredService<TRiskLevelService>().BatchEndOfDay_TRiskLevel(null);
                break;


            case "activities":

                await serviceProvider.GetRequiredService<TInternalControlsActivityService>().BatchEndOfDay_activities_schedule();
                break;
        
            case "evaluations":

                await serviceProvider.GetRequiredService<TInternalControlsEvaluationService>().BatchEndOfDay_evaluations_schedule();
                break;

            case "reports":

                await serviceProvider.GetRequiredService<TInternalControlsReportService>().BatchEndOfDay_reports_schedule();
                break;
            default:
                // Optionally log unknown job
                break;
        }
    }
}