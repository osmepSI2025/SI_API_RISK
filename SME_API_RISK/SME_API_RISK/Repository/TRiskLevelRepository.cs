using SME_API_RISK.Entities;
using Microsoft.EntityFrameworkCore;
using SME_API_RISK.Models;

namespace SME_API_RISK.Repository
{
    public class TRiskLevelRepository
    {
        private readonly RISKDBContext _context;

        public TRiskLevelRepository(RISKDBContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TRiskLevel entity)
        {
            try
            {
                await _context.TRiskLevels.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Repository failed to add TRiskLevel: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<TRiskLevel>> GetAllAsync()
        {
            try
            {
                return await _context.TRiskLevels.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Repository failed to get all TRiskLevel: {ex.Message}");
                throw;
            }
        }

        public async Task<TRiskLevel?> GetByIdAsync(int riskDefineId, string riskDefine, string riskLevelTitle)
        {
            try
            {
              
                return await _context.TRiskLevels
                   .FirstOrDefaultAsync(e => e.RiskDefineId == riskDefineId
                       && e.RiskLevelTitle == riskLevelTitle
                       && e.RiskDefine == riskDefine
                    );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Repository failed to get TRiskLevel by id: {ex.Message}");
                throw;
            }
           

        }

        public async Task UpdateAsync(TRiskLevel entity)
        {
            try
            {
                _context.TRiskLevels.Update(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Repository failed to update TRiskLevel: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var entity = await _context.TRiskLevels.FindAsync(id);
                if (entity != null)
                {
                    _context.TRiskLevels.Remove(entity);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Repository failed to delete TRiskLevel: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<TRiskLevel>> GetAllAsyncSearch_TRiskLevel(SearchTRiskLevelApiModels searchModel)
        {
            try
            {
                var query = _context.TRiskLevels.AsQueryable();



                if (searchModel.riskFactorID != 0 && searchModel.riskFactorID != null)
                {
                    query = query.Where(bu =>
                        bu.RiskDefineId == searchModel.riskFactorID
                    );
                }
                if (searchModel.riskYear != 0 && searchModel.riskYear != null)
                {
                    query = query.Where(bu =>
                        bu.YearBudget == searchModel.riskYear
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
