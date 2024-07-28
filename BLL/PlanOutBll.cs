using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Trans9.DataAccess;
using Trans9.Models;

namespace Trans9.BLL
{
    public class PlanOutBll
    {
        private readonly DataDbContext _context;
        private readonly IHostEnvironment _env;
        private readonly ShipmentBll _spcontext;
        public PlanOutBll(DataDbContext context,StoredProcedureDbContext spcontext,IHostEnvironment env)
        {
            _context = context;
            _spcontext = new ShipmentBll(context, spcontext, env);
            _env = env;
        }

        public async Task<bool> AddOrUpdatePlantout(int id, PlantOut po, string user)
        {
            DateTime dt2 = DateTime.Now;
            string formattedDateTime1 = dt2.ToString("yyyy-MM-dd HH:mm:ss");
            DateTime dt3 = DateTime.ParseExact(formattedDateTime1, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

            DateTime dt = po.createdDate.Date;
            string formattedDateTime = dt.ToString("yyyy-MM-dd HH:mm:ss");
            DateTime dt1 = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

            bool success = false;
            try
            {
                if (id == 0)
                {
                    decimal loadingchargse = 0;
                    loadingchargse = po.loadingCharges;
                    Shipment sp = await _context.Shipment.FindAsync(po.shipmentId);

                    if (((sp.detsinationId == 136 || sp.detsinationId == 134) && loadingchargse > 0) || (sp.detsinationId == 125 && loadingchargse > 50))
                    {
                        po.status = (!string.IsNullOrEmpty(po.ewayNo)) ? shipmentStatus.APPROVALPENDING.ToString() : po.status;
                        po.updatedBy = user;
                        _context.Add(po);
                        po.actualPlantOut = dt3;
                    }
                    else
                    {
                        po.status = (!string.IsNullOrEmpty(po.ewayNo)) ? shipmentStatus.IN_YARD.ToString() : po.status;
                        po.updatedBy = user;
                        _context.Add(po);
                        po.actualPlantOut = dt3;
                    }
                }
                else
                {

                    po.updatedBy = user;
                    _context.Update(po);
                }
                int n = await _context.SaveChangesAsync();
                success = (n > 0) ? true : false;
                if (success)
                {
                    Shipment sp = await _context.Shipment.FindAsync(po.shipmentId);

                    if (((sp.detsinationId == 136 || sp.detsinationId == 134) && po.loadingCharges > 0) || (sp.detsinationId == 125 && po.loadingCharges > 50))
                    {
                        sp.status = (!string.IsNullOrEmpty(po.ewayNo)) ? shipmentStatus.APPROVALPENDING.ToString() : po.spStatus;
                    }
                    else
                    {
                        sp.status = (!string.IsNullOrEmpty(po.ewayNo)) ? shipmentStatus.IN_YARD.ToString() : po.spStatus;
                    }
                    sp.updatedDate = dt1;
                    sp.updatedBy = user;
                    //sp.actualPlantOut = dt3;

                    success = await _spcontext.UpdateShipment(sp);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return success;

        }
        public async Task<List<PlantOut>> GetPlantoutList()
        {
            List<PlantOut> list = new List<PlantOut>();
            try
            {
                var l = await _context.PlantOut.ToListAsync();
                if (l.Any())
                {
                    list = (from s in l
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

        private bool PlantOutExists(Int64 id = 0)
        {
            return _context.PlantOut.Any(e => e.shipmentId == id);
        }
    }
}
