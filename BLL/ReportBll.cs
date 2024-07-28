using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics.Metrics;
using Trans9.DataAccess;
using Trans9.Models;

namespace Trans9.BLL
{
    public class ReportBll
    {
        private readonly DataDbContext _context;
        private readonly StoredProcedureDbContext _spDbContext;
        private readonly IHostEnvironment _env;
        private readonly ShipmentBll _spcontext;

        public ReportBll(DataDbContext context, StoredProcedureDbContext spDbContext, IHostEnvironment env = null)
        {
            _context = context;
            _spcontext = new ShipmentBll(context, spDbContext, env);
            _spDbContext = spDbContext;
            _env = env;
        }

        public async Task<List<BillingReport>> GetBillingList(ReportModel<BillingReport> model)
        {
            List<BillingReport> list = new List<BillingReport>();

            DateTime fromDate, toDate;
            try
            {
                fromDate = model.fromDate.Value.Date;
                toDate = model.toDate.Value.Date;

                list = await _spDbContext.BillingReport
                .FromSqlRaw("CALL sbl_billingReport({0},{1},{2},{3})", fromDate, toDate, model.destinationId, model.companyId)
                .ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }

            return list;
        }
        public async Task<List<BillingReport>> GetFBVBillingList(ReportModel<BillingReport> model)
        {
            List<BillingReport> list = new List<BillingReport>();

            DateTime fromDate, toDate;
            try
            {
                fromDate = model.fromDate.Value.Date;
                toDate = model.toDate.Value.Date;

                list = await _spDbContext.BillingReport
                .FromSqlRaw("CALL sbl_FBVbillingReport({0},{1},{2},{3})", fromDate, toDate, model.destinationId, model.companyId)
                .ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }

            return list;
        }

        public async Task<List<IncidenceReport>> GetIncidenceList(ReportModel<IncidenceReport> model)
        {
            List<IncidenceReport> incident = new List<IncidenceReport>();
            DateTime fromDate, toDate;
            try
            {
                List<Incidence> incList = new List<Incidence>();
                fromDate = model.fromDate.Value.Date;
                toDate = model.toDate.Value.Date;

                if (string.IsNullOrEmpty(model.status))
                {
                    incList = await _context.Incidence.Where(x => (x.incidenceDate.Value.Date >= fromDate
                                            && x.incidenceDate.Value.Date <= toDate)).ToListAsync();
                }
                else
                {
                    incList = await _context.Incidence.Where(x => x.type.Equals(model.status) && (x.incidenceDate.Value.Date >= fromDate
                                            && x.incidenceDate.Value.Date <= toDate)).ToListAsync();
                }

                if (incList != null)
                {
                    List<Int64> spIds = incList.Select(x => x.shipmentId).ToList();
                    var spList = await _context.Shipment.Where(x => spIds.Contains(x.shipmentId)).ToListAsync();
                    if (spList != null)
                    {
                        List<Int64> dIds = spList.Select(x => x.detsinationId).ToList();
                        List<string> vcNos = spList.Select(x => x.vcNo).ToList();

                        var dlList = await _context.Destination.Where(x => dIds.Contains(x.detsinationId)).ToListAsync();
                        var vcList = await _context.VcMaster.Where(x => vcNos.Contains(x.vcNo)).ToListAsync();
                        var mrList = await _context.Marching.Where(x => spIds.Contains(x.shipmentId)).ToListAsync();
                        List<Int64> drIds = mrList.Select(x => x.driverId).ToList();
                        var drList = await _context.Driver.Where(x => drIds.Contains(x.driverId)).ToListAsync();


                        incident = (from inc in incList
                                    join sp in spList on inc.shipmentId equals sp.shipmentId
                                    join dl in dlList on sp.detsinationId equals dl.detsinationId
                                    join vc in vcList on sp.vcNo equals vc.vcNo
                                    join mr in mrList on sp.shipmentId equals mr.shipmentId
                                    join dr in drList on mr.driverId equals dr.driverId
                                    select new IncidenceReport()
                                    {
                                        id = inc.id,
                                        shipmentNo = sp.shipmentNo,
                                        type = inc.type,
                                        vcNo = vc.vcNo,
                                        destination = dl.destination,
                                        modelDesc = vc.modelDesc,
                                        driverNameandNo = $"{dr.driverName} {dr.mobileNo}",
                                        tempRegNo = sp.tempRegNo,
                                        engineNo = inc.engineNo,
                                        incidenceDate = inc.incidenceDate,
                                        incidencePlace = inc.incidencePlace,
                                        depositDate = inc.depositDate,
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
                                    }).ToList();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return incident;
        }

        public async Task<List<ShipmentReport>> GetShipmentList(ReportModel<ShipmentReport> model)
        {
            List<ShipmentReport> list = new List<ShipmentReport>();

            DateTime fromDate, toDate;
            try
            {
                fromDate = model.fromDate.Value.Date;
                toDate = model.toDate.Value.Date;
                string status = model.statuses.Count > 0 ? string.Join(",", model.statuses) : "all";
                string destinations = model.ids.Count > 0 ? string.Join(",", model.ids) : "0";
                list = await _spDbContext.ShipmentReport
                //.FromSqlRaw("CALL sbl_shipmenReportV2({0},{1},{2},{3},{4})", fromDate, toDate, status, destinations, model.companyId)
                //.FromSqlRaw("CALL sbl_shipmenReport({0},{1},{2},{3},{4})", fromDate, toDate, status, destinations, model.companyId)
                .FromSqlRaw("CALL sbl_shipmenReportV1({0},{1},{2},{3},{4},{5})", fromDate, toDate, status, destinations, model.companyId, model.keyword)
                .ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }

            return list;
        }


        public async Task<List<Driver>> GetDriverList(ReportModel<Driver> model)
        {
            List<Driver> drList = new List<Driver>();
            DateTime fromDate, toDate;
            try
            {
                fromDate = model.fromDate.Value.Date;
                toDate = model.toDate.Value.Date;

                if (model.status.Equals("all"))
                {
                    drList = await _context.Driver.Where(x => (x.createdDate.Value.Date >= fromDate
                                        && x.createdDate.Value.Date <= toDate)).ToListAsync();
                }
                else
                {
                    drList = await _context.Driver.Where(x => (x.status.Equals(model.status)
                                      && (x.createdDate.Value.Date >= fromDate && x.createdDate.Value.Date <= toDate))).ToListAsync();
                }

            }
            catch (Exception)
            {
                throw;
            }

            return drList;
        }

        public async Task<List<Diesel>> GetDieselReport(ReportModel<Diesel> model)
        {
            List<Marching> mList = new List<Marching>();
            List<Diesel> dList = new List<Diesel>();
            DateTime fromDate, toDate;
            try
            {
                fromDate = model.fromDate.Value.Date;
                toDate = model.toDate.Value.Date;

                if (model.destinationId == 0)
                {
                    mList = await _context.Marching.Where(x => (x.createdDate.Value.Date >= fromDate
                                        && x.createdDate.Value.Date <= toDate)).ToListAsync();
                }
                else
                {
                    mList = await _context.Marching.Where(x => (x.createdDate.Value.Date >= fromDate
                                            && x.createdDate.Value.Date <= toDate) && x.pumpName == model.destinationId).ToListAsync();
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

        public async Task<List<ProfitLoss>> GetProfiLossReport(ReportModel<ProfitLoss> model)
        {
            List<ProfitLoss> list = new List<ProfitLoss>();
            DateTime fromDate, toDate;
            try
            {
                fromDate = model.fromDate.Value.Date;
                toDate = model.toDate.Value.Date;

                var sl = await _context.Shipment.Where(x => (x.shipmentDate.Date >= fromDate && x.shipmentDate.Date <= toDate)).ToListAsync();
                List<long> spIds = sl.Select(x => x.shipmentId).ToList();
                var ml = await _context.Marching.Where(x => spIds.Contains(x.shipmentId)).ToListAsync();
                var bl = await _context.Billing.Where(x => spIds.Contains(x.shipmentId)).ToListAsync();
                List<long> dlIds = sl.Select(x => x.detsinationId).ToList();
                var dl = await _context.Destination.Where(x => dlIds.Contains(x.detsinationId)).ToListAsync();

                list = (from s in sl
                        join b in bl on s.shipmentId equals b.shipmentId
                        join m in ml on s.shipmentId equals m.shipmentId
                        join d in dl on s.detsinationId equals d.detsinationId
                        select new ProfitLoss
                        {
                            shipmentId = s.shipmentId,
                            shipmentNo = s.shipmentNo,
                            modelDesc = s.modelDesc,
                            destination = d.destination,
                            vcNo = s.vcNo,
                            mfgCode = s.mfgCode,
                            routeCode = s.routeCode,
                            expense = (decimal)(m.totalExp + m.expenses + m.spdAmount),
                            freight = (b.payableAmount - b.tdsAmount - b.gstAmount)
                        }).ToList();


                if (list.Any())
                {
                    list = list.Select(x =>
                    {
                        x.plAmount = x.freight - x.expense;
                        x.plStatus = (x.plAmount > 0 ? "profit" : "loss");
                        return x;
                    }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return list;
        }

        public async Task<List<Voucher>> GetCashReport(ReportModel<Voucher> model)
        {
            List<Voucher> vcList = new List<Voucher>();
            List<Voucher> vouchers = new List<Voucher>();
            DateTime fromDate, toDate;
            try
            {
                fromDate = model.fromDate.Value.Date;
                toDate = model.toDate.Value.Date;
                if (model.status.Equals("all"))
                {
                    vcList = await _context.Voucher.Where(x => (x.createdDate.Value.Date >= fromDate
                                        && x.createdDate.Value.Date <= toDate)).ToListAsync();
                }
                else if (model.status.Equals("other"))
                {
                    List<string> status = new List<string>() { "cas", "cash", "upi" };
                    vcList = await _context.Voucher.Where(x => !status.Contains(x.payMode.ToLower()) && (x.createdDate.Value.Date >= fromDate
                                     && x.createdDate.Value.Date <= toDate)).ToListAsync();
                }
                else
                {
                    vcList = await _context.Voucher.Where(x => model.status.Contains(x.payMode.ToLower()) && (x.createdDate.Value.Date >= fromDate
                        && x.createdDate.Value.Date <= toDate)).ToListAsync();
                }
                if (vcList.Any())
                {
                    var spNos = vcList.Select(x => x.shipmentId).ToList();
                    var mList = await _context.Marching.Where(x => spNos.Contains(x.shipmentId)).ToListAsync();
                    var drIds = mList.Select(x => x.driverId).ToList();
                    var drList = await _context.Driver.Where(x => drIds.Contains(x.driverId)).ToListAsync();
                    vouchers = (from vc in vcList
                                join m in mList on vc.shipmentId equals m.shipmentId
                                join d in drList on m.driverId equals d.driverId
                                select new Voucher()
                                {
                                    id = vc.id,
                                    voucherDate = vc.createdDate,
                                    remark = vc.remark,
                                    driverName = d.driverName,
                                    voucherNo = vc.voucherNo,
                                    payMode = vc.payMode,
                                    amount = m.spdAmount
                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return vouchers;
        }

        public async Task<List<Authority>> GetAuthorityList(ReportModel<Authority> model)
        {
            List<Authority> list = new List<Authority>();
            DateTime fromDate, toDate;
            try
            {
                fromDate = model.fromDate.Value.Date;
                toDate = model.toDate.Value.Date;

                var al = await _context.tbl_authority.Where(x => (x.createdDate.Value.Date >= fromDate
                                        && x.createdDate.Value.Date <= toDate)).ToListAsync();

                if (al.Any())
                {
                    var spIds = al.Select(x => x.shipmentId).ToList();

                    var spList = await _context.Shipment.Where(x => spIds.Contains(x.shipmentId)).Select(x => new
                    {
                        x.shipmentId,
                        x.location,
                        x.chasisNo,
                        x.modelDesc
                    }).ToListAsync();

                    list = (from a in al
                            join sp in spList on a.shipmentId equals sp.shipmentId
                            select new Authority()
                            {
                                Id = a.Id,
                                RName = a.RName,
                                licenceNo = a.licenceNo,
                                authorityDate = a.authorityDate,
                                modelDesc = sp.modelDesc,
                                chasisNo = sp.chasisNo,
                                location = sp.location

                            }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return list;
        }

        public async Task<List<Voucher>> GetVoucherList(ReportModel<Voucher> model)
        {
            List<Voucher> list = new List<Voucher>();
            DateTime fromDate, toDate;
            try
            {
                fromDate = model.fromDate.Value.Date;
                toDate = model.toDate.Value.Date;

                var vcList = await _context.Marching.Where(x => (x.createdDate.Value.Date >= fromDate
                                        && x.createdDate.Value.Date <= toDate)).ToListAsync();

                if (vcList.Any())
                {
                    var spNos = vcList.DistinctBy(x => x.shipmentId).Select(x => x.shipmentId).ToList();
                    var dIds = vcList.Select(x => x.driverId).ToList();
                    var sList = await _context.Shipment.Where(x => spNos.Contains(x.shipmentId)).ToListAsync();
                    var dList = await _context.Driver.Where(x => dIds.Contains(x.driverId)).ToListAsync();

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
            catch (Exception)
            {
                throw;
            }

            return list;
        }

        public async Task<List<FreightReport>> GetFreightReport(ReportModel<FreightReport> model)
        {
            List<FreightReport> list = new List<FreightReport>();
            List<Shipment> sl = new List<Shipment>();
            DateTime fromDate, toDate;
            try
            {
                fromDate = model.fromDate.Value.Date;
                toDate = model.toDate.Value.Date;

                sl = await _context.Shipment.Where(x => (x.shipmentDate.Date >= fromDate && x.shipmentDate.Date <= toDate)).ToListAsync();

                var llIds = sl.Select(x => x.detsinationId).ToList();
                var ll = await _context.Destination.Where(x => llIds.Contains(x.detsinationId)).ToListAsync();
                list = (from s in sl
                        join l in ll on s.detsinationId equals l.detsinationId
                        select new FreightReport
                        {
                            shipmentNo = s.shipmentNo,
                            shipmentDate = s.shipmentDate,
                            routeCode = s.routeCode,
                            routeName = $"{s.plantDesc} - {l.destination}",
                            chassisNo = s.chasisNo,
                            vcNo = s.vcNo,
                            mfgCode = s.mfgCode,
                            plantCode = s.plantCode,
                            plantDesc = s.plantDesc,
                            basic = s.basicFreight,
                            enroute = s.enRoute
                        }).ToList();
            }
            catch (Exception)
            {
                throw;
            }

            return list;
        }

        public async Task<List<ExpenseReport>> GetExpenseReport(ReportModel<ExpenseReport> model)
        {
            List<ExpenseReport> list = new List<ExpenseReport>();
            List<Shipment> sl = new List<Shipment>();
            DateTime fromDate, toDate;
            try
            {
                fromDate = model.fromDate.Value.Date;
                toDate = model.toDate.Value.Date;

                //var ml = await _context.Marching.Where(x => spIds.Contains(x.shipmentId)).ToListAsync();
                var ml = await _context.Marching.Where(x => (x.createdDate.Value.Date >= fromDate && x.createdDate.Value.Date <= toDate)).ToListAsync();
                //sl = await _context.Shipment.Where(x => (x.shipmentDate.Date >= fromDate && x.shipmentDate.Date <= toDate)).ToListAsync();

                var spIds = ml.Select(x => x.shipmentId).ToList();
                sl = await _context.Shipment.Where(x => spIds.Contains(x.shipmentId)).ToListAsync();
                var dlIds = sl.Select(x => x.dealerId).ToList();
                var llIds = sl.Select(x => x.detsinationId).ToList();
                var dl = await _context.Dealer.Where(x => dlIds.Contains(x.dealerId)).ToListAsync();
                var ll = await _context.Destination.Where(x => llIds.Contains(x.detsinationId)).ToListAsync();

                list = (from s in sl
                        join m in ml on s.shipmentId equals m.shipmentId
                        join d in dl on s.dealerId equals d.dealerId
                        join l in ll on s.detsinationId equals l.detsinationId
                        select new ExpenseReport
                        {
                            shipmentNo = s.shipmentNo,
                            shipmentDate = s.shipmentDate,
                            chassisNo = s.chasisNo,
                            dealerName = d.dealerName,
                            destination = l.destination,
                            modelDesc = s.modelDesc,
                            totalHsd = m.totalHsd,
                            spotHsd = m.spdQty,
                            spotAmount = m.spdAmount,
                            inHandDiesal = (m.totalHsd - m.spdQty) * 90,
                            driverPayment = m.driverPayment,
                            enroute = m.inRouteExp,
                            fastagCharges = m.tollExp,
                            extraPayment = m.extraAmt,
                            paymentNaration = m.remark,
                            postDeliveryexp = m.expenses,
                            postExpNaration = m.narration,
                            totalExpense = (m.totalExp + m.spdAmount + m.expenses)
                        }).ToList();
            }
            catch (Exception)
            {
                throw;
            }

            return list;
        }

        public async Task<DriverPaymentReport> GetDriverPaymentReport(ReportModel<DriverPaymentReport> model)
        {
            List<DriverPayment> list = new List<DriverPayment>();
            List<Marching> mList = new List<Marching>();
            DriverPaymentReport report = new DriverPaymentReport();
            DateTime fromDate, toDate;
            try
            {
                fromDate = model.fromDate.Value.Date;
                toDate = model.toDate.Value.Date;
                if (model.destinationId > 0)
                {
                    report.noOfTrips = _context.Marching.Where(x => x.driverId == model.destinationId).Count();

                    mList = await _context.Marching.Where(x => (x.createdDate.Value.Date >= fromDate
                                                && x.createdDate.Value.Date <= toDate) && x.driverId == model.destinationId).ToListAsync();

                    var spIds = mList.Select(x => x.shipmentId).ToList();
                    var sl = await _context.Shipment.Where(x => spIds.Contains(x.shipmentId)).ToListAsync();
                    var llIds = sl.Select(x => x.detsinationId).ToList();
                    var ll = await _context.Destination.Where(x => llIds.Contains(x.detsinationId)).ToListAsync();

                    var rl = await _context.Voucher.Where(x => spIds.Contains(x.shipmentId)).ToListAsync();

                    list = (from m in mList
                            join s in sl on m.shipmentId equals s.shipmentId
                            join l in ll on s.detsinationId equals l.detsinationId
                            select new DriverPayment
                            {
                                inchargeName = m.driverIncharge,
                                tripDate = m.createdDate,
                                destination = l.destination,
                                voucherNo = m.voucherNo,
                                voucherAmount = m.totalExp,
                                receipts = rl.Where(x => x.voucherNo == m.voucherNo).Count(),
                                paidAmount = m.totalExp - m.remainBalance,
                                balanceAmount = m.remainBalance
                            }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
            report.payments = list;

            return report;
        }

        public async Task<List<LoadingChargesReport>> GetLoadingChargesReport(ReportModel<LoadingChargesReport> model)
        {
            List<LoadingChargesReport> list = new List<LoadingChargesReport>();
            DateTime fromDate, toDate;
            try
            {
                fromDate = model.fromDate.Value.Date;
                toDate = model.toDate.Value.Date;

                list = await _spDbContext.LoadingChargesReport
                .FromSqlRaw("CALL sbl_loadingChargesReport({0},{1})", fromDate, toDate)
                .ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }

            return list;
        }

        public async Task<List<PlantOut>> GetPlantOutridList()
        {
            List<string> status = new List<string>();
            List<PlantOut> list = new List<PlantOut>();
            try
            {
                status = new List<string> {
                                    shipmentStatus.APPROVALPENDING.ToString()
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
                    var poList = await _context.PlantOut.Where(x => spIds.Contains(x.shipmentId)).ToListAsync();

                    //GET Max ewayId record from plantout
                    var pList = poList.GroupBy(x => x.shipmentId)
                                    .Select(g => g.OrderByDescending(x => x.ewayId).FirstOrDefault()).ToList();


                    list = (from s in spList
                            join d in dlList on s.dealerId equals d.dealerId
                            join l in dsList on s.detsinationId equals l.detsinationId
                            join p in pList on s.shipmentId equals p.shipmentId
                            select new PlantOut()
                            {
                                ewayId = p.ewayId,
                                shipmentNo = s.shipmentNo,
                                shipmentDate = s.shipmentDate,
                                // dealerName = d.dealerName,
                                dest = l.destination,
                                //vcNo = s.vcNo,
                                modelDesc = s.modelDesc,
                                //  plantCode = s.plantCode,
                                // plantDesc = s.plantDesc,
                                chasisNo = s.chasisNo,
                                // invoiceNo = s.invoiceNo,
                                // invoiceDate = s.invoiceDate,
                                ewayNo = p.ewayNo,
                                expDate = p.expDate,
                                createdDate = p.createdDate,
                                //  deliveryDate = p.updatedDate.Value.AddDays(l.trasitDays),
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

        public async Task<List<MarchingGrid>> GetMarchingGridList()
        {
            List<string> status = new List<string>();
            List<MarchingGrid> list = new List<MarchingGrid>();
            try
            {
                status = new List<string> {
                                    shipmentStatus.APPROVALPENDING.ToString()
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
                                x.status.Equals(shipmentStatus.APPROVALPENDING.ToString())).ToListAsync();

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
                                deliveryDate = p.updatedDate.Value.AddDays(l.trasitDays),
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
        public async Task<bool> LoadingChargesUpdate(long id, string user)
        {
            bool success = false;
            try
            {
                var po = await _context.PlantOut.FindAsync(id);
                var sp = await _context.Shipment.FindAsync(po.shipmentId);
                if (po != null)
                {
                    po.status = shipmentStatus.IN_PLANT.ToString();
                    _context.Update(po);
                    int n = await _context.SaveChangesAsync();
                    success = n > 0 ? true : false;
                }
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

        public async Task<bool> LoadingChargesUpdate1(long id, string user)
        {
            bool success = false;
            try
            {
                var po = await _context.PlantOut.FindAsync(id);
                var sp = await _context.Shipment.FindAsync(po.shipmentId);
                if (po != null)
                {
                    po.status = shipmentStatus.IN_YARD.ToString();
                    _context.Update(po);
                }
                int n = await _context.SaveChangesAsync();
                success = (n > 0) ? true : false;
                if (success)
                {
                    sp.status = shipmentStatus.IN_YARD.ToString();
                    sp.updatedBy = user;
                    sp.updatedDate = DateTime.Now;

                    success = await _spcontext.UpdateShipment(sp);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return success;
        }
        public async Task<List<EwayReport>> GetEwayReport(ReportModel<EwayReport> model)
        {
            List<EwayReport> list = new List<EwayReport>();
            DateTime fromDate, toDate;
            try
            {
                fromDate = model.fromDate.Value.Date;
                toDate = model.toDate.Value.Date;

                var pl = await _context.PlantOut.Where(x => (!string.IsNullOrEmpty(x.ewayNo) && (x.createdDate.Date >= fromDate && x.createdDate.Date <= toDate))).ToListAsync();

                var spIds = pl.Select(x => x.shipmentId).ToList();
                var sl = await _context.Shipment.Where(x => spIds.Contains(x.shipmentId)).ToListAsync();

                list = (from s in sl
                        join p in pl on s.shipmentId equals p.shipmentId
                        select new EwayReport
                        {
                            chasisNo = s.chasisNo,
                            shipmentNo = s.shipmentNo,
                            ewayBillNo = p.ewayNo,
                            issueDate = p.createdDate,
                            expiryDate = p.expDate
                        }).ToList();
            }
            catch (Exception)
            {
                throw;
            }

            return list;
        }

        public async Task<List<EwayExpiryReport>> GetEwayExpiryList()
        {
            List<EwayExpiryReport> list = new List<EwayExpiryReport>();
            DateTime fromDate, toDate;
            try
            {
                fromDate = DateTime.Now;
                // toDate = DateTime.Now.AddDays(1);
                toDate = DateTime.Now;

                list = await _spDbContext.EwayExpiryReport
                .FromSqlRaw("CALL sbl_speWayExpiry({0},{1})", fromDate, toDate)
                .ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }

            return list;
        }
        public async Task<List<EstimatedDateReport>> GetEstimatedDateExpiryList()
        {
            List<EstimatedDateReport> list = new List<EstimatedDateReport>();
            EstimatedDateReport b = new EstimatedDateReport();
            DateTime fromDate, toDate;
            try
            {
                fromDate = DateTime.Now;
                // toDate = DateTime.Now.AddDays(1);
                toDate = DateTime.Now;

                list = await _spDbContext.EstimatedDateReport
                .FromSqlRaw("CALL sbl_spEstimatedDateReport({0},{1})", fromDate, toDate)
                .ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }

            return list;
        }
        public async Task<List<EstimatedDateReport>> GetEstimatedDateDelayExpiryList()
        {
            List<EstimatedDateReport> list = new List<EstimatedDateReport>();
            EstimatedDateReport b = new EstimatedDateReport();
            DateTime fromDate, toDate;
            try
            {
                fromDate = DateTime.Now;
                toDate = DateTime.Now;

                list = await _spDbContext.EstimatedDateReport
                    .FromSqlRaw("CALL sbl_spEstimatedDelayReport({0},{1})", fromDate, toDate)
                    .ToListAsync();

                if (list != null && list.Any())
                {
                    b = list.FirstOrDefault();
                    b.datediff = b.datediff;
                }
                else
                {
                    // Return an empty list if the result is null or empty
                    return list;
                }
            }
            catch (Exception)
            {
                throw; // You can handle or log the exception as needed
            }

            return list;
        }

    }
}
