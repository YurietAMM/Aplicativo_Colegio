using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MIPrimeraAplicacionWeb.Controllers
{
    public class PeriodoGradoCursoController : Controller
    {
        // GET: PeriodoGradoCurso
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ListarPeriodoGradoCurso()
        {
            PruebaDataContext bd = new PruebaDataContext();

            var lista = from pgc in bd.PeriodoGradoCurso
                        join per in bd.Periodo
                        on pgc.IIDPERIODO equals per.IIDPERIODO
                        join grad in bd.Grado
                        on pgc.IIDGRADO equals grad.IIDGRADO
                        join cur in bd.Curso
                        on pgc.IIDCURSO equals cur.IIDCURSO
                        select new
                        {
                            pgc.IID,
                            NombrePeriodo = per.NOMBRE,
                            NombreGrado = grad.NOMBRE,
                            NombreCurso = cur.NOMBRE
                        };

            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RecuperarDatos(int id)
        {
            PruebaDataContext bd = new PruebaDataContext();

            var consulta = (bd.PeriodoGradoCurso.Where(p => p.IID.Equals(id)))
                .Select(p => new { p.IID, p.IIDCURSO, p.IIDPERIODO, p.IIDGRADO });

            return Json(consulta, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarPeriodo()
        {
            PruebaDataContext bd = new PruebaDataContext();

            var lista = (bd.Periodo.Where(p => p.BHABILITADO.Equals(1))
                .Select(p => new
                {
                    IID = p.IIDPERIODO,
                    p.NOMBRE
                })).ToList();

            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarCurso()
        {
            PruebaDataContext bd = new PruebaDataContext();

            var lista = (bd.Curso.Where(p => p.BHABILITADO.Equals(1))
                .Select(p => new
                {
                    IID = p.IIDCURSO,
                    p.NOMBRE
                })).ToList();

            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarGrado()
        {
            PruebaDataContext bd = new PruebaDataContext();

            var lista = (bd.Grado.Where(p => p.BHABILITADO.Equals(1))
                .Select(p => new
                {
                    IID = p.IIDGRADO,
                    p.NOMBRE
                })).ToList();

            return Json(lista, JsonRequestBehavior.AllowGet);
        }


    }
}