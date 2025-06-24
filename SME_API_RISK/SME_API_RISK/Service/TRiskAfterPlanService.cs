using SME_API_RISK.Entities;
using SME_API_RISK.Models;
using SME_API_RISK.Repository;
using SME_API_RISK.Services;
using System.Text.Json;

namespace SME_API_RISK.Service
{
    public class TRiskAfterPlanService
    {
        private readonly TRiskAfterPlanRepository _repository;
        private readonly ICallAPIService _serviceApi;
        private readonly IApiInformationRepository _repositoryApi;
        private readonly string _FlagDev;
        public TRiskAfterPlanService(TRiskAfterPlanRepository repository, IConfiguration configuration, ICallAPIService serviceApi, IApiInformationRepository repositoryApi)

        {
            _repository = repository;
            _serviceApi = serviceApi;
            _repositoryApi = repositoryApi;
            _FlagDev = configuration["Devlopment:FlagDev"] ?? throw new ArgumentNullException("FlagDev is missing in appsettings.json");

        }

        public async Task AddAsync(TRiskAfterPlan entity)
        {
            try
            {
                await _repository.AddAsync(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to add TRiskAfterPlan: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<TRiskAfterPlan>> GetAllAsync()
        {
            try
            {
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to get all TRiskAfterPlan: {ex.Message}");
                throw;
            }
        }

        public async Task<TRiskAfterPlan?> GetByIdAsync(int riskDefineId, string xdefine, int? quater = null)
        {
            try
            {
                return await _repository.GetByIdAsync(riskDefineId, xdefine, quater);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to get TRiskAfterPlan by id: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateAsync(TRiskAfterPlan entity)
        {
            try
            {
                await _repository.UpdateAsync(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to update TRiskAfterPlan: {ex.Message}");
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
                Console.WriteLine($"[ERROR] Service failed to delete TRiskAfterPlan: {ex.Message}");
                throw;
            }
        }
        public async Task BatchEndOfDay_TRiskAfterPlan(int xId, int xyear)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
            var RiskAfterPlanRiskApiResponse = new RiskAfterPlanRiskApiResponse();
            var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "after-plan-risk" });
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
            SearchRiskAfterPlanApiModels Msearch = new SearchRiskAfterPlanApiModels
            {
                riskFactorID = xId,
                riskYear = xyear
            };
            var apiResponse = await _serviceApi.GetDataApiAsync(apiParam, Msearch);
            var result = JsonSerializer.Deserialize<RiskAfterPlanRiskApiResponse>(apiResponse, options);

            RiskAfterPlanRiskApiResponse = result ?? new RiskAfterPlanRiskApiResponse();

            if (RiskAfterPlanRiskApiResponse.responseCode == "200" && RiskAfterPlanRiskApiResponse.data != null)
            {
                foreach (var item in RiskAfterPlanRiskApiResponse.data)
                {
                    if (item.quaterList != null && item.quaterList.Count != 0)
                    {
                        foreach (var manage in item.quaterList)
                        {
                            try
                            {
                                var existing = await _repository.GetByIdAsync(
                                    item.riskDefineID,
                                    item.riskDefine,
                                    manage.quaterNo

                                );

                                if (existing == null)
                                {
                                    var newRecord = new TRiskAfterPlan
                                    {
                                        RiskDefineId = item.riskDefineID,
                                        RiskDefine = item.riskDefine,
                                        QuaterNo = manage.quaterNo,
                                        L = manage.l,
                                        I = manage.i,

                                        YearBudget = xyear,
                                        UpdateDate = manage.updateDate
                                    };
                                    await _repository.AddAsync(newRecord);
                                }
                                else if (manage.updateDate > (existing.UpdateDate ?? DateTime.MinValue))
                                {
                                    existing.RiskDefineId = item.riskDefineID;
                                    existing.RiskDefine = item.riskDefine;
                                    existing.QuaterNo = manage.quaterNo;
                                    existing.L = manage.l;
                                    existing.I = manage.i;

                                    existing.YearBudget = xyear;
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

        public async Task<RiskAfterPlanRiskApiResponse> SearchRiskAfterPlanAsync(SearchRiskAfterPlanApiModels searchModel)
        {
            try
            {
                var plans = await _repository.GetAllAsyncSearch_TRiskAfterPlan(searchModel);
                var response = new RiskAfterPlanRiskApiResponse
                {
                    responseCode = "200",
                    responseMsg = plans != null && plans.Any() ? "OK" : "No data found",
                    data = new List<AfterPlanRiskDataItem>(),
                    timestamp = DateTime.UtcNow
                };

                if (plans != null && plans.Any())
                {
                    response.data = plans
                        .GroupBy(p => p.RiskDefineId)
                        .Select(g => new AfterPlanRiskDataItem
                        {
                            riskDefineID = g.Key,
                            quaterList = g.Select(x => new AfterPlanRiskQuaterItem
                            {
                                quaterNo = x.QuaterNo, // Make sure TRiskAfterPlan has property QuaterNo
                                l = x.L, // Make sure TRiskAfterPlan has property L
                                i = x.I, // Make sure TRiskAfterPlan has property I

                                updateDate = x.UpdateDate ?? DateTime.MinValue
                            }).ToList()
                        }).ToList();
                }
                else
                {
                    await BatchEndOfDay_TRiskAfterPlan(searchModel.riskFactorID, searchModel.riskYear);
                    var plans2 = await _repository.GetAllAsyncSearch_TRiskAfterPlan(searchModel);
                    response = new RiskAfterPlanRiskApiResponse
                    {
                        responseCode = "200",
                        responseMsg = plans2 != null && plans2.Any() ? "OK" : "No data found",
                        data = new List<AfterPlanRiskDataItem>(),
                        timestamp = DateTime.UtcNow
                    };
                    if (plans2 != null && plans2.Any())
                    {
                        response.data = plans2
                            .GroupBy(p => p.RiskDefineId)
                            .Select(g => new AfterPlanRiskDataItem
                            {
                                riskDefineID = g.Key,
                                quaterList = g.Select(x => new AfterPlanRiskQuaterItem
                                {
                                    quaterNo = x.QuaterNo,
                                    l = x.L,
                                    i = x.I,

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
                return new RiskAfterPlanRiskApiResponse
                {
                    responseCode = "500",
                    responseMsg = "Internal server error",
                    data = new List<AfterPlanRiskDataItem>(),
                    timestamp = DateTime.UtcNow
                };
            }
        }

    }
}
