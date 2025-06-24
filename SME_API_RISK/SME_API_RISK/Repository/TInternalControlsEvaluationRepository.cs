using SME_API_RISK.Entities;
using Microsoft.EntityFrameworkCore;
using SME_API_RISK.Models;

namespace SME_API_RISK.Repository
{
    public class TInternalControlsEvaluationRepository
    {
        private readonly RISKDBContext _context;

        public TInternalControlsEvaluationRepository(RISKDBContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TInternalControlsEvaluation evaluation)
        {
            try
            {
                await _context.TInternalControlsEvaluations.AddAsync(evaluation);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to add TInternalControlsEvaluation: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<TInternalControlsEvaluation>> GetAllAsync()
        {
            try
            {
                return await _context.TInternalControlsEvaluations.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get all TInternalControlsEvaluation: {ex.Message}");
                throw;
            }
        }

        // If your entity does not have a primary key, you may need to adjust this logic.
        public async Task<TInternalControlsEvaluation?> GetByIdAsync(string depart, string acti,int year)
        {
            try
            {
                return await _context.TInternalControlsEvaluations.FirstOrDefaultAsync(e => e.Departments == depart && e.Activities == acti && e.RiskYear == year);

              
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get TInternalControlsEvaluation by id: {ex.Message}");
               return null;
                //throw;
            }
        }

        public async Task UpdateAsync(TInternalControlsEvaluation evaluation)
        {
            try
            {
                _context.TInternalControlsEvaluations.Update(evaluation);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to update TInternalControlsEvaluation: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var entity = await _context.TInternalControlsEvaluations.FindAsync(id);
                if (entity != null)
                {
                    _context.TInternalControlsEvaluations.Remove(entity);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to delete TInternalControlsEvaluation: {ex.Message}");
                throw;
            }
        }
        public async Task<IEnumerable<TInternalControlsEvaluation>> GetAllAsyncSearch_InternalEvaluation(SearchTInternalControlsEvaluationModels searchModel)
        {
            try
            {
                var query = _context.TInternalControlsEvaluations.AsQueryable();



                if (searchModel.riskYear != 0 && searchModel.riskYear != null)
                {
                    query = query.Where(bu =>
                        bu.RiskYear == searchModel.riskYear
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
