using SME_API_RISK.Entities;
using SME_API_RISK.Models;
using SME_API_RISK.Repository;
using SME_API_RISK.Services;
using System.Text.Json;

namespace SME_API_RISK.Service
{
    public class TRiskDataHistoryService
    {
        private readonly TRiskDataHistoryRepository _repository;
        private readonly ICallAPIService _serviceApi;
        private readonly IApiInformationRepository _repositoryApi;
        private readonly string _FlagDev;


        public TRiskDataHistoryService(TRiskDataHistoryRepository repository, IConfiguration configuration, ICallAPIService serviceApi, IApiInformationRepository repositoryApi)

        {
            _repository = repository;
            _serviceApi = serviceApi;
            _repositoryApi = repositoryApi;
            _FlagDev = configuration["Devlopment:FlagDev"] ?? throw new ArgumentNullException("FlagDev is missing in appsettings.json");


        }

        public async Task AddAsync(TRiskDataHistory dataHistory)
        {
            try
            {
                await _repository.AddAsync(dataHistory);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to add TRiskDataHistory: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<TRiskDataHistory>> GetAllAsync()
        {
            try
            {
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to get all TRiskDataHistory: {ex.Message}");
                throw;
            }
        }

        public async Task<TRiskDataHistory?> GetByIdAsync(int riskDefineId)
        {
            try
            {
                return await _repository.GetByIdAsync(riskDefineId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to get TRiskDataHistory by id: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateAsync(TRiskDataHistory dataHistory)
        {
            try
            {
                await _repository.UpdateAsync(dataHistory);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to update TRiskDataHistory: {ex.Message}");
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
                Console.WriteLine($"[ERROR] Service failed to delete TRiskDataHistory: {ex.Message}");
                throw;
            }
        }

        public async Task BatchEndOfDay_RiskTRiskDataHistory(int xid)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
            var RiskTDataHistoryApiResponse = new RiskTDataHistoryApiResponse();
            var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "data-history" });
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
            SearchRiskTDataHistoryModels Msearch = new SearchRiskTDataHistoryModels
            {

                //page = 1,
                //pageSize = 1000,
                RiskDefineId = xid

            };
            var apiResponse = await _serviceApi.GetDataApiAsync(apiParam, Msearch);
            var result = JsonSerializer.Deserialize<RiskTDataHistoryApiResponse>(apiResponse, options);

            RiskTDataHistoryApiResponse = result ?? new RiskTDataHistoryApiResponse();

            if (RiskTDataHistoryApiResponse.ResponseCode == "200" && RiskTDataHistoryApiResponse.data != null)
            {
                foreach (var item in RiskTDataHistoryApiResponse.data)
                {
                    try
                    {
                        var existing = await _repository.GetByIdAsync(item.RiskDefineId);

                        if (existing == null)
                        {
                            // Create new record
                            var newRisk = new TRiskDataHistory
                            {

                           
                                RiskDefineId = item.RiskDefineId,
                                DataOld = item.DataOld,
                                UpdateDate = item.UpdateDate,
                            };

                            await _repository.AddAsync(newRisk);

                        }
                        else if (item.UpdateDate.Value.Date != existing.UpdateDate.Value.Date)
                        {
                            // Update existing record

                             existing.DataOld = item.DataOld;
                            existing.RiskDefineId = item.RiskDefineId;
                            existing.UpdateDate = item.UpdateDate;

                            await _repository.UpdateAsync(existing);

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] Failed to process RiskTRiskDataHistory Id {item.RiskDefineId}: {ex.Message}");
                    }
                }
            }



        }

        public async Task<RiskTDataHistoryApiResponse> GetAllAsyncSearch_RiskTRiskDataHistory(SearchRiskTDataHistoryModels models)
        {
            try
            {
                // ดึงข้อมูลจาก repository
                var RiskTKpis = await _repository.GetAllAsyncSearch_RiskTRiskDataHistory(models);
              if(RiskTKpis == null || !RiskTKpis.Any())
                {
                   await BatchEndOfDay_RiskTRiskDataHistory(models.RiskDefineId);
                    RiskTKpis = await _repository.GetAllAsyncSearch_RiskTRiskDataHistory(models);
                }
                // Mapping ข้อมูล
                var response = new RiskTDataHistoryApiResponse
                {
                    ResponseCode = "200",
                    ResponseMsg = "OK",
                    data = RiskTKpis.Select(r => new RiskTDataHistoryModels
                    {
                        RiskDefineId = r.RiskDefineId,
                        DataOld = r.DataOld,
                        UpdateDate = r.UpdateDate
                    }).ToList(),
                    Timestamp = DateTime.UtcNow
                };

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to search RiskTRiskDataHistory: {ex.Message}");
                return new RiskTDataHistoryApiResponse
                {
                    ResponseCode = "500",
                    ResponseMsg = "Internal Server Error",
                    data = new List<RiskTDataHistoryModels>(),
                    Timestamp = DateTime.UtcNow
                };
            }
        }

    }
}
