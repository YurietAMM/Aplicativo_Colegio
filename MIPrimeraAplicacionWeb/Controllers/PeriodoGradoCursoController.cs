using MIPrimeraAplicacionWeb.Filters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MIPrimeraAplicacionWeb.Controllers
{
    [Seguridad]
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
                        where pgc.BHABILITADO.Equals(1)
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

        public int GuardarDatos(PeriodoGradoCurso pgc)
        {
            PruebaDataContext bd = new PruebaDataContext();
            int numRegisAfectados = 0;

            try
            {
                if (pgc.IID == 0)
                {
                    int numVeces = bd.PeriodoGradoCurso.Where(p => p.IIDPERIODO.Equals(pgc.IIDPERIODO) 
                    && p.IIDGRADO.Equals(pgc.IIDGRADO) && p.IIDCURSO.Equals(pgc.IIDCURSO)).Count();

                    if(numVeces == 0)
                    {
                        bd.PeriodoGradoCurso.InsertOnSubmit(pgc);
                        bd.SubmitChanges();
                        numRegisAfectados = 1;
                    }
                    else
                    {
                        numRegisAfectados = -1;
                    }
                }
                else
                {
                    int numVeces = bd.PeriodoGradoCurso.Where(p => p.IIDPERIODO.Equals(pgc.IIDPERIODO)
                    && p.IIDGRADO.Equals(pgc.IIDGRADO) && p.IIDCURSO.Equals(pgc.IIDCURSO)
                    && !p.IID.Equals(pgc.IID)).Count();

                    if(numVeces == 0)
                    {
                        PeriodoGradoCurso pgcGuardado = bd.PeriodoGradoCurso.Where(p => p.IID.Equals(pgc.IID)).First();
                        pgcGuardado.IIDGRADO = pgc.IIDGRADO;
                        pgcGuardado.IIDPERIODO = pgc.IIDPERIODO;
                        pgcGuardado.IIDCURSO = pgc.IIDCURSO;
                        bd.SubmitChanges();
                        numRegisAfectados = 1;
                    }
                    else
                    {
                        numRegisAfectados = -1;
                    }
                }
            }
            catch (Exception ex) { numRegisAfectados = 0; throw ex; }

            return numRegisAfectados;
        }

        public int Eliminar(int id)
        {
            PruebaDataContext bd = new PruebaDataContext();
            int numRegisAfectados = 0;
            try
            {
                PeriodoGradoCurso seleccionado = (bd.PeriodoGradoCurso.Where(p => p.IID.Equals(id))).First();
                seleccionado.BHABILITADO = 0;
                bd.SubmitChanges();
                numRegisAfectados = 1;
            }
            catch (Exception e) { numRegisAfectados = 0; throw e; }

            return numRegisAfectados;
        }
    }
}