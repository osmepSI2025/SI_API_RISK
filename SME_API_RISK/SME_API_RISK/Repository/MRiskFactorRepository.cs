using Microsoft.EntityFrameworkCore;
using SME_API_RISK.Entities;
using SME_API_RISK.Models;

namespace SME_API_RISK.Repository
{
    public class MRiskFactorRepository
    {
        private readonly RISKDBContext _context;

        public MRiskFactorRepository(RISKDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MRiskFactor>> GetAllAsync()
        {
            return await _context.MRiskFactors.ToListAsync();
        }

        public async Task<MRiskFactor?> GetByIdAsync(int RiskDefineId)
        {
            try
            {
                return await _context.MRiskFactors
        .FirstOrDefaultAsync(e => e.RiskDefineId == RiskDefineId);
            }
            catch (Exception ex)
            {
                return null;
            }
        
        }

        public async Task CreateAsync(MRiskFactor riskFactor)
        {
            await _context.MRiskFactors.AddAsync(riskFactor);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(MRiskFactor riskFactor)
        {
            _context.MRiskFactors.Update(riskFactor);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var riskFactor = await GetByIdAsync(id);
            if (riskFactor == null) return false;
            _context.MRiskFactors.Remove(riskFactor);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<IEnumerable<MRiskFactor>> GetAllAsyncSearch_RiskFactor(SearchRiskFactor searchModel)
        {
            try
            {
                var query = _context.MRiskFactors.AsQueryable();

                if (searchModel.riskYear != 0 && searchModel.riskYear!=null)
                {
                    query = query.Where(bu => bu.RiskYear == searchModel.riskYear);
                }

                if (!string.IsNullOrEmpty(searchModel.keyword))
                {
                    query = query.Where(bu =>
                        bu.RiskOwnerName.Contains(searchModel.keyword) ||
                        bu.RiskRootCause.Contains(searchModel.keyword) ||
                        bu.RiskTypeName.Contains(searchModel.keyword) ||
                        bu.RiskRfname.Contains(searchModel.keyword)
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
