using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LibreriaDAIR.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CerrarSesion() 
        { 
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}