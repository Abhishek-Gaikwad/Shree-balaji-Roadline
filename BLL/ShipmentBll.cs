using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics.Metrics;
using Trans9.DataAccess;
using Trans9.Models;
using Trans9.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Trans9.BLL
{
    public class ShipmentBll
    {
        private readonly DataDbContext _dbc;
        private readonly StoredProcedureDbContext _spc;
        private readonly IHostEnvironment _env;

        public ShipmentBll(DataDbContext context, StoredProcedureDbContext spcontext, IHostEnvironment env = null)
        {
            _dbc = context;
            _spc = spcontext;
            _env = env;
        }


        public async Task<List<Shipment>> GetShipments(string action)
        {
            List<String> status = new List<string>();
            List<Shipment> list = new List<Shipment>();
            try
            {
                switch (action)
                {
                    case "shipment":
                        status = new List<string> {
                                    shipmentStatus.IN_PLANT.ToString()
                            };
                        break;
                    case "plantout":
                        status = new List<string> {
                                    shipmentStatus.IN_PLANT.ToString(),
                                    shipmentStatus.AT_BODY_BUILDER.ToString(),
                                    shipmentStatus.EWAY_EXPIRED.ToString(),
                                    shipmentStatus.DIESEL_SHORTAGE.ToString(),
                                    shipmentStatus.DENT_OR_DAMAGE.ToString(),
                                    shipmentStatus.TOOLKIT_MISSING.ToString(),
                                    shipmentStatus.NOT_IN_RCP.ToString(),
                                    shipmentStatus.TYRE_CUT.ToString(),
                                    shipmentStatus.QUALITY_ISSUE.ToString(),
                                    shipmentStatus.PDI_PENDING.ToString(),
                                    shipmentStatus.PDI_HOLD.ToString(),
                                    shipmentStatus.PAINT_SCRATCHES.ToString(),
                                    shipmentStatus.RUSTY_VEHICLE.ToString(),
                                    shipmentStatus.MIRROR_CRACK_MISSING.ToString(),
                                    shipmentStatus.KEY_MISSING.ToString(),
                                    shipmentStatus.CNG_SHORTAGE.ToString(),
                                    shipmentStatus.DEF_SHORTAGE.ToString(),
                                    shipmentStatus.OTHERS.ToString(),
                                    shipmentStatus.BATTERY_DOWN.ToString(),
                                    shipmentStatus.BREAK_OIL_LEAKAGE.ToString(),
                                    shipmentStatus.PAINT_SCRATCH.ToString(),
                                    shipmentStatus.BUMPER_SCRATCH.ToString(),
                                    shipmentStatus.CAB_BACK_SIDE_DENT.ToString(),
                                    shipmentStatus.CLUTCH_FAIL.ToString(),
                                    shipmentStatus.CNG_SHORT.ToString(),
                                    shipmentStatus.COOLANT_LEAKAGE_FROM_RADIATOR.ToString(),
                                    shipmentStatus.CP_FLAP_DENT.ToString(),
                                    shipmentStatus.DENT_ON_BONET.ToString(),
                                    shipmentStatus.D30_VEHICLE.ToString(),
                                    shipmentStatus.LH_SIDE_DOOR_DENT.ToString(),
                                    shipmentStatus.WIPER_SHORT.ToString(),
                                    shipmentStatus.DEF_CAP_MISSING.ToString(),
                                    shipmentStatus.DENT_ON_FRONT_SHOW.ToString(),
                                    shipmentStatus.DIESAL_EXTRA.ToString(),
                                    shipmentStatus.HOLD_BY_SECURITY.ToString(),
                                    shipmentStatus.DIESAL_LOCK_MISSING.ToString(),
                                    shipmentStatus.DIESAL_SHORT.ToString(),
                                    shipmentStatus.SIESAL_SHORT_AT_HOSTEL.ToString(),
                                    shipmentStatus.DUSTY_VEHICLE.ToString(),
                                    shipmentStatus.RH_SIDE_SCRATCHES.ToString(),
                                    shipmentStatus.INDICATOR_NOT_WORKING.ToString(),
                                    shipmentStatus.RH_AND_LH_SIDE_CAIN_DENT.ToString(),
                                    shipmentStatus.BUMPER_PAINT_SCRATCH.ToString(),
                                    shipmentStatus.BREAK_LIGHT_NOT_WORKING.ToString(),
                                    shipmentStatus.DEF_CAP_AND_WIPER_SHORT.ToString(),
                                    shipmentStatus.DOOR_DENT.ToString(),
                                    shipmentStatus.RH_REAR_TYRE_CUT.ToString(),
                                    shipmentStatus.DIESAL_TANK_SCRATCHES.ToString(),
                                    shipmentStatus.LH_SIDE_TAIL_LAMP_SCRATCHES.ToString(),
                                    shipmentStatus.EXTRA_KMS.ToString(),
                                    shipmentStatus.FRONT_SHOW_DAMAGE.ToString(),
                                    shipmentStatus.FRONT_SHOW_PAINT_ISSUE.ToString(),
                                    shipmentStatus.EXPORT.ToString(),
                                    shipmentStatus.FOOTREST_DAMAGE.ToString(),
                                    shipmentStatus.FRONT_GRILL_DAMAGE.ToString(),
                                    shipmentStatus.REWORK_NOT_DONE_PROPERLY.ToString(),
                                    shipmentStatus.FUSE_COVER_NOT_FITTED_PROPERLY.ToString(),
                                    shipmentStatus.STARTING_PROBLEM.ToString(),
                                    shipmentStatus.LH_DOOR_DENT.ToString(),
                                    shipmentStatus.GEAR_KNOB_MISSING.ToString(),
                                    shipmentStatus.HOLD_BY_QA.ToString(),
                                    shipmentStatus.COLOR_MISMATCH.ToString(),
                                    shipmentStatus.HOLD_BY_SALES_OR_DEALER.ToString(),
                                    shipmentStatus.HOLD_FOR_ONLINE_CRTEM.ToString(),
                                    shipmentStatus.MIRRIR_DAMAGE.ToString(),
                                    shipmentStatus.NOT_LOCATED_IN_HOSTEL.ToString(),
                                    shipmentStatus.OK_IN_HOSTEL.ToString(),
                                    shipmentStatus.OK_IN_RCP.ToString(),
                                    shipmentStatus.RH_SIDE_DOOR_DENT.ToString(),
                                    shipmentStatus.TYRE_PUNCTURE.ToString(),
                                    shipmentStatus.PAINT_SCRACHES_BACK_SIDE.ToString(),
                                    shipmentStatus.REAR_CABIN_DENT.ToString(),
                                    shipmentStatus.JACK_MISSING.ToString(),
                                    shipmentStatus.RH_SIDE_MIRROR_MISSING.ToString(),
                                    shipmentStatus.PDI.ToString(),
                                    shipmentStatus.OIL_LEAKAGE.ToString(),
                                    shipmentStatus.WRONG_PDI_TAG.ToString(),
                                    shipmentStatus.FUSE_COVER_DAMAGE.ToString(),
                                    shipmentStatus.SUPD_DAMAGE.ToString(),
                                    shipmentStatus.RH_SIDE_DENT.ToString(),
                                    shipmentStatus.RH_SIDE_INDICATOR_MISSING.ToString(),
                                    shipmentStatus.TAIL_LAMP_CRACK.ToString(),
                                    shipmentStatus.FOG_LAMP_REFLECTOR_DAMAGE.ToString(),
                                    shipmentStatus.RUPD_DAMAGE.ToString(),
                                    shipmentStatus.RUPD_RUSTY.ToString(),
                                    shipmentStatus.AIR_LEAKAGE.ToString(),
                                    shipmentStatus.ENG_STOP.ToString(),
                                    shipmentStatus.WIRING_HARNESS_ISSUE.ToString(),
                                    shipmentStatus.WIPER_NOT_WORKING.ToString()
                            };
                        break;
                    case "marching":
                        status = new List<string> {
                                    shipmentStatus.IN_YARD.ToString()
                            };
                        break;
                    case "payment":
                        status = new List<string> {
                                    shipmentStatus.IN_TRANSIT.ToString()
                            };
                        break;
                    case "closing":
                        status = new List<string> {
                                    shipmentStatus.IN_TRANSIT.ToString(),
                                    shipmentStatus.EPOD_PENDING.ToString(),
                                    shipmentStatus.BREAKDOWN.ToString(),
                                    shipmentStatus.ACCIDENT.ToString()
                            };
                        break;
                    case "billing":
                        status = new List<string> {
                                    shipmentStatus.DELIVERED.ToString()
                            };
                        break;
                    case "incidence":
                        status = new List<string> {
                                    shipmentStatus.IN_TRANSIT.ToString()
                            };
                        break;

                    case "auth":
                        status = new List<string> {
                                    shipmentStatus.BREAKDOWN.ToString(),
                                    shipmentStatus.ACCIDENT.ToString()
                            };
                        break;
                }

                if (status.Any())
                {
                    var sl = await _dbc.Shipment.ToListAsync();
                    var dl = await _dbc.Dealer.ToListAsync();
                    var ll = await _dbc.Destination.ToListAsync();
                    var qu = await _dbc.Quotations.ToListAsync();
                    // var qud = await _context.quotedetails.ToListAsync();
                    if (action == $"{Actions.billing}")
                    {
                        list = (from s in sl
                                join d in dl on s.dealerId equals d.dealerId
                                join l in ll on s.detsinationId equals l.detsinationId
                                join q in qu on s.quotationId equals q.quoteId into qa
                                from qb in qa.DefaultIfEmpty(new Quotation())
                                where (status.Contains(s.status)
                                      && s.cid == 1)
                                select new Shipment
                                {
                                    shipmentId = s.shipmentId,
                                    shipmentNo = s.shipmentNo,
                                    shipmentDate = s.shipmentDate.Date,
                                    dealerName = d.dealerName,
                                    dest = l.destination,
                                    location = s.location,
                                    trasitDays = s.trasitDays,
                                    region = s.region,
                                    routeCode = s.routeCode,
                                    vcNo = s.vcNo,
                                    modelDesc = s.modelDesc,
                                    mfgCode = s.mfgCode,
                                    plantCode = s.plantCode,
                                    plantDesc = s.plantDesc,
                                    mfgRoute = s.mfgRoute,
                                    chasisNo = s.chasisNo,
                                    invoiceNo = s.invoiceNo,
                                    invoiceDate = s.invoiceDate.Date,
                                    basicFreight = s.basicFreight,
                                    enRoute = s.enRoute,
                                    totalFreight = s.totalFreight,
                                    tempRegNo = s.tempRegNo,
                                    quotationId = qb.quoteId,
                                    epodNo = s.epodNo,
                                    epodDate = s.epodDate,
                                    incidanceDate = s.incidanceDate,
                                    reTransitDate = s.reTransitDate,
                                    incidanceDelayed = s.incidanceDelayed,
                                    status = s.status,
                                    closingDate = s.closingDate,
                                    createdDate = s.createdDate,
                                    updatedDate = s.updatedDate,
                                    createdBy = s.createdBy,
                                    updatedBy = s.updatedBy
                                }).ToList();
                    }
                    else
                    {
                        list = (from s in sl
                                join d in dl on s.dealerId equals d.dealerId
                                join l in ll on s.detsinationId equals l.detsinationId
                                join q in qu on s.quotationId equals q.quoteId into qa
                                from qb in qa.DefaultIfEmpty(new Quotation())
                                where status.Contains(s.status)
                                select new Shipment
                                {
                                    shipmentId = s.shipmentId,
                                    shipmentNo = s.shipmentNo,
                                    shipmentDate = s.shipmentDate.Date,
                                    dealerName = d.dealerName,
                                    dest = l.destination,
                                    location = s.location,
                                    trasitDays = s.trasitDays,
                                    region = s.region,
                                    routeCode = s.routeCode,
                                    vcNo = s.vcNo,
                                    modelDesc = s.modelDesc,
                                    mfgCode = s.mfgCode,
                                    plantCode = s.plantCode,
                                    plantDesc = s.plantDesc,
                                    mfgRoute = s.mfgRoute,
                                    chasisNo = s.chasisNo,
                                    invoiceNo = s.invoiceNo,
                                    invoiceDate = s.invoiceDate.Date,
                                    basicFreight = s.basicFreight,
                                    enRoute = s.enRoute,
                                    quotationId = qb.quoteId,
                                    totalFreight = s.totalFreight,
                                    tempRegNo = s.tempRegNo,
                                    epodNo = s.epodNo,
                                    epodDate = s.epodDate,
                                    incidanceDate = s.incidanceDate,
                                    reTransitDate = s.reTransitDate,
                                    incidanceDelayed = s.incidanceDelayed,
                                    status = s.status,
                                    closingDate = s.closingDate,
                                    createdDate = s.createdDate,
                                    updatedDate = s.updatedDate,
                                    createdBy = s.createdBy,
                                    updatedBy = s.updatedBy
                                }).ToList();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return list;
        }

        public async Task<Shipment> GetShipmentById(long id, bool isSp = false)
        {
            DateTime dt = DateTime.Now.Date;
            Shipment shp = new Shipment();
            try
            {
                shp = await _dbc.Shipment.FindAsync(id);

                if (shp == null)
                {
                    shp = new Shipment()
                    { createdDate = dt, status = shipmentStatus.IN_PLANT.ToString() };
                }

                shp.updatedDate = dt;

                if (isSp)
                {
                    shp.dealerList = await DataLoader.GetDealerDropDown(_dbc);
                    shp.destinations = await DataLoader.GetDestinationDropDown(_dbc);
                    shp.vcList = await DataLoader.GetVCDropDown(_spc, shp.quotationId);
                    shp.companyList = DataLoader.GetCompanyDropdown(_dbc);
                    shp.quotationList = DataLoader.GetQuotationDropdown(_dbc);
                }
                else
                {

                    var dlr = await _dbc.Dealer.FindAsync(shp.dealerId);
                    var dst = await _dbc.Destination.FindAsync(shp.detsinationId);
                    var plo = _dbc.PlantOut.Where(x => x.shipmentId == shp.shipmentId).FirstOrDefault();
                    shp.dealerName = (dlr != null) ? dlr.dealerName : string.Empty;
                    shp.dest = (dst != null) ? dst.destination : string.Empty;
                    shp.updatedDate = (plo != null) ? plo.createdDate : null;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return shp;
        }
        public async Task<PlantOut> GetPlanttById(long id, bool isSp = false)
        {
            DateTime dt = DateTime.Now.Date;
            PlantOut po = new PlantOut();
            try
            {
              //  po = await _dbc.PlantOut.FindAsync(id);
              //  po = await _dbc.PlantOut.OrderByDescending(p => p.ewayId).FirstOrDefaultAsync();


                po = await _dbc.PlantOut
                    .Where(p => p.shipmentId == id)
                    .OrderByDescending(p => p.ewayId)
                    .FirstOrDefaultAsync();

                if (po == null)
                {
                    po = new PlantOut()
                    { createdDate = dt, status = shipmentStatus.IN_PLANT.ToString() };
                }

               
                po.createdDate = po.createdDate;
               // }
            }
            catch (Exception)
            {
                throw;
            }
            return po;
        }

        public async Task<bool> AddOrUpdateShipment(long id, Shipment shp)
        {
            bool success = false;
            try
            {
                if (id == 0)
                {
                    shp.shipmentDate = shp.shipmentDate.Date;
                    shp.invoiceDate = shp.invoiceDate.Date;
                    shp.status = shp.plantCode == "4302" ? shipmentStatus.AT_BODY_BUILDER.ToString() : shipmentStatus.IN_PLANT.ToString();
                    _dbc.Add(shp);
                }
                else
                {
                    shp.shipmentDate = shp.shipmentDate.Date;
                    shp.invoiceDate = shp.invoiceDate.Date;

                    _dbc.Update(shp);
                }
                int n = await _dbc.SaveChangesAsync();
                success = (n > 0) ? true : false;
            }
            catch (Exception)
            {
                throw;
            }
            return success;
        }

        public async Task<bool> UpdateShipment(Shipment shp)
        {
            bool success = false;
            try
            {
                _dbc.Update(shp);

                int n = await _dbc.SaveChangesAsync();
                success = (n > 0) ? true : false;
            }
            catch (Exception)
            {
                throw;
            }
            return success;
        }
        public async Task<bool> DeleteShipment(long id, string? user)
        {
            bool success = false;
            try
            {
                var shp = await _dbc.Shipment.FindAsync(id);
                if (shp != null)
                {
                    shp.updatedBy = user;
                    shp.updatedDate = DateTime.Now;
                    shp.status = shipmentStatus.CANCEL_SHIPMENT.ToString();

                    _dbc.Shipment.Update(shp);
                }

                int n = await _dbc.SaveChangesAsync();
                success = (n > 0) ? true : false;

            }
            catch (Exception)
            {
            }
            return success;
        }

        public async Task<Shipment> GetShipmentByNo(string shipmentNo)
        {
            Shipment shp = new Shipment();
            try
            {
                var list = await _dbc.Shipment.ToListAsync();
                if (list.Any())
                {
                    shp = list.Where(o => o.shipmentNo == shipmentNo).FirstOrDefault();
                }
                if (shp == null)
                    shp = new Shipment();
            }
            catch (Exception)
            {
                throw;
            }
            return shp;
        }
        public async Task<Shipment> GetchasisNo(string chasisNo)
        {
            Shipment shp = new Shipment();
            try
            {
                var list = await _dbc.Shipment.ToListAsync();
                if (list.Any())
                {
                    shp = list.Where(o => o.chasisNo == chasisNo).FirstOrDefault();
                }
                if (shp == null)
                    shp = new Shipment();
            }
            catch (Exception)
            {
                throw;
            }
            return shp;
        }

        public async Task<bool> CloseOrUpdateShipment(long id, ShipmentClosed shp, string user)
        {
            DateTime dt = DateTime.Now;
            string fileName = String.Empty;
            bool success = false;
            try
            {
                if (id > 0)
                {
                    Shipment shipment = await _dbc.Shipment.FindAsync(id);

                    if (shipment != null)
                    {
                        if (shp.epodNo != null || shp.status.ToUpper() == shipmentStatus.DELIVERED.ToString())
                        {
                            shipment.epodDate = shp.epodDate;
                            shipment.closingDate = shp.epodDate;
                            shipment.status = shipmentStatus.DELIVERED.ToString();
                        }
                        else
                        {
                            if (shipment.status.Equals(shipmentStatus.BREAKDOWN.ToString()) ||
                                shipment.status.Equals(shipmentStatus.ACCIDENT.ToString()))
                            {
                                if (shipment.incidanceDate != null)
                                {
                                    TimeSpan ts = (TimeSpan)(dt.Date - shipment.incidanceDate.Value.Date);
                                    shipment.reTransitDate = dt;
                                    shipment.incidanceDelayed += ts.Days;
                                }
                            }
                            
                            if(shp.status.Equals(shipmentStatus.EPOD_PENDING.ToString()))
                            {
                                shipment.epodDate = shp.epodDate;
                                shipment.updatedDate = dt;
                            }
                            shipment.status = shp.status.ToString();
                        }
                        shipment.updatedBy = user;
                        _dbc.Update(shipment);
                        int n = await _dbc.SaveChangesAsync();
                        success = (n > 0) ? true : false;

                        if (success)
                        {
                            if (shp.expenses > 0)
                            {
                                Marching m = _dbc.Marching.Where(x => x.shipmentId == shp.shipmentId).FirstOrDefault();
                                if (m != null)
                                {
                                    m.narration = shp.narration;
                                    m.expenses = shp.expenses;
                                    m.updatedDate = dt;
                                    _dbc.Update(m);
                                    n = await _dbc.SaveChangesAsync();
                                    success = (n > 0) ? true : false;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return success;
        }

        public async Task<List<Shipment>> GetShipmentForBilling(string action)
        {
            List<String> status = new List<string>();
            List<Shipment> list = new List<Shipment>();
            try
            {
                switch (action)
                {
                    case "shipment":
                        status = new List<string> {
                                    shipmentStatus.IN_PLANT.ToString()
                            };
                        break;
                    case "plantout":
                        status = new List<string> {
                                    shipmentStatus.IN_PLANT.ToString(),
                                    shipmentStatus.EWAY_EXPIRED.ToString(),
                                    shipmentStatus.DIESEL_SHORTAGE.ToString(),
                                    shipmentStatus.DENT_OR_DAMAGE.ToString(),
                                    shipmentStatus.TOOLKIT_MISSING.ToString(),
                                    shipmentStatus.NOT_IN_RCP.ToString(),
                                    shipmentStatus.TYRE_CUT.ToString(),
                                    shipmentStatus.QUALITY_ISSUE.ToString(),
                                    shipmentStatus.PDI_PENDING.ToString(),
                                    shipmentStatus.PDI_HOLD.ToString(),
                                    shipmentStatus.PAINT_SCRATCHES.ToString(),
                                    shipmentStatus.RUSTY_VEHICLE.ToString(),
                                    shipmentStatus.MIRROR_CRACK_MISSING.ToString(),
                                    shipmentStatus.KEY_MISSING.ToString(),
                                    shipmentStatus.CNG_SHORTAGE.ToString(),
                                    shipmentStatus.DEF_SHORTAGE.ToString(),
                                    shipmentStatus.OTHERS.ToString()
                            };
                        break;
                    case "marching":
                        status = new List<string> {
                                    shipmentStatus.IN_YARD.ToString()
                            };
                        break;
                    case "payment":
                        status = new List<string> {
                                    shipmentStatus.IN_TRANSIT.ToString()
                            };
                        break;
                    case "closing":
                        status = new List<string> {
                                    shipmentStatus.IN_TRANSIT.ToString(),
                                    shipmentStatus.EPOD_PENDING.ToString()
                            };
                        break;
                    case "billing":
                        status = new List<string> {
                                    shipmentStatus.DELIVERED.ToString()
                            };
                        break;
                    case "incidence":
                        status = new List<string> {
                                    shipmentStatus.IN_TRANSIT.ToString()
                            };
                        break;
                }

                if (status.Any())
                {
                    var sl = await _dbc.Shipment.ToListAsync();
                    list = (from s in sl
                            where status.Contains(s.status)
                            select new Shipment
                            {
                                shipmentId = s.shipmentId,
                                shipmentNo = s.shipmentNo,
                                shipmentDate = s.shipmentDate.Date,
                                location = s.location,
                                trasitDays = s.trasitDays,
                                region = s.region,
                                routeCode = s.routeCode,
                                vcNo = s.vcNo,
                                modelDesc = s.modelDesc,
                                mfgCode = s.mfgCode,
                                plantCode = s.plantCode,
                                plantDesc = s.plantDesc,
                                mfgRoute = s.mfgRoute,
                                chasisNo = s.chasisNo,
                                invoiceNo = s.invoiceNo,
                                invoiceDate = s.invoiceDate.Date,
                                basicFreight = s.basicFreight,
                                enRoute = s.enRoute,
                                totalFreight = s.totalFreight,
                                tempRegNo = s.tempRegNo,
                                epodNo = s.epodNo,
                                epodDate = s.epodDate,
                                incidanceDate = s.incidanceDate,
                                closingDate = s.closingDate,
                                reTransitDate = s.reTransitDate,
                                incidanceDelayed = s.incidanceDelayed,
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

        public bool ShipmentIsExist(string shipmentNo,string cmpId)
        {
            int cid= Convert.ToInt32(cmpId);
            bool success = false;
            try
            {
                success = _dbc.Shipment.Any(e => e.chasisNo == shipmentNo.Replace(" ", "") && e.cid == cid);
            }
            catch (Exception)
            {
                success = false;
            }
            return success;
        }

        public async Task<List<Shipment>> GetShipmentForUpdate(string action)
        {
            List<String> status = new List<string>();
            List<Shipment> list = new List<Shipment>();
            try
            {
                switch (action)
                {
                    case "marching":
                        status = new List<string> {
                                    shipmentStatus.IN_YARD.ToString()
                            };
                        break;
                    case "payment":
                        status = new List<string> {
                                    shipmentStatus.IN_TRANSIT.ToString()
                            };
                        break;
                    case "closing":
                        status = new List<string> {
                                    shipmentStatus.IN_TRANSIT.ToString(),
                                    shipmentStatus.EPOD_PENDING.ToString(),
                                    shipmentStatus.BREAKDOWN.ToString(),
                                    shipmentStatus.ACCIDENT.ToString()
                            };
                        break;
                    case "billing":
                        status = new List<string> {
                                    shipmentStatus.DELIVERED.ToString()
                            };
                        break;
                    case "incidence":
                        status = new List<string> {
                                    shipmentStatus.IN_TRANSIT.ToString()
                            };
                        break;
                }

                if (status.Any())
                {
                    var sl = await _dbc.Shipment.Where(x => status.Contains(x.status)).ToListAsync();
                    var dlIds = sl.Select(x => x.dealerId).ToList();
                    var dl = await _dbc.Dealer.Where(x => dlIds.Contains(x.dealerId)).ToListAsync();
                    var deIds = sl.Select(x => x.detsinationId).ToList();
                    var ll = await _dbc.Destination.Where(x => deIds.Contains(x.detsinationId)).ToListAsync();
                    var spIds = sl.Select(x => x.shipmentId).ToList();
                    var ml = await _dbc.Marching.Where(x => spIds.Contains(x.shipmentId)).ToListAsync();
                    //GET Max ewayId record from plantout
                    ml = ml.GroupBy(x => x.shipmentId)
                                    .Select(g => g.OrderByDescending(x => x.voucherNo).FirstOrDefault()).ToList();

                    var drIds = ml.Select(x => x.driverId).ToList();
                    var drl = await _dbc.Driver.Where(x => drIds.Contains(x.driverId)).ToListAsync();
                    var poList = await _dbc.PlantOut.Where(x => spIds.Contains(x.shipmentId)).ToListAsync();

                    //GET Max ewayId record from plantout
                    var pl = poList.GroupBy(x => x.shipmentId)
                                    .Select(g => g.OrderByDescending(x => x.ewayId).FirstOrDefault()).ToList();

                    list = (from s in sl
                            join d in dl on s.dealerId equals d.dealerId
                            join l in ll on s.detsinationId equals l.detsinationId
                            join m in ml on s.shipmentId equals m.shipmentId
                            join dr in drl on m.driverId equals dr.driverId
                            join p in pl on s.shipmentId equals p.shipmentId
                            select new Shipment
                            {
                                shipmentId = s.shipmentId,
                                shipmentNo = s.shipmentNo,
                                shipmentDate = s.shipmentDate.Date,
                                dealerName = d.dealerName,
                                dest = l.destination,
                                location = s.location,
                                trasitDays = s.trasitDays,
                                region = s.region,
                                routeCode = s.routeCode,
                                vcNo = s.vcNo,
                                modelDesc = s.modelDesc,
                                mfgCode = s.mfgCode,
                                plantCode = s.plantCode,
                                plantDesc = s.plantDesc,
                                mfgRoute = s.mfgRoute,
                                chasisNo = s.chasisNo,
                                invoiceNo = s.invoiceNo,
                                ewayno = p.ewayNo,
                                invoiceDate = s.invoiceDate.Date,
                                basicFreight = s.basicFreight,
                                enRoute = s.enRoute,
                                totalFreight = s.totalFreight,
                                tempRegNo = s.tempRegNo,
                                epodNo = s.epodNo,
                                epodDate = s.epodDate,
                                incidanceDate = s.incidanceDate,
                                reTransitDate = s.reTransitDate,
                                incidanceDelayed = s.incidanceDelayed,
                                status = s.status,
                                estimatedDate = p.createdDate.AddDays(l.trasitDays),
                                driverName = dr.driverName,
                                driverMobile = dr.mobileNo,
                                closingDate = s.closingDate,
                                createdDate = s.createdDate,
                                updatedDate = p.createdDate,
                                createdBy = s.createdBy,
                                updatedBy = s.updatedBy
                            }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return list;
        }

        public async Task<(bool, List<BulkShipment>)> AddBulkShipment(BulkInsert modal, string user)
        {
            int cnt = 0;
            bool isInserted = false;
            DateTime date = DateTime.Now;
            bool success = false;
            List<BulkShipment> notAdded = new List<BulkShipment>();
            try
            {
                DataTable dt = GenericFunctions.ReadExcel(modal.dataFile);
                List<BulkShipment> spList = dt.Rows.Count > 0 ? GenericFunctions.ConvertDataTable<BulkShipment>(dt)
                    : new List<BulkShipment>();
                if (spList.Any())
                {
                    var dlList = await _dbc.Dealer.ToListAsync();
                    var dsList = await _dbc.Destination.ToListAsync();
                    var vcList = await _dbc.VcMaster.ToListAsync();
                    var routeList = await _dbc.MfgRoute.ToListAsync();

                    foreach (BulkShipment bs in spList)
                    {
                        isInserted = false;
                        if (!ShipmentIsExist(bs.chasisNo,bs.companyId))
                        {
                            var dlr = dlList.Where(x => x.dealerName.Equals(bs.dealerName)).FirstOrDefault();
                            if (dlr != null)
                            {
                                var des = dsList.Where(x => x.destination.ToLower().StartsWith(bs.destination.ToLower())).FirstOrDefault();
                                if (des != null)
                                {
                                    var vc = vcList.Where(x => x.vcNo.Equals(bs.vcNo)).FirstOrDefault();
                                    if (vc != null)
                                    {
                                        var route = routeList.Where(x => x.mfgRoute.Equals($"{vc.mfgCode}{des.routeCode}")).FirstOrDefault();
                                        if (route != null)
                                        {
                                            if (route != null)
                                            {
                                                Shipment sp = new Shipment()
                                                {
                                                    shipmentNo = bs.shipmentNo,
                                                    tempRegNo = bs.tempRegNo,
                                                    cid = Convert.ToInt32(bs.companyId),
                                                    shipmentDate = Convert.ToDateTime(bs.shipmentDate),
                                                    dealerId = dlr.dealerId,
                                                    dealerName = dlr.dealerName,
                                                    detsinationId = des.detsinationId,
                                                    location = dlr.city,
                                                    trasitDays = des.trasitDays,
                                                    region = des.region,
                                                    routeCode = des.routeCode,
                                                    vcNo = vc.vcNo,
                                                    modelDesc = vc.modelDesc,
                                                    mfgCode = vc.mfgCode,
                                                    plantCode = vc.plantCode,
                                                    plantDesc = vc.plantDesc,
                                                    mfgRoute = route.mfgRoute,
                                                    chasisNo = bs.chasisNo,
                                                    invoiceNo = bs.invoiceNo,
                                                    invoiceDate = Convert.ToDateTime(bs.shipmentDate),
                                                    basicFreight = route.basicRate,
                                                    enRoute = route.inroute,
                                                    totalFreight = route.totalExp,
                                                    createdBy = user,
                                                    updatedBy = user,
                                                    createdDate = date,
                                                    updatedDate = date,
                                                    status = vc.plantCode == "4302" ? shipmentStatus.AT_BODY_BUILDER.ToString() : shipmentStatus.IN_PLANT.ToString()
                                                };
                                                await _dbc.Shipment.AddAsync(sp);

                                                cnt = 1;
                                                isInserted = true;
                                            }
                                            else
                                            {
                                                bs.remark = $"Company Id: {bs.companyId} for shipment No #{bs.shipmentNo}";
                                            }
                                        }
                                        else
                                        {
                                            cnt += 1;
                                            bs.remark = $"MFG-ROUTE #{vc.mfgCode}{des.routeCode} Not fount in a System";
                                        }
                                    }
                                    else
                                    {
                                        cnt += 1;
                                        bs.remark = $"VC No #{bs.vcNo} Not fount in a System";
                                    }
                                }
                                else
                                {
                                    cnt += 1;
                                    bs.remark = $"Destination #{bs.destination} Not fount in a System";
                                }
                            }
                            else
                            {
                                cnt += 1;
                                bs.remark = $"Dealer #{bs.dealerName} Not fount in a System";
                            }
                        }
                        else
                        {
                            cnt += 1;
                            bs.remark = $"Chassis Number #{bs.chasisNo} Already exist in a System";
                        }

                        if (!isInserted)
                        {
                            notAdded.Add(bs);
                        }
                    }

                    int n = await _dbc.SaveChangesAsync();

                    success = (n > 0 | cnt > 0) ? true : false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return (success, notAdded);
        }


        public async Task<List<Shipment>> GetEwayRequestList(ReportModel<Shipment> model)
        {
            List<Shipment> list = new List<Shipment>();
            List<Shipment> sl = new List<Shipment>();
            DateTime fromDate, toDate;
            try
            {
                fromDate = model.fromDate.Value.Date;
                toDate = model.toDate.Value.Date;
                List<string> status = new List<string>()
                {
                    shipmentStatus.AT_BODY_BUILDER.ToString(),
                    shipmentStatus.BREAKDOWN.ToString(),
                    shipmentStatus.ACCIDENT.ToString()
                };
                if (model.ids.Count() > 0)
                {
                    sl = await _dbc.Shipment.Where(x => (status.Contains(x.status) && (model.ids.Contains(x.shipmentId)))).ToListAsync();
                }
                else
                {
                    sl = await _dbc.Shipment.Where(x => (status.Contains(x.status) && (x.shipmentDate.Date >= fromDate && x.shipmentDate.Date <= toDate))).ToListAsync();
                }

                var dlIds = sl.Select(x => x.dealerId).ToList();
                var llIds = sl.Select(x => x.detsinationId).ToList();
                var dl = await _dbc.Dealer.Where(x => dlIds.Contains(x.dealerId)).ToListAsync();
                var ll = await _dbc.Destination.Where(x => llIds.Contains(x.detsinationId)).ToListAsync();
                list = (from s in sl
                        join d in dl on s.dealerId equals d.dealerId
                        join l in ll on s.detsinationId equals l.detsinationId
                        select new Shipment
                        {
                            shipmentId = s.shipmentId,
                            shipmentNo = s.shipmentNo,
                            invoiceNo = s.invoiceNo,
                            invoiceDate = s.invoiceDate,
                            modelDesc = s.modelDesc,
                            chasisNo = s.chasisNo,
                            dealerName = d.dealerName,
                            dest = l.destination
                        }).ToList();
            }
            catch (Exception)
            {
                throw;
            }

            return list;
        }

        private bool isExists(string shipmentNo)
        {
            bool exists = true;
            try
            {
                if (!string.IsNullOrEmpty(shipmentNo))
                {
                    exists = _dbc.Shipment.Any(x => x.shipmentNo.Equals(shipmentNo));
                }
            }
            catch (Exception)
            {
                throw;
            }

            return exists;
        }
        //public int GetStatusCount(string status_, DateTime? startDate, DateTime? endDate)
        //{
        //    string _Status = status_;
        //    DateTime? fromDate = startDate;
        //    DateTime? toDate = endDate;

        //    int count = _dbc.Shipment.Count(i => i.status == _Status &&
        //                                            i.shipmentDate >= fromDate &&
        //                                            i.shipmentDate <= toDate);

        //    return count;
        //}



        public int GetStatusCount(string status, ReportModel<Dashboard> model)
        {

            List<Dashboard> list = new List<Dashboard>();
            int count = 0;
            try
            {
                DateTime financialYearStart = new DateTime(2024, 4, 1); // Assuming financial year starts on April 1st
                DateTime financialYearEnd = new DateTime(2025, 3, 31);

                count = _dbc.Shipment.Count(x => (x.shipmentDate >= financialYearStart && x.shipmentDate <= financialYearEnd && x.status == status));


            }
            catch (Exception)
            {
                throw;
            }

            return count;
        }

       
        public int AllocateCount()
        {
            // Get the current month and year
            DateTime currentDate = DateTime.Now;
            int currentMonth = currentDate.Month;
            int currentYear = currentDate.Year;

            // Filter the shipments that are "IN_PLANT" and match the current month and year
            int allocate_count = _dbc.Shipment.Count(e => e.status == "IN_PLANT" ||
                    e.status == "AT_BODY_BUILDER" ||
                    e.status == "EWAY_EXPIRED" ||
                    e.status == "DIESEL_SHORTAGE" ||
                    e.status == "DENT_OR_DAMAGE" ||
                    e.status == "TOOLKIT_MISSING" ||
                    e.status == "NOT_IN_RCP" ||
                    e.status == "TYRE_CUT" ||
                    e.status == "QUALITY_ISSUE" ||
                    e.status == "PDI_PENDING" ||
                    e.status == "PDI_HOLD" ||
                    e.status == "PAINT_SCRATCHES" ||
                    e.status == "RUSTY_VEHICLE" ||
                    e.status == "MIRROR_CRACK_MISSING" ||
                    e.status == "KEY_MISSING" ||
                    e.status == "CNG_SHORTAGE" ||
                    e.status == "DEF_SHORTAGE" ||
                    e.status == "OTHERS" ||
                    e.status == "BATTERY_DOWN" ||
                    e.status == "BREAK_OIL_LEAKAGE" ||
                    e.status == "PAINT_SCRATCH" ||
                    e.status == "BUMPER_SCRATCH" ||
                    e.status == "CAB_BACK_SIDE_DENT" ||
                    e.status == "CLUTCH_FAIL" ||
                    e.status == "CNG_SHORT" ||
                    e.status == "COOLANT_LEAKAGE_FROM_RADIATOR" ||
                    e.status == "CP_FLAP_DENT" ||
                    e.status == "DENT_ON_BONET" ||
                    e.status == "D30_VEHICLE" ||
                    e.status == "LH_SIDE_DOOR_DENT" ||
                    e.status == "WIPER_SHORT" ||
                    e.status == "DEF_CAP_MISSING" ||
                    e.status == "DENT_ON_FRONT_SHOW" ||
                    e.status == "DIESAL_EXTRA" ||
                    e.status == "HOLD_BY_SECURITY" ||
                    e.status == "DIESAL_LOCK_MISSING" ||
                    e.status == "DIESAL_SHORT" ||
                    e.status == "SIESAL_SHORT_AT_HOSTEL" ||
                    e.status == "DUSTY_VEHICLE" ||
                    e.status == "RH_SIDE_SCRATCHES" ||
                    e.status == "INDICATOR_NOT_WORKING" ||
                    e.status == "RH_AND_LH_SIDE_CAIN_DENT" ||
                    e.status == "BUMPER_PAINT_SCRATCH" ||
                    e.status == "BREAK_LIGHT_NOT_WORKING" ||
                    e.status == "DEF_CAP_AND_WIPER_SHORT" ||
                    e.status == "DOOR_DENT" ||
                    e.status == "RH_REAR_TYRE_CUT" ||
                    e.status == "DIESAL_TANK_SCRATCHES" ||
                    e.status == "LH_SIDE_TAIL_LAMP_SCRATCHES" ||
                    e.status == "EXTRA_KMS" ||
                    e.status == "FRONT_SHOW_DAMAGE" ||
                    e.status == "FRONT_SHOW_PAINT_ISSUE" ||
                    e.status == "EXPORT" ||
                    e.status == "FOOTREST_DAMAGE" ||
                    e.status == "FRONT_GRILL_DAMAGE" ||
                    e.status == "REWORK_NOT_DONE_PROPERLY" ||
                    e.status == "FUSE_COVER_NOT_FITTED_PROPERLY" ||
                    e.status == "STARTING_PROBLEM" ||
                    e.status == "LH_DOOR_DENT" ||
                    e.status == "GEAR_KNOB_MISSING" ||
                    e.status == "HOLD_BY_QA" ||
                    e.status == "COLOR_MISMATCH" ||
                    e.status == "HOLD_BY_SALES_OR_DEALER" ||
                    e.status == "HOLD_FOR_ONLINE_CRTEM" ||
                    e.status == "MIRRIR_DAMAGE" ||
                    e.status == "NOT_LOCATED_IN_HOSTEL" ||
                    e.status == "OK_IN_HOSTEL" ||
                    e.status == "OK_IN_RCP" ||
                    e.status == "RH_SIDE_DOOR_DENT" ||
                    e.status == "TYRE_PUNCTURE" ||
                    e.status == "PAINT_SCRACHES_BACK_SIDE" ||
                    e.status == "REAR_CABIN_DENT" ||
                    e.status == "JACK_MISSING" ||
                    e.status == "RH_SIDE_MIRROR_MISSING" ||
                    e.status == "PDI" ||
                    e.status == "OIL_LEAKAGE" ||
                    e.status == "WRONG_PDI_TAG" ||
                    e.status == "FUSE_COVER_DAMAGE" ||
                    e.status == "SUPD_DAMAGE" ||
                    e.status == "RH_SIDE_DENT" ||
                    e.status == "RH_SIDE_INDICATOR_MISSING" ||
                    e.status == "TAIL_LAMP_CRACK" ||
                    e.status == "FOG_LAMP_REFLECTOR_DAMAGE" ||
                    e.status == "RUPD_DAMAGE" ||
                    e.status == "RUPD_RUSTY" ||
                    e.status == "AIR_LEAKAGE" ||
                    e.status == "ENG_STOP" ||
                    e.status == "WIRING_HARNESS_ISSUE" ||
                    e.status == "WIPER_NOT_WORKING" && e.shipmentDate.Year == currentYear);


            return allocate_count;
        }
        public int DispatchCount()
        {
            // Get the current month and year
            DateTime financialYearStart = new DateTime(2024, 4, 1);
            DateTime financialYearEnd = new DateTime(2025, 3, 31);

            // Filter the shipments that are "DELIVERED" and match the current month and year
            int dispatch_count = _dbc.Shipment.Count(e => e.status == "IN_YARD" &&
                                                     e.shipmentDate >= financialYearStart &&
                                             e.shipmentDate <= financialYearEnd);

            return dispatch_count;
        }
        public int TransDispatchCount()
        {
            // Get the current month and year
            DateTime financialYearStart = new DateTime(2024, 4, 1);
            DateTime financialYearEnd = new DateTime(2025, 3, 31);

            // Filter the shipments that are "DELIVERED" and match the current month and year
            int dispatch_count = _dbc.Shipment.Count(e => e.status == "BILLING_DONE" &&
                                                     e.shipmentDate >= financialYearStart &&
                                             e.shipmentDate <= financialYearEnd);

            return dispatch_count;
        }
        public int QuotationCount()
        {
            // Get the current month and year
            DateTime financialYearStart = new DateTime(2024, 4, 1);
            DateTime financialYearEnd = new DateTime(2025, 3, 31);

            // Filter the shipments that are "DELIVERED" and match the current month and year
            int Quotations_count = _dbc.Quotations.Count(e => e.createdDate >= financialYearStart && e.createdDate <= financialYearEnd);

            return Quotations_count;
        }
        public int DeliveredCount()
        {
            // Get the current month and year
            DateTime financialYearStart = new DateTime(2024, 4, 1);
            DateTime financialYearEnd = new DateTime(2025, 3, 31);

            // Filter the shipments that are "DELIVERED" and match the current month and year
            int del_count = _dbc.Shipment.Count(e => e.status == "DELIVERED" &&
                                                     e.shipmentDate >= financialYearStart &&
                                             e.shipmentDate <= financialYearEnd);

            return del_count;
        }
        public int EPOD_PENDING()
        {
            // Get the current month and year
            DateTime financialYearStart = new DateTime(2024, 4, 1);
            DateTime financialYearEnd = new DateTime(2025, 3, 31);

            // Filter the shipments that are "DELIVERED" and match the current month and year
            int del_count = _dbc.Shipment.Count(e => e.status == "EPOD_PENDING" &&
                                                     e.shipmentDate >= financialYearStart &&
                                             e.shipmentDate <= financialYearEnd);

            return del_count;
        }

        public int InTransit_Count()
        {
            // Get the current month and year
            DateTime financialYearStart = new DateTime(2024, 4, 1);
            DateTime financialYearEnd = new DateTime(2025, 3, 31);

            // Filter the shipments that are "IN_TRANSIT" and match the current month and year
            int intransit_count = _dbc.Shipment.Count(e => e.status == "IN_TRANSIT" &&
                                                    e.shipmentDate >= financialYearStart &&
                                             e.shipmentDate <= financialYearEnd);

            return intransit_count;
        }

    }
}
