using MIPrimeraAplicacionWeb.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MIPrimeraAplicacionWeb.Controllers
{
    public class PaginaPrincipalController : Controller
    {
        // GET: PaginaPrincipal
        public ActionResult Index()
        {
            int IdUsuario = (int)Session["IdUsuario"];

            using (PruebaDataContext bd = new PruebaDataContext())
            {
                string nombreCompleto;
                Usuario usuarioActual = bd.Usuario.Where(p => p.IIDUSUARIO.Equals(IdUsuario)).First();
                if (usuarioActual.TIPOUSUARIO.Equals('D'))
                {
                    Docente obj = bd.Docente.Where(p => p.IIDDOCENTE.Equals(usuarioActual.IID)).First();

                    nombreCompleto = obj.NOMBRE;
                }
                else
                {
                    Alumno obj = bd.Alumno.Where(p => p.IIDALUMNO.Equals(usuarioActual.IID)).First();

                    nombreCompleto = "Profesor@ " + obj.NOMBRE;
                }
                ViewBag.nombreCompleto = nombreCompleto;
            }
            return View();
        }
    }
}