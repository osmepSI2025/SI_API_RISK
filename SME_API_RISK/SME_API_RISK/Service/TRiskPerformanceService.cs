using SME_API_RISK.Entities;
using SME_API_RISK.Models;
using SME_API_RISK.Repository;
using SME_API_RISK.Services;
using System.Text.Json;

namespace SME_API_RISK.Service
{
    public class TRiskPerformanceService
    {
        private readonly TRiskPerformanceRepository _repository;
        private readonly ICallAPIService _serviceApi;
        private readonly IApiInformationRepository _repositoryApi;
        private readonly string _FlagDev;


        public TRiskPerformanceService(TRiskPerformanceRepository repository, IConfiguration configuration, ICallAPIService serviceApi, IApiInformationRepository repositoryApi)

        {
            _repository = repository;
            _serviceApi = serviceApi;
            _repositoryApi = repositoryApi;
            _FlagDev = configuration["Devlopment:FlagDev"] ?? throw new ArgumentNullException("FlagDev is missing in appsettings.json");


        }

        public async Task AddAsync(TRiskPerformance performance)
        {
            try
            {
                await _repository.AddAsync(performance);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to add TRiskPerformance: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<TRiskPerformance>> GetAllAsync()
        {
            try
            {
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to get all TRiskPerformance: {ex.Message}");
                throw;
            }
        }

        public async Task<TRiskPerformance?> GetByIdAsync(int riskDefineId)
        {
            try
            {
                return await _repository.GetByIdAsync(riskDefineId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to get TRiskPerformance by id: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateAsync(TRiskPerformance performance)
        {
            try
            {
                await _repository.UpdateAsync(performance);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to update TRiskPerformance: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(int riskDefineId)
        {
            try
            {
                await _repository.DeleteAsync(riskDefineId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to delete TRiskPerformance: {ex.Message}");
                throw;
            }
        }
        public async Task BatchEndOfDay_RiskPerformancesy(SearchTRiskPerformanceModels models)
        {
            if (models == null)
            {
                models = new SearchTRiskPerformanceModels
                {
                    page = 1,
                    pageSize = 1000,
                    riskFactorID = 0 // Default value if not provided
                };
            }
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
            var RiskTRiskPerformanceApiResponse = new RiskTRiskPerformanceApiResponse();

            var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "performances" });
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
           
            var apiResponse = await _serviceApi.GetDataApiAsync(apiParam, models);
            var result = JsonSerializer.Deserialize<RiskTRiskPerformanceApiResponse>(apiResponse, options);

            RiskTRiskPerformanceApiResponse = result ?? new RiskTRiskPerformanceApiResponse();

            if (RiskTRiskPerformanceApiResponse.ResponseCode == "200" && RiskTRiskPerformanceApiResponse.data != null)
            {
                foreach (var item in RiskTRiskPerformanceApiResponse.data)
                {
                    try
                    {
                        var existing = await _repository.GetByIdAsync(item.RiskDefineId);

                        if (existing == null)
                        {
                            // Create new record
                            var newRisk = new TRiskPerformance
                            {

                                RiskDefineId = item.RiskDefineId,
                                Performances = item.Performances,
                                Quarter = item.Quarter,
                                UpdateDate = item.UpdateDate,
                            };

                            await _repository.AddAsync(newRisk);

                        }
                        else if (item.UpdateDate.Value.Date != existing.UpdateDate.Value.Date)
                        {
                            // Update existing record

                            existing.Performances = item.Performances;
                            existing.Quarter = item.Quarter;
                            existing.RiskDefineId = item.RiskDefineId;
                            existing.UpdateDate = item.UpdateDate;

                            await _repository.UpdateAsync(existing);

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] Failed to process RiskTKpi Id {item.RiskDefineId}: {ex.Message}");
                    }
                }
            }



        }

        public async Task<RiskTRiskPerformanceApiResponse> GetAllAsyncSearch_RiskTRiskPerformance(SearchTRiskPerformanceModels models)
        {
            try
            {
                // ดึงข้อมูลจาก repository
                var RiskTKpis = await _repository.GetAllAsyncSearch_RiskTRiskPerformance(models);
                if (RiskTKpis == null || !RiskTKpis.Any())
                {
                    await BatchEndOfDay_RiskPerformancesy(models); // Call the batch process if no data found
                    RiskTKpis = await _repository.GetAllAsyncSearch_RiskTRiskPerformance(models);
                }
                // Mapping ข้อมูล
                var response = new RiskTRiskPerformanceApiResponse
                {
                    ResponseCode = "200",
                    ResponseMsg = "OK",
                    data = RiskTKpis.Select(r => new TRiskPerformanceModels
                    {
                        RiskDefineId = r.RiskDefineId,
                        Quarter = r.Quarter,
                        Performances = r.Performances,
                        UpdateDate = r.UpdateDate
                    }).ToList(),
                    Timestamp = DateTime.UtcNow
                };

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to search RiskTKpis: {ex.Message}");
                return new RiskTRiskPerformanceApiResponse
                {
                    ResponseCode = "500",
                    ResponseMsg = "Internal Server Error",
                    data = new List<TRiskPerformanceModels>(),
                    Timestamp = DateTime.UtcNow
                };
            }
        }

    }
}
