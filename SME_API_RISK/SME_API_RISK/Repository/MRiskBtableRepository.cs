using SME_API_RISK.Entities;
using Microsoft.EntityFrameworkCore;
using SME_API_RISK.Models;

namespace SME_API_RISK.Repository
{
    public class MRiskBtableRepository
    {
        private readonly RISKDBContext _context;

        public MRiskBtableRepository(RISKDBContext context)
        {
            _context = context;
        }

        public async Task AddAsync(MRiskBtable riskBtable)
        {
            try
            {
                await _context.MRiskBtables.AddAsync(riskBtable);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to add MRiskBtable: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<MRiskBtable>> GetAllAsync()
        {
            try
            {
                return await _context.MRiskBtables.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get all MRiskBtable: {ex.Message}");
                throw;
            }
        }

        public async Task<MRiskBtable?> GetByIdAsync(int levels,string performance)
        {
            try
            {
                return await _context.MRiskBtables.FirstOrDefaultAsync(e=>e.Levels ==levels && e.Performance ==performance);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get MRiskBtable by id: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateAsync(MRiskBtable riskBtable)
        {
            try
            {
                _context.MRiskBtables.Update(riskBtable);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to update MRiskBtable: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var entity = await _context.MRiskBtables.FindAsync(id);
                if (entity != null)
                {
                    _context.MRiskBtables.Remove(entity);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to delete MRiskBtable: {ex.Message}");
                throw;
            }
        }
        public async Task<IEnumerable<MRiskBtable>> GetAllAsyncSearch_RiskBTable()
        {
            try
            {
                var query = _context.MRiskBtables.AsQueryable();



                //if (!string.IsNullOrEmpty(searchModel.keyword))
                //{
                //    query = query.Where(bu =>
                //        bu.OldWork.Contains(searchModel.keyword)||
                //        bu.Performance.Contains(searchModel.keyword) 
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
