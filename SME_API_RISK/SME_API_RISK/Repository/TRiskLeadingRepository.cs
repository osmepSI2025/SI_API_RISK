using SME_API_RISK.Entities;
using Microsoft.EntityFrameworkCore;
using SME_API_RISK.Models;

namespace SME_API_RISK.Repository
{
    public class TRiskLeadingRepository
    {
        private readonly RISKDBContext _context;

        public TRiskLeadingRepository(RISKDBContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TRiskLeading entity)
        {
            try
            {
                await _context.TRiskLeadings.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to add TRiskLeading: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<TRiskLeading>> GetAllAsync()
        {
            try
            {
                return await _context.TRiskLeadings.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get all TRiskLeading: {ex.Message}");
                throw;
            }
        }

        public async Task<TRiskLeading?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.TRiskLeadings.FirstOrDefaultAsync(e => e.Id == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get TRiskLeading by id: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateAsync(TRiskLeading entity)
        {
            try
            {
                _context.TRiskLeadings.Update(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to update TRiskLeading: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var entity = await _context.TRiskLeadings.FirstOrDefaultAsync(e => e.Id == id);
                if (entity != null)
                {
                    _context.TRiskLeadings.Remove(entity);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to delete TRiskLeading: {ex.Message}");
                throw;
            }
        }
        public async Task<IEnumerable<TRiskLeading>> GetAllAsyncSearch_RiskLeading(SearchRiskLeadingModels searchModel)
        {
            try
            {
                var query = _context.TRiskLeadings.AsQueryable();



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
