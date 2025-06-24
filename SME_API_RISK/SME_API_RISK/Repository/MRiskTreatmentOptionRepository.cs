using SME_API_RISK.Entities;
using Microsoft.EntityFrameworkCore;
using SME_API_RISK.Models;

namespace SME_API_RISK.Repository
{
    public class MRiskTreatmentOptionRepository
    {
        private readonly RISKDBContext _context;

        public MRiskTreatmentOptionRepository(RISKDBContext context)
        {
            _context = context;
        }

        public async Task AddAsync(MRiskTreatmentOption treatmentOption)
        {
            try
            {
                await _context.MRiskTreatmentOptions.AddAsync(treatmentOption);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to add MRiskTreatmentOption: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<MRiskTreatmentOption>> GetAllAsync()
        {
            try
            {
                return await _context.MRiskTreatmentOptions.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get all MRiskTreatmentOption: {ex.Message}");
                throw;
            }
        }

        public async Task<MRiskTreatmentOption?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.MRiskTreatmentOptions.FindAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get MRiskTreatmentOption by id: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateAsync(MRiskTreatmentOption treatmentOption)
        {
            try
            {
                _context.MRiskTreatmentOptions.Update(treatmentOption);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to update MRiskTreatmentOption: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var entity = await _context.MRiskTreatmentOptions.FindAsync(id);
                if (entity != null)
                {
                    _context.MRiskTreatmentOptions.Remove(entity);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to delete MRiskTreatmentOption: {ex.Message}");
                throw;
            }
        }
        public async Task<IEnumerable<MRiskTreatmentOption>> GetAllAsyncSearch_RiskTreatmentOption(SearchRiskTreatmentOptionModels searchModel)
        {
            try
            {
                var query = _context.MRiskTreatmentOptions.AsQueryable();



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
