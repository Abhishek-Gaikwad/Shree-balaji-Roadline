using Microsoft.EntityFrameworkCore;
using Trans9.DataAccess;
using Trans9.Models;

namespace Trans9.BLL
{
    public class VcMasterBll
    {
        private readonly DataDbContext _context;
        private readonly StoredProcedureDbContext _spc;
        private readonly IHostEnvironment _env;
        public VcMasterBll(DataDbContext context, StoredProcedureDbContext spc, IHostEnvironment env)
        {
            _context = context;
            _spc = spc;
            _env = env;
        }

        public async Task<VcMaster> GetVcMasterById(Int64 id = 0)
        {
            DateTime dt = DateTime.Now;
            VcMaster vc = new VcMaster();
            try
            {
                vc = await _context.VcMaster.FindAsync(id);


                if (vc == null)
                {
                    vc = new VcMaster()
                    { createdDate = dt, status = comonStatus.active.ToString() };
                }

                vc.updatedDate = dt;
            }
            catch (Exception)
            {
                throw;
            }
            return vc;
        }

        public async Task<bool> AddOrUpdateVcMaster(long id, VcMaster dst)
        {
            bool success = false;
            try
            {

                dst.status = comonStatus.active.ToString();
                if (id == 0)
                {
                    dst.updatedBy = "";
                    _context.Add(dst);
                }
                else
                {

                    dst.updatedBy = "newUser";
                    _context.Update(dst);
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

        public async Task<List<VcMaster>> GetVcMasterList()
        {
            List<VcMaster> list = new List<VcMaster>();
            try
            {
                list = await _context.VcMaster.ToListAsync();
                if (list.Any())
                {
                    list = (from s in list
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

        public async Task<bool> DeleteVcMaster(Int64 id, string user)
        {
            bool success = false;
            try
            {
                var dst = await _context.VcMaster.FindAsync(id);
                if (dst != null)
                {
                    dst.updatedBy = user;
                    dst.updatedDate = DateTime.Now;
                    dst.status = comonStatus.deleted.ToString();
                    _context.VcMaster.Update(dst);
                }

                int n = await _context.SaveChangesAsync();

                success = (n > 0) ? true : false;
            }
            catch (Exception)
            {
            }
            return success;
        }

        private bool VcMasterExists(Int64 id = 0)
        {
            return _context.VcMaster.Any(e => e._Id == id);
        }

        public async Task<List<VcInfo>> GetVcMasterByNo(string quoteId = null, string id = null)
        {
            List<VcInfo> list = new List<VcInfo>();
            try
            {
                list = await _spc.VcInfo
                        .FromSqlRaw("CALL `sbl_getVcInfoList`({0},{1})", quoteId, id)
                        .IgnoreQueryFilters()
                        .ToListAsync();

            }
            catch (Exception)
            {
                throw;
            }
            return list;
        }
    }
}
