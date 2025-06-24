using SME_API_RISK.Entities;
using Microsoft.EntityFrameworkCore;

namespace SME_API_RISK.Repository
{
    public class TRiskPlanExistingControlRepository
    {
        private readonly RISKDBContext _context;

        public TRiskPlanExistingControlRepository(RISKDBContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TRiskPlanExistingControl entity)
        {
            try
            {
                await _context.TRiskPlanExistingControls.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to add TRiskPlanExistingControl: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<TRiskPlanExistingControl>> GetAllAsync()
        {
            try
            {
                return await _context.TRiskPlanExistingControls.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get all TRiskPlanExistingControl: {ex.Message}");
                throw;
            }
        }

        public async Task<TRiskPlanExistingControl?> GetByIdAsync(int riskDefineId, string existingControl)
        {
            try
            {
                return await _context.TRiskPlanExistingControls
                    .FirstOrDefaultAsync(e => e.RiskDefineId == riskDefineId && e.ExistingControl == existingControl);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get TRiskPlanExistingControl by id: {ex.Message}");
                return null;
            }
        }

        public async Task UpdateAsync(TRiskPlanExistingControl entity)
        {
            try
            {
                _context.TRiskPlanExistingControls.Update(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to update TRiskPlanExistingControl: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(int riskDefineId, string existingControl)
        {
            try
            {
                var entity = await _context.TRiskPlanExistingControls
                    .FirstOrDefaultAsync(e => e.RiskDefineId == riskDefineId && e.ExistingControl == existingControl);
                if (entity != null)
                {
                    _context.TRiskPlanExistingControls.Remove(entity);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to delete TRiskPlanExistingControl: {ex.Message}");
                throw;
            }
        }
        public async Task<IEnumerable<TRiskPlanExistingControl>> GetAllAsyncSearch_PlanExistingControl(SearchRiskPlanExistingControlModels searchModel)
        {
            try
            {
                var query = _context.TRiskPlanExistingControls.AsQueryable();



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
