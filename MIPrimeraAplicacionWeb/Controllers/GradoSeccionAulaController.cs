using MIPrimeraAplicacionWeb.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace MIPrimeraAplicacionWeb.Controllers
{
    [Seguridad]
    public class GradoSeccionAulaController : Controller
    {
        // GET: GradoSeccionAula
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ListarGradoSeccionAula()
        {
            PruebaDataContext bd = new PruebaDataContext();

            var lista = from tabla in bd.GradoSeccionAula
                        join periodo in bd.Periodo
                        on tabla.IIDPERIODO equals periodo.IIDPERIODO
                        join gradoSeccion in bd.GradoSeccion
                        on tabla.IIDGRADOSECCION equals gradoSeccion.IID
                        join docente in bd.Docente
                        on tabla.IIDDOCENTE equals docente.IIDDOCENTE
                        join curso in bd.Curso
                        on tabla.IIDCURSO equals curso.IIDCURSO
                        join grado in bd.Grado
                        on gradoSeccion.IIDGRADO equals grado.IIDGRADO
                        where tabla.BHABILITADO.Equals(1)
                        select new
                        {
                            tabla.IID,
                            NOMBREPERIODO = periodo.NOMBRE,
                            NOMBRECURSO = curso.NOMBRE,
                            NOMBREDOCENTE = docente.NOMBRE,
                            NOMBREGRADO = grado.NOMBRE
                        };

            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarPeriodo()
        {
            PruebaDataContext bd = new PruebaDataContext();

            var lista = (bd.Periodo.Where(p => p.BHABILITADO.Equals(1))
                .Select(p => new {
                    IID = p.IIDPERIODO,
                    p.NOMBRE
                }).ToList());

            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarGradoSeccion()
        {
            PruebaDataContext bd = new PruebaDataContext();

            var lista = from gs in bd.GradoSeccion
                        join grado in bd.Grado
                        on gs.IIDGRADO equals grado.IIDGRADO
                        join seccion in bd.Seccion
                        on gs.IIDSECCION equals seccion.IIDSECCION
                        select new
                        {
                            gs.IID,
                            NOMBRE = grado.NOMBRE + " - " + seccion.NOMBRE
                        };

            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarAula()
        {
            PruebaDataContext bd = new PruebaDataContext();

            var lista = (bd.Aula.Where(p => p.BHABILITADO.Equals(1))
                .Select(p => new {
                    IID = p.IIDAULA,
                    p.NOMBRE
                }).ToList());

            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarDocente()
        {
            PruebaDataContext bd = new PruebaDataContext();

            var lista = (bd.Docente.Where(p => p.BHABILITADO.Equals(1))
                .Select(p => new {
                    IID = p.IIDDOCENTE,
                    p.NOMBRE
                }).ToList());

            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarCurso (int IIDPERIODO, int IIDGRADOSECCION)
        {
            PruebaDataContext bd = new PruebaDataContext();
            
                int iidGrado = (int)bd.GradoSeccion.Where(p => p.IID.Equals(IIDGRADOSECCION)).First().IIDGRADO;

                var lista = from pgc in bd.PeriodoGradoCurso
                            join curso in bd.Curso
                            on pgc.IIDCURSO equals curso.IIDCURSO
                            join periodo in bd.Periodo
                            on pgc.IIDPERIODO equals periodo.IIDPERIODO
                            where pgc.BHABILITADO.Equals(1) && pgc.IIDPERIODO.Equals(IIDPERIODO)
                            && pgc.IIDGRADO.Equals(iidGrado)
                            select new
                            {
                                IID = pgc.IIDCURSO,
                                curso.NOMBRE
                            };
            
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RecuperarDatos(int id)
        {
            PruebaDataContext bd = new PruebaDataContext();

            var consulta = (bd.GradoSeccionAula.Where(p => p.IID.Equals(id)))
                .Select(p => new { p.IID, p.IIDPERIODO, p.IIDCURSO, p.IIDDOCENTE, p.IIDGRADOSECCION, p.IIDAULA });

            return Json(consulta, JsonRequestBehavior.AllowGet);
        }

        public int GuardarDatos(GradoSeccionAula gsa)
        {
            PruebaDataContext bd = new PruebaDataContext();
            int numRegisAfectados = 0;

            try
            {
                if (gsa.IID.Equals(0))
                {
                    int numVeces = bd.GradoSeccionAula.Where(p => p.IIDPERIODO.Equals(gsa.IIDPERIODO)
                    && p.IIDGRADOSECCION.Equals(gsa.IIDGRADOSECCION) && p.IIDCURSO.Equals(gsa.IIDCURSO)).Count();

                    if(numVeces == 0)
                    {
                        bd.GradoSeccionAula.InsertOnSubmit(gsa);
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
                    int numVeces = bd.GradoSeccionAula.Where(p => p.IIDPERIODO.Equals(gsa.IIDPERIODO)
                    && p.IIDGRADOSECCION.Equals(gsa.IIDGRADOSECCION) && p.IIDCURSO.Equals(gsa.IIDCURSO)
                    && !p.IID.Equals(gsa.IID)).Count();

                    if(numVeces == 0)
                    {
                        GradoSeccionAula gsaGuardado = bd.GradoSeccionAula.Where(p => p.IID.Equals(gsa.IID)).First();
                        gsaGuardado.IIDPERIODO = gsa.IIDPERIODO;
                        gsaGuardado.IIDCURSO = gsa.IIDCURSO;
                        gsaGuardado.IIDDOCENTE = gsa.IIDDOCENTE;
                        gsaGuardado.IIDGRADOSECCION = gsa.IIDGRADOSECCION;
                        gsaGuardado.IIDAULA = gsa.IIDAULA;
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
                GradoSeccionAula seleccionado = (bd.GradoSeccionAula.Where(p => p.IID.Equals(id))).First();
                seleccionado.BHABILITADO = 0;
                bd.SubmitChanges();
                numRegisAfectados = 1;
            }
            catch (Exception e) { numRegisAfectados = 0; throw e; }

            return numRegisAfectados;
        }
    }
}