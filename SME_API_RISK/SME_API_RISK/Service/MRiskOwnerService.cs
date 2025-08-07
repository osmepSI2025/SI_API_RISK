using Microsoft.EntityFrameworkCore;
using SME_API_RISK.Entities;
using SME_API_RISK.Models;
using SME_API_RISK.Repository;
using SME_API_RISK.Services;
using System.Text.Json;

namespace SME_API_RISK.Service
{
    public class MRiskOwnerService
    {
        private readonly MRiskOwnerRepository _repository;
        private readonly ICallAPIService _serviceApi;
        private readonly IApiInformationRepository _repositoryApi;
        private readonly string _FlagDev;


        public MRiskOwnerService(MRiskOwnerRepository repository
, IConfiguration configuration, ICallAPIService serviceApi, IApiInformationRepository repositoryApi)

        {
            _repository = repository;
            _serviceApi = serviceApi;
            _repositoryApi = repositoryApi;
            _FlagDev = configuration["Devlopment:FlagDev"] ?? throw new ArgumentNullException("FlagDev is missing in appsettings.json");

        }

        public async Task AddAsync(MRiskOwner riskOwner)
        {
            try
            {
                await _repository.AddAsync(riskOwner);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to add MRiskOwner: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<MRiskOwner>> GetAllAsync()
        {
            try
            {
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to get all MRiskOwner: {ex.Message}");
                throw;
            }
        }

        public async Task<MRiskOwner?> GetByIdAsync(int id)
        {
            try
            {
                return await _repository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to get MRiskOwner by id: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateAsync(MRiskOwner riskOwner)
        {
            try
            {
                await _repository.UpdateAsync(riskOwner);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to update MRiskOwner: {ex.Message}");
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
                Console.WriteLine($"[ERROR] Service failed to delete MRiskOwner: {ex.Message}");
                throw;
            }
        }

        public async Task BatchEndOfDay_MRiskOwner(SearchRiskOwnerModels models)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
            var RiskOwnerApiResponse = new RiskOwnerApiResponse();

            var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "risk-owners" });
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

            var apiResponse = await _serviceApi.GetDataApiAsync(apiParam, models);
            var result = JsonSerializer.Deserialize<RiskOwnerApiResponse>(apiResponse, options);

            RiskOwnerApiResponse = result ?? new RiskOwnerApiResponse();
            if (RiskOwnerApiResponse.ResponseCode == "200" && RiskOwnerApiResponse.data != null)
            {
                foreach (var item in RiskOwnerApiResponse.data)
                {
                    try
                    {
                        var existingOwner = await _repository.GetByIdAsync(item.id);

                        if (existingOwner == null)
                        {
                            // Create new record
                            var newRisk = new MRiskOwner
                            {
                                Id = item.id,
                                Name = item.name,
                                UpdateDate = item.updateDate
                            };

                            await _repository.AddAsync(newRisk);

                        }
                        else if (item.updateDate.Value.Date != existingOwner.UpdateDate.Value.Date)
                        {
                            // Update existing record
                            existingOwner.Id = item.id;
                            existingOwner.Name = item.name;

                            existingOwner.UpdateDate = item.updateDate;

                            await _repository.UpdateAsync(existingOwner);

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] Failed to process RiskFactor ID {item.id}: {ex.Message}");
                    }
                }
            }



        }

        public async Task<IEnumerable<RiskOwnerApiResponse>> GetAllAsyncSearch_RiskOwner(SearchRiskOwnerModels models)
        {
            try
            {
                // ดึงข้อมูลจาก repository
                var riskFactors = await _repository.GetAllAsyncSearch_RiskOwner(models);

                if (riskFactors == null || riskFactors.Count() == 0)
                {
                    await BatchEndOfDay_MRiskOwner(models);
                    riskFactors = await _repository.GetAllAsyncSearch_RiskOwner(models);
                    if (riskFactors == null || riskFactors.Count() == 0)
                    {
                        return Enumerable.Empty<RiskOwnerApiResponse>();
                    }
                }
               
  

                // Mapping ข้อมูล
                var responseList = new RiskOwnerApiResponse
                {
                    ResponseCode = "200",
                    ResponseMsg = "OK",
                    data = riskFactors.Select(r => new RiskOwnerModels
                    {
                        id = r.Id,
                        name = r.Name,
                        updateDate = r.UpdateDate
                    }).ToList(),
                    Timestamp = DateTime.UtcNow
                };

                return new List<RiskOwnerApiResponse> { responseList };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to search RiskFactors: {ex.Message}");
                return Enumerable.Empty<RiskOwnerApiResponse>();
            }
        }

    }
}
