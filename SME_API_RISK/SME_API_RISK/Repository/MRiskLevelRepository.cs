using SME_API_RISK.Entities;
using Microsoft.EntityFrameworkCore;
using SME_API_RISK.Models;

namespace SME_API_RISK.Repository
{
    public class MRiskLevelRepository
    {
        private readonly RISKDBContext _context;

        public MRiskLevelRepository(RISKDBContext context)
        {
            _context = context;
        }

        public async Task AddAsync(MRiskLevel riskLevel)
        {
            try
            {
                await _context.MRiskLevels.AddAsync(riskLevel);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to add MRiskLevel: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<MRiskLevel>> GetAllAsync()
        {
            try
            {
                return await _context.MRiskLevels.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get all MRiskLevel: {ex.Message}");
                throw;
            }
        }

        public async Task<MRiskLevel?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.MRiskLevels.FindAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get MRiskLevel by id: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateAsync(MRiskLevel riskLevel)
        {
            try
            {
                _context.MRiskLevels.Update(riskLevel);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to update MRiskLevel: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var entity = await _context.MRiskLevels.FindAsync(id);
                if (entity != null)
                {
                    _context.MRiskLevels.Remove(entity);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to delete MRiskLevel: {ex.Message}");
                throw;
            }
        }
        public async Task<IEnumerable<MRiskLevel>> GetAllAsyncSearch_RiskLevels()
        {
            try
            {
                var query = _context.MRiskLevels.AsQueryable();



                //if (!string.IsNullOrEmpty(searchModel.keyword))
                //{
                //    query = query.Where(bu =>
                //        //bu.Levels.Contains(searchModel.keyword)||
                //        bu.LikelihoodDefine.Contains(searchModel.keyword)||
                //        bu.ImpactDefine.Contains(searchModel.keyword)

                //    );
                //}

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
