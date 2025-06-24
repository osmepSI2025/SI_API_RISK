using SME_API_RISK.Entities;
using SME_API_RISK.Models;
using SME_API_RISK.Repository;
using SME_API_RISK.Services;
using System.Text.Json;

namespace SME_API_RISK.Service
{
    public class TRiskRootCauseService
    {
        private readonly TRiskRootCauseRepository _repository;
        private readonly ICallAPIService _serviceApi;
        private readonly IApiInformationRepository _repositoryApi;
        private readonly string _FlagDev;



        public TRiskRootCauseService(TRiskRootCauseRepository repository, IConfiguration configuration, ICallAPIService serviceApi, IApiInformationRepository repositoryApi)

        {
            _repository = repository;
            _serviceApi = serviceApi;
            _repositoryApi = repositoryApi;
            _FlagDev = configuration["Devlopment:FlagDev"] ?? throw new ArgumentNullException("FlagDev is missing in appsettings.json");
        }

        public async Task AddAsync(TRiskRootCause rootCause)
        {
            try
            {
                await _repository.AddAsync(rootCause);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to add TRiskRootCause: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<TRiskRootCause>> GetAllAsync()
        {
            try
            {
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to get all TRiskRootCause: {ex.Message}");
                throw;
            }
        }

        public async Task<TRiskRootCause?> GetByIdAsync(int riskDefineId, string rootCauseType, string rootCauseName)
        {
            try
            {
                return await _repository.GetByIdAsync(riskDefineId, rootCauseType, rootCauseName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to get TRiskRootCause by id: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateAsync(TRiskRootCause rootCause)
        {
            try
            {
                await _repository.UpdateAsync(rootCause);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to update TRiskRootCause: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(int riskDefineId, string rootCauseType, string rootCauseName)
        {
            try
            {
                await _repository.DeleteAsync(riskDefineId, rootCauseType, rootCauseName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to delete TRiskRootCause: {ex.Message}");
                throw;
            }
        }

        public async Task BatchEndOfDay_MRiskRootCauses(int xId)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
            var RiskTRootCauseApiResponse = new RiskTRootCauseApiResponse();
            var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "root-causes" });
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
            SearchRiskTRootCauseModels Msearch = new SearchRiskTRootCauseModels
            {

                page = 1,
                pageSize = 1000,
                riskFactorID = xId,


            };
            var apiResponse = await _serviceApi.GetDataApiAsync(apiParam, Msearch);
            var result = JsonSerializer.Deserialize<RiskTRootCauseApiResponse>(apiResponse, options);

            RiskTRootCauseApiResponse = result ?? new RiskTRootCauseApiResponse();

            if (RiskTRootCauseApiResponse.ResponseCode == "200" && RiskTRootCauseApiResponse.data != null)
            {
                foreach (var item in RiskTRootCauseApiResponse.data.ToList())
                {
                    if (item.rootCauseList.Count() != 0)
                    {
                        foreach (var rootCause in item.rootCauseList.ToList())
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
                                    var newRootCause = new TRiskRootCause
                                    {
                                        RiskDefineId = item.RiskDefineID,
                                        RootCauseType = rootCause.RootCauseType,
                                        RootCauseName = rootCause.RootCauseName,
                                        Ratio = decimal.Parse(rootCause.Ratio),
                                        UpdateDate = rootCause.UpdateDate
                                    };

                                    await _repository.AddAsync(newRootCause);
                                }
                                else if (rootCause.UpdateDate.HasValue && existing.UpdateDate.HasValue &&
                                         rootCause.UpdateDate.Value.Date != existing.UpdateDate.Value.Date)
                                {
                                    // Update existing record
                                    existing.Ratio = decimal.Parse(rootCause.Ratio);
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
        public async Task<IEnumerable<RiskTRootCauseApiResponse>> GetAllAsyncSearch_RiskTRootCause(SearchRiskTRootCauseModels searchModel)
        {
            try
            {
                var rootCauses = await _repository.GetAllAsyncSearch_RiskTRootCause(searchModel);

                if (rootCauses == null)
                {
            await BatchEndOfDay_MRiskRootCauses(searchModel.riskFactorID);
                    rootCauses = await _repository.GetAllAsyncSearch_RiskTRootCause(searchModel);
                }
                if (rootCauses != null && (rootCauses.Count() != 0))
                {
                    var grouped = rootCauses
                       .GroupBy(r => r.RiskDefineId)
                       .Select(g => new RiskTRootCauseData
                       {
                           RiskDefineID = g.Key,
                           rootCauseList = g.Select(rc => new RiskTRootCauseItem
                           {
                               RootCauseType = rc.RootCauseType,
                               RootCauseName = rc.RootCauseName,
                               Ratio = rc.Ratio.ToString(),
                               UpdateDate = rc.UpdateDate
                           }).ToList()
                       }).ToList();

                    var response = new RiskTRootCauseApiResponse
                    {
                        ResponseCode = "200",
                        ResponseMsg = "OK",
                        data = grouped,
                        Timestamp = DateTime.UtcNow
                    };
                    return new List<RiskTRootCauseApiResponse> { response };
                }
                else
                {
                   await BatchEndOfDay_MRiskRootCauses(searchModel.riskFactorID);
                    var rootCauses2 = await _repository.GetAllAsyncSearch_RiskTRootCause(searchModel);
                    if (rootCauses2 != null && rootCauses2.Count()!=0 )
                    {
                        var grouped = rootCauses2
                       .GroupBy(r => r.RiskDefineId)
                       .Select(g => new RiskTRootCauseData
                       {
                           RiskDefineID = g.Key,
                           rootCauseList = g.Select(rc => new RiskTRootCauseItem
                           {
                               RootCauseType = rc.RootCauseType,
                               RootCauseName = rc.RootCauseName,
                               Ratio = rc.Ratio.ToString(),
                               UpdateDate = rc.UpdateDate
                           }).ToList()
                       }).ToList();

                        var response = new RiskTRootCauseApiResponse
                        {
                            ResponseCode = "200",
                            ResponseMsg = "OK",
                            data = grouped,
                            Timestamp = DateTime.UtcNow
                        };
                        return new List<RiskTRootCauseApiResponse> { response };
                    }
                    else 
                    {
                        return new List<RiskTRootCauseApiResponse>
                    {
                        new RiskTRootCauseApiResponse
                        {
                            ResponseCode = "200",
                            ResponseMsg = "No data found",
                            data = new List<RiskTRootCauseData>(),
                            Timestamp = DateTime.UtcNow
                        }
                    };
                    }
                   
                }
              

             
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to search TRootCause: {ex.Message}");
                return Enumerable.Empty<RiskTRootCauseApiResponse>();
            }
        }
    }
}
