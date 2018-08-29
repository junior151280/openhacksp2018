using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OpenHackWebsite.Controllers
{
    public class AdmController : Controller
    {
        // GET: Index
        public ActionResult Index()
        {
            return View("index");
        }
    }
}