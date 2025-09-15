using SME_API_RISK.Entities;
using Microsoft.EntityFrameworkCore;
using SME_API_RISK.Models;

namespace SME_API_RISK.Repository
{
    public class TRiskPerformanceRepository
    {
        private readonly RISKDBContext _context;

        public TRiskPerformanceRepository(RISKDBContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TRiskPerformance performance)
        {
            try
            {
                await _context.TRiskPerformances.AddAsync(performance);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to add TRiskPerformance: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<TRiskPerformance>> GetAllAsync()
        {
            try
            {
                return await _context.TRiskPerformances.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get all TRiskPerformance: {ex.Message}");
                throw;
            }
        }

        public async Task<TRiskPerformance?> GetByIdAsync(int riskDefineId)
        {
            try
            {
                return await _context.TRiskPerformances.FindAsync(riskDefineId);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task UpdateAsync(TRiskPerformance performance)
        {
            try
            {
                _context.TRiskPerformances.Update(performance);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to update TRiskPerformance: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(int riskDefineId)
        {
            try
            {
                var entity = await _context.TRiskPerformances.FindAsync(riskDefineId);
                if (entity != null)
                {
                    _context.TRiskPerformances.Remove(entity);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to delete TRiskPerformance: {ex.Message}");
                throw;
            }
        }
        public async Task<IEnumerable<TRiskPerformance>> GetAllAsyncSearch_RiskTRiskPerformance(SearchTRiskPerformanceModels searchModel)
        {
            try
            {
                var query = _context.TRiskPerformances.AsQueryable();



                if (searchModel.riskFactorID != 0 && searchModel.riskFactorID != null)
                {
                    query = query.Where(bu =>
                        bu.RiskDefineId == searchModel.riskFactorID
                    );
                }
                if (searchModel.keyword != "" && searchModel.keyword != null)
                {
                    query = query.Where(bu =>
                        bu.Performances.Contains(searchModel.keyword));
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
