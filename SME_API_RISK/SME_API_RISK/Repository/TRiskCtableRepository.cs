using SME_API_RISK.Entities;
using Microsoft.EntityFrameworkCore;

namespace SME_API_RISK.Repository
{
    public class TRiskCtableRepository
    {
        private readonly RISKDBContext _context;

        public TRiskCtableRepository(RISKDBContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TRiskCtable entity)
        {
            try
            {
                await _context.TRiskCtables.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to add TRiskCtable: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<TRiskCtable>> GetAllAsync()
        {
            try
            {
                return await _context.TRiskCtables.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get all TRiskCtable: {ex.Message}");
                throw;
            }
        }

        public async Task<TRiskCtable?> GetByIdAsync(int Factorid,string ctype=null,string cname=null)
        {
            try
            {
                return await _context.TRiskCtables.FirstOrDefaultAsync(e => e.RiskDefineId == Factorid && e.RootCauseType == ctype && e.RootCauseName == cname);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get TRiskCtable by id: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateAsync(TRiskCtable entity)
        {
            try
            {
                _context.TRiskCtables.Update(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to update TRiskCtable: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var entity = await _context.TRiskCtables.FirstOrDefaultAsync(e => e.Id == id);
                if (entity != null)
                {
                    _context.TRiskCtables.Remove(entity);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to delete TRiskCtable: {ex.Message}");
                throw;
            }
        }
        public async Task<IEnumerable<TRiskCtable>> GetAllAsyncSearch_Ctable(SearchRiskCTableApiModels searchModel)
        {
            try
            {
                var query = _context.TRiskCtables.AsQueryable(); // Corrected entity set

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
                Console.WriteLine($"[ERROR] Failed to search TRiskEmergencyPlan: {ex.Message}");
                return Enumerable.Empty<TRiskCtable>();
            }
        }

    }
}
