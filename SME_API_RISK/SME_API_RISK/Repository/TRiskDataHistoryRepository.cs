using SME_API_RISK.Entities;
using Microsoft.EntityFrameworkCore;
using SME_API_RISK.Models;

namespace SME_API_RISK.Repository
{
    public class TRiskDataHistoryRepository
    {
        private readonly RISKDBContext _context;

        public TRiskDataHistoryRepository(RISKDBContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TRiskDataHistory dataHistory)
        {
            try
            {
                await _context.TRiskDataHistories.AddAsync(dataHistory);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to add TRiskDataHistory: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<TRiskDataHistory>> GetAllAsync()
        {
            try
            {
                return await _context.TRiskDataHistories.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get all TRiskDataHistory: {ex.Message}");
                throw;
            }
        }

        public async Task<TRiskDataHistory?> GetByIdAsync(int riskDefineId)
        {
            try
            {
                return await _context.TRiskDataHistories.FindAsync(riskDefineId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get TRiskDataHistory by id: {ex.Message}");
                return  null;
            }
        }

        public async Task UpdateAsync(TRiskDataHistory dataHistory)
        {
            try
            {
                _context.TRiskDataHistories.Update(dataHistory);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to update TRiskDataHistory: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(int riskDefineId)
        {
            try
            {
                var entity = await _context.TRiskDataHistories.FindAsync(riskDefineId);
                if (entity != null)
                {
                    _context.TRiskDataHistories.Remove(entity);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to delete TRiskDataHistory: {ex.Message}");
                throw;
            }
        }
        public async Task<IEnumerable<TRiskDataHistory>> GetAllAsyncSearch_RiskTRiskDataHistory(SearchRiskTDataHistoryModels searchModel)
        {
            try
            {
                var query = _context.TRiskDataHistories.AsQueryable();



                if (searchModel.riskFactorID != 0 && searchModel.riskFactorID != null)
                {
                    query = query.Where(bu =>
                        bu.RiskDefineId == searchModel.riskFactorID
                    );
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
