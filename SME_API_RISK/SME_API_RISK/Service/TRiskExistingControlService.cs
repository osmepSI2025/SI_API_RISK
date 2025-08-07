using SME_API_RISK.Entities;
using SME_API_RISK.Models;
using SME_API_RISK.Repository;
using SME_API_RISK.Services;
using System.Text.Json;

namespace SME_API_RISK.Service
{
    public class TRiskExistingControlService
    {
        private readonly TRiskExistingControlRepository _repository;
        private readonly ICallAPIService _serviceApi;
        private readonly IApiInformationRepository _repositoryApi;
        private readonly string _FlagDev;


        public TRiskExistingControlService(TRiskExistingControlRepository repository, IConfiguration configuration, ICallAPIService serviceApi, IApiInformationRepository repositoryApi)

        {
            _repository = repository;
            _serviceApi = serviceApi;
            _repositoryApi = repositoryApi;
            _FlagDev = configuration["Devlopment:FlagDev"] ?? throw new ArgumentNullException("FlagDev is missing in appsettings.json");

        }

        public async Task AddAsync(TRiskExistingControl entity)
        {
            try
            {
                await _repository.AddAsync(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to add TRiskExistingControl: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<TRiskExistingControl>> GetAllAsync()
        {
            try
            {
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to get all TRiskExistingControl: {ex.Message}");
                throw;
            }
        }

        public async Task<TRiskExistingControl?> GetByIdAsync(int riskDefineId, string rootCauseType, string rootCauseName)
        {
            try
            {
                return await _repository.GetByIdAsync(riskDefineId, rootCauseType, rootCauseName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to get TRiskExistingControl by id: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateAsync(TRiskExistingControl entity)
        {
            try
            {
                await _repository.UpdateAsync(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to update TRiskExistingControl: {ex.Message}");
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
                Console.WriteLine($"[ERROR] Service failed to delete TRiskExistingControl: {ex.Message}");
                throw;
            }
        }
        public async Task BatchEndOfDay_MExistingControl(SearchRiskExistingControlApiModels searchModel)
        {
            if (searchModel == null)
            {
                searchModel.keyword = "";
                searchModel.page = 1;
                searchModel.pageSize = 1000;
                searchModel.riskFactorID = 0;
            }
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
            var RiskExistingControlApiResponse = new RiskExistingControlApiResponse();
            var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "existing-controls" });
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
            var result = JsonSerializer.Deserialize<RiskExistingControlApiResponse>(apiResponse, options);

            RiskExistingControlApiResponse = result ?? new RiskExistingControlApiResponse();

            if (RiskExistingControlApiResponse.responseCode == "200" && RiskExistingControlApiResponse.data != null)
            {
                foreach (var item in RiskExistingControlApiResponse.data)
                {
                    if (item.riskManageList != null && item.riskManageList.Count != 0)
                    {
                        foreach (var manage in item.riskManageList)
                        {
                            try
                            {
                                var existing = await _repository.GetByIdAsync(
                                    item.riskDefineID,
                                    manage.rootCauseType,
                                    manage.rootCauseName
                                );

                                if (existing == null)
                                {
                                    var newRecord = new TRiskExistingControl
                                    {
                                        RiskDefineId = item.riskDefineID,
                                        RootCauseType = manage.rootCauseType,
                                        RootCauseName = manage.rootCauseName,
                                        Performances = manage.performances,
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
        public async Task<RiskExistingControlApiResponse> SearchRiskExistingControlAsync(SearchRiskExistingControlApiModels searchModel)
        {
            try
            {
                var plans = await _repository.GetAllAsync();
                var response = new RiskExistingControlApiResponse
                {
                    responseCode = "200",
                    responseMsg = plans != null && plans.Any() ? "OK" : "No data found",
                    data = new List<ExistingControlData>(),
                    timestamp = DateTime.UtcNow
                };

                if (plans != null && plans.Any())
                {
                    response.data = plans
                        .GroupBy(p => p.RiskDefineId)
                        .Select(g => new ExistingControlData
                        {
                            riskDefineID = g.Key,
                            riskManageList = g.Select(x => new ExistingControlRiskManageItem
                            {
                                rootCauseType = x.RootCauseType,
                                rootCauseName = x.RootCauseName,
                                performances = x.Performances,
                                updateDate = x.UpdateDate ?? DateTime.MinValue
                            }).ToList()
                        }).ToList();
                }
                else
                {
                    await BatchEndOfDay_MExistingControl(searchModel);
                    var plans2 = await _repository.GetAllAsync();
                    response = new RiskExistingControlApiResponse
                    {
                        responseCode = "200",
                        responseMsg = plans2 != null && plans2.Any() ? "OK" : "No data found",
                        data = new List<ExistingControlData>(),
                        timestamp = DateTime.UtcNow
                    };
                    if (plans2 != null && plans2.Any())
                    {
                        response.data = plans2
                            .GroupBy(p => p.RiskDefineId)
                            .Select(g => new ExistingControlData
                            {
                                riskDefineID = g.Key,
                                riskManageList = g.Select(x => new ExistingControlRiskManageItem
                                {
                                    rootCauseType = x.RootCauseType,
                                    rootCauseName = x.RootCauseName,
                                    performances = x.Performances,
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
                return new RiskExistingControlApiResponse
                {
                    responseCode = "500",
                    responseMsg = "Internal server error",
                    data = new List<ExistingControlData>(),
                    timestamp = DateTime.UtcNow
                };
            }
        }

    }
}
