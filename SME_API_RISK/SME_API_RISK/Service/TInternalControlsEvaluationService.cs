using Microsoft.AspNetCore.Mvc;
using SME_API_RISK.Entities;
using SME_API_RISK.Models;
using SME_API_RISK.Repository;
using SME_API_RISK.Services;
using System.Text.Json;
using YamlDotNet.Serialization.NodeTypeResolvers;

namespace SME_API_RISK.Service
{
    public class TInternalControlsEvaluationService
    {
        private readonly TInternalControlsEvaluationRepository _repository;
        private readonly ICallAPIService _serviceApi;
        private readonly IApiInformationRepository _repositoryApi;
        private readonly string _FlagDev;


        public TInternalControlsEvaluationService(TInternalControlsEvaluationRepository repository, IConfiguration configuration, ICallAPIService serviceApi, IApiInformationRepository repositoryApi)

         {
            _repository = repository;
            _serviceApi = serviceApi;
            _repositoryApi = repositoryApi;
            _FlagDev = configuration["Devlopment:FlagDev"] ?? throw new ArgumentNullException("FlagDev is missing in appsettings.json");

        }

        public async Task AddAsync(TInternalControlsEvaluation evaluation)
        {
            try
            {
                await _repository.AddAsync(evaluation);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to add TInternalControlsEvaluation: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<TInternalControlsEvaluation>> GetAllAsync()
        {
            try
            {
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to get all TInternalControlsEvaluation: {ex.Message}");
                throw;
            }
        }

    

        public async Task UpdateAsync(TInternalControlsEvaluation evaluation)
        {
            try
            {
                await _repository.UpdateAsync(evaluation);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to update TInternalControlsEvaluation: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                await _repository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to delete TInternalControlsEvaluation: {ex.Message}");
                throw;
            }
        }
        public async Task BatchEndOfDay_InternalEvaluation(int year)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
            var TInternalControlsEvaluationApiResponse = new TInternalControlsEvaluationApiResponse();
     
            var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "evaluations" });
            var apiParam = LApi.Select(x => new MapiInformationModels
            {
                ServiceNameCode = x.ServiceNameCode,
                ApiKey = x.ApiKey,
                AuthorizationType = x.AuthorizationType,
                ContentType = x.ContentType,
                CreateDate = x.CreateDate,
                Id = x.Id,
                MethodType = x.MethodType,
                ServiceNameTh = x.ServiceNameTh,
                Urldevelopment = x.Urldevelopment,
                Urlproduction = x.Urlproduction,
                Username = x.Username,
                Password = x.Password,
                UpdateDate = x.UpdateDate,
                Bearer = x.Bearer,
            }).FirstOrDefault(); // Use FirstOrDefault to handle empty lists
            SearchTInternalControlsEvaluationModels Msearch = new SearchTInternalControlsEvaluationModels
            {

                page = 1,
                pageSize = 1000,
                riskYear = year,

            };
            var apiResponse = await _serviceApi.GetDataApiAsync(apiParam, Msearch);
            var result = JsonSerializer.Deserialize<TInternalControlsEvaluationApiResponse>(apiResponse, options);

            TInternalControlsEvaluationApiResponse = result ?? new TInternalControlsEvaluationApiResponse();

            if (TInternalControlsEvaluationApiResponse.ResponseCode == "200" && TInternalControlsEvaluationApiResponse.data != null)
            {
                foreach (var item in TInternalControlsEvaluationApiResponse.data)
                {
                    try
                    {
                        var existing = await _repository.GetByIdAsync(item.Departments, item.Activities,year);

                        if (existing == null)
                        {
                            // Create new record
                            var newRisk = new TInternalControlsEvaluation
                            {

                                Activities = item.Activities,
                                Departments = item.Departments,
                                RiskDescription = item.RiskDescription,
                                RiskImpactNeg = item.RiskImpactNeg,
                                RiskRootCause = item.RiskRootCause,
                                ExitingControl = item.ExitingControl,
                                OldWorkPoint = item.OldWorkPoint,
                                ProcessPoint = item.ProcessPoint,
                                ReportPoint = item.ReportPoint,
                                Result = item.Result,
                                RiskYear = year,
                                UpdateDate = item.UpdateDate,
                            };

                            await _repository.AddAsync(newRisk);

                        }
                        else if (item.UpdateDate.Value.Date != existing.UpdateDate.Value.Date)
                        {
                            // Update existing record
                            existing.Activities = item.Activities;
                            existing.Departments = item.Departments;
                             existing.RiskDescription = item.RiskDescription;
                            existing.RiskImpactNeg = item.RiskImpactNeg;
                            existing.RiskRootCause = item.RiskRootCause;
                            existing.ExitingControl = item.ExitingControl;
                            existing.OldWorkPoint = item.OldWorkPoint;
                            existing.ProcessPoint = item.ProcessPoint;
                            existing.ReportPoint = item.ReportPoint;
                            existing.Result = item.Result;
                            existing.UpdateDate = item.UpdateDate;

                            await _repository.UpdateAsync(existing);

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] Failed to process BatchEndOfDay_InternalEvaluation  {item.Activities}: {ex.Message}");
                    }
                }
            }



        }

        public async Task<TInternalControlsEvaluationApiResponse> GetAllAsyncSearch_InternalEvaluation(SearchTInternalControlsEvaluationModels models)
        {
            try
            {
                // ดึงข้อมูลจาก repository
                var RiskTKpis = await _repository.GetAllAsyncSearch_InternalEvaluation(models);


                if (RiskTKpis == null || RiskTKpis.ToList().Count == 0)
                {
                    await BatchEndOfDay_InternalEvaluation(models.riskYear);
                    RiskTKpis = await _repository.GetAllAsyncSearch_InternalEvaluation(models);
                }
                // Mapping ข้อมูล
                var response = new TInternalControlsEvaluationApiResponse
                {
                    ResponseCode = "200",
                    ResponseMsg = "OK",
                    data = RiskTKpis.Select(r => new TInternalControlsEvaluationModels
                    {
                        Activities = r.Activities,
                        Departments = r.Departments,
                        RiskDescription = r.RiskDescription,
                        RiskRootCause = r.RiskRootCause,
                        RiskImpactNeg = r.RiskImpactNeg,
                        ExitingControl = r.ExitingControl,
                        OldWorkPoint = r.OldWorkPoint,
                        ProcessPoint = r.ProcessPoint,
                        ReportPoint = r.ReportPoint,
                        Result = r.Result,
                        UpdateDate = r.UpdateDate
                    }).ToList(),
                    Timestamp = DateTime.UtcNow
                };

                return response;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to search GetAllAsyncSearch_InternalEvaluation: {ex.Message}");
                return new TInternalControlsEvaluationApiResponse
                {
                    ResponseCode = "500",
                    ResponseMsg = "Internal Server Error",
                    data = new List<TInternalControlsEvaluationModels>(),
                    Timestamp = DateTime.UtcNow
                };
            }
        }
        public async Task<int> BatchEndOfDay_evaluations_schedule()
        {
            int currentYear = DateTime.Now.Year;

            foreach (var year in Enumerable.Range(0, 3))
            {
                int christianYear = currentYear - year;
                int buddhistYear = christianYear + 543;

                await BatchEndOfDay_InternalEvaluation(buddhistYear);
            }


            return 1;
        }
    }
}
