using Microsoft.EntityFrameworkCore;
using Trans9.DataAccess;
using Trans9.Models;
using Trans9.Utility;

namespace Trans9.BLL
{
    public class DriverBll
    {
        private readonly DataDbContext _context;
        private readonly IHostEnvironment _env;
        public DriverBll(DataDbContext context, IHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<Driver> GetDriverById(Int64 id = 0)
        {
            DateTime dt = DateTime.Now;
            Driver d = new Driver();
            try
            {
                d = await _context.Driver.FindAsync(id);

                if (d == null)
                {
                    d = new Driver()
                    { createdDate = dt, status = driverStatus.active.ToString() };
                }
                else
                {
                    Attachments a = _context.Attachments.Where(x => x.driverId == d.driverId).FirstOrDefault();
                    if (a != null)
                    {
                        d.aadharDocs = string.IsNullOrEmpty(a.aadharCard) ? new List<string>()
                            : a.aadharCard.Split(",").Select(x => x).ToList();
                        d.licenseDocs = string.IsNullOrEmpty(a.license) ? new List<string>()
                            : a.license.Split(",").Select(x => x).ToList();
                        d.photos = string.IsNullOrEmpty(a.photo) ? new List<string>()
                            : a.photo.Split(",").Select(x => x).ToList();
                        d.bankDocs = string.IsNullOrEmpty(a.bankDetails) ? new List<string>()
                            : a.bankDetails.Split(",").Select(x => x).ToList();
                    }
                }

                d.updatedDate = dt;
            }
            catch (Exception)
            {
                throw;
            }
            return d;
        }

        public async Task<bool> AddOrUpdateDriver(long id, Driver dst)
        {
            bool success = false;
            Attachments attach = new Attachments();
            try
            {
                if (id != 0)
                {
                    attach = _context.Attachments.Where(x => x.driverId == id).FirstOrDefault();
                    attach = attach != null ? attach : new Attachments();
                }

                if (dst.aadharcard.Count > 0)
                {
                    var aadhar = GenericFunctions.UploadDocuments(_env, dst.aadharcard, dst.driverName.Replace(" ", ""));
                    attach.aadharCard = string.Join(",", aadhar);
                }
                if (dst.licensecard.Count > 0)
                {
                    var license = GenericFunctions.UploadDocuments(_env, dst.licensecard, dst.driverName.Replace(" ", ""));
                    attach.license = string.Join(",", license);
                }
                if (dst.photo.Count > 0 || dst.cameraPhoto!=null)
                {

                    var photo = string.IsNullOrWhiteSpace(dst.cameraPhoto)? GenericFunctions.UploadDocuments(_env, dst.photo, dst.driverName.Replace(" ", ""))
                                : GenericFunctions.UploadDocumentByBase64(_env, dst.cameraPhoto, dst.driverName.Replace(" ", ""));

                    attach.photo = string.Join(",", photo);
                }
                if (dst.bankdetail.Count > 0)
                {
                    var cheque = GenericFunctions.UploadDocuments(_env, dst.bankdetail, dst.driverName.Replace(" ", ""));
                    attach.bankDetails = string.Join(",", cheque);
                }

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
                if (success)
                {
                    attach.driverId = dst.driverId;
                    if (attach.id == 0)
                    {
                        _context.Add(attach);
                    }
                    else
                    {
                        _context.Update(attach);
                    }
                }
                int m = await _context.SaveChangesAsync();
                success = (m > 0) ? true : false;
            }
            catch (Exception)
            {
                throw;
            }
            return success;
        }

        //private string[] UploadDocuments(List<IFormFile> formFiles, string dir)
        //{
        //    int i = 0;
        //    string[] docs = new string[formFiles.Count];
        //    try
        //    {
        //        foreach (var file in formFiles)
        //        {
        //            var path = $"docs/{dir}";
        //            string filePath = Path.Combine(_env.ContentRootPath, $"wwwroot/{path}");

        //            //create folder if not exist
        //            if (!Directory.Exists(filePath))
        //                Directory.CreateDirectory(filePath);

        //            filePath = Path.Combine(filePath, file.FileName);

        //            using (var stream = new FileStream(filePath, FileMode.Create))
        //            {
        //                file.CopyTo(stream);

        //            }
        //            docs[i++] = $"{path}/{file.FileName}";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //    return docs;
        //}

        public async Task<List<Driver>> GetDriverList()
        {
            List<Driver> list = new List<Driver>();
            try
            {
                list = await _context.Driver.ToListAsync();
                if (list.Any())
                {
                    list = (from s in list
                            where (s.status.Equals(driverStatus.active.ToString())
                             || s.status.Equals(driverStatus.blocked.ToString()))
                            orderby s.driverId descending
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

        public async Task<bool> DeleteDriver(Int64 id, string user)
        {
            bool success = false;
            try
            {
                var dst = await _context.Driver.FindAsync(id);
                if (dst != null)
                {
                    dst.status = driverStatus.deleted.ToString();
                    dst.updatedDate = DateTime.Now;
                    dst.updatedBy = user;
                    _context.Update(dst);
                }

                int n = await _context.SaveChangesAsync();

                success = (n > 0) ? true : false;
            }
            catch (Exception)
            {
            }
            return success;
        }

        private bool DriverExists(Int64 id = 0)
        {
            return _context.Driver.Any(e => e.driverId == id);
        }

        public async Task<Driver> GetDriverByAdhaarOrLc(string aadhar, string dl)
        {
            Driver d = new Driver();
            try
            {
                var list = await _context.Driver.ToListAsync();
                if (list.Any())
                {
                    d = list.Where(o => (o.aadharNo == aadhar | o.dlNo == dl)).FirstOrDefault();
                }
                if (d == null)
                    d = new Driver();
            }
            catch (Exception)
            {
                throw;
            }
            return d;
        }

        public bool DriverIsExist(string param, string condition)
        {
            bool success = false;
            try
            {
                switch (condition)
                {
                    case "aadhar":
                        success = _context.Driver.Any(e => e.aadharNo == param.Replace(" ", ""));
                        break;
                    case "dl":
                        success = _context.Driver.Any(e => e.dlNo == param.Replace(" ", ""));
                        break;
                }
            }
            catch (Exception)
            {
                success = false;
            }
            return success;
        }
    }
}
