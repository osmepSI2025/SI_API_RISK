using SME_API_RISK.Entities;
using Microsoft.EntityFrameworkCore;
using SME_API_RISK.Models;

namespace SME_API_RISK.Repository
{
    public class TRiskKpiRepository
    {
        private readonly RISKDBContext _context;

        public TRiskKpiRepository(RISKDBContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TRiskKpi riskKpi)
        {
            try
            {
                await _context.TRiskKpis.AddAsync(riskKpi);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to add TRiskKpi: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<TRiskKpi>> GetAllAsync()
        {
            try
            {
                return await _context.TRiskKpis.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get all TRiskKpi: {ex.Message}");
                throw;
            }
        }

        public async Task<TRiskKpi?> GetByIdAsync(int riskDefineId)
        {
            try
            {
                return await _context.TRiskKpis.FindAsync(riskDefineId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get TRiskKpi by id: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateAsync(TRiskKpi riskKpi)
        {
            try
            {
                _context.TRiskKpis.Update(riskKpi);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to update TRiskKpi: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(int riskDefineId)
        {
            try
            {
                var entity = await _context.TRiskKpis.FindAsync(riskDefineId);
                if (entity != null)
                {
                    _context.TRiskKpis.Remove(entity);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to delete TRiskKpi: {ex.Message}");
                throw;
            }
        }
        public async Task<IEnumerable<TRiskKpi>> GetAllAsyncSearch_RiskTKpi(SearchRiskTkpiModels searchModel)
        {
            try
            {
                var query = _context.TRiskKpis.AsQueryable();



                if (searchModel.riskFactorID!=0&& searchModel.riskFactorID!=null)
                {
                    query = query.Where(bu =>
                        bu.RiskDefineId == searchModel.riskFactorID
                    );
                }
                if (searchModel.keyword!="" && searchModel.keyword != null)
                {
                    query = query.Where(bu =>
                        bu.Kpis.Contains( searchModel.keyword)
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
