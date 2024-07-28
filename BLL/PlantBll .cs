using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Trans9.DataAccess;
using Trans9.Models;

namespace Trans9.BLL
{
    public class PlantBll
    {
        private readonly DataDbContext _context;
        private readonly IHostEnvironment _env;
        private readonly ShipmentBll _spcontext;
        public PlantBll(DataDbContext context, StoredProcedureDbContext spcontext, IHostEnvironment env)
        {
            _context = context;
            _spcontext = new ShipmentBll(context, spcontext, env);
            _env = env;
        }

        public async Task<List<Shipment>> GetPlantList()
        {
            List<string> status = new List<string>();
            List<Shipment> splist = new List<Shipment>();
            try
            {
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
                splist = await _context.Shipment
                    .Where(x => status.Contains(x.status) &&
                           x.shipmentDate >= new DateTime(2024, 04, 01) &&
                           x.shipmentDate <= new DateTime(2025, 3, 31))
                    .ToListAsync();

                if (splist != null && splist.Any())
                {
                    List<Int64> spIds = splist.Select(x => x.shipmentId).ToList();
                    List<Int64> dIds = splist.Select(x => x.detsinationId).ToList();
                    var dlList = await _context.Destination.Where(x => dIds.Contains(x.detsinationId)).ToListAsync();

                    var mrList = await _context.Marching.Where(x => spIds.Contains(x.shipmentId)).ToListAsync();

                    splist = (from sp in splist
                              join dl in dlList on sp.detsinationId equals dl.detsinationId
                              join mr in mrList on sp.shipmentId equals mr.shipmentId into mrJoined
                              from mr in mrJoined.DefaultIfEmpty()
                              join dr in _context.Driver on mr?.driverId equals dr.driverId into drJoined
                              from dr in drJoined.DefaultIfEmpty()
                              select new Shipment()
                              {
                                  shipmentNo = sp.shipmentNo,
                                  shipmentDate = sp.shipmentDate,
                                  createdDate = mr?.createdDate,
                                  dest = dl.destination,
                                  driverName = dr?.driverName,
                                  vcNo = sp.vcNo,
                                  modelDesc = sp.modelDesc,
                                  plantDesc = sp.plantDesc,
                                  invoiceNo = sp.invoiceNo,
                                  chasisNo = sp.chasisNo,
                                  status = sp.status
                              }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return splist;
        }



    }


}
