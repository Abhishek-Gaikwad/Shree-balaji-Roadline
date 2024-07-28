using DocumentFormat.OpenXml.Math;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Trans9.DataAccess;
using Trans9.Models;

namespace Trans9.BLL
{
    public class AppUserBll
    {
        private readonly AppUserDbContext _context;
        private readonly IHostEnvironment _env;
        public AppUserBll(AppUserDbContext context, IHostEnvironment env = null)
        {
            _context = context;
            _env = env;
        }

        public async Task<AppUser> GetUserByIdPassword(LoginModel m)
        {
            AppUser d = new AppUser();
            try
            {
                d = await _context.AppUsers.Where(o =>
                (o.userName.Equals(m.email) | o.email.Equals(m.email)) &&
                o.password.Equals(m.password)).FirstOrDefaultAsync();

                if (d != null)
                {
                    var pL = await _context.Pages.ToListAsync();
                    var role = await _context.RoleGroup.Where(o => o.roleName.Equals(d.roleName)).FirstOrDefaultAsync();

                    var pageIds = role.pageIds.Split(",").Select(x => Convert.ToInt32(x)).ToList();
                    d.pages = pL.Where(p => pageIds.Contains(p.id)).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return d;
        }

        public async Task<List<AppUser>> GetUserList()
        {
            List<AppUser> list = new List<AppUser>();
            try
            {
                var l = await _context.AppUsers.ToListAsync();
                if (l.Any())
                {
                    list = (from s in l
                            where !s.status.Equals("ok")
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

        public async Task<AppUser> GetUserById(Int64 id = 0)
        {
            AppUser d = new AppUser();
            try
            {
                d = await _context.AppUsers.FindAsync(id);

                if (d == null)
                    d = new AppUser();
            }
            catch (Exception)
            {
                throw;
            }
            return d;
        }

        public async Task<List<string>> GetUserRoles()
        {
            List<string> list = new List<string>();
            try
            {
                var l = await _context.RoleGroup.Where(o => !o.roleName.Equals("superadmin")).ToListAsync();
                if (l.Any())
                {
                    list = l.DistinctBy(x => x.roleName).Select(o => o.roleName).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return list;
        }

        public async Task<bool> AddOrUpdateUser(long id, AppUser pm)
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

        public async Task<bool> DeleteUser(Int64 id = 0)
        {
            bool success = false;
            try
            {
                var user = await _context.AppUsers.FindAsync(id);
                if (user != null)
                {
                    user.status = comonStatus.deleted.ToString();
                    _context.AppUsers.Update(user);
                }

                int n = await _context.SaveChangesAsync();

                success = (n > 0) ? true : false;
            }
            catch (Exception)
            {
            }
            return success;
        }

        public async Task<List<UserRole>> GetRolesList()
        {
            List<UserRole> list = new List<UserRole>();
            try
            {
                var rL = await _context.RoleGroup.ToListAsync();
                var pL = await _context.Pages.ToListAsync();
                if (rL.Any())
                {
                    list = (from r in rL
                            where !r.roleName.Equals("superadmin")
                            select r
                            ).ToList();

                    foreach (var r in list)
                    {
                        var pageIds = r.pageIds.Split(",").Select(x => Convert.ToInt32(x)).ToList();
                        r.pageIds = pL.Where(x => pageIds.Contains(x.id)).Select(x => x.pageName).Aggregate((i, j) => i + ", " + j);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return list;
        }

        public async Task<RoleModal> GetRoleById(int id = 0)
        {
            RoleModal d = new RoleModal();
            try
            {


                if (id != 0)
                {
                    var role = await _context.RoleGroup.FindAsync(id);
                    d.roleName = role.roleName;
                    d.id = role.id;
                    d.pages = role.pageIds.Split(",").Select(x => Convert.ToInt32(x)).ToList();
                }
                else
                {
                    d = new RoleModal();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return d;
        }

        public async Task<bool> AddOrUpdateRole(int id, RoleModal r)
        {
            bool success = false;
            try
            {
                if (id == 0)
                {
                    if (r.pages.Any())
                    {
                        UserRole urole = new UserRole()
                        {
                            roleName = r.roleName,
                            pageIds = string.Join(",", r.pages)
                        };
                        _context.Add(urole);
                    }
                }
                else
                {
                    var role = await _context.RoleGroup.FindAsync(id);

                    if (r.pages.Any())
                    {
                        role.roleName = r.roleName;
                        role.pageIds = string.Join(",", r.pages);
                        _context.Update(role);
                    }
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

        public async Task<List<Pages>> GetPages()
        {
            List<Pages> list = new List<Pages>();
            try
            {
                var l = await _context.Pages.ToListAsync();
                if (l.Any())
                {
                    list = l;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return list;
        }

        public async Task<bool> DeleteRole(int id)
        {
            bool success = false;
            try
            {
                if (id != 0)
                {
                    var role = await _context.RoleGroup.FindAsync(id);
                    _context.RoleGroup.Remove(role);

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
    }
}