using Microsoft.EntityFrameworkCore;
using SME_API_RISK.Entities;
using SME_API_RISK.Repository;
using SME_API_RISK.Service;
using SME_API_RISK.Services;
using Quartz;
using Serilog;
try
{
    var builder = WebApplication.CreateBuilder(args);

    // Update the Serilog configuration to use the correct method
    builder.Host.UseSerilog((context, services, loggerConfiguration) => loggerConfiguration
        .ReadFrom.Configuration(context.Configuration) // Removed GetSection("Serilog")
        .WriteTo.File(
            path: "Logs/app-log.txt",
            rollingInterval: RollingInterval.Day,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
        )
    );

    builder.Services.AddDbContext<RISKDBContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString")));
    //Add services to the container.
   builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    // ✅ Register NSwag (Swagger 2.0)
    builder.Services.AddOpenApiDocument(config =>
    {
        config.DocumentName = "API_SME_RISK_v1";
        config.Title = "API SME RISK";
        config.Version = "v1";
        config.Description = "API documentation using Swagger 2.0";
        config.SchemaType = NJsonSchema.SchemaType.Swagger2; // This makes it Swagger 2.0
    });


    builder.Services.AddScoped<MRiskFactorRepository>();
    builder.Services.AddScoped<MRiskFactorService>();
    builder.Services.AddScoped<MRiskTypeRepository>();
    builder.Services.AddScoped<MRiskTypeService>();
    builder.Services.AddScoped<MRiskUniverseService>();
    builder.Services.AddScoped<MRiskUniverseRepository>();
    builder.Services.AddScoped<MRiskOwnerService>();
    builder.Services.AddScoped<MRiskOwnerRepository>();
    builder.Services.AddScoped<MRiskLevelRepository>();
    builder.Services.AddScoped<MRiskLevelService>();
    builder.Services.AddScoped<MRiskTreatmentOptionRepository>();
    builder.Services.AddScoped<MRiskTreatmentOptionService>();
    builder.Services.AddScoped<MRiskBtableService>();
    builder.Services.AddScoped<MRiskBtableRepository>();
    builder.Services.AddScoped<MRiskTreatmentOptionRepository>();
    builder.Services.AddScoped<MRiskTreatmentOptionService>();
    builder.Services.AddScoped<TRiskKpiService>();
    builder.Services.AddScoped<TRiskKpiRepository>();
    builder.Services.AddScoped<TRiskimpactRepository>();
    builder.Services.AddScoped<TRiskimpactService>();
     builder.Services.AddScoped<TRiskTriggerService>();
    builder.Services.AddScoped<TRiskTriggerRepository>();
    builder.Services.AddScoped<TRiskAtableRepository>();
    builder.Services.AddScoped<TRiskAtableService>();
    builder.Services.AddScoped<TRiskDataHistoryRepository>();
    builder.Services.AddScoped<TRiskDataHistoryService>();
    builder.Services.AddScoped<TRiskPerformanceRepository>();
    builder.Services.AddScoped<TRiskPerformanceService>();
    builder.Services.AddScoped<TRiskRootCauseRepository>();
    builder.Services.AddScoped<TRiskRootCauseService>();
    builder.Services.AddScoped<TRiskPlanExistingControlRepository>();
    builder.Services.AddScoped<TRiskPlanExistingControlService>();
    builder.Services.AddScoped<TRiskEmergencyPlanRepository>();
    builder.Services.AddScoped<TRiskEmergencyPlanService>();
    builder.Services.AddScoped<TRiskLeadingRepository>();
    builder.Services.AddScoped<TRiskLeadingService>();
    builder.Services.AddScoped<TRiskLaggingRepository>();
    builder.Services.AddScoped<TRiskLaggingService>();
    builder.Services.AddScoped<TRiskCtableRepository>();
    builder.Services.AddScoped<TRiskCtableService>();
    builder.Services.AddScoped<TRiskExistingControlRepository>();
    builder.Services.AddScoped<TRiskExistingControlService>();
    builder.Services.AddScoped<TRiskResultRepository>();
     builder.Services.AddScoped<TRiskResultService>();
    builder.Services.AddScoped<TRiskLevelRepository>();
    builder.Services.AddScoped<TRiskLevelService>();
    builder.Services.AddScoped<TRiskAfterPlanRepository>();
    builder.Services.AddScoped<TRiskAfterPlanService>();




    //internal
    builder.Services.AddScoped<TInternalControlsActivityRepository>();
    builder.Services.AddScoped<TInternalControlsActivityService>();
    builder.Services.AddScoped<TInternalControlsEvaluationRepository>();
    builder.Services.AddScoped<TInternalControlsEvaluationService>();
    builder.Services.AddScoped<TInternalControlsReportRepository>();
    builder.Services.AddScoped<TInternalControlsReportService>();

    builder.Services.AddScoped<IApiInformationRepository, ApiInformationRepository>();
    builder.Services.AddScoped<ICallAPIService, CallAPIService>(); // Register ICallAPIService with CallAPIService
    builder.Services.AddHttpClient<CallAPIService>();

    // Add Quartz.NET services
    builder.Services.AddQuartz(q =>
    {
        //  q.UseMicrosoftDependencyInjectionScopedJobFactory();
        q.AddJob<ScheduledJobPuller>(j => j.WithIdentity("ScheduledJobPuller").StoreDurably());
    });

    builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

    // Register your IHostedService to manage jobs
    builder.Services.AddHostedService<JobSchedulerService>();


    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
    {
        app.UseOpenApi();     // Serve the Swagger JSON
        app.UseSwaggerUi3();  // Use Swagger UI v3
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}

catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
}