using Microsoft.EntityFrameworkCore;
using Trans9.DataAccess;
using Trans9.Models;

namespace Trans9.BLL
{
    public class PumpMasterBll
    {
        private readonly DataDbContext _context;
        private readonly IHostEnvironment _env;
        public PumpMasterBll(DataDbContext context, IHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<PumpMaster> GetPumpById(Int64 id = 0)
        {
            DateTime dt = DateTime.Now;
            PumpMaster d = new PumpMaster();
            try
            {
                d = await _context.PumpMaster.FindAsync(id);
                if (d == null)
                {
                    d = new PumpMaster()
                    { createdDate = dt, status = comonStatus.active.ToString() };
                }

                d.updatedDate = dt;
            }
            catch (Exception)
            {
                throw;
            }
            return d;
        }

        public async Task<bool> AddOrUpdatePump(long id, PumpMaster pm)
        {
            bool success = false;
            try
            {
                pm.status = comonStatus.active.ToString();
                if (id == 0)
                {
                    pm.updatedBy = "";
                    _context.Add(pm);
                }
                else
                {

                    pm.updatedBy = "newUser";
                    _context.Update(pm);
                }
                int n = await _context.SaveChangesAsync();
                success = (n > 0) ? true : false;
            }
            catch (Exception)
            {
                throw;
            }
            return success;
        }

        public async Task<List<PumpMaster>> GetPumpList()
        {
            List<PumpMaster> list = new List<PumpMaster>();
            try
            {
                var l = await _context.PumpMaster.ToListAsync();
                if (l.Any())
                {
                    list = (from s in l
                            where s.status.Equals(comonStatus.active.ToString())
                            select s
                            ).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return list;
        }

        public async Task<bool> DeletePump(Int64 id, string user)
        {
            bool success = false;
            try
            {
                var pm = await _context.PumpMaster.FindAsync(id);
                if (pm != null)
                {
                    pm.status = comonStatus.deleted.ToString();
                    pm.updatedDate = DateTime.Now;
                    pm.updatedBy = user;
                    _context.PumpMaster.Update(pm);
                }

                int n = await _context.SaveChangesAsync();

                success = (n > 0) ? true : false;
            }
            catch (Exception)
            {
            }
            return success;
        }

        private bool PumpMasterExists(Int64 id = 0)
        {
            return _context.PumpMaster.Any(e => e.pumpId == id);
        }
    }
}
