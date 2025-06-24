using SME_API_RISK.Entities;
using SME_API_RISK.Models;
using SME_API_RISK.Repository;
using SME_API_RISK.Services;
using System.Text.Json;

namespace SME_API_RISK.Service
{
    public class TRiskKpiService
    {
        private readonly TRiskKpiRepository _repository;
        private readonly ICallAPIService _serviceApi;
        private readonly IApiInformationRepository _repositoryApi;
        private readonly string _FlagDev;
        public TRiskKpiService(TRiskKpiRepository repository, IConfiguration configuration, ICallAPIService serviceApi, IApiInformationRepository repositoryApi)

        {
            _repository = repository;
            _serviceApi = serviceApi;
            _repositoryApi = repositoryApi;
            _FlagDev = configuration["Devlopment:FlagDev"] ?? throw new ArgumentNullException("FlagDev is missing in appsettings.json");


        }

        public async Task AddAsync(TRiskKpi riskKpi)
        {
            try
            {
                await _repository.AddAsync(riskKpi);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to add TRiskKpi: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<TRiskKpi>> GetAllAsync()
        {
            try
            {
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to get all TRiskKpi: {ex.Message}");
                throw;
            }
        }

        public async Task<TRiskKpi?> GetByIdAsync(int riskDefineId)
        {
            try
            {
                return await _repository.GetByIdAsync(riskDefineId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to get TRiskKpi by Id: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateAsync(TRiskKpi riskKpi)
        {
            try
            {
                await _repository.UpdateAsync(riskKpi);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to update TRiskKpi: {ex.Message}");
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
                Console.WriteLine($"[ERROR] Service failed to delete TRiskKpi: {ex.Message}");
                throw;
            }
        }

        public async Task BatchEndOfDay_MRiskTKpi(SearchRiskTkpiModels models)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
            var RiskTKipsApiResponse = new RiskTKipsApiResponse();
            var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "kpis" });
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
            var result = JsonSerializer.Deserialize<RiskTKipsApiResponse>(apiResponse, options);

            RiskTKipsApiResponse = result ?? new RiskTKipsApiResponse();

            if (RiskTKipsApiResponse.ResponseCode == "200" && RiskTKipsApiResponse.data != null)
            {
                foreach (var item in RiskTKipsApiResponse.data)
                {
                    try
                    {
                        var existing = await _repository.GetByIdAsync(item.RiskDefineId);

                        if (existing == null)
                        {
                            // Create new record
                            var newRisk = new TRiskKpi
                            {

                                Kpis = item.Kpis,
                                RiskDefineId = item.RiskDefineId,

                                UpdateDate = item.UpdateDate,
                            };

                            await _repository.AddAsync(newRisk);

                        }
                        else if (item.UpdateDate.Value.Date != existing.UpdateDate.Value.Date)
                        {
                            // Update existing record

                            existing.Kpis = item.Kpis;
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

        public async Task<RiskTKipsApiResponse> GetAllAsyncSearch_RiskTkpi(SearchRiskTkpiModels models)
        {
            try
            {
                // ดึงข้อมูลจาก repository
                var RiskTKpis = await _repository.GetAllAsyncSearch_RiskTKpi(models);

                if (RiskTKpis == null || !RiskTKpis.Any())
                { 
                    await BatchEndOfDay_MRiskTKpi(models); 
                    RiskTKpis = await _repository.GetAllAsyncSearch_RiskTKpi(models);
                }

                // Mapping ข้อมูล
                var response = new RiskTKipsApiResponse
                {
                    ResponseCode = "200",
                    ResponseMsg = "OK",
                    data = RiskTKpis.Select(r => new RiskTKpisModels
                    {
                        RiskDefineId = r.RiskDefineId,
                        Kpis = r.Kpis,
                        UpdateDate = r.UpdateDate
                    }).ToList(),
                    Timestamp = DateTime.UtcNow
                };

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to search RiskTKpis: {ex.Message}");
                return new RiskTKipsApiResponse
                {
                    ResponseCode = "500",
                    ResponseMsg = "Internal Server Error",
                    data = new List<RiskTKpisModels>(),
                    Timestamp = DateTime.UtcNow
                };
            }
        }

    }
}
