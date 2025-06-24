using SME_API_RISK.Entities;
using Microsoft.EntityFrameworkCore;
using SME_API_RISK.Models;

namespace SME_API_RISK.Repository
{
    public class MRiskUniverseRepository
    {
        private readonly RISKDBContext _context;

        public MRiskUniverseRepository(RISKDBContext context)
        {
            _context = context;
        }

        public async Task AddAsync(MRiskUniverse riskUniverse)
        {
            try
            {
                await _context.MRiskUniverses.AddAsync(riskUniverse);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to add MRiskUniverse: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<MRiskUniverse>> GetAllAsync()
        {
            try
            {
                return await _context.MRiskUniverses.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get all MRiskUniverse: {ex.Message}");
                throw;
            }
        }

        public async Task<MRiskUniverse?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.MRiskUniverses.FindAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get MRiskUniverse by id: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateAsync(MRiskUniverse riskUniverse)
        {
            try
            {
                _context.MRiskUniverses.Update(riskUniverse);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to update MRiskUniverse: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var entity = await _context.MRiskUniverses.FindAsync(id);
                if (entity != null)
                {
                    _context.MRiskUniverses.Remove(entity);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to delete MRiskUniverse: {ex.Message}");
                throw;
            }
        }
        public async Task<IEnumerable<MRiskUniverse>> GetAllAsyncSearch_RiskUniverse(SearchRiskRiskUniverse searchModel)
        {
            try
            {
                var query = _context.MRiskUniverses.AsQueryable();



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
