using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;
using Trans9.DataAccess;
using Trans9.Models;

namespace Trans9.BLL
{
    public class DataLoader
    {
        public async static Task<List<DropDown>> GetDealerDropDown(DataDbContext context)
        {
            List<DropDown> ddl = new List<DropDown>();
            try
            {
                var list = await context.Dealer.Where(o => o.status == comonStatus.active.ToString()).ToListAsync();
                if (list.Any())
                {
                    ddl = list
                        .Select(o => new DropDown { Id = o.dealerId, Name = o.dealerName }).ToList();
                }
                ddl.Insert(0, new DropDown { Id = "0", Name = " --- select ---" });
            }
            catch (Exception)
            {
            }
            return ddl;
        }

        public async static Task<List<DropDown>> GetDestinationDropDown(DataDbContext context, string all = "")
        {
            List<DropDown> ddl = new List<DropDown>();
            try
            {
                string name = string.IsNullOrEmpty(all) ? " --- select --- " : " All";
                int id = string.IsNullOrEmpty(all) ? -1 : 0;
                var list = await context.Destination.Where(o => o.status == comonStatus.active.ToString()).ToListAsync();
                if (list.Any())
                {
                    ddl = list.Select(o => new DropDown { Id = o.detsinationId, Name = o.destination }).ToList();
                }
                ddl.Insert(0, new DropDown { Id = id, Name = name });
            }
            catch (Exception)
            {
            }
            return ddl;
        }

        public async static Task<List<DropDown>> GetVCDropDown(StoredProcedureDbContext spc, string quoteId = null)
        {
            List<DropDown> ddl = new List<DropDown>();
            try
            {
                var list = await spc.VcInfo
                        .FromSqlRaw("CALL `sbl_getVcInfoList`({0},{1})", quoteId, null)
                        .IgnoreQueryFilters()
                        .ToListAsync();

                if (list.Any())
                {
                    ddl = list.Select(o => new DropDown
                    {
                        Id = o.vcNo,
                        Name = $"{o.vcNo}-{o.modelDesc}"
                    }).ToList();
                }
                ddl.Insert(0, new DropDown { Id = "", Name = " --- select ---" });
            }
            catch (Exception)
            {
            }
            return ddl;
        }

        public async static Task<List<DropDown>> GetDealerTypeDropDown()
        {
            List<DropDown> ddl = new List<DropDown>();
            try
            {
                var list = Enum.GetNames(typeof(DealerType)).ToList();
                if (list.Any())
                {
                    ddl = list.Select(o => new DropDown
                    {
                        Id = o,
                        Name = o
                    }).ToList();
                }
                ddl.Insert(0, new DropDown { Id = "", Name = " --- select ---" });
            }
            catch (Exception)
            {
            }
            return ddl;
        }

        public async static Task<List<DropDown>> GetDriverStatusDropDown(bool isReport = false)
        {
            List<DropDown> ddl = new List<DropDown>();
            try
            {
                var list = Enum.GetNames(typeof(driverStatus)).ToList();
                if (isReport)
                    list.Insert(0, "all");
                if (list.Any())
                {
                    ddl = list.Select(o => new DropDown
                    {
                        Id = o,
                        Name = o
                    }).ToList();
                }
                ddl.Insert(0, new DropDown { Id = "", Name = " --- select ---" });
            }
            catch (Exception)
            {
            }
            return ddl;
        }

        public async static Task<List<DropDown>> GetEwayDropDown()
        {
            List<DropDown> ddl = new List<DropDown>();
            try
            {
                var list = Enum.GetNames(typeof(ewayStatus)).ToList();
                if (list.Any())
                {
                    ddl = list.Select(o => new DropDown
                    {
                        Id = o,
                        Name = o
                    }).ToList();
                }
                ddl.Insert(0, new DropDown { Id = "", Name = " --- select ---" });
            }
            catch (Exception)
            {
            }
            return ddl;
        }

        public async static Task<List<DropDown>> GetPumpDropDown(PumpMasterBll context, string all = "")
        {
            List<DropDown> ddl = new List<DropDown>();
            try
            {
                var list = context.GetPumpList().Result;
                if (list.Any())
                {
                    ddl = list.Select(o => new DropDown
                    {
                        Id = o.pumpId,
                        Name = o.pumpName
                    }).ToList();
                }
                if (string.IsNullOrEmpty(all))
                    ddl.Insert(0, new DropDown { Id = 0, Name = " --- select ---" });
                else
                    ddl.Insert(0, new DropDown { Id = 0, Name = " All" });
            }
            catch (Exception)
            {
            }
            return ddl;
        }

        public async static Task<List<DropDown>> GetLoadingChargesDropDown()
        {
            List<DropDown> ddl = new List<DropDown>();
            try
            {
                /*var list = Enum.GetNames(typeof(ewayStatus)).ToList();
                if (list.Any())
                {
                    ddl = list.Select(o => new DropDown
                    {
                        Id = o,
                        Name = o
                    }).ToList();
                }*/
                ddl.Insert(0, new DropDown { Id = "", Name = " --- select ---" });
            }
            catch (Exception)
            {
            }
            return ddl;
        }

        public static async Task<PlantOut> GetPlanOutInfo(ShipmentBll _spcontext, Int64 shipmentId)
        {
            DateTime dt = DateTime.Now;
            PlantOut po = new PlantOut();
            try
            {
                var shipment = await _spcontext.GetShipmentById(shipmentId);
                if (shipment != null)
                {
                    po.shipmentId = shipment.shipmentId;
                    po.shipmentNo = shipment.shipmentNo;
                    po.modelDesc = shipment.modelDesc;
                    po.chasisNo = shipment.chasisNo;
                    po.dest = shipment.dest;
                    po.spStatus = shipment.status;
                    po.status = shipment.status;
                    po.updatedDate = dt;
                }

                po.updatedDate = dt.Date;
                po.createdDate = dt.Date;
                po.actualPlantOut = dt.Date;
            }
            catch (Exception)
            {
            }
            return po;
        }

        public async static Task<List<DropDown>> GetShipmentStatusDropDown(string action, bool isReport = false)
        {
            List<DropDown> ddl = new List<DropDown>();
            try
            {
                List<String> list = new List<string>();
                switch (action)
                {
                    case "shipment":
                        list = new List<string> {
                                    shipmentStatus.IN_PLANT.ToString()
                            };
                        break;
                    case "plantout":
                        list = new List<string> {
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
                                    shipmentStatus.CANCEL_SHIPMENT.ToString(),
                                    shipmentStatus.WIPER_NOT_WORKING.ToString()
                            };
                        break;
                    case "marching":
                        list = new List<string> {
                                    shipmentStatus.IN_YARD.ToString()
                            };
                        break;
                    case "payment":
                        list = new List<string> {
                                    shipmentStatus.IN_TRANSIT.ToString()
                            };
                        break;
                    case "closing":
                        list = new List<string> {
                                    shipmentStatus.IN_TRANSIT.ToString(),
                                    shipmentStatus.EPOD_PENDING.ToString(),
                                    shipmentStatus.DELIVERED.ToString()
                            };
                        break;
                    case "billing":
                        list = new List<string> {
                                    shipmentStatus.DELIVERED.ToString()
                            };
                        break;
                    case "all":
                        list = new List<string> {
                            "all",
                            shipmentStatus.IN_PLANT.ToString(),
                            shipmentStatus.APPROVALPENDING.ToString(),
                            shipmentStatus.EWAY_EXPIRED.ToString(),
                            shipmentStatus.DIESEL_SHORTAGE.ToString(),
                            shipmentStatus.DENT_OR_DAMAGE.ToString(),
                            shipmentStatus.TOOLKIT_MISSING.ToString(),
                            shipmentStatus.NOT_IN_RCP.ToString(),
                            shipmentStatus.TYRE_CUT.ToString(),
                            shipmentStatus.QUALITY_ISSUE.ToString(),
                            shipmentStatus.OTHERS.ToString(),
                            shipmentStatus.IN_YARD.ToString(),
                            shipmentStatus.ACCIDENT.ToString(),
                            shipmentStatus.BREAKDOWN.ToString(),
                            shipmentStatus.CANCEL_SHIPMENT.ToString(),
                            shipmentStatus.IN_TRANSIT.ToString(),
                            shipmentStatus.EPOD_PENDING.ToString(),
                            shipmentStatus.DELIVERED.ToString(),
                            shipmentStatus.BILLING_DONE.ToString()
                            };
                        break;
                }

                if (list.Any())
                {
                    ddl = list.Select(o => new DropDown
                    {
                        Id = o,
                        Name = o.Replace("_", " ").Replace(" or ", "/")
                    }).ToList();
                }

                if (!isReport)
                {
                    ddl.Insert(0, new DropDown { Id = "", Name = " --- select ---" });
                }
            }
            catch (Exception)
            {
            }
            return ddl;
        }

        public async static Task<List<DropDown>> GetVoucherDropDown(DataDbContext _context, string action)
        {
            List<DropDown> ddl = new List<DropDown>();
            try
            {
                var list = await _context.Marching.Where(x => x.remainBalance > 0).ToListAsync();

                if (list.Any())
                {
                    ddl = (from m in list
                           select new DropDown
                           {
                               Id = m.voucherNo,
                               Name = $"{m.voucherNo}"
                           }).ToList();
                }
                ddl.Insert(0, new DropDown { Id = "0", Name = " --- select ---" });
            }
            catch (Exception)
            {
            }
            return ddl;

        }

        public async static Task<List<DropDown>> GetUserRolesDropDown(AppUserBll context)
        {
            List<DropDown> ddl = new List<DropDown>();
            try
            {
                var list = await context.GetUserRoles();

                if (list.Any())
                {
                    ddl = list.Select(o => new DropDown
                    {
                        Id = o,
                        Name = o.ToString()
                    }).ToList();
                }
                ddl.Insert(0, new DropDown { Id = "", Name = " --- select ---" });
            }
            catch (Exception)
            {
            }
            return ddl;
        }

        public async static Task<List<DropDown>> GetPagesDropDownList(AppUserBll context)
        {
            List<DropDown> ddl = new List<DropDown>();
            try
            {
                var list = await context.GetPages();

                if (list.Any())
                {
                    ddl = list.Select(o => new DropDown
                    {
                        Id = o.id,
                        Name = o.pageName
                    }).ToList();
                }
                ddl.Insert(0, new DropDown { Id = "", Name = " --- select ---" });
            }
            catch (Exception)
            {
            }
            return ddl;
        }

        public async static Task<List<DropDown>> GetDriverDropDown(DriverBll _context)
        {
            List<DropDown> ddl = new List<DropDown>();
            try
            {
                var list = await _context.GetDriverList();

                if (list.Any())
                {
                    ddl = list.Select(o => new DropDown
                    {
                        Id = o.driverId,
                        Name = o.driverName
                    }).ToList();
                }
                ddl.Insert(0, new DropDown { Id = 0, Name = "-- select ---" });
            }
            catch (Exception)
            {
            }
            return ddl;
        }
        public async static Task<List<DropDown>> GetPayModeDropDown()
        {
            List<DropDown> ddl = new List<DropDown>();
            try
            {
                ddl.Add(new DropDown { Id = "all", Name = "all" });
                ddl.Add(new DropDown { Id = "other", Name = "other" });
                ddl.Add(new DropDown { Id = "cash", Name = "cash" });
                ddl.Add(new DropDown { Id = "upi", Name = "UPI" });
                ddl.Insert(0, new DropDown { Id = 0, Name = "-- select ---" });
            }
            catch (Exception)
            {
            }
            return ddl;
        }

        public static async Task<List<DropDown>> GetBillReferenceDropDown(BillingBll context)
        {
            List<DropDown> ddl = new List<DropDown>();
            try
            {
                var list = await context.GetBillReferenceList();

                if (list.Any())
                {
                    ddl = list.Select(o => new DropDown
                    {
                        Id = o.billGrp,
                        Name = $"{o.billGrp}-{o.billingDate.Date.ToString("dd/MM/yyyy")}"
                    }).ToList();
                }
                ddl.Insert(0, new DropDown { Id = 0, Name = "-- select ---" });
            }
            catch (Exception)
            {
            }
            return ddl;
        }

        public static List<DropDown> GetChassisDropdown(DataDbContext _context)
        {

            List<DropDown> ddl = new List<DropDown>();
            try
            {
                List<string> status = new List<string>()
                {
                    shipmentStatus.AT_BODY_BUILDER.ToString(),
                    shipmentStatus.BREAKDOWN.ToString(),
                    shipmentStatus.ACCIDENT.ToString()
                };

                var list = _context.Shipment.Where(x => status.Contains(x.status)).ToListAsync().Result;

                if (list.Any())
                {
                    ddl = list.Select(o => new DropDown
                    {
                        Id = o.shipmentId,
                        Name = o.chasisNo
                    }).ToList();
                }
                ddl.Insert(0, new DropDown { Id = 0, Name = "-- select ---" });
            }
            catch (Exception)
            {
            }
            return ddl;
        }

        public static List<DropDown> GetCompanyDropdown(DataDbContext _context)
        {

            List<DropDown> ddl = new List<DropDown>();
            try
            {
                var list = _context.Company.ToListAsync().Result;

                if (list.Any())
                {
                    ddl = list.Select(o => new DropDown
                    {
                        Id = o.cid,
                        Name = o.companyName
                    }).ToList();
                }
                else
                {
                    ddl.Insert(0, new DropDown { Id = 1, Name = "Default" });
                }
            }
            catch (Exception)
            {
            }
            return ddl;
        }


        public static List<DropDown> GetQuotationDropdown(DataDbContext _context)
        {
            List<DropDown> ddl = new List<DropDown>();
            try
            {
                var list = _context.Quotations.ToListAsync().Result;

                if (list.Any())
                {
                    ddl = list.Select(o => new DropDown
                    {
                        Id = o.quoteId,
                        Name = o.quoteNo
                    }).ToList();
                }
                ddl.Insert(0, new DropDown { Id = "-1", Name = " --- select ---" });
            }
            catch (Exception)
            {
            }
            return ddl;
        }

        public static async Task<List<DropDown>> GetDriverInchargeDropDown(DataDbContext _context)
        {
            List<DropDown> ddl = new List<DropDown>();
            try
            {
                var vList = await _context.InchargePayment.OrderByDescending(x => x.createdDate).ToListAsync();
                if (vList.Any())
                {
                    var dIds = vList.DistinctBy(x => x.inchargeId).Select(x => x.inchargeId).ToList();
                    var dList = await _context.Driver.Where(x => dIds.Contains(x.driverId)).ToListAsync();

                    ddl = (from s in dList
                             where (s.status.Equals(driverStatus.active.ToString())
                             || s.status.Equals(driverStatus.blocked.ToString()))
                            orderby s.driverId descending
                            select new DropDown
                            {
                                Id = s.driverId,
                                Name = s.driverName
                            }
                            ).ToList();
                }

                ddl.Insert(0, new DropDown { Id = 0, Name = "-- select ---" });
            }
            catch (Exception)
            {
            }
            return ddl;
        }

        public static async Task<List<DropDown>> GetEmployeeDropDown(DataDbContext context, bool isAll=false)
        {
            List<DropDown> ddl = new List<DropDown>();
            try
            {
                var list = await context.Employees.Where(o => o.status == comonStatus.active.ToString()).ToListAsync();
                if (list.Any())
                {
                    ddl = list
                        .Select(o => new DropDown { Id = o.employeeId, Name = $"{o.firstName} {o.lastName}" }).ToList();
                }

                if (!isAll)
                {
                    ddl.Insert(0, new DropDown { Id = -1, Name = " --- select ---" });
                }
                else {
                    ddl.Insert(0, new DropDown { Id = 0, Name = "ALL" });
                }
            }
            catch (Exception)
            {
            }
            return ddl;
        }
    }
}
