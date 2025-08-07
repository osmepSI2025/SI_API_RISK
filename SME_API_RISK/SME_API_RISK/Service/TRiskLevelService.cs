using SME_API_RISK.Entities;
using SME_API_RISK.Models;
using SME_API_RISK.Repository;
using SME_API_RISK.Services;
using System.Text.Json;

namespace SME_API_RISK.Service
{
    public class TRiskLevelService
    {
        private readonly TRiskLevelRepository _repository;
        private readonly ICallAPIService _serviceApi;
        private readonly IApiInformationRepository _repositoryApi;
        private readonly string _FlagDev;


        public TRiskLevelService(TRiskLevelRepository repository, IConfiguration configuration, ICallAPIService serviceApi, IApiInformationRepository repositoryApi)

        {
            _repository = repository;
            _serviceApi = serviceApi;
            _repositoryApi = repositoryApi;
            _FlagDev = configuration["Devlopment:FlagDev"] ?? throw new ArgumentNullException("FlagDev is missing in appsettings.json");


        }

        public async Task AddAsync(TRiskLevel entity)
        {
            try
            {
                await _repository.AddAsync(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to add TRiskLevel: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<TRiskLevel>> GetAllAsync()
        {
            try
            {
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to get all TRiskLevel: {ex.Message}");
                throw;
            }
        }

        public async Task<TRiskLevel?> GetByIdAsync(int riskDefineId, string riskDefine, string riskLevelTitle)
        {
            try
            {
                return await _repository.GetByIdAsync(riskDefineId, riskDefine, riskLevelTitle);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to get TRiskLevel by id: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateAsync(TRiskLevel entity)
        {
            try
            {
                await _repository.UpdateAsync(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to update TRiskLevel: {ex.Message}");
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
                Console.WriteLine($"[ERROR] Service failed to delete TRiskLevel: {ex.Message}");
                throw;
            }
        }
        public async Task BatchEndOfDay_TRiskLevel(SearchTRiskLevelApiModels searchModel)
        {
            if (searchModel==null) 
            {
                searchModel.page = 1;
                searchModel.pageSize = 1000;
                searchModel.riskYear = 0;
                searchModel.riskFactorID = 0; // Default value if not provided
            }
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
            var RiskTLevelApiResponse = new RiskTLevelApiResponse();
            var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "risk-levels-org" });
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
            var result = JsonSerializer.Deserialize<RiskTLevelApiResponse>(apiResponse, options);

            RiskTLevelApiResponse = result ?? new RiskTLevelApiResponse();

            if (RiskTLevelApiResponse.responseCode == "200" && RiskTLevelApiResponse.data != null)
            {
                foreach (var item in RiskTLevelApiResponse.data)
                {
                    if (item.riskLevelList != null && item.riskLevelList.Count != 0)
                    {
                        foreach (var manage in item.riskLevelList)
                        {
                            try
                            {
                                var existing = await _repository.GetByIdAsync(
                                    item.riskDefineID,
                                    item.riskDefine,
                                    manage.riskLevelTitle
                                    
                                );

                                if (existing == null)
                                {
                                    var newRecord = new TRiskLevel
                                    {
                                        RiskDefineId = item.riskDefineID,
                                        RiskDefine = item.riskDefine,
                                        RiskLevelTitle = manage.riskLevelTitle,
                                        L = manage.l,
                                        I = manage.i,
                                        Colors = manage.colors,
                                        YearBudget = searchModel.riskYear,
                                        UpdateDate = manage.updateDate
                                    };
                                    await _repository.AddAsync(newRecord);
                                }
                                else if (manage.updateDate > (existing.UpdateDate ?? DateTime.MinValue))
                                {
                                    existing.RiskDefineId = item.riskDefineID;
                                     existing.RiskDefine = item.riskDefine;
                                    existing.RiskLevelTitle = manage.riskLevelTitle;
                                    existing.L = manage.l;
                                    existing.I = manage.i;
                                    existing.Colors = manage.colors;
                                    existing.YearBudget = searchModel.riskYear;
                                    existing.UpdateDate = manage.updateDate;
                                    await _repository.UpdateAsync(existing);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"[ERROR] Failed to process TRiskExistingControl: {ex.Message}");
                            }
                        }
                    }
                }
            }

        }

        public async Task<RiskTLevelApiResponse> SearchRiskTLevelAsync(SearchTRiskLevelApiModels searchModel)
        {
            try
            {
                var plans = await _repository.GetAllAsyncSearch_TRiskLevel(searchModel);
                var response = new RiskTLevelApiResponse
                {
                    responseCode = "200",
                    responseMsg = plans != null && plans.Any() ? "OK" : "No data found",
                    data = new List<TRiskLevelDataItem>(),
                    timestamp = DateTime.UtcNow
                };

                if (plans != null && plans.Any())
                {
                    response.data = plans
                        .GroupBy(p => p.RiskDefineId)
                        .Select(g => new TRiskLevelDataItem
                        {
                            riskDefineID = g.Key,
                            riskLevelList = g.Select(x => new TRiskLevelListItem
                            {
                                riskLevelTitle = x.RiskLevelTitle,
                                l = x.L, // Make sure TRiskLevel has property L
                                i = x.I, // Make sure TRiskLevel has property I
                                colors = x.Colors, // Make sure TRiskLevel has property Colors
                                
                                updateDate = x.UpdateDate ?? DateTime.MinValue
                            }).ToList()
                        }).ToList();
                }
                else
                {
                    await BatchEndOfDay_TRiskLevel(searchModel);
                    var plans2 = await _repository.GetAllAsyncSearch_TRiskLevel(searchModel);
                    response = new RiskTLevelApiResponse
                    {
                        responseCode = "200",
                        responseMsg = plans2 != null && plans2.Any() ? "OK" : "No data found",
                        data = new List<TRiskLevelDataItem>(),
                        timestamp = DateTime.UtcNow
                    };
                    if (plans2 != null && plans2.Any())
                    {
                        response.data = plans2
                            .GroupBy(p => p.RiskDefineId)
                            .Select(g => new TRiskLevelDataItem
                            {
                                riskDefineID = g.Key,
                                riskLevelList = g.Select(x => new TRiskLevelListItem
                                {
                                    riskLevelTitle = x.RiskLevelTitle,
                                    l = x.L,
                                    i = x.I,
                                    colors = x.Colors,
                                    updateDate = x.UpdateDate ?? DateTime.MinValue
                                }).ToList()
                            }).ToList();
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to search TRiskExistingControl: {ex.Message}");
                return new RiskTLevelApiResponse
                {
                    responseCode = "500",
                    responseMsg = "Internal server error",
                    data = new List<TRiskLevelDataItem>(),
                    timestamp = DateTime.UtcNow
                };
            }
        }

    }
}
