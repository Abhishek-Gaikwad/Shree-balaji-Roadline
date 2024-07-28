using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Trans9.DataAccess;
using Trans9.Models;

namespace Trans9.BLL
{
    public class MarchingBll
    {
        private readonly DataDbContext _context;
        private readonly ShipmentBll _spcontext;
        private readonly DriverBll _drcontext;
        private readonly PlanOutBll _plcontext;
        private readonly IHostEnvironment _env;
        public MarchingBll(DataDbContext context, StoredProcedureDbContext spcontext, IHostEnvironment env)
        {
            _context = context;
            _spcontext = new ShipmentBll(context, spcontext, env);
            _drcontext = new DriverBll(context, env);
            _plcontext = new PlanOutBll(context, spcontext, env);
            _env = env;
        }

        public async Task<(Marching, string)> GetMarchingDetail(Int64 shipmentId)
        {
            DateTime dt = DateTime.Now;
            Marching m = new Marching();
            string message = "";
            try
            {
                var sl = await _context.Shipment.FindAsync(shipmentId);

                if (sl != null)
                {
                    m.shipmentId = sl.shipmentId;
                    m.shipmentNo = sl.shipmentNo;
                    m.chasisNo = sl.chasisNo;
                    m.model = sl.modelDesc;
                    m.tempRegNo = sl.tempRegNo;

                    var dl = await _context.Destination.FindAsync(sl.detsinationId);
                    var vl = await _context.VcMaster.Where(x => x.vcNo.Equals(sl.vcNo)).FirstOrDefaultAsync();
                    var pl = await _context.PlantOut.Where(x => x.shipmentId == shipmentId).OrderByDescending(x => x.ewayId).FirstOrDefaultAsync();
                    if (dl != null)
                    {
                        m.sblKm = dl.sblKms;
                        m.driverPayment = 0;
                        m.destination = dl.destination;
                        if (vl != null)
                        {
                            if (dl.sblKms > 0 && vl.sblKmpl > 0)
                                m.totalHsd = (dl.sblKms / vl.sblKmpl);
                            else
                            {
                                message += (dl.sblKms == 0) ? $"Please Update Destination: {dl.destination}. SBL Kms must be greater than 0\r\n" : "";
                                message += (vl.sblKmpl == 0) ? $"Please Update VC No: {vl.vcNo}. SBL Kmpl must be greater than 0" : "";
                            }
                        }

                        if (pl != null)
                        {
                            m.estimatedDate = pl.updatedDate.Value.AddDays(dl.trasitDays).ToString("dd-MM-yyyy");
                            m.plantOutDate = pl.updatedDate.Value.ToString("dd-MM-yyyy");
                        }
                    }
                    m.status = comonStatus.active.ToString();

                    m.createdDate = dt;
                    m.actualMarching = dt;
                    m.status = comonStatus.active.ToString();
                }
                m.updatedDate = dt;
            }
            catch (Exception)
            {
                throw;
            }
            return (m, message);
        }

        public async Task<Tuple<bool, Int64>> AddOrUpdateMarching(Marching mrc)
        {
            DateTime dt2 = DateTime.Now;
            string formattedDateTime1 = dt2.ToString("yyyy-MM-dd HH:mm:ss");
            DateTime dt3 = DateTime.ParseExact(formattedDateTime1, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            bool success = false;
            try
            {
                DateTime dt = new DateTime();

                if (mrc.driverId == 0)
                {
                    Driver d = new Driver()
                    {
                        driverName = mrc.driverName,
                        aadharNo = mrc.aadharNo,
                        dlNo = mrc.dlNo,
                        mobileNo = mrc.mobileNo,
                        licenseExpDate = mrc.licenseExpDate,
                        status = driverStatus.active.ToString(),
                        createdDate = dt,
                        updatedDate = dt,
                        aadharcard = mrc.aadharcard,
                        licensecard = mrc.licensecard,
                        photo = mrc.photo,
                        bankdetail = mrc.bankdetail,
                        cameraPhoto=mrc.cameraPhoto,
                        createdBy = mrc.createdBy,
                        updatedBy = mrc.updatedBy,
                    };

                    await _drcontext.AddOrUpdateDriver(mrc.driverId, d);
                }

                var dr = await _drcontext.GetDriverByAdhaarOrLc(mrc.aadharNo, mrc.dlNo);

                mrc.remainBalance = mrc.totalExp;
                mrc.driverId = dr.driverId;
                mrc.updatedDate = dt;
                mrc.status = comonStatus.active.ToString();

                if (mrc.voucherNo == 0)
                {
                    await _context.AddAsync(mrc);
                    mrc.actualMarching = dt3;
                }
                else
                {
                    _context.Update(mrc);
                }

                int n = await _context.SaveChangesAsync();
                success = (n > 0) ? true : false;

                if (success)
                {
                    var sp = await _context.Shipment.FindAsync(mrc.shipmentId);

                    sp.status = shipmentStatus.IN_TRANSIT.ToString();
                    sp.tempRegNo = mrc.tempRegNo;
                    sp.updatedDate = dt;
                    sp.updatedBy = mrc.updatedBy;

                    await _spcontext.UpdateShipment(sp);

                   success = await AddOrUpdateInchargeVoucher(new InchargePayment() { 
                        receivedAmount=mrc.totalExp,
                        inchargeId=mrc.inchargeId,
                        referenceNo=mrc.voucherNo
                    }, mrc.updatedBy);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return Tuple.Create(success, mrc.voucherNo);
        }

        private async Task<bool> AddOrUpdateInchargeVoucher(InchargePayment ip, string? user)
        {
            bool success = false;
            DateTime dt = DateTime.Now;
            try
            {
                var vc = _context.InchargePayment.Where(x => x.referenceNo == ip.referenceNo).FirstOrDefault();

                if (vc != null)
                {
                    decimal old = vc.receivedAmount;

                    vc.receivedAmount = ip.receivedAmount;
                    vc.balanceAmount = vc.balanceAmount - old + ip.receivedAmount;
                    vc.updatedDate = dt;
                    vc.updatedBy = user;

                    _context.InchargePayment.Update(vc);
                }
                else {
                    var balance = _context.InchargePayment.Where(x => x.inchargeId == ip.inchargeId).OrderByDescending(x=>x.voucherId).FirstOrDefault();

                    ip.balanceAmount += (balance == null) ? ip.receivedAmount : balance.balanceAmount+ ip.receivedAmount; 
                    ip.voucherDate = dt.Date;
                    ip.remark = $"Payment Received against ref no #: {ip.referenceNo}";

                    ip.createdBy = user;
                    ip.createdDate = dt;
                    ip.updatedBy = user;
                    ip.updatedDate = dt;

                    _context.InchargePayment.Add(ip);
                }

                int n = await _context.SaveChangesAsync();

                success = n > 0 ? true : false;
            }
            catch (Exception)
            {

                throw;
            }
            return success;
        }

        public async Task<bool> DeleteMarchingDetail(Int64 id, string user)
        {
            bool success = false;
            try
            {
                var dst = await _context.Marching.FindAsync(id);
                if (dst != null)
                {
                    dst.status = comonStatus.active.ToString();
                    dst.updatedDate = DateTime.Now;
                    dst.updatedBy = user;
                    _context.Marching.Update(dst);
                }

                int n = await _context.SaveChangesAsync();

                success = (n > 0) ? true : false;
            }
            catch (Exception)
            {
            }
            return success;
        }

        private bool MarchingExists(Int64 id = 0)
        {
            return _context.Marching.Any(e => e.voucherNo == id);
        }

        public async Task<List<Marching>> GetMarchingList(string action)
        {
            List<String> status = new List<string>();
            List<Marching> list = new List<Marching>();
            try
            {
                list = await _context.Marching.ToListAsync();
                if (list.Any())
                {
                    switch (action)
                    {
                        case "payment":
                            list = (from s in list
                                    where s.remainBalance > 0
                                    select s).ToList();
                            break;
                        case "billing":
                            list = (from s in list
                                    where s.remainBalance == 0
                                    select s).ToList();
                            break;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return list;
        }

        public async Task<List<MarchingGrid>> GetMarchingGridList()
        {
            List<string> status = new List<string>();
            List<MarchingGrid> list = new List<MarchingGrid>();
            try
            {
                status = new List<string> {
                                    shipmentStatus.IN_YARD.ToString()
                            };
                var spList = await _context.Shipment.Where(x => status.Contains(x.status)).ToListAsync();

                if (spList != null)
                {
                    var dlIds = spList.Select(x => x.dealerId).ToList();
                    var locIds = spList.Select(x => x.detsinationId).ToList();
                    //var vcNos = spList.Select(x => x.vcNo).ToList();
                    var spIds = spList.Select(x => x.shipmentId).ToList();

                    var dlList = await _context.Dealer.Where(x => dlIds.Contains(x.dealerId)).ToListAsync();
                    var dsList = await _context.Destination.Where(x => locIds.Contains(x.detsinationId)).ToListAsync();
                    //var vcList = await _context.VcMaster.Where(x => vcNos.Contains(x.vcNo)).ToListAsync();
                    var poList = await _context.PlantOut.Where(x => spIds.Contains(x.shipmentId) &&
                                x.status.Equals(shipmentStatus.IN_YARD.ToString())).ToListAsync();

                    //GET Max ewayId record from plantout
                    var pList = poList.GroupBy(x => x.shipmentId)
                                    .Select(g => g.OrderByDescending(x => x.ewayId).FirstOrDefault()).ToList();


                    list = (from s in spList
                            join d in dlList on s.dealerId equals d.dealerId
                            join l in dsList on s.detsinationId equals l.detsinationId
                            join p in pList on s.shipmentId equals p.shipmentId
                            select new MarchingGrid()
                            {
                                id = s.shipmentId,
                                shipmentNo = s.shipmentNo,
                                shipmentDate = s.shipmentDate,
                                dealerName = d.dealerName,
                                dest = l.destination,
                                vcNo = s.vcNo,
                                modelDesc = s.modelDesc,
                                plantCode = s.plantCode,
                                plantDesc = s.plantDesc,
                                chasisNo = s.chasisNo,
                                invoiceNo = s.invoiceNo,
                                invoiceDate = s.invoiceDate,
                                ewayNo = p.ewayNo,
                                expDate = p.expDate,
                                createdDate = p.createdDate,
                                deliveryDate = p.createdDate.AddDays(l.trasitDays),
                                status = s.status
                            }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return list;
        }


        public async Task<Marching> GetMarchingByVoucherNo(Int64 shipmentNo)
        {
            Marching m = new Marching();
            try
            {
                m = await _context.Marching.FindAsync(shipmentNo);
                if (m != null)
                {
                    var d = await _context.Driver.FindAsync(m.driverId);
                    m.driverName = d.driverName;

                    var inc = await _context.Driver.FindAsync(m.inchargeId);
                    m.driverIncharge = inc.driverName;

                    var sp = await _context.Shipment.FindAsync(m.shipmentId);
                    m.model = sp.modelDesc;

                    var des = await _context.Destination.FindAsync(sp.detsinationId);
                    m.destination = des.destination;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return m;
        }

        public async Task<bool> UpdateMarching(Marching m)
        {
            bool success = false;
            try
            {
                DateTime dt = new DateTime();
                m.updatedDate = dt;
                _context.Update(m);
                int n = await _context.SaveChangesAsync();
                success = (n > 0) ? true : false;
            }
            catch (Exception)
            {
                throw;
            }

            return success;
        }

        public async Task<bool> ShipmentPushback(long id, string user)
        {
            bool success = false;
            try
            {
                var sp = await _context.Shipment.FindAsync(id);
                if (sp != null)
                {
                    sp.status = shipmentStatus.IN_PLANT.ToString();
                    sp.updatedBy = user;
                    sp.updatedDate = DateTime.Now;

                    _context.Update(sp);
                    int n = await _context.SaveChangesAsync();
                    success = n > 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return success;
        }
        
    }
}
