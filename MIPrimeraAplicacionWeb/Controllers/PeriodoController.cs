using MIPrimeraAplicacionWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MIPrimeraAplicacionWeb.Controllers
{
    public class PeriodoController : Controller
    {
        // GET: Periodo
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ListarPeriodos()
        {
            PruebaDataContext db = new PruebaDataContext();
            var Lista = (db.Periodo.Where(p => p.BHABILITADO.Equals(1))
                .Select(p => new { p.IIDPERIODO, p.NOMBRE, FECHAINICIO = ((DateTime)p.FECHAINICIO).ToShortDateString(), FECHAFIN = ((DateTime)p.FECHAFIN).ToShortDateString() })).ToList();
            return Json(Lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BuscarPeriodoPorNombre(string nombrePeriodo)
        {
            PruebaDataContext bd = new PruebaDataContext();
            var lista = bd.Periodo.Where(p => p.BHABILITADO.Equals(1) && p.NOMBRE.Contains(nombrePeriodo))
                .Select(p => new { p.IIDPERIODO, p.NOMBRE, FECHAINICIO = ((DateTime)p.FECHAINICIO).ToShortDateString(), FECHAFIN = ((DateTime)p.FECHAFIN).ToShortDateString() }).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public int EliminarPeriodo (Periodo periodito)
        {
            PruebaDataContext bd = new PruebaDataContext();
            int numRegisAfectados = 0;
            try
            {
                Periodo periodoSel = (bd.Periodo.Where(p => p.IIDPERIODO.Equals(periodito.IIDPERIODO))).First();
                periodoSel.BHABILITADO = 0;
                bd.SubmitChanges();
                numRegisAfectados = 1;
            }
            catch (Exception e) { numRegisAfectados = 0; throw e; }

            return numRegisAfectados;
        }

        public JsonResult RecuperarDatos(int id)
        {
            PruebaDataContext bd = new PruebaDataContext();
            var lista = (bd.Periodo.Where(p => p.BHABILITADO.Equals(1) && p.IIDPERIODO.Equals(id))
                .Select(p => new {p.IIDPERIODO, p.NOMBRE, FECHAINICIO = ((DateTime)p.FECHAINICIO).ToShortDateString(), FECHAFIN = ((DateTime)p.FECHAFIN).ToShortDateString() })).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
            
        }

        public int GuardarDatos(Periodo periodito)
        {
            PruebaDataContext bd = new PruebaDataContext();
            int numRegisAfectados = 0;
            try
            {
                if (periodito.IIDPERIODO == 0)
                {
                    bd.Periodo.InsertOnSubmit(periodito);
                    bd.SubmitChanges();
                    numRegisAfectados = 1;
                }
                else
                {
                    Periodo periodoSel = (bd.Periodo.Where(p => p.IIDPERIODO.Equals(periodito.IIDPERIODO))).First();
                    periodoSel.NOMBRE = periodito.NOMBRE;
                    periodoSel.FECHAINICIO = periodito.FECHAINICIO;
                    periodoSel.FECHAFIN = periodito.FECHAFIN;
                    bd.SubmitChanges();
                    numRegisAfectados = 1;
                }
            }catch(Exception e)
            { 
                numRegisAfectados = 0;
                throw e;
            }

            return numRegisAfectados;
        }
    }
}