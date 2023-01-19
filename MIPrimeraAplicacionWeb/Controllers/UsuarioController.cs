using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MIPrimeraAplicacionWeb.Controllers
{
    public class UsuarioController : Controller
    {
        // GET: Usuario
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ListarRoles()
        {
            PruebaDataContext bd = new PruebaDataContext();

            var lista = bd.Rol.Where(p => p.BHABILITADO.Equals(1)).Select(p => new
            {
                IID = p.IIDROL,
                p.NOMBRE
            }).ToList();

            return Json(lista, JsonRequestBehavior.AllowGet);
        }
    }
}