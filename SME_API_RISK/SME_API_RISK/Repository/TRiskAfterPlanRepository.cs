using SME_API_RISK.Entities;
using Microsoft.EntityFrameworkCore;
using SME_API_RISK.Models;

namespace SME_API_RISK.Repository
{
    public class TRiskAfterPlanRepository
    {
        private readonly RISKDBContext _context;

        public TRiskAfterPlanRepository(RISKDBContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TRiskAfterPlan entity)
        {
            try
            {
                await _context.TRiskAfterPlans.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Repository failed to add TRiskAfterPlan: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<TRiskAfterPlan>> GetAllAsync()
        {
            try
            {
                return await _context.TRiskAfterPlans.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Repository failed to get all TRiskAfterPlan: {ex.Message}");
                throw;
            }
        }

        public async Task<TRiskAfterPlan?> GetByIdAsync(int riskDefineId, string xdefine,int? quater=null)
        {
         

            try
            {

                return await _context.TRiskAfterPlans
                   .FirstOrDefaultAsync(e => e.RiskDefineId == riskDefineId
                       && e.RiskDefine == xdefine
                       && e.QuaterNo == quater
                    );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Repository failed to get TRiskAfterPlan by id: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateAsync(TRiskAfterPlan entity)
        {
            try
            {
                _context.TRiskAfterPlans.Update(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Repository failed to update TRiskAfterPlan: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var entity = await _context.TRiskAfterPlans.FindAsync(id);
                if (entity != null)
                {
                    _context.TRiskAfterPlans.Remove(entity);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Repository failed to delete TRiskAfterPlan: {ex.Message}");
                throw;
            }
        }
        public async Task<IEnumerable<TRiskAfterPlan>> GetAllAsyncSearch_TRiskAfterPlan(SearchRiskAfterPlanApiModels searchModel)
        {
            try
            {
                var query = _context.TRiskAfterPlans.AsQueryable();



                if (searchModel.riskFactorID != 0 && searchModel.riskFactorID != null)
                {
                    query = query.Where(bu =>
                        bu.RiskDefineId == searchModel.riskFactorID
                    );
                }
                if (searchModel.riskYear != 0 && searchModel.riskYear != null)
                {
                    query = query.Where(bu =>
                        bu.YearBudget == searchModel.riskYear
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
