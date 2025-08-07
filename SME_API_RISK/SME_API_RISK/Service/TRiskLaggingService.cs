using SME_API_RISK.Entities;
using SME_API_RISK.Models;
using SME_API_RISK.Repository;
using SME_API_RISK.Services;
using System.Text.Json;

namespace SME_API_RISK.Service
{
    public class TRiskLaggingService
    {
        private readonly TRiskLaggingRepository _repository;
        private readonly ICallAPIService _serviceApi;
        private readonly IApiInformationRepository _repositoryApi;
        private readonly string _FlagDev;

        public TRiskLaggingService(TRiskLaggingRepository repository, IConfiguration configuration, ICallAPIService serviceApi, IApiInformationRepository repositoryApi)
        {
            _repository = repository;
            _serviceApi = serviceApi;
            _repositoryApi = repositoryApi;
            _FlagDev = configuration["Devlopment:FlagDev"] ?? throw new ArgumentNullException("FlagDev is missing in appsettings.json");
        }

        public async Task AddAsync(TRiskLagging entity)
        {
            try
            {
                await _repository.AddAsync(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to add TRiskLagging: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<TRiskLagging>> GetAllAsync()
        {
            try
            {
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to get all TRiskLagging: {ex.Message}");
                throw;
            }
        }

        public async Task<TRiskLagging?> GetByIdAsync(int id)
        {
            try
            {
                return await _repository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to get TRiskLagging by id: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateAsync(TRiskLagging entity)
        {
            try
            {
                await _repository.UpdateAsync(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to update TRiskLagging: {ex.Message}");
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
                Console.WriteLine($"[ERROR] Service failed to delete TRiskLagging: {ex.Message}");
                throw;
            }
        }
        public async Task BatchEndOfDay_RiskLagging(SearchRiskLaggingModels searchModel)
        {
            if (searchModel == null)
            {
                searchModel.page = 1;
                searchModel.pageSize = 1000;
                searchModel.riskFactorID = 0;

            }
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
            var RiskLaggingApiResponse = new RiskLaggingApiResponse();
            var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "Lagging" });
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
      
            var apiResponse = await _serviceApi.GetDataApiAsync(apiParam, searchModel);
            var result = JsonSerializer.Deserialize<RiskLaggingApiResponse>(apiResponse, options);

            RiskLaggingApiResponse = result ?? new RiskLaggingApiResponse();

            if (RiskLaggingApiResponse.responseCode == "200" && RiskLaggingApiResponse.data != null)
            {
                foreach (var item in RiskLaggingApiResponse.data)
                {
                    try
                    {
                        var existing = await _repository.GetByIdAsync(item.riskDefineID);

                        if (existing == null)
                        {
                            // Create new record
                            var newRisk = new TRiskLagging
                            {

                                LaggingIndicator = item.laggingIndicator,
                                RiskDefineId = item.riskDefineID,

                                UpdateDate = item.updateDate,
                            };

                            await _repository.AddAsync(newRisk);

                        }
                        else if (item.updateDate.Value.Date != existing.UpdateDate.Value.Date)
                        {
                            // Update existing record

                            existing.LaggingIndicator = item.laggingIndicator;
                            existing.RiskDefineId = item.riskDefineID;
                            existing.UpdateDate = item.updateDate;

                            await _repository.UpdateAsync(existing);

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] Failed to process BatchEndOfDay_RiskLagging : {ex.Message}");
                    }
                }
            }



        }

        public async Task<RiskLaggingApiResponse> GetAllAsyncSearch_RiskLagging(SearchRiskLaggingModels models)
        {
            try
            {
                // ดึงข้อมูลจาก repository
                var RiskTKpis = await _repository.GetAllAsyncSearch_RiskLagging(models);
                if (RiskTKpis == null || !RiskTKpis.Any())
                {
                    await BatchEndOfDay_RiskLagging(models);
                    var RiskTKpis2 = await _repository.GetAllAsyncSearch_RiskLagging(models);
                    if (RiskTKpis2 == null || !RiskTKpis2.Any())
                    {
                        return new RiskLaggingApiResponse
                        {
                            responseCode = "404",
                            responseMsg = "No data found",
                            data = new List<LaggingIndicatorData>(),
                            timestamp = DateTime.UtcNow
                        };
                    }
                    else
                    {
                        return new RiskLaggingApiResponse
                        {
                            responseCode = "200",
                            responseMsg = "OK",
                            data = RiskTKpis2.Select(r => new LaggingIndicatorData
                            {
                                riskDefineID = r.RiskDefineId,
                                laggingIndicator = r.LaggingIndicator,
                                updateDate = r.UpdateDate
                            }).ToList(),
                            timestamp = DateTime.UtcNow
                        };
                    }
                }
                else
                {
                    return new RiskLaggingApiResponse
                    {
                        responseCode = "200",
                        responseMsg = "OK",
                        data = RiskTKpis.Select(r => new LaggingIndicatorData
                        {
                            riskDefineID = r.RiskDefineId,
                            laggingIndicator = r.LaggingIndicator,
                            updateDate = r.UpdateDate
                        }).ToList(),
                        timestamp = DateTime.UtcNow
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to search RiskTKpis: {ex.Message}");
                return new RiskLaggingApiResponse
                {
                    responseCode = "500",
                    responseMsg = "Internal Server Error",
                    data = new List<LaggingIndicatorData>(),
                    timestamp = DateTime.UtcNow
                };
            }
        }
    }
}
