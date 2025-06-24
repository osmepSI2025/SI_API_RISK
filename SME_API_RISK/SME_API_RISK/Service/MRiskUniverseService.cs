using SME_API_RISK.Entities;
using SME_API_RISK.Models;
using SME_API_RISK.Repository;
using SME_API_RISK.Services;
using System.Text.Json;

namespace SME_API_RISK.Service
{
    public class MRiskUniverseService
    {
        private readonly MRiskUniverseRepository _repository;
        private readonly ICallAPIService _serviceApi;
        private readonly IApiInformationRepository _repositoryApi;
        private readonly string _FlagDev;
        public MRiskUniverseService(MRiskUniverseRepository repository, IConfiguration configuration, ICallAPIService serviceApi, IApiInformationRepository repositoryApi)
        {

            _repository = repository;
            _serviceApi = serviceApi;
            _repositoryApi = repositoryApi;
            _FlagDev = configuration["Devlopment:FlagDev"] ?? throw new ArgumentNullException("FlagDev is missing in appsettings.json");
        }

        public async Task AddAsync(MRiskUniverse riskUniverse)
        {
            try
            {
                await _repository.AddAsync(riskUniverse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to add MRiskUniverse: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<MRiskUniverse>> GetAllAsync()
        {
            try
            {
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to get all MRiskUniverse: {ex.Message}");
                throw;
            }
        }

        public async Task<MRiskUniverse?> GetByIdAsync(int id)
        {
            try
            {
                return await _repository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to get MRiskUniverse by id: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateAsync(MRiskUniverse riskUniverse)
        {
            try
            {
                await _repository.UpdateAsync(riskUniverse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to update MRiskUniverse: {ex.Message}");
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
                Console.WriteLine($"[ERROR] Service failed to delete MRiskUniverse: {ex.Message}");
                throw;
            }
        }
        public async Task<RiskUniverseApiResponse> GetAllAsyncSearch_RiskUniverse(SearchRiskRiskUniverse models)
        {
            try
            {
                // ดึงข้อมูลจาก repository
                var riskFactors = await _repository.GetAllAsyncSearch_RiskUniverse(models);

                if (riskFactors == null || !riskFactors.Any())
                {
                    await BatchEndOfDay_MRiskUniverse(models);
                    riskFactors = await _repository.GetAllAsyncSearch_RiskUniverse(models);
                  
                }
                // Mapping ข้อมูล
                var response = new RiskUniverseApiResponse
                {
                    ResponseCode = "200",
                    ResponseMsg = "OK",
                    data = riskFactors.Select(r => new RiskUniverseModels
                    {
                        id = r.Id,
                        name = r.Name,
                        updateDate = r.UpdateDate
                    }).ToList(),
                    Timestamp = DateTime.UtcNow
                };

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to search RiskFactors: {ex.Message}");
                return new RiskUniverseApiResponse
                {
                    ResponseCode = "500",
                    ResponseMsg = "Internal Server Error",
                    data = new List<RiskUniverseModels>(),
                    Timestamp = DateTime.UtcNow
                };
            }
        }



        public async Task BatchEndOfDay_MRiskUniverse(SearchRiskRiskUniverse models)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
            var dataResponse = new RiskUniverseApiResponse();
    
            var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "risk-universe" });
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
            var result = JsonSerializer.Deserialize<RiskUniverseApiResponse>(apiResponse, options);

            dataResponse = result ?? new RiskUniverseApiResponse();
            if (dataResponse.ResponseCode == "200" && dataResponse.data != null)
            {
                foreach (var item in dataResponse.data)
                {
                    try
                    {
                        var existingRiskType = await _repository.GetByIdAsync(item.id);

                        if (existingRiskType == null)
                        {
                            // Create new record
                            var newRisk = new MRiskUniverse
                            {
                                Id = item.id,
                                Name = item.name,
                                UpdateDate = item.updateDate
                            };

                            await _repository.AddAsync(newRisk);

                        }
                        else if (item.updateDate.Value.Date != existingRiskType.UpdateDate.Value.Date)
                        {
                            // Update existing record
                            existingRiskType.Id = item.id;
                            existingRiskType.Name = item.name;

                            existingRiskType.UpdateDate = item.updateDate;

                            await _repository.UpdateAsync(existingRiskType);

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] Failed to process RiskFactor ID {item.id}: {ex.Message}");
                    }
                }
            }



        }
    }
}
