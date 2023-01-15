using MIPrimeraAplicacionWeb.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MIPrimeraAplicacionWeb.Controllers
{
    public class CursoController : Controller
    {
        // GET: Curso
        public ActionResult Index()
        {
            return View();
        }
        
        public JsonResult ListarCurso()
        {
            PruebaDataContext bd = new PruebaDataContext();
            var lista = bd.Curso.Where(p => p.BHABILITADO.Equals(1))
                .Select(p => new { p.IIDCURSO, p.NOMBRE, p.DESCRIPCION }).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BuscarCursoPorNombre(string nombreCurso)
        {
            PruebaDataContext bd = new PruebaDataContext();
            var lista = bd.Curso.Where(p => p.BHABILITADO.Equals(1) && p.NOMBRE.Contains(nombreCurso))
                .Select(p => new { p.IIDCURSO, p.NOMBRE, p.DESCRIPCION }).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RecuperarDatos(int id)
        {
            PruebaDataContext bd = new PruebaDataContext();
            var lista = (bd.Curso.Where(p => p.BHABILITADO.Equals(1) && p.IIDCURSO.Equals(id))
                .Select(p => new {p.IIDCURSO, p.NOMBRE, p.DESCRIPCION})).ToList();
                
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public int GuardarDatos(Curso cursito)
        {
            PruebaDataContext bd = new PruebaDataContext();
            int numRegisAfectados = 0;

            try
            {
                if(cursito.IIDCURSO == 0)
                {
                    int numVeces = bd.Curso.Where(p => p.NOMBRE.Equals(cursito.NOMBRE)).Count();
                    if(numVeces == 0) 
                    {
                        bd.Curso.InsertOnSubmit(cursito);
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
                    int numVeces = bd.Curso.Where(p => p.NOMBRE.Equals(cursito.NOMBRE) && !p.IIDCURSO.Equals(cursito.IIDCURSO)).Count();
                    if (numVeces == 0)
                    {
                        Curso cursoSel = (bd.Curso.Where(p => p.IIDCURSO.Equals(cursito.IIDCURSO))).First();
                        cursoSel.NOMBRE = cursito.NOMBRE;
                        cursoSel.DESCRIPCION = cursito.DESCRIPCION;
                        bd.SubmitChanges();
                        numRegisAfectados = 1;
                    }
                    else
                    {
                        numRegisAfectados= -1;
                    }
                }
            }
            catch(Exception e)
            {
                numRegisAfectados = 0;
                throw e;
            }

            return numRegisAfectados;
        }

        public int EliminarCurso(Curso cursito)
        {
            PruebaDataContext bd = new PruebaDataContext();
            int numRegisAfectados = 0;
            try
            {
                Curso cursoSel = (bd.Curso.Where(p => p.IIDCURSO.Equals(cursito.IIDCURSO))).First();
                cursoSel.BHABILITADO = 0;
                bd.SubmitChanges();
                numRegisAfectados = 1;
            }
            catch(Exception e ) {numRegisAfectados = 0; throw e; }

            return numRegisAfectados;
        }
    }
}