using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json;
using System.Data;
using System.Linq;
using System.Reflection;
using Trans9.DataAccess;
using Trans9.Models;
using Trans9.Utility;

namespace Trans9.BLL
{
    public class BillingBll
    {
        private readonly DataDbContext _context;
        private readonly ShipmentBll _sc;
        private readonly StoredProcedureDbContext _spc;
        private readonly IHostEnvironment _env;
        private readonly IHttpContextAccessor _http;
        private string user = string.Empty;
        public BillingBll(DataDbContext context, StoredProcedureDbContext spcontext, IHttpContextAccessor http, IHostEnvironment env)
        {
            _context = context;
            _http = http;
            _sc = new ShipmentBll(context, spcontext, env);
            _spc = spcontext;
            _env = env;
        }

        #region "BILLING"
        public async Task<BillingModel> GetBillingById(Int64 id = 0)
        {
            DateTime dt = DateTime.Now;
            BillingModel b = new BillingModel();
            try
            {
                //b = await _context.Billing.FindAsync(id);

                /*if (b == null)
                {*/
                b = new BillingModel()
                { createdDate = dt, status = comonStatus.active.ToString() };
                /*}*/

                b.updatedDate = dt;
            }
            catch (Exception)
            {
                throw;
            }
            return b;
        }

        public async Task<Tuple<bool, string>> AddOrUpdateBilling(long id, BillingModel b)
        {
            bool success = false;
            BillNumer bs = new BillNumer();
            Int64 billgrp = 0;
            string fy = "", temp = "";
            try
            {
                //temp = "start before billgroup";
                billgrp = GenericFunctions.ToUnixTime();
                //temp = "start before serialNo";
                var serialList = GetBillingSerial(b.billingDate, out fy);

                //temp = "start before foreach";
                //int c = 1;
                foreach (var item in b.shipmentNos.Split(","))
                {
                    //temp = $"start before Find shipment {c}";
                    Shipment sp = await _context.Shipment.FindAsync(Int64.Parse(item));
                    if (sp != null)
                    {
                        //temp = $"start before generate Bill No {c}";
                        bs = serialList.Where(x => x.plantCode.ToString() == sp.plantCode).FirstOrDefault();
                        string billNo = $"SBL{fy}{bs.prefix}{(bs.serial <= 9 ? $"0{bs.serial}" : $"{bs.serial}")}";
                        //temp = $"start before Find bill ROw {c}";
                        var invoice = await _context.BillRows.Where(x => x.shipmentId == sp.shipmentId).FirstOrDefaultAsync();

                        //temp = $"start before Generating Billing Obj {c}";
                        Billing billing = new Billing()
                        {
                            billGrp = billgrp,
                            billNo = billNo,
                            shipmentId = sp.shipmentId,
                            shipmentNo = sp.shipmentNo,
                            billingDate = b.billingDate,
                            basicFreight = invoice.basicRate,
                            enRoute = invoice.inroute,
                            totalFreight = invoice.totalExp,
                            //epcAmount = invoice.discount,
                            taxableAmount = invoice.taxable,
                            gstAmount = Math.Round(invoice.taxable * (decimal)0.18, 2),
                            payableAmount = (invoice.taxable + Math.Round(invoice.taxable * (decimal)0.18, 2)),
                            createdBy = b.createdBy,
                            createdDate = b.createdDate,
                            updatedBy = b.updatedBy,
                            updatedDate = b.updatedDate,
                            status = b.status
                        };
                        //temp = $"start before Adding Billing Obj {c}";
                        await _context.AddAsync(billing);

                        //temp = $"start before Updating Shipment {c}";
                        sp.updatedBy = b.updatedBy;
                        //sp.updatedDate = b.updatedDate;  //skiped for reachdate
                        sp.status = shipmentStatus.BILLING_DONE.ToString();
                        _context.Update(sp);
                    }
                    //temp = $"start before Updating Serial No {c++}";
                    serialList.Remove(bs);
                    bs.serial += 1;
                    serialList.Add(bs);
                }
                //temp = "start before Saving all changes";
                int n = await _context.SaveChangesAsync();

                success = n > 0 ? true : false;

                if (success)
                {
                    //temp = "start before Updating Serial NO";
                    success = UpdateBillingSerial(serialList, fy);
                    temp = $"{billgrp}";
                }
            }
            catch (Exception ex)
            {
                //temp = $"{temp} || {ex.Message} || {ex.StackTrace}";
            }
            return new Tuple<bool, string>(success, temp);
        }

        private bool UpdateBillingSerial(List<BillNumer> serialList, string fy)
        {
            bool success = false;
            try
            {
                string filePath = Path.Combine(_env.ContentRootPath, $"wwwroot/dist/template/serial.json");

                string datastring = System.IO.File.ReadAllText(filePath);
                if (!string.IsNullOrEmpty(datastring))
                {
                    Dictionary<string, List<BillNumer>> data = JsonConvert.DeserializeObject<Dictionary<string, List<BillNumer>>>(datastring);
                    if (data != null)
                    {
                        if (data.ContainsKey(fy))
                        {
                            data[fy] = serialList;
                        }
                        else
                        {
                            data.Add(fy, serialList);
                        }
                    }
                    else
                    {
                        data = new Dictionary<string, List<BillNumer>>();
                        data.Add(fy, serialList);
                    }

                    System.IO.File.WriteAllText(filePath, JsonConvert.SerializeObject(data));

                    success = true;
                }

            }
            catch (Exception)
            {
                throw;
            }
            return success;
        }

        private List<BillNumer> GetBillingSerial(DateTime date, out string fy)
        {
            fy = string.Empty;
            List<BillNumer> bill = new List<BillNumer>();
            try
            {
                Int64 yr = Convert.ToInt64(date.ToString("yy"));
                var fyear = (date.Month >= 4) ? $"{yr}{yr + 1}"
                    : $"{yr - 1}{yr}";

                string filePath = Path.Combine(_env.ContentRootPath, $"wwwroot/dist/template/serial.json");

                string datastring = System.IO.File.ReadAllText(filePath);
                if (!string.IsNullOrEmpty(datastring))
                {
                    Dictionary<string, List<BillNumer>> data = JsonConvert.DeserializeObject<Dictionary<string, List<BillNumer>>>(datastring);
                    if (data != null)
                    {
                        bill = data.Where(x => x.Key == fyear).Select(x => x.Value).FirstOrDefault();
                    }
                }
                if (bill == null)
                {
                    bill = new List<BillNumer>();
                    bill.Add(new BillNumer()
                    {
                        plantCode = "1001",
                        serial = 1,
                        prefix = "CV"
                    });
                    bill.Add(new BillNumer()
                    {
                        plantCode = "4302",
                        serial = 1,
                        prefix = "FBV"
                    });
                    bill.Add(new BillNumer()
                    {
                        plantCode = "LKO FBV",
                        serial = 1,
                        prefix = "LKO"
                    });
                }
                fy = fyear;
            }
            catch (Exception)
            {
                throw;
            }
            return bill;
        }

         public async Task<List<Billing>> GetBillingList()
        {
            List<Billing> list = new List<Billing>();
            try
            {
                list = await _context.Billing.ToListAsync();

                list = list.Skip(Math.Max(0, list.Count() - 200)).ToList();
                var spIds = list.Select(x => x.shipmentId).ToList();
                var spList = await _context.Shipment.Where(x => spIds.Contains(x.shipmentId)).ToListAsync();
                if (list.Any())
                {
                    list = (from s in spList
                            join b in list on s.shipmentId equals b.shipmentId
                            where (s.cid == 1 && b.status.Equals(comonStatus.active.ToString()))
                            select b
                            ).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return list;
        }
        public async Task<bool> DeleteBilling(Int64 id, string user)
        {
            bool success = false;
            try
            {
                var b = await _context.Billing.FindAsync(id);
                if (b != null)
                {
                    b.updatedBy = user;
                    b.updatedDate = DateTime.Now;
                    b.status = comonStatus.deleted.ToString();
                    _context.Billing.Update(b);
                }

                int n = await _context.SaveChangesAsync();

                success = (n > 0) ? true : false;
            }
            catch (Exception)
            {
            }
            return success;
        }

        public async Task<List<Shipment>> GetShipmentsList()
        {
            List<Shipment> list = new List<Shipment>();
            try
            {
                list = await _sc.GetShipments(Actions.billing.ToString());
            }
            catch (Exception)
            {
                throw;
            }

            return list;
        }

        private bool BillingExists(Int64 id = 0)
        {
            return _context.Billing.Any(e => e.id == id);
        }

        public async Task<MemoryStream> GetBillingData(long id)
        {
            MemoryStream stream = new MemoryStream();
            List<BillModel> tbl = new List<BillModel>();
            try
            {
                List<Billing> bList = await _context.Billing.Where(x => x.billGrp == id).ToListAsync();
                if (bList.Any())
                {
                    List<Int64> shipmentNos = bList.Select(x => x.shipmentId).ToList();
                    var spList = await _context.Shipment.Where(x => shipmentNos.Contains(x.shipmentId)).ToListAsync();

                    tbl = (from s in spList
                           join b in bList on s.shipmentId equals b.shipmentId
                           select new BillModel()
                           {
                               billNo = b.billNo,
                               billDate = b.billingDate.Date,
                               shipmentNo = b.shipmentNo,
                               shipmentDate = s.shipmentDate.Date,
                               plantCode = s.plantCode,
                               invoiceNo = s.invoiceNo,
                               invoiceDate = s.invoiceDate,
                               basicFreight = b.basicFreight,
                               enroute = b.enRoute,
                               totalfreight = b.totalFreight,
                               //discountAmount = b.epcAmount,
                               taxebleValue = b.taxableAmount,
                               CGST = Math.Round(b.gstAmount / 2, 2),
                               SGST = Math.Round(b.gstAmount / 2, 2),
                               totalInvoiceAmount = Math.Round(b.taxableAmount + b.gstAmount, 2)
                           }).ToList();
                }
                if (tbl.Any())
                {

                    PropertyInfo[] properties = tbl.First().GetType().GetProperties();
                    List<string> headerNames = properties.Select(prop => prop.Name).ToList();

                    XLWorkbook wb = new XLWorkbook();

                    stream = GenericFunctions.GetExcelData(wb, tbl, "billing", headerNames, true);
                }
            }
            catch (Exception)
            {

                throw;
            }
            return stream;
        }

        public async Task<MemoryStream> GetBillingIrnData(Int64 referenceNo)
        {
            MemoryStream stream = new MemoryStream();
            List<IrnBilling> tbl = new List<IrnBilling>();
            try
            {
                List<Billing> bList = await _context.Billing.Where(x => x.billGrp == referenceNo).ToListAsync();

                if (bList.Any())
                {
                    tbl = (from b in bList
                           select new IrnBilling()
                           {
                               id = b.id,
                               billGrp = b.billGrp,
                               billNo = b.billNo,
                               shipmentId = b.shipmentId,
                               shipmentNo = b.shipmentNo,
                               billingDate = b.billingDate,
                               basicFreight = b.basicFreight,
                               enRoute = b.enRoute,
                               totalFreight = b.totalFreight,
                               //epcAmount = b.epcAmount,
                               taxableAmount = b.taxableAmount,
                               gstAmount = b.gstAmount,
                               tdsAmount = b.tdsAmount,
                               penaltyAmount = b.penaltyAmount,
                               payableAmount = b.payableAmount
                           }).ToList();
                }

                if (tbl.Any())
                {

                    PropertyInfo[] properties = tbl.First().GetType().GetProperties();
                    List<string> headerNames = properties.Select(prop => prop.Name).ToList();

                    XLWorkbook wb = new XLWorkbook();

                    stream = GenericFunctions.GetExcelData(wb, tbl, "billing", headerNames, true);
                }
            }
            catch (Exception)
            {

                throw;
            }
            return stream;
        }

        public async Task<MemoryStream> GetBillingIrnsData(Int64 referenceNo)
        {
            MemoryStream stream = new MemoryStream();
            List<IrnsBilling> tbl = new List<IrnsBilling>();
            try
            {
                List<Billing> bList = await _context.Billing.Where(x => x.billGrp == referenceNo).ToListAsync();

                if (bList.Any())
                {
                    tbl = (from b in bList
                           select new IrnsBilling()
                           {
                               id = b.id,
                               billGrp = b.billGrp,
                               billNo = b.billNo,
                               shipmentId = b.shipmentId,
                               shipmentNo = b.shipmentNo,
                               billingDate = b.billingDate,
                               basicFreight = b.basicFreight,
                               enRoute = b.enRoute,
                               totalFreight = b.totalFreight,
                               //epcAmount = b.epcAmount,
                               taxableAmount = b.taxableAmount,
                               gstAmount = b.gstAmount,
                               tdsAmount = b.tdsAmount,
                               penaltyAmount = b.penaltyAmount,
                               payableAmount = b.payableAmount
                           }).ToList();
                }

                if (tbl.Any())
                {

                    PropertyInfo[] properties = tbl.First().GetType().GetProperties();
                    List<string> headerNames = properties.Select(prop => prop.Name).ToList();

                    XLWorkbook wb = new XLWorkbook();

                    stream = GenericFunctions.GetExcelData(wb, tbl, "billing", headerNames, true);
                }
            }
            catch (Exception)
            {

                throw;
            }
            return stream;
        }


        public async Task<List<Billing>> GetBillReferenceList()
        {
            try
            {
                var list = await _context.Billing.ToListAsync();
                var spIds = list.Select(x => x.shipmentId).ToList();
                var spList = await _context.Shipment.Where(x => spIds.Contains(x.shipmentId)).ToListAsync();

                list = (from b in list
                        join s in spList on b.shipmentId equals s.shipmentId
                        where s.cid == 1
                        select b
                    ).ToList();

                if (list.Any())
                {
                    list = list.GroupBy(x => x.billGrp)
                  .Select(x => x.Select(y => y).FirstOrDefault()).ToList();
                }
                return list;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdateIRNReceivedAmount(ReportModel<Billing> model, string user)
        {
            int cnt = 0, n = 0;
            DateTime date = DateTime.Now;
            bool success = false;
            List<IrnBillingModel> list = new List<IrnBillingModel>();
            try
            {
                DataTable dt = GenericFunctions.ReadExcel(model.dataFile);
                if (dt.Rows.Count > 0)
                {
                    list = GenericFunctions.ConvertDataTable<IrnBillingModel>(dt);
                    if (list.Any())
                    {
                        foreach (var item in list)
                        {
                            Billing b = await _context.Billing.Where(x => x.id.ToString() == item.id).FirstOrDefaultAsync();
                            if (b != null)
                            {
                                b.receivedAmount = Convert.ToDecimal(item.receivedAmount);
                                b.tdsAmount = b.payableAmount - Convert.ToDecimal(item.receivedAmount);
                                b.updatedBy = user;
                                b.refno = item.refno;
                                b.cvno = item.cvno;
                                b.paymentdate = date;

                                _context.Update(b);

                                n += await _context.SaveChangesAsync();
                                cnt += 1;
                            }

                        }
                    }

                    success = (n > 0 | cnt > 0) ? true : false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return success;
        }

        public async Task<bool> UpdateIRN(ReportModel<Billing> model, string user)
        {
            int cnt = 0, n = 0;
            DateTime date = DateTime.Now;
            bool success = false;
            List<IrnModel> list = new List<IrnModel>();
            try
            {
                DataTable dt = GenericFunctions.ReadExcel(model.dataFile);
                if (dt.Rows.Count > 0)
                {
                    list = GenericFunctions.ConvertDataTable<IrnModel>(dt);
                    if (list.Any())
                    {
                        foreach (var item in list)
                        {
                            Billing b = await _context.Billing.Where(x => x.id.ToString() == item.id).FirstOrDefaultAsync();
                            if (b != null)
                            {
                                //b.receivedAmount = Convert.ToDecimal(item.receivedAmount);
                                b.irnNo = item.irnNo;
                                b.aknowledgeno = item.irnNo;
                                b.aknowledgedate = date;
                                //b.tdsAmount = (b.payableAmount - Convert.ToDecimal(item.receivedAmount));
                                //b.updatedBy = user;
                                //b.updatedDate = date;

                                _context.Update(b);

                                n = await _context.SaveChangesAsync();
                                cnt += 1;
                            }
                        }
                    }

                    success = (n > 0 | cnt > 0) ? true : false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return success;
        }

        #endregion "BILLING"

        #region "FBV BILLING"
        public async Task<List<FbvBilling>> GetFBVBillingList(ReportModel<FbvBilling> model)
        {
            List<FbvBilling> list = new List<FbvBilling>();
            DateTime fromDate, toDate;
            try
            {
                fromDate = model.fromDate.Value.Date;
                toDate = model.toDate.Value.Date;

                list = await _spc.FbvBilling
                .FromSqlRaw("CALL sbl_FbvShipmentListV1({0},{1})", fromDate, toDate)
                .ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }

            return list;
        }

        public async Task<FbvBillingModel> getFBVShipment(long shipmentId, long id)
        {
            FbvBillingModel fb = new FbvBillingModel();
            try
            {
                Shipment sp = await _context.Shipment.FindAsync(shipmentId);

                if (id != 0)
                {
                    var b = await _context.Billing.FindAsync(id);
                    if (b != null)
                    {
                        fb.billId = b.id;
                        fb.billNo = b.billNo;
                        fb.shipmentId = b.shipmentId;
                        fb.shipmentNo = b.shipmentNo;
                        fb.basicFreight = b.basicFreight;
                        fb.enRoute = b.enRoute;
                        fb.totalFreight = b.totalFreight;
                        fb.receivedAmount = b.receivedAmount;
                        fb.irnNo = b.irnNo;
                    }
                    else
                    {
                        fb.billId = id;
                        fb.billNo = "-";
                        fb.shipmentId = sp.shipmentId;
                        fb.shipmentNo = sp.shipmentNo;
                        fb.basicFreight = sp.basicFreight;
                        fb.enRoute = sp.enRoute;
                        fb.totalFreight = sp.totalFreight;
                        fb.receivedAmount = 0;
                    }
                }
                else
                {
                    fb.billId = id;
                    fb.billNo = "-";
                    fb.shipmentId = sp.shipmentId;
                    fb.shipmentNo = sp.shipmentNo;
                    fb.basicFreight = sp.basicFreight;
                    fb.enRoute = sp.enRoute;
                    fb.totalFreight = sp.totalFreight;
                    fb.receivedAmount = 0;
                }
                fb.billingDate = DateTime.Now.Date;
            }
            catch (Exception)
            {
                throw;
            }

            return fb;
        }

        public async Task<Tuple<bool, string>> AddOrUpdateFBVBilling(long id, long shipmentId, FbvBillingModel b, string user)
        {
            bool success = false;
            BillNumer bs = new BillNumer();
            Int64 billgrp = 0;
            DateTime dt = DateTime.Now;
            string fy = "", temp = "";
            try
            {
                if (b.podAtttachedment.Count > 0)
                {
                    var aadhar = GenericFunctions.UploadDocuments(_env, b.podAtttachedment, "fbv");
                    b.attachments = string.Join(",", aadhar);
                }

                if (id == 0)
                {
                    billgrp = GenericFunctions.ToUnixTime();

                    var serialList = GetBillingSerial(b.billingDate, out fy);

                    Shipment sp = await _context.Shipment.FindAsync(shipmentId);
                    if (sp != null)
                    {
                        bs = serialList.Where(x => x.plantCode.ToString() == sp.plantCode).FirstOrDefault();
                        string billNo = $"SBL{fy}{bs.prefix}{(bs.serial <= 9 ? $"0{bs.serial}" : $"{bs.serial}")}";

                        //decimal discount = Math.Round(b.basicFreight * (decimal)0.0108, 2);
                        decimal taxable = Math.Round(b.basicFreight  + b.enRoute, 2);
                        Billing billing = new Billing()
                        {
                            billGrp = billgrp,
                            billNo = billNo,
                            shipmentId = sp.shipmentId,
                            shipmentNo = sp.shipmentNo,
                            billingDate = b.billingDate,
                            basicFreight = b.basicFreight,
                            enRoute = b.enRoute,
                            totalFreight = b.totalFreight,
                            //epcAmount = discount,
                            taxableAmount = taxable,
                            gstAmount = Math.Round(taxable * (decimal)0.18, 2),
                            payableAmount = (taxable + Math.Round(taxable * (decimal)0.18, 2)),
                            createdBy = user,
                            createdDate = dt.Date,
                            updatedBy = user,
                            updatedDate = dt.Date,
                            irnNo = b.irnNo,
                            status = comonStatus.active.ToString(),
                            attachments = b.attachments
                        };

                        await _context.AddAsync(billing);

                        int n = await _context.SaveChangesAsync();
                        if (n > 0)
                        {
                            serialList.Remove(bs);
                            bs.serial += 1;
                            serialList.Add(bs);

                            temp = $"{billing.id}";
                        }
                        success = n > 0 ? true : false;
                    }

                    if (success)
                    {
                        success = UpdateBillingSerial(serialList, fy);
                    }
                }
                else
                {
                    var bill = await _context.Billing.FindAsync(id);

                    bill.receivedAmount = Convert.ToDecimal(b.receivedAmount);
                    bill.irnNo = b.irnNo;
                    bill.tdsAmount = bill.payableAmount - b.receivedAmount;
                    bill.updatedBy = user;
                    bill.updatedDate = dt.Date;

                    _context.Update(bill);
                    int n = await _context.SaveChangesAsync();

                    var sp = await _context.Shipment.FindAsync(shipmentId);
                    sp.updatedBy = user;
                    //sp.updatedDate = b.updatedDate;  //skiped for reachdate
                    sp.status = shipmentStatus.BILLING_DONE.ToString();
                    _context.Update(sp);

                    n = await _context.SaveChangesAsync();

                    if (n > 0)
                    {
                        temp = $"{bill.id}";
                        success = true;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return new Tuple<bool, string>(success, temp);
        }

        public async Task<FbvInvoice> GetFbvBillingInvoice(Int64 id)
        {
            FbvInvoice b = new FbvInvoice();
            try
            {
                var list = await _spc.FbvInvoice
                .FromSqlRaw("CALL sbl_PrintFblInvoice({0})", id)
                .ToListAsync();

                if (list != null)
                {
                    b = list.FirstOrDefault();
                    b.amountInWords = GetAmountInWords(b.totalFreight);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return b;
        }
        #endregion "FBV BILLING"

        #region "IB BILLING"
        public async Task<List<IbBilling>> GetIbBillingList(ReportModel<IbBilling> model)
        {
            List<IbBilling> list = new List<IbBilling>();
            DateTime fromDate, toDate;
            try
            {
                fromDate = model.fromDate.Value.Date;
                toDate = model.toDate.Value.Date;

                list = await _spc.IbBilling
                .FromSqlRaw("CALL sbl_IbBillingListV1({0},{1})", fromDate, toDate)
                .ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }

            return list;
        }

        public async Task<QueryResult> generateIbBilling(IbBillingModel ib)
        {

            QueryResult result = new QueryResult()
            {
                errorCode = 404,
                Message = "Something went wrong.."
            };
            try
            {
                if (ib.podAtttachedment.Count > 0)
                {
                    var aadhar = GenericFunctions.UploadDocuments(_env, ib.podAtttachedment, "ib");
                    ib.attachments = string.Join(",", aadhar);
                }

                Int64[] shpids = ib.shipmentNos.Split(",").ToList().Select(x => Convert.ToInt64(x)).ToArray();
                var shpl = await _context.Shipment.Where(x => shpids.Contains(x.shipmentId)).ToListAsync();
                if (shpl.Any())
                {
                    decimal basic = shpl.Sum(x => x.basicFreight);
                    decimal enRoute = shpl.Sum(x => x.enRoute);
                    decimal gst = shpl.Sum(x => x.basicFreight) * (decimal)0.18;
                    decimal totalFreight = basic + enRoute + gst;
                    user = _http.HttpContext.Session.GetString("username");
                    string quoteIds = string.Join(",", shpl.Select(x => x.quotationId).Distinct().ToList());

                    var qs = await _spc.QueryResult
                        .FromSqlRaw("CALL `sbl_gererateIbBillV1`({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11})",
                        ib.id,
                        quoteIds,
                        ib.invoiceNo,
                        ib.invoiceDate,
                        ib.poNumber,
                        basic,
                        enRoute,
                        gst,
                        totalFreight,
                        ib.shipmentNos,
                        ib.attachments,
                        user)
                        .IgnoreQueryFilters()
                        .ToListAsync();

                    result = qs.Any() ? qs.FirstOrDefault() : result;
                }
                else {
                    result = new QueryResult()
                    {
                        errorCode = 301,
                        Message = "Shipments not found..."
                    };
                }
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        public async Task<QueryResult> updateIbBill(IbBillingModel ib)
        {
            QueryResult result = new QueryResult()
            {
                errorCode = 404,
                Message = "Something went wrong.."
            };
            try
            {
                user = _http.HttpContext.Session.GetString("username");
                var qs = await _spc.QueryResult
                    .FromSqlRaw("CALL `sbl_updateIbBill`({0},{1},{2},{3},{4})",
                    ib.id,
                    ib.invoiceNo,
                    ib.invoiceDate,
                    ib.poNumber,
                    user)
                    .IgnoreQueryFilters()
                    .ToListAsync();

                result = qs.Any() ? qs.FirstOrDefault() : result;
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        public async Task<QueryResult> removeIbBill(IbBillingModel ib)
        {
            QueryResult result = new QueryResult()
            {
                errorCode = 404,
                Message = "Something went wrong.."
            };
            try
            {
                Int64[] shpids = ib.shipmentNos.Split().ToList().Select(x => Convert.ToInt64(x)).ToArray();
                var shpl = await _context.Shipment.Where(x => shpids.Contains(x.shipmentId)).ToListAsync();

                user = _http.HttpContext.Session.GetString("username");
                var qs = await _spc.QueryResult
                    .FromSqlRaw("CALL `sbl_removeIbBill`({0},{1})",
                    ib.id,
                    user)
                    .IgnoreQueryFilters()
                    .ToListAsync();

                result = qs.Any() ? qs.FirstOrDefault() : result;
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        public async Task<IbBillingModel> GetIbBillingById(string billNo = null)
        {
            IbBillingModel ib = new IbBillingModel();
            try
            {
                if (string.IsNullOrWhiteSpace(billNo))
                {
                    ib.id = Guid.NewGuid().ToString().Replace("-", "");

                    ib.shipmentList=await _spc.IbBillingShipments
                        .FromSqlRaw("CALL `sbl_getIbShipments`()")
                        .IgnoreQueryFilters()
                        .ToListAsync();
                }
                else
                {
                    var pbi = _context.IbBilling.Find(billNo);

                    ib.id = pbi.id;
                    ib.invoiceNo = pbi.invoiceNo;
                    ib.invoiceDate = pbi.invoiceDate;
                    ib.poNumber = pbi.poNumber;
                    ib.shipmentNos = pbi.shipments;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ib;
        }

        public async Task<Tuple<bool, string, string>> getShipmetListByQuote(string id)
        {
            bool success = false;
            string temp = string.Empty;
            string shipments = string.Empty;
            try
            {
                if (!string.IsNullOrWhiteSpace(id))
                {
                    var list = await _context.Shipment.Where(
                        x => (x.quotationId.Equals(id) && x.status.Equals(shipmentStatus.DELIVERED.ToString()))
                        ).ToListAsync();

                    if (list.Any())
                    {
                        int i = 0;
                        shipments = string.Join(",", list.Select(x=>x.shipmentId).ToArray());
                        foreach (var s in list)
                        {
                            temp += $"<tr>" +
                                $"<td>{++i}</td>" +
                                $"<td>{s.shipmentNo}</td>" +
                                $"<td>{s.shipmentDate.ToString("dd-MM-yyyy")}</td>" +
                                $"<td>{s.vcNo}</td>" +
                                $"<td>{s.modelDesc}</td>" +
                                $"<td>{s.chasisNo}</td>" +
                                $"<td>{s.invoiceNo}</td>" +
                                $"<td>{s.invoiceDate.ToString("dd-MM-yyyy")}</td>" +
                                $"<td>{s.trasitDays}</td>" +
                                $"<td>{s.basicFreight}</td>" +
                                $"<td>{s.enRoute}</td>" +
                                $"<td>{s.totalFreight}</td>" +
                                $"</tr>";
                        }
                    }
                    else
                    {
                        temp = $"<tr>" +
                                $"<td>Shipments not found...!</td>" +
                                $"</tr>";
                    }
                }
                success = true;
            }
            catch (Exception)
            {

                throw;
            }

            return new Tuple<bool, string, string>(success, temp, shipments);
        }

        public async Task<IbBilling> GetIbBillingInvoice(string id,string type, int pageRef=1)
        {
            IbBilling b = new IbBilling();
            List<QuoteShipment> list = new List<QuoteShipment>();
            try
            {
                b = await _context.IbBilling.FindAsync(id);
                if (b != null)
                {
                    list = await _spc.QuoteShipments
                        .FromSqlRaw("CALL `sbl_getQuoteShipments`({0})", b.quoteId)
                        .IgnoreQueryFilters()
                        .ToListAsync();

                    if (list.Any())
                    {
                        var quotes = (from a in list
                                      group a by a.vcNo into g
                                      select new QuoteDetail
                                      {
                                          source = g.FirstOrDefault()?.source,
                                          destination = g.FirstOrDefault()?.destination,
                                          qty = g.Count(),
                                          modelDesc = g.FirstOrDefault()?.model,
                                          basicFreight = g.FirstOrDefault().basic,
                                          enRoute = g.FirstOrDefault().enRoute,
                                          totalFreight = g.FirstOrDefault().basic + g.FirstOrDefault().enRoute
                                      }).ToList();
                        b.list = pageRef == 2 ? null : quotes;
                        b.spList = pageRef == 2 ? list : null;
                    }

                    if (type.Equals("performa"))
                    {
                        b.invoiceNo = null;
                        b.invoiceDate = null;
                        b.poNumber = null;
                    }

                    b.amountInWord = GetAmountInWords(b.totalFreight);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return b;
        }

        #endregion "IB BILLING"

        #region "HELPER"
        private string GetAmountInWords(decimal totalFreight)
        {
            string returnValue;
            string strNum, strNumDec, StrWord;

            strNum = totalFreight.ToString();


            if (strNum.IndexOf(".") + 1 != 0)
            {
                strNumDec = strNum.Substring(strNum.IndexOf(".") + 2 - 1);


                if (strNumDec.Length == 1)
                {
                    strNumDec = strNumDec + "0";
                }
                if (strNumDec.Length > 2)
                {
                    strNumDec = strNumDec.Substring(0, 2);
                }


                strNum = strNum.Substring(0, strNum.IndexOf(".") + 0);
                StrWord = ((double.Parse(strNum) == 1) ? " Rupee " : " Rupees ") + NumToWord((decimal)(double.Parse(strNum))) + ((double.Parse(strNumDec) > 0) ? (" and " + cWord3((decimal)(double.Parse(strNumDec))) + " Paise") : "");
            }
            else
            {
                StrWord = ((double.Parse(strNum) == 1) ? " Rupee " : " Rupees ") + NumToWord((decimal)(double.Parse(strNum)));
            }
            returnValue = StrWord + " Only";
            return returnValue;
        }
        public string NumToWord(decimal Num)
        {
            string returnValue;
            string strNum;
            string StrWord;
            strNum = Num.ToString();


            if (strNum.Length <= 3)
            {
                StrWord = cWord3((decimal)(double.Parse(strNum)));
            }
            else
            {
                StrWord = cWordG3((decimal)(double.Parse(strNum.Substring(0, strNum.Length - 3)))) + " " + cWord3((decimal)(double.Parse(strNum.Substring(strNum.Length - 2 - 1))));
            }
            returnValue = StrWord;
            return returnValue;
        }

        public string cWordG3(decimal Num)
        {
            string returnValue;
            //2. more than three digit number.
            string strNum = "";
            string StrWord = "";
            string readNum = "";
            strNum = Num.ToString();
            if (strNum.Length % 2 != 0)
            {
                readNum = System.Convert.ToString(double.Parse(strNum.Substring(0, 1)));
                if (readNum != "0")
                {
                    StrWord = retWord(decimal.Parse(readNum));
                    readNum = System.Convert.ToString(double.Parse("1" + strReplicate("0", strNum.Length - 1) + "000"));
                    StrWord = StrWord + " " + retWord(decimal.Parse(readNum));
                }
                strNum = strNum.Substring(1);
            }
            while (!System.Convert.ToBoolean(strNum.Length == 0))
            {
                readNum = System.Convert.ToString(double.Parse(strNum.Substring(0, 2)));
                if (readNum != "0")
                {
                    StrWord = StrWord + " " + cWord3(decimal.Parse(readNum));
                    readNum = System.Convert.ToString(double.Parse("1" + strReplicate("0", strNum.Length - 2) + "000"));
                    StrWord = StrWord + " " + retWord(decimal.Parse(readNum));
                }
                strNum = strNum.Substring(2);
            }
            returnValue = StrWord;
            return returnValue;
        }
        public string cWord3(decimal Num)
        {
            string returnValue;
            //1. Three or less digit number.
            string strNum = "";
            string StrWord = "";
            string readNum = "";
            if (Num < 0)
            {
                Num = Num * -1;
            }
            strNum = Num.ToString();


            if (strNum.Length == 3)
            {
                readNum = System.Convert.ToString(double.Parse(strNum.Substring(0, 1)));
                StrWord = retWord(decimal.Parse(readNum)) + " Hundred";
                strNum = strNum.Substring(1, strNum.Length - 1);
            }


            if (strNum.Length <= 2)
            {
                if (double.Parse(strNum) >= 0 && double.Parse(strNum) <= 20)
                {
                    StrWord = StrWord + " " + retWord((decimal)(double.Parse(strNum)));
                }
                else
                {
                    StrWord = StrWord + " " + retWord((decimal)(System.Convert.ToDouble(strNum.Substring(0, 1) + "0"))) + " " + retWord((decimal)(double.Parse(strNum.Substring(1, 1))));
                }
            }


            strNum = Num.ToString();
            returnValue = StrWord;
            return returnValue;
        }
        public string retWord(decimal Num)
        {
            string returnValue;
            //This two dimensional array store the primary word convertion of number.
            returnValue = "";
            object[,] ArrWordList = new object[,] { { 0, "" }, { 1, "One" }, { 2, "Two" }, { 3, "Three" }, { 4, "Four" }, { 5, "Five" }, { 6, "Six" }, { 7, "Seven" }, { 8, "Eight" }, { 9, "Nine" }, { 10, "Ten" }, { 11, "Eleven" }, { 12, "Twelve" }, { 13, "Thirteen" }, { 14, "Fourteen" }, { 15, "Fifteen" }, { 16, "Sixteen" }, { 17, "Seventeen" }, { 18, "Eighteen" }, { 19, "Nineteen" }, { 20, "Twenty" }, { 30, "Thirty" }, { 40, "Forty" }, { 50, "Fifty" }, { 60, "Sixty" }, { 70, "Seventy" }, { 80, "Eighty" }, { 90, "Ninety" }, { 100, "Hundred" }, { 1000, "Thousand" }, { 100000, "Lakh" }, { 10000000, "Crore" } };


            int i;
            for (i = 0; i <= (ArrWordList.Length - 1); i++)
            {
                if (Num == System.Convert.ToDecimal(ArrWordList[i, 0]))
                {
                    returnValue = (string)(ArrWordList[i, 1]);
                    break;
                }
            }
            return returnValue;
        }
        public string strReplicate(string str, int intD)
        {
            string returnValue;
            //This fucntion padded "0" after the number to evaluate hundred, thousand and on....
            //using this function you can replicate any Charactor with given string.
            int i;
            returnValue = "";
            for (i = 1; i <= intD; i++)
            {
                returnValue = returnValue + str;
            }
            return returnValue;
        }

        #endregion "HELPER"
    }
}
