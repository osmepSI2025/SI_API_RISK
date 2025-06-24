using SME_API_RISK.Entities;
using Microsoft.EntityFrameworkCore;
using SME_API_RISK.Models;

namespace SME_API_RISK.Repository
{
    public class TRiskTriggerRepository
    {
        private readonly RISKDBContext _context;

        public TRiskTriggerRepository(RISKDBContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TRiskTrigger riskTrigger)
        {
            try
            {
                await _context.TRiskTriggers.AddAsync(riskTrigger);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to add TRiskTrigger: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<TRiskTrigger>> GetAllAsync()
        {
            try
            {
                return await _context.TRiskTriggers.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get all TRiskTrigger: {ex.Message}");
                throw;
            }
        }

        public async Task<TRiskTrigger?> GetByIdAsync(int riskDefineId)
        {
            try
            {
                return await _context.TRiskTriggers.FindAsync(riskDefineId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get TRiskTrigger by id: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateAsync(TRiskTrigger riskTrigger)
        {
            try
            {
                _context.TRiskTriggers.Update(riskTrigger);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to update TRiskTrigger: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(int riskDefineId)
        {
            try
            {
                var entity = await _context.TRiskTriggers.FindAsync(riskDefineId);
                if (entity != null)
                {
                    _context.TRiskTriggers.Remove(entity);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to delete TRiskTrigger: {ex.Message}");
                throw;
            }
        }
        public async Task<IEnumerable<TRiskTrigger>> GetAllAsyncSearch_RiskTTrigger(SearchRiskTTriggersModels searchModel)
        {
            try
            {
                var query = _context.TRiskTriggers.AsQueryable();



                if (searchModel.riskFactorID != 0 && searchModel.riskFactorID != null)
                {
                    query = query.Where(bu =>
                        bu.RiskDefineId == searchModel.riskFactorID
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
