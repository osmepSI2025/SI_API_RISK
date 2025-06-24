using Microsoft.EntityFrameworkCore;
using SME_API_RISK.Entities;
using SME_API_RISK.Models;

namespace SME_API_RISK.Repository
{
    public class MRiskTypeRepository 
    {
        private readonly RISKDBContext _context;

        public MRiskTypeRepository(RISKDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MRiskType>> GetAllAsync()
        {
            return await _context.Set<MRiskType>().ToListAsync();
        }

        public async Task<MRiskType?> GetByIdAsync(int id)
        {
            return await _context.Set<MRiskType>().FindAsync(id);
        }

        public async Task AddAsync(MRiskType riskType)
        {
            _context.Set<MRiskType>().Add(riskType);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(MRiskType riskType)
        {
            _context.Set<MRiskType>().Update(riskType);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var riskType = await GetByIdAsync(id);
            if (riskType != null)
            {
                _context.Set<MRiskType>().Remove(riskType);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<MRiskType>> GetAllAsyncSearch_RiskType(SearchRiskType searchModel)
        {
            try
            {
                var query = _context.MRiskTypes.AsQueryable();

              

                if (!string.IsNullOrEmpty(searchModel.keyword))
                {
                    query = query.Where(bu =>
                        bu.Name.Contains(searchModel.keyword) 
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
