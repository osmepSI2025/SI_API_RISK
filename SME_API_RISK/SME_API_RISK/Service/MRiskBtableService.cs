using SME_API_RISK.Entities;
using SME_API_RISK.Models;
using SME_API_RISK.Repository;
using SME_API_RISK.Services;
using System.Text.Json;

namespace SME_API_RISK.Service
{
    public class MRiskBtableService
    {
        private readonly MRiskBtableRepository _repository;
        private readonly ICallAPIService _serviceApi;
        private readonly IApiInformationRepository _repositoryApi;
        private readonly string _FlagDev;

        public MRiskBtableService(MRiskBtableRepository repository, IConfiguration configuration, ICallAPIService serviceApi, IApiInformationRepository repositoryApi)

        {
            _repository = repository;
            _serviceApi = serviceApi;
            _repositoryApi = repositoryApi;
            _FlagDev = configuration["Devlopment:FlagDev"] ?? throw new ArgumentNullException("FlagDev is missing in appsettings.json");


        }

        public async Task AddAsync(MRiskBtable riskBtable)
        {
            try
            {
                await _repository.AddAsync(riskBtable);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to add MRiskBtable: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<MRiskBtable>> GetAllAsync()
        {
            try
            {
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to get all MRiskBtable: {ex.Message}");
                throw;
            }
        }

        public async Task<MRiskBtable?> GetByIdAsync(int levels, string performance)
        {
            try
            {
                return await _repository.GetByIdAsync(levels, performance);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to get MRiskBtable by id: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateAsync(MRiskBtable riskBtable)
        {
            try
            {
                await _repository.UpdateAsync(riskBtable);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to update MRiskBtable: {ex.Message}");
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
                Console.WriteLine($"[ERROR] Service failed to delete MRiskBtable: {ex.Message}");
                throw;
            }
        }

        public async Task BatchEndOfDay_MRiskBTable()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
            var RiskBTableApiResponse = new RiskBTableApiResponse();
            if (_FlagDev == "Y")
            {
                var filePath = Path.GetFullPath("MocData/risk-b-table.json", AppContext.BaseDirectory);

                try
                {
                    if (!File.Exists(filePath))
                        RiskBTableApiResponse = new RiskBTableApiResponse();

                    var jsonString = await File.ReadAllTextAsync(filePath);

                    var result = JsonSerializer.Deserialize<RiskBTableApiResponse>(jsonString, options);

                    RiskBTableApiResponse = result ?? new RiskBTableApiResponse();
                }
                catch (Exception ex)
                {
                    RiskBTableApiResponse = new RiskBTableApiResponse();
                }


            }
            else
            {
                var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "b-table" });
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
                SearchRiskBTableModels Msearch = new SearchRiskBTableModels
                {

                    page = 1,
                    pageSize = 1000,
                    keyword = ""

                };
                var apiResponse = await _serviceApi.GetDataApiAsync(apiParam, Msearch);
                var result = JsonSerializer.Deserialize<RiskBTableApiResponse>(apiResponse, options);

                RiskBTableApiResponse = result ?? new RiskBTableApiResponse();
            }

            if (RiskBTableApiResponse.ResponseCode == "200" && RiskBTableApiResponse.data != null)
            {
                foreach (var item in RiskBTableApiResponse.data)
                {
                    try
                    {
                        var existingOwner = await _repository.GetByIdAsync(item.Levels,item.Performance);

                        if (existingOwner == null)
                        {
                            // Create new record
                            var newRisk = new MRiskBtable
                            {
                              //  Id = item.Id,
                                Levels = item.Levels,
                                Performance = item.Performance,
                                OldWork = item.OldWork,
                                Process = item.Process,
                                Report = item.Report,
                      
                                UpdateDate = item.UpdateDate
                            };

                            await _repository.AddAsync(newRisk);

                        }
                        else if (item.UpdateDate.Value.Date != existingOwner.UpdateDate.Value.Date)
                        {
                            // Update existing record
                          //  existingOwner.Id = item.Id;
                            existingOwner.Levels = item.Levels;
                            existingOwner.Performance = item.Performance;
                            existingOwner.OldWork = item.OldWork;
                            existingOwner.Process = item.Process;
                            existingOwner.Report = item.Report;

                            existingOwner.UpdateDate = item.UpdateDate;

                            await _repository.UpdateAsync(existingOwner);

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] Failed to process RiskFactor ID {item.Levels}: {ex.Message}");
                    }
                }
            }



        }

        public async Task<RiskBTableApiResponse> GetAllAsyncSearch_RiskBTable()
        {
            try
            {
                // ดึงข้อมูลจาก repository
                var riskFactors = await _repository.GetAllAsyncSearch_RiskBTable();
                if (riskFactors == null || !riskFactors.Any())
                {
                    await BatchEndOfDay_MRiskBTable();
                    riskFactors = await _repository.GetAllAsyncSearch_RiskBTable();
                   
                }
                // Mapping ข้อมูล
                var response = new RiskBTableApiResponse
                {
                    ResponseCode = "200",
                    ResponseMsg = "OK",
                    data = riskFactors.Select(r => new RiskBTableModels
                    {
                        Levels = r.Levels,
                        Performance = r.Performance,
                        OldWork = r.OldWork,
                        Process = r.Process,
                        Report = r.Report,
                        UpdateDate = r.UpdateDate
                    }).ToList(),
                    Timestamp = DateTime.UtcNow
                };

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to search RiskFactors: {ex.Message}");
                return new RiskBTableApiResponse
                {
                    ResponseCode = "500",
                    ResponseMsg = "Internal Server Error",
                    data = new List<RiskBTableModels>(),
                    Timestamp = DateTime.UtcNow
                };
            }
        }

    }
}
