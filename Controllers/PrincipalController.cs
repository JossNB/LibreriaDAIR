using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LibreriaDAIR.Controllers
{
    public class PrincipalController : Controller
    {
        // GET: Principal
        public ActionResult Index()
        {
            if (Session["Nombre"] == null) 
            {
                return RedirectToAction("Index", "Login");
            }

            ViewBag.Nombre = Session["Nombre"]; //Mostrará el nombre de la persona que inicia sesión
            return View();
        }

        public ActionResult Servicios() 
        { 
            return View();
        }
      

    }
}