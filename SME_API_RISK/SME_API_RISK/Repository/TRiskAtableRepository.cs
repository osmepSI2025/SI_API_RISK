using SME_API_RISK.Entities;
using Microsoft.EntityFrameworkCore;
using SME_API_RISK.Models;

namespace SME_API_RISK.Repository
{
    public class TRiskAtableRepository
    {
        private readonly RISKDBContext _context;

        public TRiskAtableRepository(RISKDBContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TRiskAtable riskAtable)
        {
            try
            {
                await _context.TRiskAtables.AddAsync(riskAtable);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to add TRiskAtable: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<TRiskAtable>> GetAllAsync()
        {
            try
            {
                return await _context.TRiskAtables.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get all TRiskAtable: {ex.Message}");
                throw;
            }
        }

        public async Task<TRiskAtable?> GetByIdAsync(int riskDefineId)
        {
            try
            {
                return await _context.TRiskAtables.FindAsync(riskDefineId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get TRiskAtable by id: {ex.Message}");
                return null;
            }
        }

        public async Task UpdateAsync(TRiskAtable riskAtable)
        {
            try
            {
                _context.TRiskAtables.Update(riskAtable);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to update TRiskAtable: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(int riskDefineId)
        {
            try
            {
                var entity = await _context.TRiskAtables.FindAsync(riskDefineId);
                if (entity != null)
                {
                    _context.TRiskAtables.Remove(entity);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to delete TRiskAtable: {ex.Message}");
                throw;
            }
        }
        public async Task<IEnumerable<TRiskAtable>> GetAllAsyncSearch_RiskTAtable(SearchRiskTATableModels searchModel)
        {
            try
            {
                var query = _context.TRiskAtables.AsQueryable();



                if (searchModel.RiskDefineId != 0 && searchModel.RiskDefineId != null)
                {
                    query = query.Where(bu =>
                        bu.RiskDefineId == searchModel.RiskDefineId
                    );
                }

                // Apply pagination
                //if (searchModel.page != 0 && searchModel.pageSize != 0)
                //{
                //    int skip = (searchModel.page - 1) * searchModel.pageSize;
                //    query = query.Skip(skip).Take(searchModel.pageSize);
                //}


                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
