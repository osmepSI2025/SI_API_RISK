using SME_API_RISK.Entities;
using Microsoft.EntityFrameworkCore;

namespace SME_API_RISK.Repository
{
    public class TRiskLaggingRepository
    {
        private readonly RISKDBContext _context;

        public TRiskLaggingRepository(RISKDBContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TRiskLagging entity)
        {
            try
            {
                await _context.TRiskLaggings.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to add TRiskLagging: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<TRiskLagging>> GetAllAsync()
        {
            try
            {
                return await _context.TRiskLaggings.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get all TRiskLagging: {ex.Message}");
                throw;
            }
        }

        public async Task<TRiskLagging?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.TRiskLaggings.FirstOrDefaultAsync(e => e.Id == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get TRiskLagging by id: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateAsync(TRiskLagging entity)
        {
            try
            {
                _context.TRiskLaggings.Update(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to update TRiskLagging: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var entity = await _context.TRiskLaggings.FirstOrDefaultAsync(e => e.Id == id);
                if (entity != null)
                {
                    _context.TRiskLaggings.Remove(entity);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to delete TRiskLagging: {ex.Message}");
                throw;
            }
        }
        public async Task<IEnumerable<TRiskLagging>> GetAllAsyncSearch_RiskLagging(SearchRiskLaggingModels searchModel)
        {
            try
            {
                var query = _context.TRiskLaggings.AsQueryable();



                if (searchModel.riskFactorID != 0 && searchModel.riskFactorID != null)
                {
                    query = query.Where(bu =>
                        bu.RiskDefineId == searchModel.riskFactorID
                    );
                }

                //// Apply pagination
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
