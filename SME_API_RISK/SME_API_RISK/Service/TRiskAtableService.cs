using SME_API_RISK.Entities;
using SME_API_RISK.Models;
using SME_API_RISK.Repository;
using SME_API_RISK.Services;
using System.Text.Json;

namespace SME_API_RISK.Service
{
    public class TRiskAtableService
    {
        private readonly TRiskAtableRepository _repository;
        private readonly ICallAPIService _serviceApi;
        private readonly IApiInformationRepository _repositoryApi;
        private readonly string _FlagDev;

        public TRiskAtableService(TRiskAtableRepository repository, IConfiguration configuration, ICallAPIService serviceApi, IApiInformationRepository repositoryApi)
        {
            _repository = repository;
            _serviceApi = serviceApi;
            _repositoryApi = repositoryApi;
            _FlagDev = configuration["Devlopment:FlagDev"] ?? throw new ArgumentNullException("FlagDev is missing in appsettings.json");

        }

        public async Task AddAsync(TRiskAtable riskAtable)
        {
            try
            {
                await _repository.AddAsync(riskAtable);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to add TRiskAtable: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<TRiskAtable>> GetAllAsync()
        {
            try
            {
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to get all TRiskAtable: {ex.Message}");
                throw;
            }
        }

        public async Task<TRiskAtable?> GetByIdAsync(int riskDefineId)
        {
            try
            {
                return await _repository.GetByIdAsync(riskDefineId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to get TRiskAtable by id: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateAsync(TRiskAtable riskAtable)
        {
            try
            {
                await _repository.UpdateAsync(riskAtable);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to update TRiskAtable: {ex.Message}");
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
                Console.WriteLine($"[ERROR] Service failed to delete TRiskAtable: {ex.Message}");
                throw;
            }
        }
        public async Task BatchEndOfDay_MRiskTAtable(int xid)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
            var RiskTATableApiResponse = new RiskTATableApiResponse();
            var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "a-table" });
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
            SearchRiskTATableModels Msearch = new SearchRiskTATableModels
            {


                RiskDefineId = xid

            };
            var apiResponse = await _serviceApi.GetDataApiAsync(apiParam, Msearch);
            var result = JsonSerializer.Deserialize<RiskTATableApiResponse>(apiResponse, options);

            RiskTATableApiResponse = result ?? new RiskTATableApiResponse();

            if (RiskTATableApiResponse.ResponseCode == "200" && RiskTATableApiResponse.data != null)
            {
                foreach (var item in RiskTATableApiResponse.data)
                {
                    try
                    {
                        var existing = await _repository.GetByIdAsync(item.RiskDefineId);

                        if (existing == null)
                        {
                            // Create new record
                            var newRisk = new TRiskAtable
                            {
                                RiskDefineId = item.RiskDefineId,
                                LikelihoodDefine = item.LikelihoodDefine,
                                ImpactDefine = item.ImpactDefine,

                                UpdateDate = item.UpdateDate,
                            };

                            await _repository.AddAsync(newRisk);

                        }
                        else if (item.UpdateDate.Value.Date != existing.UpdateDate.Value.Date)
                        {
                            // Update existing record

                        
                            existing.RiskDefineId = item.RiskDefineId;
                            existing.LikelihoodDefine = item.LikelihoodDefine;
                            existing.ImpactDefine = item.ImpactDefine;
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

        public async Task<RiskTATableApiResponse> GetAllAsyncSearch_RiskTAtable(SearchRiskTATableModels models)
        {
            try
            {
                // ดึงข้อมูลจาก repository
                var RiskTKpis = await _repository.GetAllAsyncSearch_RiskTAtable(models);
                if (RiskTKpis == null || !RiskTKpis.Any())
                {
                   await BatchEndOfDay_MRiskTAtable(models.RiskDefineId);
                    RiskTKpis = await _repository.GetAllAsyncSearch_RiskTAtable(models);
                }

                // Mapping ข้อมูล
                var response = new RiskTATableApiResponse
                {
                    ResponseCode = "200",
                    ResponseMsg = "OK",
                    data = RiskTKpis.Select(r => new RiskTATableModels
                    {
                        RiskDefineId = r.RiskDefineId,
                        ImpactDefine = r.ImpactDefine,
                        LikelihoodDefine = r.LikelihoodDefine,
                        UpdateDate = r.UpdateDate
                    }).ToList(),
                    Timestamp = DateTime.UtcNow
                };

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to search RiskTKpis: {ex.Message}");
                return new RiskTATableApiResponse
                {
                    ResponseCode = "500",
                    ResponseMsg = "Internal Server Error",
                    data = new List<RiskTATableModels>(),
                    Timestamp = DateTime.UtcNow
                };
            }
        }

    }
}
