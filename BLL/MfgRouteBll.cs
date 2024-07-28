using Microsoft.EntityFrameworkCore;
using System.Data;
using Trans9.DataAccess;
using Trans9.Models;
using Trans9.Utility;

namespace Trans9.BLL
{
    public class MfgRouteBll
    {
        private readonly DataDbContext _context;
        private readonly IHostEnvironment _env;
        public MfgRouteBll(DataDbContext context, IHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<MfgRoute> GetMfgRouteById(Int64 id = 0)
        {
            DateTime dt = DateTime.Now;
            MfgRoute d = new MfgRoute();
            try
            {
                d = await _context.MfgRoute.FindAsync(id);

                if (d == null)
                {
                    d = new MfgRoute()
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

        public async Task<bool> AddOrUpdateMfgRoute(long id, MfgRoute pm)
        {
            bool success = false;
            try
            {
                pm.status = comonStatus.active.ToString();
                if (id == 0)
                {
                    _context.Add(pm);
                }
                else
                {
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

        public async Task<List<MfgRoute>> GetMfgRouteList()
        {
            List<MfgRoute> list = new List<MfgRoute>();
            try
            {
                list = await _context.MfgRoute
                    .Where(x=>x.status.Equals(comonStatus.active.ToString()))
                    .OrderByDescending(x => x._routId)
                    .Take(100)
                    .ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }

            return list;
        }

        public async Task<bool> DeleteMfgRoute(Int64 id, string user)
        {
            bool success = false;
            try
            {
                var pm = await _context.MfgRoute.FindAsync(id);
                if (pm != null)
                {
                    pm.status = comonStatus.deleted.ToString();
                    pm.updatedBy = user;
                    pm.updatedDate = DateTime.Now;
                    _context.MfgRoute.Update(pm);
                }

                int n = await _context.SaveChangesAsync();

                success = (n > 0) ? true : false;
            }
            catch (Exception)
            {
            }
            return success;
        }

        private bool MfgRouteExists(Int64 id = 0)
        {
            return _context.MfgRoute.Any(e => e._routId == id);
        }

        public async Task<MfgRoute> GetMfgRouteByRoute(string id, string quoteId, string vcNo)
        {
            MfgRoute mr = new MfgRoute();
            try
            {
                mr = await _context.MfgRoute.Where(x => x.mfgRoute.Equals(id)).FirstOrDefaultAsync();
                if (mr!=null)
                {
                    if (!string.IsNullOrWhiteSpace(quoteId)) {
                        var mfg = await _context.QuoteDetails.Where(x=> x.quoteId.Equals(quoteId) && x.modelDesc.Equals(vcNo)).FirstOrDefaultAsync();

                        if (mfg!=null) {
                            mr.basicRate = mfg.basicFreight;
                            mr.inroute = mfg.enRoute;
                            mr.totalExp = mfg.totalFreight;
                        }
                    }
                }
                if (mr == null)
                    mr = new MfgRoute();
            }
            catch (Exception)
            {
                throw;
            }
            return mr;
        }

        public async Task<bool> AddBulkMfgRoutes(BulkInsert modal, string user)
        {
            int cnt = 0;
            DateTime date = DateTime.Now;
            bool success = false;
            List<MfgRoute> routes = new List<MfgRoute>();
            try
            {
                DataTable dt = GenericFunctions.ReadExcel(modal.dataFile);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow r in dt.Rows)
                    {
                        if (!isExists($"{r["mfgRoute"]}"))
                        {
                            MfgRoute route = new MfgRoute()
                            {
                                mfgRoute = $"{r["mfgRoute"]}",
                                basicRate = Convert.ToDecimal($"{r["basicRate"]}"),
                                inroute = Convert.ToDecimal($"{r["inroute"]}"),
                                tollExp = Convert.ToDecimal($"{r["tollExp"]}"),
                                totalExp = Convert.ToDecimal($"{r["totalExp"]}"),
                                createdBy = user,
                                updatedBy = user,
                                createdDate = date,
                                updatedDate = date,
                                status = comonStatus.active.ToString()
                            };
                            await _context.MfgRoute.AddAsync(route);

                            cnt = 1;
                        }
                        else
                        {
                            cnt += 1;
                        }
                    }

                    int n = await _context.SaveChangesAsync();

                    success = (n > 0 | cnt > 0) ? true : false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return success;
        }

        private bool isExists(string route)
        {
            bool exists = true;
            try
            {
                if (!string.IsNullOrEmpty(route))
                {
                    exists = _context.MfgRoute.Any(x => x.mfgRoute.Equals(route));
                }
            }
            catch (Exception)
            {
                throw;
            }

            return exists;
        }
    }
}
