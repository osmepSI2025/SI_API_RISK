using SME_API_RISK.Entities;
using SME_API_RISK.Models;
using SME_API_RISK.Repository;
using SME_API_RISK.Services;
using System.Text.Json;

namespace SME_API_RISK.Service
{
    public class TRiskLeadingService
    {
        private readonly TRiskLeadingRepository _repository;
        private readonly ICallAPIService _serviceApi;
        private readonly IApiInformationRepository _repositoryApi;
        private readonly string _FlagDev;


        public TRiskLeadingService(TRiskLeadingRepository repository, IConfiguration configuration, ICallAPIService serviceApi, IApiInformationRepository repositoryApi)
        {
            _repository = repository;
            _serviceApi = serviceApi;
            _repositoryApi = repositoryApi;
            _FlagDev = configuration["Devlopment:FlagDev"] ?? throw new ArgumentNullException("FlagDev is missing in appsettings.json");

        }

        public async Task AddAsync(TRiskLeading entity)
        {
            try
            {
                await _repository.AddAsync(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to add TRiskLeading: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<TRiskLeading>> GetAllAsync()
        {
            try
            {
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to get all TRiskLeading: {ex.Message}");
                throw;
            }
        }

        public async Task<TRiskLeading?> GetByIdAsync(int id)
        {
            try
            {
                return await _repository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to get TRiskLeading by id: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateAsync(TRiskLeading entity)
        {
            try
            {
                await _repository.UpdateAsync(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to update TRiskLeading: {ex.Message}");
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
                Console.WriteLine($"[ERROR] Service failed to delete TRiskLeading: {ex.Message}");
                throw;
            }
        }
        public async Task BatchEndOfDay_RiskLeading(int xid)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
            var RiskLeadingApiResponse = new RiskLeadingApiResponse();
            var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "leading" });
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
            SearchRiskLeadingModels Msearch = new SearchRiskLeadingModels
            {

                riskFactorID = xid,


            };
            var apiResponse = await _serviceApi.GetDataApiAsync(apiParam, Msearch);
            var result = JsonSerializer.Deserialize<RiskLeadingApiResponse>(apiResponse, options);

            RiskLeadingApiResponse = result ?? new RiskLeadingApiResponse();
            if (RiskLeadingApiResponse.responseCode == "200" && RiskLeadingApiResponse.data != null)
            {
                foreach (var item in RiskLeadingApiResponse.data)
                {
                    try
                    {
                        var existing = await _repository.GetByIdAsync(item.riskDefineID);

                        if (existing == null)
                        {
                            // Create new record
                            var newRisk = new TRiskLeading
                            {

                                LeadingIndicator = item.leadingIndicator,
                                RiskDefineId = item.riskDefineID,

                                UpdateDate = item.updateDate,
                            };

                            await _repository.AddAsync(newRisk);

                        }
                        else if (item.updateDate.Value.Date != existing.UpdateDate.Value.Date)
                        {
                            // Update existing record

                            existing.LeadingIndicator = item.leadingIndicator;
                            existing.RiskDefineId = item.riskDefineID;
                            existing.UpdateDate = item.updateDate;

                            await _repository.UpdateAsync(existing);

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] Failed to process BatchEndOfDay_RiskLeading Id {xid}: {ex.Message}");
                    }
                }
            }



        }

        public async Task<RiskLeadingApiResponse> GetAllAsyncSearch_RiskLeading(SearchRiskLeadingModels models)
        {
            try
            {
                // ดึงข้อมูลจาก repository
                var RiskTKpis = await _repository.GetAllAsyncSearch_RiskLeading(models);
                if (RiskTKpis == null || !RiskTKpis.Any())
                {
                    await BatchEndOfDay_RiskLeading(models.riskFactorID);
                    var RiskTKpis2 = await _repository.GetAllAsyncSearch_RiskLeading(models);
                    if (RiskTKpis2 == null || !RiskTKpis2.Any())
                    {
                        return new RiskLeadingApiResponse
                        {
                            responseCode = "404",
                            responseMsg = "No data found",
                            data = new List<LeadingIndicatorData>(),
                            timestamp = DateTime.UtcNow
                        };
                    }
                    else
                    {
                        return new RiskLeadingApiResponse
                        {
                            responseCode = "200",
                            responseMsg = "OK",
                            data = RiskTKpis2.Select(r => new LeadingIndicatorData
                            {
                                riskDefineID = r.RiskDefineId,
                                leadingIndicator = r.LeadingIndicator,
                                updateDate = r.UpdateDate
                            }).ToList(),
                            timestamp = DateTime.UtcNow
                        };
                    }
                }
                else
                {
                    return new RiskLeadingApiResponse
                    {
                        responseCode = "200",
                        responseMsg = "OK",
                        data = RiskTKpis.Select(r => new LeadingIndicatorData
                        {
                            riskDefineID = r.RiskDefineId,
                            leadingIndicator = r.LeadingIndicator,
                            updateDate = r.UpdateDate
                        }).ToList(),
                        timestamp = DateTime.UtcNow
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to search RiskTKpis: {ex.Message}");
                return new RiskLeadingApiResponse
                {
                    responseCode = "500",
                    responseMsg = "Internal Server Error",
                    data = new List<LeadingIndicatorData>(),
                    timestamp = DateTime.UtcNow
                };
            }
        }
    }
}
