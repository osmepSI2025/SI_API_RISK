using SME_API_RISK.Entities;
using Microsoft.EntityFrameworkCore;

namespace SME_API_RISK.Repository
{
    public class TRiskEmergencyPlanRepository
    {
        private readonly RISKDBContext _context;

        public TRiskEmergencyPlanRepository(RISKDBContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TRiskEmergencyPlan entity)
        {
            try
            {
                await _context.TRiskEmergencyPlans.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to add TRiskEmergencyPlan: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<TRiskEmergencyPlan>> GetAllAsync()
        {
            try
            {
                return await _context.TRiskEmergencyPlans.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get all TRiskEmergencyPlan: {ex.Message}");
                throw;
            }
        }

        public async Task<TRiskEmergencyPlan?> GetByIdAsync(int riskDefineId,string rootCauseType,string rootCauseName)
        {
            try
            {
                return await _context.TRiskEmergencyPlans
                    .FirstOrDefaultAsync(e => e.RiskDefineId == riskDefineId &&
                    e.RootCauseType == rootCauseType &&
                    e.RootCauseName == rootCauseName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get TRiskEmergencyPlan by id: {ex.Message}");
                return null;
            }
        }
      
        public async Task UpdateAsync(TRiskEmergencyPlan entity)
        {
            try
            {
                _context.TRiskEmergencyPlans.Update(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to update TRiskEmergencyPlan: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(int riskDefineId)
        {
            try
            {
                var entity = await _context.TRiskEmergencyPlans
                    .FirstOrDefaultAsync(e => e.RiskDefineId == riskDefineId);
                if (entity != null)
                {
                    _context.TRiskEmergencyPlans.Remove(entity);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to delete TRiskEmergencyPlan: {ex.Message}");
                throw;
            }
        }
        public async Task<IEnumerable<TRiskEmergencyPlan>> GetAllAsyncSearch_EmergencyPlan(SearchRiskEmergencyPlanModels searchModel)
        {
            try
            {
                var query = _context.TRiskEmergencyPlans.AsQueryable(); // Corrected entity set

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
                return Enumerable.Empty<TRiskEmergencyPlan>();
            }
        }

    }
}
