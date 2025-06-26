using Microsoft.AspNetCore.Mvc;
using SME_API_RISK.Entities;
using SME_API_RISK.Models;
using SME_API_RISK.Repository;
using SME_API_RISK.Services;
using System.Text.Json;

namespace SME_API_RISK.Service
{
    public class MRiskFactorService
    {
        private readonly MRiskFactorRepository _repository;
        private readonly ICallAPIService _serviceApi;
        private readonly IApiInformationRepository _repositoryApi;
        private readonly string _FlagDev;

        public MRiskFactorService(MRiskFactorRepository repository, IConfiguration configuration, ICallAPIService serviceApi, IApiInformationRepository repositoryApi)

        {
            _repository = repository;
            _serviceApi = serviceApi;
            _repositoryApi = repositoryApi;
            _FlagDev = configuration["Devlopment:FlagDev"] ?? throw new ArgumentNullException("FlagDev is missing in appsettings.json");
        }

        public async Task<IEnumerable<MRiskFactor>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<MRiskFactor?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task CreateAsync(MRiskFactor riskFactor)
        {
            await _repository.CreateAsync(riskFactor);
        }

        public async Task<bool> UpdateAsync(MRiskFactor riskFactor)
        {
            return await _repository.UpdateAsync(riskFactor);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
      
        public async Task BatchEndOfDay_MRiskFactor(int xyear,string xkeyword)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
            var riskFactorApiResponse = new RiskFactorApiResponse();
            var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "risk-factors" });
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
            SearchRiskFactor searchRiskFactor = new SearchRiskFactor
            {

                page = 1,
                pageSize = 1000,
                riskYear = xyear,
                keyword = xkeyword,

            };
            var apiResponse = await _serviceApi.GetDataApiAsync(apiParam, searchRiskFactor);
            var result = JsonSerializer.Deserialize<RiskFactorApiResponse>(apiResponse, options);

            riskFactorApiResponse = result ?? new RiskFactorApiResponse();

            if (riskFactorApiResponse.ResponseCode == "200" && riskFactorApiResponse.RiskFactorList != null)
            {
                foreach (var item in riskFactorApiResponse.RiskFactorList)
                {
                    try
                    {
                        var existingRiskFactor = await _repository.GetByIdAsync(item.RiskDefineID);

                        if (existingRiskFactor == null)
                        {
                            // Create new record
                            var newRiskFactor = new MRiskFactor
                            {
                                RiskDefineId = item.RiskDefineID,
                                RiskOwnerName = item.RiskOwnerName,
                                RiskRfname = item.RiskRFName,
                                RiskRootCause = item.RiskRootCause,
                                RiskTypeId = item.RiskTypeId,
                                RiskTypeName = item.RiskTypeName,
                                RiskYear = item.RiskYear,
                                UpdateDate = item.UpdateDate
                            };

                            await _repository.CreateAsync(newRiskFactor);
                            Console.WriteLine($"[INFO] Created new RiskFactor with ID {newRiskFactor.RiskDefineId}");
                        }
                        else if (item.UpdateDate.Date != existingRiskFactor.UpdateDate.Date)
                        {
                            // Update existing record
                            existingRiskFactor.RiskDefineId = item.RiskDefineID;
                            existingRiskFactor.RiskOwnerName = item.RiskOwnerName;
                            existingRiskFactor.RiskRfname = item.RiskRFName;
                            existingRiskFactor.RiskRootCause = item.RiskRootCause;
                            existingRiskFactor.RiskTypeId = item.RiskTypeId;
                            existingRiskFactor.RiskTypeName = item.RiskTypeName;
                            existingRiskFactor.RiskYear = item.RiskYear;
                            existingRiskFactor.UpdateDate = item.UpdateDate;

                            await _repository.UpdateAsync(existingRiskFactor);
                            Console.WriteLine($"[INFO] Updated RiskFactor with ID {existingRiskFactor.RiskDefineId}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] Failed to process RiskFactor ID {item.RiskDefineID}: {ex.Message}");
                    }
                }
            }



        }

        public async Task<RiskFactorApiResponse> GetAllAsyncSearch_RiskFactor(SearchRiskFactor models)
        {
            try
            {
                // ดึงข้อมูลจาก repository
                var riskFactors = await _repository.GetAllAsyncSearch_RiskFactor(models);

                if (riskFactors == null || !riskFactors.Any())
                {
                    await BatchEndOfDay_MRiskFactor(models.riskYear, models.keyword);
                    riskFactors = await _repository.GetAllAsyncSearch_RiskFactor(models);
                   
                }
              
                // Mapping ข้อมูล
                var response = new RiskFactorApiResponse
                {
                    ResponseCode = "200",
                    ResponseMsg = "OK",
                    RiskFactorList = riskFactors.Select(r => new RiskFactorModels
                    {
                        RiskDefineID = r.RiskDefineId,
                        RiskYear = r.RiskYear,
                        RiskRFName = r.RiskRfname,
                        RiskTypeId = r.RiskTypeId,
                        RiskTypeName = r.RiskTypeName,
                        RiskRootCause = r.RiskRootCause,
                        RiskOwnerName = r.RiskOwnerName,
                        UpdateDate = r.UpdateDate
                    }).ToList(),
                    Timestamp = DateTime.UtcNow
                };

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to search RiskFactors: {ex.Message}");
                return new RiskFactorApiResponse
                {
                    ResponseCode = "500",
                    ResponseMsg = "Internal Server Error",
                    RiskFactorList = new List<RiskFactorModels>(),
                    Timestamp = DateTime.UtcNow
                };
            }
        }

        public async Task<int> BatchEndOfDay_MRiskFactor_schedule()
        {
            int currentYear = DateTime.Now.Year;

            foreach (var year in Enumerable.Range(0, 3))
            {
                int christianYear = currentYear - year;
                int buddhistYear = christianYear + 543;
                ///
               await BatchEndOfDay_MRiskFactor(buddhistYear, "");
                ///

            }
            return 1;
        }
    }
}
