using Microsoft.EntityFrameworkCore;
using Trans9.DataAccess;
using Trans9.Models;

namespace Trans9.BLL
{
    public class IncidenceBll
    {
        private readonly DataDbContext _context;
        private readonly IHostEnvironment _env;
        public IncidenceBll(DataDbContext context, IHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public int AccidentOrBreakdownCount()
        {
            // Get the current month and year
            DateTime financialYearStart = new DateTime(2024, 4, 1);
            DateTime financialYearEnd = new DateTime(2025, 3, 31);

            // Filter the shipments that are "ACCIDENT" or "BREAKDOWN" and match the current month and year
            int count = _context.Shipment.Count(e => (e.status == "ACCIDENT" || e.status == "BREAKDOWN") &&
                                                 e.shipmentDate >= financialYearStart &&
                                             e.shipmentDate <= financialYearEnd);

            return count;
        }
        public decimal DieselLit()
        {
            // Get the current date
            DateTime currentDate = DateTime.Now.Date;

            // Sum a particular amount for shipments that are "ACTIVE" and match the current month and year
            decimal sumLit = _context.Marching
                .Where(e => e.status == "active" && e.createdDate.Value.Date == currentDate)
                .Sum(e => e.spdQty); // Replace "YourAmountProperty" with the actual property you want to sum

            return sumLit;
        }

        public decimal DieselAmount()
        {
            // Get the current month and year
            DateTime currentDate = DateTime.Now.Date;

            // Sum a particular amount for shipments that are "ACTIVE" and match the current month and year
            decimal sumAmount = _context.Marching
                .Where(e => e.status == "ACTIVE" && e.createdDate.Value.Date == currentDate)
                .Sum(e => e.spdAmount); // Replace "YourAmountProperty" with the actual property you want to sum

            return sumAmount;
        }
        public async Task<Incidence> GetIncedentInfo(Int64 id)
        {
            DateTime dt = DateTime.Now;
            Incidence inc = new Incidence();
            try
            {
                var sl = await _context.Shipment.Where(x => x.shipmentId == id).FirstOrDefaultAsync();
                if (sl != null)
                {
                    inc.shipmentNo = sl.shipmentNo;
                    inc.shipmentId = sl.shipmentId;
                    inc.createdDate = dt;
                    inc.status = comonStatus.active.ToString();
                }

                inc.updatedDate = dt;
            }
            catch (Exception)
            {
                throw;
            }
            return inc;
        }

        public async Task<Authority> GetAuthorityInfo(Int64 id)
        {
            DateTime dt = DateTime.Now;
            Authority auth = new Authority();
            try
            {
                var sl = await _context.Shipment.Where(x => x.shipmentId == id).FirstOrDefaultAsync();
                if (sl != null)
                {
                    auth.shipmentId = sl.shipmentId;
                    auth.createdDate = dt;
                    auth.chasisNo = sl.chasisNo;
                    auth.modelDesc = sl.modelDesc;
                    auth.location = sl.location;
                    auth.status = comonStatus.active.ToString();
                    auth.Id = 0;
                }

                auth.updatedDate = dt;
            }
            catch (Exception)
            {
                throw;
            }
            return auth;
        }

        public async Task<Tuple<bool, Int64>> BreakdownOrAccidentUpdate(Int64 id, Incidence inc)
        {
            Int64 incidentId = 0;
            bool success = false;
            string attached = string.Empty;
            try
            {
                var m = _context.Marching.Where(x => x.shipmentId == inc.shipmentId).FirstOrDefault();
                inc.tempRegNo = (m != null) ? m.tempRegNo : inc.tempRegNo;
                if (inc.incidenceFiles != null)
                {
                    if (inc.incidenceFiles.Count > 0)
                    {
                        foreach (var file in inc.incidenceFiles)
                        {
                            var path = $"wwwroot/docs/incidence-docs";
                            string filePath = Path.Combine(_env.ContentRootPath, path);

                            //create folder if not exist
                            if (!Directory.Exists(path))
                                Directory.CreateDirectory(path);

                            filePath = Path.Combine(filePath, file.FileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                file.CopyTo(stream);
                            }
                            attached = string.IsNullOrEmpty(attached) ? file.FileName : $"{attached},{file.FileName}";
                        }
                    }
                }

                inc.attachments = attached;

                if (id == 0)
                    await _context.Incidence.AddAsync(inc);
                else
                    _context.Incidence.Update(inc);

                int n = await _context.SaveChangesAsync();
                success = (n > 0) ? true : false;
                if (success)
                    incidentId = inc.id;

                if (success)
                {
                    var sp = _context.Shipment.Where(x => x.shipmentNo.Equals(inc.shipmentNo)).FirstOrDefault();
                    sp.status = (inc.type.Equals("breakdown")) ? shipmentStatus.BREAKDOWN.ToString()
                        : shipmentStatus.ACCIDENT.ToString();

                    sp.incidanceDate = inc.incidenceDate;
                    sp.updatedDate = inc.updatedDate;
                    sp.updatedBy = inc.updatedBy;

                    _context.Shipment.Update(sp);
                    n = await _context.SaveChangesAsync();
                    success = (n > 0) ? true : false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return Tuple.Create(success, incidentId);
        }

        public async Task<Tuple<bool, Int64>> AuthorityUpdate(Int64 id, Authority auth)
        {
            Int64 authorityId = 0;
            bool success = false;
            string attached = string.Empty;
            try
            {
                if (id == 0)
                    await _context.tbl_authority.AddAsync(auth);
                else
                    _context.tbl_authority.Update(auth);

                int n = await _context.SaveChangesAsync();
                success = (n > 0) ? true : false;
                if (success)
                    authorityId = auth.Id;

            }
            catch (Exception)
            {
                throw;
            }
            return Tuple.Create(success, authorityId);
        }

        public async Task<bool> DeleteIncidence(long id, string user)
        {
            bool success = false;
            try
            {
                var d = await _context.Incidence.FindAsync(id);
                if (d != null)
                {
                    d.updatedBy = user;
                    d.updatedDate = DateTime.Now;
                    d.status = comonStatus.deleted.ToString();
                    _context.Incidence.Update(d);
                }

                int n = await _context.SaveChangesAsync();

                success = (n > 0) ? true : false;
            }
            catch (Exception)
            {
            }
            return success;
        }

        public async Task<IncidenceReport> GetIncedentReport(long id)
        {
            IncidenceReport incident = null;
            try
            {
                var inc = await _context.Incidence.FindAsync(id);
                if (inc != null)
                {
                    var sp = await _context.Shipment.FindAsync(inc.shipmentId);
                    if (sp != null)
                    {
                        var dl = await _context.Destination.FindAsync(sp.detsinationId);
                        var vc = await _context.VcMaster.Where(x => x.vcNo.Equals(sp.vcNo)).FirstOrDefaultAsync();
                        var mr = await _context.Marching.Where(x => x.shipmentId.Equals(inc.shipmentId)).FirstOrDefaultAsync();
                        var dr = await _context.Driver.FindAsync(mr.driverId);

                        incident = new IncidenceReport()
                        {
                            id = inc.id,
                            //shipmentNo =inc.shipmentNo,
                            type = inc.type,
                            vcNo = vc.vcNo,
                            destination = dl.destination,
                            modelDesc = vc.modelDesc,
                            driverNameandNo = $"{dr.driverName} {dr.mobileNo}",
                            tempRegNo = sp.tempRegNo,
                            engineNo = inc.engineNo,
                            incidenceDate = inc.incidenceDate.Value,
                            incidencePlace = inc.incidencePlace,
                            depositDate = inc.depositDate.Value,
                            incidenceNature = inc.incidenceNature,
                            immediateAction = inc.immediateAction,
                            panchnamaHeld = inc.panchnamaHeld,
                            driverReleased = inc.driverReleased,
                            thirdPartyInvolved = inc.thirdPartyInvolved,
                            vcHandedOver = inc.vcHandedOver,
                            pcStationName = inc.pcStationName,
                            ChassisReleased = inc.ChassisReleased,
                            insSurveyDone = inc.insSurveyDone,
                            nearestDealer = inc.nearestDealer,
                            complaintNo = inc.complaintNo,
                            invoiceNo = sp.invoiceNo,
                            invoiceDate = sp.invoiceDate,
                            dlNo = dr.dlNo,
                            chassisNo = sp.chasisNo
                            //attachments =inc.attachments
                        };
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return incident;
        }
        public async Task<Authority> GetAuthorityReport(long id)
        {
            Authority auth = new Authority();
            try
            {
                auth = await _context.tbl_authority.FindAsync(id);
                if (auth != null)
                {
                    var sp = await _context.Shipment.Where(x => x.shipmentId.Equals(auth.shipmentId)).FirstOrDefaultAsync();
                    if (sp != null)
                    {

                        auth.modelDesc = sp.modelDesc;
                        auth.chasisNo = sp.chasisNo;
                        auth.location = sp.location;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return auth;
        }

        public async Task<List<Authority>> GetAuthorityList()
        {
            List<Authority> authority = new List<Authority>();
            try
            {
                var authList = await _context.tbl_authority.OrderByDescending(o => o.Id).Take(100).ToListAsync();
                if (authList != null)
                {
                    List<Int64> spIds = authList.Select(x => x.shipmentId).ToList();
                    var spList = await _context.Shipment.Where(x => spIds.Contains(x.shipmentId)).ToListAsync();
                    if (spList != null)
                    {
                        authority = (from auth in authList
                                     join sp in spList on auth.shipmentId equals sp.shipmentId
                                     select new Authority()
                                     {
                                         Id = auth.Id,
                                         RName = auth.RName,
                                         licenceNo = auth.licenceNo,
                                         authorityDate = auth.authorityDate,
                                         modelDesc = sp.modelDesc,
                                         chasisNo = sp.chasisNo,
                                         location = sp.location

                                     }).ToList();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return authority;
        }
    }
}
