using SME_API_RISK.Entities;
using SME_API_RISK.Models;
using SME_API_RISK.Repository;
using SME_API_RISK.Services;
using System.Text.Json;

namespace SME_API_RISK.Service
{
    public class TRiskEmergencyPlanService
    {
        private readonly TRiskEmergencyPlanRepository _repository;
        private readonly ICallAPIService _serviceApi;
        private readonly IApiInformationRepository _repositoryApi;
        private readonly string _FlagDev;

        public TRiskEmergencyPlanService(TRiskEmergencyPlanRepository repository, IConfiguration configuration, ICallAPIService serviceApi, IApiInformationRepository repositoryApi)

        {
            _repository = repository;
            _serviceApi = serviceApi;
            _repositoryApi = repositoryApi;
            _FlagDev = configuration["Devlopment:FlagDev"] ?? throw new ArgumentNullException("FlagDev is missing in appsettings.json");


        }

        public async Task AddAsync(TRiskEmergencyPlan entity)
        {
            try
            {
                await _repository.AddAsync(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to add TRiskEmergencyPlan: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<TRiskEmergencyPlan>> GetAllAsync()
        {
            try
            {
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to get all TRiskEmergencyPlan: {ex.Message}");
                throw;
            }
        }


        public async Task UpdateAsync(TRiskEmergencyPlan entity)
        {
            try
            {
                await _repository.UpdateAsync(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to update TRiskEmergencyPlan: {ex.Message}");
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
                Console.WriteLine($"[ERROR] Service failed to delete TRiskEmergencyPlan: {ex.Message}");
                throw;
            }
        }
        public async Task<RiskEmergencyPlanApiResponse> GetAllAsyncSearch_EmergencyPlan(SearchRiskEmergencyPlanModels searchModel)
        {
            try
            {
                var plans = await _repository.GetAllAsyncSearch_EmergencyPlan(searchModel);
                if (plans == null)
                {
              await BatchEndOfDay_MRiskEmergencyPlan(searchModel);
                    plans = await _repository.GetAllAsyncSearch_EmergencyPlan(searchModel);
                }
                if (plans != null && plans.Count() != 0)
                {
                    var grouped = plans
                       .GroupBy(p => p.RiskDefineId)
                       .Select(g => new RiskEmergencyPlanData
                       {
                           RiskDefineID = g.Key,
                           riskManageList = g.Select(plan => new RiskEmergencyPlanItem
                           {
                               RootCauseType = plan.RootCauseType,
                               RootCauseName = plan.RootCauseName,
                               StandardRiskManage = plan.StandardRiskManage,
                               QplanStart = plan.QplanStart,
                               QplanEnd = plan.QplanEnd,
                               Objectives = plan.Objectives,
                               UpdateDate = plan.UpdateDate
                           }).ToList()
                       }).ToList();

                    var response = new RiskEmergencyPlanApiResponse
                    {
                        ResponseCode = "200",
                        ResponseMsg = "OK",
                        data = grouped,
                        Timestamp = DateTime.UtcNow
                    };

                    return response;
                }
                else
                {
                    await BatchEndOfDay_MRiskEmergencyPlan(searchModel);
                    var rootCauses2 = await _repository.GetAllAsyncSearch_EmergencyPlan(searchModel);
                    if (rootCauses2 != null && rootCauses2.Count() != 0)
                    {
                        var grouped = rootCauses2
                       .GroupBy(r => r.RiskDefineId)
                       .Select(g => new RiskEmergencyPlanData
                       {
                           RiskDefineID = g.Key,
                           riskManageList = g.Select(rc => new RiskEmergencyPlanItem
                           {
                               RootCauseType = rc.RootCauseType,
                               RootCauseName = rc.RootCauseName,
                               StandardRiskManage = rc.StandardRiskManage,
                               QplanStart = rc.QplanStart,
                               QplanEnd = rc.QplanEnd,
                               Objectives = rc.Objectives,
                               UpdateDate = rc.UpdateDate
                           }).ToList()
                       }).ToList();

                        var response = new RiskEmergencyPlanApiResponse
                        {
                            ResponseCode = "200",
                            ResponseMsg = "OK",
                            data = grouped,
                            Timestamp = DateTime.UtcNow
                        };
                        return response;
                    }
                    else
                    {
                        return new RiskEmergencyPlanApiResponse
                        {
                            ResponseCode = "200",
                            ResponseMsg = "No data found",
                            data = new List<RiskEmergencyPlanData>(),
                            Timestamp = DateTime.UtcNow
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to search EmergencyPlan: {ex.Message}");
                return new RiskEmergencyPlanApiResponse
                {
                    ResponseCode = "500",
                    ResponseMsg = "Internal Server Error",
                    data = new List<RiskEmergencyPlanData>(),
                    Timestamp = DateTime.UtcNow
                };
            }
        }
        public async Task BatchEndOfDay_MRiskEmergencyPlan(SearchRiskEmergencyPlanModels searchModel)
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
            var RiskEmergencyPlanApiResponse = new RiskEmergencyPlanApiResponse();
            var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "emergency-plan" });
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
            var result = JsonSerializer.Deserialize<RiskEmergencyPlanApiResponse>(apiResponse, options);

            RiskEmergencyPlanApiResponse = result ?? new RiskEmergencyPlanApiResponse();

            if (RiskEmergencyPlanApiResponse.ResponseCode == "200" && RiskEmergencyPlanApiResponse.data != null)
            {
                foreach (var item in RiskEmergencyPlanApiResponse.data.ToList())
                {
                    if (item.riskManageList.Count() != 0)
                    {
                        foreach (var rootCause in item.riskManageList.ToList())
                        {
                            try
                            {
                                var existing = await _repository.GetByIdAsync(
                                    item.RiskDefineID,
                                     rootCause.RootCauseType,
                                        rootCause.RootCauseName                                


                                );

                                if (existing == null)
                                {
                                    // Create new record
                                    var newRootCause = new TRiskEmergencyPlan
                                    {
                                        RiskDefineId = item.RiskDefineID,
                                        RootCauseType = rootCause.RootCauseType,
                                        RootCauseName = rootCause.RootCauseName,
                                        StandardRiskManage = rootCause.StandardRiskManage,
                                        QplanStart = rootCause.QplanStart,
                                        QplanEnd = rootCause.QplanEnd,
                                        Objectives = rootCause.Objectives,
                                    };

                                    await _repository.AddAsync(newRootCause);
                                }
                                else if (rootCause.UpdateDate.HasValue && existing.UpdateDate.HasValue &&
                                         rootCause.UpdateDate.Value.Date != existing.UpdateDate.Value.Date)
                                {
                                    // Update existing record
                                    existing.RiskDefineId = item.RiskDefineID;
                                    existing.RootCauseName = rootCause.RootCauseName;
                                    existing.StandardRiskManage = rootCause.StandardRiskManage;
                                    existing.QplanStart = rootCause.QplanStart;
                                    existing.QplanEnd = rootCause.QplanEnd;
                                    existing.Objectives = rootCause.Objectives;
                                     existing.RootCauseType = rootCause.RootCauseType;
                                    existing.UpdateDate = rootCause.UpdateDate;

                                    await _repository.UpdateAsync(existing);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"[ERROR] Failed to process TRiskRootCause: {ex.Message}");
                            }
                        }
                    }
                }
            }


        }
    }
}
