using SME_API_RISK.Entities;
using Microsoft.EntityFrameworkCore;
using SME_API_RISK.Models;

namespace SME_API_RISK.Repository
{
    public class TRiskimpactRepository
    {
        private readonly RISKDBContext _context;

        public TRiskimpactRepository(RISKDBContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TRiskimpact riskImpact)
        {
            try
            {
                await _context.TRiskimpacts.AddAsync(riskImpact);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to add TRiskimpact: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<TRiskimpact>> GetAllAsync()
        {
            try
            {
                return await _context.TRiskimpacts.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get all TRiskimpact: {ex.Message}");
                throw;
            }
        }

        public async Task<TRiskimpact?> GetByIdAsync(int riskDefineId)
        {
            try
            {
                return await _context.TRiskimpacts.FindAsync(riskDefineId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get TRiskimpact by id: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateAsync(TRiskimpact riskImpact)
        {
            try
            {
                _context.TRiskimpacts.Update(riskImpact);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to update TRiskimpact: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(int riskDefineId)
        {
            try
            {
                var entity = await _context.TRiskimpacts.FindAsync(riskDefineId);
                if (entity != null)
                {
                    _context.TRiskimpacts.Remove(entity);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to delete TRiskimpact: {ex.Message}");
                throw;
            }
        }
        public async Task<IEnumerable<TRiskimpact>> GetAllAsyncSearch_RiskTImpact(SearchRiskTImpactModels searchModel)
        {
            try
            {
                var query = _context.TRiskimpacts.AsQueryable();



                if (searchModel.riskFactorID != 0 && searchModel.riskFactorID != null)
                {
                    query = query.Where(bu =>
                        bu.RiskDefineId == searchModel.riskFactorID
                    );
                }
                if (searchModel.keyword != "" && searchModel.keyword != null)
                {
                    query = query.Where(bu =>
                        bu.Impacts.Contains(searchModel.keyword)
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
