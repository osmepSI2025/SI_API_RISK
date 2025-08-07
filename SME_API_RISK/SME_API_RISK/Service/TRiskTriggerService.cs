using SME_API_RISK.Entities;
using SME_API_RISK.Models;
using SME_API_RISK.Repository;
using SME_API_RISK.Services;
using System.Text.Json;

namespace SME_API_RISK.Service
{
    public class TRiskTriggerService
    {
        private readonly TRiskTriggerRepository _repository;
        private readonly ICallAPIService _serviceApi;
        private readonly IApiInformationRepository _repositoryApi;
        private readonly string _FlagDev;

        public TRiskTriggerService(TRiskTriggerRepository repository, IConfiguration configuration, ICallAPIService serviceApi, IApiInformationRepository repositoryApi)

        {
            _repository = repository;
            _serviceApi = serviceApi;
            _repositoryApi = repositoryApi;
            _FlagDev = configuration["Devlopment:FlagDev"] ?? throw new ArgumentNullException("FlagDev is missing in appsettings.json");


        }

        public async Task AddAsync(TRiskTrigger riskTrigger)
        {
            try
            {
                await _repository.AddAsync(riskTrigger);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to add TRiskTrigger: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<TRiskTrigger>> GetAllAsync()
        {
            try
            {
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to get all TRiskTrigger: {ex.Message}");
                throw;
            }
        }

        public async Task<TRiskTrigger?> GetByIdAsync(int riskDefineId)
        {
            try
            {
                return await _repository.GetByIdAsync(riskDefineId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to get TRiskTrigger by id: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateAsync(TRiskTrigger riskTrigger)
        {
            try
            {
                await _repository.UpdateAsync(riskTrigger);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to update TRiskTrigger: {ex.Message}");
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
                Console.WriteLine($"[ERROR] Service failed to delete TRiskTrigger: {ex.Message}");
                throw;
            }
        }
        public async Task BatchEndOfDay_MRiskTTrigger(SearchRiskTTriggersModels models)
        {
            if (models == null)
            {
                models.page = 1;
                models.pageSize = 1000;
                models.riskFactorID = 0;

            }
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
            var RiskTTriggerApiResponse = new RiskTTriggerApiResponse();
            var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "triggers" });
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
            var result = JsonSerializer.Deserialize<RiskTTriggerApiResponse>(apiResponse, options);

            RiskTTriggerApiResponse = result ?? new RiskTTriggerApiResponse();

            if (RiskTTriggerApiResponse.ResponseCode == "200" && RiskTTriggerApiResponse.data != null)
            {
                foreach (var item in RiskTTriggerApiResponse.data)
                {
                    try
                    {
                        var existing = await _repository.GetByIdAsync(item.RiskDefineId);

                        if (existing == null)
                        {
                            // Create new record
                            var newRisk = new TRiskTrigger
                            {

                                Triggers = item.triggers,
                                RiskDefineId = item.RiskDefineId,

                                UpdateDate = item.UpdateDate,
                            };

                            await _repository.AddAsync(newRisk);

                        }
                        else if (item.UpdateDate.Value.Date != existing.UpdateDate.Value.Date)
                        {
                            // Update existing record

                            existing.Triggers = item.triggers;
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

        public async Task<RiskTTriggerApiResponse> GetAllAsyncSearch_RiskTTrigger(SearchRiskTTriggersModels models)
        {
            try
            {
                // ดึงข้อมูลจาก repository
                var RiskTKpis = await _repository.GetAllAsyncSearch_RiskTTrigger(models);
                if (RiskTKpis == null || !RiskTKpis.Any())
                {
                   await BatchEndOfDay_MRiskTTrigger(models);
                    RiskTKpis = await _repository.GetAllAsyncSearch_RiskTTrigger(models);
                }

                // Mapping ข้อมูล
                var response = new RiskTTriggerApiResponse
                {
                    ResponseCode = "200",
                    ResponseMsg = "OK",
                    data = RiskTKpis.Select(r => new RiskTTriggersModels
                    {
                        RiskDefineId = r.RiskDefineId,
                        triggers = r.Triggers,
                        UpdateDate = r.UpdateDate
                    }).ToList(),
                    Timestamp = DateTime.UtcNow
                };

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to search RiskTKpis: {ex.Message}");
                return new RiskTTriggerApiResponse
                {
                    ResponseCode = "500",
                    ResponseMsg = "Internal Server Error",
                    data = new List<RiskTTriggersModels>(),
                    Timestamp = DateTime.UtcNow
                };
            }
        }

    }
}
