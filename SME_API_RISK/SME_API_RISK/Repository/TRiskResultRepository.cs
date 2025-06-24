using SME_API_RISK.Entities;
using Microsoft.EntityFrameworkCore;

namespace SME_API_RISK.Repository
{
    public class TRiskResultRepository
    {
        private readonly RISKDBContext _context;

        public TRiskResultRepository(RISKDBContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TRiskResult entity)
        {
            try
            {
                await _context.TRiskResults.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to add TRiskResult: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<TRiskResult>> GetAllAsync()
        {
            try
            {
                return await _context.TRiskResults.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get all TRiskResult: {ex.Message}");
                throw;
            }
        }

        public async Task<TRiskResult?> GetByIdAsync(int riskDefineId)
        {
            try
            {
                return await _context.TRiskResults.FirstOrDefaultAsync(e => e.RiskDefineId == riskDefineId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get TRiskResult by id: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateAsync(TRiskResult entity)
        {
            try
            {
                _context.TRiskResults.Update(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to update TRiskResult: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(int riskDefineId)
        {
            try
            {
                var entity = await _context.TRiskResults.FirstOrDefaultAsync(e => e.RiskDefineId == riskDefineId);
                if (entity != null)
                {
                    _context.TRiskResults.Remove(entity);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to delete TRiskResult: {ex.Message}");
                throw;
            }
        }
    }
}
