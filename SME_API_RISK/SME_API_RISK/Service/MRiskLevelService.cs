using SME_API_RISK.Entities;
using SME_API_RISK.Models;
using SME_API_RISK.Repository;
using SME_API_RISK.Services;
using System.Text.Json;

namespace SME_API_RISK.Service
{
    public class MRiskLevelService
    {
        private readonly MRiskLevelRepository _repository;
        private readonly ICallAPIService _serviceApi;
        private readonly IApiInformationRepository _repositoryApi;
        private readonly string _FlagDev;
        public MRiskLevelService(MRiskLevelRepository repository
, IConfiguration configuration, ICallAPIService serviceApi, IApiInformationRepository repositoryApi)

        {
            _repository = repository;
            _serviceApi = serviceApi;
            _repositoryApi = repositoryApi;
            _FlagDev = configuration["Devlopment:FlagDev"] ?? throw new ArgumentNullException("FlagDev is missing in appsettings.json");

        }

        public async Task AddAsync(MRiskLevel riskLevel)
        {
            try
            {
                await _repository.AddAsync(riskLevel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to add MRiskLevel: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<MRiskLevel>> GetAllAsync()
        {
            try
            {
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to get all MRiskLevel: {ex.Message}");
                throw;
            }
        }

        public async Task<MRiskLevel?> GetByIdAsync(int id)
        {
            try
            {
                return await _repository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to get MRiskLevel by id: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateAsync(MRiskLevel riskLevel)
        {
            try
            {
                await _repository.UpdateAsync(riskLevel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to update MRiskLevel: {ex.Message}");
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
                Console.WriteLine($"[ERROR] Service failed to delete MRiskLevel: {ex.Message}");
                throw;
            }
        }

        public async Task BatchEndOfDay_MRiskLevel()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
            var RiskLevelsApiResponse = new RiskLevelsApiResponse();
            if (_FlagDev == "Y")
            {
                var filePath = Path.GetFullPath("MocData/risk-level.json", AppContext.BaseDirectory);

                try
                {
                    if (!File.Exists(filePath))
                        RiskLevelsApiResponse = new RiskLevelsApiResponse();

                    var jsonString = await File.ReadAllTextAsync(filePath);

                    var result = JsonSerializer.Deserialize<RiskLevelsApiResponse>(jsonString, options);

                    RiskLevelsApiResponse = result ?? new RiskLevelsApiResponse();
                }
                catch (Exception ex)
                {
                    RiskLevelsApiResponse = new RiskLevelsApiResponse();
                }


            }
            else
            {
                var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "risk-levels" });
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
                SearchRiskLevelsModels Msearch = new SearchRiskLevelsModels
                {

                    //page = 1,
                    //pageSize = 1000,
                    keyword = ""

                };
                var apiResponse = await _serviceApi.GetDataApiAsync(apiParam, Msearch);
                var result = JsonSerializer.Deserialize<RiskLevelsApiResponse>(apiResponse, options);

                RiskLevelsApiResponse = result ?? new RiskLevelsApiResponse();
            }

            if (RiskLevelsApiResponse.ResponseCode == "200" && RiskLevelsApiResponse.data != null)
            {
                foreach (var item in RiskLevelsApiResponse.data)
                {
                    try
                    {
                        var existingLevel = await _repository.GetByIdAsync(item.Id);

                        if (existingLevel == null)
                        {
                            // Create new record
                            var newRisk = new MRiskLevel
                            {
                                Id = item.Id,
                                Levels = item.Levels,
                                 ImpactDefine = item.ImpactDefine,
                                LikelihoodDefine = item.LikelihoodDefine,
                                UpdateDate = item.UpdateDate,
                            };

                            await _repository.AddAsync(newRisk);

                        }
                        else if (item.UpdateDate.Value.Date != existingLevel.UpdateDate.Value.Date)
                        {
                            // Update existing record
                            existingLevel.Id = item.Id;
                            existingLevel.Levels = item.Levels;

                            existingLevel.UpdateDate = item.UpdateDate;

                            await _repository.UpdateAsync(existingLevel);

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] Failed to process RiskFactor ID {item.Id}: {ex.Message}");
                    }
                }
            }



        }

        public async Task<RiskLevelsApiResponse> GetAllAsyncSearch_RiskLevels()
        {
            try
            {
                // ดึงข้อมูลจาก repository
                var riskLevels = await _repository.GetAllAsyncSearch_RiskLevels();

                if (riskLevels == null || !riskLevels.Any())
                {
                    await BatchEndOfDay_MRiskLevel();
                    riskLevels = await _repository.GetAllAsyncSearch_RiskLevels();
                    if (riskLevels == null || !riskLevels.Any())
                    {
                        return new RiskLevelsApiResponse
                        {
                            ResponseCode = "404",
                            ResponseMsg = "No data found",
                            data = new List<RiskLevelModels>(),
                            Timestamp = DateTime.UtcNow
                        };
                    }
                }

                // Mapping ข้อมูล
                var response = new RiskLevelsApiResponse
                {
                    ResponseCode = "200",
                    ResponseMsg = "OK",
                    data = riskLevels.Select(r => new RiskLevelModels
                    {
                        Id = r.Id,
                        Levels = r.Levels,
                        ImpactDefine = r.ImpactDefine,
                        LikelihoodDefine = r.LikelihoodDefine,
                        UpdateDate = r.UpdateDate
                    }).ToList(),
                    Timestamp = DateTime.UtcNow
                };

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to search riskLevels: {ex.Message}");
                return new RiskLevelsApiResponse
                {
                    ResponseCode = "500",
                    ResponseMsg = "Internal Server Error",
                    data = new List<RiskLevelModels>(),
                    Timestamp = DateTime.UtcNow
                };
            }
        }
    }
}
