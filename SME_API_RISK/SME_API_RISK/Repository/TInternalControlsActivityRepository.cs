using SME_API_RISK.Entities;
using Microsoft.EntityFrameworkCore;
using SME_API_RISK.Models;

namespace SME_API_RISK.Repository
{
    public class TInternalControlsActivityRepository
    {
        private readonly RISKDBContext _context;

        public TInternalControlsActivityRepository(RISKDBContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TInternalControlsActivity activity)
        {
            try
            {
                await _context.TInternalControlsActivities.AddAsync(activity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to add TInternalControlsActivity: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<TInternalControlsActivity>> GetAllAsync()
        {
            try
            {
                return await _context.TInternalControlsActivities.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get all TInternalControlsActivity: {ex.Message}");
                throw;
            }
        }

        public async Task<TInternalControlsActivity?> GetByIdAsync(string depart,string acti)
        {
            try
            {
                // If there is no primary key, you may need to adjust this logic.
               // return await _context.TInternalControlsActivities.FindAsync(id);
                return await _context.TInternalControlsActivities.FirstOrDefaultAsync(e => e.Departments == depart && e.Activities == acti );

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get TInternalControlsActivity by id: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateAsync(TInternalControlsActivity activity)
        {
            try
            {
                _context.TInternalControlsActivities.Update(activity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to update TInternalControlsActivity: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var entity = await _context.TInternalControlsActivities.FindAsync(id);
                if (entity != null)
                {
                    _context.TInternalControlsActivities.Remove(entity);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to delete TInternalControlsActivity: {ex.Message}");
                throw;
            }
        }
        public async Task<IEnumerable<TInternalControlsActivity>> GetAllAsyncSearch_InternalActivity(SearchTInternalControlsActivityModels searchModel)
        {
            try
            {
                var query = _context.TInternalControlsActivities.AsQueryable();



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
