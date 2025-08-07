using SME_API_RISK.Entities;
using SME_API_RISK.Models;
using SME_API_RISK.Repository;
using SME_API_RISK.Services;
using System.Text.Json;

namespace SME_API_RISK.Service
{
    public class MRiskTypeService
    {
        private readonly MRiskTypeRepository _repository;
        private readonly ICallAPIService _serviceApi;
        private readonly IApiInformationRepository _repositoryApi;
        private readonly string _FlagDev;
        public MRiskTypeService(MRiskTypeRepository repository, IConfiguration configuration, ICallAPIService serviceApi, IApiInformationRepository repositoryApi)
        {
            _repository = repository;
            _serviceApi = serviceApi;
            _repositoryApi = repositoryApi;
            _FlagDev = configuration["Devlopment:FlagDev"] ?? throw new ArgumentNullException("FlagDev is missing in appsettings.json");
        }

        public async Task<IEnumerable<MRiskType>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<MRiskType?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task AddAsync(MRiskType riskType)
        {
            await _repository.AddAsync(riskType);
        }

        public async Task UpdateAsync(MRiskType riskType)
        {
            await _repository.UpdateAsync(riskType);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
        public async Task BatchEndOfDay_MRiskType()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
            var RiskTypeApiResponse = new RiskTypeApiResponse();
          
            var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "risk-types" });
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
            SearchRiskType Msearch = new SearchRiskType
            {

                page = 1,
                pageSize = 1000,

                keyword = ""

            };
            var apiResponse = await _serviceApi.GetDataApiAsync(apiParam, Msearch);
            var result = JsonSerializer.Deserialize<RiskTypeApiResponse>(apiResponse, options);

            RiskTypeApiResponse = result ?? new RiskTypeApiResponse();
            if (RiskTypeApiResponse.ResponseCode == "200" && RiskTypeApiResponse.data != null)
            {
                foreach (var item in RiskTypeApiResponse.data)
                {
                    try
                    {
                        var existingRiskType = await _repository.GetByIdAsync(item.id);

                        if (existingRiskType == null)
                        {
                            // Create new record
                            var newRiskFactor = new MRiskType
                            {
                                Id = item.id,
                                Name = item.name,
                                UpdateDate = item.updateDate
                            };

                            await _repository.AddAsync(newRiskFactor);

                        }
                        else if (item.updateDate.Value.Date != existingRiskType.UpdateDate.Value.Date)
                        {
                            // Update existing record
                            existingRiskType.Id = item.id;
                            existingRiskType.Name = item.name;

                            existingRiskType.UpdateDate = item.updateDate;

                            await _repository.UpdateAsync(existingRiskType);

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] Failed to process RiskFactor ID {item.id}: {ex.Message}");
                    }
                }
            }



        }

        public async Task<RiskUniverseApiResponse> GetAllAsyncSearch_RiskType(SearchRiskType models)
        {
            try
            {
                // ดึงข้อมูลจาก repository
                var riskFactors = await _repository.GetAllAsyncSearch_RiskType(models);

                if (riskFactors == null || !riskFactors.Any())
                {
                    await BatchEndOfDay_MRiskType(); // Call the batch process if no data found
                    riskFactors = await _repository.GetAllAsyncSearch_RiskType(models); // Retry fetching data after batch process

                }
                // Mapping ข้อมูล
                var response = new RiskUniverseApiResponse
                {
                    ResponseCode = "200",
                    ResponseMsg = "OK",
                    data = riskFactors.Select(r => new RiskUniverseModels
                    {
                        id = r.Id,
                        name = r.Name,
                        updateDate = r.UpdateDate
                    }).ToList(),
                    Timestamp = DateTime.UtcNow
                };

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to search RiskFactors: {ex.Message}");
                return new RiskUniverseApiResponse
                {
                    ResponseCode = "500",
                    ResponseMsg = "Internal Server Error",
                    data = new List<RiskUniverseModels>(),
                    Timestamp = DateTime.UtcNow
                };
            }
        }
    }

}
