using SME_API_RISK.Entities;
using SME_API_RISK.Models;
using SME_API_RISK.Repository;
using SME_API_RISK.Services;
using System.Text.Json;

namespace SME_API_RISK.Service
{
    public class TRiskPlanExistingControlService
    {
        private readonly TRiskPlanExistingControlRepository _repository;
        private readonly ICallAPIService _serviceApi;
        private readonly IApiInformationRepository _repositoryApi;
        private readonly string _FlagDev;

        public TRiskPlanExistingControlService(TRiskPlanExistingControlRepository repository, IConfiguration configuration, ICallAPIService serviceApi, IApiInformationRepository repositoryApi)

        {
            _repository = repository;
            _serviceApi = serviceApi;
            _repositoryApi = repositoryApi;
            _FlagDev = configuration["Devlopment:FlagDev"] ?? throw new ArgumentNullException("FlagDev is missing in appsettings.json");


        }

        public async Task AddAsync(TRiskPlanExistingControl entity)
        {
            try
            {
                await _repository.AddAsync(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to add TRiskPlanExistingControl: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<TRiskPlanExistingControl>> GetAllAsync()
        {
            try
            {
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to get all TRiskPlanExistingControl: {ex.Message}");
                throw;
            }
        }

        public async Task<TRiskPlanExistingControl?> GetByIdAsync(int riskDefineId, string ExistingControl)
        {
            try
            {
                return await _repository.GetByIdAsync(riskDefineId, ExistingControl);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to get TRiskPlanExistingControl by id: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateAsync(TRiskPlanExistingControl entity)
        {
            try
            {
                await _repository.UpdateAsync(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to update TRiskPlanExistingControl: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(int riskDefineId, string ExistingControl)
        {
            try
            {
                await _repository.DeleteAsync(riskDefineId, ExistingControl);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to delete TRiskPlanExistingControl: {ex.Message}");
                throw;
            }
        }
        public async Task BatchEndOfDay_RiskExistingControls(SearchRiskPlanExistingControlModels searchModel)
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
            var RiskPlanExistingControlApiResponse = new RiskPlanExistingControlApiResponse();
            var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "plan-existing-controls" });
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
            var result = JsonSerializer.Deserialize<RiskPlanExistingControlApiResponse>(apiResponse, options);

            RiskPlanExistingControlApiResponse = result ?? new RiskPlanExistingControlApiResponse();

            if (RiskPlanExistingControlApiResponse.ResponseCode == "200" && RiskPlanExistingControlApiResponse.data != null)
            {
                foreach (var item in RiskPlanExistingControlApiResponse.data.ToList())
                {
                    if (item.existingControlList.Count() != 0)
                    {
                        foreach (var rootCause in item.existingControlList.ToList())
                        {
                            try
                            {
                                var existing = await _repository.GetByIdAsync(
                                    item.RiskDefineID,
                                    rootCause.ExistingControl

                                );

                                if (existing == null)
                                {
                                    // Create new record
                                    var newRootCause = new TRiskPlanExistingControl
                                    {
                                        RiskDefineId = item.RiskDefineID,
                                        ExistingControl = rootCause.ExistingControl,
                                        UpdateDate = rootCause.UpdateDate
                                    };

                                    await _repository.AddAsync(newRootCause);
                                }
                                else if (rootCause.UpdateDate.HasValue && existing.UpdateDate.HasValue &&
                                         rootCause.UpdateDate.Value.Date != existing.UpdateDate.Value.Date)
                                {
                                    // Update existing record
                                    existing.ExistingControl = rootCause.ExistingControl;
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
        public async Task<RiskPlanExistingControlApiResponse> GetAllAsyncSearch_PlanExistingControl(SearchRiskPlanExistingControlModels searchModel)
        {
            try
            {
                var rootCauses = await _repository.GetAllAsyncSearch_PlanExistingControl(searchModel);
                if (rootCauses == null) 
                {
                await BatchEndOfDay_RiskExistingControls(searchModel);
                    rootCauses = await _repository.GetAllAsyncSearch_PlanExistingControl(searchModel);
                }

                if (rootCauses != null && rootCauses.Count() != 0)
                {
                    var grouped = rootCauses
                       .GroupBy(r => r.RiskDefineId)
                       .Select(g => new RiskPlanExistingControlData
                       {
                           RiskDefineID = g.Key,
                           existingControlList = g.Select(rc => new RiskPlanExistingControlItem
                           {
                               ExistingControl = rc.ExistingControl,
                               UpdateDate = rc.UpdateDate
                           }).ToList()
                       }).ToList();

                    var response = new RiskPlanExistingControlApiResponse
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
                    await BatchEndOfDay_RiskExistingControls(searchModel);
                    var rootCauses2 = await _repository.GetAllAsyncSearch_PlanExistingControl(searchModel);
                    if (rootCauses2 != null && rootCauses2.Count() != 0)
                    {
                        var grouped = rootCauses2
                       .GroupBy(r => r.RiskDefineId)
                       .Select(g => new RiskPlanExistingControlData
                       {
                           RiskDefineID = g.Key,
                           existingControlList = g.Select(rc => new RiskPlanExistingControlItem
                           {
                               ExistingControl = rc.ExistingControl,
                               UpdateDate = rc.UpdateDate
                           }).ToList()
                       }).ToList();

                        var response = new RiskPlanExistingControlApiResponse
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
                        return new RiskPlanExistingControlApiResponse
                        {
                            ResponseCode = "200",
                            ResponseMsg = "No data found",
                            data = new List<RiskPlanExistingControlData>(),
                            Timestamp = DateTime.UtcNow
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to search TRootCause: {ex.Message}");
                return new RiskPlanExistingControlApiResponse
                {
                    ResponseCode = "500",
                    ResponseMsg = "Internal Server Error",
                    data = new List<RiskPlanExistingControlData>(),
                    Timestamp = DateTime.UtcNow
                };
            }
        }

    }
}
