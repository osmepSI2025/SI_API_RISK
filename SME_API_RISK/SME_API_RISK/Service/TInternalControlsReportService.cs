using SME_API_RISK.Entities;
using SME_API_RISK.Models;
using SME_API_RISK.Repository;
using SME_API_RISK.Services;
using System.Text.Json;
using YamlDotNet.Serialization.NodeTypeResolvers;

namespace SME_API_RISK.Service
{
    public class TInternalControlsReportService
    {
        private readonly TInternalControlsReportRepository _repository;
        private readonly ICallAPIService _serviceApi;
        private readonly IApiInformationRepository _repositoryApi;
        private readonly string _FlagDev;

        public TInternalControlsReportService(TInternalControlsReportRepository repository, IConfiguration configuration, ICallAPIService serviceApi, IApiInformationRepository repositoryApi)

        {
            _repository = repository;
            _serviceApi = serviceApi;
            _repositoryApi = repositoryApi;
            _FlagDev = configuration["Devlopment:FlagDev"] ?? throw new ArgumentNullException("FlagDev is missing in appsettings.json");

        }

        public async Task AddAsync(TInternalControlsReport report)
        {
            try
            {
                await _repository.AddAsync(report);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to add TInternalControlsReport: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<TInternalControlsReport>> GetAllAsync()
        {
            try
            {
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to get all TInternalControlsReport: {ex.Message}");
                throw;
            }
        }

        //public async Task<TInternalControlsReport?> GetByIdAsync(int id)
        //{
        //    try
        //    {
        //        return await _repository.GetByIdAsync(id);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"[ERROR] Service failed to get TInternalControlsReport by id: {ex.Message}");
        //        throw;
        //    }
        //}

        public async Task UpdateAsync(TInternalControlsReport report)
        {
            try
            {
                await _repository.UpdateAsync(report);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to update TInternalControlsReport: {ex.Message}");
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
                Console.WriteLine($"[ERROR] Service failed to delete TInternalControlsReport: {ex.Message}");
                throw;
            }
        }
        public async Task BatchEndOfDay_InternalActivity(int year)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
            var TInternalControlsReportApiResponse = new TInternalControlsReportApiResponse();
            if (_FlagDev == "Y")
            {
                var filePath = Path.GetFullPath("MocData/Internalreport.json", AppContext.BaseDirectory);

                try
                {
                    if (!File.Exists(filePath))
                        TInternalControlsReportApiResponse = new TInternalControlsReportApiResponse();

                    var jsonString = await File.ReadAllTextAsync(filePath);

                    var result = JsonSerializer.Deserialize<TInternalControlsReportApiResponse>(jsonString, options);

                    TInternalControlsReportApiResponse = result ?? new TInternalControlsReportApiResponse();
                }
                catch (Exception ex)
                {
                    TInternalControlsReportApiResponse = new TInternalControlsReportApiResponse();
                }


            }
            else
            {
                var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "reports" });
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
                SearchTInternalControlsReportModels Msearch = new SearchTInternalControlsReportModels
                {

                    page = 1,
                    pageSize = 1000,
                    riskYear = year,

                };
                var apiResponse = await _serviceApi.GetDataApiAsync(apiParam, Msearch);
                var result = JsonSerializer.Deserialize<TInternalControlsReportApiResponse>(apiResponse, options);

                TInternalControlsReportApiResponse = result ?? new TInternalControlsReportApiResponse();
            }

            if (TInternalControlsReportApiResponse.ResponseCode == "200" && TInternalControlsReportApiResponse.data != null)
            {
                foreach (var item in TInternalControlsReportApiResponse.data)
                {
                    try
                    {
                        var existing = await _repository.GetByIdAsync(item.Departments, item.AssessControlResult,year);

                        if (existing == null)
                        {
                            // Create new record
                            var newRisk = new TInternalControlsReport
                            {

                             
                                Departments = item.Departments,
                                 AssessControlResult = item.AssessControlResult,
                                ClosedComment = item.ClosedComment,
                                QuaterFinished = item.QuaterFinished,
                                Q1 = item.Q1,
                                Q2 = item.Q2,
                                Q3 = item.Q3,
                                Q4 = item.Q4,
                                Result = item.Result,
                                RiskYear = year,
                                UpdateDate = item.UpdateDate,
                            };

                            await _repository.AddAsync(newRisk);

                        }
                        else if (item.UpdateDate.Value.Date != existing.UpdateDate.Value.Date)
                        {
                            // Update existing record
                       
                            existing.Departments = item.Departments;
                            existing.AssessControlResult = item.AssessControlResult;

                            existing.ClosedComment = item.ClosedComment;
                            existing.QuaterFinished = item.QuaterFinished;
                            existing.Q1 = item.Q1;
                             existing.Q2 = item.Q2;
                            existing.Q3 = item.Q3;
                            existing.Q4 = item.Q4;
                            existing.Result = item.Result;
                            existing.RiskYear = year;
                            existing.UpdateDate = item.UpdateDate;

                            await _repository.UpdateAsync(existing);

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] Failed to process BatchEndOfDay_InternalActivity  {item.Departments}: {ex.Message}");
                    }
                }
            }



        }
        public async Task<TInternalControlsReportApiResponse> GetAllAsyncSearch_TInternalControlsReport(SearchTInternalControlsReportModels models)
        {
            try
            {
                // ดึงข้อมูลจาก repository
                var reports = await _repository.GetAllAsyncSearch_TInternalControlsReport(models);
                if (reports!= null) 
                {
                await BatchEndOfDay_InternalActivity(models.riskYear);
                    reports = await _repository.GetAllAsyncSearch_TInternalControlsReport(models);
                }
                var response = new TInternalControlsReportApiResponse
                {
                    ResponseCode = "200",
                    ResponseMsg = "OK",
                    data = reports.Select(r => new TInternalControlsReportModels
                    {
                        Departments = r.Departments,
                        AssessControlResult = r.AssessControlResult,
                        QuaterFinished = r.QuaterFinished,
                        Q1 = r.Q1,
                        Q2 = r.Q2,
                        Q3 = r.Q3,
                        Q4 = r.Q4,
                        Result = r.Result,
                        ClosedComment = r.ClosedComment,
                        UpdateDate = r.UpdateDate
                    }).ToList(),
                    Timestamp = DateTime.UtcNow
                };

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to search GetAllAsyncSearch_TInternalControlsReport: {ex.Message}");
                return new TInternalControlsReportApiResponse
                {
                    ResponseCode = "500",
                    ResponseMsg = "Internal Server Error",
                    data = new List<TInternalControlsReportModels>(),
                    Timestamp = DateTime.UtcNow
                };
            }
        }


        //public async Task<IEnumerable<TInternalControlsReportApiResponse>> GetAllAsyncSearch_TInternalControlsReport(SearchTInternalControlsReportModels models)
        //{
        //    try
        //    {
        //        // ดึงข้อมูลจาก repository
        //        var RiskTKpis = await _repository.GetAllAsyncSearch_TInternalControlsReport(models);

        //        // Mapping ข้อมูล
        //        var responseList = RiskTKpis.Select(r => new TInternalControlsReportApiResponse
        //        {
        //            ResponseCode = "200",
        //            ResponseMsg = "OK",
        //            data = new List<TInternalControlsReportModels>
        //    {
        //        new TInternalControlsReportModels
        //        {

        //            Departments = r.Departments,

        //            AssessControlResult = r.AssessControlResult,
        //            QuaterFinished = r.QuaterFinished,
        //            Q1 = r.Q1,
        //            Q2 = r.Q2,
        //             Q3 = r.Q3,
        //             Q4 = r.Q4,
        //             Result = r.Result,
        //             ClosedComment = r.ClosedComment,
        //            UpdateDate = r.UpdateDate
        //        }
        //    },
        //            Timestamp = DateTime.UtcNow
        //        }).ToList();

        //        return responseList;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"[ERROR] Failed to search GetAllAsyncSearch_TInternalControlsReport: {ex.Message}");
        //        return Enumerable.Empty<TInternalControlsReportApiResponse>();
        //    }
        //}

    }
}
