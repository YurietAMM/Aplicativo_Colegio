using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Transactions;

namespace MIPrimeraAplicacionWeb.Controllers
{
    public class MatriculaController : Controller
    {
        // GET: Matricula
        public ActionResult Index()
        {
            return View();
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

        public JsonResult ListarAlumnos()
        {
            PruebaDataContext bd = new PruebaDataContext();

            var lista = (bd.Alumno.Where(p => p.BHABILITADO.Equals(1))
                .Select(p => new {
                    IID = p.IIDALUMNO,
                    NOMBRE = p.NOMBRE + " " + p.APPATERNO + " " + p.APMATERNO
                }).ToList());

            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public int GuardarDatos(Matricula gsa, int IIDGRADOSECCION)
        {
            PruebaDataContext bd = new PruebaDataContext();
            int numRegisAfectados = 0;

            GradoSeccion obj = bd.GradoSeccion.Where(p => p.IID.Equals(IIDGRADOSECCION)).First();

            int IIDGrado = (int) obj.IIDGRADO;
            int IIDSeccion = (int) obj.IIDSECCION;

            gsa.IIDGRADO = IIDGrado;
            gsa.IIDSECCION = IIDSeccion;
            gsa.FECHA = DateTime.Now;

            try
            {
                
                using(var transaccion = new TransactionScope())
                {
                    if (gsa.IIDMATRICULA.Equals(0))
                    {
                        bd.Matricula.InsertOnSubmit(gsa);
                        bd.SubmitChanges();

                        int idMG = gsa.IIDMATRICULA;

                        var listaCursos = bd.PeriodoGradoCurso.Where(p => p.IIDPERIODO.Equals(gsa.IIDPERIODO)
                        && p.IIDGRADO.Equals(IIDGrado)).Select(p => p.IIDCURSO);

                        foreach(var curso in listaCursos)
                        {
                            DetalleMatricula dm = new DetalleMatricula();
                            dm.IIDMATRICULA = idMG;
                            dm.IIDCURSO = (int) curso;
                            dm.NOTA1 = 0;
                            dm.NOTA2 = 0;
                            dm.NOTA3 = 0;
                            dm.NOTA4 = 0;
                            dm.PROMEDIO = 0;
                            dm.bhabilitado = 1;
                            bd.DetalleMatricula.InsertOnSubmit(dm);
                        }
                        bd.SubmitChanges();
                        transaccion.Complete();
                        numRegisAfectados = 1;
                    }
                    else
                    {

                    }
                }

            }
            catch (Exception ex) { numRegisAfectados = 0; throw ex; }

            return numRegisAfectados;
        }

    }
}