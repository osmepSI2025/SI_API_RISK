using SME_API_RISK.Entities;
using SME_API_RISK.Models;
using SME_API_RISK.Repository;
using SME_API_RISK.Services;
using System.Text.Json;

namespace SME_API_RISK.Service
{
    public class TRiskimpactService
    {
        private readonly TRiskimpactRepository _repository;
        private readonly ICallAPIService _serviceApi;
        private readonly IApiInformationRepository _repositoryApi;
        private readonly string _FlagDev;

        public TRiskimpactService(TRiskimpactRepository repository, IConfiguration configuration, ICallAPIService serviceApi, IApiInformationRepository repositoryApi)

        {
            _repository = repository;
            _serviceApi = serviceApi;
            _repositoryApi = repositoryApi;
            _FlagDev = configuration["Devlopment:FlagDev"] ?? throw new ArgumentNullException("FlagDev is missing in appsettings.json");


        }

        public async Task AddAsync(TRiskimpact riskImpact)
        {
            try
            {
                await _repository.AddAsync(riskImpact);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to add TRiskimpact: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<TRiskimpact>> GetAllAsync()
        {
            try
            {
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to get all TRiskimpact: {ex.Message}");
                throw;
            }
        }

        public async Task<TRiskimpact?> GetByIdAsync(int riskDefineId)
        {
            try
            {
                return await _repository.GetByIdAsync(riskDefineId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to get TRiskimpact by id: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateAsync(TRiskimpact riskImpact)
        {
            try
            {
                await _repository.UpdateAsync(riskImpact);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to update TRiskimpact: {ex.Message}");
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
                Console.WriteLine($"[ERROR] Service failed to delete TRiskimpact: {ex.Message}");
                throw;
            }
        }
        public async Task BatchEndOfDay_MRiskTImpact(SearchRiskTImpactModels models)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
            var RiskTImpactApiResponse = new RiskTImpactApiResponse();
            var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "impacts" });
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
            var result = JsonSerializer.Deserialize<RiskTImpactApiResponse>(apiResponse, options);

            RiskTImpactApiResponse = result ?? new RiskTImpactApiResponse();

            if (RiskTImpactApiResponse.ResponseCode == "200" && RiskTImpactApiResponse.data != null)
            {
                foreach (var item in RiskTImpactApiResponse.data)
                {
                    try
                    {
                        var existing = await _repository.GetByIdAsync(item.RiskDefineId);

                        if (existing == null)
                        {
                            // Create new record
                            var newRisk = new TRiskimpact
                            {

                                Impacts = item.Impacts,

                                RiskDefineId = item.RiskDefineId,

                                UpdateDate = item.UpdateDate,
                            };

                            await _repository.AddAsync(newRisk);

                        }
                        else if (item.UpdateDate.Value.Date != existing.UpdateDate.Value.Date)
                        {
                            // Update existing record

                      
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

        public async Task<RiskTImpactApiResponse> GetAllAsyncSearch_RiskTImpact(SearchRiskTImpactModels models)
        {
            try
            {
                // ดึงข้อมูลจาก repository
                var RiskTKpis = await _repository.GetAllAsyncSearch_RiskTImpact(models);

                if (RiskTKpis == null || !RiskTKpis.Any()) 
                {
                await BatchEndOfDay_MRiskTImpact(models);
                    RiskTKpis = await _repository.GetAllAsyncSearch_RiskTImpact(models);
                }



                    // Mapping ข้อมูล
                    var response = new RiskTImpactApiResponse
                {
                    ResponseCode = "200",
                    ResponseMsg = "OK",
                    data = RiskTKpis.Select(r => new RiskTImpactModels
                    {
                        RiskDefineId = r.RiskDefineId,
                        Impacts = r.Impacts,
                        UpdateDate = r.UpdateDate
                    }).ToList(),
                    Timestamp = DateTime.UtcNow
                };

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to search RiskTKpis: {ex.Message}");
                return new RiskTImpactApiResponse
                {
                    ResponseCode = "500",
                    ResponseMsg = "Internal Server Error",
                    data = new List<RiskTImpactModels>(),
                    Timestamp = DateTime.UtcNow
                };
            }
        }

    }
}
