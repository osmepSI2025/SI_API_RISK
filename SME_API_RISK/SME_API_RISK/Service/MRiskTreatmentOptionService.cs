using SME_API_RISK.Entities;
using SME_API_RISK.Models;
using SME_API_RISK.Repository;
using SME_API_RISK.Services;
using System.Text.Json;

namespace SME_API_RISK.Service
{
    public class MRiskTreatmentOptionService
    {
        private readonly MRiskTreatmentOptionRepository _repository;
        private readonly ICallAPIService _serviceApi;
        private readonly IApiInformationRepository _repositoryApi;
        private readonly string _FlagDev;


        public MRiskTreatmentOptionService(MRiskTreatmentOptionRepository repository, IConfiguration configuration, ICallAPIService serviceApi, IApiInformationRepository repositoryApi)

        {
            _repository = repository;
            _serviceApi = serviceApi;
            _repositoryApi = repositoryApi;
            _FlagDev = configuration["Devlopment:FlagDev"] ?? throw new ArgumentNullException("FlagDev is missing in appsettings.json");

        }

        public async Task AddAsync(MRiskTreatmentOption treatmentOption)
        {
            try
            {
                await _repository.AddAsync(treatmentOption);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to add MRiskTreatmentOption: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<MRiskTreatmentOption>> GetAllAsync()
        {
            try
            {
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to get all MRiskTreatmentOption: {ex.Message}");
                throw;
            }
        }

        public async Task<MRiskTreatmentOption?> GetByIdAsync(int id)
        {
            try
            {
                return await _repository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to get MRiskTreatmentOption by id: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateAsync(MRiskTreatmentOption treatmentOption)
        {
            try
            {
                await _repository.UpdateAsync(treatmentOption);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to update MRiskTreatmentOption: {ex.Message}");
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
                Console.WriteLine($"[ERROR] Service failed to delete MRiskTreatmentOption: {ex.Message}");
                throw;
            }
        }

        public async Task BatchEndOfDay_MRiskTreatmentOption(SearchRiskTreatmentOptionModels models)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
            var RiskTreatmentOptionApiResponse = new RiskTreatmentOptionApiResponse();
      
            var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "risk-treatment-options" });
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
            var result = JsonSerializer.Deserialize<RiskTreatmentOptionApiResponse>(apiResponse, options);

            RiskTreatmentOptionApiResponse = result ?? new RiskTreatmentOptionApiResponse();
            if (RiskTreatmentOptionApiResponse.ResponseCode == "200" && RiskTreatmentOptionApiResponse.data != null)
            {
                foreach (var item in RiskTreatmentOptionApiResponse.data)
                {
                    try
                    {
                        var existing = await _repository.GetByIdAsync(item.id);

                        if (existing == null)
                        {
                            // Create new record
                            var newRisk = new MRiskTreatmentOption
                            {
                                Id = item.id,
                                Name = item.name,
                                UpdateDate = item.updateDate
                            };

                            await _repository.AddAsync(newRisk);

                        }
                        else if (item.updateDate.Value.Date != existing.UpdateDate.Value.Date)
                        {
                            // Update existing record
                            existing.Id = item.id;
                            existing.Name = item.name;

                            existing.UpdateDate = item.updateDate;

                            await _repository.UpdateAsync(existing);

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] Failed to process RiskFactor ID {item.id}: {ex.Message}");
                    }
                }
            }



        }

        public async Task<RiskTreatmentOptionApiResponse> GetAllAsyncSearch_RiskTreatmentOption(SearchRiskTreatmentOptionModels models)
        {
            try
            {
                // ดึงข้อมูลจาก repository
                var riskFactors = await _repository.GetAllAsyncSearch_RiskTreatmentOption(models);
                if (riskFactors == null || !riskFactors.Any())
                {
                    await BatchEndOfDay_MRiskTreatmentOption(models);
                    riskFactors = await _repository.GetAllAsyncSearch_RiskTreatmentOption(models);
                }

                // Mapping ข้อมูล
                var response = new RiskTreatmentOptionApiResponse
                {
                    ResponseCode = "200",
                    ResponseMsg = "OK",
                    data = riskFactors.Select(r => new RiskTreatmentOptionModels
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
                Console.WriteLine($"[ERROR] Failed to search RiskTreatmentOptionApiResponse: {ex.Message}");
                return new RiskTreatmentOptionApiResponse
                {
                    ResponseCode = "500",
                    ResponseMsg = "Internal Server Error",
                    data = new List<RiskTreatmentOptionModels>(),
                    Timestamp = DateTime.UtcNow
                };
            }
        }

    }
}
