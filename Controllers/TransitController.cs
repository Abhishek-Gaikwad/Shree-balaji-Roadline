﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trans9.BLL;
using Trans9.DataAccess;
using Trans9.Models;
using Trans9.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace Trans9.Controllers
{
    [Authorize]
    public class TransitController : Controller
    {
        private readonly TransitBll _context;
        private readonly ShipmentBll _spcontext;
        private readonly IViewRenderService _viewRender;
        private readonly IHostEnvironment _env;
        private readonly string user;

        public TransitController(DataDbContext context, StoredProcedureDbContext spcontext, IViewRenderService renderService, IHttpContextAccessor httpContextAccessor, IHostEnvironment env)
        {
            _context = new TransitBll(context, spcontext, env);
            _spcontext = new ShipmentBll(context, spcontext, env);
            _viewRender = renderService;
            _env = env;
            user = httpContextAccessor.HttpContext.Session.GetString("username");
        }

        // GET: Search
        public async Task<IActionResult> Index()
        {
            ViewBag.PageTitle = "Transit";
            var list = await _context.GetTransitList();
            return View(list);
        }
    }
}
