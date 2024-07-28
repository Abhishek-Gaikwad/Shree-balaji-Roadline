using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2013.Word;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;
using Trans9.DataAccess;
using Trans9.Models;
using Trans9.Utility;

namespace Trans9.BLL
{
    public class PaymentBll
    {
        private readonly DataDbContext _context;
        private readonly IHostEnvironment _env;
        private readonly MarchingBll _mcontext;
        private readonly StoredProcedureDbContext _spcontext;
        public PaymentBll(DataDbContext context, StoredProcedureDbContext spcontext, IHostEnvironment env)
        {
            _context = context;
            _spcontext = spcontext;
            _mcontext = new MarchingBll(context, spcontext, env);
            _env = env;
        }

        public async Task<List<InchargePayment>> GetInchargeVoucherList(bool limit = true)
        {
            List<InchargePayment> list = new List<InchargePayment>();
            try
            {
                var vcList = await _context.InchargePayment.OrderByDescending(x => x.createdDate).ToListAsync();

                if (vcList.Any())
                {
                    var dIds = vcList.DistinctBy(x => x.inchargeId).Select(x => x.inchargeId).ToList();
                    var dList = await _context.Driver.Where(x => dIds.Contains(x.driverId)).ToListAsync();
                    if (limit)
                    {
                        list = (from v in vcList
                                join d in dList on v.inchargeId equals d.driverId
                                select new InchargePayment()
                                {
                                    voucherId = v.voucherId,
                                    voucherDate = v.voucherDate.Date,
                                    inchargeId = v.inchargeId,
                                    inchargeName = d.driverName,
                                    referenceNo = v.referenceNo,
                                    receivedAmount = v.receivedAmount,
                                    paidAmount = v.paidAmount,
                                    balanceAmount = v.balanceAmount,
                                    remark = v.remark
                                }).OrderByDescending(o => o.updatedDate).Take(100).ToList();
                    }
                    else
                    {
                        list = (from v in vcList
                                join d in dList on v.inchargeId equals d.driverId
                                select new InchargePayment()
                                {
                                    voucherId = v.voucherId,
                                    voucherDate = v.voucherDate.Date,
                                    inchargeId = v.inchargeId,
                                    inchargeName = d.driverName,
                                    referenceNo = v.referenceNo,
                                    receivedAmount = v.receivedAmount,
                                    paidAmount = v.paidAmount,
                                    balanceAmount = v.balanceAmount,
                                    remark = v.remark
                                }).OrderByDescending(o => o.updatedDate).ToList();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return list;
        }

        public async Task<List<Voucher>> GetVoucherList(bool limit = true)
        {
            List<Voucher> list = new List<Voucher>();
            try
            {
                var vcList = await _context.Marching.OrderByDescending(x => x.createdDate).ToListAsync();

                if (vcList.Any())
                {
                    var spNos = vcList.DistinctBy(x => x.shipmentId).Select(x => x.shipmentId).ToList();
                    var dIds = vcList.Select(x => x.driverId).ToList();
                    var sList = await _context.Shipment.Where(x => spNos.Contains(x.shipmentId)).ToListAsync();
                    var dList = await _context.Driver.Where(x => dIds.Contains(x.driverId)).ToListAsync();
                    if (limit)
                    {
                        list = (from v in vcList
                                join s in sList on v.shipmentId equals s.shipmentId
                                join d in dList on v.driverId equals d.driverId
                                select new Voucher()
                                {
                                    voucherNo = v.voucherNo,
                                    voucherDate = v.createdDate,
                                    amount = v.totalExp,
                                    driverName = d.driverName,
                                    inchargeName = v.driverIncharge,
                                    balance = v.remainBalance,
                                    location = s.location,
                                    id = v.voucherNo
                                }).OrderByDescending(o => o.updatedDate).Take(100).ToList();
                    }
                    else
                    {
                        list = (from v in vcList
                                join s in sList on v.shipmentId equals s.shipmentId
                                join d in dList on v.driverId equals d.driverId
                                select new Voucher()
                                {
                                    voucherNo = v.voucherNo,
                                    voucherDate = v.createdDate,
                                    amount = v.totalExp,
                                    driverName = d.driverName,
                                    inchargeName = v.driverIncharge,
                                    balance = v.remainBalance,
                                    location = s.location,
                                    id = v.voucherNo
                                }).OrderByDescending(o => o.updatedDate).ToList();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return list;
        }

        public async Task<Voucher> GetVoucherById(Int64 id = 0)
        {
            DateTime dt = DateTime.Now;
            Voucher vc = new Voucher();
            try
            {
                vc = await _context.Voucher.FindAsync(id);

                if (vc == null)
                {
                    vc = new Voucher()
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

        public async Task<Tuple<bool, Int64>> AddOrUpdateVoucher(int id, Voucher vc)
        {
            bool success = false;
            try
            {
                vc.remark = $"Paid agaist voucher No #{vc.voucherNo}";
                if (id == 0)
                {
                    vc.status = payment.paid.ToString();
                    _context.Add(vc);
                }
                else
                {
                    vc.status = payment.paid.ToString();
                    _context.Update(vc);
                }
                int n = await _context.SaveChangesAsync();
                success = (n > 0) ? true : false;

                if (success)
                {
                    Marching m = await _mcontext.GetMarchingByVoucherNo(vc.voucherNo);
                    m.remainBalance = (m.remainBalance - vc.amount);
                    m.updatedBy = vc.updatedBy;
                    m.updatedDate = vc.updatedDate;
                    success = await _mcontext.UpdateMarching(m);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return Tuple.Create(success, vc.id);
        }
        private bool VoucherExists(Int64 id = 0)
        {
            return _context.Voucher.Any(e => e.id == id);
        }

        public async Task<List<Payment>> GetVoucherListByVoucherNo(Int64 shipmentNo)
        {
            List<Payment> vclist = new List<Payment>();
            try
            {
                var list = await _context.Voucher.Where(v => v.voucherNo == shipmentNo).ToListAsync();
                if (list.Any())
                {
                    vclist = (from s in list
                              where s.status.Equals(payment.paid.ToString())
                              select new Payment
                              {
                                  id = s.receiptNo,
                                  voucherNo = s.voucherNo,
                                  voucherDate = s.voucherDate,
                                  amount = s.amount,
                                  payMode = s.payMode,
                                  payPercentage = s.payPercentage,
                                  remark = s.remark
                              }).OrderByDescending(o => o.voucherDate).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return vclist;
        }

        public async Task<bool> DeleteVoucher(Int64 id, string user)
        {
            DateTime dt = DateTime.Now;
            bool success = false;
            try
            {
                var vc = await _context.Voucher.FindAsync(id);
                if (vc != null)
                {
                    vc.status = payment.deleted.ToString();
                    vc.updatedDate = dt;
                    vc.updatedBy = user;
                    _context.Voucher.Update(vc);
                }

                int n = await _context.SaveChangesAsync();
                success = (n > 0) ? true : false;

                if (success)
                {
                    Marching m = await _mcontext.GetMarchingByVoucherNo(vc.voucherNo);
                    m.remainBalance = (m.remainBalance + vc.amount);
                    m.updatedDate = dt;
                    m.updatedBy = user;
                    success = await _mcontext.UpdateMarching(m);
                }
            }
            catch (Exception)
            {
            }
            return success;
        }

        public async Task<Receipt> GetVoucherInfo(long id)
        {
            Receipt receipt = new Receipt();
            try
            {
                var rc = await _context.Voucher.FindAsync(id);
                if (rc != null)
                {
                    var vc = await _context.Marching.FindAsync(rc.voucherNo);
                    if (vc != null)
                    {
                        var dr = await _context.Driver.FindAsync(vc.driverId);
                        receipt = new Receipt()
                        {
                            receiptNo = rc.id,
                            paidTo = dr.driverName,
                            amountInWord = GenericFunctions.NumberToWord(rc.amount),
                            payMode = rc.payMode,
                            amount = rc.amount,
                            remark = rc.remark,
                            voucherDate = rc.voucherDate.Value.Date
                        };
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return receipt;
        }

        public async Task<InchargePayment> GetPayableByVoucherNo(long id)
        {
            InchargePayment receipt = new InchargePayment();
            try
            {
                var rc = _context.InchargePayment.Where(x => x.inchargeId == id).OrderByDescending(x => x.voucherId).FirstOrDefault();
                if (rc != null)
                {
                    receipt = rc;
                }
                else
                {
                    receipt = new InchargePayment()
                    {

                    };
                }
            }
            catch (Exception)
            {
                throw;
            }

            return receipt;
        }


        public async Task<List<DieselPayment>> GetDieselPaymentVouchers(bool isIndex = false, long Id = 0)
        {
            List<DieselPayment> list = new List<DieselPayment>();
            try
            {
                var dpList = await _context.DieselPayment.OrderByDescending(x => x.createdDate).ToListAsync();
                if (dpList != null)
                {
                    if (dpList.Any())
                    {
                        var pList = await _context.PumpMaster.ToListAsync();
                        if (isIndex)
                        {
                            var groupByPump = dpList.GroupBy(p => p.pumpId)
                            .Select(g => g.MaxBy(x => x.voucherId)).ToList();

                            if (groupByPump.Any())
                            {

                                list = (from p in pList
                                        join g in groupByPump
                                        on p.pumpId equals g.pumpId into temp
                                        from g in temp.DefaultIfEmpty()
                                        select new DieselPayment
                                        {
                                            pumpName = p.pumpName,
                                            voucherId = g != null ? g.voucherId : 0,
                                            voucherDate = g != null ? g.voucherDate : DateTime.Now,
                                            balanceAmount = g != null ? g.balanceAmount : 0,
                                            currentAmount = g != null ? g.currentAmount : 0,
                                            prevBalance = g != null ? g.prevBalance : 0,
                                            payableAmount = g != null ? g.payableAmount : 0,
                                            paidAmount = g != null ? g.paidAmount : 0,
                                            pumpId = g != null ? g.pumpId : p.pumpId,
                                            remark = g != null ? g.remark : string.Empty,
                                            spdAmount = g != null ? g.spdAmount : 0,
                                            spdQty = g != null ? g.spdQty : 0,
                                            spdRate = g != null ? g.spdRate : 0,
                                            vouchers = g != null ? g.vouchers : string.Empty,
                                            createdDate = g != null ? g.createdDate : DateTime.Now,
                                            createdBy = g != null ? g.createdBy : string.Empty,
                                            updatedBy = g != null ? g.updatedBy : string.Empty,
                                            updatedDate = g != null ? g.updatedDate : DateTime.Now
                                        }).ToList();
                            }
                        }
                        else
                        {
                            dpList = Id == 0 ? dpList : dpList.Where(x => x.pumpId == Id).ToList();
                            foreach (var dp in dpList)
                            {
                                dp.pumpName = pList.Where(x => x.pumpId == dp.pumpId).Select(x => x.pumpName).FirstOrDefault();
                            }

                            list = dpList;
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return list;
        }

        public async Task<List<Diesel>> GetDieselReport(DieselRequestModel<Diesel> model)
        {
            List<Marching> mList = new List<Marching>();
            List<Diesel> dList = new List<Diesel>();
            DateTime fromDate, toDate;
            try
            {
                fromDate = model.fromDate.Value.Date;
                toDate = model.toDate.Value.Date;

                if (model.pumpId != 0)
                {
                    mList = await _context.Marching.Where(x => (x.createdDate.Value.Date >= fromDate
                                            && x.createdDate.Value.Date <= toDate) && x.pumpName == model.pumpId && x.spdPaid == 0).ToListAsync();
                }

                if (mList.Any())
                {
                    var pIds = mList.Select(x => x.pumpName).ToList();
                    var pumpList = await _context.PumpMaster.Where(x => pIds.Contains(x.pumpId)).ToListAsync();

                    dList = (from m in mList
                             join p in pumpList on m.pumpName equals p.pumpId
                             select new Diesel()
                             {
                                 voucherNo = m.voucherNo,
                                 voucherDate = m.createdDate.Value,
                                 shipmentNo = m.shipmentNo,
                                 pumpName = p.pumpName,
                                 location = p.location,
                                 receiptNo = m.receiptNo,
                                 rate = m.spdRate,
                                 qty = m.spdQty,
                                 amount = m.spdAmount
                             }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return dList;
        }

        public async Task<DieselPaymentCalc> GetTotalDieselPayment(CalculateDiesel data)
        {
            DieselPaymentCalc calc = new DieselPaymentCalc();
            try
            {
                DieselPayment dp = _context.DieselPayment.Where(x => x.pumpId == data.pumpId).OrderByDescending(x => x.voucherId).FirstOrDefault();
                List<Marching> ml = await _context.Marching.Where(x => data.vouchers.Contains(x.voucherNo)).ToListAsync();
                if (dp == null)
                {
                    dp = new DieselPayment()
                    {
                        balanceAmount = 0
                    };
                }
                if (ml != null)
                {
                    if (ml.Any())
                    {
                        decimal amount = ml.Sum(x => x.spdAmount);
                        decimal qty = ml.Sum(x => x.spdQty);
                        decimal rate = ml.Average(x => x.spdRate);

                        calc = new DieselPaymentCalc()
                        {
                            prevBalance = dp.balanceAmount,
                            currentAmount = amount,
                            payableAmount = dp.balanceAmount + amount,
                            paidAmount = 0,
                            balanceAmount = dp.balanceAmount + amount,
                            spdQty = qty,
                            spdRate = rate,
                            spdAmount = amount
                        };
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return calc;
        }

        public async Task<bool> CreateOrEditDieselBilllPayment(DieselRequestModel<Diesel> data, string user)
        {
            bool success = false;
            try
            {
                DateTime date = DateTime.Now;
                DieselPayment dp = new DieselPayment()
                {
                    voucherDate = data.voucherDate.Value.Date,
                    pumpId = data.pumpId,
                    //remark = data.,
                    prevBalance = data.prevBalance.HasValue ? data.prevBalance.Value : 0,
                    currentAmount = data.currentAmount.HasValue ? data.currentAmount.Value : 0,
                    payableAmount = data.payableAmount.HasValue ? data.payableAmount.Value : 0,
                    paidAmount = data.paidAmount.HasValue ? data.paidAmount.Value : 0,
                    balanceAmount = data.balanceAmount.HasValue ? data.balanceAmount.Value : 0,
                    spdQty = data.spdQty,
                    spdRate = data.spdRate,
                    spdAmount = data.spdAmount,
                    vouchers = data.vouchers,
                    createdBy = user,
                    updatedBy = user,
                    createdDate = date,
                    updatedDate = date,
                };

                var qs = await _spcontext.QueryResult
                     .FromSqlRaw("CALL `sbl_UpdateDieselPaymentVouchers`({0})",
                     data.vouchers)
                     .IgnoreQueryFilters()
                     .ToListAsync();

                QueryResult result = qs.Any() ? qs.FirstOrDefault() : new QueryResult() { errorCode = -1, Message = "Something Went Wrong.." };

                success = (result.errorCode == 1) ? true : false;

                if (success)
                {
                    _context.DieselPayment.Add(dp);
                    int n = await _context.SaveChangesAsync();
                    success = (n > 0) ? true : false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return success;
        }

        public async Task<bool> CreateDieselPaymentVoucher(DieselPayment data, string user)
        {
            bool success = false;
            try
            {
                DateTime date = DateTime.Now;
                DieselPayment dp = new DieselPayment()
                {
                    voucherDate = data.voucherDate,
                    pumpId = data.pumpId,
                    remark = data.remark,
                    prevBalance = data.prevBalance,
                    currentAmount = data.currentAmount,
                    payableAmount = data.payableAmount,
                    paidAmount = data.paidAmount,
                    balanceAmount = data.balanceAmount,
                    spdQty = data.spdQty,
                    spdRate = data.spdRate,
                    spdAmount = data.spdAmount,
                    vouchers = data.vouchers,
                    createdBy = user,
                    updatedBy = user,
                    createdDate = date,
                    updatedDate = date,
                };

                _context.DieselPayment.Add(dp);
                int n = await _context.SaveChangesAsync();
                success = (n > 0) ? true : false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return success;
        }

        public async Task<DieselPayment> GetDieselVoucherById(long id)
        {
            DieselPayment dp = new DieselPayment();
            try
            {
                var vc = _context.DieselPayment.Where(x => x.voucherId == id).FirstOrDefault();
                if (vc != null)
                {
                    dp.prevBalance = vc.balanceAmount;
                    dp.currentAmount = 0;
                    dp.remark = string.Empty;
                    dp.vouchers = string.Empty;
                    dp.balanceAmount = vc.balanceAmount;
                    dp.paidAmount = 0;
                    dp.payableAmount = vc.balanceAmount;
                    dp.spdAmount = 0;
                    dp.spdQty = 0;
                    dp.spdRate = 0;
                    dp.pumpId = vc.pumpId;
                    dp.voucherDate = DateTime.Now.Date;
                    dp.pumpName = _context.PumpMaster.Where(x => x.pumpId == vc.pumpId).Select(x => x.pumpName).FirstOrDefault();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return dp;
        }

        public List<DieselPayment> GetDieselVoucherByPumpId(long pumpId, out string pumpName)
        {
            pumpName = "";
            List<DieselPayment> list = new List<DieselPayment>();
            try
            {
                var data = GetDieselPaymentVouchers(false, pumpId).Result;
                if (data != null)
                {
                    if (data.Any())
                    {
                        pumpName = data.Select(x => x.pumpName).FirstOrDefault();
                        list = data;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return list;
        }

        public async Task<Tuple<bool, Int64>> AddOrUpdateInchargeVoucher(InchargePayment vc)
        {
            bool success = false;
            try
            {
                vc.remark = $"Paid by #{vc.payMode}";
                vc.receivedAmount = 0;
                vc.referenceNo = 0;

                _context.InchargePayment.Add(vc);

                int n = await _context.SaveChangesAsync();
                success = (n > 0) ? true : false;

            }
            catch (Exception)
            {
                throw;
            }
            return Tuple.Create(success, vc.voucherId);
        }

        public async Task<Receipt> GetInchargeVoucherInfo(long id)
        {
            Receipt receipt = new Receipt();
            try
            {
                var rc = await _context.InchargePayment.FindAsync(id);
                if (rc != null)
                {
                    var dr = await _context.Driver.FindAsync(rc.inchargeId);
                    receipt = new Receipt()
                    {
                        receiptNo = rc.voucherId,
                        paidTo = dr.driverName,
                        amountInWord = GenericFunctions.NumberToWord(rc.paidAmount),
                        payMode = rc.payMode,
                        amount = rc.paidAmount,
                        remark = rc.remark,
                        voucherDate = rc.voucherDate.Date
                    };
                }
            }
            catch (Exception)
            {
                throw;
            }

            return receipt;
        }
    }
}
