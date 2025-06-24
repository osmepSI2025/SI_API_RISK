using SME_API_RISK.Entities;
using Microsoft.EntityFrameworkCore;
using SME_API_RISK.Models;

namespace SME_API_RISK.Repository
{
    public class MRiskOwnerRepository
    {
        private readonly RISKDBContext _context;

        public MRiskOwnerRepository(RISKDBContext context)
        {
            _context = context;
        }

        public async Task AddAsync(MRiskOwner riskOwner)
        {
            try
            {
                await _context.MRiskOwners.AddAsync(riskOwner);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to add MRiskOwner: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<MRiskOwner>> GetAllAsync()
        {
            try
            {
                return await _context.MRiskOwners.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get all MRiskOwner: {ex.Message}");
                throw;
            }
        }

        public async Task<MRiskOwner?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.MRiskOwners.FindAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get MRiskOwner by id: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateAsync(MRiskOwner riskOwner)
        {
            try
            {
                _context.MRiskOwners.Update(riskOwner);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to update MRiskOwner: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var entity = await _context.MRiskOwners.FindAsync(id);
                if (entity != null)
                {
                    _context.MRiskOwners.Remove(entity);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to delete MRiskOwner: {ex.Message}");
                throw;
            }
        }
        public async Task<IEnumerable<MRiskOwner>> GetAllAsyncSearch_RiskOwner(SearchRiskOwnerModels searchModel)
        {
            try
            {
                var query = _context.MRiskOwners.AsQueryable();



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
