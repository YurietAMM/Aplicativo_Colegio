using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MIPrimeraAplicacionWeb.Controllers
{
    public class DocenteController : Controller
    {
        // GET: Docente
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ListarDocentes() { 
            PruebaDataContext bd = new PruebaDataContext();
            var lista = (bd.Docente.Where(p => p.BHABILITADO.Equals(1))
                .Select(p => new { p.IIDDOCENTE, p.NOMBRE, p.EMAIL, p.TELEFONOCELULAR})).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LlenarCombo()
        {
            PruebaDataContext bd = new PruebaDataContext();
            var Lista = (bd.ModalidadContrato.Where(p => p.BHABILITADO.Equals(1))
                .Select(p => new { IID = p.IIDMODALIDADCONTRATO , p.NOMBRE } )).ToList();
            return Json(Lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarDocentesPorModalidad(int modo)
        {
            PruebaDataContext bd = new PruebaDataContext();
            var lista = (bd.Docente.Where(p => p.BHABILITADO.Equals(1) && p.IIDMODALIDADCONTRATO.Equals(modo))
                .Select(p => new { p.IIDDOCENTE, p.NOMBRE, p.EMAIL, p.TELEFONOCELULAR })).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LlenarComboSexo()
        {
            PruebaDataContext bd = new PruebaDataContext();
            var Lista = (bd.Sexo.Where(p => p.BHABILITADO.Equals(1))
                .Select(p => new { IID = p.IIDSEXO, p.NOMBRE })).ToList();
            return Json(Lista, JsonRequestBehavior.AllowGet);
        }

        public int EliminarDocente(Docente docentito)
        {
            PruebaDataContext bd = new PruebaDataContext();
            int numRegisAfectados = 0;
            try
            {
                Docente docenteSel = (bd.Docente.Where(p => p.IIDDOCENTE.Equals(docentito.IIDDOCENTE))).First();
                docenteSel.BHABILITADO = 0;
                bd.SubmitChanges();
                numRegisAfectados = 1;
            }
            catch (Exception e) { numRegisAfectados = 0; throw e; }

            return numRegisAfectados;
        }
    }
}