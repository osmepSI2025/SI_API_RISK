using SME_API_RISK.Entities;
using SME_API_RISK.Models;
using SME_API_RISK.Repository;
using SME_API_RISK.Services;
using System.Text.Json;

namespace SME_API_RISK.Service
{
    public class TRiskCtableService
    {
        private readonly TRiskCtableRepository _repository;
        private readonly ICallAPIService _serviceApi;
        private readonly IApiInformationRepository _repositoryApi;
        private readonly string _FlagDev;

        public TRiskCtableService(TRiskCtableRepository repository, IConfiguration configuration, ICallAPIService serviceApi, IApiInformationRepository repositoryApi)

        {
            _repository = repository;
            _serviceApi = serviceApi;
            _repositoryApi = repositoryApi;
            _FlagDev = configuration["Devlopment:FlagDev"] ?? throw new ArgumentNullException("FlagDev is missing in appsettings.json");


        }

        public async Task AddAsync(TRiskCtable entity)
        {
            try
            {
                await _repository.AddAsync(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to add TRiskCtable: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<TRiskCtable>> GetAllAsync()
        {
            try
            {
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to get all TRiskCtable: {ex.Message}");
                throw;
            }
        }

        public async Task<TRiskCtable?> GetByIdAsync(int id)
        {
            try
            {
                return await _repository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to get TRiskCtable by id: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateAsync(TRiskCtable entity)
        {
            try
            {
                await _repository.UpdateAsync(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to update TRiskCtable: {ex.Message}");
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
                Console.WriteLine($"[ERROR] Service failed to delete TRiskCtable: {ex.Message}");
                throw;
            }
        }

        public async Task BatchEndOfDay_MRiskCtable(int xId)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
            var RiskCTableApiResponse = new RiskCTableApiResponse();
            var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "c-table" });
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
            SearchRiskCTableApiModels Msearch = new SearchRiskCTableApiModels
            {

                page = 1,
                pageSize = 1000,
                riskFactorID = xId,


            };
            var apiResponse = await _serviceApi.GetDataApiAsync(apiParam, Msearch);
            var result = JsonSerializer.Deserialize<RiskCTableApiResponse>(apiResponse, options);

            RiskCTableApiResponse = result ?? new RiskCTableApiResponse();

            if (RiskCTableApiResponse.responseCode == "200" && RiskCTableApiResponse.data != null)
            {
                foreach (var item in RiskCTableApiResponse.data)
                {
                    if (item.rootCauseList != null && item.rootCauseList.Count != 0)
                    {
                        foreach (var rootCause in item.rootCauseList)
                        {
                            if (rootCause.solotionsList != null && rootCause.solotionsList.Count != 0)
                            {
                                foreach (var sol in rootCause.solotionsList)
                                {
                                    try
                                    {
                                        var existing = await _repository.GetByIdAsync(
                                            item.riskDefineID,
                                            rootCause.rootCauseType,
                                            rootCause.rootCauseName
                                        );

                                        if (existing == null)
                                        {
                                            var newRecord = new TRiskCtable
                                            {
                                                RiskDefineId = item.riskDefineID,
                                                RootCauseType = rootCause.rootCauseType,
                                                RootCauseName = rootCause.rootCauseName,
                                                Solutions = sol.solotions,
                                                QualityCost = sol.qualityCost,
                                                QuantityCost = sol.quantityCost,
                                                QualityBenefit = sol.qualityBenefit,
                                                QuantityBenefit = sol.quantityenefit,
                                                UpdateDate = rootCause.updateDate
                                            };

                                            await _repository.AddAsync(newRecord);
                                        }
                                        else if (rootCause.updateDate > (existing.UpdateDate ?? DateTime.MinValue))
                                        {
                                            existing.RiskDefineId = item.riskDefineID;
                                            existing.RootCauseType = rootCause.rootCauseType;
                                            existing.RootCauseName = rootCause.rootCauseName;
                                            existing.Solutions = sol.solotions;
                                            existing.QualityCost = sol.qualityCost;
                                            existing.QuantityCost = sol.quantityCost;
                                            existing.QualityBenefit = sol.qualityBenefit;
                                            existing.QuantityBenefit = sol.quantityenefit;
                                            existing.UpdateDate = rootCause.updateDate;

                                            await _repository.UpdateAsync(existing);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"[ERROR] Failed to process TRiskCtable: {ex.Message}");
                                    }
                                }
                            }
                        }
                    }
                }
            }




        }
        public async Task<RiskCTableApiResponse> SearchCTableAsync(SearchRiskCTableApiModels searchModel)
        {
            try
            {
                var plans = await _repository.GetAllAsyncSearch_Ctable(searchModel);

            
                var response = new RiskCTableApiResponse
                {
                    responseCode = "200",
                    responseMsg = plans != null && plans.Any() ? "OK" : "No data found",
                    data = new List<CTableData>(),
                    timestamp = DateTime.UtcNow
                };

                if (plans != null && plans.Any())
                {
                    response.data = plans
                        .GroupBy(p => p.RiskDefineId)
                        .Select(g => new CTableData
                        {
                            riskDefineID = g.Key,
                            rootCauseList = g
                                .GroupBy(x => new { x.RootCauseType, x.RootCauseName, x.UpdateDate })
                                .Select(rcg => new CTableRootCause
                                {
                                    rootCauseType = rcg.Key.RootCauseType,
                                    rootCauseName = rcg.Key.RootCauseName,
                                    updateDate = rcg.Key.UpdateDate ?? DateTime.MinValue,
                                    solotionsList = rcg.Select(sol => new CTableSolotion
                                    {
                                        solotions = sol.Solutions,
                                        qualityCost = sol.QualityCost,
                                        quantityCost = sol.QuantityCost,
                                        qualityBenefit = sol.QualityBenefit,
                                        quantityenefit = sol.QuantityBenefit
                                    }).ToList()
                                }).ToList()
                        }).ToList();
                }
                else 
                {
                    await BatchEndOfDay_MRiskCtable(searchModel.riskFactorID);
                    var plans2 = await _repository.GetAllAsyncSearch_Ctable(searchModel);
                    response = new RiskCTableApiResponse
                    {
                        responseCode = "200",
                        responseMsg = plans2 != null && plans2.Any() ? "OK" : "No data found",
                        data = new List<CTableData>(),
                        timestamp = DateTime.UtcNow
                    };
                    if (plans2 != null && plans2.Any())
                    {
                        response.data = plans2
                            .GroupBy(p => p.RiskDefineId)
                            .Select(g => new CTableData
                            {
                                riskDefineID = g.Key,
                                rootCauseList = g
                                    .GroupBy(x => new { x.RootCauseType, x.RootCauseName, x.UpdateDate })
                                    .Select(rcg => new CTableRootCause
                                    {
                                        rootCauseType = rcg.Key.RootCauseType,
                                        rootCauseName = rcg.Key.RootCauseName,
                                        updateDate = rcg.Key.UpdateDate ?? DateTime.MinValue,
                                        solotionsList = rcg.Select(sol => new CTableSolotion
                                        {
                                            solotions = sol.Solutions,
                                            qualityCost = sol.QualityCost,
                                            quantityCost = sol.QuantityCost,
                                            qualityBenefit = sol.QualityBenefit,
                                            quantityenefit = sol.QuantityBenefit
                                        }).ToList()
                                    }).ToList()
                            }).ToList();
                    }
                }

                    return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to search TRiskCtable: {ex.Message}");
                return new RiskCTableApiResponse
                {
                    responseCode = "500",
                    responseMsg = "Internal server error",
                    data = new List<CTableData>(),
                    timestamp = DateTime.UtcNow
                };
            }
        }


    }
}
