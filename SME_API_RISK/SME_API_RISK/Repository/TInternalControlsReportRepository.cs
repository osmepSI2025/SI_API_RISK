using SME_API_RISK.Entities;
using Microsoft.EntityFrameworkCore;
using SME_API_RISK.Models;

namespace SME_API_RISK.Repository
{
    public class TInternalControlsReportRepository
    {
        private readonly RISKDBContext _context;

        public TInternalControlsReportRepository(RISKDBContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TInternalControlsReport report)
        {
            try
            {
                await _context.TInternalControlsReports.AddAsync(report);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to add TInternalControlsReport: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<TInternalControlsReport>> GetAllAsync()
        {
            try
            {
                return await _context.TInternalControlsReports.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get all TInternalControlsReport: {ex.Message}");
                throw;
            }
        }

        // If your entity does not have a primary key, you may need to adjust this logic.
        public async Task<TInternalControlsReport?> GetByIdAsync(string depart, string ass, int year)
        {
            try
            {
               // return await _context.TInternalControlsReports.FindAsync(id);
                return await _context.TInternalControlsReports.FirstOrDefaultAsync(e => e.Departments == depart && e.AssessControlResult == ass && e.RiskYear == year);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get TInternalControlsReport by id: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateAsync(TInternalControlsReport report)
        {
            try
            {
                _context.TInternalControlsReports.Update(report);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to update TInternalControlsReport: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var entity = await _context.TInternalControlsReports.FindAsync(id);
                if (entity != null)
                {
                    _context.TInternalControlsReports.Remove(entity);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to delete TInternalControlsReport: {ex.Message}");
                throw;
            }
        }
        public async Task<IEnumerable<TInternalControlsReport>> GetAllAsyncSearch_TInternalControlsReport(SearchTInternalControlsReportModels searchModel)
        {
            try
            {
                var query = _context.TInternalControlsReports.AsQueryable();



                if (searchModel.riskYear != 0 && searchModel.riskYear != null)
                {
                    query = query.Where(bu =>
                        bu.RiskYear == searchModel.riskYear
                    );
                }

                // Apply pagination
                if (searchModel.page != 0 && searchModel.pageSize != 0)
                {
                    int skip = (searchModel.page - 1) * searchModel.pageSize;
                    query = query.Skip(skip).Take(searchModel.pageSize);
                }


                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
         
    }
}
