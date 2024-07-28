using Microsoft.EntityFrameworkCore;
using Trans9.DataAccess;
using Trans9.Models;

namespace Trans9.BLL
{
    public class DestinationBll
    {
        private readonly DataDbContext _context;
        private readonly IHostEnvironment _env;
        public DestinationBll(DataDbContext context, IHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<Destination> GetDestinationById(Int64 id = 0)
        {
            DateTime dt = DateTime.Now;
            Destination d = new Destination();
            try
            {
                d = await _context.Destination.FindAsync(id);

                if (d == null)
                {
                    d = new Destination()
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

        public async Task<bool> AddOrUpdateDestination(long id, Destination dst)
        {
            bool success = false;
            try
            {
                //dst.status = comonStatus.active.ToString();
                if (id == 0)
                {
                    _context.Add(dst);
                }
                else
                {
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

        public async Task<List<Destination>> GetDestinationList()
        {
            List<Destination> list = new List<Destination>();
            try
            {
                list = await _context.Destination.ToListAsync();
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

        public async Task<bool> DeleteDestination(Int64 id, string user)
        {
            bool success = false;
            try
            {
                var dst = await _context.Destination.FindAsync(id);
                if (dst != null)
                {
                    dst.status = comonStatus.deleted.ToString();
                    dst.updatedBy = user;
                    dst.updatedDate = DateTime.Now;
                    _context.Destination.Update(dst);
                }

                int n = await _context.SaveChangesAsync();

                success = (n > 0) ? true : false;
            }
            catch (Exception)
            {
            }
            return success;
        }

        private bool DestinationExists(Int64 id = 0)
        {
            return _context.Destination.Any(e => e.detsinationId == id);
        }
    }
}
