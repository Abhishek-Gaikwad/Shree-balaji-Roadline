using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;
using System.Data;
using System.Reflection;
using Trans9.DataAccess;
using Trans9.Models;
using Trans9.Utility;

namespace Trans9.BLL
{
    public class QuotationBll
    {
        private readonly DataDbContext _context;
        private readonly StoredProcedureDbContext _spc;
        private readonly IHostEnvironment _env;
        private readonly IHttpContextAccessor _http;
        private string user = string.Empty;
        public QuotationBll(DataDbContext context, StoredProcedureDbContext spc, IHttpContextAccessor http, IHostEnvironment env)
        {
            _context = context;
            _spc = spc;
            _env = env;
            _http = http;
        }

        public async Task<Tuple<bool, string>> InsertDetails(QuoteDetailDto dt)
        {
            bool success = false;
            try
            {

                dt.Id = Guid.NewGuid().ToString().Replace("-", "");
                dt.userid = _http.HttpContext.Session.GetString("username");
                _spc.TempQuoteDetails.Add(dt);
                int n = await _spc.SaveChangesAsync();
                success = (n > 0) ? true : false;
            }
            catch (Exception)
            {
                throw;
            }
            return Tuple.Create(success, dt.Id);
        }

        public async Task<Tuple<bool, string>> Delete(string Id)
        {
            bool success = false;
            try
            {
                var d = _spc.TempQuoteDetails.Find(Id);
                _spc.TempQuoteDetails?.Remove(d);
                int n = await _spc.SaveChangesAsync();
                success = (n > 0) ? true : false;
            }
            catch (Exception)
            {
                throw;
            }
            return Tuple.Create(success, Id);
        }

        public async Task<QueryResult> Insert(QuoteDto dt)
        {
            QueryResult result = new QueryResult()
            {
                errorCode = 404,
                Message = "Something went wrong.."
            };
            bool success = true;
            string fy = "", temp = "";
            QuotationNumer bs = new QuotationNumer();
            Int64 billgrp = 0;
            try
            {
                //temp = "start before billgroup";
                billgrp = GenericFunctions.ToUnixTime();
                //temp = "start before serialNo";
                var serialList = GetQuotationSerial(dt.quoteDate, out fy);
                user = _http.HttpContext.Session.GetString("username");

                var qs = await _spc.QueryResult
                    .FromSqlRaw("CALL `sbl_createQuotation`({0},{1},{2},{3},{4})", dt.quoteId, dt.quoteNo, dt.quoteFor, dt.quoteDate, user)
                    .IgnoreQueryFilters()
                    .ToListAsync();

                result = qs.Any() ? qs.FirstOrDefault() : result;
                bs = serialList.FirstOrDefault();
                serialList.Remove(bs);
                bs.serial += 1;
                serialList.Add(bs);
           
                //temp = "start before Saving all changes";
            //    int n = await _context.SaveChangesAsync();

            //success = n > 0 ? true : false;

            //if (success)
            //{
            //    //temp = "start before Updating Serial NO";
                success = UpdatequotationSerial(serialList, fy);

           // }
        }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        public async Task<QuoteDto> GetQuotationById(string? quoteId)
        {
            QuoteDto q = new QuoteDto();
            bool success = false;
            QuotationNumer bs = new QuotationNumer();
            Int64 billgrp = 0;
            string fy = "", temp = "";
            try
            {
                //temp = "start before billgroup";
                billgrp = GenericFunctions.ToUnixTime();
                //temp = "start before serialNo";
                var serialList = GetQuotationSerial(q.quoteDate, out fy);
                if (string.IsNullOrWhiteSpace(quoteId))
                {
                    q.quoteId = Guid.NewGuid().ToString().Replace("-", "");
                }
                else
                {
                    var quot = _context.Quotations.Find(quoteId);

                    user = _http.HttpContext.Session.GetString("username");
                    var qs = await _spc.QuoteDetailShow
                        .FromSqlRaw("CALL `sbl_getQuotation`({0},{1})", quoteId, user)
                        .IgnoreQueryFilters()
                        .ToListAsync();

                    q.quoteId = quot.quoteId;
                    q.quoteNo = quot.quoteNo;
                    q.quoteFor = quot.quoteFor;
                    q.quoteDate = quot.quoteDate;
                    q.details = qs;
                }
                bs = serialList.FirstOrDefault();
                //var  bs = new { prefix = "QT", serial = 9 };
                string billNo = $"SBL/{fy}/{(bs.serial <= 99 ? $"00{bs.serial}" : $"{bs.serial}")}";
                q.quoteNo = billNo;
                q.vcList = await DataLoader.GetVCDropDown(_spc);
                q.destinationList = await DataLoader.GetDestinationDropDown(_context);

            }

            catch (Exception ex) {
                throw ex;
            }
            return q;
        }

        private List<QuotationNumer> GetQuotationSerial(DateTime date, out string fy)
        {
            fy = string.Empty;
            List<QuotationNumer> bill = new List<QuotationNumer>();
            try
            {
                Int64 yr = Convert.ToInt64(date.ToString("yy"));
                var fyear = (date.Month >= 4) ? $"{yr}-{yr + 1}"
                    : $"{yr - 1}{yr}";

                string filePath = Path.Combine(_env.ContentRootPath, $"wwwroot/dist/template/quotation.json");

                string datastring = System.IO.File.ReadAllText(filePath);
                if (!string.IsNullOrEmpty(datastring))
                {
                    Dictionary<string, List<QuotationNumer>> data = JsonConvert.DeserializeObject<Dictionary<string, List<QuotationNumer>>>(datastring);
                    if (data != null)
                    {
                        bill = data.Where(x => x.Key == fyear).Select(x => x.Value).FirstOrDefault();
                    }
                }
                if (bill == null)
                {
                    bill = new List<QuotationNumer>();
                    bill.Add(new QuotationNumer()
                    {
                        serial = 1,
                        prefix = "QT"
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
        private bool UpdatequotationSerial(List<QuotationNumer> serialList, string fy)
        {
            bool success = false;
            try
            {
                string filePath = Path.Combine(_env.ContentRootPath, $"wwwroot/dist/template/quotation.json");

                string datastring = System.IO.File.ReadAllText(filePath);
                if (!string.IsNullOrEmpty(datastring))
                {
                    Dictionary<string, List<QuotationNumer>> data = JsonConvert.DeserializeObject<Dictionary<string, List<QuotationNumer>>>(datastring);
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
                        data = new Dictionary<string, List<QuotationNumer>>();
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

        public async Task<List<Quotes>> GetQuotationList()
        {
            List<Quotes> list = new List<Quotes>();
            try
            {
                list = await _spc.Quotes
                    .FromSqlRaw("CALL `sbl_GetQuotationList`()")
                    .IgnoreQueryFilters()
                .ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
            return list;
        }

        public async Task<Quotation> GetQuoteById(string id)
        {
            Quotation q = new Quotation();
            try
            {
                q = await _context.Quotations.FindAsync(id);
                q.QuoteDetails = await _context.QuoteDetails.Where(x => x.quoteId.Equals(id)).ToListAsync();
            }
            catch (Exception)
            {
                q = null;
                throw;
            }
            return q;
        }

        public async Task<QueryResult> Update(QuoteDto dt)
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
                    .FromSqlRaw("CALL `sbl_updateQuotation`({0},{1},{2},{3},{4})", dt.quoteId, dt.quoteNo, dt.quoteFor, dt.quoteDate, user)
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

        public async Task<QueryResult> RemoveCache(string id)
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
                    .FromSqlRaw("CALL `sbl_removeQuoteCache`({0},{1})", id, user)
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

        public async Task<MemoryStream> GetQuotationExcel(string id)
        {
            MemoryStream stream = new MemoryStream();

            try
            {
                int i = 1;
                var tbl = await _spc.QuoteReport
                    .FromSqlRaw("CALL `sbl_GetQuoteReport`({0})", id)
                    .IgnoreQueryFilters()
                    .ToListAsync();

  
                if (tbl.Any())
                {

                    PropertyInfo[] properties = tbl.First().GetType().GetProperties();
                    List<string> headerNames = properties.Select(prop => prop.Name.Replace("_", " ")).ToList();

                    XLWorkbook wb = new XLWorkbook();

                    stream = GenericFunctions.GetExcelData(wb, tbl, "Quotation-details", headerNames, true);
                }
            }
            catch (Exception)
            {

                throw;
            }
            return stream;
        }

        public async Task<bool> DeleteQuotation(string id)
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
                    .FromSqlRaw("CALL `sbl_deleteQuotation`({0})", id)
                    .IgnoreQueryFilters()
                    .ToListAsync();

                result = qs.Any() ? qs.FirstOrDefault() : result;
            }
            catch (Exception)
            {
                throw;
            }
            return result.errorCode == 1 ? true : false;
        }
    }
}
