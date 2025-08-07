using SME_API_RISK.Entities;
using SME_API_RISK.Models;
using SME_API_RISK.Repository;
using SME_API_RISK.Services;
using System.Text.Json;
namespace SME_API_RISK.Service
{
    public class TRiskResultService
    {
        private readonly TRiskResultRepository _repository;
        private readonly ICallAPIService _serviceApi;
        private readonly IApiInformationRepository _repositoryApi;
        private readonly string _FlagDev;



        public TRiskResultService(TRiskResultRepository repository, IConfiguration configuration, ICallAPIService serviceApi, IApiInformationRepository repositoryApi)

        {
            _repository = repository;
            _serviceApi = serviceApi;
            _repositoryApi = repositoryApi;
            _FlagDev = configuration["Devlopment:FlagDev"] ?? throw new ArgumentNullException("FlagDev is missing in appsettings.json");

        }

        public async Task AddAsync(TRiskResult entity)
        {
            try
            {
                await _repository.AddAsync(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to add TRiskResult: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<TRiskResult>> GetAllAsync(SearchRiskResultModels searchModel)
        {
            try
            {
                return await _repository.GetAllAsync(searchModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to get all TRiskResult: {ex.Message}");
                throw;
            }
        }

        public async Task<TRiskResult?> GetByIdAsync(int riskDefineId)
        {
            try
            {
                return await _repository.GetByIdAsync(riskDefineId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to get TRiskResult by id: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateAsync(TRiskResult entity)
        {
            try
            {
                await _repository.UpdateAsync(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to update TRiskResult: {ex.Message}");
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
                Console.WriteLine($"[ERROR] Service failed to delete TRiskResult: {ex.Message}");
                throw;
            }
        }
        public async Task BatchEndOfDay_RiskReult(SearchRiskResultModels searchModel)
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
            var RiskResultApiResponse = new RiskResultApiResponse();
            var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "result" });
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
            var result = JsonSerializer.Deserialize<RiskResultApiResponse>(apiResponse, options);

            RiskResultApiResponse = result ?? new RiskResultApiResponse();

            if (RiskResultApiResponse.responseCode == "200" && RiskResultApiResponse.data != null)
            {
                foreach (var item in RiskResultApiResponse.data)
                {
                    if (item.riskManageList != null && item.riskManageList.Count != 0)
                    {
                        foreach (var manage in item.riskManageList)
                        {
                            try
                            {
                                var existing = await _repository.GetByIdAsync(item.riskDefineID);
                                if (existing == null)
                                {
                                    var newRecord = new TRiskResult
                                    {
                                        RiskDefineId = item.riskDefineID,
                                        RootCauseType = manage.rootCauseType,
                                        RootCauseName = manage.rootCauseName,
                                        Performances = manage.performances,
                                        Status = manage.status,
                                        UpdateDate = manage.updateDate
                                    };
                                    await _repository.AddAsync(newRecord);
                                }
                                else if (manage.updateDate > (existing.UpdateDate ?? DateTime.MinValue))
                                {
                                    existing.RiskDefineId = item.riskDefineID;
                                    existing.RootCauseType = manage.rootCauseType;
                                    existing.RootCauseName = manage.rootCauseName;
                                    existing.Performances = manage.performances;
                                    existing.Status = manage.status;
                                    existing.UpdateDate = manage.updateDate;
                                    await _repository.UpdateAsync(existing);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"[ERROR] Failed to process TRiskResult: {ex.Message}");
                            }
                        }
                    }
                }
            }


        }
        public async Task<RiskResultApiResponse> SearchRiskResullAsync(SearchRiskResultModels searchModel)
        {
            try
            {
                var plans = await _repository.GetAllAsync(searchModel);
                var response = new RiskResultApiResponse
                {
                    responseCode = "200",
                    responseMsg = plans != null && plans.Any() ? "OK" : "No data found",
                    data = new List<ResultData>(),
                    timestamp = DateTime.UtcNow
                };

                if (plans != null && plans.Any())
                {
                    response.data = plans
                        .GroupBy(p => p.RiskDefineId)
                        .Select(g => new ResultData
                        {
                            riskDefineID = g.Key,
                            riskManageList = g.Select(x => new ResultRiskManageItem
                            {
                                rootCauseType = x.RootCauseType,
                                rootCauseName = x.RootCauseName,
                                performances = x.Performances,
                                status = x.Status,
                                updateDate = x.UpdateDate ?? DateTime.MinValue
                            }).ToList()
                        }).ToList();
                }
                else
                {
                    await BatchEndOfDay_RiskReult(searchModel);
                    var plans2 = await _repository.GetAllAsync(searchModel);
                    response = new RiskResultApiResponse
                    {
                        responseCode = "200",
                        responseMsg = plans2 != null && plans2.Any() ? "OK" : "No data found",
                        data = new List<ResultData>(),
                        timestamp = DateTime.UtcNow
                    };
                    if (plans2 != null && plans2.Any())
                    {
                        response.data = plans2
                            .GroupBy(p => p.RiskDefineId)
                            .Select(g => new ResultData
                            {
                                riskDefineID = g.Key,
                                riskManageList = g.Select(x => new ResultRiskManageItem
                                {
                                    rootCauseType = x.RootCauseType,
                                    rootCauseName = x.RootCauseName,
                                    performances = x.Performances,
                                    status = x.Status,
                                    updateDate = x.UpdateDate ?? DateTime.MinValue
                                }).ToList()
                            }).ToList();
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to search TRiskResult: {ex.Message}");
                return new RiskResultApiResponse
                {
                    responseCode = "500",
                    responseMsg = "Internal server error",
                    data = new List<ResultData>(),
                    timestamp = DateTime.UtcNow
                };
            }
        }

    }
}
