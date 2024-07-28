using Microsoft.EntityFrameworkCore;
using Trans9.DataAccess;
using Trans9.Models;

namespace Trans9.BLL
{
    public class DealerBll
    {
        private readonly DataDbContext _context;
        private readonly IHostEnvironment _env;
        public DealerBll(DataDbContext context, IHostEnvironment env = null)
        {
            _context = context;
            _env = env;
        }

        public async Task<Dealer> GetDealerById(Int64 id = 0)
        {
            DateTime dt = DateTime.Now;
            Dealer d = new Dealer();
            try
            {
                d = await _context.Dealer.FindAsync(id);

                if (d == null)
                {
                    d = new Dealer()
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

        public async Task<bool> AddOrUpdateDealer(long id, Dealer d)
        {
            bool success = false;
            try
            {
                d.status = comonStatus.active.ToString();
                if (id == 0)
                {
                    _context.Add(d);
                }
                else
                {
                    _context.Update(d);
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

        public async Task<List<Dealer>> GetDealerList()
        {
            List<Dealer> list = new List<Dealer>();
            try
            {
                list = await _context.Dealer.ToListAsync();
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

        public async Task<bool> DeleteDealer(Int64 id, string user)
        {
            bool success = false;
            try
            {
                var d = await _context.Dealer.FindAsync(id);
                if (d != null)
                {
                    d.updatedBy = user;
                    d.updatedDate = DateTime.Now;
                    d.status = comonStatus.deleted.ToString();
                    _context.Dealer.Update(d);
                }

                int n = await _context.SaveChangesAsync();

                success = (n > 0) ? true : false;
            }
            catch (Exception)
            {
            }
            return success;
        }

        private bool DealerExists(Int64 id = 0)
        {
            return _context.Dealer.Any(e => e.dealerId == id);
        }
    }
}
