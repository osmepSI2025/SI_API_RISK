using Microsoft.AspNetCore.Mvc;
using SME_API_RISK.Entities;
using SME_API_RISK.Models;
using SME_API_RISK.Repository;
using SME_API_RISK.Services;
using System.Text.Json;

namespace SME_API_RISK.Service
{
    public class TInternalControlsActivityService
    {
        private readonly TInternalControlsActivityRepository _repository;
        private readonly ICallAPIService _serviceApi;
        private readonly IApiInformationRepository _repositoryApi;
        private readonly string _FlagDev;


        public TInternalControlsActivityService(TInternalControlsActivityRepository repository, IConfiguration configuration, ICallAPIService serviceApi, IApiInformationRepository repositoryApi)

        {
            _repository = repository;
            _serviceApi = serviceApi;
            _repositoryApi = repositoryApi;
            _FlagDev = configuration["Devlopment:FlagDev"] ?? throw new ArgumentNullException("FlagDev is missing in appsettings.json");


        }

        public async Task AddAsync(TInternalControlsActivity activity)
        {
            try
            {
                await _repository.AddAsync(activity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to add TInternalControlsActivity: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<TInternalControlsActivity>> GetAllAsync()
        {
            try
            {
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to get all TInternalControlsActivity: {ex.Message}");
                throw;
            }
        }

        //public async Task<TInternalControlsActivity?> GetByIdAsync(int id)
        //{
        //    try
        //    {
        //        return await _repository.GetByIdAsync(id);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"[ERROR] Service failed to get TInternalControlsActivity by id: {ex.Message}");
        //        throw;
        //    }
        //}

        public async Task UpdateAsync(TInternalControlsActivity activity)
        {
            try
            {
                await _repository.UpdateAsync(activity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Service failed to update TInternalControlsActivity: {ex.Message}");
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
                Console.WriteLine($"[ERROR] Service failed to delete TInternalControlsActivity: {ex.Message}");
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
            var TInternalControlsActivityApiResponse = new TInternalControlsActivityApiResponse();
       

            var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "activities" });
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
            SearchTInternalControlsActivityModels Msearch = new SearchTInternalControlsActivityModels
            {

                page = 1,
                pageSize = 1000,
                riskYear = year,

            };
            var apiResponse = await _serviceApi.GetDataApiAsync(apiParam, Msearch);
            var result = JsonSerializer.Deserialize<TInternalControlsActivityApiResponse>(apiResponse, options);

            TInternalControlsActivityApiResponse = result ?? new TInternalControlsActivityApiResponse();

            if (TInternalControlsActivityApiResponse.ResponseCode == "200" && TInternalControlsActivityApiResponse.data != null)
            {
                foreach (var item in TInternalControlsActivityApiResponse.data)
                {
                    try
                    {
                        var existing = await _repository.GetByIdAsync(item.Departments, item.Activities);

                        if (existing == null)
                        {
                            // Create new record
                            var newRisk = new TInternalControlsActivity
                            {

                                Activities = item.Activities,
                                Departments = item.Departments,
                                Process = item.Process,
                                RiskYear = year,
                                UpdateDate = item.UpdateDate,
                            };

                            await _repository.AddAsync(newRisk);

                        }
                        else if (item.UpdateDate.Value.Date != existing.UpdateDate.Value.Date)
                        {
                            // Update existing record
                            existing.Activities = item.Activities;
                            existing.Departments = item.Departments;
                            existing.Process = item.Process;
                            existing.UpdateDate = item.UpdateDate;

                            await _repository.UpdateAsync(existing);

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] Failed to process BatchEndOfDay_InternalActivity  {item.Activities}: {ex.Message}");
                    }
                }
            }



        }

        public async Task<TInternalControlsActivityApiResponse> GetAllAsyncSearch_InternalActivity(SearchTInternalControlsActivityModels models)
        {
            try
            {
                // ดึงข้อมูลจาก repository
                var RiskTKpis = await _repository.GetAllAsyncSearch_InternalActivity(models);

                if (RiskTKpis == null || RiskTKpis.ToList().Count==0)
                {
                   await BatchEndOfDay_InternalActivity(models.riskYear);
                     RiskTKpis = await _repository.GetAllAsyncSearch_InternalActivity(models);
                    // Mapping ข้อมูล
                    var response = new TInternalControlsActivityApiResponse
                    {
                        ResponseCode = "200",
                        ResponseMsg = "OK",
                        data = RiskTKpis.Select(r => new TInternalControlsActivityModels
                        {
                            Activities = r.Activities,
                            Departments = r.Departments,
                            Process = r.Process,
                            UpdateDate = r.UpdateDate
                        }).ToList(),
                        Timestamp = DateTime.UtcNow
                    };
                    return response;
                }
                else 
                {
                    // Mapping ข้อมูล
                    var response = new TInternalControlsActivityApiResponse
                    {
                        ResponseCode = "200",
                        ResponseMsg = "OK",
                        data = RiskTKpis.Select(r => new TInternalControlsActivityModels
                        {
                            Activities = r.Activities,
                            Departments = r.Departments,
                            Process = r.Process,
                            UpdateDate = r.UpdateDate
                        }).ToList(),
                        Timestamp = DateTime.UtcNow
                    };
                    return response;
                }
                   

               
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to search GetAllAsyncSearch_TInternalControlsReport: {ex.Message}");
                return new TInternalControlsActivityApiResponse
                {
                    ResponseCode = "500",
                    ResponseMsg = "Internal Server Error",
                    data = new List<TInternalControlsActivityModels>(),
                    Timestamp = DateTime.UtcNow
                };
            }
        }
        public async Task<int> BatchEndOfDay_activities_schedule()
        {
            int currentYear = DateTime.Now.Year;

            foreach (var year in Enumerable.Range(0, 3))
            {
                int christianYear = currentYear - year;
                int buddhistYear = christianYear + 543;

                await BatchEndOfDay_InternalActivity(buddhistYear);
            }


            return 1;
        }

 
    }
}
