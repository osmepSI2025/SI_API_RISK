using SME_API_RISK.Entities;
using Microsoft.EntityFrameworkCore;

namespace SME_API_RISK.Repository
{
    public class TRiskExistingControlRepository
    {
        private readonly RISKDBContext _context;

        public TRiskExistingControlRepository(RISKDBContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TRiskExistingControl entity)
        {
            try
            {
                await _context.TRiskExistingControls.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to add TRiskExistingControl: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<TRiskExistingControl>> GetAllAsync()
        {
            try
            {
                return await _context.TRiskExistingControls.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get all TRiskExistingControl: {ex.Message}");
                throw;
            }
        }

        public async Task<TRiskExistingControl?> GetByIdAsync(int riskDefineId, string rootCauseType, string rootCauseName)
        {
            try
            {
                return await _context.TRiskExistingControls
                    .FirstOrDefaultAsync(e => e.RiskDefineId == riskDefineId
                        && e.RootCauseType == rootCauseType
                        && e.RootCauseName == rootCauseName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get TRiskExistingControl by id: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateAsync(TRiskExistingControl entity)
        {
            try
            {
                _context.TRiskExistingControls.Update(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to update TRiskExistingControl: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var entity = await _context.TRiskExistingControls.FirstOrDefaultAsync(e => e.Id == id);
                if (entity != null)
                {
                    _context.TRiskExistingControls.Remove(entity);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to delete TRiskExistingControl: {ex.Message}");
                throw;
            }
        }
    }
}
