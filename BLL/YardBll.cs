using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Trans9.DataAccess;
using Trans9.Models;

namespace Trans9.BLL
{
    public class YardBll
    {
        private readonly DataDbContext _context;
        private readonly IHostEnvironment _env;
        private readonly ShipmentBll _spcontext;
        public YardBll(DataDbContext context, StoredProcedureDbContext spcontext, IHostEnvironment env)
        {
            _context = context;
            _spcontext = new ShipmentBll(context, spcontext, env);
            _env = env;
        }

        public async Task<List<Shipment>> GetYardList()
        {
            List<Shipment> splist = new List<Shipment>();
            try
            {
                List<string> status = new List<string> {
            shipmentStatus.IN_YARD.ToString()
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
