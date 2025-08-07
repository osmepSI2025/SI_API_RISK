using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SME_API_RISK.Entities;

public partial class RISKDBContext : DbContext
{
    public RISKDBContext()
    {
    }

    public RISKDBContext(DbContextOptions<RISKDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<MApiInformation> MApiInformations { get; set; }

    public virtual DbSet<MRiskBtable> MRiskBtables { get; set; }

    public virtual DbSet<MRiskFactor> MRiskFactors { get; set; }

    public virtual DbSet<MRiskLevel> MRiskLevels { get; set; }

    public virtual DbSet<MRiskOwner> MRiskOwners { get; set; }

    public virtual DbSet<MRiskTreatmentOption> MRiskTreatmentOptions { get; set; }

    public virtual DbSet<MRiskType> MRiskTypes { get; set; }

    public virtual DbSet<MRiskUniverse> MRiskUniverses { get; set; }

    public virtual DbSet<MScheduledJob> MScheduledJobs { get; set; }

    public virtual DbSet<MScheduledJob1> MScheduledJobs1 { get; set; }

    public virtual DbSet<TInternalControlsActivity> TInternalControlsActivities { get; set; }

    public virtual DbSet<TInternalControlsEvaluation> TInternalControlsEvaluations { get; set; }

    public virtual DbSet<TInternalControlsReport> TInternalControlsReports { get; set; }

    public virtual DbSet<TRiskAfterPlan> TRiskAfterPlans { get; set; }

    public virtual DbSet<TRiskAtable> TRiskAtables { get; set; }

    public virtual DbSet<TRiskCtable> TRiskCtables { get; set; }

    public virtual DbSet<TRiskDataHistory> TRiskDataHistories { get; set; }

    public virtual DbSet<TRiskEmergencyPlan> TRiskEmergencyPlans { get; set; }

    public virtual DbSet<TRiskExistingControl> TRiskExistingControls { get; set; }

    public virtual DbSet<TRiskKpi> TRiskKpis { get; set; }

    public virtual DbSet<TRiskLagging> TRiskLaggings { get; set; }

    public virtual DbSet<TRiskLeading> TRiskLeadings { get; set; }

    public virtual DbSet<TRiskLevel> TRiskLevels { get; set; }

    public virtual DbSet<TRiskPerformance> TRiskPerformances { get; set; }

    public virtual DbSet<TRiskPlanExistingControl> TRiskPlanExistingControls { get; set; }

    public virtual DbSet<TRiskResult> TRiskResults { get; set; }

    public virtual DbSet<TRiskRootCause> TRiskRootCauses { get; set; }

    public virtual DbSet<TRiskTrigger> TRiskTriggers { get; set; }

    public virtual DbSet<TRiskimpact> TRiskimpacts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=192.168.9.155;Database=bluecarg_SME_RISK;User Id=sa;Password=Osmep@2025;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Thai_CI_AS");

        modelBuilder.Entity<MApiInformation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_MApiInformation");

            entity.ToTable("M_ApiInformation", "SME_RISK");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ApiKey).HasMaxLength(150);
            entity.Property(e => e.AuthorizationType).HasMaxLength(50);
            entity.Property(e => e.Bearer).HasColumnType("ntext");
            entity.Property(e => e.ContentType).HasMaxLength(150);
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.MethodType).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(150);
            entity.Property(e => e.ServiceNameCode).HasMaxLength(250);
            entity.Property(e => e.ServiceNameTh).HasMaxLength(250);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.Urldevelopment).HasColumnName("URLDevelopment");
            entity.Property(e => e.Urlproduction).HasColumnName("URLProduction");
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        modelBuilder.Entity<MRiskBtable>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__M_Btable__3214EC07D6AC64C6");

            entity.ToTable("M_RiskBtable", "SME_RISK");

            entity.Property(e => e.OldWork).HasMaxLength(255);
            entity.Property(e => e.Performance).HasMaxLength(255);
            entity.Property(e => e.Process).HasMaxLength(255);
            entity.Property(e => e.Report).HasMaxLength(255);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<MRiskFactor>(entity =>
        {
            entity.HasKey(e => e.RiskFactorId).HasName("PK__RiskFact__7C28B9349C932AB5");

            entity.ToTable("M_RiskFactor", "SME_RISK");

            entity.Property(e => e.RiskFactorId).HasColumnName("RiskFactorID");
            entity.Property(e => e.RiskDefineId).HasColumnName("RiskDefineID");
            entity.Property(e => e.RiskOwnerName).HasMaxLength(255);
            entity.Property(e => e.RiskRfname).HasColumnName("RiskRFName");
            entity.Property(e => e.RiskTypeName).HasMaxLength(255);
        });

        modelBuilder.Entity<MRiskLevel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__M_RiskLe__3214EC0779229861");

            entity.ToTable("M_RiskLevel", "SME_RISK");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ImpactDefine).HasMaxLength(255);
            entity.Property(e => e.LikelihoodDefine).HasMaxLength(255);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<MRiskOwner>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__M_RiskOw__3214EC07E45BEFC2");

            entity.ToTable("M_RiskOwner", "SME_RISK");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<MRiskTreatmentOption>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__M_RiskTr__3214EC07E5E72384");

            entity.ToTable("M_RiskTreatmentOptions", "SME_RISK");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<MRiskType>(entity =>
        {
            entity.ToTable("M_RiskType", "SME_RISK");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("datetime")
                .HasColumnName("updateDate");
        });

        modelBuilder.Entity<MRiskUniverse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__M_RiskUn__3214EC076DFB4D2F");

            entity.ToTable("M_RiskUniverse", "SME_RISK");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<MScheduledJob>(entity =>
        {
            entity.ToTable("M_ScheduledJobs");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.JobName).HasMaxLength(150);
        });

        modelBuilder.Entity<MScheduledJob1>(entity =>
        {
            entity.ToTable("M_ScheduledJobs", "SME_RISK");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.JobName).HasMaxLength(150);
        });

        modelBuilder.Entity<TInternalControlsActivity>(entity =>
        {
            entity.ToTable("T_internalControlsActivities", "SME_RISK");

            entity.Property(e => e.Activities).HasColumnName("activities");
            entity.Property(e => e.Departments).HasColumnName("departments");
            entity.Property(e => e.Process).HasColumnName("process");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TInternalControlsEvaluation>(entity =>
        {
            entity.ToTable("T_internalControlsEvaluations", "SME_RISK");

            entity.Property(e => e.Activities).HasColumnName("activities");
            entity.Property(e => e.Departments).HasColumnName("departments");
            entity.Property(e => e.ExitingControl).HasColumnName("exitingControl");
            entity.Property(e => e.OldWorkPoint).HasColumnName("oldWorkPoint");
            entity.Property(e => e.ProcessPoint).HasColumnName("processPoint");
            entity.Property(e => e.ReportPoint).HasColumnName("reportPoint");
            entity.Property(e => e.Result).HasColumnName("result");
            entity.Property(e => e.RiskDescription).HasColumnName("riskDescription");
            entity.Property(e => e.RiskImpactNeg).HasColumnName("riskImpactNeg");
            entity.Property(e => e.RiskRootCause).HasColumnName("riskRootCause");
            entity.Property(e => e.RiskYear).HasColumnName("riskYear");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TInternalControlsReport>(entity =>
        {
            entity.ToTable("T_internalControlsReports", "SME_RISK");

            entity.Property(e => e.Departments).HasColumnName("departments");
            entity.Property(e => e.Q1).HasColumnName("q1");
            entity.Property(e => e.Q2).HasColumnName("q2");
            entity.Property(e => e.Q3).HasColumnName("q3");
            entity.Property(e => e.Q4).HasColumnName("q4");
            entity.Property(e => e.QuaterFinished).HasColumnName("quaterFinished");
            entity.Property(e => e.Result).HasColumnName("result");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TRiskAfterPlan>(entity =>
        {
            entity.ToTable("T_RiskAfterPlan", "SME_RISK");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.RiskDefineId).HasColumnName("RiskDefineID");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TRiskAtable>(entity =>
        {
            entity.HasKey(e => e.RiskDefineId);

            entity.ToTable("T_RiskAtable", "SME_RISK");

            entity.Property(e => e.RiskDefineId)
                .ValueGeneratedNever()
                .HasColumnName("RiskDefineID");
            entity.Property(e => e.LikelihoodDefine).HasColumnName("likelihoodDefine");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TRiskCtable>(entity =>
        {
            entity.ToTable("T_RiskCtable", "SME_RISK");

            entity.Property(e => e.QualityBenefit).HasMaxLength(255);
            entity.Property(e => e.QualityCost).HasMaxLength(255);
            entity.Property(e => e.QuantityBenefit).HasMaxLength(255);
            entity.Property(e => e.QuantityCost).HasMaxLength(255);
            entity.Property(e => e.RiskDefineId).HasColumnName("RiskDefineID");
            entity.Property(e => e.RootCauseName).HasMaxLength(255);
            entity.Property(e => e.RootCauseType).HasMaxLength(255);
            entity.Property(e => e.Solutions).HasMaxLength(255);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TRiskDataHistory>(entity =>
        {
            entity.HasKey(e => e.RiskDefineId);

            entity.ToTable("T_RiskDataHistory", "SME_RISK");

            entity.Property(e => e.RiskDefineId)
                .ValueGeneratedNever()
                .HasColumnName("RiskDefineID");
            entity.Property(e => e.DataOld).HasColumnName("dataOld");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TRiskEmergencyPlan>(entity =>
        {
            entity.ToTable("T_RiskEmergencyPlan", "SME_RISK");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.QplanEnd)
                .HasMaxLength(50)
                .HasColumnName("QPlanEnd");
            entity.Property(e => e.QplanStart)
                .HasMaxLength(50)
                .HasColumnName("QPlanStart");
            entity.Property(e => e.RiskDefineId).HasColumnName("RiskDefineID");
            entity.Property(e => e.RootCauseName).HasMaxLength(2000);
            entity.Property(e => e.RootCauseType).HasMaxLength(255);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TRiskExistingControl>(entity =>
        {
            entity.ToTable("T_RiskExistingControls", "SME_RISK");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Performances)
                .HasMaxLength(2000)
                .HasColumnName("performances");
            entity.Property(e => e.RiskDefineId).HasColumnName("RiskDefineID");
            entity.Property(e => e.RootCauseName)
                .HasMaxLength(2000)
                .HasColumnName("rootCauseName");
            entity.Property(e => e.RootCauseType)
                .HasMaxLength(255)
                .HasColumnName("rootCauseType");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TRiskKpi>(entity =>
        {
            entity.HasKey(e => e.RiskDefineId).HasName("PK__M_RiskKP__850A50F5434B9247");

            entity.ToTable("T_RiskKPI", "SME_RISK");

            entity.Property(e => e.RiskDefineId)
                .ValueGeneratedNever()
                .HasColumnName("RiskDefineID");
            entity.Property(e => e.Kpis)
                .HasMaxLength(255)
                .HasColumnName("KPIs");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TRiskLagging>(entity =>
        {
            entity.ToTable("T_RiskLagging", "SME_RISK");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.LaggingIndicator)
                .HasMaxLength(255)
                .HasColumnName("laggingIndicator");
            entity.Property(e => e.RiskDefineId).HasColumnName("RiskDefineID");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TRiskLeading>(entity =>
        {
            entity.ToTable("T_RiskLeading", "SME_RISK");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.LeadingIndicator)
                .HasMaxLength(255)
                .HasColumnName("leadingIndicator");
            entity.Property(e => e.RiskDefineId).HasColumnName("RiskDefineID");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TRiskLevel>(entity =>
        {
            entity.ToTable("T_RiskLevels", "SME_RISK");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Colors).HasMaxLength(50);
            entity.Property(e => e.RiskDefine).HasMaxLength(255);
            entity.Property(e => e.RiskDefineId).HasColumnName("RiskDefineID");
            entity.Property(e => e.RiskLevelTitle).HasMaxLength(255);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TRiskPerformance>(entity =>
        {
            entity.ToTable("T_RiskPerformances", "SME_RISK");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Performances).HasColumnName("performances");
            entity.Property(e => e.RiskDefineId).HasColumnName("RiskDefineID");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TRiskPlanExistingControl>(entity =>
        {
            entity.ToTable("T_RiskPlanExistingControls", "SME_RISK");

            entity.Property(e => e.ExistingControl).HasMaxLength(255);
            entity.Property(e => e.RiskDefineId).HasColumnName("RiskDefineID");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TRiskResult>(entity =>
        {
            entity.ToTable("T_RiskResult", "SME_RISK");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Performances)
                .HasMaxLength(2000)
                .HasColumnName("performances");
            entity.Property(e => e.RiskDefineId).HasColumnName("RiskDefineID");
            entity.Property(e => e.RootCauseName)
                .HasMaxLength(2000)
                .HasColumnName("rootCauseName");
            entity.Property(e => e.RootCauseType)
                .HasMaxLength(255)
                .HasColumnName("rootCauseType");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TRiskRootCause>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_MRiskRootCause");

            entity.ToTable("T_RiskRootCause", "SME_RISK");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasDefaultValueSql("('system')");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.RiskDefineId).HasColumnName("RiskDefineID");
            entity.Property(e => e.RootCauseName).HasMaxLength(255);
            entity.Property(e => e.RootCauseType).HasMaxLength(255);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TRiskTrigger>(entity =>
        {
            entity.HasKey(e => e.RiskDefineId).HasName("PK__M_RiskTr__850A50F590FF493F");

            entity.ToTable("T_RiskTriggers", "SME_RISK");

            entity.Property(e => e.RiskDefineId)
                .ValueGeneratedNever()
                .HasColumnName("RiskDefineID");
            entity.Property(e => e.Triggers)
                .HasMaxLength(255)
                .HasColumnName("triggers");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TRiskimpact>(entity =>
        {
            entity.HasKey(e => e.RiskDefineId).HasName("PK__M_Riskim__850A50F50E07D651");

            entity.ToTable("T_Riskimpacts", "SME_RISK");

            entity.Property(e => e.RiskDefineId)
                .ValueGeneratedNever()
                .HasColumnName("RiskDefineID");
            entity.Property(e => e.Impacts).HasMaxLength(255);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
