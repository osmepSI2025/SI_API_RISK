using SME_API_RISK.Entities;
using Microsoft.EntityFrameworkCore;
using SME_API_RISK.Models;

namespace SME_API_RISK.Repository
{
    public class TRiskRootCauseRepository
    {
        private readonly RISKDBContext _context;

        public TRiskRootCauseRepository(RISKDBContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TRiskRootCause rootCause)
        {
            try
            {
                await _context.TRiskRootCauses.AddAsync(rootCause);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to add TRiskRootCause: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<TRiskRootCause>> GetAllAsync()
        {
            try
            {
                return await _context.TRiskRootCauses.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get all TRiskRootCause: {ex.Message}");
                throw;
            }
        }

        public async Task<TRiskRootCause?> GetByIdAsync(int riskDefineId, string rootCauseType, string rootCauseName)
        {
            try
            {
                return await _context.TRiskRootCauses
                    .FirstOrDefaultAsync(e =>
                        e.RiskDefineId == riskDefineId &&
                        e.RootCauseType == rootCauseType &&
                        e.RootCauseName == rootCauseName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get TRiskRootCause by id: {ex.Message}");
                return null;
            }
        }

        public async Task UpdateAsync(TRiskRootCause rootCause)
        {
            try
            {
                _context.TRiskRootCauses.Update(rootCause);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to update TRiskRootCause: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(int riskDefineId, string rootCauseType, string rootCauseName)
        {
            try
            {
                var entity = await _context.TRiskRootCauses
                    .FirstOrDefaultAsync(e =>
                        e.RiskDefineId == riskDefineId &&
                        e.RootCauseType == rootCauseType &&
                        e.RootCauseName == rootCauseName);
                if (entity != null)
                {
                    _context.TRiskRootCauses.Remove(entity);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to delete TRiskRootCause: {ex.Message}");
                throw;
            }
        }
        public async Task<IEnumerable<TRiskRootCause>> GetByRiskDefineIdAsync(int riskDefineId)
        {
            try
            {
                return await _context.TRiskRootCauses
                    .Where(e => e.RiskDefineId == riskDefineId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get TRiskRootCause by RiskDefineId: {ex.Message}");
                throw;
            }
        }
        public async Task<IEnumerable<TRiskRootCause>> GetAllAsyncSearch_RiskTRootCause(SearchRiskTRootCauseModels searchModel)
        {
            try
            {
                var query = _context.TRiskRootCauses.AsQueryable();



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
